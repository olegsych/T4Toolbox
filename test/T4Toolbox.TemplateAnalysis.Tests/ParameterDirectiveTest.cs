// <copyright file="ParameterDirectiveTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.TemplateAnalysis
{
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.VisualStudio.Text;
    using NSubstitute;

    [TestClass]
    public class ParameterDirectiveTest
    {
        [TestMethod]
        public void ParameterDirectiveIsSubclassOfDirective()
        {
            Assert.IsTrue(typeof(ParameterDirective).IsSubclassOf(typeof(Directive)));
        }

        [TestMethod]
        public void ParameterDirectiveIsSealed()
        {
            Assert.IsTrue(typeof(ParameterDirective).IsSealed);
        }

        [TestMethod]
        public void AcceptCallsVisitParameterDirectiveMethodOfSyntaxNodeVisitor()
        {
            var visitor = Substitute.For<SyntaxNodeVisitor>();
            var directive = new ParameterDirective(new DirectiveBlockStart(0), new DirectiveName(4, "parameter"), new Attribute[0], new BlockEnd(24));
            directive.Accept(visitor);
            visitor.Received().VisitParameterDirective(directive);
        }

        [TestMethod]
        public void GetDescriptionReturnsDescriptionOfDirective()
        {
            var directive = new ParameterDirective(new DirectiveBlockStart(0), new DirectiveName(4, "parameter"), new Attribute[0], new BlockEnd(24));
            string description;
            Span applicableTo;
            Assert.IsTrue(directive.TryGetDescription(4, out description, out applicableTo));
            StringAssert.Contains(description, "property in template code");
        }

        [TestMethod]
        public void GetDescriptionReturnsDescriptionOfNameAttribute()
        {
            var directive = new ParameterDirective(
                new DirectiveBlockStart(0),
                new DirectiveName(4, "parameter"),
                new[] { new Attribute(new AttributeName(14, "name"), new Equals(18), new DoubleQuote(19), new AttributeValue(20, "42"), new DoubleQuote(22)) },
                new BlockEnd(24));
            string description;
            Span applicableTo;
            Assert.IsTrue(directive.TryGetDescription(14, out description, out applicableTo));
            StringAssert.Contains(description, "Name of the property");
        }

        [TestMethod]
        public void GetDescriptionReturnsDescriptionOfTypeAttribute()
        {
            var directive = new ParameterDirective(
                new DirectiveBlockStart(0),
                new DirectiveName(4, "parameter"),
                new[] { new Attribute(new AttributeName(14, "type"), new Equals(18), new DoubleQuote(19), new AttributeValue(20, "42"), new DoubleQuote(22)) },
                new BlockEnd(24));
            string description;
            Span applicableTo;
            Assert.IsTrue(directive.TryGetDescription(14, out description, out applicableTo));
            StringAssert.Contains(description, "Fully-qualified name of the property type");
        }

        [TestMethod]
        public void ParameterNameReturnsValueOfNameAttribute()
        {
            var directive = new ParameterDirective(
                new DirectiveBlockStart(0),
                new DirectiveName(4, "parameter"),
                new[] { new Attribute(new AttributeName(14, "name"), new Equals(18), new DoubleQuote(19), new AttributeValue(20, "42"), new DoubleQuote(22)) },
                new BlockEnd(24));
            Assert.AreEqual("42", directive.ParameterName);
        }

        [TestMethod]
        public void ParameterNameReturnsEmptyStringWhenNameAttributeIsNotSpecified()
        {
            var directive = new ParameterDirective(new DirectiveBlockStart(0), new DirectiveName(4, "parameter"), new Attribute[0], new BlockEnd(24));
            Assert.AreEqual(string.Empty, directive.ParameterName);            
        }

        [TestMethod]
        public void ParameterTypeReturnsValueOfTypeAttribute()
        {
            var directive = new ParameterDirective(
                new DirectiveBlockStart(0),
                new DirectiveName(4, "parameter"),
                new[] { new Attribute(new AttributeName(14, "type"), new Equals(18), new DoubleQuote(19), new AttributeValue(20, "42"), new DoubleQuote(22)) },
                new BlockEnd(24));
            Assert.AreEqual("42", directive.ParameterType);
        }

        [TestMethod]
        public void ParameterTypeReturnsEmptyStringWhenTypeAttributeIsNotSpecified()
        {
            var directive = new ParameterDirective(new DirectiveBlockStart(0), new DirectiveName(4, "parameter"), new Attribute[0], new BlockEnd(24));
            Assert.AreEqual(string.Empty, directive.ParameterType);
        }

        [TestMethod]
        public void ValidateReturnsErrorWhenNameAttributeIsNotSpecified()
        {
            var directive = new ParameterDirective(
                new DirectiveBlockStart(0),
                new DirectiveName(4, "parameter"),
                new[] { new Attribute(new AttributeName(14, "type"), new Equals(18), new DoubleQuote(19), new AttributeValue(20, "System.Int32"), new DoubleQuote(22)) },
                new BlockEnd(24));
            TemplateError error = directive.Validate().Single();
            StringAssert.Contains(error.Message, "Name");
        }

        [TestMethod]
        public void ValidateReturnsErrorWhenTypeAttributeIsNotSpecified()
        {
            var directive = new ParameterDirective(
                new DirectiveBlockStart(0),
                new DirectiveName(4, "parameter"),
                new[] { new Attribute(new AttributeName(14, "name"), new Equals(18), new DoubleQuote(19), new AttributeValue(20, "p1"), new DoubleQuote(22)) },
                new BlockEnd(24));
            TemplateError error = directive.Validate().Single();
            StringAssert.Contains(error.Message, "Type");
        }
    }
}