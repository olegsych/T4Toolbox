// <copyright file="TemplateAnalyzer.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.TemplateAnalysis
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Microsoft.VisualStudio.Text;

    internal sealed class TemplateAnalyzer
    {
        private TemplateAnalyzer(ITextBuffer textBuffer)
        {
            textBuffer.Changed += this.BufferChanged;
            this.ParseTextSnapshot(textBuffer.CurrentSnapshot);
        }

        public event EventHandler<TemplateAnalysis> TemplateChanged;

        public TemplateAnalysis CurrentAnalysis { get; private set; }

        public static TemplateAnalyzer GetOrCreate(ITextBuffer buffer)
        {
            Debug.Assert(buffer != null, "buffer");
            return buffer.Properties.GetOrCreateSingletonProperty(() => new TemplateAnalyzer(buffer));
        }

        private void BufferChanged(object sender, TextContentChangedEventArgs e)
        {
            this.ParseTextSnapshot(e.After);
        }

        private void OnTemplateChanged(TemplateAnalysis templateAnalysis)
        {
            this.CurrentAnalysis = templateAnalysis;

            EventHandler<TemplateAnalysis> handler = this.TemplateChanged;
            if (handler != null)
            {
                handler(this, templateAnalysis);
            }
        }

        private void ParseTextSnapshot(ITextSnapshot snapshot)
        {
            var scanner = new TemplateScanner(snapshot.GetText());

            var parser = new TemplateParser(scanner);
            parser.Parse();

            // Always return a template object, even if the parser couldn't build one, to avoid downstream errors.
            Template template = parser.Template ?? new Template();

            var errors = new List<TemplateError>(parser.Errors);
            errors.AddRange(template.Validate());

            this.OnTemplateChanged(new TemplateAnalysis(snapshot, template, errors));
        }
    }
}