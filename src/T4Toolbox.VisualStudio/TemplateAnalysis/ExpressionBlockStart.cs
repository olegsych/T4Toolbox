// <copyright file="ExpressionBlockStart.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.TemplateAnalysis
{
    using System.ComponentModel;
    using Microsoft.VisualStudio.Text;

    [Description("Starts an expression control block, which contains a Visual Basic or Visual C# expression that will be converted to a string and written to the output file.")]
    internal sealed class ExpressionBlockStart : CodeBlockStart
    {
        public ExpressionBlockStart(int start, Position position = default(Position)) : base(start, position)
        {
        }

        public override SyntaxKind Kind
        {
            get { return SyntaxKind.ExpressionBlockStart; }
        }

        public override Span Span
        {
            get { return new Span(this.Start, 3); }
        }

        protected internal override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitExpressionBlockStart(this);
        }
    }
}