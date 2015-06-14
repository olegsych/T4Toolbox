// <copyright file="SyntaxTokenTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.TemplateAnalysis
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.VisualStudio.Text;

    [TestClass]
    public class SyntaxTokenTest
    {
        [TestMethod]
        public void ConstructorSetsPositionProperty()
        {
            var target = new TestableSyntaxToken(0, new Position(4, 2));
            Assert.AreEqual(new Position(4, 2), target.Position);
        }

        [TestMethod]
        public void SyntaxTokenIsSubclassOfTerminalNode()
        {
            Assert.IsTrue(typeof(SyntaxToken).IsSubclassOf(typeof(TerminalNode)));
        }

        [TestMethod]
        public void StartReturnsValueSpecifiedInConstructor()
        {
            var target = new TestableSyntaxToken(42);
            Assert.AreEqual(42, target.Start);
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