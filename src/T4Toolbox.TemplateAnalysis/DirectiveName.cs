// <copyright file="DirectiveName.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.TemplateAnalysis
{
    internal sealed class DirectiveName : CaptureNode
    {
        public DirectiveName(int start, string text, Position position = default(Position)) : base(start, text, position)
        {
        }

        public override SyntaxKind Kind
        {
            get { return SyntaxKind.DirectiveName; }
        }

        protected internal override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitDirectiveName(this);
        }
    }
}