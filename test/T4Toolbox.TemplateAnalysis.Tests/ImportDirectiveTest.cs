// <copyright file="ImportDirectiveTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.TemplateAnalysis
{
    using System;
    using System.Linq;
    using Microsoft.VisualStudio.Text;
    using NSubstitute;
    using Xunit;

    public static class ImportDirectiveTest
    {
        [Fact]
        public static void ImportDirectiveIsSubclassOfDirective()
        {
            Assert.True(typeof(ImportDirective).IsSubclassOf(typeof(Directive)));
        }

        [Fact]
        public static void ImportDirectiveIsSealed()
        {
            Assert.True(typeof(ImportDirective).IsSealed);
        }

        [Fact]
        public static void AcceptCallsVisitImportDirectiveMethodOfSyntaxNodeVisitor()
        {
            var visitor = Substitute.For<SyntaxNodeVisitor>();
            var directive = new ImportDirective(new DirectiveBlockStart(0), new DirectiveName(4, "import"), new Attribute[0], new BlockEnd(18));
            directive.Accept(visitor);
            visitor.Received().VisitImportDirective(directive);
        }

        [Fact]
        public static void GetDescriptionReturnsDescriptionOfDirective()
        {
            var directive = new ImportDirective(new DirectiveBlockStart(0), new DirectiveName(4, "import"), new Attribute[0], new BlockEnd(18));
            string description;
            Span applicableTo;
            Assert.True(directive.TryGetDescription(4, out description, out applicableTo));
            Assert.Contains("imports", description, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("using", description, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public static void GetDescriptionReturnsDescriptionOfNamespaceAttribute()
        {
            var directive = new ImportDirective(
                new DirectiveBlockStart(0),
                new DirectiveName(4, "import"),
                new[] { new Attribute(new AttributeName(11, "namespace"), new Equals(12), new DoubleQuote(13), new AttributeValue(14, "42"), new DoubleQuote(16)) },
                new BlockEnd(18));
            string description;
            Span applicableTo;
            Assert.True(directive.TryGetDescription(11, out description, out applicableTo));
            Assert.Contains("fully-qualified name of the namespace being imported", description, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public static void NamespaceReturnsValueOfNamespaceAttribute()
        {
            var directive = new ImportDirective(
                new DirectiveBlockStart(0),
                new DirectiveName(4, "import"),
                new[] { new Attribute(new AttributeName(11, "namespace"), new Equals(12), new DoubleQuote(13), new AttributeValue(14, "42"), new DoubleQuote(16)) },
                new BlockEnd(18));
            Assert.Equal("42", directive.Namespace);
        }

        [Fact]
        public static void NamespaceReturnsEmptyStringWhenNamespaceAttributeIsNotSpecified()
        {
            var directive = new ImportDirective(new DirectiveBlockStart(0), new DirectiveName(4, "import"), new Attribute[0], new BlockEnd(18));
            Assert.Equal(string.Empty, directive.Namespace);
        }

        [Fact]
        public static void ValidateReturnsErrorWhenNamespaceAttributeIsNotSpecified()
        {
            var directive = new ImportDirective(new DirectiveBlockStart(0), new DirectiveName(4, "import"), new Attribute[0], new BlockEnd(18));
            TemplateError error = directive.Validate().Single();
            Assert.Contains("Namespace", error.Message, StringComparison.OrdinalIgnoreCase);
        }
    }
}