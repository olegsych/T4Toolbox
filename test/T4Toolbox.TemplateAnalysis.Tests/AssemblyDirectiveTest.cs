// <copyright file="AssemblyDirectiveTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.TemplateAnalysis
{
    using System;
    using System.Linq;
    using Microsoft.VisualStudio.Text;
    using NSubstitute;
    using Xunit;

    public static class AssemblyDirectiveTest
    {
        [Fact]
        public static void AssemblyDirectiveIsSubclassOfDirective()
        {
            Assert.True(typeof(AssemblyDirective).IsSubclassOf(typeof(Directive)));
        }

        [Fact]
        public static void AssemblyDirectiveIsSealed()
        {
            Assert.True(typeof(AssemblyDirective).IsSealed);
        }

        [Fact]
        public static void AssemblyNameReturnsValueOfNameAttribute()
        {
            var directive = new AssemblyDirective(
                new DirectiveBlockStart(0),
                new DirectiveName(4, "assembly"),
                new[] { new Attribute(new AttributeName(13, "name"), new Equals(17), new DoubleQuote(18), new AttributeValue(19, "42"), new DoubleQuote(21)) },
                new BlockEnd(23));
            Assert.Equal("42", directive.AssemblyName);
        }

        [Fact]
        public static void AssemblyNameReturnsEmptyStringWhenNameAttributeIsNotSpecified()
        {
            var directive = new AssemblyDirective(new DirectiveBlockStart(0), new DirectiveName(4, "assembly"), new Attribute[0], new BlockEnd(23));
            Assert.Equal(string.Empty, directive.AssemblyName);            
        }

        [Fact]
        public static void AcceptCallsVisitAssemblyDirectiveMethodOfSyntaxNodeVisitor()
        {
            var visitor = Substitute.For<SyntaxNodeVisitor>();
            var directive = new AssemblyDirective(new DirectiveBlockStart(0), new DirectiveName(0, "assembly"), new Attribute[0], new BlockEnd(0));
            directive.Accept(visitor);
            visitor.Received().VisitAssemblyDirective(directive);
        }

        [Fact]
        public static void GetDescriptionReturnsDescriptionOfTheDirective()
        {
            var directive = new AssemblyDirective(new DirectiveBlockStart(0), new DirectiveName(4, "assembly"), new Attribute[0], new BlockEnd(23));
            string description;
            Span applicableTo;
            Assert.True(directive.TryGetDescription(4, out description, out applicableTo));
            Assert.StartsWith("Loads an assembly", description, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public static void GetDescriptionReturnsDescriptionOfTheNameAttribute()
        {
            var directive = new AssemblyDirective(
                new DirectiveBlockStart(0),
                new DirectiveName(4, "assembly"),
                new[] { new Attribute(new AttributeName(13, "name"), new Equals(17), new DoubleQuote(18), new AttributeValue(19, "42"), new DoubleQuote(21)) },
                new BlockEnd(23));
            string description;
            Span applicableTo;
            Assert.True(directive.TryGetDescription(13, out description, out applicableTo));
            Assert.StartsWith("Name of an assembly", description, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public static void ValidateReturnsErrorWhenNameAttributeIsNotSpecified()
        {
            var directive = new AssemblyDirective(new DirectiveBlockStart(0), new DirectiveName(4, "assembly"), new Attribute[0], new BlockEnd(23));
            TemplateError error = directive.Validate().Single();
            Assert.Contains("Name", error.Message, StringComparison.OrdinalIgnoreCase);
        }
    }
}