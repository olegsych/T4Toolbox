// <copyright file="CustomToolTemplateEditorTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>
/* This code requires Microsoft Fakes which comes only with Premium and Ultimate edititions.
namespace T4Toolbox.VisualStudio.Tests
{
    using System;
    using System.Globalization;
    using System.IO;
    using Microsoft.VisualStudio.Shell.Interop;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Win32;
    using T4Toolbox.VisualStudio.Tests.Fakes;

    [TestClass]
    public class CustomToolTemplateEditorTest : IDisposable
    {
        private readonly FakeDte dte;
        private readonly FakeSolution solution;
        private readonly FakeProject project;
        private readonly FakeProjectItem projectItem;
        private readonly FakeTypeDescriptorContext context;

        public CustomToolTemplateEditorTest()
        {
            this.dte = new FakeDte();
            this.dte.AddService(typeof(TemplateLocator), new FakeTemplateLocator(), false);
            this.solution = new FakeSolution(this.dte);
            this.project = new FakeProject(this.solution);
            this.projectItem = new FakeProjectItem(this.project);
            this.context = new FakeTypeDescriptorContext(this.dte, this.projectItem);
        }

        public void Dispose()
        {
            this.dte.Dispose();
        }

        [TestMethod]
        public void EditValueShowsDialogWithEmptyFileNameAndProjectItemDirectoryIfTemplateIsEmpty()
        {
            OpenFileDialog openFileDialog = null;
            using (ShimsContext.Create())
            {
                ShimCommonDialog.AllInstances.ShowDialog = (dialog) =>
                {
                    openFileDialog = (OpenFileDialog)dialog;
                    return false;
                };

                new CustomToolTemplateEditor().EditValue(this.context, null, string.Empty);
            }

            Assert.AreEqual(string.Empty, openFileDialog.FileName);
            Assert.AreEqual(Path.GetDirectoryName(this.projectItem.TestFile.FullName), openFileDialog.InitialDirectory, true, CultureInfo.InvariantCulture);
        }

        [TestMethod]
        public void EditValueShowsDialogWithFileNameAndProjectItemDirectoryIfTemplateDoesNotExist()
        {
            string template = Path.GetRandomFileName() + ".tt";
            OpenFileDialog openFileDialog = null;
            using (ShimsContext.Create())
            {
                ShimCommonDialog.AllInstances.ShowDialog = (dialog) =>
                {
                    openFileDialog = (OpenFileDialog)dialog;
                    return false;
                };

                new CustomToolTemplateEditor().EditValue(this.context, null, template);
            }

            Assert.AreEqual(template, openFileDialog.FileName);
            Assert.AreEqual(Path.GetDirectoryName(this.projectItem.TestFile.FullName), openFileDialog.InitialDirectory, true, CultureInfo.InvariantCulture);
        }

        [TestMethod]
        public void EditValueShowsDialogWithFileNameAndTemplateDirectoryIfTemplateExists()
        {
            string inputDirectory = Path.GetDirectoryName(this.projectItem.TestFile.FullName);
            DirectoryInfo templateDirectory = Directory.CreateDirectory(Path.Combine(inputDirectory, Path.GetRandomFileName()));
            string templateFullPath = Path.Combine(templateDirectory.FullName, Path.GetRandomFileName());
            File.WriteAllText(templateFullPath, string.Empty);

            OpenFileDialog openFileDialog = null;
            using (ShimsContext.Create())
            {
                ShimCommonDialog.AllInstances.ShowDialog = (dialog) =>
                {
                    openFileDialog = (OpenFileDialog)dialog;
                    return false;
                };

                new CustomToolTemplateEditor().EditValue(this.context, null, templateFullPath);
            }

            Assert.AreEqual(Path.GetFileName(templateFullPath), openFileDialog.FileName, true, CultureInfo.InvariantCulture);
            Assert.AreEqual(templateDirectory.FullName, openFileDialog.InitialDirectory, true, CultureInfo.InvariantCulture);
        }

        [TestMethod]
        public void EditValueReturnsTemplatePathRelativeToProjectItem()
        {
            string inputDirectoryPath = Path.GetDirectoryName(this.projectItem.TestFile.FullName);
            DirectoryInfo templateDirectory = Directory.CreateDirectory(Path.Combine(inputDirectoryPath, Path.GetRandomFileName()));
            string templateFullPath = Path.Combine(templateDirectory.FullName, Path.GetRandomFileName());
            File.WriteAllText(templateFullPath, string.Empty);
            string relativeTemplatePath = Path.Combine(templateDirectory.Name, Path.GetFileName(templateFullPath));

            using (ShimsContext.Create())
            {
                ShimCommonDialog.AllInstances.ShowDialog = (dialog) =>
                {
                    ((OpenFileDialog)dialog).FileName = templateFullPath;
                    return true;
                };

                object result = new CustomToolTemplateEditor().EditValue(this.context, null, templateFullPath);
                Assert.AreEqual(relativeTemplatePath, (string)result, true, CultureInfo.InvariantCulture);
            }
        }
    }
}
*/