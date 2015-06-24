// <copyright file="CaptureNodeTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.TemplateAnalysis
{
    using System;
    using Xunit;

    public static class CaptureNodeTest
    {
        [Fact]
        public static void CaptureNodeIsSubclassOfTerminalNode()
        {
            Assert.True(typeof(CaptureNode).IsSubclassOf(typeof(TerminalNode)));
        }

        [Fact]
        public static void ConstructorSetsPositionProperty()
        {
            var target = new TestableCaptureNode(0, string.Empty, new Position(4, 2));
            Assert.Equal(new Position(4, 2), target.Position);
        }

        [Fact]
        public static void SpanStartsAtPositionSpecifiedInConstructor()
        {
            var target = new TestableCaptureNode(42, "template");
            Assert.Equal(42, target.Span.Start);
        }

        [Fact]
        public static void SpanLengthEqualsToLengthOfTextSpecifiedInConstructor()
        {
            var target = new TestableCaptureNode(0, "template");
            Assert.Equal("template".Length, target.Span.Length);
        }

        [Fact]
        public static void TextReturnsValueSpecifiedInConstructor()
        {
            var target = new TestableCaptureNode(0, "template");
            Assert.Equal("template", target.Text);
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