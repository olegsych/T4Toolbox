// <copyright file="TerminalNodeTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.TemplateAnalysis
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.VisualStudio.Text;
    using Xunit;

    public static class TerminalNodeTest
    {
        [Fact]
        public static void ConstructorSetsPositionProperty()
        {
            var target = new TestableTerminalNode(new Position(4, 2));
            Assert.Equal(new Position(4, 2), target.Position);
        }

        [Fact]
        public static void ChildNodesReturnsEmptyEnumerable()
        {
            var node = new TestableTerminalNode();
            IEnumerable<SyntaxNode> childNodes = node.ChildNodes();
            Assert.NotNull(childNodes);
            Assert.False(childNodes.Any());
        }

        [Fact]
        public static void ValidateReturnsEmptyEnumerable()
        {
            var node = new TestableTerminalNode();
            IEnumerable<TemplateError> errors = node.Validate();
            Assert.NotNull(errors);
            Assert.False(errors.Any());
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