// <copyright file="CodeBlock.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.TemplateAnalysis
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using Microsoft.VisualStudio.Text;

    internal sealed class CodeBlock : NonterminalNode
    {
        private readonly CodeBlockStart start;
        private readonly Code code;
        private readonly BlockEnd end;

        public CodeBlock(CodeBlockStart start, BlockEnd end)
        {
            Debug.Assert(start != null, "start");
            Debug.Assert(end != null, "end");
            this.start = start;
            this.end = end;
        }

        public CodeBlock(CodeBlockStart start, Code code, BlockEnd end) : this(start, end)
        {
            Debug.Assert(code != null, "code");
            this.code = code;
        }

        public BlockEnd End
        {
            get { return this.end; }
        }    

        public override SyntaxKind Kind
        {
            get { return SyntaxKind.CodeBlock; }
        }

        public override Position Position
        {
            get { return this.start.Position; }
        }

        public override Span Span
        {
            get { return Span.FromBounds(this.start.Span.Start, this.end.Span.End); }
        }

        public CodeBlockStart Start
        {
            get { return this.start; }
        }

        public override IEnumerable<SyntaxNode> ChildNodes()
        {
            yield return this.start;

            if (this.code != null)
            {
                yield return this.code;
            }

            yield return this.end;
        }

        protected internal override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitCodeBlock(this);
        }
    }
}