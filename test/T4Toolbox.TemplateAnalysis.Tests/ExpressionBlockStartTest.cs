// <copyright file="ExpressionBlockStartTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.TemplateAnalysis
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.VisualStudio.Text;
    using NSubstitute;

    [TestClass]
    public class ExpressionBlockStartTest
    {
        [TestMethod]
        public void ExpressionBlockStartIsSubclassOfCodeBlockStart()
        {
            Assert.IsTrue(typeof(ExpressionBlockStart).IsSubclassOf(typeof(CodeBlockStart)));
        }

        [TestMethod]
        public void ExpressionBlockStartIsSealed()
        {
            Assert.IsTrue(typeof(ExpressionBlockStart).IsSealed);
        }

        [TestMethod]
        public void AcceptCallsVisitExpressionBlockStartMethodOfSyntaxNodeVisitor()
        {
            var visitor = Substitute.For<SyntaxNodeVisitor>();
            var node = new ExpressionBlockStart(default(int));
            node.Accept(visitor);
            visitor.Received().VisitExpressionBlockStart(node);
        }

        [TestMethod]
        public void GetDescriptionReturnsDescriptionOfExpressionCodeBlocks()
        {
            var target = new ExpressionBlockStart(0);
            string description;
            Span applicableTo;
            Assert.IsTrue(target.TryGetDescription(0, out description, out applicableTo));
            StringAssert.Contains(description, "expression");
            Assert.AreEqual(target.Span, applicableTo);
        }

        [TestMethod]
        public void KindReturnsExpressionBlockStartSyntaxKind()
        {
            Assert.AreEqual(SyntaxKind.ExpressionBlockStart, new ExpressionBlockStart(0).Kind);
        }

        [TestMethod]
        public void SpanLengthReturnsLengthOfToken()
        {
            Assert.AreEqual("<#=".Length, new ExpressionBlockStart(0).Span.Length);
        }
    }
}