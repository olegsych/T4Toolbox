// <copyright file="DoubleQuote.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.TemplateAnalysis
{
    using Microsoft.VisualStudio.Text;

    internal sealed class DoubleQuote : SyntaxToken
    {
        public DoubleQuote(int start, Position position = default(Position)) : base(start, position)
        {
        }

        public override SyntaxKind Kind
        {
            get { return SyntaxKind.DoubleQuote; }
        }

        public override Span Span
        {
            get { return new Span(this.Start, 1); } 
        }

        protected internal override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitDoubleQuote(this);
        }
    }
}