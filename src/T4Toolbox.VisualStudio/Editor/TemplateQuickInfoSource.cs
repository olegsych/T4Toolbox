// <copyright file="TemplateQuickInfoSource.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.Editor
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Microsoft.VisualStudio.Language.Intellisense;
    using Microsoft.VisualStudio.Text;
    using T4Toolbox.VisualStudio.TemplateAnalysis;

    internal sealed class TemplateQuickInfoSource : IQuickInfoSource
    {
        private readonly TemplateAnalyzer analyzer;

        public TemplateQuickInfoSource(ITextBuffer buffer)
        {
            Debug.Assert(buffer != null, "buffer");
            this.analyzer = TemplateAnalyzer.GetOrCreate(buffer);
        }

        public void AugmentQuickInfoSession(IQuickInfoSession session, IList<object> quickInfoContent, out ITrackingSpan applicableToSpan)
        {
            if (session == null)
            {
                throw new ArgumentNullException("session");
            }

            if (quickInfoContent == null)
            {
                throw new ArgumentNullException("quickInfoContent");
            }

            TemplateAnalysis analysis = this.analyzer.CurrentAnalysis;
            SnapshotPoint? triggerPoint = session.GetTriggerPoint(analysis.TextSnapshot);
            if (triggerPoint != null && analysis.Template != null)
            {
                string description;
                Span applicableTo;
                if (analysis.Template.TryGetDescription(triggerPoint.Value.Position, out description, out applicableTo))
                {
                    quickInfoContent.Add(description);
                    applicableToSpan = analysis.TextSnapshot.CreateTrackingSpan(applicableTo, SpanTrackingMode.EdgeExclusive); 
                    return;                    
                }
            }

            applicableToSpan = null;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}