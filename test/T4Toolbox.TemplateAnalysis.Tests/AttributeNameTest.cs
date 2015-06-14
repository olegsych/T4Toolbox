// <copyright file="AttributeNameTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.TemplateAnalysis
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using NSubstitute;

    [TestClass]
    public class AttributeNameTest
    {
        [TestMethod]
        public void AttributeNameIsSubclassOfCaptureNode()
        {
            Assert.IsTrue(typeof(AttributeName).IsSubclassOf(typeof(CaptureNode)));
        }

        [TestMethod]
        public void AttributeNameIsSealed()
        {
            Assert.IsTrue(typeof(AttributeName).IsSealed);
        }

        [TestMethod]
        public void KindReturnsAttributeNameSyntaxKind()
        {
            var target = new AttributeName(0, string.Empty);
            Assert.AreEqual(SyntaxKind.AttributeName, target.Kind);
        }

        [TestMethod]
        public void AcceptCallsVisitAttributeNameMethodOfSyntaxNodeVisitor()
        {
            var visitor = Substitute.For<SyntaxNodeVisitor>();
            var node = new AttributeName(0, string.Empty);
            node.Accept(visitor);
            visitor.Received().VisitAttributeName(node);
        }
    }
}