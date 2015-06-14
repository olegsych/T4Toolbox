// <copyright file="DirectiveBlockStartTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.TemplateAnalysis
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.VisualStudio.Text;
    using NSubstitute;

    [TestClass]
    public class DirectiveBlockStartTest
    {
        [TestMethod]
        public void DirectiveBlockStartIsSubclassOfSyntaxToken()
        {
            Assert.IsTrue(typeof(DirectiveBlockStart).IsSubclassOf(typeof(SyntaxToken)));
        }

        [TestMethod]
        public void DirectiveBlockStartIsSealed()
        {
            Assert.IsTrue(typeof(DirectiveBlockStart).IsSealed);
        }

        [TestMethod]
        public void AcceptCallsVisitDirectiveBlockStartMethodOfSyntaxNodeVisitor()
        {
            var visitor = Substitute.For<SyntaxNodeVisitor>();
            var node = new DirectiveBlockStart(default(int));
            node.Accept(visitor);
            visitor.Received().VisitDirectiveBlockStart(node);
        }

        [TestMethod]
        public void GetDescriptionReturnsDescriptionOfDirectiveBlockStart()
        {
            var target = new DirectiveBlockStart(0);
            string description;
            Span applicableTo;
            Assert.IsTrue(target.TryGetDescription(0, out description, out applicableTo));
            StringAssert.Contains(description, "directive");
            Assert.AreEqual(target.Span, applicableTo);
        }

        [TestMethod]
        public void KindReturnsDirectiveBlockStartSyntaxKind()
        {
            Assert.AreEqual(SyntaxKind.DirectiveBlockStart, new DirectiveBlockStart(0).Kind);
        }

        [TestMethod]
        public void SpanLengthReturnsLengthOfToken()
        {
            Assert.AreEqual("<#@".Length, new DirectiveBlockStart(0).Span.Length);
        }
    }
}