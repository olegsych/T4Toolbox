// <copyright file="TemplateTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.TemplateAnalysis
{
    using System.Linq;
    using Microsoft.VisualStudio.Text;
    using NSubstitute;
    using Xunit;

    public static class TemplateTest
    {
        [Fact]
        public static void TemplateIsSubclassOfNonterminalNode()
        {
            Assert.True(typeof(Template).IsSubclassOf(typeof(NonterminalNode)));
        }

        [Fact]
        public static void TemplateIsSealed()
        {
            Assert.True(typeof(Template).IsSealed);
        }

        [Fact]
        public static void AcceptCallsVisitTemplateMethodOfSyntaxNodeVisitor()
        {
            var visitor = Substitute.For<SyntaxNodeVisitor>();
            var template = new Template();
            template.Accept(visitor);
            visitor.Received().VisitTemplate(template);
        }

        [Fact]
        public static void KindReturnsTemplateSyntaxKind()
        {
            Assert.Equal(SyntaxKind.Template, new Template().Kind);
        }

        [Fact]
        public static void PositionReturnsFixedValue()
        {
            var target = new Template();
            Assert.Equal(new Position(0, 0), target.Position);
        }

        [Fact]
        public static void SpanStartReturnsMinStartOfChildNodes()
        {
            var end = new BlockEnd(40);
            var start = new StatementBlockStart(4);
            var target = new Template(end, start); // intentionally reversed
            Assert.Equal(start.Span.Start, target.Span.Start);
        }

        [Fact]
        public static void SpanEndReturnsMaxEndOfChildNodes()
        {
            var end = new BlockEnd(40);
            var start = new StatementBlockStart(4);
            var target = new Template(end, start); // intentionally reversed
            Assert.Equal(end.Span.End, target.Span.End);
        }

        [Fact]
        public static void SpanIsEmptyWhenNodeHasNoChildren()
        {
            Assert.Equal(new Span(), new Template().Span);
        }

        [Fact]
        public static void ChildNodesReturnsNodesSpecifiedInConstructor()
        {
            var start = new StatementBlockStart(0);
            var end = new BlockEnd(2);
            var node = new Template(start, end);
            Assert.Same(start, node.ChildNodes().First());
            Assert.Same(end, node.ChildNodes().Last());
        }
    }
}