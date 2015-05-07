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
        [Import]
        private SVsServiceProvider serviceProvider = null;

        [Import]
        private ITextDocumentFactoryService documentFactory = null;

        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {
            if (T4ToolboxOptions.Instance.ErrorReportingEnabled)
            {
                TemplateErrorReporter.GetOrCreate(buffer, this.serviceProvider, this.documentFactory);
            }

            return null;
        }
    }
}