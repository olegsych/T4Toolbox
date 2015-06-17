// <copyright file="AttributeTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.TemplateAnalysis
{
    using System.Linq;
    using NSubstitute;
    using Xunit;

    public static class AttributeTest
    {
        [Fact]
        public static void AttributeIsSubclassOfNonterminalNode()
        {
            Assert.True(typeof(Attribute).IsSubclassOf(typeof(NonterminalNode)));
        }

        [Fact]
        public static void AttributeIsSealed()
        {
            Assert.True(typeof(Attribute).IsSealed);
        }

        [Fact]
        public static void AcceptCallsVisitAttributeMethodOfSyntaxNodeVisitor()
        {
            var visitor = Substitute.For<SyntaxNodeVisitor>();
            var attribute = new Attribute(new AttributeName(0, string.Empty), new Equals(0), new DoubleQuote(0), new AttributeValue(0, string.Empty), new DoubleQuote(0));
            attribute.Accept(visitor);
            visitor.Received().VisitAttribute(attribute);
        }

        [Fact]
        public static void ChildNodesReturnsNodesSpecifiedInConstructor()
        {
            var name = new AttributeName(0, "language");
            var equals = new Equals(8);
            var quote1 = new DoubleQuote(9);
            var value = new AttributeValue(10, "C#");
            var quote2 = new DoubleQuote(12);
            var attribute = new Attribute(name, equals, quote1, value, quote2);
            Assert.True(new SyntaxNode[] { name, equals, quote1, value, quote2 }.SequenceEqual(attribute.ChildNodes()));
        }

        [Fact]
        public static void KindReturnsAttributeSyntaxKind()
        {
            var attribute = new Attribute(new AttributeName(0, "language"), new Equals(8), new DoubleQuote(9), new AttributeValue(10, "C#"), new DoubleQuote(12));
            Assert.Equal(SyntaxKind.Attribute, attribute.Kind);
        }

        [Fact]
        public static void NameReturnsTextOfAttributeName()
        {
            var attribute = new Attribute(new AttributeName(0, "language"), new Equals(8), new DoubleQuote(9), new AttributeValue(10, "C#"), new DoubleQuote(12));
            Assert.Equal("language", attribute.Name);
        }

        [Fact]
        public static void PositionReturnsPositionOfAttributeName()
        {
            var attribute = new Attribute(new AttributeName(0, "language", new Position(4, 2)), new Equals(8), new DoubleQuote(9), new AttributeValue(10, "C#"), new DoubleQuote(12));
            Assert.Equal(new Position(4, 2), attribute.Position);
        }

        [Fact]
        public static void ValueReturnsTextOfAttributeValue()
        {
            var attribute = new Attribute(new AttributeName(0, "language"), new Equals(8), new DoubleQuote(9), new AttributeValue(10, "C#"), new DoubleQuote(12));
            Assert.Equal("C#", attribute.Value);
        }

        [Fact]
        public static void SpanStartsAtName()
        {
            AttributeName name;
            var attribute = new Attribute(
                name = new AttributeName(10, "language"), 
                new Equals(18), 
                new DoubleQuote(19), 
                new AttributeValue(20, "C#"), 
                new DoubleQuote(22));
            Assert.Equal(name.Span.Start, attribute.Span.Start);
        }

        [Fact]
        public static void SpanEndsAtSecondDoubleQuote()
        {
            DoubleQuote quote2;
            var attribute = new Attribute(
                new AttributeName(10, "language"),
                new Equals(18),
                new DoubleQuote(19),
                new AttributeValue(20, "C#"),
                quote2 = new DoubleQuote(22));
            Assert.Equal(quote2.Span.End, attribute.Span.End);
        }
    }
}