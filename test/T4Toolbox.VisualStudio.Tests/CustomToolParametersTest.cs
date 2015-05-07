// <copyright file="CustomToolParametersTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.Tests
{
    using System;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.VisualStudio.TextTemplating.VSHost;
    using T4Toolbox.Tests;
    using T4Toolbox.VisualStudio.Tests.Fakes;

    [TestClass]
    public class CustomToolParametersTest : IDisposable
    {
        private readonly FakeDte dte;
        private readonly FakeSolution solution;
        private readonly FakeProject project;
        private readonly FakeProjectItem projectItem;

        public CustomToolParametersTest()
        {
            this.dte = new FakeDte();
            this.dte.AddService(typeof(TemplateLocator), new FakeTemplateLocator(), false);
            this.dte.AddService(typeof(STextTemplating), new FakeTextTemplatingService(), false);
            this.solution = new FakeSolution(this.dte);
            this.project = new FakeProject(this.solution);
            this.projectItem = new FakeProjectItem(this.project);
        }

        public void Dispose()
        {
            this.dte.Dispose();
        }

        [TestMethod]
        public void ParametersForProjectItemWithoutCustomToolAreEmpty()
        {
            File.WriteAllText(this.projectItem.TestFile.FullName, "<#@ parameter name=\"TestParameter\" type=\"System.String\" #>");
            var target = new CustomToolParameters(this.dte, this.project, this.projectItem.Id);
            Assert.AreEqual(0, target.GetProperties().Count);
        }

        [TestMethod]
        public void ParametersForProjectItemWithoutTemplateAreEmpty()
        {
            File.WriteAllText(this.projectItem.TestFile.FullName, "<#@ parameter name=\"TestParameter\" type=\"System.String\" #>");
            this.projectItem.SetItemAttribute(ItemMetadata.Generator, TemplatedFileGenerator.Name);
            var target = new CustomToolParameters(this.dte, this.project, this.projectItem.Id);
            Assert.AreEqual(0, target.GetProperties().Count);
        }

        [TestMethod]
        public void ParametersForProjectItemWithNonexistentTemplateAreEmpty()
        {
            File.WriteAllText(this.projectItem.TestFile.FullName, "<#@ parameter name=\"TestParameter\" type=\"System.String\" #>");
            this.projectItem.SetItemAttribute(ItemMetadata.Generator, TemplatedFileGenerator.Name);
            this.projectItem.SetItemAttribute(ItemMetadata.Template, Path.GetRandomFileName());
            var target = new CustomToolParameters(this.dte, this.project, this.projectItem.Id);
            Assert.AreEqual(0, target.GetProperties().Count);
        }

        [TestMethod]
        public void ParametersDefinedInInputFileAreRecognized()
        {
            this.projectItem.SetItemAttribute(ItemMetadata.Generator, "TextTemplatingFileGenerator");
            File.WriteAllText(this.projectItem.TestFile.FullName, "<#@ parameter name=\"TestParameter\" type=\"System.String\" #>");
            var target = new CustomToolParameters(this.dte, this.project, this.projectItem.Id);
            Assert.AreEqual("TestParameter", target.GetProperties().Cast<PropertyDescriptor>().Single().Name);
        }

        [TestMethod]
        public void ParametersDefinedInIncludedFilesAreRecognized()
        {
            this.projectItem.SetItemAttribute(ItemMetadata.Generator, "TextTemplatingFileGenerator");
            string includeFile = Path.Combine(Path.GetDirectoryName(this.projectItem.TestFile.FullName), Path.GetRandomFileName());
            File.WriteAllText(this.projectItem.TestFile.FullName, "<#@ include file=\"" + includeFile + "\" #>");
            File.WriteAllText(includeFile, "<#@ parameter name=\"TestParameter\" type=\"System.String\" #>");
            var target = new CustomToolParameters(this.dte, this.project, this.projectItem.Id);
            Assert.AreEqual("TestParameter", target.GetProperties().Cast<PropertyDescriptor>().Single().Name);
        }

        [TestMethod]
        public void ParametersDefinedInAssociatedTemplateAreRecognized()
        {
            string templateFile = Path.Combine(Path.GetDirectoryName(this.projectItem.TestFile.FullName), Path.GetRandomFileName());
            File.WriteAllText(templateFile, "<#@ parameter name=\"TestParameter\" type=\"System.String\" #>");
            this.projectItem.SetItemAttribute(ItemMetadata.Generator, TemplatedFileGenerator.Name);
            this.projectItem.SetItemAttribute(ItemMetadata.Template, templateFile);
            var target = new CustomToolParameters(this.dte, this.project, this.projectItem.Id);
            Assert.AreEqual("TestParameter", target.GetProperties().Cast<PropertyDescriptor>().Single().Name);
        }

        [TestMethod]
        public void ParametersDefinedOutsideOfMscorlibAreRecognized()
        {
            this.projectItem.SetItemAttribute(ItemMetadata.Generator, "TextTemplatingFileGenerator");
            File.WriteAllText(this.projectItem.TestFile.FullName, "<#@ parameter name=\"TestParameter\" type=\"System.ComponentModel.EditorBrowsableState\" #>");
            var target = new CustomToolParameters(this.dte, this.project, this.projectItem.Id);
            Assert.AreEqual(typeof(EditorBrowsableState), target.GetProperties().Cast<PropertyDescriptor>().Single().PropertyType);
        }

        [TestMethod]
        public void ParametersDefinedInReferencedAssembliesAreRecognized()
        {
            this.projectItem.SetItemAttribute(ItemMetadata.Generator, "TextTemplatingFileGenerator");
            const string TemplateContent = 
                "<#@ assembly name=\"System.Data.OracleClient, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089, processorArchitecture=x86\" #>" +
                "<#@ parameter name=\"TestParameter\" type=\"System.Data.OracleClient.OracleType\" #>";
            File.WriteAllText(this.projectItem.TestFile.FullName, TemplateContent);
            var target = new CustomToolParameters(this.dte, this.project, this.projectItem.Id);
            PropertyDescriptor parameter = target.GetProperties().Cast<PropertyDescriptor>().Single();
            StringAssert.Contains(parameter.PropertyType.Assembly.FullName, "System.Data.OracleClient");
        }

        [TestMethod]
        public void ParametersObjectConvertsToEmptyStringToPreventDisplayOfTypeNameInPropertiesWindow()
        {
            var target = new CustomToolParameters(this.dte, this.project, this.projectItem.Id);
            TypeConverter converter = TypeDescriptor.GetConverter(target);
            Assert.AreEqual(string.Empty, converter.ConvertToInvariantString(target));
        }

        [TestMethod]
        public void ParameterWhoseTypeCannotBeResolvedIsPresentedAsReadOnlyPropertyWithErrorInDescription()
        {
            this.projectItem.SetItemAttribute(ItemMetadata.Generator, "TextTemplatingFileGenerator");
            File.WriteAllText(this.projectItem.TestFile.FullName, "<#@ parameter name=\"TestParameter\" type=\"NonexistentType\" #>");
            var target = new CustomToolParameters(this.dte, this.project, this.projectItem.Id);
            PropertyDescriptor property = target.GetProperties()[0];
            Assert.AreEqual("TestParameter", property.Name);
            Assert.IsTrue(property.IsReadOnly);
            var attribute = property.Attributes.OfType<System.ComponentModel.DescriptionAttribute>().Single();
            StringAssert.Contains(attribute.Description, "NonexistentType");
        }

        [TestMethod]
        public void CustomToolComparisonIsCaseInsensitive()
        {
            this.projectItem.SetItemAttribute(ItemMetadata.Generator, "TextTemplatingFileGenerator".ToUpperInvariant());
            File.WriteAllText(this.projectItem.TestFile.FullName, "<#@ parameter name=\"TestParameter\" type=\"System.String\" #>");
            var target = new CustomToolParameters(this.dte, this.project, this.projectItem.Id);
            Assert.AreEqual("TestParameter", target.GetProperties().Cast<PropertyDescriptor>().Single().Name);
        }
    }
}