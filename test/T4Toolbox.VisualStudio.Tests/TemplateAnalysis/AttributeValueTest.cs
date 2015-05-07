// <copyright file="AttributeValueTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.TemplateAnalysis
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using NSubstitute;

    [TestClass]
    public class AttributeValueTest
    {
        [TestMethod]
        public void AttributeValueIsSubclassOfCaptureNode()
        {
            Assert.IsTrue(typeof(AttributeValue).IsSubclassOf(typeof(CaptureNode)));
        }

        [TestMethod]
        public void AttributeValueIsSealed()
        {
            Assert.IsTrue(typeof(AttributeValue).IsSealed);
        }

        [TestMethod]
        public void KindReturnsAttributeValueSyntaxKind()
        {
            var target = new AttributeValue(0, string.Empty);
            Assert.AreEqual(SyntaxKind.AttributeValue, target.Kind);
        }

        [TestMethod]
        public void AcceptCallsVisitAttributeValueMethodOfSyntaxNodeVisitor()
        {
            var visitor = Substitute.For<SyntaxNodeVisitor>();
            var node = new AttributeValue(0, string.Empty);
            node.Accept(visitor);
            visitor.Received().VisitAttributeValue(node);
        }
    }
}