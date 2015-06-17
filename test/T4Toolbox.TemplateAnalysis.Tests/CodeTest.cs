// <copyright file="CodeTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.TemplateAnalysis
{
    using Microsoft.VisualStudio.Text;
    using NSubstitute;
    using Xunit;

    public static class CodeTest
    {
        [Fact]
        public static void CodeIsSubclassOfTerminalNode()
        {
            Assert.True(typeof(Code).IsSubclassOf(typeof(TerminalNode)));
        }

        [Fact]
        public static void CodeIsSealed()
        {
            Assert.True(typeof(Code).IsSealed);
        }

        [Fact]
        public static void KindReturnsCodeSyntaxKind()
        {
            Assert.Equal(SyntaxKind.Code, new Code(default(Span)).Kind);
        }

        [Fact]
        public static void SpanReturnsValueSpecifiedInConstructor()
        {
            Assert.Equal(new Span(4, 2), new Code(new Span(4, 2)).Span);
        }

        [Fact]
        public static void AcceptCallsVisitCodeMethodOfSyntaxNodeVisitor()
        {
            var visitor = Substitute.For<SyntaxNodeVisitor>();
            var node = new Code(default(Span));
            node.Accept(visitor);
            visitor.Received().VisitCode(node);
        }
    }
}