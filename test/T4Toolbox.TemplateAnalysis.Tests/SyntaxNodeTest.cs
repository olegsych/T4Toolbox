// <copyright file="SyntaxNodeTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.TemplateAnalysis
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.VisualStudio.Text;
    using DescriptionAttribute = System.ComponentModel.DescriptionAttribute;

    [TestClass]
    public class SyntaxNodeTest
    {
        #region Equals

        [TestMethod]
        public void EqualsReturnsTrueWhenKindAndSpanAreTheSame()
        {
            var left = new TestableSyntaxNode(SyntaxKind.BlockEnd, new Span(4, 2));
            var right = new TestableSyntaxNode(SyntaxKind.BlockEnd, new Span(4, 2));
            Assert.IsTrue(left.Equals(right));
        }

        [TestMethod]
        public void EqualsReturnsFalseWhenKindIsDifferent()
        {
            var left = new TestableSyntaxNode(SyntaxKind.BlockEnd, new Span(4, 2));
            var right = new TestableSyntaxNode(SyntaxKind.StatementBlockStart, new Span(4, 2));
            Assert.IsFalse(left.Equals(right));            
        }

        [TestMethod]
        public void EqualsReturnsTrueWhenPositionIsSame()
        {
            var left = new TestableSyntaxNode(new Position(4, 2));
            var right = new TestableSyntaxNode(new Position(4, 2));
            Assert.IsTrue(left.Equals(right));
        }

        [TestMethod]
        public void EqualsReturnsFalseWhenPositionIsDifferent()
        {
            var left = new TestableSyntaxNode(new Position(4, 2));
            var right = new TestableSyntaxNode(new Position(2, 4));
            Assert.IsFalse(left.Equals(right));            
        }

        [TestMethod]
        public void EqualsReturnsFalseWhenSpanIsDifferent()
        {
            var left = new TestableSyntaxNode(SyntaxKind.BlockEnd, new Span(4, 2));
            var right = new TestableSyntaxNode(SyntaxKind.BlockEnd, new Span(2, 4));
            Assert.IsFalse(left.Equals(right));            
        }

        [TestMethod]
        public void EqualsReturnsTrueWhenChildNodesAreSame()
        {
            var left = new TestableSyntaxNode(
                SyntaxKind.CodeBlock,
                new TestableSyntaxNode(SyntaxKind.StatementBlockStart, new Span(0, 2)),
                new TestableSyntaxNode(SyntaxKind.BlockEnd, new Span(2, 2)));
            var right = new TestableSyntaxNode(
                SyntaxKind.CodeBlock,
                new TestableSyntaxNode(SyntaxKind.StatementBlockStart, new Span(0, 2)),
                new TestableSyntaxNode(SyntaxKind.BlockEnd, new Span(2, 2)));
            Assert.IsTrue(left.Equals(right));
        }

        [TestMethod]
        public void EqualsReturnsFalseWhenChildNodesAreDifferent()
        {
            var left = new TestableSyntaxNode(
                SyntaxKind.CodeBlock,
                new TestableSyntaxNode(SyntaxKind.ClassBlockStart, new Span(0, 3)),
                new TestableSyntaxNode(SyntaxKind.BlockEnd, new Span(3, 2)));
            var right = new TestableSyntaxNode(
                SyntaxKind.CodeBlock,
                new TestableSyntaxNode(SyntaxKind.ExpressionBlockStart, new Span(0, 3)),
                new TestableSyntaxNode(SyntaxKind.BlockEnd, new Span(3, 2)));
            Assert.IsFalse(left.Equals(right));
        }

        #endregion

        #region GetHashCode

        [TestMethod]
        public void GetHashCodeReturnsSameValuesForSamePositions()
        {
            var left = new TestableSyntaxNode(new Position(4, 2));
            var right = new TestableSyntaxNode(new Position(4, 2));
            Assert.AreEqual(left.GetHashCode(), right.GetHashCode());
        }

        [TestMethod]
        public void GetHashCodeReturnsDifferentValuesForDifferentPositions()
        {
            var left = new TestableSyntaxNode(new Position(4, 2));
            var right = new TestableSyntaxNode(new Position(2, 4));
            Assert.AreNotEqual(left.GetHashCode(), right.GetHashCode());
        }

        #endregion

        [TestMethod]
        public void GetTextReturnsSubstringOfTemplateBasedOnSpan()
        {
            var node = new TestableSyntaxNode(SyntaxKind.DirectiveName, new Span(4, 9));
            Assert.AreEqual("directive", node.GetText("<#@ directive #>"));
        }

        #region TryGetDescription

        [TestMethod]
        public void TryGetDescriptionReturnsEmptyDescriptionAndSpanGivenPositionOutsideOfItsSpan()
        {
            var target = new TestableSyntaxNode(SyntaxKind.Template, new Span(1, 1));
            string description;
            Span applicableTo;
            Assert.IsFalse(target.TryGetDescription(2, out description, out applicableTo));
            Assert.AreEqual(string.Empty, description);
            Assert.AreEqual(default(Span), applicableTo);
        }

        [TestMethod]
        public void TryGetDescriptionReturnsEmptyDescriptionAndSpanWhenNodeHasNoDescriptionAttributeAndNoChildren()
        {
            var target = new TestableSyntaxNode(SyntaxKind.Template, new Span(0, 1));
            string description;
            Span applicableTo;
            Assert.IsFalse(target.TryGetDescription(0, out description, out applicableTo));
            Assert.AreEqual(string.Empty, description);
            Assert.AreEqual(default(Span), applicableTo);
        }

        [TestMethod]
        public void TryGetDescriptionReturnsDescriptionAndSpanOfNodeWhenNodeHasNoChildren()
        {
            var target = new TestableSyntaxNodeWithDescription(new Span(0, 1));
            var attribute = typeof(TestableSyntaxNodeWithDescription).GetCustomAttributes(false).OfType<DescriptionAttribute>().Single();
            string description;
            Span applicableTo;
            Assert.IsTrue(target.TryGetDescription(0, out description, out applicableTo));
            Assert.AreEqual(attribute.Description, description);
            Assert.AreEqual(target.Span, applicableTo);
        }

        [TestMethod]
        public void TryGetDescriptionReturnsDescriptionAndSpanOfChildNodeWhoseSpanContainsGivenPosition()
        {
            SyntaxNode child = new TestableSyntaxNodeWithDescription(new Span(0, 2));
            var childAttribute = typeof(TestableSyntaxNodeWithDescription).GetCustomAttributes(false).OfType<DescriptionAttribute>().Single();
            var parent = new TestableSyntaxNode(default(SyntaxKind), new Span(0, 4), child);
            string description;
            Span applicableTo;
            Assert.IsTrue(parent.TryGetDescription(0, out description, out applicableTo));
            Assert.AreEqual(childAttribute.Description, description);
            Assert.AreEqual(child.Span, applicableTo);
        }

        [TestMethod]
        public void TryGetDescriptionReturnsDescriptionAndSpanOfParentIfChildsDescriptionIsEmpty()
        {
            var parent = new TestableSyntaxNodeWithDescription(new Span(0, 4), new TestableSyntaxNode(SyntaxKind.BlockEnd, new Span(0, 2)));
            var parentAttribute = typeof(TestableSyntaxNodeWithDescription).GetCustomAttributes(false).OfType<DescriptionAttribute>().Single();
            string description;
            Span applicableTo;
            Assert.IsTrue(parent.TryGetDescription(0, out description, out applicableTo));
            Assert.AreEqual(parentAttribute.Description, description);
            Assert.AreEqual(parent.Span, applicableTo);
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