// <copyright file="Attribute.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.TemplateAnalysis
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using Microsoft.VisualStudio.Text;

    internal sealed class Attribute : NonterminalNode
    {
        private readonly AttributeName name;
        private readonly Equals equals;
        private readonly DoubleQuote quote1;
        private readonly AttributeValue value;
        private readonly DoubleQuote quote2;

        public Attribute(AttributeName name, Equals equals, DoubleQuote quote1, AttributeValue value, DoubleQuote quote2)
        {
            Debug.Assert(name != null, "name");
            Debug.Assert(equals != null, "equals");
            Debug.Assert(quote1 != null, "quote1");
            Debug.Assert(value != null, "value");
            Debug.Assert(quote2 != null, "quote2");

            this.name = name;
            this.equals = equals;
            this.quote1 = quote1;
            this.value = value;
            this.quote2 = quote2;
        }

        public override SyntaxKind Kind
        {
            get { return SyntaxKind.Attribute; }
        }

        public string Name
        {
            get { return this.name.Text; }
        }

        public override Position Position
        {
            get { return this.name.Position; }
        }

        public override Span Span
        {
            get { return Span.FromBounds(this.name.Span.Start, this.quote2.Span.End); }
        }

        public string Value
        {
            get { return this.value.Text; }
        }

        public override IEnumerable<SyntaxNode> ChildNodes()
        {
            yield return this.name;
            yield return this.equals;
            yield return this.quote1;
            yield return this.value;
            yield return this.quote2;
        }

        protected internal override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitAttribute(this);
        }
    }
}