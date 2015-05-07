// <copyright file="TemplatedFileGeneratorTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.IntegrationTests
{
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using EnvDTE;
    using EnvDTE80;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class TemplatedFileGeneratorTest : IntegrationTest
    {
        private const string TestText = "42";
        private const string TextFileItemTemplate = "Text File";

        [TestMethod, DataSource(TargetProject.Provider, TargetProject.Connection, TargetProject.Table, DataAccessMethod.Sequential)]
        public async Task GeneratorPassesInputFileNameToTemplate()
        {
            await UIThreadDispatcher.InvokeAsync(delegate
            {
                Project project = this.CreateTestProject();

                // Create template that writes input file name to the output file
                string templateContent = "<#@ parameter type=\"System.String\" name=\"InputFileName\" #><#= InputFileName #>";
                string templatePath = this.CreateTempFile(Path.GetDirectoryName(project.FullName), templateContent);

                // Create input project item and associate it with the template
                ProjectItem inputItem = this.CreateTestProjectItem(project.ProjectItems, TextFileItemTemplate);
                inputItem.SetItemAttribute(ItemMetadata.Template, templatePath);
                inputItem.Properties.Item(ProjectItemProperty.CustomTool).Value = TemplatedFileGenerator.Name;

                // Verify that name of the input file was written to the output file
                ProjectItem outputItem = inputItem.ProjectItems.Cast<ProjectItem>().Single();
                Assert.AreEqual(inputItem.FileNames[1], File.ReadAllText(outputItem.FileNames[1]));
            });
        }

        [TestMethod, DataSource(TargetProject.Provider, TargetProject.Connection, TargetProject.Table, DataAccessMethod.Sequential)]
        public async Task GeneratorPassesInputFileContentToTemplate()
        {
            await UIThreadDispatcher.InvokeAsync(delegate
            {
                Project project = this.CreateTestProject();
                string projectDirectory = Path.GetDirectoryName(project.FullName);

                // Create template that writes input file name to the output file
                string templateContent = "<#@ parameter type=\"System.String\" name=\"InputFileContent\" #><#= InputFileContent #>";
                string templatePath = this.CreateTempFile(projectDirectory, templateContent);

                // Create input project item and associate it with the template
                string inputPath = this.CreateTempFile(projectDirectory, TestText);
                ProjectItem inputItem = project.ProjectItems.AddFromFile(inputPath);
                inputItem.SetItemAttribute(ItemMetadata.Template, templatePath);
                inputItem.Properties.Item(ProjectItemProperty.CustomTool).Value = TemplatedFileGenerator.Name;

                // Verify that name of the input file was written to the output file
                ProjectItem outputItem = inputItem.ProjectItems.Cast<ProjectItem>().Single();
                Assert.AreEqual(TestText, File.ReadAllText(outputItem.FileNames[1]));
            });
        }

        [TestMethod, DataSource(TargetProject.Provider, TargetProject.Connection, TargetProject.Table, DataAccessMethod.Sequential)]
        public async Task GeneratorProducesMeaningfulErrorWhenTemplateIsNotSpecified()
        {
            await UIThreadDispatcher.InvokeAsync(delegate
            {
                // Create input project item and associate it with the non-existent template
                ProjectItem inputItem = this.CreateTestProjectItem(TextFileItemTemplate);
                inputItem.Properties.Item(ProjectItemProperty.CustomTool).Value = TemplatedFileGenerator.Name;

                // Verify that an error was generated pointing to the input file and the Template metadata
                ErrorItem error = IntegrationTest.ErrorItems.Single(e => e.FileName == inputItem.FileNames[1]);
                StringAssert.Contains(error.Description, ItemMetadata.Template);
            });
        }

        [TestMethod, DataSource(TargetProject.Provider, TargetProject.Connection, TargetProject.Table, DataAccessMethod.Sequential)]
        public async Task GeneratorProducesMeaningfulErrorWhenTemplateCannotBeFound()
        {
            await UIThreadDispatcher.InvokeAsync(delegate
            {
                // Create input project item and associate it with the non-existent template
                ProjectItem inputItem = this.CreateTestProjectItem(TextFileItemTemplate);
                string nonExistentTemplate = Path.GetRandomFileName();
                inputItem.SetItemAttribute(ItemMetadata.Template, nonExistentTemplate);
                inputItem.Properties.Item(ProjectItemProperty.CustomTool).Value = TemplatedFileGenerator.Name;

                // Verify that an error was generated pointing to the input file and the non-existent template
                ErrorItem error = IntegrationTest.ErrorItems.Single(e => e.FileName == inputItem.FileNames[1]);
                StringAssert.Contains(error.Description, ItemMetadata.Template);
                StringAssert.Contains(error.Description, nonExistentTemplate);
            });
        }

        [TestMethod, DataSource(TargetProject.Provider, TargetProject.Connection, TargetProject.Table, DataAccessMethod.Sequential)]
        public async Task GeneratorConvertsScriptFileToTemplate()
        {
            await UIThreadDispatcher.InvokeAsync(delegate
            {
                ProjectItem inputItem = this.CreateTestProjectItem(TextFileItemTemplate);

                string scriptFile = Path.ChangeExtension(inputItem.FileNames[1], ".tt");
                File.WriteAllText(scriptFile, TestText);
                inputItem.Properties.Item(ProjectItemProperty.CustomTool).Value = ScriptFileGenerator.Name;

                inputItem.Properties.Item(ProjectItemProperty.CustomTool).Value = TemplatedFileGenerator.Name;

                // Verify that no errors were reported
                Assert.IsFalse(IntegrationTest.ErrorItems.Any(e => e.FileName == inputItem.FileNames[1]));

                // Verify that CustomToolTemplate property was set
                Assert.AreEqual(Path.GetFileName(scriptFile), inputItem.Properties.Item(ProjectItemProperty.CustomToolTemplate).Value);

                // Verify that template was preserved
                Assert.AreEqual(TestText, File.ReadAllText(scriptFile));
            });
        }
    }
}
