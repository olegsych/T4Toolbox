// <copyright file="TemplateErrorReporterProvider.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.Editor
{
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Tagging;
    using Microsoft.VisualStudio.Utilities;

    [Export(typeof(ITaggerProvider)), TagType(typeof(ErrorTag)), ContentType(TemplateContentType.Name)]
    internal sealed class TemplateErrorReporterProvider : ITaggerProvider
    {
        private readonly ITemplateEditorOptions options;

        // TODO: change to private fields
        [Import]
        private SVsServiceProvider serviceProvider = null;

        [Import]
        private ITextDocumentFactoryService documentFactory = null;

        [ImportingConstructor]
        public TemplateErrorReporterProvider(ITemplateEditorOptions options)
        {
            // TODO: throw ArgumentNullException
            this.options = options;
        }

        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {
            if (this.options.ErrorReportingEnabled)
            {
                TemplateErrorReporter.GetOrCreate(buffer, this.serviceProvider, this.documentFactory);
            }

            return null;
        }
    }
}