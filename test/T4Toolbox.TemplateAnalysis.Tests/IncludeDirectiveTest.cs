// <copyright file="IncludeDirectiveTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.TemplateAnalysis
{
    using System;
    using System.Linq;
    using Microsoft.VisualStudio.Text;
    using NSubstitute;
    using Xunit;

    public static class IncludeDirectiveTest
    {
        [Fact]
        public static void IncludeDirectiveIsSubclassOfDirective()
        {
            Assert.True(typeof(IncludeDirective).IsSubclassOf(typeof(Directive)));
        }

        [Fact]
        public static void IncludeDirectiveIsSealed()
        {
            Assert.True(typeof(IncludeDirective).IsSealed);
        }

        [Fact]
        public static void AcceptCallsVisitIncludeDirectiveMethodOfSyntaxNodeVisitor()
        {
            var visitor = Substitute.For<SyntaxNodeVisitor>();
            var directive = new IncludeDirective(new DirectiveBlockStart(0), new DirectiveName(4, "include"), new Attribute[0], new BlockEnd(12));
            directive.Accept(visitor);
            visitor.Received().VisitIncludeDirective(directive);
        }

        [Fact]
        public static void FileReturnsValueOfFileAttribute()
        {
            var directive = new IncludeDirective(
                new DirectiveBlockStart(0),
                new DirectiveName(4, "include"),
                new[] { new Attribute(new AttributeName(12, "file"), new Equals(16), new DoubleQuote(17), new AttributeValue(18, "template.tt"), new DoubleQuote(29)) },
                new BlockEnd(30));
            Assert.Equal("template.tt", directive.File);
        }

        [Fact]
        public static void FileReturnsEmptyStringWhenFileAttributeIsNotSpecified()
        {
            var directive = new IncludeDirective(new DirectiveBlockStart(0), new DirectiveName(4, "include"), new Attribute[0], new BlockEnd(12));
            Assert.Equal(string.Empty, directive.File);
        }

        [Fact]
        public static void GetDescriptionReturnsDescriptionOfDirective()
        {
            var directive = new IncludeDirective(new DirectiveBlockStart(0), new DirectiveName(4, "include"), new Attribute[0], new BlockEnd(12));
            string description;
            Span applicableTo;
            Assert.True(directive.TryGetDescription(4, out description, out applicableTo));            
            Assert.Contains("text from another file", description, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public static void GetDescriptionReturnsDescriptionOfNameAttribute()
        {
            var directive = new IncludeDirective(
                new DirectiveBlockStart(0),
                new DirectiveName(4, "include"),
                new[] { new Attribute(new AttributeName(12, "file"), new Equals(16), new DoubleQuote(17), new AttributeValue(18, "template.tt"), new DoubleQuote(29)) },
                new BlockEnd(30));
            string description;
            Span applicableTo;
            Assert.True(directive.TryGetDescription(12, out description, out applicableTo));
            Assert.Contains("path to the included file", description, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public static void ValidateReturnsErrorWhenFileAttributeIsNotSpecified()
        {
            var directive = new IncludeDirective(new DirectiveBlockStart(0), new DirectiveName(4, "include"), new Attribute[0], new BlockEnd(12));
            TemplateError error = directive.Validate().Single();
            Assert.Contains("File", error.Message, StringComparison.OrdinalIgnoreCase);
        }
    }
}