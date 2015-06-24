// <copyright file="DirectiveNameTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.TemplateAnalysis
{
    using NSubstitute;
    using Xunit;

    public static class DirectiveNameTest
    {
        [Fact]
        public static void DirectiveNameIsSubclassOfTerminalNode()
        {
            Assert.True(typeof(DirectiveName).IsSubclassOf(typeof(TerminalNode)));
        }

        [Fact]
        public static void DirectiveNameIsSealed()
        {
            Assert.True(typeof(DirectiveName).IsSealed);
        }

        [Fact]
        public static void KindReturnsDirectiveNameSyntaxKind()
        {
            var target = new DirectiveName(0, string.Empty);
            Assert.Equal(SyntaxKind.DirectiveName, target.Kind);
        }

        [Fact]
        public static void AcceptCallsVisitDirectiveNameMethodOfSyntaxNodeVisitor()
        {
            var visitor = Substitute.For<SyntaxNodeVisitor>();
            var node = new DirectiveName(0, string.Empty);
            node.Accept(visitor);
            visitor.Received().VisitDirectiveName(node);
        }
    }
}