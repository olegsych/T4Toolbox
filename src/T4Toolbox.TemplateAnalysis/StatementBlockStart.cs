// <copyright file="StatementBlockStart.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.TemplateAnalysis
{
    using System.ComponentModel;
    using Microsoft.VisualStudio.Text;

    [Description("Starts a statement control block, which contains Visual Basic or Visual C# code that will become a part of the TransformText method in the compiled TextTransformation class.")]
    internal sealed class StatementBlockStart : CodeBlockStart
    {
        public StatementBlockStart(int start, Position position = default(Position)) : base(start, position)
        {
        }

        public override SyntaxKind Kind
        {
            get { return SyntaxKind.StatementBlockStart; }
        }

        public override Span Span
        {
            get { return new Span(this.Start, 2); }
        }

        protected internal override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitStatementBlockStart(this);
        }
    }
}