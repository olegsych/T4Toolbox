// <copyright file="TemplateOutliningTagger.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.Editor
{
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Text;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Tagging;
    using T4Toolbox.VisualStudio.TemplateAnalysis;

    internal sealed class TemplateOutliningTagger : TemplateTagger<OutliningRegionTag>
    {
        public TemplateOutliningTagger(ITextBuffer buffer) : base(buffer)
        {
            this.UpdateTagSpans(this.Analyzer.CurrentAnalysis);
        }

        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "This is an internal method, called by base class.")]
        protected override void CreateTagSpans(TemplateAnalysis analysis)
        {
            // If text buffer contains recognizable template
            Template template = analysis.Template;
            if (template != null)
            {
                ITextSnapshot snapshot = analysis.TextSnapshot;
                string text = snapshot.GetText();
                foreach (CodeBlock codeBlock in template.ChildNodes().OfType<CodeBlock>())
                {
                    ITrackingSpan trackingSpan = snapshot.CreateTrackingSpan(codeBlock.Span, SpanTrackingMode.EdgeNegative);

                    string collapsedForm = GetCollapsedForm(codeBlock, text);
                    string collapsedHintForm = GetCollapsedHintForm(codeBlock, text);
                    var tag = new OutliningRegionTag(collapsedForm, collapsedHintForm);

                    this.CreateTagSpan(trackingSpan, tag);
                }
            }
        }

        private static string GetCollapsedForm(CodeBlock codeBlock, string template)
        {
            var text = new StringBuilder();
            text.Append(codeBlock.Start.GetText(template));
            text.Append("...");
            text.Append(codeBlock.End.GetText(template));
            return text.ToString();
        }

        private static string GetCollapsedHintForm(CodeBlock codeBlock, string template)
        {
            var text = new StringBuilder();

            using (var reader = new StringReader(codeBlock.GetText(template)))
            {
                for (int i = 0; i < 10; i++)
                {
                    string line = reader.ReadLine();
                    if (line == null)
                    {
                        return text.ToString();
                    }

                    // Append new line manually to avoid unnecessary \r\n at the end
                    if (i > 0)
                    {
                        text.AppendLine();
                    }

                    text.Append(line);
                }

                text.AppendLine();
                text.Append("...");
            }

            return text.ToString();
        }
    }
}