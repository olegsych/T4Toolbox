// <copyright file="AttributeName.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.TemplateAnalysis
{
    internal sealed class AttributeName : CaptureNode
    {
        public AttributeName(int start, string text, Position position = default(Position)) : base(start, text, position)
        {
        }

        public override SyntaxKind Kind
        {
            get { return SyntaxKind.AttributeName; }
        }

        protected internal override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitAttributeName(this);
        }
    }
}