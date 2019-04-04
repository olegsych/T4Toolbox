// <copyright file="TemplateQuickInfoSource.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.Editor
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.Language.Intellisense;
    using Microsoft.VisualStudio.Text;

    internal sealed class TemplateQuickInfoSource : IAsyncQuickInfoSource
    {
        private readonly TemplateAnalyzer analyzer;

        public TemplateQuickInfoSource(ITextBuffer buffer)
        {
            Debug.Assert(buffer != null, "buffer");
            analyzer = TemplateAnalyzer.GetOrCreate(buffer);
        }

        public Task<QuickInfoItem> GetQuickInfoItemAsync(IAsyncQuickInfoSession session, CancellationToken cancellationToken)
        {
            QuickInfoItem quickInfoItem = null;
            if (session == null)
            {
                throw new ArgumentNullException("session");
            }

            TemplateAnalysis analysis = analyzer.CurrentAnalysis;
            SnapshotPoint? triggerPoint = session.GetTriggerPoint(analysis.TextSnapshot);
            if (triggerPoint != null && analysis.Template != null)
            {
                string description;
                Span applicableTo;
                if (analysis.Template.TryGetDescription(triggerPoint.Value.Position, out description, out applicableTo))
                {
                    ITrackingSpan applicableToSpan = analysis.TextSnapshot.CreateTrackingSpan(applicableTo, SpanTrackingMode.EdgeExclusive);
                    quickInfoItem = new QuickInfoItem(applicableToSpan, description);
                }
            }

            return Task.FromResult(quickInfoItem);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}