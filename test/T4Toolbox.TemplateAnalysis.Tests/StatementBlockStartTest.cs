// <copyright file="StatementBlockStartTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.TemplateAnalysis
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.VisualStudio.Text;
    using NSubstitute;

    [TestClass]
    public class StatementBlockStartTest
    {
        [TestMethod]
        public void StatementBlockStartIsSubclassOfCodeBlockStart()
        {
            Assert.IsTrue(typeof(StatementBlockStart).IsSubclassOf(typeof(CodeBlockStart)));
        }

        [TestMethod]
        public void StatementBlockStartIsSealed()
        {
            Assert.IsTrue(typeof(StatementBlockStart).IsSealed);
        }

        [TestMethod]
        public void AcceptCallsAcceptStatementBlockStartMethodOfSyntaxNodeVisitor()
        {
            var visitor = Substitute.For<SyntaxNodeVisitor>();
            var node = new StatementBlockStart(default(int));
            node.Accept(visitor);
            visitor.Received().VisitStatementBlockStart(node);
        }

        [TestMethod]
        public void GetDescriptionReturnsDescriptionOfStatementCodeBlocks()
        {
            var target = new StatementBlockStart(0);
            string description;
            Span applicableTo;
            Assert.IsTrue(target.TryGetDescription(0, out description, out applicableTo));
            StringAssert.Contains(description, "statement");
            Assert.AreEqual(target.Span, applicableTo);
        }

        [TestMethod]
        public void KindReturnsStatementBlockStartSyntaxKind()
        {
            Assert.AreEqual(SyntaxKind.StatementBlockStart, new StatementBlockStart(0).Kind);
        }

        [TestMethod]
        public void SpanLengthReturnsLengthOfTheToken()
        {
            Assert.AreEqual("<#".Length, new StatementBlockStart(0).Span.Length);
        }
    }
}