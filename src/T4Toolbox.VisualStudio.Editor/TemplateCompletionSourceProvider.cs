// <copyright file="TemplateCompletionSourceProvider.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.Editor
{
    using System.ComponentModel.Composition;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.VisualStudio.Language.Intellisense;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Utilities;

    [Export(typeof(ICompletionSourceProvider)), Name(TemplateContentType.Name), ContentType(TemplateContentType.Name)]
    internal sealed class TemplateCompletionSourceProvider : ICompletionSourceProvider
    {
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "This method is called by only by the Visual Studio editor and assumes that a textBuffer is supplied")]
        public ICompletionSource TryCreateCompletionSource(ITextBuffer textBuffer)
        {
            Debug.Assert(textBuffer != null, "textBuffer");
            return textBuffer.Properties.GetOrCreateSingletonProperty(() => new TemplateCompletionSource(textBuffer));
        }
    }
}