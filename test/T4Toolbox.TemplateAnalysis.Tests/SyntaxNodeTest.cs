// <copyright file="SyntaxNodeTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.TemplateAnalysis
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using Microsoft.VisualStudio.Text;
    using Xunit;

    public static class SyntaxNodeTest
    {
        #region Equals

        [Fact]
        public static void EqualsReturnsTrueWhenKindAndSpanAreTheSame()
        {
            var left = new TestableSyntaxNode(SyntaxKind.BlockEnd, new Span(4, 2));
            var right = new TestableSyntaxNode(SyntaxKind.BlockEnd, new Span(4, 2));
            Assert.True(left.Equals(right));
        }

        [Fact]
        public static void EqualsReturnsFalseWhenKindIsDifferent()
        {
            var left = new TestableSyntaxNode(SyntaxKind.BlockEnd, new Span(4, 2));
            var right = new TestableSyntaxNode(SyntaxKind.StatementBlockStart, new Span(4, 2));
            Assert.False(left.Equals(right));            
        }

        [Fact]
        public static void EqualsReturnsTrueWhenPositionIsSame()
        {
            var left = new TestableSyntaxNode(new Position(4, 2));
            var right = new TestableSyntaxNode(new Position(4, 2));
            Assert.True(left.Equals(right));
        }

        [Fact]
        public static void EqualsReturnsFalseWhenPositionIsDifferent()
        {
            var left = new TestableSyntaxNode(new Position(4, 2));
            var right = new TestableSyntaxNode(new Position(2, 4));
            Assert.False(left.Equals(right));            
        }

        [Fact]
        public static void EqualsReturnsFalseWhenSpanIsDifferent()
        {
            var left = new TestableSyntaxNode(SyntaxKind.BlockEnd, new Span(4, 2));
            var right = new TestableSyntaxNode(SyntaxKind.BlockEnd, new Span(2, 4));
            Assert.False(left.Equals(right));            
        }

        [Fact]
        public static void EqualsReturnsTrueWhenChildNodesAreSame()
        {
            var left = new TestableSyntaxNode(
                SyntaxKind.CodeBlock,
                new TestableSyntaxNode(SyntaxKind.StatementBlockStart, new Span(0, 2)),
                new TestableSyntaxNode(SyntaxKind.BlockEnd, new Span(2, 2)));
            var right = new TestableSyntaxNode(
                SyntaxKind.CodeBlock,
                new TestableSyntaxNode(SyntaxKind.StatementBlockStart, new Span(0, 2)),
                new TestableSyntaxNode(SyntaxKind.BlockEnd, new Span(2, 2)));
            Assert.True(left.Equals(right));
        }

        [Fact]
        public static void EqualsReturnsFalseWhenChildNodesAreDifferent()
        {
            var left = new TestableSyntaxNode(
                SyntaxKind.CodeBlock,
                new TestableSyntaxNode(SyntaxKind.ClassBlockStart, new Span(0, 3)),
                new TestableSyntaxNode(SyntaxKind.BlockEnd, new Span(3, 2)));
            var right = new TestableSyntaxNode(
                SyntaxKind.CodeBlock,
                new TestableSyntaxNode(SyntaxKind.ExpressionBlockStart, new Span(0, 3)),
                new TestableSyntaxNode(SyntaxKind.BlockEnd, new Span(3, 2)));
            Assert.False(left.Equals(right));
        }

        #endregion

        #region GetHashCode

        [Fact]
        public static void GetHashCodeReturnsSameValuesForSamePositions()
        {
            var left = new TestableSyntaxNode(new Position(4, 2));
            var right = new TestableSyntaxNode(new Position(4, 2));
            Assert.Equal(left.GetHashCode(), right.GetHashCode());
        }

        [Fact]
        public static void GetHashCodeReturnsDifferentValuesForDifferentPositions()
        {
            var left = new TestableSyntaxNode(new Position(4, 2));
            var right = new TestableSyntaxNode(new Position(2, 4));
            Assert.NotEqual(left.GetHashCode(), right.GetHashCode());
        }

        #endregion

        [Fact]
        public static void GetTextReturnsSubstringOfTemplateBasedOnSpan()
        {
            var node = new TestableSyntaxNode(SyntaxKind.DirectiveName, new Span(4, 9));
            Assert.Equal("directive", node.GetText("<#@ directive #>"));
        }

        #region TryGetDescription

        [Fact]
        public static void TryGetDescriptionReturnsEmptyDescriptionAndSpanGivenPositionOutsideOfItsSpan()
        {
            var target = new TestableSyntaxNode(SyntaxKind.Template, new Span(1, 1));
            string description;
            Span applicableTo;
            Assert.False(target.TryGetDescription(2, out description, out applicableTo));
            Assert.Equal(string.Empty, description);
            Assert.Equal(default(Span), applicableTo);
        }

        [Fact]
        public static void TryGetDescriptionReturnsEmptyDescriptionAndSpanWhenNodeHasNoDescriptionAttributeAndNoChildren()
        {
            var target = new TestableSyntaxNode(SyntaxKind.Template, new Span(0, 1));
            string description;
            Span applicableTo;
            Assert.False(target.TryGetDescription(0, out description, out applicableTo));
            Assert.Equal(string.Empty, description);
            Assert.Equal(default(Span), applicableTo);
        }

        [Fact]
        public static void TryGetDescriptionReturnsDescriptionAndSpanOfNodeWhenNodeHasNoChildren()
        {
            var target = new TestableSyntaxNodeWithDescription(new Span(0, 1));
            var attribute = typeof(TestableSyntaxNodeWithDescription).GetCustomAttributes(false).OfType<DescriptionAttribute>().Single();
            string description;
            Span applicableTo;
            Assert.True(target.TryGetDescription(0, out description, out applicableTo));
            Assert.Equal(attribute.Description, description);
            Assert.Equal(target.Span, applicableTo);
        }

        [Fact]
        public static void TryGetDescriptionReturnsDescriptionAndSpanOfChildNodeWhoseSpanContainsGivenPosition()
        {
            SyntaxNode child = new TestableSyntaxNodeWithDescription(new Span(0, 2));
            var childAttribute = typeof(TestableSyntaxNodeWithDescription).GetCustomAttributes(false).OfType<DescriptionAttribute>().Single();
            var parent = new TestableSyntaxNode(default(SyntaxKind), new Span(0, 4), child);
            string description;
            Span applicableTo;
            Assert.True(parent.TryGetDescription(0, out description, out applicableTo));
            Assert.Equal(childAttribute.Description, description);
            Assert.Equal(child.Span, applicableTo);
        }

        [Fact]
        public static void TryGetDescriptionReturnsDescriptionAndSpanOfParentIfChildsDescriptionIsEmpty()
        {
            var parent = new TestableSyntaxNodeWithDescription(new Span(0, 4), new TestableSyntaxNode(SyntaxKind.BlockEnd, new Span(0, 2)));
            var parentAttribute = typeof(TestableSyntaxNodeWithDescription).GetCustomAttributes(false).OfType<DescriptionAttribute>().Single();
            string description;
            Span applicableTo;
            Assert.True(parent.TryGetDescription(0, out description, out applicableTo));
            Assert.Equal(parentAttribute.Description, description);
            Assert.Equal(parent.Span, applicableTo);
        }

        #endregion

        private class TestableSyntaxNode : SyntaxNode
        {
            private readonly SyntaxKind kind;
            private readonly Position position;
            private readonly Span span;
            private readonly SyntaxNode[] childNodes;

            public TestableSyntaxNode(Position position)
            {
                this.position = position;
            }

            public TestableSyntaxNode(SyntaxKind kind, params SyntaxNode[] childNodes)
                : this(kind, default(Span), childNodes)
            {               
            }

            public TestableSyntaxNode(SyntaxKind kind, Span span, params SyntaxNode[] childNodes)
            {
                this.kind = kind;
                this.span = span;
                this.childNodes = childNodes;
            }

            public override SyntaxKind Kind
            {
                get { return this.kind; }
            }

            public override Position Position
            {
                get { return this.position; }
            }

            public override Span Span
            {
                get { return this.span; }
            }

            public override IEnumerable<SyntaxNode> ChildNodes()
            {
                return this.childNodes ?? Enumerable.Empty<SyntaxNode>();
            }

            public override IEnumerable<TemplateError> Validate()
            {
                throw new NotImplementedException();
            }

            protected internal override void Accept(SyntaxNodeVisitor visitor)
            {
                throw new NotImplementedException();
            }
        }

        [Description("Description of TestableSyntaxNodeWithDescription")]
        private class TestableSyntaxNodeWithDescription : TestableSyntaxNode
        {
            public TestableSyntaxNodeWithDescription(Span span, params SyntaxNode[] childNodes)
                : base(default(SyntaxKind), span, childNodes)
            {                
            }
        }
    }
}