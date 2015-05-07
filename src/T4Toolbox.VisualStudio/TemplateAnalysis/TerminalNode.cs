// <copyright file="TerminalNode.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.TemplateAnalysis
{
    using System.Collections.Generic;
    using System.Linq;

    internal abstract class TerminalNode : SyntaxNode
    {
        private readonly Position position;

        protected TerminalNode(Position position)
        {
            this.position = position;
        }

        public override Position Position
        {
            get { return this.position; }
        }

        public sealed override IEnumerable<SyntaxNode> ChildNodes()
        {
            return Enumerable.Empty<TerminalNode>();
        }

        public override string ToString()
        {
            return this.Kind.ToString() + this.Span.ToString() + this.Position.ToString();
        }

        public override IEnumerable<TemplateError> Validate()
        {
            return Enumerable.Empty<TemplateError>();
        }
    }
}