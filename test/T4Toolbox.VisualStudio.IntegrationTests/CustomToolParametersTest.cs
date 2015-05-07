// <copyright file="CustomToolParametersTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.IntegrationTests
{
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using EnvDTE;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class CustomToolParametersTest : IntegrationTest
    {
        [TestMethod, DataSource(TargetProject.Provider, TargetProject.Connection, TargetProject.Table, DataAccessMethod.Sequential)]
        public async Task ParametersDefinedInAssociatedTemplateAreRecognized()
        {
            await UIThreadDispatcher.InvokeAsync(delegate
            {
                ProjectItem projectItem = this.CreateTestProjectItem("Text File");
                string templateFile = Path.Combine(Path.GetDirectoryName(projectItem.FileNames[1]), Path.GetRandomFileName());
                File.WriteAllText(templateFile, "<#@ parameter name=\"TestParameter\" type=\"System.String\" #>");
                projectItem.Properties.Item(ProjectItemProperty.CustomTool).Value = TemplatedFileGenerator.Name;
                projectItem.Properties.Item(ProjectItemProperty.CustomToolTemplate).Value = Path.GetFileName(templateFile);
                var target = (ICustomTypeDescriptor)projectItem.Properties.Item(ProjectItemProperty.CustomToolParameters).Value;
                Assert.AreEqual("TestParameter", target.GetProperties().Cast<PropertyDescriptor>().Single().Name);
            });
        }

        [TestMethod, DataSource(TargetProject.Provider, TargetProject.Connection, TargetProject.Table, DataAccessMethod.Sequential)]
        public async Task ParametersDefinedInScriptFileAreRecognized()
        {
            await UIThreadDispatcher.InvokeAsync(delegate
            {
                ProjectItem projectItem = this.CreateTestProjectItem("Text File");
                projectItem.Properties.Item(ProjectItemProperty.CustomTool).Value = ScriptFileGenerator.Name;

                ProjectItem scriptItem = projectItem.ProjectItems.Cast<ProjectItem>().Single();
                File.AppendAllText(scriptItem.FileNames[1], "<#@ parameter name=\"TestParameter\" type=\"System.String\" #>");

                var target = (ICustomTypeDescriptor)projectItem.Properties.Item(ProjectItemProperty.CustomToolParameters).Value;
                Assert.AreEqual("TestParameter", target.GetProperties().Cast<PropertyDescriptor>().Single().Name);
            });
        }

        [TestMethod, DataSource(TargetProject.Provider, TargetProject.Connection, TargetProject.Table, DataAccessMethod.Sequential)]
        public async Task ParametersDefinedInInputFileAreRecognized()
        {
            await UIThreadDispatcher.InvokeAsync(delegate
            {
                ProjectItem projectItem = this.CreateTestProjectItem("Text File");
                projectItem.Properties.Item(ProjectItemProperty.CustomTool).Value = "TextTemplatingFileGenerator";
                File.WriteAllText(projectItem.FileNames[1], "<#@ parameter name=\"TestParameter\" type=\"System.String\" #>");
                var target = (ICustomTypeDescriptor)projectItem.Properties.Item(ProjectItemProperty.CustomToolParameters).Value;
                Assert.AreEqual("TestParameter", target.GetProperties().Cast<PropertyDescriptor>().Single().Name);
            });
        }
    }
}