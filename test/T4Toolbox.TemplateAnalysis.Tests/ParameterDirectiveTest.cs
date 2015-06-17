// <copyright file="ParameterDirectiveTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.TemplateAnalysis
{
    using System;
    using System.Linq;
    using Microsoft.VisualStudio.Text;
    using NSubstitute;
    using Xunit;

    public static class ParameterDirectiveTest
    {
        [Fact]
        public static void ParameterDirectiveIsSubclassOfDirective()
        {
            Assert.True(typeof(ParameterDirective).IsSubclassOf(typeof(Directive)));
        }

        [Fact]
        public static void ParameterDirectiveIsSealed()
        {
            Assert.True(typeof(ParameterDirective).IsSealed);
        }

        [Fact]
        public static void AcceptCallsVisitParameterDirectiveMethodOfSyntaxNodeVisitor()
        {
            var visitor = Substitute.For<SyntaxNodeVisitor>();
            var directive = new ParameterDirective(new DirectiveBlockStart(0), new DirectiveName(4, "parameter"), new Attribute[0], new BlockEnd(24));
            directive.Accept(visitor);
            visitor.Received().VisitParameterDirective(directive);
        }

        [Fact]
        public static void GetDescriptionReturnsDescriptionOfDirective()
        {
            var directive = new ParameterDirective(new DirectiveBlockStart(0), new DirectiveName(4, "parameter"), new Attribute[0], new BlockEnd(24));
            string description;
            Span applicableTo;
            Assert.True(directive.TryGetDescription(4, out description, out applicableTo));
            Assert.Contains("property in template code", description, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public static void GetDescriptionReturnsDescriptionOfNameAttribute()
        {
            var directive = new ParameterDirective(
                new DirectiveBlockStart(0),
                new DirectiveName(4, "parameter"),
                new[] { new Attribute(new AttributeName(14, "name"), new Equals(18), new DoubleQuote(19), new AttributeValue(20, "42"), new DoubleQuote(22)) },
                new BlockEnd(24));
            string description;
            Span applicableTo;
            Assert.True(directive.TryGetDescription(14, out description, out applicableTo));
            Assert.Contains("Name of the property", description, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public static void GetDescriptionReturnsDescriptionOfTypeAttribute()
        {
            var directive = new ParameterDirective(
                new DirectiveBlockStart(0),
                new DirectiveName(4, "parameter"),
                new[] { new Attribute(new AttributeName(14, "type"), new Equals(18), new DoubleQuote(19), new AttributeValue(20, "42"), new DoubleQuote(22)) },
                new BlockEnd(24));
            string description;
            Span applicableTo;
            Assert.True(directive.TryGetDescription(14, out description, out applicableTo));
            Assert.Contains("Fully-qualified name of the property type", description, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public static void ParameterNameReturnsValueOfNameAttribute()
        {
            var directive = new ParameterDirective(
                new DirectiveBlockStart(0),
                new DirectiveName(4, "parameter"),
                new[] { new Attribute(new AttributeName(14, "name"), new Equals(18), new DoubleQuote(19), new AttributeValue(20, "42"), new DoubleQuote(22)) },
                new BlockEnd(24));
            Assert.Equal("42", directive.ParameterName);
        }

        [Fact]
        public static void ParameterNameReturnsEmptyStringWhenNameAttributeIsNotSpecified()
        {
            var directive = new ParameterDirective(new DirectiveBlockStart(0), new DirectiveName(4, "parameter"), new Attribute[0], new BlockEnd(24));
            Assert.Equal(string.Empty, directive.ParameterName);            
        }

        [Fact]
        public static void ParameterTypeReturnsValueOfTypeAttribute()
        {
            var directive = new ParameterDirective(
                new DirectiveBlockStart(0),
                new DirectiveName(4, "parameter"),
                new[] { new Attribute(new AttributeName(14, "type"), new Equals(18), new DoubleQuote(19), new AttributeValue(20, "42"), new DoubleQuote(22)) },
                new BlockEnd(24));
            Assert.Equal("42", directive.ParameterType);
        }

        [Fact]
        public static void ParameterTypeReturnsEmptyStringWhenTypeAttributeIsNotSpecified()
        {
            var directive = new ParameterDirective(new DirectiveBlockStart(0), new DirectiveName(4, "parameter"), new Attribute[0], new BlockEnd(24));
            Assert.Equal(string.Empty, directive.ParameterType);
        }

        [Fact]
        public static void ValidateReturnsErrorWhenNameAttributeIsNotSpecified()
        {
            var directive = new ParameterDirective(
                new DirectiveBlockStart(0),
                new DirectiveName(4, "parameter"),
                new[] { new Attribute(new AttributeName(14, "type"), new Equals(18), new DoubleQuote(19), new AttributeValue(20, "System.Int32"), new DoubleQuote(22)) },
                new BlockEnd(24));
            TemplateError error = directive.Validate().Single();
            Assert.Contains("Name", error.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public static void ValidateReturnsErrorWhenTypeAttributeIsNotSpecified()
        {
            var directive = new ParameterDirective(
                new DirectiveBlockStart(0),
                new DirectiveName(4, "parameter"),
                new[] { new Attribute(new AttributeName(14, "name"), new Equals(18), new DoubleQuote(19), new AttributeValue(20, "p1"), new DoubleQuote(22)) },
                new BlockEnd(24));
            TemplateError error = directive.Validate().Single();
            Assert.Contains("Type", error.Message, StringComparison.OrdinalIgnoreCase);
        }
    }
}