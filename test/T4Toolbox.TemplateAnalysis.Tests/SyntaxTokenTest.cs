// <copyright file="SyntaxTokenTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.TemplateAnalysis
{
    using System;
    using Microsoft.VisualStudio.Text;
    using Xunit;

    public static class SyntaxTokenTest
    {
        [Fact]
        public static void ConstructorSetsPositionProperty()
        {
            var target = new TestableSyntaxToken(0, new Position(4, 2));
            Assert.Equal(new Position(4, 2), target.Position);
        }

        [Fact]
        public static void SyntaxTokenIsSubclassOfTerminalNode()
        {
            Assert.True(typeof(SyntaxToken).IsSubclassOf(typeof(TerminalNode)));
        }

        [Fact]
        public static void StartReturnsValueSpecifiedInConstructor()
        {
            var target = new TestableSyntaxToken(42);
            Assert.Equal(42, target.Start);
        }

        private class TestableSyntaxToken : SyntaxToken
        {
            public TestableSyntaxToken(int start, Position position = default(Position)) : base(start, position)
            {
            }

            public new int Start
            {
                get { return base.Start; }
            }

            public override SyntaxKind Kind
            {
                get { throw new NotImplementedException(); }
            }

            public override Span Span
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