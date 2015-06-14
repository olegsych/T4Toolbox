// <copyright file="TemplateCompletionSource.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.Editor
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Microsoft.VisualStudio.Language.Intellisense;
    using Microsoft.VisualStudio.Text;
    using T4Toolbox.TemplateAnalysis;

    internal sealed class TemplateCompletionSource : ICompletionSource
    {
        private readonly ITextBuffer buffer;
        private readonly TemplateAnalyzer analyzer;

        internal TemplateCompletionSource(ITextBuffer buffer)
        {
            Debug.Assert(buffer != null, "buffer");

            this.buffer = buffer;
            this.analyzer = TemplateAnalyzer.GetOrCreate(buffer);
        }

        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "This method is called by the Visual Studio editor and expects to receive valid arguments")]
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "This method is called by the Visual Studio editor and expects to receive valid arguments")]
        public void AugmentCompletionSession(ICompletionSession session, IList<CompletionSet> completionSets)
        {
            Debug.Assert(session != null, "session");
            Debug.Assert(completionSets != null, "completionSets");

            TemplateAnalysis current = this.analyzer.CurrentAnalysis;
            var builder = new TemplateCompletionBuilder(session.GetTriggerPoint(current.TextSnapshot).Value.Position);
            builder.Visit(current.Template);
            if (builder.Completions != null)
            {
                ITrackingSpan applicableTo = current.TextSnapshot.CreateTrackingSpan(builder.Node.Span, SpanTrackingMode.EdgeInclusive);
                IEnumerable<Completion> completions = builder.Completions.OrderBy(completion => completion.DisplayText);
                var completionSet = new CompletionSet("All", "All", applicableTo, completions, null);
                completionSets.Add(completionSet);
            }
        }

        public void Dispose()
        {
            this.buffer.Properties.RemoveProperty(this.GetType());
        }
    }
}