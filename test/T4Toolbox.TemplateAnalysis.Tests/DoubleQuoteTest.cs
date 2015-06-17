// <copyright file="DoubleQuoteTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.TemplateAnalysis
{
    using NSubstitute;
    using Xunit;

    public static class DoubleQuoteTest
    {
        [Fact]
        public static void DoubleQuoteIsSubclassOfSyntaxToken()
        {
            Assert.True(typeof(DoubleQuote).IsSubclassOf(typeof(SyntaxToken)));
        }

        [Fact]
        public static void DoubleQuoteIsSealed()
        {
            Assert.True(typeof(DoubleQuote).IsSealed);
        }

        [Fact]
        public static void KindReturnsDoubleQuoteSyntaxKind()
        {            
            Assert.Equal(SyntaxKind.DoubleQuote, new DoubleQuote(0).Kind);
        }

        [Fact]
        public static void SpanLengthReturnsLengthOfToken()
        {
            Assert.Equal("\"".Length, new DoubleQuote(0).Span.Length);
        }

        [Fact]
        public static void AcceptCallsVisitDoubleQuoteMethodOfSyntaxNodeVisitor()
        {
            var visitor = Substitute.For<SyntaxNodeVisitor>();
            var syntaxNode = new DoubleQuote(default(int));
            syntaxNode.Accept(visitor);
            visitor.Received().VisitDoubleQuote(syntaxNode);
        }
    }
}