// <copyright file="BlockEnd.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.TemplateAnalysis
{
    using System.ComponentModel;
    using Microsoft.VisualStudio.Text;

    [Description("Ends a text template control block or directive.")]
    internal sealed class BlockEnd : SyntaxToken
    {
        public BlockEnd(int start, Position position = default(Position)) : base(start, position)
        {
        }

        public override SyntaxKind Kind
        {
            get { return SyntaxKind.BlockEnd; }
        }

        public override Span Span
        {
            get { return new Span(this.Start, 2); }
        }

        protected internal override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitBlockEnd(this);
        }
    }
}