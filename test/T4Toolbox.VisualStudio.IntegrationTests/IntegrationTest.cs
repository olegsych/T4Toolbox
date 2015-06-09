// <copyright file="IntegrationTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.IntegrationTests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Windows.Threading;
    using EnvDTE;
    using EnvDTE80;
    using EnvDTE90;
    using Microsoft.VisualStudio;    
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Shell.Interop;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    [SuppressMessage("Microsoft.Design", "CA1053:StaticHolderTypesShouldNotHaveConstructors", Justification = "This class serves as base for non-static classes.")]
    public class IntegrationTest : IDisposable
    {
        private static Dictionary<string, Project> projects;

        private readonly List<string> tempFiles = new List<string>();
        private TargetProject targetProject;

        public TestContext TestContext { get; set; }

        internal TargetProject TargetProject
        {
            get { return this.targetProject ?? (this.targetProject = new TargetProject(this.TestContext)); }
        }

        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Dte", Justification = "Property name needs to be consistent with the API.")]
        protected static DTE2 Dte { get; private set; }

        protected static IServiceProvider ServiceProvider { get; private set; }
        
        protected static string SolutionDirectory { get; private set; }

        protected static Solution3 Solution { get; private set; }

        protected static Dispatcher UIThreadDispatcher { get; private set; }

        [AssemblyInitialize]
        public static void AssemblyInitialize(TestContext context)
        {
            ThreadHelper.Generic.Invoke(delegate
            {
                CreateTestSolution();
                UIThreadDispatcher = Dispatcher.CurrentDispatcher;
            });
        }

        [AssemblyCleanup]
        public static void AssemblyCleanup()
        {
            ThreadHelper.Generic.Invoke(DeleteTestSolution);
        }

        public void Dispose()
        {
            foreach (string tempFile in this.tempFiles)
            {
                File.Delete(tempFile);
            }
        }

        protected static IEnumerable<ErrorItem> ErrorItems
        {
            get
            {
                ErrorItems errorItems = Dte.ToolWindows.ErrorList.ErrorItems;
                for (int i = 1; i <= errorItems.Count; i++)
                {
                    yield return errorItems.Item(i);
                }
            }
        }

        protected static void CreateTestSolution()
        {
            // Create a temporary folder for the test solution
            SolutionDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(SolutionDirectory);

            // Create the test solution
            ServiceProvider = Microsoft.VisualStudio.Shell.ServiceProvider.GlobalProvider;
            Dte = (DTE2)ServiceProvider.GetService(typeof(DTE));
            Solution = (Solution3)Dte.Solution;
            Solution.Create(SolutionDirectory, "TestSolution");

            projects = new Dictionary<string, Project>();
        }

        protected static Project CreateTestProject(string templateName, string language)
        {
            Project project;
            if (!projects.TryGetValue(language, out project))
            {
                string projectTemplate = Solution.GetProjectTemplate(templateName, language);
                string projectName = language + "Project";
                string projectFolder = Path.Combine(SolutionDirectory, projectName);
                Solution.AddFromTemplate(projectTemplate, projectFolder, projectName);
                project = Solution.Projects.Cast<Project>().First(p => p.Name == projectName);
                projects.Add(language, project);
            }

            return project;
        }

        protected static ProjectItem CreateTestProjectItemFromFile(ProjectItems projectItems, string fileName)
        {
            if (projectItems == null)
            {
                throw new ArgumentNullException("projectItems");
            }

            if (!File.Exists(fileName))
            {
                File.WriteAllText(fileName, string.Empty);                
            }

            return projectItems.AddFromFile(fileName);
        }

        protected static void DeleteTestSolution()
        {
            // Close test solution
            Solution.Close();
            Solution = null;
            projects = null;

            // Delete the temporary folder
            Directory.Delete(SolutionDirectory, true);
        }

        protected static void LoadT4ToolboxPackage()
        {
            var shell = (IVsShell)ServiceProvider.GetService(typeof(SVsShell));
            IVsPackage package;
            var packageGuid = new Guid(T4ToolboxPackage.Id);
            ErrorHandler.ThrowOnFailure(shell.LoadPackage(ref packageGuid, out package));
        }

        protected string CreateTempFile(string directory, string content)
        {
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            string fileName = Path.Combine(directory, Path.GetRandomFileName());
            this.tempFiles.Add(fileName);
            File.WriteAllText(fileName, content);
            return fileName;
        }

        protected Project CreateTestProject()
        {
            return CreateTestProject(this.TargetProject.Template, this.TargetProject.Language);
        }

        protected ProjectItem CreateTestProjectItem(ProjectItems projectItems, string templateName)
        {
            if (projectItems == null)
            {
                throw new ArgumentNullException("projectItems");
            }

            string templateFile = Solution.GetProjectItemTemplate(templateName, this.TargetProject.Language);
            string itemName = Path.GetFileNameWithoutExtension(Path.GetRandomFileName());
            ProjectItem projectItem = projectItems.AddFromTemplate(templateFile, itemName)
                ?? projectItems.Cast<ProjectItem>().First(pi => Path.GetFileNameWithoutExtension(pi.Name) == itemName);
            projectItem.Document.Close(); // To avoid sideffects and enable testing by writing to files as opposed to document manipulation
            return projectItem;
        }

        protected ProjectItem CreateTestProjectItem(string templateName)
        {
            Project project = this.CreateTestProject();
            return this.CreateTestProjectItem(project.ProjectItems, templateName);
        }
    }
}