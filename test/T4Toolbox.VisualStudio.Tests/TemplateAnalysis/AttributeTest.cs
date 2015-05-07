// <copyright file="AttributeTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.TemplateAnalysis
{
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using NSubstitute;

    [TestClass]
    public class AttributeTest
    {
        [TestMethod]
        public void AttributeIsSubclassOfNonterminalNode()
        {
            Assert.IsTrue(typeof(Attribute).IsSubclassOf(typeof(NonterminalNode)));
        }

        [TestMethod]
        public void AttributeIsSealed()
        {
            Assert.IsTrue(typeof(Attribute).IsSealed);
        }

        [TestMethod]
        public void AcceptCallsVisitAttributeMethodOfSyntaxNodeVisitor()
        {
            var visitor = Substitute.For<SyntaxNodeVisitor>();
            var attribute = new Attribute(new AttributeName(0, string.Empty), new Equals(0), new DoubleQuote(0), new AttributeValue(0, string.Empty), new DoubleQuote(0));
            attribute.Accept(visitor);
            visitor.Received().VisitAttribute(attribute);
        }

        [TestMethod]
        public void ChildNodesReturnsNodesSpecifiedInConstructor()
        {
            var name = new AttributeName(0, "language");
            var equals = new Equals(8);
            var quote1 = new DoubleQuote(9);
            var value = new AttributeValue(10, "C#");
            var quote2 = new DoubleQuote(12);
            var attribute = new Attribute(name, equals, quote1, value, quote2);
            Assert.IsTrue(new SyntaxNode[] { name, equals, quote1, value, quote2 }.SequenceEqual(attribute.ChildNodes()));
        }

        [TestMethod]
        public void KindReturnsAttributeSyntaxKind()
        {
            var attribute = new Attribute(new AttributeName(0, "language"), new Equals(8), new DoubleQuote(9), new AttributeValue(10, "C#"), new DoubleQuote(12));
            Assert.AreEqual(SyntaxKind.Attribute, attribute.Kind);
        }

        [TestMethod]
        public void NameReturnsTextOfAttributeName()
        {
            var attribute = new Attribute(new AttributeName(0, "language"), new Equals(8), new DoubleQuote(9), new AttributeValue(10, "C#"), new DoubleQuote(12));
            Assert.AreEqual("language", attribute.Name);
        }

        [TestMethod]
        public void PositionReturnsPositionOfAttributeName()
        {
            var attribute = new Attribute(new AttributeName(0, "language", new Position(4, 2)), new Equals(8), new DoubleQuote(9), new AttributeValue(10, "C#"), new DoubleQuote(12));
            Assert.AreEqual(new Position(4, 2), attribute.Position);
        }

        [TestMethod]
        public void ValueReturnsTextOfAttributeValue()
        {
            var attribute = new Attribute(new AttributeName(0, "language"), new Equals(8), new DoubleQuote(9), new AttributeValue(10, "C#"), new DoubleQuote(12));
            Assert.AreEqual("C#", attribute.Value);
        }

        [TestMethod]
        public void SpanStartsAtName()
        {
            AttributeName name;
            var attribute = new Attribute(
                name = new AttributeName(10, "language"), 
                new Equals(18), 
                new DoubleQuote(19), 
                new AttributeValue(20, "C#"), 
                new DoubleQuote(22));
            Assert.AreEqual(name.Span.Start, attribute.Span.Start);
        }

        [TestMethod]
        public void SpanEndsAtSecondDoubleQuote()
        {
            DoubleQuote quote2;
            var attribute = new Attribute(
                new AttributeName(10, "language"),
                new Equals(18),
                new DoubleQuote(19),
                new AttributeValue(20, "C#"),
                quote2 = new DoubleQuote(22));
            Assert.AreEqual(quote2.Span.End, attribute.Span.End);
        }
    }
}