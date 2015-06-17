// <copyright file="ExpressionBlockStartTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.TemplateAnalysis
{
    using System;
    using Microsoft.VisualStudio.Text;
    using NSubstitute;
    using Xunit;

    public static class ExpressionBlockStartTest
    {
        [Fact]
        public static void ExpressionBlockStartIsSubclassOfCodeBlockStart()
        {
            Assert.True(typeof(ExpressionBlockStart).IsSubclassOf(typeof(CodeBlockStart)));
        }

        [Fact]
        public static void ExpressionBlockStartIsSealed()
        {
            Assert.True(typeof(ExpressionBlockStart).IsSealed);
        }

        [Fact]
        public static void AcceptCallsVisitExpressionBlockStartMethodOfSyntaxNodeVisitor()
        {
            var visitor = Substitute.For<SyntaxNodeVisitor>();
            var node = new ExpressionBlockStart(default(int));
            node.Accept(visitor);
            visitor.Received().VisitExpressionBlockStart(node);
        }

        [Fact]
        public static void GetDescriptionReturnsDescriptionOfExpressionCodeBlocks()
        {
            var target = new ExpressionBlockStart(0);
            string description;
            Span applicableTo;
            Assert.True(target.TryGetDescription(0, out description, out applicableTo));
            Assert.Contains("expression", description, StringComparison.OrdinalIgnoreCase);
            Assert.Equal(target.Span, applicableTo);
        }

        [Fact]
        public static void KindReturnsExpressionBlockStartSyntaxKind()
        {
            Assert.Equal(SyntaxKind.ExpressionBlockStart, new ExpressionBlockStart(0).Kind);
        }

        [Fact]
        public static void SpanLengthReturnsLengthOfToken()
        {
            Assert.Equal("<#=".Length, new ExpressionBlockStart(0).Span.Length);
        }
    }
}