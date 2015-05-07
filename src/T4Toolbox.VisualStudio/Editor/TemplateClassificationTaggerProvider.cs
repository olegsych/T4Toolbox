// <copyright file="TemplateClassificationTaggerProvider.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.Editor
{
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Classification;
    using Microsoft.VisualStudio.Text.Tagging;
    using Microsoft.VisualStudio.Utilities;

    [Export(typeof(ITaggerProvider)), TagType(typeof(ClassificationTag)), ContentType(TemplateContentType.Name)]
    internal sealed class TemplateClassificationTaggerProvider : ITaggerProvider
    {
        [Import]
        internal IClassificationTypeRegistryService ClassificationRegistry { private get; set; }

        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {
            if (T4ToolboxOptions.Instance.SyntaxColorizationEnabled)
            {
                return buffer.Properties.GetOrCreateSingletonProperty(() => new TemplateClassificationTagger(buffer, this.ClassificationRegistry)) as ITagger<T>;
            }

            return null;
        }
    }
}