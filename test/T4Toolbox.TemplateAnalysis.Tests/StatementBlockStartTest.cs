// <copyright file="StatementBlockStartTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.TemplateAnalysis
{
    using System;
    using Microsoft.VisualStudio.Text;
    using NSubstitute;
    using Xunit;

    public static class StatementBlockStartTest
    {
        [Fact]
        public static void StatementBlockStartIsSubclassOfCodeBlockStart()
        {
            Assert.True(typeof(StatementBlockStart).IsSubclassOf(typeof(CodeBlockStart)));
        }

        [Fact]
        public static void StatementBlockStartIsSealed()
        {
            Assert.True(typeof(StatementBlockStart).IsSealed);
        }

        [Fact]
        public static void AcceptCallsAcceptStatementBlockStartMethodOfSyntaxNodeVisitor()
        {
            var visitor = Substitute.For<SyntaxNodeVisitor>();
            var node = new StatementBlockStart(default(int));
            node.Accept(visitor);
            visitor.Received().VisitStatementBlockStart(node);
        }

        [Fact]
        public static void GetDescriptionReturnsDescriptionOfStatementCodeBlocks()
        {
            var target = new StatementBlockStart(0);
            string description;
            Span applicableTo;
            Assert.True(target.TryGetDescription(0, out description, out applicableTo));
            Assert.Contains("statement", description, StringComparison.OrdinalIgnoreCase);
            Assert.Equal(target.Span, applicableTo);
        }

        [Fact]
        public static void KindReturnsStatementBlockStartSyntaxKind()
        {
            Assert.Equal(SyntaxKind.StatementBlockStart, new StatementBlockStart(0).Kind);
        }

        [Fact]
        public static void SpanLengthReturnsLengthOfTheToken()
        {
            Assert.Equal("<#".Length, new StatementBlockStart(0).Span.Length);
        }
    }
}