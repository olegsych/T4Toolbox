// <copyright file="Code.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.TemplateAnalysis
{
    using Microsoft.VisualStudio.Text;

    internal sealed class Code : TerminalNode
    {
        private readonly Span span;

        public Code(Span span, Position position = default(Position)) : base(position)
        {
            this.span = span;
        }

        public override SyntaxKind Kind
        {
            get { return SyntaxKind.Code; }
        }

        public override Span Span
        {
            get { return this.span; }
        }

        protected internal override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitCode(this);
        }
    }
}