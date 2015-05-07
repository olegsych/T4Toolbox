// <copyright file="TemplateDirectiveTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.TemplateAnalysis
{
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.VisualStudio.Text;
    using NSubstitute;

    [TestClass]
    public class TemplateDirectiveTest
    {
        [TestMethod]
        public void TemplateDirectiveIsSubclassOfDirective()
        {
            Assert.IsTrue(typeof(TemplateDirective).IsSubclassOf(typeof(Directive)));
        }

        [TestMethod]
        public void TemplateDirectiveIsSealed()
        {
            Assert.IsTrue(typeof(TemplateDirective).IsSealed);
        }

        [TestMethod]
        public void AcceptCallsVisitTemplateDirectiveMethodOfSyntaxNodeVisitor()
        {
            var visitor = Substitute.For<SyntaxNodeVisitor>();
            var directive = new TemplateDirective(new DirectiveBlockStart(0), new DirectiveName(4, "template"), new Attribute[0], new BlockEnd(24));
            directive.Accept(visitor);
            visitor.Received().VisitTemplateDirective(directive);
        }

        [TestMethod]
        public void CompilerOptionsReturnsValueOfCompilerOptionsAttribute()
        {
            var directive = new TemplateDirective(
                new DirectiveBlockStart(0),
                new DirectiveName(4, "template"),
                new[] { new Attribute(new AttributeName(14, "compilerOptions"), new Equals(18), new DoubleQuote(19), new AttributeValue(20, "optimizer+"), new DoubleQuote(22)) },
                new BlockEnd(24));
            Assert.AreEqual("optimizer+", directive.CompilerOptions);
        }

        [TestMethod]
        public void CompilerOptionsReturnsEmptyStringWhenCompilerOptionsAttributeIsNotSpecified()
        {
            var directive = new TemplateDirective(new DirectiveBlockStart(0), new DirectiveName(4, "template"), new Attribute[0], new BlockEnd(24));
            Assert.AreEqual(string.Empty, directive.CompilerOptions);
        }

        [TestMethod]
        public void CultureReturnsValueOfCultureAttribute()
        {
            var directive = new TemplateDirective(
                new DirectiveBlockStart(0),
                new DirectiveName(4, "template"),
                new[] { new Attribute(new AttributeName(14, "culture"), new Equals(18), new DoubleQuote(19), new AttributeValue(20, "en-US"), new DoubleQuote(22)) },
                new BlockEnd(24));
            Assert.AreEqual("en-US", directive.Culture);
        }

        [TestMethod]
        public void CultureReturnsEmptyStringWhenCultureAttributeIsNotSpecified()
        {
            var directive = new TemplateDirective(new DirectiveBlockStart(0), new DirectiveName(4, "template"), new Attribute[0], new BlockEnd(24));
            Assert.AreEqual(string.Empty, directive.Culture);
        }

        [TestMethod]
        public void CultureProvidesMetadataAboutWellKnownValues()
        {
            var attributeDescriptor = new AttributeDescriptor(TypeDescriptor.GetProperties(typeof(TemplateDirective))["Culture"]);
            Assert.AreNotEqual(0, attributeDescriptor.Values.Count);
            ValueDescriptor valueDescriptor = attributeDescriptor.Values["en-US"];
            Assert.AreEqual("English (United States)", valueDescriptor.Description);
        }

        [TestMethod]
        public void DebugReturnsValueOfDebugAttribute()
        {
            var directive = new TemplateDirective(
                new DirectiveBlockStart(0),
                new DirectiveName(4, "template"),
                new[] { new Attribute(new AttributeName(14, "debug"), new Equals(18), new DoubleQuote(19), new AttributeValue(20, "true"), new DoubleQuote(22)) },
                new BlockEnd(24));
            Assert.AreEqual("true", directive.Debug);            
        }

        [TestMethod]
        public void DebugReturnsEmptyStringWhenDebugAttributeIsNotSpecified()
        {
            var directive = new TemplateDirective(new DirectiveBlockStart(0), new DirectiveName(4, "template"), new Attribute[0], new BlockEnd(24));
            Assert.AreEqual(string.Empty, directive.Debug);
        }

        [TestMethod]
        public void DebugAttributeProvidesMetadataAboutWellKnownValues()
        {
            var attributeDescriptor = new AttributeDescriptor(TypeDescriptor.GetProperties(typeof(TemplateDirective))["Debug"]);
            Assert.AreEqual(2, attributeDescriptor.Values.Count);
            VerifyAttributeValueDescriptor(attributeDescriptor, "false");
            VerifyAttributeValueDescriptor(attributeDescriptor, "true");
        }

        #region GetDescription

        [TestMethod]
        public void GetDescriptionReturnsDescriptionOfDirective()
        {
            var directive = new TemplateDirective(new DirectiveBlockStart(0), new DirectiveName(4, "template"), new Attribute[0], new BlockEnd(24));
            string description;
            Span applicableTo;
            Assert.IsTrue(directive.TryGetDescription(4, out description, out applicableTo));
            StringAssert.Contains(description, "Specifies how the template should be processed");
        }

        [TestMethod]
        public void GetDescriptionReturnsDescriptionOfCompilerOptionsAttribute()
        {
            var directive = new TemplateDirective(
                new DirectiveBlockStart(0),
                new DirectiveName(4, "template"),
                new[] { new Attribute(new AttributeName(14, "compilerOptions"), new Equals(18), new DoubleQuote(19), new AttributeValue(20, "optimizer+"), new DoubleQuote(22)) },
                new BlockEnd(24));
            string description;
            Span applicableTo;
            Assert.IsTrue(directive.TryGetDescription(14, out description, out applicableTo));
            StringAssert.Contains(description, "compiler options");
        }

        [TestMethod]
        public void GetDescriptionReturnsDescriptionOfCultureAttribute()
        {
            var directive = new TemplateDirective(
                new DirectiveBlockStart(0),
                new DirectiveName(4, "template"),
                new[] { new Attribute(new AttributeName(14, "culture"), new Equals(18), new DoubleQuote(19), new AttributeValue(20, "en-US"), new DoubleQuote(22)) },
                new BlockEnd(24));
            string description;
            Span applicableTo;
            Assert.IsTrue(directive.TryGetDescription(14, out description, out applicableTo));
            StringAssert.Contains(description, "Culture");
        }

        [TestMethod]
        public void GetDescriptionReturnsDescriptionOfDebugAttribute()
        {
            var directive = new TemplateDirective(
                new DirectiveBlockStart(0),
                new DirectiveName(4, "template"),
                new[] { new Attribute(new AttributeName(14, "debug"), new Equals(18), new DoubleQuote(19), new AttributeValue(20, "true"), new DoubleQuote(22)) },
                new BlockEnd(24));
            string description;
            Span applicableTo;
            Assert.IsTrue(directive.TryGetDescription(14, out description, out applicableTo));
            StringAssert.Contains(description, "debugging");
        }

        [TestMethod]
        public void GetDescriptionReturnsDescriptionOfHostSpecificAttribute()
        {
            var directive = new TemplateDirective(
                new DirectiveBlockStart(0),
                new DirectiveName(4, "template"),
                new[] { new Attribute(new AttributeName(14, "hostspecific"), new Equals(18), new DoubleQuote(19), new AttributeValue(20, "true"), new DoubleQuote(22)) },
                new BlockEnd(24));
            string description;
            Span applicableTo;
            Assert.IsTrue(directive.TryGetDescription(14, out description, out applicableTo));
            StringAssert.Contains(description, "host");   
        }

        [TestMethod]
        public void GetDescriptionReturnsDescriptionOfInheritsAttribute()
        {
            var directive = new TemplateDirective(
                new DirectiveBlockStart(0),
                new DirectiveName(4, "template"),
                new[] { new Attribute(new AttributeName(14, "inherits"), new Equals(18), new DoubleQuote(19), new AttributeValue(20, "TextTransformation"), new DoubleQuote(22)) },
                new BlockEnd(24));
            string description;
            Span applicableTo;
            Assert.IsTrue(directive.TryGetDescription(14, out description, out applicableTo));
            StringAssert.Contains(description, "base class");
        }

        [TestMethod]
        public void GetDescriptionReturnsDescriptionOfLanguageAttribute()
        {
            var directive = new TemplateDirective(
                new DirectiveBlockStart(0),
                new DirectiveName(4, "template"),
                new[] { new Attribute(new AttributeName(13, "language"), new Equals(21), new DoubleQuote(22), new AttributeValue(23, "C#"), new DoubleQuote(22)) },
                new BlockEnd(24));
            string description;
            Span applicableTo;
            Assert.IsTrue(directive.TryGetDescription(13, out description, out applicableTo));
            StringAssert.Contains(description, "language");
        }

        [TestMethod]
        public void GetDescriptionReturnsDescriptionOfLinePragmasAttribute()
        {
            var directive = new TemplateDirective(
                new DirectiveBlockStart(0),
                new DirectiveName(4, "template"),
                new[] { new Attribute(new AttributeName(13, "linePragmas"), new Equals(21), new DoubleQuote(22), new AttributeValue(23, "true"), new DoubleQuote(22)) },
                new BlockEnd(24));
            string description;
            Span applicableTo;
            Assert.IsTrue(directive.TryGetDescription(13, out description, out applicableTo));
            StringAssert.Contains(description, "line numbers");
        }

        [TestMethod]
        public void GetDescriptionReturnsDescriptionOfVisibilityAttribute()
        {
            var directive = new TemplateDirective(
                new DirectiveBlockStart(0),
                new DirectiveName(4, "template"),
                new[] { new Attribute(new AttributeName(13, "visibility"), new Equals(21), new DoubleQuote(22), new AttributeValue(23, "internal"), new DoubleQuote(22)) },
                new BlockEnd(24));
            string description;
            Span applicableTo;
            Assert.IsTrue(directive.TryGetDescription(14, out description, out applicableTo));
            StringAssert.Contains(description, "visibility");
        }
        
        #endregion

        [TestMethod]
        public void InheritsAttributeReturnsValueOfInheritsAttribute()
        {
            var directive = new TemplateDirective(
                new DirectiveBlockStart(0),
                new DirectiveName(4, "template"),
                new[] { new Attribute(new AttributeName(14, "inherits"), new Equals(18), new DoubleQuote(19), new AttributeValue(20, "TextTransformation"), new DoubleQuote(22)) },
                new BlockEnd(24));
            Assert.AreEqual("TextTransformation", directive.Inherits);
        }

        [TestMethod]
        public void InheritsReturnsEmptyStringWhenInheritsAttributeIsNotSpecified()
        {
            var directive = new TemplateDirective(new DirectiveBlockStart(0), new DirectiveName(4, "template"), new Attribute[0], new BlockEnd(24));
            Assert.AreEqual(string.Empty, directive.Inherits);
        }

        [TestMethod]
        public void HostSpecificReturnsValueOfHostSpecificAttribute()
        {
            var directive = new TemplateDirective(
                new DirectiveBlockStart(0),
                new DirectiveName(4, "template"),
                new[] { new Attribute(new AttributeName(14, "hostspecific"), new Equals(18), new DoubleQuote(19), new AttributeValue(20, "true"), new DoubleQuote(22)) },
                new BlockEnd(24));
            Assert.AreEqual("true", directive.HostSpecific);
        }

        [TestMethod]
        public void HostSpecificReturnsEmptyStringWhenHostSpecificAttributeIsNotSpecified()
        {
            var directive = new TemplateDirective(new DirectiveBlockStart(0), new DirectiveName(4, "template"), new Attribute[0], new BlockEnd(24));
            Assert.AreEqual(string.Empty, directive.HostSpecific);
        }

        [TestMethod]
        public void HostSpecificProvidesMetadataAboutWellKnownValues()
        {
            var attributeDescriptor = new AttributeDescriptor(TypeDescriptor.GetProperties(typeof(TemplateDirective))["HostSpecific"]);
            Assert.AreEqual(3, attributeDescriptor.Values.Count);
            VerifyAttributeValueDescriptor(attributeDescriptor, "false");
            VerifyAttributeValueDescriptor(attributeDescriptor, "true");
            VerifyAttributeValueDescriptor(attributeDescriptor, "trueFromBase");
        }

        [TestMethod]
        public void LanguageReturnsValueOfLanguageAttribute()
        {
            var directive = new TemplateDirective(
                new DirectiveBlockStart(0),
                new DirectiveName(4, "template"),
                new[] { new Attribute(new AttributeName(13, "language"), new Equals(21), new DoubleQuote(22), new AttributeValue(23, "C#"), new DoubleQuote(22)) },
                new BlockEnd(24));
            Assert.AreEqual("C#", directive.Language);
        }

        [TestMethod]
        public void LanguageReturnsEmptyStringWhenLanguageAttributeIsNotSpecified()
        {
            var directive = new TemplateDirective(new DirectiveBlockStart(0), new DirectiveName(4, "template"), new Attribute[0], new BlockEnd(24));
            Assert.AreEqual(string.Empty, directive.Language);
        }

        [TestMethod]
        public void LanguageProvidesMetadataAboutWellKnownValues()
        {
            var attributeDescriptor = new AttributeDescriptor(TypeDescriptor.GetProperties(typeof(TemplateDirective))["Language"]);
            Assert.AreEqual(2, attributeDescriptor.Values.Count);
            VerifyAttributeValueDescriptor(attributeDescriptor, "C#");
            VerifyAttributeValueDescriptor(attributeDescriptor, "VB");
        }

        [TestMethod]
        public void LinePragmasReturnsValueOfLinePragmasAttribute()
        {
            var directive = new TemplateDirective(
                new DirectiveBlockStart(0),
                new DirectiveName(4, "template"),
                new[] { new Attribute(new AttributeName(13, "linePragmas"), new Equals(21), new DoubleQuote(22), new AttributeValue(23, "true"), new DoubleQuote(22)) },
                new BlockEnd(24));
            Assert.AreEqual("true", directive.LinePragmas);
        }

        [TestMethod]
        public void LinePragmasReturnsEmptyStringWhenLinePragmasAttributeIsNotSpecified()
        {
            var directive = new TemplateDirective(new DirectiveBlockStart(0), new DirectiveName(4, "template"), new Attribute[0], new BlockEnd(24));
            Assert.AreEqual(string.Empty, directive.LinePragmas);
        }

        [TestMethod]
        public void LinePragmasProvidesMetadataAboutWellKnownValues()
        {
            var attributeDescriptor = new AttributeDescriptor(TypeDescriptor.GetProperties(typeof(TemplateDirective))["LinePragmas"]);
            Assert.AreEqual(2, attributeDescriptor.Values.Count);
            VerifyAttributeValueDescriptor(attributeDescriptor, "false");
            VerifyAttributeValueDescriptor(attributeDescriptor, "true");
        }

        [TestMethod]
        public void VisibilityReturnsValueOfVisibilityAttribute()
        {
            var directive = new TemplateDirective(
                new DirectiveBlockStart(0),
                new DirectiveName(4, "template"),
                new[] { new Attribute(new AttributeName(13, "visibility"), new Equals(21), new DoubleQuote(22), new AttributeValue(23, "internal"), new DoubleQuote(22)) },
                new BlockEnd(24));
            Assert.AreEqual("internal", directive.Visibility);
        }

        [TestMethod]
        public void VisibilityReturnsEmptyStringWhenVisibilityAttributeIsNotSpecified()
        {
            var directive = new TemplateDirective(new DirectiveBlockStart(0), new DirectiveName(4, "template"), new Attribute[0], new BlockEnd(24));
            Assert.AreEqual(string.Empty, directive.Visibility);
        }

        [TestMethod]
        public void VisibilityProvidesMetadataAboutWellKnownValues()
        {
            var attributeDescriptor = new AttributeDescriptor(TypeDescriptor.GetProperties(typeof(TemplateDirective))["Visibility"]);
            Assert.AreEqual(2, attributeDescriptor.Values.Count);
            VerifyAttributeValueDescriptor(attributeDescriptor, "public");
            VerifyAttributeValueDescriptor(attributeDescriptor, "internal");
        }

        [TestMethod]
        public void ValidateDoesNotReturnErrorsWhenNoAttributesAreSpecified()
        {
            var directive = new TemplateDirective(new DirectiveBlockStart(0), new DirectiveName(4, "template"), new Attribute[0], new BlockEnd(24));
            Assert.IsFalse(directive.Validate().Any());
        }

        private static void VerifyAttributeValueDescriptor(AttributeDescriptor attribute, string valueName)
        {
            ValueDescriptor value = attribute.Values[valueName];
            Assert.IsFalse(string.IsNullOrWhiteSpace(value.Description), valueName + " attribute value does not have a description");
        }
    }
}