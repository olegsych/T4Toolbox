// <copyright file="DirectiveNameTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.TemplateAnalysis
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using NSubstitute;

    [TestClass]
    public class DirectiveNameTest
    {
        [TestMethod]
        public void DirectiveNameIsSubclassOfTerminalNode()
        {
            Assert.IsTrue(typeof(DirectiveName).IsSubclassOf(typeof(TerminalNode)));
        }

        [TestMethod]
        public void DirectiveNameIsSealed()
        {
            Assert.IsTrue(typeof(DirectiveName).IsSealed);
        }

        [TestMethod]
        public void KindReturnsDirectiveNameSyntaxKind()
        {
            var target = new DirectiveName(0, string.Empty);
            Assert.AreEqual(SyntaxKind.DirectiveName, target.Kind);
        }

        [TestMethod]
        public void AcceptCallsVisitDirectiveNameMethodOfSyntaxNodeVisitor()
        {
            var visitor = Substitute.For<SyntaxNodeVisitor>();
            var node = new DirectiveName(0, string.Empty);
            node.Accept(visitor);
            visitor.Received().VisitDirectiveName(node);
        }
    }
}