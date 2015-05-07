// <copyright file="ScriptFileGeneratorTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.IntegrationTests
{
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using EnvDTE;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.VisualStudio.TextTemplating;
    using VSLangProj;

    [TestClass]
    public class ScriptFileGeneratorTest : IntegrationTest
    {
        private const string TestText = "42";
        private const string TextFileItemTemplate = "Text File";

        [TestMethod, DataSource(TargetProject.Provider, TargetProject.Connection, TargetProject.Table, DataAccessMethod.Sequential)]
        public async Task GeneratorProducesEmptyScriptFile()
        {
            await UIThreadDispatcher.InvokeAsync(delegate
            {
                ProjectItem inputItem = this.CreateTestProjectItem(TextFileItemTemplate);
                inputItem.Properties.Item(ProjectItemProperty.CustomTool).Value = ScriptFileGenerator.Name;
                ProjectItem scriptItem = inputItem.ProjectItems.Cast<ProjectItem>().Single();
                Assert.AreEqual(".tt", Path.GetExtension(scriptItem.FileNames[1]));
                Assert.AreEqual("TextTemplatingFileGenerator", scriptItem.GetItemAttribute(ItemMetadata.Generator));
            });
        }

        [TestMethod, DataSource(TargetProject.Provider, TargetProject.Connection, TargetProject.Table, DataAccessMethod.Sequential)]
        public async Task GeneratedScriptFileSpecifiesDefaultTextTemplateLanguageForProjectType()
        {
            await UIThreadDispatcher.InvokeAsync(delegate
            {
                ProjectItem inputItem = this.CreateTestProjectItem(TextFileItemTemplate);
                inputItem.Properties.Item(ProjectItemProperty.CustomTool).Value = ScriptFileGenerator.Name;
                ProjectItem scriptItem = inputItem.ProjectItems.Cast<ProjectItem>().Single();
                string scriptContent = File.ReadAllText(scriptItem.FileNames[1]);
                StringAssert.StartsWith(scriptContent, "<#@ template language=\"" + this.TargetProject.DefaultTextTemplateLanguage + "\"");
            });                        
        }

        [TestMethod, DataSource(TargetProject.Provider, TargetProject.Connection, TargetProject.Table, DataAccessMethod.Sequential)]
        public async Task GeneratedScriptFileSpecifiesDefaultProjectFileExtension()
        {
            await UIThreadDispatcher.InvokeAsync(delegate
            {
                ProjectItem inputItem = this.CreateTestProjectItem(TextFileItemTemplate);
                inputItem.Properties.Item(ProjectItemProperty.CustomTool).Value = ScriptFileGenerator.Name;
                ProjectItem scriptItem = inputItem.ProjectItems.Cast<ProjectItem>().Single();
                string scriptContent = File.ReadAllText(scriptItem.FileNames[1]);
                StringAssert.Contains(scriptContent, "<#@ output extension=\"" + this.TargetProject.CodeFileExtension.Trim('.') + "\"");
            });
        }

        [TestMethod, DataSource(TargetProject.Provider, TargetProject.Connection, TargetProject.Table, DataAccessMethod.Sequential)]
        public async Task GeneratorTriggersTransformationOfNewlyGeneratedScriptFile()
        {
            await UIThreadDispatcher.InvokeAsync(delegate
            {
                ProjectItem inputItem = this.CreateTestProjectItem(TextFileItemTemplate);
                inputItem.Properties.Item(ProjectItemProperty.CustomTool).Value = ScriptFileGenerator.Name;
                ProjectItem scriptITem = inputItem.ProjectItems.Cast<ProjectItem>().Single();
                Assert.AreEqual(1, scriptITem.ProjectItems.Count);
            });            
        }

        [TestMethod, DataSource(TargetProject.Provider, TargetProject.Connection, TargetProject.Table, DataAccessMethod.Sequential)]
        public async Task GeneratorPreservesExistingScriptFile()
        {
            await UIThreadDispatcher.InvokeAsync(delegate
            {
                ProjectItem inputItem = this.CreateTestProjectItem(TextFileItemTemplate);
                File.WriteAllText(Path.ChangeExtension(inputItem.FileNames[1], ".tt"), TestText);
                inputItem.Properties.Item(ProjectItemProperty.CustomTool).Value = ScriptFileGenerator.Name;
                ProjectItem scriptItem = inputItem.ProjectItems.Cast<ProjectItem>().Single();
                Assert.AreEqual(TestText, File.ReadAllText(scriptItem.FileNames[1]));
            });                        
        }

        [TestMethod, DataSource(TargetProject.Provider, TargetProject.Connection, TargetProject.Table, DataAccessMethod.Sequential)]
        public async Task GeneratorPreservesEncodingOfExistingScriptFile()
        {
            await UIThreadDispatcher.InvokeAsync(delegate
            {
                ProjectItem inputItem = this.CreateTestProjectItem(TextFileItemTemplate);
                File.WriteAllText(Path.ChangeExtension(inputItem.FileNames[1], ".tt"), TestText, Encoding.UTF32);
                inputItem.Properties.Item(ProjectItemProperty.CustomTool).Value = ScriptFileGenerator.Name;
                ProjectItem scriptItem = inputItem.ProjectItems.Cast<ProjectItem>().Single();
                Assert.AreEqual(Encoding.UTF32, EncodingHelper.GetEncoding(scriptItem.FileNames[1]));
            });
        }

        [TestMethod, DataSource(TargetProject.Provider, TargetProject.Connection, TargetProject.Table, DataAccessMethod.Sequential)]
        public async Task GeneratorTriggersTransformationOfExistingScriptFile()
        {
            await UIThreadDispatcher.InvokeAsync(delegate
            {
                ProjectItem inputItem = this.CreateTestProjectItem(TextFileItemTemplate);
                File.WriteAllText(Path.ChangeExtension(inputItem.FileNames[1], ".tt"), TestText);
                inputItem.Properties.Item(ProjectItemProperty.CustomTool).Value = ScriptFileGenerator.Name;
                ProjectItem scriptItem = inputItem.ProjectItems.Cast<ProjectItem>().Single();
                ProjectItem outputItem = scriptItem.ProjectItems.Cast<ProjectItem>().Single();
                Assert.AreEqual(TestText, File.ReadAllText(outputItem.FileNames[1]));
            });            
        }

        [TestMethod, DataSource(TargetProject.Provider, TargetProject.Connection, TargetProject.Table, DataAccessMethod.Sequential)]
        public async Task GeneratorSavesExistingScriptFileOpenedInEditorToPreventRunCustomToolImplementationFromSilentlyDiscardingChanges()
        {
            await UIThreadDispatcher.InvokeAsync(delegate
            {
                ProjectItem inputItem = this.CreateTestProjectItem(TextFileItemTemplate);
                inputItem.Properties.Item(ProjectItemProperty.CustomTool).Value = ScriptFileGenerator.Name;
                ProjectItem scriptItem = inputItem.ProjectItems.Cast<ProjectItem>().Single();

                // Simulate changing the script file in Visual Studio editor
                Document document = Dte.Documents.Open(scriptItem.FileNames[1]);
                var textDocument = (TextDocument)document.Object();
                textDocument.Selection.EndOfDocument(Extend: true);
                textDocument.Selection.Text = TestText;

                // Run the custom tool
                ((VSProjectItem)inputItem.Object).RunCustomTool();

                // Verify that changes made in the editor were saved
                Assert.AreEqual(TestText, File.ReadAllText(scriptItem.FileNames[1]));
            });
        }

        [TestMethod, DataSource(TargetProject.Provider, TargetProject.Connection, TargetProject.Table, DataAccessMethod.Sequential)]
        public async Task GeneratorConvertsCustomToolTemplateToScriptFile()
        {
            await UIThreadDispatcher.InvokeAsync(delegate
            {
                ProjectItem inputItem = this.CreateTestProjectItem(TextFileItemTemplate);
                string customToolTemplate = Path.GetRandomFileName();
                File.WriteAllText(Path.Combine(Path.GetDirectoryName(inputItem.FileNames[1]), customToolTemplate), TestText);
                inputItem.Properties.Item(ProjectItemProperty.CustomToolTemplate).Value = customToolTemplate;
                inputItem.Properties.Item(ProjectItemProperty.CustomTool).Value = ScriptFileGenerator.Name;
                ProjectItem scriptItem = inputItem.ProjectItems.Cast<ProjectItem>().Single();
                Assert.AreEqual(TestText, File.ReadAllText(scriptItem.FileNames[1]));
                Assert.AreEqual(string.Empty, inputItem.Properties.Item(ProjectItemProperty.CustomToolTemplate).Value);
            });                                    
        }
    }
}