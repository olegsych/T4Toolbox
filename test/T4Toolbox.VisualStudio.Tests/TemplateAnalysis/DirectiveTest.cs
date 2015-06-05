// <copyright file="DirectiveTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.TemplateAnalysis
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.VisualStudio.Text;

    using DescriptionAttribute = System.ComponentModel.DescriptionAttribute;

    [TestClass]
    public class DirectiveTest
    {
        [TestMethod]
        public void DirectiveIsSubclassOfNonterminalNode()
        {
            Assert.IsTrue(typeof(Directive).IsSubclassOf(typeof(NonterminalNode)));
        }

        [TestMethod]
        public void DirectiveIsAbstract()
        {
            Assert.IsTrue(typeof(Directive).IsAbstract);
        }

        [TestMethod]
        public void AttributesReturnsCaseInsensitiveDictionaryOfAttributesByName()
        {
            var a1 = new Attribute(new AttributeName(13, "language"), new Equals(21), new DoubleQuote(22), new AttributeValue(23, "C#"), new DoubleQuote(25));
            var a2 = new Attribute(new AttributeName(27, "debug"), new Equals(32), new DoubleQuote(33), new AttributeValue(34, "True"), new DoubleQuote(38));
            var directive = new TestableDirective(new DirectiveBlockStart(0), new DirectiveName(4, "template"), new[] { a1, a2 }, new BlockEnd(27));            
            IReadOnlyDictionary<string, Attribute> dictionary = directive.Attributes;
            Assert.IsNotNull(dictionary);
            Assert.AreSame(a1, dictionary["Language"]);
            Assert.AreSame(a2, dictionary["Debug"]);
        }

        [TestMethod]
        public void ChildNodesReturnsNodesSpecifiedInConstructor()
        {
            var start = new DirectiveBlockStart(0);
            var name = new DirectiveName(4, "template");
            var a1 = new Attribute(new AttributeName(13, "language"), new Equals(21), new DoubleQuote(22), new AttributeValue(23, "C#"), new DoubleQuote(25));
            var a2 = new Attribute(new AttributeName(27, "debug"), new Equals(32), new DoubleQuote(33), new AttributeValue(34, "True"), new DoubleQuote(38));
            var end = new BlockEnd(27);
            var directive = new TestableDirective(start, name, new[] { a1, a2 }, end);
            Assert.IsTrue(directive.ChildNodes().SequenceEqual(new SyntaxNode[] { start, name, a1, a2, end }));
        }

        #region Create

        [TestMethod]
        public void CreateReturnsAssemblyDirectiveGivenAssemblyDirectiveName()
        {
            var directive = Directive.Create(new DirectiveBlockStart(0), new DirectiveName(4, "assembly"), new Attribute[0], new BlockEnd(0));
            Assert.AreEqual(typeof(AssemblyDirective), directive.GetType());
        }

        [TestMethod]
        public void CreateReturnsCustomDirectiveGivenUnrecognizedDirectiveName()
        {
            var directive = Directive.Create(new DirectiveBlockStart(0), new DirectiveName(4, "custom"), new Attribute[0], new BlockEnd(0));
            Assert.AreEqual(typeof(CustomDirective), directive.GetType());
        }

        [TestMethod]
        public void CreateReturnsIncludeDirectiveGivenIncludeDirectiveName()
        {
            var directive = Directive.Create(new DirectiveBlockStart(0), new DirectiveName(4, "Include"), new Attribute[0], new BlockEnd(0));
            Assert.AreEqual(typeof(IncludeDirective), directive.GetType());
        }

        [TestMethod]
        public void CreateReturnsImportDirectiveGivenImportDirectiveName()
        {
            var directive = Directive.Create(new DirectiveBlockStart(0), new DirectiveName(4, "Import"), new Attribute[0], new BlockEnd(0));
            Assert.AreEqual(typeof(ImportDirective), directive.GetType());
        }

        [TestMethod]
        public void CreateReturnsOutputDirectiveGivenOutputDirectiveName()
        {
            var directive = Directive.Create(new DirectiveBlockStart(0), new DirectiveName(4, "Output"), new Attribute[0], new BlockEnd(0));
            Assert.AreEqual(typeof(OutputDirective), directive.GetType());
        }

        [TestMethod]
        public void CreateReturnsParameterDirectiveGivenParameterDirectiveName()
        {
            var directive = Directive.Create(new DirectiveBlockStart(0), new DirectiveName(4, "Parameter"), new Attribute[0], new BlockEnd(0));
            Assert.AreEqual(typeof(ParameterDirective), directive.GetType());
        }

        [TestMethod]
        public void CreateReturnsTemplateDirectiveGivenTemplateDirectiveName()
        {
            var directive = Directive.Create(new DirectiveBlockStart(0), new DirectiveName(4, "Template"), new Attribute[0], new BlockEnd(0));
            Assert.AreEqual(typeof(TemplateDirective), directive.GetType());
        }

        #endregion

        [TestMethod]
        public void DirectiveNameReturnsTextOfDirectiveNameNode()
        {
            var directive = new TestableDirective(new DirectiveBlockStart(0), new DirectiveName(4, "template"), new Attribute[0], new BlockEnd(13));
            Assert.AreEqual("template", directive.DirectiveName);
        }

        [TestMethod]
        public void KindReturnsDirectiveSyntaxKind()
        {
            var directive = new TestableDirective(new DirectiveBlockStart(0), new DirectiveName(4, "template"), new Attribute[0], new BlockEnd(13));
            Assert.AreEqual(SyntaxKind.Directive, directive.Kind);
        }

        [TestMethod]
        public void GetAttributeNameReturnsValueOfAttributeWithGivenName()
        {
            var directive = new TestableDirective(
                new DirectiveBlockStart(0),
                new DirectiveName(4, "template"),
                new[] { new Attribute(new AttributeName(13, "hostspecific"), new Equals(26), new DoubleQuote(27), new AttributeValue(28, "true"), new DoubleQuote(32)) },
                new BlockEnd(23));
            Assert.AreEqual("true", directive.GetAttributeValue("hostspecific"));
        }

        [TestMethod]
        public void GetAttributeNameReturnsValueOfAttributeWithGivenDisplayName()
        {
            var directive = new DirectiveWithAttributeProperty(
                new DirectiveBlockStart(0),
                new DirectiveName(4, "directive"),
                new[] { new Attribute(new AttributeName(13, "attribute"), new Equals(26), new DoubleQuote(27), new AttributeValue(28, "true"), new DoubleQuote(32)) },
                new BlockEnd(23));
            Assert.AreEqual("true", directive.GetAttributeValue("property"));
        }

        #region GetDescription

        [TestMethod]
        public void GetDescriptionReturnsDescriptionOfBlockStart()
        {
            var directive = new DirectiveWithDescription(new DirectiveBlockStart(0), new DirectiveName(4, "directive"), new Attribute[0], new BlockEnd(14));
            string description;
            Span applicableTo;
            Assert.IsTrue(directive.TryGetDescription(0, out description, out applicableTo));
        }

        [TestMethod]
        public void GetDescriptionReturnsStringSpecifiedInDescriptionAttributeForAttributeProperty()
        {
            var attribute = new Attribute(new AttributeName(13, "attribute"), new Equals(26), new DoubleQuote(27), new AttributeValue(28, "true"), new DoubleQuote(32));
            var directive = new DirectiveWithAttributeProperty(
                new DirectiveBlockStart(0),
                new DirectiveName(4, "directive"),
                new[] { attribute },
                new BlockEnd(23));
            DescriptionAttribute descriptionAttribute = directive.GetType().GetProperty("Property").GetCustomAttributes(false).OfType<DescriptionAttribute>().Single();
            string description;
            Span applicableTo;
            Assert.IsTrue(directive.TryGetDescription(13, out description, out applicableTo));
            Assert.AreEqual(descriptionAttribute.Description, description);
            Assert.AreEqual(attribute.Span, applicableTo);
        }

        [TestMethod]
        public void GetDescriptionReturnsDescriptionOfDirectiveAndSpanOfDirectiveNameGivenPositionWithinDirectiveName()
        {
            var directiveName = new DirectiveName(4, "directive");
            var directive = new DirectiveWithDescription(new DirectiveBlockStart(0), directiveName, new Attribute[0], new BlockEnd(14));
            DescriptionAttribute descriptionAttribute = directive.GetType().GetCustomAttributes(false).OfType<DescriptionAttribute>().Single();
            string description;
            Span applicableTo;
            Assert.IsTrue(directive.TryGetDescription(4, out description, out applicableTo));
            Assert.AreEqual(descriptionAttribute.Description, description);
            Assert.AreEqual(directiveName.Span, applicableTo);
        }

        [TestMethod]
        public void GetDescriptionReturnsEmptyStringAndSpanGivenPositionBetweenItsChildNodesToPreventDirectiveTooltipFromStickingDuringHorizontalMouseMovement()
        {
            var directive = new DirectiveWithDescription(new DirectiveBlockStart(0), new DirectiveName(4, "directive"), new Attribute[0], new BlockEnd(14));
            string description;
            Span applicableTo;
            Assert.IsFalse(directive.TryGetDescription(3, out description, out applicableTo));
            Assert.AreEqual(string.Empty, description);
            Assert.AreEqual(default(Span), applicableTo);
        }

        [TestMethod]
        public void GetDescriptionReturnsDescriptionOfBlockEnd()
        {
            var directive = new DirectiveWithDescription(new DirectiveBlockStart(0), new DirectiveName(4, "directive"), new Attribute[0], new BlockEnd(14));
            string description;
            Span applicableTo;
            Assert.IsTrue(directive.TryGetDescription(14, out description, out applicableTo));
        }

        #endregion

        [TestMethod]
        public void PositionReturnsPositionOfBlockStart()
        {
            var target = new TestableDirective(new DirectiveBlockStart(0, new Position(4, 2)), new DirectiveName(0, string.Empty), Enumerable.Empty<Attribute>(), new BlockEnd(0));
            Assert.AreEqual(new Position(4, 2), target.Position);
        }

        [TestMethod]
        public void SpanStartsAtBlockStart()
        {
            DirectiveBlockStart start;
            var directive = new TestableDirective(
                start = new DirectiveBlockStart(10), 
                new DirectiveName(14, "template"), 
                new Attribute[0], 
                new BlockEnd(23));
            Assert.AreEqual(start.Span.Start, directive.Span.Start);
        }

        [TestMethod]
        public void SpanEndsAtBlockEnd()
        {
            BlockEnd end;
            var directive = new TestableDirective(
                new DirectiveBlockStart(10),
                new DirectiveName(14, "template"),
                new Attribute[0],
                end = new BlockEnd(23));
            Assert.AreEqual(end.Span.End, directive.Span.End);
        }

        [TestMethod]
        public void ValidateReturnsTemplateErrorWhenAttributeValueDoesNotPassPropertyValidationAttributes()
        {
            var directive = new DirectiveWithRequiredAttribute(new DirectiveBlockStart(0), new DirectiveName(4, "template"), new Attribute[0], new BlockEnd(13));
            TemplateError error = directive.Validate().Single();
            StringAssert.Contains(error.Message, "RequiredAttribute");
            Assert.AreEqual(directive.Span, error.Span);
            Assert.AreEqual(directive.Position, error.Position);
        }

        [TestMethod]
        public void ValidateReturnsTemplateErrorWhenAttributeValueDoesNotMatchKnownValues()
        {
            AttributeValue value;
            var directive = new DirectiveWithKnownAttributeValues(
                new DirectiveBlockStart(0), 
                new DirectiveName(4, "directive"), 
                new[] { new Attribute(new AttributeName(13, "attributeWithKnownValues"), new Equals(37), new DoubleQuote(38), value = new AttributeValue(39, "wrong"), new DoubleQuote(44)) },
                new BlockEnd(46));
            TemplateError error = directive.Validate().Single();
            StringAssert.Contains(error.Message, "attributeWithKnownValues");
            StringAssert.Contains(error.Message, "wrong");
            Assert.AreEqual(value.Span, error.Span);
            Assert.AreEqual(value.Position, error.Position);
        }

        [TestMethod]
        public void ValidateIgnoresCaseWhenComparingAttributeValuesToKnownValues()
        {
            var directive = new DirectiveWithKnownAttributeValues(
                new DirectiveBlockStart(0), 
                new DirectiveName(4, "directive"), 
                new[] { new Attribute(new AttributeName(13, "attributeWithKnownValues"), new Equals(37), new DoubleQuote(38), new AttributeValue(39, "KNOWNVALUE"), new DoubleQuote(49)) },
                new BlockEnd(51));
            Assert.IsFalse(directive.Validate().Any());            
        }

        [TestMethod]
        public void ValidateReturnsTemplateErrorWhenDirectiveContainsUnrecognizedAttribute()
        {
            var unrecognizedAttribute = new Attribute(new AttributeName(27, "UnrecognizedAttribute"), new Equals(32), new DoubleQuote(33), new AttributeValue(34, "v2"), new DoubleQuote(38));
            var directive = new TestableDirective(
                new DirectiveBlockStart(10),
                new DirectiveName(14, "template"),
                new[] { unrecognizedAttribute },
                new BlockEnd(23));
            TemplateError error = directive.Validate().Single();
            StringAssert.Contains(error.Message, unrecognizedAttribute.Name);
            Assert.AreEqual(unrecognizedAttribute.Span, error.Span);
            Assert.AreEqual(unrecognizedAttribute.Position, error.Position);
        }

        [TestMethod]
        public void ValidateDoesNotReturnErrorsForAttributesWithoutKnownValues()
        {
            var directive = new DirectiveWithAttributeProperty(
                new DirectiveBlockStart(10),
                new DirectiveName(14, "directive"),
                new[] { new Attribute(new AttributeName(27, "attribute"), new Equals(32), new DoubleQuote(33), new AttributeValue(34, "value"), new DoubleQuote(38)) },
                new BlockEnd(23));
            Assert.IsFalse(directive.Validate().Any());
        }

        private class TestableDirective : Directive
        {
            public TestableDirective(DirectiveBlockStart start, DirectiveName name, IEnumerable<Attribute> attributes, BlockEnd end)
                : base(start, name, attributes, end)
            {
            }

            public new string GetAttributeValue([CallerMemberName]string propertyName = null)
            {
                return base.GetAttributeValue(propertyName);
            }

            protected internal override void Accept(SyntaxNodeVisitor visitor)
            {
                throw new NotImplementedException();
            }
        }

        [Description("Directive Description")]
        private class DirectiveWithDescription : TestableDirective
        {
            public DirectiveWithDescription(DirectiveBlockStart start, DirectiveName name, IEnumerable<Attribute> attributes, BlockEnd end)
                : base(start, name, attributes, end)
            {
            }            
        }

        private class DirectiveWithRequiredAttribute : TestableDirective
        {
            public DirectiveWithRequiredAttribute(DirectiveBlockStart start, DirectiveName name, IEnumerable<Attribute> attributes, BlockEnd end)
                : base(start, name, attributes, end)
            {
            }

            [Required]
            public string RequiredAttribute
            {
                get { return this.GetAttributeValue(); }
            }
        }

        private class DirectiveWithAttributeProperty : TestableDirective
        {
            public DirectiveWithAttributeProperty(DirectiveBlockStart start, DirectiveName name, IEnumerable<Attribute> attributes, BlockEnd end)
                : base(start, name, attributes, end)
            {
            }

            [DisplayName("Attribute")]
            [Description("Attribute Description")]
            public string Property
            {
                get { return this.GetAttributeValue(); }
            }
        }

        private class DirectiveWithKnownAttributeValues : TestableDirective
        {
            public DirectiveWithKnownAttributeValues(DirectiveBlockStart start, DirectiveName name, IEnumerable<Attribute> attributes, BlockEnd end)
                : base(start, name, attributes, end)
            {
            }

            private enum AttributeValue
            {
                KnownValue
            }

            [KnownValues(typeof(AttributeValue))]
            public string AttributeWithKnownValues
            {
                get { return this.GetAttributeValue(); }
            }
        }
    }
}