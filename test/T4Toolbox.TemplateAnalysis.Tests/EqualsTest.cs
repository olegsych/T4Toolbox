// <copyright file="EqualsTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.TemplateAnalysis
{
    using NSubstitute;
    using Xunit;

    public static class EqualsTest
    {
        [Fact]
        public static void EqualsIsSubclassOfSyntaxToken()
        {
            Assert.True(typeof(Equals).IsSubclassOf(typeof(SyntaxToken)));
        }

        [Fact]
        public static void EqualsIsSealed()
        {
            Assert.True(typeof(Equals).IsSealed);
        }

        [Fact]
        public static void KindReturnsEqualsSyntaxKind()
        {
            Assert.Equal(SyntaxKind.Equals, new Equals(0).Kind);
        }

        [Fact]
        public static void SpanLengthReturnsLengthOfToken()
        {
            Assert.Equal("=".Length, new Equals(0).Span.Length);
        }

        [Fact]
        public static void AcceptCallsVisitEqualsMethodOfSyntaxNodeVisitor()
        {
            var visitor = Substitute.For<SyntaxNodeVisitor>();
            var node = new Equals(default(int));
            node.Accept(visitor);
            visitor.Received().VisitEquals(node);
        }
    }
}