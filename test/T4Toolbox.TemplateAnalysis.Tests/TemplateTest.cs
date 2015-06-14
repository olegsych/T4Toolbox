// <copyright file="TemplateTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.TemplateAnalysis
{
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.VisualStudio.Text;
    using NSubstitute;

    [TestClass]
    public class TemplateTest
    {
        [TestMethod]
        public void TemplateIsSubclassOfNonterminalNode()
        {
            Assert.IsTrue(typeof(Template).IsSubclassOf(typeof(NonterminalNode)));
        }

        [TestMethod]
        public void TemplateIsSealed()
        {
            Assert.IsTrue(typeof(Template).IsSealed);
        }

        [TestMethod]
        public void AcceptCallsVisitTemplateMethodOfSyntaxNodeVisitor()
        {
            var visitor = Substitute.For<SyntaxNodeVisitor>();
            var template = new Template();
            template.Accept(visitor);
            visitor.Received().VisitTemplate(template);
        }

        [TestMethod]
        public void KindReturnsTemplateSyntaxKind()
        {
            Assert.AreEqual(SyntaxKind.Template, new Template().Kind);
        }

        [TestMethod]
        public void PositionReturnsFixedValue()
        {
            var target = new Template();
            Assert.AreEqual(new Position(0, 0), target.Position);
        }

        [TestMethod]
        public void SpanStartReturnsMinStartOfChildNodes()
        {
            var end = new BlockEnd(40);
            var start = new StatementBlockStart(4);
            var target = new Template(end, start); // intentionally reversed
            Assert.AreEqual(start.Span.Start, target.Span.Start);
        }

        [TestMethod]
        public void SpanEndReturnsMaxEndOfChildNodes()
        {
            var end = new BlockEnd(40);
            var start = new StatementBlockStart(4);
            var target = new Template(end, start); // intentionally reversed
            Assert.AreEqual(end.Span.End, target.Span.End);
        }

        [TestMethod]
        public void SpanIsEmptyWhenNodeHasNoChildren()
        {
            Assert.AreEqual(new Span(), new Template().Span);
        }

        [TestMethod]
        public void ChildNodesReturnsNodesSpecifiedInConstructor()
        {
            var start = new StatementBlockStart(0);
            var end = new BlockEnd(2);
            var node = new Template(start, end);
            Assert.AreSame(start, node.ChildNodes().First());
            Assert.AreSame(end, node.ChildNodes().Last());
        }
    }
}