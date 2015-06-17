// <copyright file="AttributeValueTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.TemplateAnalysis
{
    using NSubstitute;
    using Xunit;

    public static class AttributeValueTest
    {
        [Fact]
        public static void AttributeValueIsSubclassOfCaptureNode()
        {
            Assert.True(typeof(AttributeValue).IsSubclassOf(typeof(CaptureNode)));
        }

        [Fact]
        public static void AttributeValueIsSealed()
        {
            Assert.True(typeof(AttributeValue).IsSealed);
        }

        [Fact]
        public static void KindReturnsAttributeValueSyntaxKind()
        {
            var target = new AttributeValue(0, string.Empty);
            Assert.Equal(SyntaxKind.AttributeValue, target.Kind);
        }

        [Fact]
        public static void AcceptCallsVisitAttributeValueMethodOfSyntaxNodeVisitor()
        {
            var visitor = Substitute.For<SyntaxNodeVisitor>();
            var node = new AttributeValue(0, string.Empty);
            node.Accept(visitor);
            visitor.Received().VisitAttributeValue(node);
        }
    }
}