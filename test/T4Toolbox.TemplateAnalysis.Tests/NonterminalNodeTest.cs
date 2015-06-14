// <copyright file="NonterminalNodeTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.TemplateAnalysis
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.VisualStudio.Text;

    [TestClass]
    public class NonterminalNodeTest
    {
        [TestMethod]
        public void NonterminalNodeIsSubclassOfSyntaxNode()
        {
            Assert.IsTrue(typeof(NonterminalNode).IsSubclassOf(typeof(SyntaxNode)));
        }

        [TestMethod]
        public void ValidateReturnsMultipleErrorsFromSingleChild()
        {
            var child = new TestableNonterminalNode();
            child.ValidationErrors = new[]
            {
                new TemplateError(string.Empty, default(Span), default(Position)), 
                new TemplateError(string.Empty, default(Span), default(Position))
            };
            var parent = new TestableNonterminalNode(child);
            Assert.AreEqual(2, parent.Validate().Count());
        }

        [TestMethod]
        public void ValidateReturnsErrorsFromMultipleChildren()
        {
            var child1 = new TestableNonterminalNode { ValidationErrors = new[] { new TemplateError(string.Empty, default(Span), default(Position)) } };
            var child2 = new TestableNonterminalNode { ValidationErrors = new[] { new TemplateError(string.Empty, default(Span), default(Position)) } };
            var parent = new TestableNonterminalNode(child1, child2);
            Assert.AreEqual(2, parent.Validate().Count());
        }

        private class TestableNonterminalNode : NonterminalNode
        {
            private readonly SyntaxNode[] childNodes;

            public TestableNonterminalNode(params SyntaxNode[] childNodes)
            {
                this.childNodes = childNodes;
            }

            public override SyntaxKind Kind
            {
                get { throw new NotImplementedException(); }
            }

            public override Position Position
            {
                get { throw new NotImplementedException(); }
            }

            public override Span Span
            {
                get { throw new NotImplementedException(); }
            }

            public IEnumerable<TemplateError> ValidationErrors { get; set; }

            public override IEnumerable<SyntaxNode> ChildNodes()
            {
                return this.childNodes;
            }

            public override IEnumerable<TemplateError> Validate()
            {
                return this.ValidationErrors ?? base.Validate();
            }

            protected internal override void Accept(SyntaxNodeVisitor visitor)
            {
                throw new NotImplementedException();
            }
        }
    }
}