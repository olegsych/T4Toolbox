// <copyright file="EndOfFileTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.TemplateAnalysis
{
    using NSubstitute;
    using Xunit;

    public static class EndOfFileTest
    {
        [Fact]
        public static void EndOfFileIsSubclassOfSyntaxToken()
        {
            Assert.True(typeof(EndOfFile).IsSubclassOf(typeof(SyntaxToken)));
        }

        [Fact]
        public static void EndOfFileIsSealed()
        {
            Assert.True(typeof(EndOfFile).IsSealed);
        }

        [Fact]
        public static void KindReturnsEndOfFileSyntaxKind()
        {
            Assert.Equal(SyntaxKind.EOF, new EndOfFile(0).Kind);
        }

        [Fact]
        public static void SpanLengthReturnsLengthOfToken()
        {
            Assert.Equal(0, new EndOfFile(0).Span.Length);
        }

        [Fact]
        public static void AcceptCallsVisitEndOfFileMethodOfSyntaxNodeVisitor()
        {
            var visitor = Substitute.For<SyntaxNodeVisitor>();
            var node = new EndOfFile(default(int));
            node.Accept(visitor);
            visitor.Received().VisitEndOfFile(node);
        }
    }
}