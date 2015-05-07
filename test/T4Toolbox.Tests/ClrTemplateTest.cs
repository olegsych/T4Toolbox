// <copyright file="ClrTemplateTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.Tests
{
    using System;
    using System.IO;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.VisualStudio.TextTemplating;

    /// <summary>
    /// A test class for <see cref="ClrTemplate" />.
    /// </summary>
    [TestClass]
    public class ClrTemplateTest 
    {
        #region DefaultNamespace

        [TestMethod]
        public void DefaultNamespaceReturnsRootNamespaceForInputFileInRootOfProject()
        {
            using (var transformation = new FakeTransformation())
            using (var context = new TransformationContext(transformation, transformation.GenerationEnvironment))
            using (var template = new TestClrTemplate())
            {
                template.Context = context;
                transformation.Host.TemplateFile = Path.Combine(Environment.CurrentDirectory, "Template.tt");
                transformation.Host.GetMetadataValue = (hierarhcy, inputFile, metadataName) => string.Empty;
                transformation.Host.GetPropertyValue = (hierarchy, propertyName) =>
                {
                    switch (propertyName)
                    {
                        case "RootNamespace":
                            return "TestNamespace";
                        case "MSBuildProjectFullPath":
                            return Path.Combine(Environment.CurrentDirectory, "Project.proj");
                        default:
                            return string.Empty;
                    }
                };

                Assert.AreEqual("TestNamespace", template.DefaultNamespace);
            }
        }

        [TestMethod]
        public void DefaultNamespaceCombinesRelativeInputFilePathWithRootNamespace()
        {
            using (var transformation = new FakeTransformation())
            using (var context = new TransformationContext(transformation, transformation.GenerationEnvironment))
            using (var template = new TestClrTemplate())
            {
                template.Context = context;
                transformation.Host.TemplateFile = Path.Combine(Environment.CurrentDirectory, "SubFolder\\Template.tt");
                transformation.Host.GetMetadataValue = (hierarhcy, inputFile, metadataName) => string.Empty;
                transformation.Host.GetPropertyValue = (hierarchy, propertyName) =>
                {
                    switch (propertyName)
                    {
                        case "RootNamespace":
                            return "TestNamespace";
                        case "MSBuildProjectFullPath":
                            return Path.Combine(Environment.CurrentDirectory, "Project.proj");
                        default:
                            return string.Empty;
                    }
                };

                Assert.AreEqual("TestNamespace.SubFolder", template.DefaultNamespace);
            }
        }

        [TestMethod]
        public void DefaultNamespaceUsesProjectItemLinkPathInsteadOfPhysicalFilePath()
        {
            using (var transformation = new FakeTransformation())
            using (var context = new TransformationContext(transformation, transformation.GenerationEnvironment))
            using (var template = new TestClrTemplate())
            {
                template.Context = context;

                transformation.Host.TemplateFile = Path.Combine(Environment.CurrentDirectory, "Template.tt");

                transformation.Host.GetMetadataValue = (hierarhcy, inputFile, metadataName) =>
                {
                    switch (metadataName)
                    {
                        case "Link":
                            return "SubFolder\\Template.tt";
                        default:
                            return string.Empty;
                    }
                };

                transformation.Host.GetPropertyValue = (hierarchy, propertyName) =>
                {
                    switch (propertyName)
                    {
                        case "RootNamespace":
                            return "TestNamespace";
                        case "MSBuildProjectFullPath":
                            return Path.Combine(Environment.CurrentDirectory, "Project.proj");
                        default:
                            return string.Empty;
                    }
                };

                Assert.AreEqual("TestNamespace.SubFolder", template.DefaultNamespace);
            }
        }

        #endregion

        [TestMethod]
        public void RootNamespaceReturnsPropertyValueSuppliedByProvider()
        {
            using (var transformation = new FakeTransformation())
            using (var context = new TransformationContext(transformation, transformation.GenerationEnvironment))
            using (var template = new TestClrTemplate())
            {
                template.Context = context;

                const string ExpectedValue = "TestNamespace";
                string actualPropertyName = null;
                transformation.Host.GetPropertyValue = (hierarchy, propertyName) =>
                {
                    actualPropertyName = propertyName;
                    return ExpectedValue;
                };

                Assert.AreEqual(ExpectedValue, template.RootNamespace);
                Assert.AreEqual("RootNamespace", actualPropertyName);
            }
        }

        // ClrTemplate is an abstract class. Need a concrete descendant to test it.
        private class TestClrTemplate : ClrTemplate
        {
            public override string TransformText()
            {
                return string.Empty;
            }
        }
    }
}