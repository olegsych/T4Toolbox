// <copyright file="TemplateLocatorTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.IntegrationTests
{
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    
    [TestClass]
    public class TemplateLocatorTest : IntegrationTest
    {
        [TestMethod]
        public async Task TemplateLocatorFindsTemplateRelativeToInputFolder()
        {
            await UIThreadDispatcher.InvokeAsync(delegate
            {
                IntegrationTest.LoadT4ToolboxPackage();
                string inputPath = this.CreateTempFile(SolutionDirectory, string.Empty);
                string templatePath = this.CreateTempFile(SolutionDirectory, string.Empty);
                var templateLocator = (TemplateLocator)ServiceProvider.GetService(typeof(TemplateLocator));
                string resolvedPath = Path.GetFileName(templatePath);
                Assert.IsTrue(templateLocator.LocateTemplate(inputPath, ref resolvedPath));                
                Assert.AreEqual(templatePath, resolvedPath);
            });
        }

        [TestMethod]
        public async Task TemplateLocatorFindsTemplateRelativeToIncludeFolder()
        {
            await UIThreadDispatcher.InvokeAsync(delegate
            {
                IntegrationTest.LoadT4ToolboxPackage();
                string inputPath = this.CreateTempFile(SolutionDirectory, string.Empty);
                string includeFolder = Path.Combine(Path.GetDirectoryName(typeof(TemplateLocator).Assembly.Location), "Include");
                string templatePath = this.CreateTempFile(includeFolder, string.Empty);

                var templateLocator = (TemplateLocator)ServiceProvider.GetService(typeof(TemplateLocator));
                string resolvedPath = Path.GetFileName(templatePath);
                Assert.IsTrue(templateLocator.LocateTemplate(inputPath, ref resolvedPath));
                Assert.AreEqual(templatePath, resolvedPath);
            });
        }
    }
}