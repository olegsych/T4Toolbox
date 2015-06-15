// <copyright file="TemplateClassificationTaggerProvider.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.Editor
{
    using System;
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Classification;
    using Microsoft.VisualStudio.Text.Tagging;
    using Microsoft.VisualStudio.Utilities;

    [Export(typeof(ITaggerProvider)), TagType(typeof(ClassificationTag)), ContentType(TemplateContentType.Name)]
    internal sealed class TemplateClassificationTaggerProvider : ITaggerProvider
    {
        private readonly ITemplateEditorOptions options;
        private readonly IClassificationTypeRegistryService classificationTypeRegistry;

        [ImportingConstructor]
        public TemplateClassificationTaggerProvider(ITemplateEditorOptions options, IClassificationTypeRegistryService classificationTypeRegistry)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (classificationTypeRegistry == null)
            {
                throw new ArgumentNullException(nameof(classificationTypeRegistry));
            }

            this.options = options;
            this.classificationTypeRegistry = classificationTypeRegistry;
        }

        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {
            if (this.options.SyntaxColorizationEnabled)
            {
                return buffer.Properties.GetOrCreateSingletonProperty(() => new TemplateClassificationTagger(buffer, this.classificationTypeRegistry)) as ITagger<T>;
            }

            return null;
        }
    }
}