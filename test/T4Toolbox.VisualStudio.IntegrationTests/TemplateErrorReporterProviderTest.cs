// <copyright file="TemplateErrorReporterProviderTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.IntegrationTests
{
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.ComponentModelHost;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Tagging;
    using Microsoft.VisualStudio.Utilities;
    using T4Toolbox.VisualStudio.Editor;

    [TestClass]
    public class TemplateErrorReporterProviderTest : IntegrationTest
    {
        private IContentType textContentType;
        private ITextDocumentFactoryService documentFactory;
        private IComponentModel componentModel;

        public TemplateErrorReporterProviderTest()
        {
            UIThreadDispatcher.Invoke(delegate
            {
                this.componentModel = (IComponentModel)ServiceProvider.GetService(typeof(SComponentModel));
                
                // Use Text content type to test in isolation from TemplateErrorTaggerProvider
                var contentTypeRegistry = this.componentModel.DefaultExportProvider.GetExportedValue<IContentTypeRegistryService>();
                this.textContentType = contentTypeRegistry.GetContentType("Text");

                this.documentFactory = this.componentModel.DefaultExportProvider.GetExportedValue<ITextDocumentFactoryService>();
            });
        }

        [TestMethod]
        public async Task CreateTaggerNeverCreatesErrorTagger()
        {
            await UIThreadDispatcher.InvokeAsync(delegate
            {
                string tempFile = this.CreateTempFile(Path.GetTempPath(), string.Empty);
                using (ITextDocument document = this.documentFactory.CreateAndLoadTextDocument(tempFile, this.textContentType))
                {
                    var taggerProvider = this.componentModel.DefaultExportProvider.GetExportedValues<ITaggerProvider>().OfType<TemplateErrorReporterProvider>().Single();
                    Assert.IsNull(taggerProvider.CreateTagger<ErrorTag>(document.TextBuffer));
                }
            });
        }

        [TestMethod]
        public async Task CreateTaggerCreatesTemplateErrorReporter()
        {
            await UIThreadDispatcher.InvokeAsync(delegate
            {
                string tempFile = this.CreateTempFile(Path.GetTempPath(), string.Empty);
                using (ITextDocument document = this.documentFactory.CreateAndLoadTextDocument(tempFile, this.textContentType))
                {
                    TemplateErrorReporterProvider taggerProvider = this.componentModel.DefaultExportProvider.GetExportedValues<ITaggerProvider>().OfType<TemplateErrorReporterProvider>().Single();
                    taggerProvider.CreateTagger<ErrorTag>(document.TextBuffer);
                    Assert.IsTrue(document.TextBuffer.Properties.ContainsProperty(typeof(TemplateErrorReporter)));
                }
            });
        }

        [TestMethod]
        public async Task CreateTaggerDoesNotCreateTemplateErrorReporterWhenErrorReportingIsDisabled()
        {
            await UIThreadDispatcher.InvokeAsync(delegate
            {
                string tempFile = this.CreateTempFile(Path.GetTempPath(), string.Empty);
                using (ITextDocument document = this.documentFactory.CreateAndLoadTextDocument(tempFile, this.textContentType))
                {
                    TemplateErrorReporterProvider taggerProvider = this.componentModel.DefaultExportProvider.GetExportedValues<ITaggerProvider>().OfType<TemplateErrorReporterProvider>().Single();

                    T4ToolboxOptions.Instance.ErrorReportingEnabled = false;
                    taggerProvider.CreateTagger<ErrorTag>(document.TextBuffer);
                    Assert.IsFalse(document.TextBuffer.Properties.ContainsProperty(typeof(TemplateErrorReporter)));
                    T4ToolboxOptions.Instance.ErrorReportingEnabled = true;
                }
            });
        }
    }
}