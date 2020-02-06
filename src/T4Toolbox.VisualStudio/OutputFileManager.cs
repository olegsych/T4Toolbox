// <copyright file="OutputFileManager.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio
{
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;

    using EnvDTE;

    using EnvDTE80;

    using Microsoft.Build.Execution;
    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Shell.Interop;
    using Microsoft.VisualStudio.TextTemplating;
    using Microsoft.VisualStudio.TextTemplating.VSHost;

    using VSLangProj;

    /// <summary>
    /// Handles a single request to update output files.
    /// </summary>
    internal class OutputFileManager
    {
        private readonly DTE dte;
        private readonly ProjectItem input;
        private readonly string inputFile;
        private readonly string inputDirectory;
        private readonly OutputFile[] outputFiles;
        private readonly IDictionary<string, Project> projects;
        private readonly IAsyncServiceProvider2 serviceProvider;
        private readonly ITextTemplatingEngineHost templatingHost;

        public OutputFileManager(
            IAsyncServiceProvider2 serviceProvider,
            DTE dte,
            ITextTemplatingEngineHost textTemplatingEngineHost,
            string inputFile,
            OutputFile[] outputFiles)
        {
            this.serviceProvider = serviceProvider;
            this.dte = dte;
            this.templatingHost = textTemplatingEngineHost;
            this.inputFile = inputFile;
            this.outputFiles = outputFiles;

            this.inputDirectory = Path.GetDirectoryName(inputFile);
            this.projects = GetAllProjects(this.dte.Solution);
            this.input = this.dte.Solution.FindProjectItem(this.inputFile);
        }

        /// <summary>
        /// Executes the logic necessary to update output files.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "How else do we report an error from a background task?")]
        public async System.Threading.Tasks.Task DoWorkAsync()
        {
            try
            {
                this.DeleteOldOutputs();
                List<OutputFile> outputsToSave = this.GetOutputFilesToSave().ToList();
                await this.CheckoutFilesAsync(outputsToSave.Select(output => this.GetFullPath(output.Path)).ToArray());
                await this.SaveOutputFilesAsync(outputsToSave);
                this.ConfigureOutputFiles();
                this.RecordLastOutputs();
            }
            catch (TransformationException e)
            {
                // Expected error condition. Log message only.
                this.LogError(e.Message);
            }
            catch (Exception e)
            {
                // Unexpected error. Log the whole thing, including its callstack.
                this.LogError(e.ToString());
            }
        }

        /// <summary>
        /// Performs validation tasks that require accessing Visual Studio automation model.
        /// </summary>
        public async System.Threading.Tasks.Task ValidateAsync()
        {
            foreach (OutputFile output in this.outputFiles)
            {
                Project project;
                this.ValidateOutputProject(output, out project);
                this.ValidateOutputDirectory(output, project);
                ValidateOutputItemType(output, project);
                await this.ValidateOutputEncodingAsync(output);
                this.ValidateOutputContent(output);
            }
        }

        /// <summary>
        /// Adds projects, recursively, from the specified <paramref name="solutionItem" /> to the collection.
        /// </summary>
        private static void AddAllProjects(Project solutionItem, IDictionary<string, Project> projects)
        {
            if (solutionItem.Kind == ProjectKinds.vsProjectKindSolutionFolder)
            {
                foreach (ProjectItem item in solutionItem.ProjectItems)
                {
                    if (item.SubProject != null)
                    {
                        AddAllProjects(item.SubProject, projects);
                    }
                }
            }
            else
            {
                try
                {
                    projects.Add(solutionItem.FullName, solutionItem);
                }
                catch (NotImplementedException)
                {
                    // Ignore projects that don't support FullName property.
                }
            }
        }

        /// <summary>
        /// Adds a folder to a specified <paramref name="collection" /> of project items.
        /// </summary>
        /// <param name="collection">
        /// A <see cref="ProjectItems" /> collection that belongs to a <see cref="Project" /> or
        /// <see cref="ProjectItem" /> of type <see cref="EnvDTE.Constants.vsProjectItemKindPhysicalFolder" />.
        /// </param>
        /// <param name="folderName">
        /// Name of the folder to be added.
        /// </param>
        /// <param name="basePath">
        /// Absolute path to the directory where the folder is located.
        /// </param>
        /// <returns>
        /// A <see cref="ProjectItem" /> that represents new folder added to the <see cref="Project" />.
        /// </returns>
        /// <remarks>
        /// If the specified folder doesn't exist in the solution and the file system,
        /// a new folder will be created in both. However, if the specified folder
        /// already exists in the file system, it will be added to the solution instead.
        /// Unfortunately, an existing folder can only be added to the solution with
        /// all of sub-folders and files in it. Thus, if a single output file is
        /// generated in an existing folders not in the solution, the target folder will
        /// be added to the solution with all files in it, generated or not. The
        /// only way to avoid this would be to immediately remove all child items
        /// from a newly added existing folder. However, this could lead to having
        /// orphaned files that were added to source control and later excluded from
        /// the project. We may need to revisit this code and access <see cref="SourceControl" />
        /// automation model to remove the child items from source control too.
        /// </remarks>
        private static ProjectItem AddFolder(ProjectItems collection, string folderName, string basePath)
        {
            // Does the folder already exist in the solution?
            ProjectItem folder = collection.Cast<ProjectItem>().FirstOrDefault(p => string.Equals(p.Name, folderName, StringComparison.OrdinalIgnoreCase));
            if (folder != null)
            {
                return folder;
            }

            try
            {
                // Try adding folder to the project.
                // Note that this will work for existing folder in a Database project but not in C#.
                return collection.AddFolder(folderName);
            }
            catch (COMException)
            {
                // If folder already exists on disk and the previous attempt to add failed
                string folderPath = Path.Combine(basePath, folderName);
                if (Directory.Exists(folderPath))
                {
                    // Try adding it from disk
                    // Note that this will work in a C# but is not implemented in Database projects.
                    return collection.AddFromDirectory(folderPath);
                }

                throw;
            }
        }

        private static Exception CheckoutAbortedException()
        {
            return new TransformationException(
                "The code generation cannot be completed because one or more files that must be modified cannot be changed. " +
                "If the files are under source control, you may want to check them out; if the files are read-only on disk, " +
                "you may want to change their attributes.");
        }

        /// <summary>
        /// Configures properties, metadata and references of the <paramref name="outputItem" />.
        /// </summary>
        private static void ConfigureProjectItem(ProjectItem outputItem, OutputFile output)
        {
            ConfigureProjectItemProperties(outputItem, output);
            ConfigureProjectItemMetadata(outputItem, output);
            ConfigureProjectItemReferences(outputItem, output);
        }

        /// <summary>
        /// Sets the metadata for the <see cref="ProjectItem" /> added to solution.
        /// </summary>
        private static void ConfigureProjectItemMetadata(ProjectItem projectItem, OutputFile output)
        {
            // Set build projerties for the target project item
            foreach (KeyValuePair<string, string> metadata in output.Metadata)
            {
                // Set well-known metadata items via ProjectItem.Properties for immediate effect in Visual Studio
                switch (metadata.Key)
                {
                    case ItemMetadata.CopyToOutputDirectory:
                        projectItem.SetPropertyValue(ProjectItemProperty.CopyToOutputDirectory, output.CopyToOutputDirectory);
                        continue;

                    case ItemMetadata.CustomToolNamespace:
                        projectItem.SetPropertyValue(ProjectItemProperty.CustomToolNamespace, metadata.Value);
                        continue;

                    case ItemMetadata.Generator:
                        projectItem.SetPropertyValue(ProjectItemProperty.CustomTool, metadata.Value);
                        continue;
                }

                // Set all other metadata items
                projectItem.SetItemAttribute(metadata.Key, metadata.Value);
            }
        }

        /// <summary>
        /// Sets the known properties for the <see cref="ProjectItem" /> to be added to solution.
        /// </summary>
        private static void ConfigureProjectItemProperties(ProjectItem projectItem, OutputFile output)
        {
            if (!string.IsNullOrEmpty(output.ItemType))
            {
                projectItem.SetPropertyValue(ProjectItemProperty.ItemType, output.ItemType);
            }
        }

        /// <summary>
        /// Adds assembly references required by the project item to its containing project.
        /// </summary>
        private static void ConfigureProjectItemReferences(ProjectItem projectItem, OutputFile output)
        {
            if (output.References.Count > 0)
            {
                var project = projectItem.ContainingProject.Object as VSProject;
                if (project == null)
                {
                    throw new TransformationException(string.Format(CultureInfo.CurrentCulture, "Project {0} does not support references required by {1}", projectItem.ContainingProject.Name, projectItem.Name));
                }

                foreach (string reference in output.References)
                {
                    try
                    {
                        project.References.Add(reference);
                    }
                    catch (COMException)
                    {
                        throw new TransformationException(string.Format(CultureInfo.CurrentCulture, "Reference {0} required by {1} could not be added to project {2}", reference, projectItem.Name, projectItem.ContainingProject.Name));
                    }
                }
            }
        }

        /// <summary>
        /// Deletes the specified <paramref name="item" /> and its parent folders if they are empty.
        /// </summary>
        /// <param name="item">
        /// A Visual Studio <see cref="ProjectItem" />.
        /// </param>
        /// <remarks>
        /// This method correctly deletes empty parent folders in C# and probably
        /// Visual Basic projects which are implemented in C++ as pure COM objects.
        /// However, for Database and probably WiX projects, which are implemented
        /// as .NET COM objects, the parent collection indicates item count = 1
        /// even after its only child item is deleted. So, for new project types,
        /// this method doesn't delete empty parent folders. However, this is probably
        /// desirable for Database projects that create a predefined, empty folder
        /// structure for each schema. We may need to solve this problem in the
        /// future by recording which folders were actually created by the code
        /// generator in the log file and deleting the empty parent folders when
        /// the previously generated folders become empty.
        /// </remarks>
        private static void DeleteProjectItem(ProjectItem item)
        {
            ProjectItems parentCollection = item.Collection;

            item.Delete();

            if (parentCollection.Count == 0)
            {
                var parent = parentCollection.Parent as ProjectItem;
                if (parent != null && parent.Kind == EnvDTE.Constants.vsProjectItemKindPhysicalFolder)
                {
                    DeleteProjectItem(parent);
                }
            }
        }

        /// <summary>
        /// Retrieves projects from the specified <paramref name="solution" />.
        /// </summary>
        private static IDictionary<string, Project> GetAllProjects(Solution solution)
        {
            var projects = new Dictionary<string, Project>(StringComparer.OrdinalIgnoreCase);
            foreach (Project project in solution.Projects)
            {
                AddAllProjects(project, projects);
            }

            return projects;
        }

        /// <summary>
        /// Returns a list of item types available in the specified <paramref name="project" />.
        /// </summary>
        private static ICollection<string> GetAvailableItemTypes(Project project)
        {
            var itemTypes = new List<string> { ItemType.None, ItemType.Compile, ItemType.Content, ItemType.EmbeddedResource };

            var projectInstance = new ProjectInstance(project.FullName);
            foreach (ProjectItemInstance item in projectInstance.Items)
            {
                if (item.ItemType == "AvailableItemName")
                {
                    itemTypes.Add(item.EvaluatedInclude);
                }
            }

            return itemTypes;
        }

        private static bool IsEmptyOrWhiteSpace(StringBuilder text)
        {
            for (int i = 0; i < text.Length; i++)
            {
                if (!char.IsWhiteSpace(text[i]))
                {
                    return false;
                }
            }

            return true;
        }

        private static void ReloadDocument(IVsRunningDocumentTable runningDocumentTable, string outputFilePath)
        {
            if (runningDocumentTable == null)
            {
                // SVsRunningDocumentTable service is not available (as in a unit test).
                return;
            }

            IVsHierarchy hierarchy;
            uint itemId;
            IntPtr persistDocDataPointer;
            uint cookie;
            ErrorHandler.ThrowOnFailure(runningDocumentTable.FindAndLockDocument((uint)_VSRDTFLAGS.RDT_NoLock, outputFilePath, out hierarchy, out itemId, out persistDocDataPointer, out cookie));
            if (persistDocDataPointer == IntPtr.Zero)
            {
                // Document is not currently opened in Visual Studio editor. 
                return;
            }

            var persistDocData = (IVsPersistDocData)Marshal.GetObjectForIUnknown(persistDocDataPointer);
            ErrorHandler.ThrowOnFailure(persistDocData.ReloadDocData((uint)(_VSRELOADDOCDATA.RDD_IgnoreNextFileChange | _VSRELOADDOCDATA.RDD_RemoveUndoStack)));
        }

        /// <summary>
        /// Determines whether two project items collections are the same.
        /// </summary>
        /// <param name="collection1">
        /// First <see cref="ProjectItems" /> collection.
        /// </param>
        /// <param name="collection2">
        /// Second <see cref="ProjectItems" /> collection.
        /// </param>
        /// <returns>True, if the two collections are the same.</returns>
        /// <remarks>
        /// This method is necessary for MPF-based project implementations, such as database projects, which can return different
        /// ProjectItems instances ultimately pointing to the same folder.
        /// </remarks>
        private static bool Same(ProjectItems collection1, ProjectItems collection2)
        {
            if (collection1 == collection2)
            {
                return true;
            }

            var parentItem1 = collection1.Parent as ProjectItem;
            var parentItem2 = collection2.Parent as ProjectItem;
            if (parentItem1 != null && parentItem2 != null)
            {
                if (!string.Equals(parentItem1.Name, parentItem2.Name, StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }

                return Same(parentItem1.Collection, parentItem2.Collection);
            }

            var parentProject1 = collection1.Parent as Project;
            var parentProject2 = collection2.Parent as Project;
            if (parentProject1 != null && parentProject2 != null)
            {
                return string.Equals(parentProject1.FullName, parentProject2.FullName, StringComparison.OrdinalIgnoreCase);
            }

            return false;
        }

        private static void ValidateOutputItemType(OutputFile output, Project outputProject)
        {
            if (!string.IsNullOrEmpty(output.ItemType))
            {
                ICollection<string> itemTypes = GetAvailableItemTypes(outputProject);
                if (!itemTypes.Contains(output.ItemType))
                {
                    throw new TransformationException(string.Format(CultureInfo.CurrentCulture, "ItemType {0} specified for output file {1} is not supported for project {2}", output.ItemType, output.Path, outputProject.FullName));
                }
            }
        }

        /// <summary>
        /// Deletes output files that were not generated by the current session.
        /// </summary>
        private void DeleteOldOutputs()
        {
            string lastOutputs = this.input.GetItemAttribute(ItemMetadata.LastOutputs);

            // Delete all files recorded in the log that were not regenerated
            string[] logEntries = lastOutputs.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in logEntries)
            {
                string relativePath = line.Trim();

                // Skip blank lines
                if (relativePath.Length == 0)
                {
                    continue;
                }

                string absolutePath = this.GetFullPath(relativePath);

                // Skip the file if it was regenerated during current transformation
                if (this.outputFiles.Any(output => string.Equals(this.GetFullPath(output.Path), absolutePath, StringComparison.OrdinalIgnoreCase)))
                {
                    continue;
                }

                // The file wasn't regenerated, delete it from the solution, source control and file storage
                ProjectItem projectItem = this.dte.Solution.FindProjectItem(absolutePath);
                if (projectItem != null)
                {
                    DeleteProjectItem(projectItem);
                }
            }
        }

        /// <summary>
        /// Finds project item collection for the output file in the currently loaded Visual Studio solution.
        /// </summary>
        /// <param name="output">
        /// An <see cref="OutputFile" /> that needs to be added to the solution.
        /// </param>
        /// <returns>
        /// A <see cref="ProjectItems" /> collection where the generated file should be added.
        /// </returns>
        private ProjectItems FindProjectItemCollection(OutputFile output)
        {
            string outputFilePath = this.GetFullPath(output.Path);
            ProjectItems collection; // collection to which output file needs to be added
            string relativePath; // path from the collection to the file
            string basePath; // absolute path to the directory to which an item is being added

            if (!string.IsNullOrEmpty(output.Project))
            {
                // If output file needs to be added to another project
                Project project = this.projects[this.GetFullPath(output.Project)];
                collection = project.ProjectItems;
                relativePath = FileMethods.GetRelativePath(project.FullName, outputFilePath);
                basePath = Path.GetDirectoryName(project.FullName);
            }
            else if (!string.IsNullOrEmpty(output.Directory))
            {
                // If output file needs to be added to another folder of the current project
                collection = this.input.ContainingProject.ProjectItems;
                relativePath = FileMethods.GetRelativePath(this.input.ContainingProject.FullName, outputFilePath);
                basePath = Path.GetDirectoryName(this.input.ContainingProject.FullName);
            }
            else
            {
                // Add the output file to the list of children of the input file
                collection = this.input.ProjectItems;
                relativePath = FileMethods.GetRelativePath(this.inputFile, outputFilePath);
                basePath = Path.GetDirectoryName(this.inputFile);
            }

            // make sure that all folders in the file path exist in the project.
            if (relativePath.StartsWith("." + Path.DirectorySeparatorChar, StringComparison.Ordinal))
            {
                // Remove leading .\ from the path
                relativePath = relativePath.Substring(relativePath.IndexOf(Path.DirectorySeparatorChar) + 1);

                while (relativePath.Contains(Path.DirectorySeparatorChar))
                {
                    string folderName = relativePath.Substring(0, relativePath.IndexOf(Path.DirectorySeparatorChar));
                    ProjectItem folder = AddFolder(collection, folderName, basePath);

                    collection = folder.ProjectItems;
                    relativePath = relativePath.Substring(folderName.Length + 1);
                    basePath = Path.Combine(basePath, folderName);
                }
            }

            return collection;
        }

        private string GetFullPath(string path)
        {
            if (!Path.IsPathRooted(path))
            {
                path = Path.Combine(this.inputDirectory, path);
            }

            return Path.GetFullPath(path);
        }

        private string GetLastGenOutputFullPath()
        {
            string relativePath = this.input.GetItemAttribute(ItemMetadata.LastGenOutput);
            if (!string.IsNullOrEmpty(relativePath))
            {
                string projectDirectory = Path.GetDirectoryName(this.input.ContainingProject.FullName);
                return Path.GetFullPath(Path.Combine(projectDirectory, relativePath));
            }

            return string.Empty;
        }

        private void LogError(string message)
        {
            this.templatingHost.LogErrors(new CompilerErrorCollection { new CompilerError { ErrorText = message, FileName = this.inputFile } });
        }

        private void LogWarning(string message)
        {
            this.templatingHost.LogErrors(new CompilerErrorCollection { new CompilerError { ErrorText = message, FileName = this.inputFile, IsWarning = true } });
        }

        /// <summary>
        /// Records a list of the generated files in the LastOutputs metadata of the input item.
        /// </summary>
        private void RecordLastOutputs()
        {
            string lastGenOutputFullPath = this.GetLastGenOutputFullPath();

            // Create a list of files that may be regenerated/overwritten in the future
            var outputFileList = new List<string>();
            foreach (OutputFile output in this.outputFiles)
            {
                // Don't store the name of file user wants to preserve so that we don't deleted next time.
                if (output.PreserveExistingFile)
                {
                    continue;
                }

                // Don't store the name of default output file second time
                string outputFullPath = this.GetFullPath(output.Path);
                if (string.Equals(lastGenOutputFullPath, outputFullPath, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                // Store a relative path from the input file to the output file
                outputFileList.Add(FileMethods.GetRelativePath(this.inputFile, outputFullPath));
            }

            // If more than one output file was generated, write one file per line in alphabetical order for readability
            outputFileList.Sort();
            string lastOutputs = string.Join(Environment.NewLine, outputFileList);
            if (outputFileList.Count > 1)
            {
                lastOutputs = Environment.NewLine + lastOutputs + Environment.NewLine;
            }

            // Write the file list to the project file
            this.input.SetItemAttribute(ItemMetadata.LastOutputs, lastOutputs);
        }

        private IEnumerable<OutputFile> GetOutputFilesToSave()
        {
            foreach (OutputFile output in this.outputFiles)
            {
                string outputFilePath = this.GetFullPath(output.Path);

                // Don't do anything unless the output file has changed and needs to be overwritten
                if (File.Exists(outputFilePath))
                {
                    if (output.PreserveExistingFile || output.Content.ToString() == File.ReadAllText(outputFilePath, output.Encoding))
                    {
                        continue;
                    }
                }

                yield return output;
            }
        }

        private async System.Threading.Tasks.Task SaveOutputFilesAsync(IEnumerable<OutputFile> outputsToSave)
        {
            var runningDocumentTable = (IVsRunningDocumentTable)await this.serviceProvider.GetServiceAsync(typeof(SVsRunningDocumentTable));
            foreach (OutputFile output in outputsToSave)
            {
                string outputFilePath = this.GetFullPath(output.Path);
                Directory.CreateDirectory(Path.GetDirectoryName(outputFilePath));
                File.WriteAllText(outputFilePath, output.Content.ToString(), output.Encoding);
                ReloadDocument(runningDocumentTable, outputFilePath);
            }
        }

        /// <summary>
        /// Uses the <see cref="SVsQueryEditQuerySave"/> service to checkout specified files with a minimum number of visual prompts.
        /// </summary>
        private async System.Threading.Tasks.Task CheckoutFilesAsync(string[] filePaths)
        {
            var queryService = (IVsQueryEditQuerySave2)await this.serviceProvider.GetServiceAsync(typeof(SVsQueryEditQuerySave));
            if (queryService == null)
            {
                // SVsQueryEditQueryService is not available, don't try to check out files.
                return;
            }

            // Call QueryEditFiles to perform the action specified in the Source Control/Editing setting of the Visual Studio Options dialog.
            // Although, technically, we are not "editing" the generated files, we call this method because, unlike QuerySaveFiles, it displays 
            // a single visual prompt for all files that need to be checked out.
            uint editInfo;
            uint editResult;
            ErrorHandler.ThrowOnFailure(queryService.QueryEditFiles((uint)tagVSQueryEditFlags.QEF_DisallowInMemoryEdits, filePaths.Length, filePaths, null, null, out editResult, out editInfo));
            if (editResult == (uint)tagVSQueryEditResult.QER_EditOK)
            {
                return;
            }

            if (editResult == (uint)tagVSQueryEditResult.QER_NoEdit_UserCanceled &&
                (editInfo & (uint)tagVSQueryEditResultFlags.QER_CheckoutCanceledOrFailed) == (uint)tagVSQueryEditResultFlags.QER_CheckoutCanceledOrFailed)
            {
                throw CheckoutAbortedException();
            }

            // If QueryEditFiles did not allow us to modify the generated files, call QuerySaveFiles to perform the action specified in the 
            // Source Control/Saving setting of the Visual Studio Options dialog.            
            ErrorHandler.ThrowOnFailure(queryService.BeginQuerySaveBatch()); // Allow the user to cancel check-out-on-save for all files in the batch
            try
            {
                uint saveResult;
                ErrorHandler.ThrowOnFailure(queryService.QuerySaveFiles(0, filePaths.Length, filePaths, null, null, out saveResult));
                if (saveResult != (uint)tagVSQuerySaveResult.QSR_SaveOK)
                {
                    throw CheckoutAbortedException();
                }
            }
            finally
            {
                ErrorHandler.ThrowOnFailure(queryService.EndQuerySaveBatch());
            }
        }

        /// <summary>
        /// Saves and configures the additional output created by the transformation.
        /// </summary>
        /// <remarks>
        /// Note that this method currently cannot distinguish between files that are
        /// already in a Database project and files that are simply displayed with
        /// "Show All Files" option. Database project model makes these items appear
        /// as if they were included in the project.
        /// </remarks>
        private void ConfigureOutputFile(OutputFile output)
        {
            string outputFilePath = this.GetFullPath(output.Path);

            ProjectItem outputItem = this.dte.Solution.FindProjectItem(outputFilePath);
            ProjectItems collection = this.FindProjectItemCollection(output);

            if (outputItem == null)
            {
                // If output file has not been added to the solution
                outputItem = collection.AddFromFile(outputFilePath);
            }
            else if (!Same(outputItem.Collection, collection))
            {
                // If the output file moved from one collection to another                    
                string backupFile = outputFilePath + ".bak";
                File.Move(outputFilePath, backupFile); // Prevent unnecessary source control operations
                outputItem.Delete(); // Remove doesn't work on "DependentUpon" items
                File.Move(backupFile, outputFilePath);

                outputItem = collection.AddFromFile(outputFilePath);
            }

            ConfigureProjectItem(outputItem, output);
        }

        /// <summary>
        /// Saves output files, creates and configures project items.
        /// </summary>
        private void ConfigureOutputFiles()
        {
            foreach (OutputFile output in this.outputFiles)
            {
                this.ConfigureOutputFile(output);
            }
        }

        private void ValidateOutputContent(OutputFile output)
        {
            // If additional output file is empty, warn the user to encourage them to cleanup their code generator
            if (!string.IsNullOrEmpty(output.File) && IsEmptyOrWhiteSpace(output.Content))
            {
                this.LogWarning(string.Format(CultureInfo.CurrentCulture, "Generated output file '{0}' is empty.", output.Path));
            }
        }

        private void ValidateOutputDirectory(OutputFile output, Project outputProject)
        {
            if (!string.IsNullOrEmpty(output.Directory))
            {
                string projectPath = Path.GetDirectoryName(outputProject.FullName);
                string outputPath = this.GetFullPath(output.Path);
                if (!outputPath.StartsWith(projectPath, StringComparison.OrdinalIgnoreCase))
                {
                    throw new TransformationException(string.Format(CultureInfo.CurrentCulture, "Output file {0} is located outside of directory of target project {1}", outputPath, outputProject.FullName));
                }
            }
        }

        private async System.Threading.Tasks.Task ValidateOutputEncodingAsync(OutputFile output)
        {
            if (string.IsNullOrEmpty(output.File))
            {
                object service = await this.serviceProvider.GetServiceAsync(typeof(STextTemplating));

                // Try to change the encoding
                var host = (ITextTemplatingEngineHost)service;
                host.SetOutputEncoding(output.Encoding, false);

                // Check if the encoding was already set by the output directive and cannot be changed
                var components = (ITextTemplatingComponents)service;
                var callback = components.Callback as TextTemplatingCallback; // Callback can be provided by user code, not only by T4.
                if (callback != null && !object.Equals(callback.OutputEncoding, output.Encoding))
                {
                    throw new TransformationException(
                        string.Format(
                            CultureInfo.CurrentCulture,
                            "Encoding value {0} does not match value {1} set by the output directive.",
                            output.Encoding.EncodingName,
                            callback.OutputEncoding.EncodingName));
                }
            }
        }

        private void ValidateOutputProject(OutputFile output, out Project project)
        {
            if (string.IsNullOrEmpty(output.Project))
            {
                project = this.input.ContainingProject;
            }
            else
            {
                if (!this.projects.TryGetValue(this.GetFullPath(output.Project), out project))
                {
                    throw new TransformationException(string.Format(CultureInfo.CurrentCulture, "Target project {0} does not belong to the solution", output.Project));
                }
            }
        }
    }
}