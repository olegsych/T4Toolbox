// <copyright file="AttributeNameTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.TemplateAnalysis
{
    using NSubstitute;
    using Xunit;

    public static class AttributeNameTest
    {
        [Fact]
        public static void AttributeNameIsSubclassOfCaptureNode()
        {
            Assert.True(typeof(AttributeName).IsSubclassOf(typeof(CaptureNode)));
        }

        [Fact]
        public static void AttributeNameIsSealed()
        {
            Assert.True(typeof(AttributeName).IsSealed);
        }

        [Fact]
        public static void KindReturnsAttributeNameSyntaxKind()
        {
            var target = new AttributeName(0, string.Empty);
            Assert.Equal(SyntaxKind.AttributeName, target.Kind);
        }

        [Fact]
        public static void AcceptCallsVisitAttributeNameMethodOfSyntaxNodeVisitor()
        {
            var visitor = Substitute.For<SyntaxNodeVisitor>();
            var node = new AttributeName(0, string.Empty);
            node.Accept(visitor);
            visitor.Received().VisitAttributeName(node);
        }
    }
}