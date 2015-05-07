// <copyright file="DirectiveBlockStart.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.TemplateAnalysis
{
    using System.ComponentModel;
    using Microsoft.VisualStudio.Text;

    [Description("Starts a directive, which provides instructions to the text template transformation engine on how to process the template.")]
    internal sealed class DirectiveBlockStart : SyntaxToken
    {
        public DirectiveBlockStart(int start, Position position = default(Position)) : base(start, position)
        {
        }

        public override SyntaxKind Kind
        {
            get { return SyntaxKind.DirectiveBlockStart; }
        }

        public override Span Span
        {
            get { return new Span(this.Start, 3); }
        }

        protected internal override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitDirectiveBlockStart(this);
        }
    }
}