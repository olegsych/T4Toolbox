// <copyright file="ClassBlockStart.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.TemplateAnalysis
{
    using System.ComponentModel;
    using Microsoft.VisualStudio.Text;

    [Description("Starts a class feature control block, which contains Visual Basic or Visual C# code (methods, fields, properties, etc.) that will become a part of the compiled TextTransformation class.")]
    internal sealed class ClassBlockStart : CodeBlockStart
    {
        public ClassBlockStart(int start, Position position = default(Position)) : base(start, position)
        {
        }

        public override SyntaxKind Kind
        {
            get { return SyntaxKind.ClassBlockStart; }
        }

        public override Span Span
        {
            get { return new Span(this.Start, 3); }
        }

        protected internal override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitClassBlockStart(this);
        }
    }
}