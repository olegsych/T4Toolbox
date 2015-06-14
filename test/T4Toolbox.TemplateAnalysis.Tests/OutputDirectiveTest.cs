// <copyright file="OutputDirectiveTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.TemplateAnalysis
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.VisualStudio.Text;
    using NSubstitute;

    [TestClass]
    public class OutputDirectiveTest
    {
        [TestMethod]
        public void OutputDirectiveIsSubclassOfDirective()
        {
            Assert.IsTrue(typeof(OutputDirective).IsSubclassOf(typeof(Directive)));
        }

        [TestMethod]
        public void OutputDirectiveIsSealed()
        {
            Assert.IsTrue(typeof(OutputDirective).IsSealed);
        }

        [TestMethod]
        public void AcceptCallsVisitOutputDirectiveMethodOfSyntaxNodeVisitor()
        {
            var visitor = Substitute.For<SyntaxNodeVisitor>();
            var directive = new OutputDirective(new DirectiveBlockStart(0), new DirectiveName(4, "output"), new Attribute[0], new BlockEnd(24));
            directive.Accept(visitor);
            visitor.Received().VisitOutputDirective(directive);
        }

        [TestMethod]
        public void ExtensionReturnsValueOfExtensionAttribute()
        {
            var directive = new OutputDirective(
                new DirectiveBlockStart(0),
                new DirectiveName(4, "output"),
                new[] { new Attribute(new AttributeName(14, "extension"), new Equals(18), new DoubleQuote(19), new AttributeValue(20, ".txt"), new DoubleQuote(22)) },
                new BlockEnd(24));
            Assert.AreEqual(".txt", directive.Extension);
        }

        [TestMethod]
        public void ExtensionReturnsEmptyStringWhenExtensionAttributeIsNotSpecified()
        {
            var directive = new OutputDirective(new DirectiveBlockStart(0), new DirectiveName(4, "output"), new Attribute[0], new BlockEnd(24));
            Assert.AreEqual(string.Empty, directive.Extension);
        }

        [TestMethod]
        public void EncodingReturnsValueOfEncodingAttribute()
        {
            var directive = new OutputDirective(
                new DirectiveBlockStart(0),
                new DirectiveName(4, "output"),
                new[] { new Attribute(new AttributeName(14, "encoding"), new Equals(18), new DoubleQuote(19), new AttributeValue(20, "utf-8"), new DoubleQuote(22)) },
                new BlockEnd(24));
            Assert.AreEqual("utf-8", directive.Encoding);
        }

        [TestMethod]
        public void EncodingReturnsEmptyStringWhenEncodingAttributeIsNotSpecified()
        {
            var directive = new OutputDirective(new DirectiveBlockStart(0), new DirectiveName(4, "output"), new Attribute[0], new BlockEnd(24));
            Assert.AreEqual(string.Empty, directive.Encoding);
        }

        [TestMethod]
        public void EncodingProvidesMetadataAboutKnownValues()
        {
            var attributeDescriptor = new AttributeDescriptor(TypeDescriptor.GetProperties(typeof(OutputDirective))["Encoding"]);
            Assert.AreNotEqual(0, attributeDescriptor.Values.Count);
            ValueDescriptor valueDescriptor = attributeDescriptor.Values["utf-16"];
            Assert.AreEqual("Unicode", valueDescriptor.Description);
        }

        [TestMethod]
        public void GetDescriptionReturnsDescriptionOfOutputDirective()
        {
            var directive = new OutputDirective(new DirectiveBlockStart(0), new DirectiveName(4, "output"), new Attribute[0], new BlockEnd(24));
            string description;
            Span applicableTo;
            Assert.IsTrue(directive.TryGetDescription(4, out description, out applicableTo));
            StringAssert.Contains(description, "extension");
            StringAssert.Contains(description, "encoding");
        }

        [TestMethod]
        public void GetDescriptionReturnsDescriptionOfEncodingAttribute()
        {
            var directive = new OutputDirective(
                new DirectiveBlockStart(0),
                new DirectiveName(4, "output"),
                new[] { new Attribute(new AttributeName(14, "encoding"), new Equals(18), new DoubleQuote(19), new AttributeValue(20, "utf-8"), new DoubleQuote(22)) },
                new BlockEnd(24));
            string description;
            Span applicableTo;
            Assert.IsTrue(directive.TryGetDescription(14, out description, out applicableTo));
            StringAssert.Contains(description, "Encoding of the output file");
        }

        [TestMethod]
        public void GetDescriptionReturnsDescriptionOfExtensionAttribute()
        {
            var directive = new OutputDirective(
                new DirectiveBlockStart(0),
                new DirectiveName(4, "output"),
                new[] { new Attribute(new AttributeName(14, "extension"), new Equals(18), new DoubleQuote(19), new AttributeValue(20, ".txt"), new DoubleQuote(22)) },
                new BlockEnd(24));
            string description;
            Span applicableTo;
            Assert.IsTrue(directive.TryGetDescription(14, out description, out applicableTo));
            StringAssert.Contains(description, "Extension of the output file");
        }
        
        [TestMethod]
        public void ValidateReturnsErrorWhenExtensionAttributeIsNotSpecified()
        {
            var directive = new OutputDirective(
                new DirectiveBlockStart(0),
                new DirectiveName(4, "output"),
                new[] { new Attribute(new AttributeName(14, "encoding"), new Equals(18), new DoubleQuote(19), new AttributeValue(20, "utf-8"), new DoubleQuote(22)) },
                new BlockEnd(24));
            TemplateError error = directive.Validate().Single();
            StringAssert.Contains(error.Message, "Extension");
        }

        [TestMethod]
        public void ValidateDoesNotReturnErrorWhenEncodingAttributeIsNotSpecified()
        {
            var directive = new OutputDirective(
                new DirectiveBlockStart(0),
                new DirectiveName(4, "output"),
                new[] { new Attribute(new AttributeName(14, "extension"), new Equals(18), new DoubleQuote(19), new AttributeValue(20, ".ext"), new DoubleQuote(22)) },
                new BlockEnd(24));
            Assert.IsFalse(directive.Validate().Any());
       }
    }
}