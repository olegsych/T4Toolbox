// <copyright file="TemplateCompletionHandlerProvider.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.Editor
{
    using System;
    using System.ComponentModel.Composition;
    using System.Diagnostics;
    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.Editor;
    using Microsoft.VisualStudio.Language.Intellisense;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.TextManager.Interop;
    using Microsoft.VisualStudio.Utilities;

    /// <summary>
    /// Creates a <see cref="TemplateCompletionHandler"/> for text template files opened in Visual Studio editor.
    /// </summary>
    [Export(typeof(IVsTextViewCreationListener)), ContentType(TemplateContentType.Name), TextViewRole(PredefinedTextViewRoles.Editable)]
    internal sealed class TemplateCompletionHandlerProvider : IVsTextViewCreationListener
    {
        private readonly ITemplateEditorOptions options;
        private readonly IVsEditorAdaptersFactoryService editorAdapterFactory;
        private readonly IServiceProvider serviceProvider;
        private readonly ICompletionBroker completionBroker;

        [ImportingConstructor]
        public TemplateCompletionHandlerProvider(
            ITemplateEditorOptions options, 
            IVsEditorAdaptersFactoryService editorAdapterFactory,
            SVsServiceProvider serviceProvider,
            ICompletionBroker completionBroker)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (editorAdapterFactory == null)
            {
                throw new ArgumentNullException(nameof(editorAdapterFactory));
            }

            if (serviceProvider == null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }

            if (completionBroker == null)
            {
                throw new ArgumentNullException(nameof(completionBroker));
            }

            this.options = options;
            this.editorAdapterFactory = editorAdapterFactory;
            this.serviceProvider = serviceProvider;
            this.completionBroker = completionBroker;
        }

        public void VsTextViewCreated(IVsTextView viewAdapter)
        {
            Debug.Assert(viewAdapter != null, "viewAdapter");

            if (!this.options.CompletionListsEnabled)
            {
                return;
            }

            IWpfTextView textView = this.editorAdapterFactory.GetWpfTextView(viewAdapter);
            if (textView == null)
            {
                return;
            }

            textView.Properties.GetOrCreateSingletonProperty(() => this.CreateHandler(viewAdapter, textView));
        }

        private TemplateCompletionHandler CreateHandler(IVsTextView viewAdapter, IWpfTextView textView)
        {
            var handler = new TemplateCompletionHandler();
            handler.TextView = textView;
            handler.ServiceProvider = this.serviceProvider;
            handler.CompletionBroker = this.completionBroker;
            ErrorHandler.ThrowOnFailure(viewAdapter.AddCommandFilter(handler, out handler.NextHandler));
            return handler;
        }
    }
}