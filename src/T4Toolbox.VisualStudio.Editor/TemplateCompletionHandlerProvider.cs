// <copyright file="TemplateCompletionHandlerProvider.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.Editor
{
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
        // TODO: make private fields initialized by constructor
        [Import]internal IVsEditorAdaptersFactoryService AdapterFactory;
        [Import]internal SVsServiceProvider ServiceProvider;
        [Import]internal ICompletionBroker CompletionBroker;

        private readonly ITemplateEditorOptions options;

        [ImportingConstructor]
        public TemplateCompletionHandlerProvider(ITemplateEditorOptions options)
        {
            // TODO: throw ArgumentNullException
            this.options = options;
        }

        public void VsTextViewCreated(IVsTextView viewAdapter)
        {
            Debug.Assert(this.AdapterFactory != null, "AdapterFactory");
            Debug.Assert(viewAdapter != null, "viewAdapter");

            if (!this.options.CompletionListsEnabled)
            {
                return;
            }

            IWpfTextView textView = this.AdapterFactory.GetWpfTextView(viewAdapter);
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
            handler.ServiceProvider = this.ServiceProvider;
            handler.CompletionBroker = this.CompletionBroker;
            ErrorHandler.ThrowOnFailure(viewAdapter.AddCommandFilter(handler, out handler.NextHandler));
            return handler;
        }
    }
}