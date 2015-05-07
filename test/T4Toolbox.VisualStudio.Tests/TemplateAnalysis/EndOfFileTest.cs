// <copyright file="EndOfFileTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.TemplateAnalysis
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using NSubstitute;

    [TestClass]
    public class EndOfFileTest
    {
        [TestMethod]
        public void EndOfFileIsSubclassOfSyntaxToken()
        {
            Assert.IsTrue(typeof(EndOfFile).IsSubclassOf(typeof(SyntaxToken)));
        }

        [TestMethod]
        public void EndOfFileIsSealed()
        {
            Assert.IsTrue(typeof(EndOfFile).IsSealed);
        }

        [TestMethod]
        public void KindReturnsEndOfFileSyntaxKind()
        {
            Assert.AreEqual(SyntaxKind.EOF, new EndOfFile(0).Kind);
        }

        [TestMethod]
        public void SpanLengthReturnsLengthOfToken()
        {
            Assert.AreEqual(0, new EndOfFile(0).Span.Length);
        }

        [TestMethod]
        public void AcceptCallsVisitEndOfFileMethodOfSyntaxNodeVisitor()
        {
            var visitor = Substitute.For<SyntaxNodeVisitor>();
            var node = new EndOfFile(default(int));
            node.Accept(visitor);
            visitor.Received().VisitEndOfFile(node);
        }
    }
}