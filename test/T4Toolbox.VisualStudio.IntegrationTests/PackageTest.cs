// <copyright file="PackageTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.IntegrationTests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Threading.Tasks;
    using System.Windows.Media;
    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.ComponentModelHost;
    using Microsoft.VisualStudio.Shell.Interop;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.VisualStudio.Text.Classification;
    using Microsoft.VisualStudio.Text.Formatting;
    using Microsoft.VisualStudio.TextTemplating;
    using Microsoft.VisualStudio.TextTemplating.VSHost;
    using Microsoft.VisualStudio.Utilities;
    using T4Toolbox.DirectiveProcessors;

    [TestClass]
    public class PackageTest : IntegrationTest
    {
        [TestMethod]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "FxCop incorrectly flags async test methods.")]
        public async Task PackageLoads()
        {
            await UIThreadDispatcher.InvokeAsync(delegate
            {
                var shell = (IVsShell)ServiceProvider.GetService(typeof(SVsShell));
                IVsPackage package;
                var packageGuid = new Guid(T4ToolboxPackage.Id);
                Assert.AreEqual(VSConstants.S_OK, shell.LoadPackage(ref packageGuid, out package));
                Assert.IsNotNull(package);
            });
        }

        [TestMethod]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "FxCop incorrectly flags async test methods.")]
        public async Task PackageRegistersTransformationContextProcessor()
        {
            await UIThreadDispatcher.InvokeAsync(delegate
            {
                var templatingHost = (ITextTemplatingEngineHost)ServiceProvider.GetService(typeof(STextTemplating));
                Type processorType = templatingHost.ResolveDirectiveProcessor(TransformationContextProcessor.Name);
                Assert.AreEqual(typeof(TransformationContextProcessor), processorType);
            });
        }

        [TestMethod]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "FxCop incorrectly flags async test methods.")]
        public async Task PackageDeploysAndRegistersT4ToolboxIncludeFile()
        {
            await UIThreadDispatcher.InvokeAsync(delegate
            {
                // Make the T4 Host think it is processing a template, otherwise it doesn't search for include folders
                var templatingService = (ITextTemplating)ServiceProvider.GetService(typeof(STextTemplating));
                templatingService.ProcessTemplate("C:\\dummy.txt", string.Empty); // Note the non-.TT extension. T4 Toolbox include files should be available for templates of all file extensions.

                // Ask the T4 host to resolve the T4Toolbox.tt include file
                var templatingHost = (ITextTemplatingEngineHost)ServiceProvider.GetService(typeof(STextTemplating));
                string content, location;
                Assert.IsTrue(templatingHost.LoadIncludeText("T4Toolbox.tt", out content, out location));
                Assert.IsFalse(string.IsNullOrWhiteSpace(content));
                Assert.IsTrue(File.Exists(location));
            });
        }

        [TestMethod]
        public void PackageRegistersTextTemplateContentType()
        {
            var componentModel = (IComponentModel)ServiceProvider.GetService(typeof(SComponentModel));
            var contentTypeRegistry = componentModel.DefaultExportProvider.GetExportedValue<IContentTypeRegistryService>();
            Assert.IsNotNull(contentTypeRegistry.GetContentType("TextTemplate"));
        }

        [TestMethod]
        public void PackageAssociatesTextTemplateContentTypeWithFileExtension()
        {
            var componentModel = (IComponentModel)ServiceProvider.GetService(typeof(SComponentModel));
            var fileExtensionRegistry = componentModel.DefaultExportProvider.GetExportedValue<IFileExtensionRegistryService>();
            var contentType = fileExtensionRegistry.GetContentTypeForExtension("tt");
            Assert.AreEqual("TextTemplate", contentType.TypeName);
        }

        [TestMethod]
        public void PackageRegistersTextTemplateDelimiterClassificationType()
        {
            AssertClassificationTypeIsRegistered("TextTemplate.Delimiter");
        }

        [TestMethod]
        public void PackageRegistersTextTemplateCodeBlockClassificationType()
        {
            AssertClassificationTypeIsRegistered("TextTemplate.CodeBlock");            
        }

        [TestMethod]
        public void PackageRegistersTextTemplateDirectiveNameClassificationType()
        {
            AssertClassificationTypeIsRegistered("TextTemplate.DirectiveName");
        }

        [TestMethod]
        public void PackageRegistersTextTemplateAttributeNameClassificationType()
        {
            AssertClassificationTypeIsRegistered("TextTemplate.AttributeName");
        }

        [TestMethod]
        public void PackageRegistersTextTemplateAttributeValueClassificationType()
        {
            AssertClassificationTypeIsRegistered("TextTemplate.AttributeValue");
        }

        [TestMethod]
        public void PackageRegistersTextTemplateCodeBlockFormatDefinition()
        {
            TextFormattingRunProperties properties = GetClassificationTypeTextProperties("TextTemplate.CodeBlock");
            Assert.AreEqual(Colors.Lavender, ((SolidColorBrush)properties.BackgroundBrush).Color);
        }

        [TestMethod]
        public void PackageRegistersTextTemplateDelimiterFormatDefinition()
        {
            TextFormattingRunProperties properties = GetClassificationTypeTextProperties("TextTemplate.Delimiter");
            Assert.AreEqual(Colors.Yellow, ((SolidColorBrush)properties.BackgroundBrush).Color);
        }

        [TestMethod]
        public void PackageRegistersTextTemplateDirectiveNameFormatDefinition()
        {
            TextFormattingRunProperties properties = GetClassificationTypeTextProperties("TextTemplate.DirectiveName");
            Assert.AreEqual(Colors.Maroon, ((SolidColorBrush)properties.ForegroundBrush).Color);
        }

        [TestMethod]
        public void PackageRegistersTextTemplateAttributeNameFormatDefinition()
        {
            TextFormattingRunProperties properties = GetClassificationTypeTextProperties("TextTemplate.AttributeName");
            Assert.AreEqual(Colors.Red, ((SolidColorBrush)properties.ForegroundBrush).Color);
        }

        [TestMethod]
        public void PackageRegistersTextTemplateAttributeValueFormatDefinition()
        {
            TextFormattingRunProperties properties = GetClassificationTypeTextProperties("TextTemplate.AttributeValue");
            Assert.AreEqual(Colors.Blue, ((SolidColorBrush)properties.ForegroundBrush).Color);
        }

        private static void AssertClassificationTypeIsRegistered(string classificationTypeName)
        {
            var componentModel = (IComponentModel)ServiceProvider.GetService(typeof(SComponentModel));
            var classificationTypeRegistry = componentModel.DefaultExportProvider.GetExportedValue<IClassificationTypeRegistryService>();
            Assert.IsNotNull(classificationTypeRegistry.GetClassificationType(classificationTypeName));
        }

        private static TextFormattingRunProperties GetClassificationTypeTextProperties(string classificationTypeName)
        {
            var componentModel = (IComponentModel)ServiceProvider.GetService(typeof(SComponentModel));

            var classificationTypeRegistry = componentModel.DefaultExportProvider.GetExportedValue<IClassificationTypeRegistryService>();
            IClassificationType delimiterClassification = classificationTypeRegistry.GetClassificationType(classificationTypeName);

            var formatMapService = componentModel.DefaultExportProvider.GetExportedValue<IClassificationFormatMapService>();
            IClassificationFormatMap formatMap = formatMapService.GetClassificationFormatMap("Text Editor");
            TextFormattingRunProperties properties = formatMap.GetTextProperties(delimiterClassification);
            return properties;
        }
    }
}
