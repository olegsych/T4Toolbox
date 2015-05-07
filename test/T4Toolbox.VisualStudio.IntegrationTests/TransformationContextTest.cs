// <copyright file="TransformationContextTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.IntegrationTests
{
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using EnvDTE;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using VSLangProj;

    [TestClass]
    public class TransformationContextTest : IntegrationTest
    {
        [TestMethod, DataSource(TargetProject.Provider, TargetProject.Connection, TargetProject.Table, DataAccessMethod.Sequential)]
        public async Task TransformationContextInitializesParameterValuesFromMetadata()
        {
            await UIThreadDispatcher.InvokeAsync(delegate
            {
                Project project = this.CreateTestProject();
                string templateFile = Path.Combine(Path.GetDirectoryName(project.FullName), Path.ChangeExtension(Path.GetRandomFileName(), "tt"));
                ProjectItem template = CreateTestProjectItemFromFile(project.ProjectItems, templateFile);

                const string ParameterName = "StringParameter";
                const string ParameterValue = "42";
                template.SetItemAttribute(ParameterName, ParameterValue);

                string templateContent = 
                    "<#@ template language=\"" + this.TargetProject.DefaultTextTemplateLanguage + "\" #>" + 
                    "<#@ output extension=\"txt\" #>" + 
                    "<#@ include file=\"T4Toolbox.tt\" #>" + 
                    "<#@ parameter type=\"System.String\" name=\"" + ParameterName + "\" #>" + 
                    "<#= " + ParameterName + " #>";

                File.WriteAllText(template.FileNames[1], templateContent);
                ((VSProjectItem)template.Object).RunCustomTool();

                ProjectItem output = template.ProjectItems.Cast<ProjectItem>().Single();
                Assert.AreEqual(ParameterValue, File.ReadAllText(output.FileNames[1]));
            });
        }
    }
}