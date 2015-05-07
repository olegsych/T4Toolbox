// <copyright file="Equals.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.TemplateAnalysis
{
    using Microsoft.VisualStudio.Text;

    internal sealed class Equals : SyntaxToken
    {
        public Equals(int start, Position position = default(Position)) : base(start, position)
        {
        }

        public override SyntaxKind Kind
        {
            get { return SyntaxKind.Equals; }
        }

        public override Span Span
        {
            get { return new Span(this.Start, 1); }
        }

        protected internal override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitEquals(this);
        }
    }
}