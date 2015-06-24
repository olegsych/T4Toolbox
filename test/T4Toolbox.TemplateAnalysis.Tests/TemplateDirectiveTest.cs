// <copyright file="TemplateDirectiveTest.cs" company="Oleg Sych">
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

    public static class TemplateDirectiveTest
    {
        [Fact]
        public static void TemplateDirectiveIsSubclassOfDirective()
        {
            Assert.True(typeof(TemplateDirective).IsSubclassOf(typeof(Directive)));
        }

        [Fact]
        public static void TemplateDirectiveIsSealed()
        {
            Assert.True(typeof(TemplateDirective).IsSealed);
        }

        [Fact]
        public static void AcceptCallsVisitTemplateDirectiveMethodOfSyntaxNodeVisitor()
        {
            var visitor = Substitute.For<SyntaxNodeVisitor>();
            var directive = new TemplateDirective(new DirectiveBlockStart(0), new DirectiveName(4, "template"), new Attribute[0], new BlockEnd(24));
            directive.Accept(visitor);
            visitor.Received().VisitTemplateDirective(directive);
        }

        [Fact]
        public static void CompilerOptionsReturnsValueOfCompilerOptionsAttribute()
        {
            var directive = new TemplateDirective(
                new DirectiveBlockStart(0),
                new DirectiveName(4, "template"),
                new[] { new Attribute(new AttributeName(14, "compilerOptions"), new Equals(18), new DoubleQuote(19), new AttributeValue(20, "optimizer+"), new DoubleQuote(22)) },
                new BlockEnd(24));
            Assert.Equal("optimizer+", directive.CompilerOptions);
        }

        [Fact]
        public static void CompilerOptionsReturnsEmptyStringWhenCompilerOptionsAttributeIsNotSpecified()
        {
            var directive = new TemplateDirective(new DirectiveBlockStart(0), new DirectiveName(4, "template"), new Attribute[0], new BlockEnd(24));
            Assert.Equal(string.Empty, directive.CompilerOptions);
        }

        [Fact]
        public static void CultureReturnsValueOfCultureAttribute()
        {
            var directive = new TemplateDirective(
                new DirectiveBlockStart(0),
                new DirectiveName(4, "template"),
                new[] { new Attribute(new AttributeName(14, "culture"), new Equals(18), new DoubleQuote(19), new AttributeValue(20, "en-US"), new DoubleQuote(22)) },
                new BlockEnd(24));
            Assert.Equal("en-US", directive.Culture);
        }

        [Fact]
        public static void CultureReturnsEmptyStringWhenCultureAttributeIsNotSpecified()
        {
            var directive = new TemplateDirective(new DirectiveBlockStart(0), new DirectiveName(4, "template"), new Attribute[0], new BlockEnd(24));
            Assert.Equal(string.Empty, directive.Culture);
        }

        [Fact]
        public static void CultureProvidesMetadataAboutWellKnownValues()
        {
            var attributeDescriptor = new AttributeDescriptor(TypeDescriptor.GetProperties(typeof(TemplateDirective))["Culture"]);
            Assert.NotEqual(0, attributeDescriptor.Values.Count);
            ValueDescriptor valueDescriptor = attributeDescriptor.Values["en-US"];
            Assert.Equal("English (United States)", valueDescriptor.Description);
        }

        [Fact]
        public static void DebugReturnsValueOfDebugAttribute()
        {
            var directive = new TemplateDirective(
                new DirectiveBlockStart(0),
                new DirectiveName(4, "template"),
                new[] { new Attribute(new AttributeName(14, "debug"), new Equals(18), new DoubleQuote(19), new AttributeValue(20, "true"), new DoubleQuote(22)) },
                new BlockEnd(24));
            Assert.Equal("true", directive.Debug);            
        }

        [Fact]
        public static void DebugReturnsEmptyStringWhenDebugAttributeIsNotSpecified()
        {
            var directive = new TemplateDirective(new DirectiveBlockStart(0), new DirectiveName(4, "template"), new Attribute[0], new BlockEnd(24));
            Assert.Equal(string.Empty, directive.Debug);
        }

        [Fact]
        public static void DebugAttributeProvidesMetadataAboutWellKnownValues()
        {
            var attributeDescriptor = new AttributeDescriptor(TypeDescriptor.GetProperties(typeof(TemplateDirective))["Debug"]);
            Assert.Equal(2, attributeDescriptor.Values.Count);
            VerifyAttributeValueDescriptor(attributeDescriptor, "false");
            VerifyAttributeValueDescriptor(attributeDescriptor, "true");
        }

        #region GetDescription

        [Fact]
        public static void GetDescriptionReturnsDescriptionOfDirective()
        {
            var directive = new TemplateDirective(new DirectiveBlockStart(0), new DirectiveName(4, "template"), new Attribute[0], new BlockEnd(24));
            string description;
            Span applicableTo;
            Assert.True(directive.TryGetDescription(4, out description, out applicableTo));
            Assert.Contains("Specifies how the template should be processed", description, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public static void GetDescriptionReturnsDescriptionOfCompilerOptionsAttribute()
        {
            var directive = new TemplateDirective(
                new DirectiveBlockStart(0),
                new DirectiveName(4, "template"),
                new[] { new Attribute(new AttributeName(14, "compilerOptions"), new Equals(18), new DoubleQuote(19), new AttributeValue(20, "optimizer+"), new DoubleQuote(22)) },
                new BlockEnd(24));
            string description;
            Span applicableTo;
            Assert.True(directive.TryGetDescription(14, out description, out applicableTo));
            Assert.Contains("compiler options", description, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public static void GetDescriptionReturnsDescriptionOfCultureAttribute()
        {
            var directive = new TemplateDirective(
                new DirectiveBlockStart(0),
                new DirectiveName(4, "template"),
                new[] { new Attribute(new AttributeName(14, "culture"), new Equals(18), new DoubleQuote(19), new AttributeValue(20, "en-US"), new DoubleQuote(22)) },
                new BlockEnd(24));
            string description;
            Span applicableTo;
            Assert.True(directive.TryGetDescription(14, out description, out applicableTo));
            Assert.Contains("Culture", description, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public static void GetDescriptionReturnsDescriptionOfDebugAttribute()
        {
            var directive = new TemplateDirective(
                new DirectiveBlockStart(0),
                new DirectiveName(4, "template"),
                new[] { new Attribute(new AttributeName(14, "debug"), new Equals(18), new DoubleQuote(19), new AttributeValue(20, "true"), new DoubleQuote(22)) },
                new BlockEnd(24));
            string description;
            Span applicableTo;
            Assert.True(directive.TryGetDescription(14, out description, out applicableTo));
            Assert.Contains("debugging", description, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public static void GetDescriptionReturnsDescriptionOfHostSpecificAttribute()
        {
            var directive = new TemplateDirective(
                new DirectiveBlockStart(0),
                new DirectiveName(4, "template"),
                new[] { new Attribute(new AttributeName(14, "hostspecific"), new Equals(18), new DoubleQuote(19), new AttributeValue(20, "true"), new DoubleQuote(22)) },
                new BlockEnd(24));
            string description;
            Span applicableTo;
            Assert.True(directive.TryGetDescription(14, out description, out applicableTo));
            Assert.Contains("host", description, StringComparison.OrdinalIgnoreCase);   
        }

        [Fact]
        public static void GetDescriptionReturnsDescriptionOfInheritsAttribute()
        {
            var directive = new TemplateDirective(
                new DirectiveBlockStart(0),
                new DirectiveName(4, "template"),
                new[] { new Attribute(new AttributeName(14, "inherits"), new Equals(18), new DoubleQuote(19), new AttributeValue(20, "TextTransformation"), new DoubleQuote(22)) },
                new BlockEnd(24));
            string description;
            Span applicableTo;
            Assert.True(directive.TryGetDescription(14, out description, out applicableTo));
            Assert.Contains("base class", description, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public static void GetDescriptionReturnsDescriptionOfLanguageAttribute()
        {
            var directive = new TemplateDirective(
                new DirectiveBlockStart(0),
                new DirectiveName(4, "template"),
                new[] { new Attribute(new AttributeName(13, "language"), new Equals(21), new DoubleQuote(22), new AttributeValue(23, "C#"), new DoubleQuote(22)) },
                new BlockEnd(24));
            string description;
            Span applicableTo;
            Assert.True(directive.TryGetDescription(13, out description, out applicableTo));
            Assert.Contains("language", description, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public static void GetDescriptionReturnsDescriptionOfLinePragmasAttribute()
        {
            var directive = new TemplateDirective(
                new DirectiveBlockStart(0),
                new DirectiveName(4, "template"),
                new[] { new Attribute(new AttributeName(13, "linePragmas"), new Equals(21), new DoubleQuote(22), new AttributeValue(23, "true"), new DoubleQuote(22)) },
                new BlockEnd(24));
            string description;
            Span applicableTo;
            Assert.True(directive.TryGetDescription(13, out description, out applicableTo));
            Assert.Contains("line numbers", description, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public static void GetDescriptionReturnsDescriptionOfVisibilityAttribute()
        {
            var directive = new TemplateDirective(
                new DirectiveBlockStart(0),
                new DirectiveName(4, "template"),
                new[] { new Attribute(new AttributeName(13, "visibility"), new Equals(21), new DoubleQuote(22), new AttributeValue(23, "internal"), new DoubleQuote(22)) },
                new BlockEnd(24));
            string description;
            Span applicableTo;
            Assert.True(directive.TryGetDescription(14, out description, out applicableTo));
            Assert.Contains("visibility", description, StringComparison.OrdinalIgnoreCase);
        }
        
        #endregion

        [Fact]
        public static void InheritsAttributeReturnsValueOfInheritsAttribute()
        {
            var directive = new TemplateDirective(
                new DirectiveBlockStart(0),
                new DirectiveName(4, "template"),
                new[] { new Attribute(new AttributeName(14, "inherits"), new Equals(18), new DoubleQuote(19), new AttributeValue(20, "TextTransformation"), new DoubleQuote(22)) },
                new BlockEnd(24));
            Assert.Equal("TextTransformation", directive.Inherits);
        }

        [Fact]
        public static void InheritsReturnsEmptyStringWhenInheritsAttributeIsNotSpecified()
        {
            var directive = new TemplateDirective(new DirectiveBlockStart(0), new DirectiveName(4, "template"), new Attribute[0], new BlockEnd(24));
            Assert.Equal(string.Empty, directive.Inherits);
        }

        [Fact]
        public static void HostSpecificReturnsValueOfHostSpecificAttribute()
        {
            var directive = new TemplateDirective(
                new DirectiveBlockStart(0),
                new DirectiveName(4, "template"),
                new[] { new Attribute(new AttributeName(14, "hostspecific"), new Equals(18), new DoubleQuote(19), new AttributeValue(20, "true"), new DoubleQuote(22)) },
                new BlockEnd(24));
            Assert.Equal("true", directive.HostSpecific);
        }

        [Fact]
        public static void HostSpecificReturnsEmptyStringWhenHostSpecificAttributeIsNotSpecified()
        {
            var directive = new TemplateDirective(new DirectiveBlockStart(0), new DirectiveName(4, "template"), new Attribute[0], new BlockEnd(24));
            Assert.Equal(string.Empty, directive.HostSpecific);
        }

        [Fact]
        public static void HostSpecificProvidesMetadataAboutWellKnownValues()
        {
            var attributeDescriptor = new AttributeDescriptor(TypeDescriptor.GetProperties(typeof(TemplateDirective))["HostSpecific"]);
            Assert.Equal(3, attributeDescriptor.Values.Count);
            VerifyAttributeValueDescriptor(attributeDescriptor, "false");
            VerifyAttributeValueDescriptor(attributeDescriptor, "true");
            VerifyAttributeValueDescriptor(attributeDescriptor, "trueFromBase");
        }

        [Fact]
        public static void LanguageReturnsValueOfLanguageAttribute()
        {
            var directive = new TemplateDirective(
                new DirectiveBlockStart(0),
                new DirectiveName(4, "template"),
                new[] { new Attribute(new AttributeName(13, "language"), new Equals(21), new DoubleQuote(22), new AttributeValue(23, "C#"), new DoubleQuote(22)) },
                new BlockEnd(24));
            Assert.Equal("C#", directive.Language);
        }

        [Fact]
        public static void LanguageReturnsEmptyStringWhenLanguageAttributeIsNotSpecified()
        {
            var directive = new TemplateDirective(new DirectiveBlockStart(0), new DirectiveName(4, "template"), new Attribute[0], new BlockEnd(24));
            Assert.Equal(string.Empty, directive.Language);
        }

        [Fact]
        public static void LanguageProvidesMetadataAboutWellKnownValues()
        {
            var attributeDescriptor = new AttributeDescriptor(TypeDescriptor.GetProperties(typeof(TemplateDirective))["Language"]);
            Assert.Equal(2, attributeDescriptor.Values.Count);
            VerifyAttributeValueDescriptor(attributeDescriptor, "C#");
            VerifyAttributeValueDescriptor(attributeDescriptor, "VB");
        }

        [Fact]
        public static void LinePragmasReturnsValueOfLinePragmasAttribute()
        {
            var directive = new TemplateDirective(
                new DirectiveBlockStart(0),
                new DirectiveName(4, "template"),
                new[] { new Attribute(new AttributeName(13, "linePragmas"), new Equals(21), new DoubleQuote(22), new AttributeValue(23, "true"), new DoubleQuote(22)) },
                new BlockEnd(24));
            Assert.Equal("true", directive.LinePragmas);
        }

        [Fact]
        public static void LinePragmasReturnsEmptyStringWhenLinePragmasAttributeIsNotSpecified()
        {
            var directive = new TemplateDirective(new DirectiveBlockStart(0), new DirectiveName(4, "template"), new Attribute[0], new BlockEnd(24));
            Assert.Equal(string.Empty, directive.LinePragmas);
        }

        [Fact]
        public static void LinePragmasProvidesMetadataAboutWellKnownValues()
        {
            var attributeDescriptor = new AttributeDescriptor(TypeDescriptor.GetProperties(typeof(TemplateDirective))["LinePragmas"]);
            Assert.Equal(2, attributeDescriptor.Values.Count);
            VerifyAttributeValueDescriptor(attributeDescriptor, "false");
            VerifyAttributeValueDescriptor(attributeDescriptor, "true");
        }

        [Fact]
        public static void VisibilityReturnsValueOfVisibilityAttribute()
        {
            var directive = new TemplateDirective(
                new DirectiveBlockStart(0),
                new DirectiveName(4, "template"),
                new[] { new Attribute(new AttributeName(13, "visibility"), new Equals(21), new DoubleQuote(22), new AttributeValue(23, "internal"), new DoubleQuote(22)) },
                new BlockEnd(24));
            Assert.Equal("internal", directive.Visibility);
        }

        [Fact]
        public static void VisibilityReturnsEmptyStringWhenVisibilityAttributeIsNotSpecified()
        {
            var directive = new TemplateDirective(new DirectiveBlockStart(0), new DirectiveName(4, "template"), new Attribute[0], new BlockEnd(24));
            Assert.Equal(string.Empty, directive.Visibility);
        }

        [Fact]
        public static void VisibilityProvidesMetadataAboutWellKnownValues()
        {
            var attributeDescriptor = new AttributeDescriptor(TypeDescriptor.GetProperties(typeof(TemplateDirective))["Visibility"]);
            Assert.Equal(2, attributeDescriptor.Values.Count);
            VerifyAttributeValueDescriptor(attributeDescriptor, "public");
            VerifyAttributeValueDescriptor(attributeDescriptor, "internal");
        }

        [Fact]
        public static void ValidateDoesNotReturnErrorsWhenNoAttributesAreSpecified()
        {
            var directive = new TemplateDirective(new DirectiveBlockStart(0), new DirectiveName(4, "template"), new Attribute[0], new BlockEnd(24));
            Assert.False(directive.Validate().Any());
        }

        private static void VerifyAttributeValueDescriptor(AttributeDescriptor attribute, string valueName)
        {
            ValueDescriptor value = attribute.Values[valueName];
            Assert.False(string.IsNullOrWhiteSpace(value.Description), valueName + " attribute value does not have a description");
        }
    }
}