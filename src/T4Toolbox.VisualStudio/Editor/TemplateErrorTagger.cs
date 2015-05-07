// <copyright file="TemplateErrorTagger.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.Editor
{
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Adornments;
    using Microsoft.VisualStudio.Text.Tagging;
    using T4Toolbox.VisualStudio.TemplateAnalysis;

    internal sealed class TemplateErrorTagger : TemplateTagger<ErrorTag>
    {
        public TemplateErrorTagger(ITextBuffer buffer) : base(buffer)
        {
            this.UpdateTagSpans(this.Analyzer.CurrentAnalysis);
        }

        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "This is an internal method, called by base class.")]
        protected override void CreateTagSpans(TemplateAnalysis analysis)
        {
            ITextSnapshot snapshot = analysis.TextSnapshot;
            foreach (TemplateError error in analysis.Errors)
            {
                this.CreateTagSpan(snapshot.CreateTrackingSpan(error.Span, SpanTrackingMode.EdgeNegative), new ErrorTag(PredefinedErrorTypeNames.SyntaxError, error.Message));
            }
        }
    }
}