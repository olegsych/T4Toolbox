// <copyright file="TemplateTagger.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.Editor
{
    using System.Windows;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Tagging;
    using T4Toolbox.TemplateAnalysis;

    internal abstract class TemplateTagger<T> : SimpleTagger<T> where T : ITag
    {
        private readonly TemplateAnalyzer analyzer;

        protected TemplateTagger(ITextBuffer buffer) : base(buffer)
        {
            this.analyzer = TemplateAnalyzer.GetOrCreate(buffer);
            WeakEventManager<TemplateAnalyzer, TemplateAnalysis>.AddHandler(this.analyzer, "TemplateChanged", this.TemplateChanged);
        }

        protected TemplateAnalyzer Analyzer
        {
            get { return this.analyzer; }
        }

        protected abstract void CreateTagSpans(TemplateAnalysis analysis);

        protected void UpdateTagSpans(TemplateAnalysis templateAnalysis)
        {
            using (this.Update())
            {
                this.RemoveTagSpans(trackingTagSpan => true); // remove all tag spans
                this.CreateTagSpans(templateAnalysis);
            }
        }

        private void TemplateChanged(object sender, TemplateAnalysis currentAnalysis)
        {
            this.UpdateTagSpans(currentAnalysis);
        }
    }
}