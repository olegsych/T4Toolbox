// <copyright file="TerminalNodeTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.TemplateAnalysis
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.VisualStudio.Text;

    [TestClass]
    public class TerminalNodeTest
    {
        [TestMethod]
        public void ConstructorSetsPositionProperty()
        {
            var target = new TestableTerminalNode(new Position(4, 2));
            Assert.AreEqual(new Position(4, 2), target.Position);
        }

        [TestMethod]
        public void ChildNodesReturnsEmptyEnumerable()
        {
            var node = new TestableTerminalNode();
            IEnumerable<SyntaxNode> childNodes = node.ChildNodes();
            Assert.IsNotNull(childNodes);
            Assert.IsFalse(childNodes.Any());
        }

        [TestMethod]
        public void ValidateReturnsEmptyEnumerable()
        {
            var node = new TestableTerminalNode();
            IEnumerable<TemplateError> errors = node.Validate();
            Assert.IsNotNull(errors);
            Assert.IsFalse(errors.Any());
        }

        private class TestableTerminalNode : TerminalNode
        {
            public TestableTerminalNode(Position position = default(Position)) : base(position)
            {                
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