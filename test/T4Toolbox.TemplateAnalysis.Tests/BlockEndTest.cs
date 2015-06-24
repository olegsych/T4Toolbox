// <copyright file="BlockEndTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.TemplateAnalysis
{
    using System;
    using Microsoft.VisualStudio.Text;
    using NSubstitute;
    using Xunit;

    public static class BlockEndTest
    {
        [Fact]
        public static void BlockEndIsSubclassOfSyntaxToken()
        {
            Assert.True(typeof(BlockEnd).IsSubclassOf(typeof(SyntaxToken)));
        }

        [Fact]
        public static void BlockEndIsSealed()
        {
            Assert.True(typeof(BlockEnd).IsSealed);
        }

        [Fact]
        public static void GetDescriptionReturnsDescriptionOfDirectiveBlockStart()
        {
            var target = new BlockEnd(0);
            string description;
            Span applicableTo;
            Assert.True(target.TryGetDescription(0, out description, out applicableTo));
            Assert.Contains("control block", description, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("directive", description, StringComparison.OrdinalIgnoreCase);
            Assert.Equal(target.Span, applicableTo);
        }

        [Fact]
        public static void KindReturnsBlockEndSyntaxKind()
        {
            Assert.Equal(SyntaxKind.BlockEnd, new BlockEnd(0).Kind);
        }

        [Fact]
        public static void SpanLengthReturnsLengthOfToken()
        {
            Assert.Equal("#>".Length, new BlockEnd(0).Span.Length);
        }

        [Fact]
        public static void AcceptCallsVisitBlockEndMethodOfSyntaxNodeVisitor()
        {
            var visitor = Substitute.For<SyntaxNodeVisitor>();
            var node = new BlockEnd(0);
            node.Accept(visitor);
            visitor.Received().VisitBlockEnd(node);
        }
    }
}