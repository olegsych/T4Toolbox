// <copyright file="Template.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.TemplateAnalysis
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Microsoft.VisualStudio.Text;

    internal sealed class Template : NonterminalNode
    {        
        private readonly SyntaxNode[] childNodes;

        public Template(params SyntaxNode[] childNodes)
        {
            Debug.Assert(childNodes != null, "childNodes");
            this.childNodes = childNodes;
        }

        public override SyntaxKind Kind
        {
            get { return SyntaxKind.Template; }
        }

        public override Position Position
        {
            get { return new Position(0, 0); }
        }

        public override Span Span
        {
            get
            {
                if (this.childNodes.Length == 0)
                {
                    return new Span();
                }

                int start = int.MaxValue;
                int end = 0;
                foreach (SyntaxNode child in this.childNodes)
                {
                    start = Math.Min(start, child.Span.Start);
                    end = Math.Max(end, child.Span.End);
                }

                return Span.FromBounds(start, end);
            }
        }

        public override IEnumerable<SyntaxNode> ChildNodes()
        {
            return this.childNodes;
        }

        protected internal override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitTemplate(this);
        }
    }
}