// <copyright file="BlockEndTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.TemplateAnalysis
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.VisualStudio.Text;
    using NSubstitute;

    [TestClass]
    public class BlockEndTest
    {
        [TestMethod]
        public void BlockEndIsSubclassOfSyntaxToken()
        {
            Assert.IsTrue(typeof(BlockEnd).IsSubclassOf(typeof(SyntaxToken)));
        }

        [TestMethod]
        public void BlockEndIsSealed()
        {
            Assert.IsTrue(typeof(BlockEnd).IsSealed);
        }

        [TestMethod]
        public void GetDescriptionReturnsDescriptionOfDirectiveBlockStart()
        {
            var target = new BlockEnd(0);
            string description;
            Span applicableTo;
            Assert.IsTrue(target.TryGetDescription(0, out description, out applicableTo));
            StringAssert.Contains(description, "control block");
            StringAssert.Contains(description, "directive");
            Assert.AreEqual(target.Span, applicableTo);
        }

        [TestMethod]
        public void KindReturnsBlockEndSyntaxKind()
        {
            Assert.AreEqual(SyntaxKind.BlockEnd, new BlockEnd(0).Kind);
        }

        [TestMethod]
        public void SpanLengthReturnsLengthOfToken()
        {
            Assert.AreEqual("#>".Length, new BlockEnd(0).Span.Length);
        }

        [TestMethod]
        public void AcceptCallsVisitBlockEndMethodOfSyntaxNodeVisitor()
        {
            var visitor = Substitute.For<SyntaxNodeVisitor>();
            var node = new BlockEnd(0);
            node.Accept(visitor);
            visitor.Received().VisitBlockEnd(node);
        }
    }
}