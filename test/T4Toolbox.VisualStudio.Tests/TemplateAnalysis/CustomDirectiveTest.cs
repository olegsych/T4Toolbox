// <copyright file="CustomDirectiveTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.TemplateAnalysis
{
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.VisualStudio.Text;
    using NSubstitute;

    [TestClass]
    public class CustomDirectiveTest
    {
        [TestMethod]
        public void CustomDirectiveIsSubclassOfDirective()
        {
            Assert.IsTrue(typeof(CustomDirective).IsSubclassOf(typeof(Directive)));
        }

        [TestMethod]
        public void CustomDirectiveIsSealed()
        {
            Assert.IsTrue(typeof(CustomDirective).IsSealed);
        }

        [TestMethod]
        public void AcceptCallsVisitCustomDirectiveMethodOfSyntaxNodeVisitor()
        {
            var visitor = Substitute.For<SyntaxNodeVisitor>();
            var directive = new CustomDirective(new DirectiveBlockStart(0), new DirectiveName(4, "custom"), new Attribute[0], new BlockEnd(24));
            directive.Accept(visitor);
            visitor.Received().VisitCustomDirective(directive);
        }

        [TestMethod]
        public void GetDescriptionReturnsDescriptionOfDirective()
        {
            var directive = new CustomDirective(new DirectiveBlockStart(0), new DirectiveName(4, "custom"), new Attribute[0], new BlockEnd(24));
            string description;
            Span applicableTo;
            Assert.IsTrue(directive.TryGetDescription(4, out description, out applicableTo));
            StringAssert.Contains(description, "directive");
        }

        [TestMethod]
        public void GetDescriptionReturnsDescriptionOfProcessorAttribute()
        {
            var directive = new CustomDirective(
                new DirectiveBlockStart(0),
                new DirectiveName(4, "custom"),
                new[] { new Attribute(new AttributeName(13, "processor"), new Equals(21), new DoubleQuote(22), new AttributeValue(23, "CustomProcessor"), new DoubleQuote(22)) },
                new BlockEnd(24));
            string description;
            Span applicableTo;
            Assert.IsTrue(directive.TryGetDescription(13, out description, out applicableTo));
            StringAssert.Contains(description, "processor");
        }

        [TestMethod]
        public void ProcessorReturnsValueOfProcessorAttribute()
        {
            var directive = new CustomDirective(
                new DirectiveBlockStart(0),
                new DirectiveName(4, "custom"),
                new[] { new Attribute(new AttributeName(13, "processor"), new Equals(21), new DoubleQuote(22), new AttributeValue(23, "CustomProcessor"), new DoubleQuote(22)) },
                new BlockEnd(24));
            Assert.AreEqual("CustomProcessor", directive.Processor);
        }

        [TestMethod]
        public void ProcessorReturnsEmptyStringWhenProcessorAttributeIsNotSpecified()
        {
            var directive = new CustomDirective(new DirectiveBlockStart(0), new DirectiveName(4, "custom"), new Attribute[0], new BlockEnd(24));
            Assert.AreEqual(string.Empty, directive.Processor);
        }

        [TestMethod]
        public void ValidateReturnsErrorWhenProcessorAttributeIsNotSpecified()
        {
            var directive = new CustomDirective(new DirectiveBlockStart(0), new DirectiveName(4, "custom"), new Attribute[0], new BlockEnd(24));
            TemplateError error = directive.Validate().Single();
            StringAssert.Contains(error.Message, "Processor");
        }

        [TestMethod]
        public void ValidateReturnsNoErrorsWhenDirectiveContainsUnrecognizedAttributes()
        {
            var a1 = new Attribute(new AttributeName(13, "processor"), new Equals(21), new DoubleQuote(22), new AttributeValue(23, "CustomProcessor"), new DoubleQuote(22));
            var a2 = new Attribute(new AttributeName(24, "custom"), new Equals(30), new DoubleQuote(31), new AttributeValue(32, "CustomValue"), new DoubleQuote(43));
            var directive = new CustomDirective(
                new DirectiveBlockStart(0), 
                new DirectiveName(4, "custom"),
                new[] { a1, a2 },
                new BlockEnd(45));
            Assert.IsFalse(directive.Validate().Any());
        }
    }
}