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

    [Export(typeof(IQuickInfoSourceProvider)), ContentType(TemplateContentType.Name)]
    [Name("Template Quick Info Source"), Order(Before = "Default Quick Info Presenter")]
    internal sealed class TemplateQuickInfoSourceProvider : IQuickInfoSourceProvider
    {
        public IQuickInfoSource TryCreateQuickInfoSource(ITextBuffer buffer)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }

            if (T4ToolboxOptions.Instance.QuickInfoTooltipsEnabled)
            {
                return buffer.Properties.GetOrCreateSingletonProperty(() => new TemplateQuickInfoSource(buffer));                
            }

            return null;
        }
    }
}