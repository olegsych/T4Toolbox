// <copyright file="DirectiveBlockStartTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.TemplateAnalysis
{
    using System;
    using Microsoft.VisualStudio.Text;
    using NSubstitute;
    using Xunit;

    public static class DirectiveBlockStartTest
    {
        [Fact]
        public static void DirectiveBlockStartIsSubclassOfSyntaxToken()
        {
            Assert.True(typeof(DirectiveBlockStart).IsSubclassOf(typeof(SyntaxToken)));
        }

        [Fact]
        public static void DirectiveBlockStartIsSealed()
        {
            Assert.True(typeof(DirectiveBlockStart).IsSealed);
        }

        [Fact]
        public static void AcceptCallsVisitDirectiveBlockStartMethodOfSyntaxNodeVisitor()
        {
            var visitor = Substitute.For<SyntaxNodeVisitor>();
            var node = new DirectiveBlockStart(default(int));
            node.Accept(visitor);
            visitor.Received().VisitDirectiveBlockStart(node);
        }

        [Fact]
        public static void GetDescriptionReturnsDescriptionOfDirectiveBlockStart()
        {
            var target = new DirectiveBlockStart(0);
            string description;
            Span applicableTo;
            Assert.True(target.TryGetDescription(0, out description, out applicableTo));
            Assert.Contains("directive", description, StringComparison.OrdinalIgnoreCase);
            Assert.Equal(target.Span, applicableTo);
        }

        [Fact]
        public static void KindReturnsDirectiveBlockStartSyntaxKind()
        {
            Assert.Equal(SyntaxKind.DirectiveBlockStart, new DirectiveBlockStart(0).Kind);
        }

        [Fact]
        public static void SpanLengthReturnsLengthOfToken()
        {
            Assert.Equal("<#@".Length, new DirectiveBlockStart(0).Span.Length);
        }
    }
}