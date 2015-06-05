// <copyright file="TemplateErrorReporterTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.IntegrationTests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using EnvDTE;
    using EnvDTE80;
    using Microsoft.VisualStudio.ComponentModelHost;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Utilities;
    
    using T4Toolbox.VisualStudio.Editor;

    [TestClass]
    public class TemplateErrorReporterTest : IntegrationTest
    {
        private IContentType textContentType;
        private ITextDocumentFactoryService documentFactory;

        public TemplateErrorReporterTest()
        {
            UIThreadDispatcher.Invoke(delegate
            {
                var componentModel = (IComponentModel)ServiceProvider.GetService(typeof(SComponentModel));
                
                // Use Text content type to test in isolation from TemplateErrorTaggerProvider
                var contentTypeRegistry = componentModel.DefaultExportProvider.GetExportedValue<IContentTypeRegistryService>();
                this.textContentType = contentTypeRegistry.GetContentType("Text");

                this.documentFactory = componentModel.DefaultExportProvider.GetExportedValue<ITextDocumentFactoryService>();
            });
        }

        [TestMethod]
        public async Task TemplateErrorReporterCreatesErrorItemsForTemplateErrors()
        {
            await UIThreadDispatcher.InvokeAsync(delegate
            {
                string tempFile = this.CreateTempFile(Path.GetTempPath(), "<#@");
                using (ITextDocument document = this.documentFactory.CreateAndLoadTextDocument(tempFile, this.textContentType))
                {
                    TemplateErrorReporter.GetOrCreate(document.TextBuffer, IntegrationTest.ServiceProvider, this.documentFactory);
                    Assert.AreEqual(1, IntegrationTest.ErrorItems.Count(e => e.FileName.Equals(tempFile, StringComparison.OrdinalIgnoreCase)));                    
                }
            });
        }

        [TestMethod]
        public async Task TemplateErrorReporterPopulatesLineAndColumnNumberInErrorItems()
        {
            await UIThreadDispatcher.InvokeAsync(delegate
            {
                string tempFile = this.CreateTempFile(Path.GetTempPath(), "<#@");
                using (ITextDocument document = this.documentFactory.CreateAndLoadTextDocument(tempFile, this.textContentType))
                {
                    TemplateErrorReporter.GetOrCreate(document.TextBuffer, IntegrationTest.ServiceProvider, this.documentFactory);
                    ErrorItem errorItem = IntegrationTest.ErrorItems.Single(e => e.FileName.Equals(tempFile, StringComparison.OrdinalIgnoreCase));
                    Assert.AreNotEqual(0, errorItem.Line);
                    Assert.AreNotEqual(0, errorItem.Column);
                }
            });
        }

        [TestMethod]
        public async Task TemplateErrorReporterCreatesErrorItemsThatNavigateToSourceOfError()
        {
            await UIThreadDispatcher.InvokeAsync(delegate
            {
                const string Template = "text <#@<# code #>"; // an error in the middle of the line
                string fileName = this.CreateTempFile(Path.GetTempPath(), Template);
                using (ITextDocument document = this.documentFactory.CreateAndLoadTextDocument(fileName, this.textContentType))
                {
                    TemplateErrorReporter.GetOrCreate(document.TextBuffer, IntegrationTest.ServiceProvider, this.documentFactory);
                    ErrorItem errorItem = IntegrationTest.ErrorItems.Single(e => e.FileName.Equals(fileName, StringComparison.OrdinalIgnoreCase));
                    errorItem.Navigate();

                    Assert.AreEqual(fileName, Dte.ActiveDocument.FullName);

                    var activeDocument = (TextDocument)Dte.ActiveDocument.Object();
                    Assert.AreEqual(Template.LastIndexOf("<#", System.StringComparison.Ordinal) + 1, activeDocument.Selection.CurrentColumn);
                }
            });
        }

        [TestMethod]
        public async Task TemplateErrorReporterClearsErrorItemsWhenTextDocumentIsDisposed()
        {
            await UIThreadDispatcher.InvokeAsync(delegate
            {
                string tempFile = this.CreateTempFile(Path.GetTempPath(), "<#@");
                using (ITextDocument document = this.documentFactory.CreateAndLoadTextDocument(tempFile, this.textContentType))
                {
                    TemplateErrorReporter.GetOrCreate(document.TextBuffer, IntegrationTest.ServiceProvider, this.documentFactory);
                }

                Assert.AreEqual(0, IntegrationTest.ErrorItems.Count(e => e.FileName.Equals(tempFile, StringComparison.OrdinalIgnoreCase)));
            });
        }

        [TestMethod]
        public async Task TemplateErrorReporterIsNotGarbageCollectedBeforeTextDocumentIsDisposed()
        {
            await UIThreadDispatcher.InvokeAsync(delegate
            {
                string tempFile = this.CreateTempFile(Path.GetTempPath(), string.Empty);
                using (ITextDocument document = this.documentFactory.CreateAndLoadTextDocument(tempFile, this.textContentType))
                {
                    var errorProvider = new WeakReference(TemplateErrorReporter.GetOrCreate(document.TextBuffer, ServiceProvider, this.documentFactory));
                    GC.Collect(2, GCCollectionMode.Forced);
                    GC.WaitForPendingFinalizers();
                    Assert.IsTrue(errorProvider.IsAlive);
                }
            });
        }

        [TestMethod]
        public async Task TemplateErrorReporterCanBeGarbageCollectedAfterTextDocumentIsDisposed()
        {
            await UIThreadDispatcher.InvokeAsync(delegate
            {
                WeakReference errorProvider;
                string tempFile = this.CreateTempFile(Path.GetTempPath(), string.Empty);
                using (ITextDocument document = this.documentFactory.CreateAndLoadTextDocument(tempFile, this.textContentType))
                {
                    errorProvider = new WeakReference(TemplateErrorReporter.GetOrCreate(document.TextBuffer, ServiceProvider, this.documentFactory));
                }

                GC.Collect(2, GCCollectionMode.Forced);
                GC.WaitForPendingFinalizers();
                Assert.IsFalse(errorProvider.IsAlive);
            });
        }

        [TestMethod]
        public async Task TemplateErrorReporterUpdatesErrorItemsWhenTemplateChanges()
        {
            await UIThreadDispatcher.InvokeAsync(delegate
            {
                string tempFile = this.CreateTempFile(Path.GetTempPath(), "<#@"); // syntax error expected
                using (ITextDocument document = this.documentFactory.CreateAndLoadTextDocument(tempFile, this.textContentType))
                {
                    TemplateErrorReporter.GetOrCreate(document.TextBuffer, IntegrationTest.ServiceProvider, this.documentFactory);

                    // Change template to fix the error
                    using (ITextEdit edit = document.TextBuffer.CreateEdit())
                    {
                        edit.Delete(0, 3);
                        edit.Apply();
                    }

                    Assert.AreEqual(0, IntegrationTest.ErrorItems.Count(e => e.FileName.Equals(tempFile, StringComparison.OrdinalIgnoreCase)));
                }
            });
        }

        [TestMethod, SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "GetOrCreate is the target method name")]
        public async Task GetOrCreateStoresErrorProviderInBufferProperties()
        {
            await UIThreadDispatcher.InvokeAsync(delegate
            {
                string tempFile = this.CreateTempFile(Path.GetTempPath(), string.Empty);
                using (ITextDocument document = this.documentFactory.CreateAndLoadTextDocument(tempFile, this.textContentType))
                {
                    var errorProvider = TemplateErrorReporter.GetOrCreate(document.TextBuffer, ServiceProvider, this.documentFactory);
                    TemplateErrorReporter propertyValue;
                    Assert.IsTrue(document.TextBuffer.Properties.TryGetProperty(typeof(TemplateErrorReporter), out propertyValue));
                    Assert.AreSame(errorProvider, propertyValue);
                }
            });
        }

        [TestMethod, SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "GetOrCreate is the target method name")]
        public async Task GetOrCreateDoesNotCreateNewTemplateErrorReporterIfItAlreadyExistsForTextBuffer()
        {
            // Visual Studio creates separate instances of the error tagger to display squiglies and tooltops. 
            // However, only a single instance of the TemplateErrorReporter should be created to avoid duplicate items in the Error List window.
            await UIThreadDispatcher.InvokeAsync(delegate
            {
                string tempFile = this.CreateTempFile(Path.GetTempPath(), string.Empty);
                using (ITextDocument document = this.documentFactory.CreateAndLoadTextDocument(tempFile, this.textContentType))
                {
                    var errorProvider1 = TemplateErrorReporter.GetOrCreate(document.TextBuffer, ServiceProvider, this.documentFactory);
                    var errorProvider2 = TemplateErrorReporter.GetOrCreate(document.TextBuffer, ServiceProvider, this.documentFactory);
                    Assert.AreSame(errorProvider1, errorProvider2);
                }
            });
        }

        [TestMethod]
        public async Task DisposeRemovesErrorProviderFromBufferProperties()
        {
            await UIThreadDispatcher.InvokeAsync(delegate
            {
                string tempFile = this.CreateTempFile(Path.GetTempPath(), string.Empty);
                using (ITextDocument document = this.documentFactory.CreateAndLoadTextDocument(tempFile, this.textContentType))
                {
                    var errorProvider = TemplateErrorReporter.GetOrCreate(document.TextBuffer, ServiceProvider, this.documentFactory);
                    errorProvider.Dispose();
                    Assert.IsFalse(document.TextBuffer.Properties.TryGetProperty(typeof(TemplateErrorReporter), out errorProvider));
                }
            });
        }
    }
}