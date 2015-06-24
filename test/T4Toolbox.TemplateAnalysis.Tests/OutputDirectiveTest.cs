// <copyright file="OutputDirectiveTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.TemplateAnalysis
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using Microsoft.VisualStudio.Text;
    using NSubstitute;
    using Xunit;

    public static class OutputDirectiveTest
    {
        [Fact]
        public static void OutputDirectiveIsSubclassOfDirective()
        {
            Assert.True(typeof(OutputDirective).IsSubclassOf(typeof(Directive)));
        }

        [Fact]
        public static void OutputDirectiveIsSealed()
        {
            Assert.True(typeof(OutputDirective).IsSealed);
        }

        [Fact]
        public static void AcceptCallsVisitOutputDirectiveMethodOfSyntaxNodeVisitor()
        {
            var visitor = Substitute.For<SyntaxNodeVisitor>();
            var directive = new OutputDirective(new DirectiveBlockStart(0), new DirectiveName(4, "output"), new Attribute[0], new BlockEnd(24));
            directive.Accept(visitor);
            visitor.Received().VisitOutputDirective(directive);
        }

        [Fact]
        public static void ExtensionReturnsValueOfExtensionAttribute()
        {
            var directive = new OutputDirective(
                new DirectiveBlockStart(0),
                new DirectiveName(4, "output"),
                new[] { new Attribute(new AttributeName(14, "extension"), new Equals(18), new DoubleQuote(19), new AttributeValue(20, ".txt"), new DoubleQuote(22)) },
                new BlockEnd(24));
            Assert.Equal(".txt", directive.Extension);
        }

        [Fact]
        public static void ExtensionReturnsEmptyStringWhenExtensionAttributeIsNotSpecified()
        {
            var directive = new OutputDirective(new DirectiveBlockStart(0), new DirectiveName(4, "output"), new Attribute[0], new BlockEnd(24));
            Assert.Equal(string.Empty, directive.Extension);
        }

        [Fact]
        public static void EncodingReturnsValueOfEncodingAttribute()
        {
            var directive = new OutputDirective(
                new DirectiveBlockStart(0),
                new DirectiveName(4, "output"),
                new[] { new Attribute(new AttributeName(14, "encoding"), new Equals(18), new DoubleQuote(19), new AttributeValue(20, "utf-8"), new DoubleQuote(22)) },
                new BlockEnd(24));
            Assert.Equal("utf-8", directive.Encoding);
        }

        [Fact]
        public static void EncodingReturnsEmptyStringWhenEncodingAttributeIsNotSpecified()
        {
            var directive = new OutputDirective(new DirectiveBlockStart(0), new DirectiveName(4, "output"), new Attribute[0], new BlockEnd(24));
            Assert.Equal(string.Empty, directive.Encoding);
        }

        [Fact]
        public static void EncodingProvidesMetadataAboutKnownValues()
        {
            var attributeDescriptor = new AttributeDescriptor(TypeDescriptor.GetProperties(typeof(OutputDirective))["Encoding"]);
            Assert.NotEqual(0, attributeDescriptor.Values.Count);
            ValueDescriptor valueDescriptor = attributeDescriptor.Values["utf-16"];
            Assert.Equal("Unicode", valueDescriptor.Description);
        }

        [Fact]
        public static void GetDescriptionReturnsDescriptionOfOutputDirective()
        {
            var directive = new OutputDirective(new DirectiveBlockStart(0), new DirectiveName(4, "output"), new Attribute[0], new BlockEnd(24));
            string description;
            Span applicableTo;
            Assert.True(directive.TryGetDescription(4, out description, out applicableTo));
            Assert.Contains("extension", description, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("encoding", description, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public static void GetDescriptionReturnsDescriptionOfEncodingAttribute()
        {
            var directive = new OutputDirective(
                new DirectiveBlockStart(0),
                new DirectiveName(4, "output"),
                new[] { new Attribute(new AttributeName(14, "encoding"), new Equals(18), new DoubleQuote(19), new AttributeValue(20, "utf-8"), new DoubleQuote(22)) },
                new BlockEnd(24));
            string description;
            Span applicableTo;
            Assert.True(directive.TryGetDescription(14, out description, out applicableTo));
            Assert.Contains("Encoding of the output file", description, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public static void GetDescriptionReturnsDescriptionOfExtensionAttribute()
        {
            var directive = new OutputDirective(
                new DirectiveBlockStart(0),
                new DirectiveName(4, "output"),
                new[] { new Attribute(new AttributeName(14, "extension"), new Equals(18), new DoubleQuote(19), new AttributeValue(20, ".txt"), new DoubleQuote(22)) },
                new BlockEnd(24));
            string description;
            Span applicableTo;
            Assert.True(directive.TryGetDescription(14, out description, out applicableTo));
            Assert.Contains("Extension of the output file", description, StringComparison.OrdinalIgnoreCase);
        }
        
        [Fact]
        public static void ValidateReturnsErrorWhenExtensionAttributeIsNotSpecified()
        {
            var directive = new OutputDirective(
                new DirectiveBlockStart(0),
                new DirectiveName(4, "output"),
                new[] { new Attribute(new AttributeName(14, "encoding"), new Equals(18), new DoubleQuote(19), new AttributeValue(20, "utf-8"), new DoubleQuote(22)) },
                new BlockEnd(24));
            TemplateError error = directive.Validate().Single();
            Assert.Contains("Extension", error.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public static void ValidateDoesNotReturnErrorWhenEncodingAttributeIsNotSpecified()
        {
            var directive = new OutputDirective(
                new DirectiveBlockStart(0),
                new DirectiveName(4, "output"),
                new[] { new Attribute(new AttributeName(14, "extension"), new Equals(18), new DoubleQuote(19), new AttributeValue(20, ".ext"), new DoubleQuote(22)) },
                new BlockEnd(24));
            Assert.False(directive.Validate().Any());
       }
    }
}