// <copyright file="CodeTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.TemplateAnalysis
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.VisualStudio.Text;
    using NSubstitute;

    [TestClass]
    public class CodeTest
    {
        [TestMethod]
        public void CodeIsSubclassOfTerminalNode()
        {
            Assert.IsTrue(typeof(Code).IsSubclassOf(typeof(TerminalNode)));
        }

        [TestMethod]
        public void CodeIsSealed()
        {
            Assert.IsTrue(typeof(Code).IsSealed);
        }

        [TestMethod]
        public void KindReturnsCodeSyntaxKind()
        {
            Assert.AreEqual(SyntaxKind.Code, new Code(default(Span)).Kind);
        }

        [TestMethod]
        public void SpanReturnsValueSpecifiedInConstructor()
        {
            Assert.AreEqual(new Span(4, 2), new Code(new Span(4, 2)).Span);
        }

        [TestMethod]
        public void AcceptCallsVisitCodeMethodOfSyntaxNodeVisitor()
        {
            var visitor = Substitute.For<SyntaxNodeVisitor>();
            var node = new Code(default(Span));
            node.Accept(visitor);
            visitor.Received().VisitCode(node);
        }
    }
}