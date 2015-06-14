// <copyright file="EqualsTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.TemplateAnalysis
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using NSubstitute;

    [TestClass]
    public class EqualsTest
    {
        [TestMethod]
        public void EqualsIsSubclassOfSyntaxToken()
        {
            Assert.IsTrue(typeof(Equals).IsSubclassOf(typeof(SyntaxToken)));
        }

        [TestMethod]
        public void EqualsIsSealed()
        {
            Assert.IsTrue(typeof(Equals).IsSealed);
        }

        [TestMethod]
        public void KindReturnsEqualsSyntaxKind()
        {
            Assert.AreEqual(SyntaxKind.Equals, new Equals(0).Kind);
        }

        [TestMethod]
        public void SpanLengthReturnsLengthOfToken()
        {
            Assert.AreEqual("=".Length, new Equals(0).Span.Length);
        }

        [TestMethod]
        public void AcceptCallsVisitEqualsMethodOfSyntaxNodeVisitor()
        {
            var visitor = Substitute.For<SyntaxNodeVisitor>();
            var node = new Equals(default(int));
            node.Accept(visitor);
            visitor.Received().VisitEquals(node);
        }
    }
}