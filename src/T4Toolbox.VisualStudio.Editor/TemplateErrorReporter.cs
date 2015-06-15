// <copyright file="TemplateErrorReporter.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.Editor
{
    using System;
    using System.Diagnostics;
    using System.Windows;
    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Shell.Interop;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.TextManager.Interop;
    using T4Toolbox.TemplateAnalysis;

    /// <summary>
    /// Displays template errors in the Error List window.
    /// </summary>
    internal sealed class TemplateErrorReporter : IDisposable
    {
        private readonly TemplateAnalyzer analyzer;
        private readonly ITextDocument document;
        private readonly IServiceProvider serviceProvider;

        private ErrorListProvider errorListProvider;

        private TemplateErrorReporter(ITextBuffer buffer, IServiceProvider serviceProvider, ITextDocumentFactoryService documentFactory)
        {
            Debug.Assert(buffer != null, "buffer");
            Debug.Assert(serviceProvider != null, "serviceProvider");
            Debug.Assert(documentFactory != null, "documentFactory");

            this.serviceProvider = serviceProvider;
            
            documentFactory.TryGetTextDocument(buffer, out this.document);
            WeakEventManager<ITextDocumentFactoryService, TextDocumentEventArgs>.AddHandler(documentFactory, "TextDocumentDisposed", this.DocumentDisposed);

            this.analyzer = TemplateAnalyzer.GetOrCreate(buffer);
            WeakEventManager<TemplateAnalyzer, TemplateAnalysis>.AddHandler(this.analyzer, "TemplateChanged", this.TemplateChanged);

            this.UpdateErrorTasks(this.analyzer.CurrentAnalysis);
        }

        public static TemplateErrorReporter GetOrCreate(ITextBuffer buffer, IServiceProvider serviceProvider, ITextDocumentFactoryService documentFactory)
        {
            return buffer.Properties.GetOrCreateSingletonProperty(() => new TemplateErrorReporter(buffer, serviceProvider, documentFactory));
        }

        public void Dispose()
        {
            this.document.TextBuffer.Properties.RemoveProperty(typeof(TemplateErrorReporter));

            if (this.errorListProvider != null)
            {
                this.errorListProvider.Dispose();
                this.errorListProvider = null;
            }
        }

        private void DocumentDisposed(object sender, TextDocumentEventArgs e)
        {
            if (e.TextDocument == this.document)
            {
                this.Dispose();
            }
        }

        private void TemplateChanged(object sender, TemplateAnalysis e)
        {
            this.UpdateErrorTasks(e);
        }

        private void UpdateErrorTasks(TemplateAnalysis templateAnalysis)
        {
            if (this.errorListProvider != null)
            {
                this.errorListProvider.Tasks.Clear();
            }
            else if (templateAnalysis.Errors.Count > 0)
            {
                this.errorListProvider = new ErrorListProvider(this.serviceProvider);
            }

            foreach (TemplateError error in templateAnalysis.Errors)
            {
                var errorTask = new ErrorTask();
                errorTask.Document = this.document.FilePath;
                errorTask.Category = TaskCategory.BuildCompile;
                errorTask.Text = error.Message;
                errorTask.ErrorCategory = TaskErrorCategory.Error;
                errorTask.Line = error.Position.Line;
                errorTask.Column = error.Position.Column;
                errorTask.Navigate += this.NavigateToError;
                this.errorListProvider.Tasks.Add(errorTask);
            }
        }

        private void NavigateToError(object sender, EventArgs e)
        {
            var errorTask = (ErrorTask)sender;
            
            IVsUIHierarchy hierarchyItem;
            uint num;
            IVsWindowFrame windowFrame;
            VsShellUtilities.OpenDocument(this.serviceProvider, errorTask.Document, Guid.Empty, out hierarchyItem, out num, out windowFrame);
            if (windowFrame != null)
            {
                errorTask.HierarchyItem = hierarchyItem;
                this.errorListProvider.Refresh();
                IVsTextView textView = VsShellUtilities.GetTextView(windowFrame);
                if (textView != null)
                {
                    ErrorHandler.ThrowOnFailure(textView.SetSelection(errorTask.Line, errorTask.Column, errorTask.Line, errorTask.Column));
                }
            }
        }
    }
}