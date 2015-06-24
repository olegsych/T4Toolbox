// <copyright file="ClassBlockStartTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.TemplateAnalysis
{
    using System;
    using Microsoft.VisualStudio.Text;
    using NSubstitute;
    using Xunit;

    public static class ClassBlockStartTest
    {
        [Fact]
        public static void ClassBlockStartIsSubclassOfCodeBlockStart()
        {
            Assert.True(typeof(ClassBlockStart).IsSubclassOf(typeof(CodeBlockStart)));
        }

        [Fact]
        public static void ClassBlockStartIsSealed()
        {
            Assert.True(typeof(ClassBlockStart).IsSealed);
        }

        [Fact]
        public static void AcceptCallsVisitClassBlockStartMethodOfSyntaxNodeVisitor()
        {
            var visitor = Substitute.For<SyntaxNodeVisitor>();
            var node = new ClassBlockStart(default(int));
            node.Accept(visitor);
            visitor.Received().VisitClassBlockStart(node);
        }

        [Fact]
        public static void GetDescriptionReturnsDescriptionOfClassCodeBlocks()
        {
            var target = new ClassBlockStart(0);
            string description;
            Span applicableTo;
            Assert.True(target.TryGetDescription(0, out description, out applicableTo));
            Assert.Contains("class feature", description, StringComparison.OrdinalIgnoreCase);
            Assert.Equal(target.Span, applicableTo);
        }

        [Fact]
        public static void KindReturnsClassBlockStartSyntaxKind()
        {
            Assert.Equal(SyntaxKind.ClassBlockStart, new ClassBlockStart(0).Kind);
        }

        [Fact]
        public static void SpanLengthReturnsLengthOfToken()
        {
            Assert.Equal("<#+".Length, new ClassBlockStart(0).Span.Length);
        }
    }
}