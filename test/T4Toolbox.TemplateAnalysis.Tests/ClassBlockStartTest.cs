// <copyright file="ClassBlockStartTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.TemplateAnalysis
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.VisualStudio.Text;
    using NSubstitute;

    [TestClass]
    public class ClassBlockStartTest
    {
        [TestMethod]
        public void ClassBlockStartIsSubclassOfCodeBlockStart()
        {
            Assert.IsTrue(typeof(ClassBlockStart).IsSubclassOf(typeof(CodeBlockStart)));
        }

        [TestMethod]
        public void ClassBlockStartIsSealed()
        {
            Assert.IsTrue(typeof(ClassBlockStart).IsSealed);
        }

        [TestMethod]
        public void AcceptCallsVisitClassBlockStartMethodOfSyntaxNodeVisitor()
        {
            var visitor = Substitute.For<SyntaxNodeVisitor>();
            var node = new ClassBlockStart(default(int));
            node.Accept(visitor);
            visitor.Received().VisitClassBlockStart(node);
        }

        [TestMethod]
        public void GetDescriptionReturnsDescriptionOfClassCodeBlocks()
        {
            var target = new ClassBlockStart(0);
            string description;
            Span applicableTo;
            Assert.IsTrue(target.TryGetDescription(0, out description, out applicableTo));
            StringAssert.Contains(description, "class feature");
            Assert.AreEqual(target.Span, applicableTo);
        }

        [TestMethod]
        public void KindReturnsClassBlockStartSyntaxKind()
        {
            Assert.AreEqual(SyntaxKind.ClassBlockStart, new ClassBlockStart(0).Kind);
        }

        [TestMethod]
        public void SpanLengthReturnsLengthOfToken()
        {
            Assert.AreEqual("<#+".Length, new ClassBlockStart(0).Span.Length);
        }
    }
}