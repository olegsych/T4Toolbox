// <copyright file="OutputFileManagerTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.IntegrationTests
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Threading;
    using EnvDTE;
    using EnvDTE80;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.VisualStudio.TextTemplating;
    using Microsoft.VisualStudio.TextTemplating.VSHost;
    
    using VSLangProj;

    [TestClass]
    public class OutputFileManagerTest : IntegrationTest
    {
        private const string TextFileItemTemplate = "Text File";
        private const string TestText = "Test Text";
        private const string WhiteSpaceText = " \t\r\n";

        private ProjectItem folder;
        private ProjectItem input;
        private ITransformationContextProvider provider;
        private OutputFile output;
        private Project project;
        private ITextTemplatingEngineHost templatingHost;
        private ITextTemplating templatingService;
        private TextTemplatingCallback templatingCallback;

        [TestInitialize]
        public void TestInitialize()
        {
            UIThreadDispatcher.Invoke(delegate
            {
                this.templatingService = (ITextTemplating)ServiceProvider.GetService(typeof(STextTemplating));
                this.templatingHost = (ITextTemplatingEngineHost)this.templatingService;
                this.provider = (ITransformationContextProvider)ServiceProvider.GetService(typeof(ITransformationContextProvider));

                this.project = this.CreateTestProject();
                this.folder = this.project.ProjectItems.AddFolder(Path.GetRandomFileName());
                this.input = this.CreateTestProjectItem(this.folder.ProjectItems, TextFileItemTemplate);

                this.output = new OutputFile { File = Path.GetRandomFileName() + ".txt" };
                this.output.Content.Append(TestText);

                this.SimulateTransformation();
            });
        }

        #region Output.File

        [TestMethod, DataSource(TargetProject.Provider, TargetProject.Connection, TargetProject.Table, DataAccessMethod.Sequential)]
        public async Task UpdateOutputFilesCreatesNewProjectItemWithSpecifiedFile()
        {
            await UIThreadDispatcher.Invoke<Task>(async delegate
            {
                this.provider.UpdateOutputFiles(this.input.FileNames[1], new[] { this.output });
                await this.SimulateCustomTool();
                
                ProjectItem outputItem = this.input.ProjectItems.Cast<ProjectItem>().Single(item => item.Name == this.output.File);
                Assert.AreEqual(this.output.Content.ToString(), File.ReadAllText(outputItem.FileNames[1]));
            });
        }

        [TestMethod, DataSource(TargetProject.Provider, TargetProject.Connection, TargetProject.Table, DataAccessMethod.Sequential)]
        public async Task UpdateOutputFilesDeletesPreviouslyCreatedFileThatWasNotRecreated()
        {
            await UIThreadDispatcher.Invoke<Task>(async delegate
            {
                this.provider.UpdateOutputFiles(this.input.FileNames[1], new[] { this.output });
                await this.SimulateCustomTool();

                this.provider.UpdateOutputFiles(this.input.FileNames[1], new OutputFile[0]);
                await this.SimulateCustomTool();

                Assert.IsFalse(this.input.ProjectItems.Cast<ProjectItem>().Any(item => item.Name == this.output.File));
            });
        }

        #endregion

        #region Output.Directory
        //// Note: Without Output.Project, Output.File and Output.Directory are relative to input file location.

        [TestMethod, DataSource(TargetProject.Provider, TargetProject.Connection, TargetProject.Table, DataAccessMethod.Sequential)]
        public async Task UpdateOutputFilesCreatesNewProjectItemInDirectorySpecifiedAsPathRelativeToInputFile()
        {
            await UIThreadDispatcher.Invoke<Task>(async delegate
            {
                this.output.Directory = Path.GetRandomFileName();                
                this.provider.UpdateOutputFiles(this.input.FileNames[1], new[] { this.output });
                await this.SimulateCustomTool();

                ProjectItem outputFolder = this.folder.ProjectItems.Cast<ProjectItem>().Single(item => item.Name == this.output.Directory);
                Assert.IsTrue(outputFolder.ProjectItems.Cast<ProjectItem>().Any(item => item.Name == this.output.File));
            });
        }

        [TestMethod, DataSource(TargetProject.Provider, TargetProject.Connection, TargetProject.Table, DataAccessMethod.Sequential)]
        public async Task UpdateOutputFilesCreatesNewProjectItemInDirectorySpecifiedAsAbsolutePath()
        {
            await UIThreadDispatcher.Invoke<Task>(async delegate
            {
                string folderName = Path.GetRandomFileName();
                this.output.Directory = Path.Combine(Path.GetDirectoryName(this.project.FullName), folderName);          
                this.provider.UpdateOutputFiles(this.input.FileNames[1], new[] { this.output });
                await this.SimulateCustomTool();

                ProjectItem outputFolder = this.project.ProjectItems.Cast<ProjectItem>().Single(item => item.Name == folderName);
                Assert.IsTrue(outputFolder.ProjectItems.Cast<ProjectItem>().Any(item => item.Name == this.output.File));
            });
        }

        [TestMethod, ExpectedException(typeof(TransformationException))]
        [DataSource(TargetProject.Provider, TargetProject.Connection, TargetProject.Table, DataAccessMethod.Sequential)]
        public async Task UpdateOutputFilesThrowsTransformationExceptionWhenAbsoluteDirectoryIsOutsideOfProject()
        {
            await UIThreadDispatcher.InvokeAsync(delegate
            {
                this.output.Directory = Directory.GetParent(this.project.FullName).Parent.FullName;
                this.provider.UpdateOutputFiles(this.input.FileNames[1], new[] { this.output });
            });
        }

        [TestMethod, ExpectedException(typeof(TransformationException))]
        [DataSource(TargetProject.Provider, TargetProject.Connection, TargetProject.Table, DataAccessMethod.Sequential)]
        public async Task UpdateOutputFilesThrowsTransformationExceptionWhenRelativeDirectoryIsOutsideOfProject()
        {
            await UIThreadDispatcher.InvokeAsync(delegate
            {
                this.output.Directory = @"..\..";
                this.provider.UpdateOutputFiles(this.input.FileNames[1], new[] { this.output });
            });
        }

        [TestMethod, DataSource(TargetProject.Provider, TargetProject.Connection, TargetProject.Table, DataAccessMethod.Sequential)]
        public async Task UpdateOutputFilesDeletesPreviouslyCreatedDirectoryWhereFilesWereNotRecreated()
        {
            await UIThreadDispatcher.Invoke<Task>(async delegate
            {
                this.output.Directory = Path.GetRandomFileName();

                this.provider.UpdateOutputFiles(this.input.FileNames[1], new[] { this.output });
                await this.SimulateCustomTool();

                this.provider.UpdateOutputFiles(this.input.FileNames[1], new OutputFile[0]);
                await this.SimulateCustomTool();

                Assert.IsFalse(this.folder.ProjectItems.Cast<ProjectItem>().Any(item => item.Name == this.output.Directory));
            });
        }

        [TestMethod, DataSource(TargetProject.Provider, TargetProject.Connection, TargetProject.Table, DataAccessMethod.Sequential)]
        public async Task UpdateOutputFilesMovesPreviouslyCreatedItemToTheSpecifiedDirectory()
        {
            await UIThreadDispatcher.Invoke<Task>(async delegate
            {
                // Create output file nested under the input file
                this.provider.UpdateOutputFiles(this.input.FileNames[1], new[] { this.output });
                await this.SimulateCustomTool();

                // Now regenerate the file, this time specifying Directory
                this.output.Directory = Path.GetDirectoryName(this.input.FileNames[1]);
                this.provider.UpdateOutputFiles(this.input.FileNames[1], new[] { this.output });
                await this.SimulateCustomTool();

                // The output file path hasn't changed, however, it should be moved from the input's collection to the folder's collection
                Assert.IsTrue(this.folder.ProjectItems.Cast<ProjectItem>().Any(item => item.Name == this.output.File));
            });
        }

        #endregion

        [TestMethod, DataSource(TargetProject.Provider, TargetProject.Connection, TargetProject.Table, DataAccessMethod.Sequential)]
        public async Task UpdateOutputFilesStoresRelativePathsOfGeneratedFiles()
        {
            await UIThreadDispatcher.Invoke<Task>(async delegate
            {
                this.output.Directory = Path.GetRandomFileName();
                this.provider.UpdateOutputFiles(this.input.FileNames[1], new[] { this.output });
                await this.SimulateCustomTool();

                string lastGenOutput = this.input.GetItemAttribute(ItemMetadata.LastOutputs);
                Assert.AreEqual(this.output.Path, lastGenOutput.TrimStart('.', '\\'));
                Assert.IsFalse(Path.IsPathRooted(lastGenOutput));
            });
        }

        [TestMethod, DataSource(TargetProject.Provider, TargetProject.Connection, TargetProject.Table, DataAccessMethod.Sequential)]
        public async Task UpdateOutputFilesDoesNotStoreNameOfDefaultOutputTwice()
        {
            await UIThreadDispatcher.Invoke<Task>(async delegate
            {
                this.output.File = string.Empty;
                this.provider.UpdateOutputFiles(this.input.FileNames[1], new[] { this.output });
                await this.SimulateCustomTool();
                Assert.AreEqual(string.Empty, this.input.GetItemAttribute(ItemMetadata.LastOutputs));
            });            
        }

        #region Output.Project
        //// Note: With Output.Project specified, Output.File and Output.Directory are relative to the target project location.

        [TestMethod, DataSource(TargetProject.Provider, TargetProject.Connection, TargetProject.Table, DataAccessMethod.Sequential)]
        public async Task UpdateOutputFilesCreatesNewProjectItemInProjectSpecifiedAsAbsolutePath()
        {
            await UIThreadDispatcher.Invoke<Task>(async delegate
            {
                this.output.Project = this.project.FullName;
                this.provider.UpdateOutputFiles(this.input.FileNames[1], new[] { this.output });
                await this.SimulateCustomTool();
                Assert.IsTrue(this.project.ProjectItems.Cast<ProjectItem>().Any(item => item.Name == this.output.File));
            });
        }

        [TestMethod, DataSource(TargetProject.Provider, TargetProject.Connection, TargetProject.Table, DataAccessMethod.Sequential)]
        public async Task UpdateOutputFilesCreatesNewProjectItemInProjectSpecifiedAsPathRelativeToInputFile()
        {
            await UIThreadDispatcher.Invoke<Task>(async delegate
            {
                this.output.Project = FileMethods.GetRelativePath(this.input.FileNames[1], this.project.FullName);
                this.provider.UpdateOutputFiles(this.input.FileNames[1], new[] { this.output });
                await this.SimulateCustomTool();
                Assert.IsTrue(this.project.ProjectItems.Cast<ProjectItem>().Any(item => item.Name == this.output.File));
            });
        }
        
        [TestMethod, DataSource(TargetProject.Provider, TargetProject.Connection, TargetProject.Table, DataAccessMethod.Sequential)]
        public async Task UpdateOutputFilesCreatesNewProjectItemInAbsoluteDirectoryOfTargetProject()
        {
            await UIThreadDispatcher.Invoke<Task>(async delegate
            {
                string directoryName = Path.GetRandomFileName();
                this.output.Directory = Path.Combine(Path.GetDirectoryName(this.project.FullName), directoryName);
                this.output.Project = this.project.FullName;

                this.provider.UpdateOutputFiles(this.input.FileNames[1], new[] { this.output });
                await this.SimulateCustomTool();

                ProjectItem outputFolder = this.project.ProjectItems.Cast<ProjectItem>().Single(item => item.Name == directoryName);
                Assert.IsTrue(outputFolder.ProjectItems.Cast<ProjectItem>().Any(item => item.Name == this.output.File));
            });
        }

        [TestMethod, DataSource(TargetProject.Provider, TargetProject.Connection, TargetProject.Table, DataAccessMethod.Sequential)]
        public async Task UpdateOutputFilesCreatesNewProjectItemInRelativeDirectoryOfTargetProject()
        {
            await UIThreadDispatcher.Invoke<Task>(async delegate
            {
                this.output.Directory = Path.GetRandomFileName();
                this.output.Project = this.project.FullName;
                this.provider.UpdateOutputFiles(this.input.FileNames[1], new[] { this.output });
                await this.SimulateCustomTool();

                ProjectItem outputFolder = this.project.ProjectItems.Cast<ProjectItem>().Single(item => item.Name == this.output.Directory);
                Assert.IsTrue(outputFolder.ProjectItems.Cast<ProjectItem>().Any(item => item.Name == this.output.File));
            });
        }

        [TestMethod, ExpectedException(typeof(TransformationException))]
        [DataSource(TargetProject.Provider, TargetProject.Connection, TargetProject.Table, DataAccessMethod.Sequential)]
        public async Task UpdateOutputFilesThrowsTransformationExceptionWhenAbsoluteDirectoryIsOutsideOfTargetProject()
        {
            await UIThreadDispatcher.InvokeAsync(delegate
            {
                this.output.Directory = Directory.GetParent(this.project.FullName).Parent.FullName;
                this.output.Project = this.project.FullName;
                this.provider.UpdateOutputFiles(this.input.FileNames[1], new[] { this.output });
            });
        }

        [TestMethod, ExpectedException(typeof(TransformationException))]
        [DataSource(TargetProject.Provider, TargetProject.Connection, TargetProject.Table, DataAccessMethod.Sequential)]
        public async Task UpdateOutputFilesThrowsTransformationExceptionWhenRelativeDirectoryIsOutsideOfTargetProject()
        {
            await UIThreadDispatcher.InvokeAsync(delegate
            {
                this.output.Directory = "..";
                this.output.Project = this.project.FullName;
                this.provider.UpdateOutputFiles(this.input.FileNames[1], new[] { this.output });
            });
        }

        [TestMethod, ExpectedException(typeof(TransformationException))]
        [DataSource(TargetProject.Provider, TargetProject.Connection, TargetProject.Table, DataAccessMethod.Sequential)]
        public async Task UpdateOutputFilesThrowsTransformationExceptionWhenSpecifiedProjectIsNotInTheSolution()
        {
            await UIThreadDispatcher.InvokeAsync(delegate
            {
                this.output.Project = "Invalid.proj";
                this.provider.UpdateOutputFiles(this.input.FileNames[1], new[] { this.output });
            });
        }

        [TestMethod, DataSource(TargetProject.Provider, TargetProject.Connection, TargetProject.Table, DataAccessMethod.Sequential)]
        public async Task UpdateOutputFilesCreatesNewProjectItemInTargetProjectLocatedInSolutionFolder()
        {
            await UIThreadDispatcher.Invoke<Task>(async delegate
            {
                // Create a new solution folder
                Project solutionFolderProject = Solution.AddSolutionFolder(Path.GetRandomFileName());
                var solutionFolder = (SolutionFolder)solutionFolderProject.Object;

                // Create a new project in the solution folder
                string projectTemplate = Solution.GetProjectTemplate(this.TargetProject.Template, this.TargetProject.Language);
                string projectName = Path.GetRandomFileName();
                string projectFolder = Path.Combine(SolutionDirectory, projectName);
                solutionFolder.AddFromTemplate(projectTemplate, projectFolder, projectName);
                Project project = solutionFolderProject.ProjectItems.Item(1).SubProject;

                // Generate output file in the new project
                this.output.Project = project.FullName;
                this.provider.UpdateOutputFiles(this.input.FileNames[1], new[] { this.output });
                await this.SimulateCustomTool();
                Assert.IsTrue(project.ProjectItems.Cast<ProjectItem>().Any(item => item.Name == this.output.File));
            });
        }

        [Ignore] // It appears in Visual Studio 2013 behavior of Solution.AddFromTemplate changed to prevent creation of multiple projects in the same folder
        [TestMethod, DataSource(TargetProject.Provider, TargetProject.Connection, TargetProject.Table, DataAccessMethod.Sequential)]
        public async Task UpdateOutputFilesMovesPreviouslyCreatedItemToTargetProject()
        {
            await UIThreadDispatcher.Invoke<Task>(async delegate
            {
                // Generate output file in the source project
                Project sourceProject = this.input.ContainingProject;
                this.output.Project = sourceProject.FullName;
                this.output.Directory = this.folder.FileNames[1];
                this.provider.UpdateOutputFiles(this.input.FileNames[1], new[] { this.output });
                await this.SimulateCustomTool();

                // Create a new project in the same directory
                string projectTemplate = Solution.GetProjectTemplate(this.TargetProject.Template, this.TargetProject.Language);
                string projectName = Path.GetRandomFileName();
                string projectFolder = Path.GetDirectoryName(sourceProject.FullName);
                Solution.AddFromTemplate(projectTemplate, projectFolder, projectName);
                Project targetProject = Solution.Projects.Cast<Project>().Single(project => project.Name == projectName);

                // Re-generate output in the target project
                this.output.Project = targetProject.FullName;
                this.provider.UpdateOutputFiles(this.input.FileNames[1], new[] { this.output });
                await this.SimulateCustomTool();

                // Verify that the output file was moved from the source project to the target project
                Assert.IsFalse(this.folder.ProjectItems.Cast<ProjectItem>().Any(item => item.Name == this.output.File));
                ProjectItem targetFolder = targetProject.ProjectItems.Cast<ProjectItem>().Single(item => item.Name == this.folder.Name);
                Assert.IsTrue(targetFolder.ProjectItems.Cast<ProjectItem>().Any(item => item.Name == this.output.File));
            });
        }

        #endregion

        #region Output.ItemType

        [TestMethod, DataSource(TargetProject.Provider, TargetProject.Connection, TargetProject.Table, DataAccessMethod.Sequential)]
        public async Task UpdateOutputFilesCreatesProjectItemWithSpecifiedItemType()
        {
            await UIThreadDispatcher.Invoke<Task>(async delegate
            {
                this.output.ItemType = ItemType.EmbeddedResource;
                this.provider.UpdateOutputFiles(this.input.FileNames[1], new[] { this.output });
                await this.SimulateCustomTool();

                ProjectItem outputItem = this.input.ProjectItems.Cast<ProjectItem>().Single(item => item.Name == this.output.File);
                Assert.AreEqual(this.output.ItemType, outputItem.Properties.Item(ProjectItemProperty.ItemType).Value);
            });
        }

        [TestMethod, DataSource(TargetProject.Provider, TargetProject.Connection, TargetProject.Table, DataAccessMethod.Sequential)]
        public async Task UpdateOutputFilesConfiguresDefaultOutputItemWithSpecifiedItemType()
        {
            await UIThreadDispatcher.Invoke<Task>(async delegate
            {
                this.output.File = string.Empty;
                this.output.ItemType = ItemType.Content;
                this.provider.UpdateOutputFiles(this.input.FileNames[1], new[] { this.output });

                ProjectItem outputItem = await this.SimulateCustomTool();
                Assert.AreEqual(this.output.ItemType, outputItem.Properties.Item(ProjectItemProperty.ItemType).Value);
            });
        }

        [TestMethod, ExpectedException(typeof(TransformationException))]
        [DataSource(TargetProject.Provider, TargetProject.Connection, TargetProject.Table, DataAccessMethod.Sequential)]
        public async Task UpdateOutputFilesThrowsTransformationExceptionWhenSpecifiedItemTypeIsNotAvailable()
        {
            await UIThreadDispatcher.InvokeAsync(delegate
            {
                this.output.ItemType = "InvalidItemType";
                this.provider.UpdateOutputFiles(this.input.FileNames[1], new[] { this.output });
            });
        }

        #endregion

        #region Output.CopyToOutputDirectory

        [TestMethod, DataSource(TargetProject.Provider, TargetProject.Connection, TargetProject.Table, DataAccessMethod.Sequential)]
        public async Task UpdateOutputFilesCreatesProjectItemWithSpecifiedCopyToOutputDirectory()
        {
            await UIThreadDispatcher.Invoke<Task>(async delegate
            {
                this.output.CopyToOutputDirectory = CopyToOutputDirectory.CopyAlways;
                this.provider.UpdateOutputFiles(this.input.FileNames[1], new[] { this.output });
                await this.SimulateCustomTool();

                ProjectItem outputItem = this.input.ProjectItems.Cast<ProjectItem>().Single(item => item.Name == this.output.File);
                Assert.AreEqual((uint)this.output.CopyToOutputDirectory, outputItem.Properties.Item(ProjectItemProperty.CopyToOutputDirectory).Value);
            });
        }

        [TestMethod, DataSource(TargetProject.Provider, TargetProject.Connection, TargetProject.Table, DataAccessMethod.Sequential)]
        public async Task UpdateOutputFilesConfiguresDefaultOutputItemWithSpecifiedCopyToOutputDirectory()
        {
            await UIThreadDispatcher.Invoke<Task>(async delegate
            {
                this.output.File = string.Empty;
                this.output.CopyToOutputDirectory = CopyToOutputDirectory.CopyAlways;
                this.provider.UpdateOutputFiles(this.input.FileNames[1], new[] { this.output });

                ProjectItem outputItem = await this.SimulateCustomTool();
                Assert.AreEqual((uint)this.output.CopyToOutputDirectory, outputItem.Properties.Item(ProjectItemProperty.CopyToOutputDirectory).Value);
            });
        }

        #endregion

        #region Output.CustomTool

        [TestMethod, DataSource(TargetProject.Provider, TargetProject.Connection, TargetProject.Table, DataAccessMethod.Sequential)]
        public async Task UpdateOutputFilesCreatesProjectItemWithSpecifiedCustomTool()
        {
            await UIThreadDispatcher.Invoke<Task>(async delegate
            {
                this.output.CustomTool = "TextTemplatingFilePreprocessor";
                this.provider.UpdateOutputFiles(this.input.FileNames[1], new[] { this.output });
                await this.SimulateCustomTool();

                ProjectItem outputItem = this.input.ProjectItems.Cast<ProjectItem>().Single(item => item.Name == this.output.File);
                Assert.AreEqual(this.output.CustomTool, outputItem.Properties.Item(ProjectItemProperty.CustomTool).Value);
            });
        }

        [TestMethod, DataSource(TargetProject.Provider, TargetProject.Connection, TargetProject.Table, DataAccessMethod.Sequential)]
        public async Task UpdateOutputFilesConfiguresDefaultOutputItemWithSpecifiedCustomTool()
        {
            await UIThreadDispatcher.Invoke<Task>(async delegate
            {
                this.output.File = string.Empty;
                this.output.CustomTool = "TextTemplatingFilePreprocessor";
                this.provider.UpdateOutputFiles(this.input.FileNames[1], new[] { this.output });

                ProjectItem outputItem = await this.SimulateCustomTool();
                Assert.AreEqual(this.output.CustomTool, outputItem.Properties.Item(ProjectItemProperty.CustomTool).Value);
            });
        }

        #endregion 

        #region Output.CustomToolNamespace

        [TestMethod, DataSource(TargetProject.Provider, TargetProject.Connection, TargetProject.Table, DataAccessMethod.Sequential)]
        public async Task UpdateOutputFilesCreatesProjectItemWithSpecifiedCustomToolNamespace()
        {
            await UIThreadDispatcher.Invoke<Task>(async delegate
            {
                this.output.CustomToolNamespace = "T4Toolbox";
                this.provider.UpdateOutputFiles(this.input.FileNames[1], new[] { this.output });
                await this.SimulateCustomTool();

                ProjectItem outputItem = this.input.ProjectItems.Cast<ProjectItem>().Single(item => item.Name == this.output.File);
                Assert.AreEqual(this.output.CustomToolNamespace, outputItem.Properties.Item(ProjectItemProperty.CustomToolNamespace).Value);
            });
        }

        [TestMethod, DataSource(TargetProject.Provider, TargetProject.Connection, TargetProject.Table, DataAccessMethod.Sequential)]
        public async Task UpdateOutputFilesConfiguresDefaultOutputItemWithSpecifiedCustomToolNamespace()
        {
            await UIThreadDispatcher.Invoke<Task>(async delegate
            {
                this.output.File = string.Empty;
                this.output.CustomToolNamespace = "T4Toolbox";
                this.provider.UpdateOutputFiles(this.input.FileNames[1], new[] { this.output });

                ProjectItem outputItem = await this.SimulateCustomTool();
                Assert.AreEqual(this.output.CustomToolNamespace, outputItem.Properties.Item(ProjectItemProperty.CustomToolNamespace).Value);
            });
        }

        #endregion

        #region Output.Encoding

        [TestMethod, DataSource(TargetProject.Provider, TargetProject.Connection, TargetProject.Table, DataAccessMethod.Sequential)]
        public async Task UpdateOutputFilesCreatesProjectItemWithSpecifiedEncoding()
        {
            await UIThreadDispatcher.Invoke<Task>(async delegate
            {
                this.output.Encoding = Encoding.UTF32;
                this.provider.UpdateOutputFiles(this.input.FileNames[1], new[] { this.output });
                await this.SimulateCustomTool();

                ProjectItem outputItem = this.input.ProjectItems.Cast<ProjectItem>().Single(item => item.Name == this.output.File);
                byte[] preamble = this.output.Encoding.GetPreamble();
                byte[] contents = File.ReadAllBytes(outputItem.FileNames[1]);
                Assert.IsTrue(preamble.Length > 0);
                CollectionAssert.AreEqual(preamble, contents.Take(preamble.Length).ToArray());
            });
        }

        [TestMethod, DataSource(TargetProject.Provider, TargetProject.Connection, TargetProject.Table, DataAccessMethod.Sequential)]
        public async Task UpdateOutputFilesChangesDefaultOutputItemToSpecifiedEncoding()
        {
            await UIThreadDispatcher.InvokeAsync(delegate
            {
                this.output.File = string.Empty;
                this.output.Encoding = Encoding.UTF32;
                this.provider.UpdateOutputFiles(this.input.FileNames[1], new[] { this.output });
                Assert.AreEqual(this.output.Encoding, this.templatingCallback.OutputEncoding);
            });
        }

        [TestMethod, ExpectedException(typeof(TransformationException))]
        [DataSource(TargetProject.Provider, TargetProject.Connection, TargetProject.Table, DataAccessMethod.Sequential)]
        public async Task UpdateOutputFilesThrowsTransformationExceptionWhenEncodingWasSetByOutputDirectoryAndSpecified()
        {
            await UIThreadDispatcher.InvokeAsync(delegate
            {
                // Default output encoding was set by an <#@ output #> directive
                this.templatingHost.SetOutputEncoding(Encoding.ASCII, true);

                this.output.File = string.Empty;
                this.output.Encoding = Encoding.UTF32;
                this.provider.UpdateOutputFiles(this.input.FileNames[1], new[] { this.output });
            });
        }

        #endregion

        #region Output.References

        [TestMethod, DataSource(TargetProject.Provider, TargetProject.Connection, TargetProject.Table, DataAccessMethod.Sequential)]
        public async Task UpdateOutputFilesConfiguresReferencesOfOutputItem()
        {
            await UIThreadDispatcher.Invoke<Task>(async delegate
            {
                this.output.References.Add("System.Web");
                this.provider.UpdateOutputFiles(this.input.FileNames[1], new[] { this.output });
                await this.SimulateCustomTool();

                var project = (VSProject)this.project.Object;
                Reference reference = project.References.Find(this.output.References.Single());
                Assert.IsNotNull(reference);
                reference.Remove();
            });
        }

        [TestMethod, DataSource(TargetProject.Provider, TargetProject.Connection, TargetProject.Table, DataAccessMethod.Sequential)]
        public async Task UpdateOutputFilesConfiguresReferencesOfDefaultOutputItem()
        {
            await UIThreadDispatcher.Invoke<Task>(async delegate
            {
                this.output.File = string.Empty;
                this.output.References.Add("System.Web");
                this.provider.UpdateOutputFiles(this.input.FileNames[1], new[] { this.output });
                await this.SimulateCustomTool();

                var project = (VSProject)this.project.Object;
                Reference reference = project.References.Find(this.output.References.Single());
                Assert.IsNotNull(reference);
                reference.Remove();
            });
        }

        [TestMethod, DataSource(TargetProject.Provider, TargetProject.Connection, TargetProject.Table, DataAccessMethod.Sequential)]
        public async Task UpdateOutputFilesReportsErrorWhenReferenceCannotBeAdded()
        {
            await UIThreadDispatcher.Invoke<Task>(async delegate
            {
                this.output.References.Add("InvalidAssemblyName");
                this.provider.UpdateOutputFiles(this.input.FileNames[1], new[] { this.output });
                await this.SimulateCustomTool();

                ErrorItem error = IntegrationTest.ErrorItems.Single(item => item.FileName == this.input.FileNames[1]);
                StringAssert.Contains(error.Description, this.output.References.Single());
            });
        }

        #endregion

        #region Output.Metadata

        [TestMethod, DataSource(TargetProject.Provider, TargetProject.Connection, TargetProject.Table, DataAccessMethod.Sequential)]
        public async Task UpdateOutputFilesCreatesProjectItemWithSpecifiedMetadata()
        {
            await UIThreadDispatcher.Invoke<Task>(async delegate
            {
                this.output.Metadata["Marco"] = "Polo";
                this.provider.UpdateOutputFiles(this.input.FileNames[1], new[] { this.output });
                await this.SimulateCustomTool();

                ProjectItem outputItem = this.input.ProjectItems.Cast<ProjectItem>().Single(item => item.Name == this.output.File);
                Assert.AreEqual("Polo", outputItem.GetItemAttribute("Marco"));
            });
        }

        [TestMethod, DataSource(TargetProject.Provider, TargetProject.Connection, TargetProject.Table, DataAccessMethod.Sequential)]
        public async Task UpdateOutputFilesCreatesDefaultOutputItemWithSpecifiedMetadata()
        {
            await UIThreadDispatcher.Invoke<Task>(async delegate
            {
                this.output.File = string.Empty;
                this.output.Metadata["Marco"] = "Polo";
                this.provider.UpdateOutputFiles(this.input.FileNames[1], new[] { this.output });

                ProjectItem outputItem = await this.SimulateCustomTool();
                Assert.AreEqual("Polo", outputItem.GetItemAttribute("Marco"));
            });
        }

        #endregion

        #region Output.PreserveExistingFile

        [TestMethod, DataSource(TargetProject.Provider, TargetProject.Connection, TargetProject.Table, DataAccessMethod.Sequential)]
        public async Task UpdateOutputFilesDoesNotOverwriteExistingFileWhenPreserveExistingFileIsTrue()
        {
            await UIThreadDispatcher.Invoke<Task>(async delegate
            {
                const string ExistingContent = "Existing Content";
                string outputFilePath = Path.Combine(Path.GetDirectoryName(this.input.FileNames[1]), this.output.File); 
                File.WriteAllText(outputFilePath, ExistingContent);

                this.output.PreserveExistingFile = true;
                this.provider.UpdateOutputFiles(this.input.FileNames[1], new[] { this.output });
                await this.SimulateCustomTool();

                ProjectItem outputItem = this.input.ProjectItems.Cast<ProjectItem>().Single(item => item.Name == this.output.File);
                Assert.AreEqual(ExistingContent, File.ReadAllText(outputItem.FileNames[1]));
            });
        }

        [TestMethod, DataSource(TargetProject.Provider, TargetProject.Connection, TargetProject.Table, DataAccessMethod.Sequential)]
        public async Task UpdateOutputFilesDoesNotDeletePreviouslyCreatedFileWhenPreserveExistingFileIsTrue()
        {
            await UIThreadDispatcher.Invoke<Task>(async delegate
            {
                this.output.PreserveExistingFile = true;
                this.provider.UpdateOutputFiles(this.input.FileNames[1], new[] { this.output });
                await this.SimulateCustomTool();

                this.provider.UpdateOutputFiles(this.input.FileNames[1], new OutputFile[0]);
                await this.SimulateCustomTool();

                Assert.IsTrue(this.input.ProjectItems.Cast<ProjectItem>().Any(item => item.Name == this.output.File));
            });
        }

        #endregion

        #region Default Output File

        [TestMethod, DataSource(TargetProject.Provider, TargetProject.Connection, TargetProject.Table, DataAccessMethod.Sequential)]
        public async Task UpdateOutputFilesConfiguresDefaultOutputCreatedAfterTransformation()
        {
            await UIThreadDispatcher.Invoke<Task>(async delegate
            {
                this.output.File = string.Empty;
                this.output.ItemType = ItemType.Content;
                this.provider.UpdateOutputFiles(this.input.FileNames[1], new[] { this.output });
                ProjectItem outputItem = await this.SimulateCustomTool();
                Assert.AreEqual(this.output.ItemType, outputItem.Properties.Item(ProjectItemProperty.ItemType).Value);
            });
        }

        [TestMethod, DataSource(TargetProject.Provider, TargetProject.Connection, TargetProject.Table, DataAccessMethod.Sequential)]
        public async Task UpdateOutputFilesConfiguresDefaultOutputCreatedBeforeTransformation()
        {
            await UIThreadDispatcher.Invoke<Task>(async delegate
            {
                // Default output was created by previous transformation
                ProjectItem outputItem = await this.SimulateCustomTool();

                // New transformation generates the file with the same extension
                this.SimulateTransformation();
                this.output.File = string.Empty;
                this.output.ItemType = ItemType.EmbeddedResource;
                this.provider.UpdateOutputFiles(this.input.FileNames[1], new[] { this.output });
                await this.SimulateCustomTool();

                // Verify that output item was configured
                Assert.AreEqual(this.output.ItemType, outputItem.Properties.Item(ProjectItemProperty.ItemType).Value);
            });
        }

        [TestMethod, DataSource(TargetProject.Provider, TargetProject.Connection, TargetProject.Table, DataAccessMethod.Sequential)]
        public async Task UpdateOutputFilesConfiguresDefaultOutputRecreatedAfterTransformation()
        {
            await UIThreadDispatcher.Invoke<Task>(async delegate
            {
                // Default output was created by previous transformation
                await this.SimulateCustomTool();

                // New transformation changes extension of the default output
                this.SimulateTransformation();
                this.templatingHost.SetFileExtension("new");

                this.output.File = string.Empty;
                this.output.ItemType = ItemType.Content;
                this.provider.UpdateOutputFiles(this.input.FileNames[1], new[] { this.output });
                ProjectItem outputItem = await this.SimulateCustomTool();

                // Verify that output item was configured
                Assert.AreEqual(this.output.ItemType, outputItem.Properties.Item(ProjectItemProperty.ItemType).Value);
            });
        }

        #endregion

        #region Empty Output Files

        [TestMethod, DataSource(TargetProject.Provider, TargetProject.Connection, TargetProject.Table, DataAccessMethod.Sequential)]
        public async Task UpdateOutputFilesGeneratesWarningWhenAdditionalOutputFileIsEmpty() // To encourage developer to cleanup their code generator
        {
            await UIThreadDispatcher.Invoke<Task>(async delegate
            {
                this.output.Content.Clear();
                this.output.Content.Append(WhiteSpaceText);
                this.provider.UpdateOutputFiles(this.input.FileNames[1], new[] { this.output });
                await this.SimulateCustomTool();
                ErrorItem warning = ErrorItems.Single(item => item.FileName == this.input.FileNames[1]);
                StringAssert.Contains(warning.Description, this.output.Path);
                Assert.AreEqual(vsBuildErrorLevel.vsBuildErrorLevelMedium, warning.ErrorLevel);
            });
        }

        [TestMethod, DataSource(TargetProject.Provider, TargetProject.Connection, TargetProject.Table, DataAccessMethod.Sequential)]
        public async Task UpdateOutputFilesDoesNotGenerateWarningWhenDefaultOutputFileIsEmpty() // Because there may be nothing developer can do about it
        {
            await UIThreadDispatcher.Invoke<Task>(async delegate
            {
                this.output.File = string.Empty;
                this.output.Content.Clear();
                this.output.Content.Append(WhiteSpaceText);
                this.provider.UpdateOutputFiles(this.input.FileNames[1], new[] { this.output });
                await this.SimulateCustomTool();
                Assert.IsFalse(ErrorItems.Any(item => item.FileName == this.input.FileNames[1]));
            });
        }

        #endregion 

        [TestMethod, DataSource(TargetProject.Provider, TargetProject.Connection, TargetProject.Table, DataAccessMethod.Sequential)]
        public async Task UpdateOutputFilesAutomaticallyReloadsOpenDocumentsFromDisk()
        {
            await UIThreadDispatcher.Invoke<Task>(async delegate
            {
                // Output file already exists and it is opened in text editor
                string outputFilePath = Path.Combine(Path.GetDirectoryName(this.input.FileNames[1]), this.output.File);
                ProjectItem outputItem = IntegrationTest.CreateTestProjectItemFromFile(this.input.ProjectItems, outputFilePath);
                Window window = outputItem.Open(Constants.vsViewKindTextView);

                // Code generation occurs
                this.provider.UpdateOutputFiles(this.input.FileNames[1], new[] { this.output });
                await this.SimulateCustomTool();

                // Verify that document text has been reloads generated document from disk
                var textDocument = (TextDocument)window.Document.Object();
                textDocument.Selection.StartOfDocument(Extend: true);
                textDocument.Selection.EndOfDocument(Extend: true);
                Assert.AreEqual(this.output.Content.ToString(), textDocument.Selection.Text);
            });            
        }

        private async Task<ProjectItem> SimulateCustomTool()
        {
            string outputFileName = Path.Combine(Path.GetDirectoryName(this.project.FullName), this.input.GetItemAttribute(ItemMetadata.LastGenOutput));
            ProjectItem outputItem = this.input.ProjectItems.Cast<ProjectItem>().SingleOrDefault(item => item.FileNames[1] == outputFileName);
            if (outputItem != null)
            {
                if (Path.GetExtension(outputFileName) == this.templatingCallback.Extension)
                {
                    // Required output file exists. Touch it and return.
                    File.SetLastWriteTime(outputFileName, DateTime.UtcNow);
                    await Dispatcher.Yield(); 
                    return outputItem;
                }

                // Output file has a wrong name, delete it.
                outputItem.Delete();
            }

            // Create the new output file
            outputFileName = Path.ChangeExtension(this.input.FileNames[1], this.templatingCallback.Extension);
            this.input.SetItemAttribute(ItemMetadata.LastGenOutput, FileMethods.GetRelativePath(this.project.FullName, outputFileName).TrimStart('.', '\\'));
            outputItem = IntegrationTest.CreateTestProjectItemFromFile(this.input.ProjectItems, outputFileName);
            await Dispatcher.Yield(); 
            return outputItem;
        }

        private void SimulateTransformation()
        {
            this.templatingCallback = new TextTemplatingCallback();
            this.templatingCallback.Initialize();
            string[] references;
            this.templatingService.PreprocessTemplate(this.input.FileNames[1], string.Empty, this.templatingCallback, "GeneratedTextTransformation", string.Empty, out references);            
        }
    }
}
