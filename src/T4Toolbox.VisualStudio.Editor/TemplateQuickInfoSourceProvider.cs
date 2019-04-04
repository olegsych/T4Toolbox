// <copyright file="TemplateQuickInfoSourceProvider.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.Editor
{
    using System;
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Language.Intellisense;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Utilities;

    [Export(typeof(IAsyncQuickInfoSourceProvider)), ContentType(TemplateContentType.Name)]
    [Name("Template Quick Info Source"), Order(Before = "Default Quick Info Presenter")]
    internal sealed class TemplateQuickInfoSourceProvider : IAsyncQuickInfoSourceProvider
    {
        private readonly ITemplateEditorOptions _options;

        [ImportingConstructor]
        public TemplateQuickInfoSourceProvider(ITemplateEditorOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            _options = options;
        }

        public IAsyncQuickInfoSource TryCreateQuickInfoSource(ITextBuffer textBuffer)
        {
            if (textBuffer == null)
            {
                throw new ArgumentNullException(nameof(textBuffer));
            }

            if (_options.QuickInfoTooltipsEnabled)
            {
                return textBuffer.Properties.GetOrCreateSingletonProperty(() => new TemplateQuickInfoSource(textBuffer));
            }

            return null;
        }
    }
}