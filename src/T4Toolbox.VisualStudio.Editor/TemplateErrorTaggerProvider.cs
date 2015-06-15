// <copyright file="TemplateErrorTaggerProvider.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.Editor
{
    using System;
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Tagging;
    using Microsoft.VisualStudio.Utilities;

    [Export(typeof(ITaggerProvider)), TagType(typeof(ErrorTag)), ContentType(TemplateContentType.Name)]
    internal sealed class TemplateErrorTaggerProvider : ITaggerProvider
    {
        private readonly ITemplateEditorOptions options;

        [ImportingConstructor]
        public TemplateErrorTaggerProvider(ITemplateEditorOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            this.options = options;
        }

        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            if (this.options.ErrorUnderliningEnabled)
            {
                return buffer.Properties.GetOrCreateSingletonProperty(() => new TemplateErrorTagger(buffer)) as ITagger<T>;
            }

            return null;
        }
    }
}