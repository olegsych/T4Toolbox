// <copyright file="CodeBlockTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.TemplateAnalysis
{
    using System.Linq;
    using Microsoft.VisualStudio.Text;
    using NSubstitute;
    using Xunit;

    public static class CodeBlockTest
    {
        [Fact]
        public static void CodeBlockIsSubclassOfNonterminalNode()
        {
            Assert.True(typeof(CodeBlock).IsSubclassOf(typeof(NonterminalNode)));
        }

        [Fact]
        public static void CodeBlockIsSealed()
        {
            Assert.True(typeof(CodeBlock).IsSealed);
        }

        [Fact]
        public static void KindReturnsCodeBlockSyntaxKind()
        {
            Assert.Equal(SyntaxKind.CodeBlock, new CodeBlock(new StatementBlockStart(default(int)), new BlockEnd(default(int))).Kind);
        }

        [Fact]
        public static void PositionReturnsPositionOfCodeBlockStart()
        {
            var target = new CodeBlock(new StatementBlockStart(0, new Position(4, 2)), new BlockEnd(0));
            Assert.Equal(new Position(4, 2), target.Position);
        }

        [Fact]
        public static void AcceptCallsVisitCodeBlockMethodOfSyntaxNodeVisitor()
        {
            var visitor = Substitute.For<SyntaxNodeVisitor>();
            var codeBlock = new CodeBlock(new StatementBlockStart(0), new BlockEnd(0));
            codeBlock.Accept(visitor);
            visitor.Received().VisitCodeBlock(codeBlock);
        }

        [Fact]
        public static void ChildNodesReturnsBlockStartCodeAndBlockEnd()
        {
            var start = new StatementBlockStart(default(int));
            var code = new Code(default(Span));
            var end = new BlockEnd(default(int));
            var codeBlock = new CodeBlock(start, code, end);
            Assert.True(codeBlock.ChildNodes().SequenceEqual(new SyntaxNode[] { start, code, end }));
        }

        [Fact]
        public static void ChildNodesReturnsBlockStartAndBlockEnd()
        {
            var start = new StatementBlockStart(default(int));
            var end = new BlockEnd(default(int));
            var codeBlock = new CodeBlock(start, end);
            Assert.True(codeBlock.ChildNodes().SequenceEqual(new SyntaxNode[] { start, end }));
        }

        [Fact]
        public static void SpanStartsAtBlockStart()
        {
            StatementBlockStart start;
            var codeBlock = new CodeBlock(start = new StatementBlockStart(42), new BlockEnd(100));
            Assert.Equal(start.Span.Start, codeBlock.Span.Start);
        }

        [Fact]
        public static void SpanEndsAtBlockEnd()
        {
            BlockEnd end;
            var codeBlock = new CodeBlock(new StatementBlockStart(0), end = new BlockEnd(40));
            Assert.Equal(end.Span.End, codeBlock.Span.End);
        }
    }
}