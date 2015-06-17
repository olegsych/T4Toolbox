// <copyright file="DirectiveTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.TemplateAnalysis
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using Microsoft.VisualStudio.Text;
    using Xunit;

    public static class DirectiveTest
    {
        [Fact]
        public static void DirectiveIsSubclassOfNonterminalNode()
        {
            Assert.True(typeof(Directive).IsSubclassOf(typeof(NonterminalNode)));
        }

        [Fact]
        public static void DirectiveIsAbstract()
        {
            Assert.True(typeof(Directive).IsAbstract);
        }

        [Fact]
        public static void AttributesReturnsCaseInsensitiveDictionaryOfAttributesByName()
        {
            var a1 = new Attribute(new AttributeName(13, "language"), new Equals(21), new DoubleQuote(22), new AttributeValue(23, "C#"), new DoubleQuote(25));
            var a2 = new Attribute(new AttributeName(27, "debug"), new Equals(32), new DoubleQuote(33), new AttributeValue(34, "True"), new DoubleQuote(38));
            var directive = new TestableDirective(new DirectiveBlockStart(0), new DirectiveName(4, "template"), new[] { a1, a2 }, new BlockEnd(27));            
            IReadOnlyDictionary<string, Attribute> dictionary = directive.Attributes;
            Assert.NotNull(dictionary);
            Assert.Same(a1, dictionary["Language"]);
            Assert.Same(a2, dictionary["Debug"]);
        }

        [Fact]
        public static void ChildNodesReturnsNodesSpecifiedInConstructor()
        {
            var start = new DirectiveBlockStart(0);
            var name = new DirectiveName(4, "template");
            var a1 = new Attribute(new AttributeName(13, "language"), new Equals(21), new DoubleQuote(22), new AttributeValue(23, "C#"), new DoubleQuote(25));
            var a2 = new Attribute(new AttributeName(27, "debug"), new Equals(32), new DoubleQuote(33), new AttributeValue(34, "True"), new DoubleQuote(38));
            var end = new BlockEnd(27);
            var directive = new TestableDirective(start, name, new[] { a1, a2 }, end);
            Assert.True(directive.ChildNodes().SequenceEqual(new SyntaxNode[] { start, name, a1, a2, end }));
        }

        #region Create

        [Fact]
        public static void CreateReturnsAssemblyDirectiveGivenAssemblyDirectiveName()
        {
            var directive = Directive.Create(new DirectiveBlockStart(0), new DirectiveName(4, "assembly"), new Attribute[0], new BlockEnd(0));
            Assert.Equal(typeof(AssemblyDirective), directive.GetType());
        }

        [Fact]
        public static void CreateReturnsCustomDirectiveGivenUnrecognizedDirectiveName()
        {
            var directive = Directive.Create(new DirectiveBlockStart(0), new DirectiveName(4, "custom"), new Attribute[0], new BlockEnd(0));
            Assert.Equal(typeof(CustomDirective), directive.GetType());
        }

        [Fact]
        public static void CreateReturnsIncludeDirectiveGivenIncludeDirectiveName()
        {
            var directive = Directive.Create(new DirectiveBlockStart(0), new DirectiveName(4, "Include"), new Attribute[0], new BlockEnd(0));
            Assert.Equal(typeof(IncludeDirective), directive.GetType());
        }

        [Fact]
        public static void CreateReturnsImportDirectiveGivenImportDirectiveName()
        {
            var directive = Directive.Create(new DirectiveBlockStart(0), new DirectiveName(4, "Import"), new Attribute[0], new BlockEnd(0));
            Assert.Equal(typeof(ImportDirective), directive.GetType());
        }

        [Fact]
        public static void CreateReturnsOutputDirectiveGivenOutputDirectiveName()
        {
            var directive = Directive.Create(new DirectiveBlockStart(0), new DirectiveName(4, "Output"), new Attribute[0], new BlockEnd(0));
            Assert.Equal(typeof(OutputDirective), directive.GetType());
        }

        [Fact]
        public static void CreateReturnsParameterDirectiveGivenParameterDirectiveName()
        {
            var directive = Directive.Create(new DirectiveBlockStart(0), new DirectiveName(4, "Parameter"), new Attribute[0], new BlockEnd(0));
            Assert.Equal(typeof(ParameterDirective), directive.GetType());
        }

        [Fact]
        public static void CreateReturnsTemplateDirectiveGivenTemplateDirectiveName()
        {
            var directive = Directive.Create(new DirectiveBlockStart(0), new DirectiveName(4, "Template"), new Attribute[0], new BlockEnd(0));
            Assert.Equal(typeof(TemplateDirective), directive.GetType());
        }

        #endregion

        [Fact]
        public static void DirectiveNameReturnsTextOfDirectiveNameNode()
        {
            var directive = new TestableDirective(new DirectiveBlockStart(0), new DirectiveName(4, "template"), new Attribute[0], new BlockEnd(13));
            Assert.Equal("template", directive.DirectiveName);
        }

        [Fact]
        public static void KindReturnsDirectiveSyntaxKind()
        {
            var directive = new TestableDirective(new DirectiveBlockStart(0), new DirectiveName(4, "template"), new Attribute[0], new BlockEnd(13));
            Assert.Equal(SyntaxKind.Directive, directive.Kind);
        }

        [Fact]
        public static void GetAttributeNameReturnsValueOfAttributeWithGivenName()
        {
            var directive = new TestableDirective(
                new DirectiveBlockStart(0),
                new DirectiveName(4, "template"),
                new[] { new Attribute(new AttributeName(13, "hostspecific"), new Equals(26), new DoubleQuote(27), new AttributeValue(28, "true"), new DoubleQuote(32)) },
                new BlockEnd(23));
            Assert.Equal("true", directive.GetAttributeValue("hostspecific"));
        }

        [Fact]
        public static void GetAttributeNameReturnsValueOfAttributeWithGivenDisplayName()
        {
            var directive = new DirectiveWithAttributeProperty(
                new DirectiveBlockStart(0),
                new DirectiveName(4, "directive"),
                new[] { new Attribute(new AttributeName(13, "attribute"), new Equals(26), new DoubleQuote(27), new AttributeValue(28, "true"), new DoubleQuote(32)) },
                new BlockEnd(23));
            Assert.Equal("true", directive.GetAttributeValue("property"));
        }

        #region GetDescription

        [Fact]
        public static void GetDescriptionReturnsDescriptionOfBlockStart()
        {
            var directive = new DirectiveWithDescription(new DirectiveBlockStart(0), new DirectiveName(4, "directive"), new Attribute[0], new BlockEnd(14));
            string description;
            Span applicableTo;
            Assert.True(directive.TryGetDescription(0, out description, out applicableTo));
        }

        [Fact]
        public static void GetDescriptionReturnsStringSpecifiedInDescriptionAttributeForAttributeProperty()
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
            Assert.True(directive.TryGetDescription(13, out description, out applicableTo));
            Assert.Equal(descriptionAttribute.Description, description);
            Assert.Equal(attribute.Span, applicableTo);
        }

        [Fact]
        public static void GetDescriptionReturnsDescriptionOfDirectiveAndSpanOfDirectiveNameGivenPositionWithinDirectiveName()
        {
            var directiveName = new DirectiveName(4, "directive");
            var directive = new DirectiveWithDescription(new DirectiveBlockStart(0), directiveName, new Attribute[0], new BlockEnd(14));
            DescriptionAttribute descriptionAttribute = directive.GetType().GetCustomAttributes(false).OfType<DescriptionAttribute>().Single();
            string description;
            Span applicableTo;
            Assert.True(directive.TryGetDescription(4, out description, out applicableTo));
            Assert.Equal(descriptionAttribute.Description, description);
            Assert.Equal(directiveName.Span, applicableTo);
        }

        [Fact]
        public static void GetDescriptionReturnsEmptyStringAndSpanGivenPositionBetweenItsChildNodesToPreventDirectiveTooltipFromStickingDuringHorizontalMouseMovement()
        {
            var directive = new DirectiveWithDescription(new DirectiveBlockStart(0), new DirectiveName(4, "directive"), new Attribute[0], new BlockEnd(14));
            string description;
            Span applicableTo;
            Assert.False(directive.TryGetDescription(3, out description, out applicableTo));
            Assert.Equal(string.Empty, description);
            Assert.Equal(default(Span), applicableTo);
        }

        [Fact]
        public static void GetDescriptionReturnsDescriptionOfBlockEnd()
        {
            var directive = new DirectiveWithDescription(new DirectiveBlockStart(0), new DirectiveName(4, "directive"), new Attribute[0], new BlockEnd(14));
            string description;
            Span applicableTo;
            Assert.True(directive.TryGetDescription(14, out description, out applicableTo));
        }

        #endregion

        [Fact]
        public static void PositionReturnsPositionOfBlockStart()
        {
            var target = new TestableDirective(new DirectiveBlockStart(0, new Position(4, 2)), new DirectiveName(0, string.Empty), Enumerable.Empty<Attribute>(), new BlockEnd(0));
            Assert.Equal(new Position(4, 2), target.Position);
        }

        [Fact]
        public static void SpanStartsAtBlockStart()
        {
            DirectiveBlockStart start;
            var directive = new TestableDirective(
                start = new DirectiveBlockStart(10), 
                new DirectiveName(14, "template"), 
                new Attribute[0], 
                new BlockEnd(23));
            Assert.Equal(start.Span.Start, directive.Span.Start);
        }

        [Fact]
        public static void SpanEndsAtBlockEnd()
        {
            BlockEnd end;
            var directive = new TestableDirective(
                new DirectiveBlockStart(10),
                new DirectiveName(14, "template"),
                new Attribute[0],
                end = new BlockEnd(23));
            Assert.Equal(end.Span.End, directive.Span.End);
        }

        [Fact]
        public static void ValidateReturnsTemplateErrorWhenAttributeValueDoesNotPassPropertyValidationAttributes()
        {
            var directive = new DirectiveWithRequiredAttribute(new DirectiveBlockStart(0), new DirectiveName(4, "template"), new Attribute[0], new BlockEnd(13));
            TemplateError error = directive.Validate().Single();
            Assert.Contains("RequiredAttribute", error.Message, StringComparison.OrdinalIgnoreCase);
            Assert.Equal(directive.Span, error.Span);
            Assert.Equal(directive.Position, error.Position);
        }

        [Fact]
        public static void ValidateReturnsTemplateErrorWhenAttributeValueDoesNotMatchKnownValues()
        {
            AttributeValue value;
            var directive = new DirectiveWithKnownAttributeValues(
                new DirectiveBlockStart(0), 
                new DirectiveName(4, "directive"), 
                new[] { new Attribute(new AttributeName(13, "attributeWithKnownValues"), new Equals(37), new DoubleQuote(38), value = new AttributeValue(39, "wrong"), new DoubleQuote(44)) },
                new BlockEnd(46));
            TemplateError error = directive.Validate().Single();
            Assert.Contains("attributeWithKnownValues", error.Message, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("wrong", error.Message, StringComparison.OrdinalIgnoreCase);
            Assert.Equal(value.Span, error.Span);
            Assert.Equal(value.Position, error.Position);
        }

        [Fact]
        public static void ValidateIgnoresCaseWhenComparingAttributeValuesToKnownValues()
        {
            var directive = new DirectiveWithKnownAttributeValues(
                new DirectiveBlockStart(0), 
                new DirectiveName(4, "directive"), 
                new[] { new Attribute(new AttributeName(13, "attributeWithKnownValues"), new Equals(37), new DoubleQuote(38), new AttributeValue(39, "KNOWNVALUE"), new DoubleQuote(49)) },
                new BlockEnd(51));
            Assert.False(directive.Validate().Any());            
        }

        [Fact]
        public static void ValidateReturnsTemplateErrorWhenDirectiveContainsUnrecognizedAttribute()
        {
            var unrecognizedAttribute = new Attribute(new AttributeName(27, "UnrecognizedAttribute"), new Equals(32), new DoubleQuote(33), new AttributeValue(34, "v2"), new DoubleQuote(38));
            var directive = new TestableDirective(
                new DirectiveBlockStart(10),
                new DirectiveName(14, "template"),
                new[] { unrecognizedAttribute },
                new BlockEnd(23));
            TemplateError error = directive.Validate().Single();
            Assert.Contains(unrecognizedAttribute.Name, error.Message, StringComparison.OrdinalIgnoreCase);
            Assert.Equal(unrecognizedAttribute.Span, error.Span);
            Assert.Equal(unrecognizedAttribute.Position, error.Position);
        }

        [Fact]
        public static void ValidateDoesNotReturnErrorsForAttributesWithoutKnownValues()
        {
            var directive = new DirectiveWithAttributeProperty(
                new DirectiveBlockStart(10),
                new DirectiveName(14, "directive"),
                new[] { new Attribute(new AttributeName(27, "attribute"), new Equals(32), new DoubleQuote(33), new AttributeValue(34, "value"), new DoubleQuote(38)) },
                new BlockEnd(23));
            Assert.False(directive.Validate().Any());
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