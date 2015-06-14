// <copyright file="EndOfFile.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.TemplateAnalysis
{
    using Microsoft.VisualStudio.Text;

    internal sealed class EndOfFile : SyntaxToken
    {
        public EndOfFile(int start, Position position = default(Position)) : base(start, position)
        {
        }

        public override SyntaxKind Kind
        {
            get { return SyntaxKind.EOF; }
        }

        public override Span Span
        {
            get { return new Span(this.Start, 0); }
        }

        protected internal override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitEndOfFile(this);
        }
    }
}