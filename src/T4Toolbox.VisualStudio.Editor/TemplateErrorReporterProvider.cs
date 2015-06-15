// <copyright file="TemplateErrorReporterProvider.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.Editor
{
    using System;
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Tagging;
    using Microsoft.VisualStudio.Utilities;

    [Export(typeof(ITaggerProvider)), TagType(typeof(ErrorTag)), ContentType(TemplateContentType.Name)]
    internal sealed class TemplateErrorReporterProvider : ITaggerProvider
    {
        private readonly ITemplateEditorOptions options;
        private readonly IServiceProvider serviceProvider;
        private readonly ITextDocumentFactoryService textDocumentFactory;

        [ImportingConstructor]
        public TemplateErrorReporterProvider(
            ITemplateEditorOptions options, 
            SVsServiceProvider serviceProvider,
            ITextDocumentFactoryService textDocumentFactory)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (serviceProvider == null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }

            if (textDocumentFactory == null)
            {
                throw new ArgumentNullException(nameof(textDocumentFactory));
            }

            this.options = options;
            this.serviceProvider = serviceProvider;
            this.textDocumentFactory = textDocumentFactory;
        }

        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {
            if (this.options.ErrorReportingEnabled)
            {
                TemplateErrorReporter.GetOrCreate(buffer, this.serviceProvider, this.textDocumentFactory);
            }

            return null;
        }
    }
}