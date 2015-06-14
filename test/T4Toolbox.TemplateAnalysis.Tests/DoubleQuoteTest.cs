// <copyright file="DoubleQuoteTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.TemplateAnalysis
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using NSubstitute;

    [TestClass]
    public class DoubleQuoteTest
    {
        [TestMethod]
        public void DoubleQuoteIsSubclassOfSyntaxToken()
        {
            Assert.IsTrue(typeof(DoubleQuote).IsSubclassOf(typeof(SyntaxToken)));
        }

        [TestMethod]
        public void DoubleQuoteIsSealed()
        {
            Assert.IsTrue(typeof(DoubleQuote).IsSealed);
        }

        [TestMethod]
        public void KindReturnsDoubleQuoteSyntaxKind()
        {            
            Assert.AreEqual(SyntaxKind.DoubleQuote, new DoubleQuote(0).Kind);
        }

        [TestMethod]
        public void SpanLengthReturnsLengthOfToken()
        {
            Assert.AreEqual("\"".Length, new DoubleQuote(0).Span.Length);
        }

        [TestMethod]
        public void AcceptCallsVisitDoubleQuoteMethodOfSyntaxNodeVisitor()
        {
            var visitor = Substitute.For<SyntaxNodeVisitor>();
            var syntaxNode = new DoubleQuote(default(int));
            syntaxNode.Accept(visitor);
            visitor.Received().VisitDoubleQuote(syntaxNode);
        }
    }
}