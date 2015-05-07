// <copyright file="CodeBlockTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.TemplateAnalysis
{
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.VisualStudio.Text;
    using NSubstitute;

    [TestClass]
    public class CodeBlockTest
    {
        [TestMethod]
        public void CodeBlockIsSubclassOfNonterminalNode()
        {
            Assert.IsTrue(typeof(CodeBlock).IsSubclassOf(typeof(NonterminalNode)));
        }

        [TestMethod]
        public void CodeBlockIsSealed()
        {
            Assert.IsTrue(typeof(CodeBlock).IsSealed);
        }

        [TestMethod]
        public void KindReturnsCodeBlockSyntaxKind()
        {
            Assert.AreEqual(SyntaxKind.CodeBlock, new CodeBlock(new StatementBlockStart(default(int)), new BlockEnd(default(int))).Kind);
        }

        [TestMethod]
        public void PositionReturnsPositionOfCodeBlockStart()
        {
            var target = new CodeBlock(new StatementBlockStart(0, new Position(4, 2)), new BlockEnd(0));
            Assert.AreEqual(new Position(4, 2), target.Position);
        }

        [TestMethod]
        public void AcceptCallsVisitCodeBlockMethodOfSyntaxNodeVisitor()
        {
            var visitor = Substitute.For<SyntaxNodeVisitor>();
            var codeBlock = new CodeBlock(new StatementBlockStart(0), new BlockEnd(0));
            codeBlock.Accept(visitor);
            visitor.Received().VisitCodeBlock(codeBlock);
        }

        [TestMethod]
        public void ChildNodesReturnsBlockStartCodeAndBlockEnd()
        {
            var start = new StatementBlockStart(default(int));
            var code = new Code(default(Span));
            var end = new BlockEnd(default(int));
            var codeBlock = new CodeBlock(start, code, end);
            Assert.IsTrue(codeBlock.ChildNodes().SequenceEqual(new SyntaxNode[] { start, code, end }));
        }

        [TestMethod]
        public void ChildNodesReturnsBlockStartAndBlockEnd()
        {
            var start = new StatementBlockStart(default(int));
            var end = new BlockEnd(default(int));
            var codeBlock = new CodeBlock(start, end);
            Assert.IsTrue(codeBlock.ChildNodes().SequenceEqual(new SyntaxNode[] { start, end }));
        }

        [TestMethod]
        public void SpanStartsAtBlockStart()
        {
            StatementBlockStart start;
            var codeBlock = new CodeBlock(start = new StatementBlockStart(42), new BlockEnd(100));
            Assert.AreEqual(start.Span.Start, codeBlock.Span.Start);
        }

        [TestMethod]
        public void SpanEndsAtBlockEnd()
        {
            BlockEnd end;
            var codeBlock = new CodeBlock(new StatementBlockStart(0), end = new BlockEnd(40));
            Assert.AreEqual(end.Span.End, codeBlock.Span.End);
        }
    }
}