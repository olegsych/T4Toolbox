﻿// <copyright file="TemplateQuickInfoSourceProvider.cs" company="Oleg Sych">
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
        private readonly ITemplateEditorOptions options;

        [ImportingConstructor]
        public TemplateQuickInfoSourceProvider(ITemplateEditorOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            this.options = options;
        }

        public IAsyncQuickInfoSource TryCreateQuickInfoSource(ITextBuffer buffer)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            if (this.options.QuickInfoTooltipsEnabled)
            {
                return buffer.Properties.GetOrCreateSingletonProperty(() => new TemplateQuickInfoSource(buffer));                
            }

            return null;
        }
    }
}