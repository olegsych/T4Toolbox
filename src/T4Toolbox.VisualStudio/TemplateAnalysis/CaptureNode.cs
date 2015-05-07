// <copyright file="CaptureNode.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.TemplateAnalysis
{
    using Microsoft.VisualStudio.Text;

    internal abstract class CaptureNode : TerminalNode
    {
        private readonly int start;
        private readonly string text;

        protected CaptureNode(int start, string text, Position position) : base(position)
        {
            this.start = start;
            this.text = text;
        }

        public string Text
        {
            get { return this.text; }
        }

        public sealed override Span Span
        {
            get { return new Span(this.start, this.text.Length); }
        }
    }
}