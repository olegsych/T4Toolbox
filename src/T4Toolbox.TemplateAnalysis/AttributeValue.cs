// <copyright file="AttributeValue.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.TemplateAnalysis
{
    internal sealed class AttributeValue : CaptureNode
    {
        public AttributeValue(int start, string text, Position position = default(Position)) : base(start, text, position)
        {
        }

        public override SyntaxKind Kind
        {
            get { return SyntaxKind.AttributeValue; }
        }

        protected internal override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitAttributeValue(this);
        }
    }
}