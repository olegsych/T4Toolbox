// <copyright file="CaptureNodeTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.TemplateAnalysis
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class CaptureNodeTest
    {
        [TestMethod]
        public void CaptureNodeIsSubclassOfTerminalNode()
        {
            Assert.IsTrue(typeof(CaptureNode).IsSubclassOf(typeof(TerminalNode)));
        }

        [TestMethod]
        public void ConstructorSetsPositionProperty()
        {
            var target = new TestableCaptureNode(0, string.Empty, new Position(4, 2));
            Assert.AreEqual(new Position(4, 2), target.Position);
        }

        [TestMethod]
        public void SpanStartsAtPositionSpecifiedInConstructor()
        {
            var target = new TestableCaptureNode(42, "template");
            Assert.AreEqual(42, target.Span.Start);
        }

        [TestMethod]
        public void SpanLengthEqualsToLengthOfTextSpecifiedInConstructor()
        {
            var target = new TestableCaptureNode(0, "template");
            Assert.AreEqual("template".Length, target.Span.Length);
        }

        [TestMethod]
        public void TextReturnsValueSpecifiedInConstructor()
        {
            var target = new TestableCaptureNode(0, "template");
            Assert.AreEqual("template", target.Text);
        }

        private class TestableCaptureNode : CaptureNode
        {
            public TestableCaptureNode(int start, string text, Position position = default(Position))
                : base(start, text, position)
            {                
            }

            public override SyntaxKind Kind
            {
                get { throw new NotImplementedException(); }
            }

            protected internal override void Accept(SyntaxNodeVisitor visitor)
            {
                throw new NotImplementedException();
            }
        }
    }
}