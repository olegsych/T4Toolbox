// <copyright file="CustomDirectiveTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.TemplateAnalysis
{
    using System;
    using System.Linq;
    using Microsoft.VisualStudio.Text;
    using NSubstitute;
    using Xunit;

    public static class CustomDirectiveTest
    {
        [Fact]
        public static void CustomDirectiveIsSubclassOfDirective()
        {
            Assert.True(typeof(CustomDirective).IsSubclassOf(typeof(Directive)));
        }

        [Fact]
        public static void CustomDirectiveIsSealed()
        {
            Assert.True(typeof(CustomDirective).IsSealed);
        }

        [Fact]
        public static void AcceptCallsVisitCustomDirectiveMethodOfSyntaxNodeVisitor()
        {
            var visitor = Substitute.For<SyntaxNodeVisitor>();
            var directive = new CustomDirective(new DirectiveBlockStart(0), new DirectiveName(4, "custom"), new Attribute[0], new BlockEnd(24));
            directive.Accept(visitor);
            visitor.Received().VisitCustomDirective(directive);
        }

        [Fact]
        public static void GetDescriptionReturnsDescriptionOfDirective()
        {
            var directive = new CustomDirective(new DirectiveBlockStart(0), new DirectiveName(4, "custom"), new Attribute[0], new BlockEnd(24));
            string description;
            Span applicableTo;
            Assert.True(directive.TryGetDescription(4, out description, out applicableTo));
            Assert.Contains("directive", description, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public static void GetDescriptionReturnsDescriptionOfProcessorAttribute()
        {
            var directive = new CustomDirective(
                new DirectiveBlockStart(0),
                new DirectiveName(4, "custom"),
                new[] { new Attribute(new AttributeName(13, "processor"), new Equals(21), new DoubleQuote(22), new AttributeValue(23, "CustomProcessor"), new DoubleQuote(22)) },
                new BlockEnd(24));
            string description;
            Span applicableTo;
            Assert.True(directive.TryGetDescription(13, out description, out applicableTo));
            Assert.Contains("processor", description, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public static void ProcessorReturnsValueOfProcessorAttribute()
        {
            var directive = new CustomDirective(
                new DirectiveBlockStart(0),
                new DirectiveName(4, "custom"),
                new[] { new Attribute(new AttributeName(13, "processor"), new Equals(21), new DoubleQuote(22), new AttributeValue(23, "CustomProcessor"), new DoubleQuote(22)) },
                new BlockEnd(24));
            Assert.Equal("CustomProcessor", directive.Processor);
        }

        [Fact]
        public static void ProcessorReturnsEmptyStringWhenProcessorAttributeIsNotSpecified()
        {
            var directive = new CustomDirective(new DirectiveBlockStart(0), new DirectiveName(4, "custom"), new Attribute[0], new BlockEnd(24));
            Assert.Equal(string.Empty, directive.Processor);
        }

        [Fact]
        public static void ValidateReturnsErrorWhenProcessorAttributeIsNotSpecified()
        {
            var directive = new CustomDirective(new DirectiveBlockStart(0), new DirectiveName(4, "custom"), new Attribute[0], new BlockEnd(24));
            TemplateError error = directive.Validate().Single();
            Assert.Contains("Processor", error.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public static void ValidateReturnsNoErrorsWhenDirectiveContainsUnrecognizedAttributes()
        {
            var a1 = new Attribute(new AttributeName(13, "processor"), new Equals(21), new DoubleQuote(22), new AttributeValue(23, "CustomProcessor"), new DoubleQuote(22));
            var a2 = new Attribute(new AttributeName(24, "custom"), new Equals(30), new DoubleQuote(31), new AttributeValue(32, "CustomValue"), new DoubleQuote(43));
            var directive = new CustomDirective(
                new DirectiveBlockStart(0), 
                new DirectiveName(4, "custom"),
                new[] { a1, a2 },
                new BlockEnd(45));
            Assert.False(directive.Validate().Any());
        }
    }
}