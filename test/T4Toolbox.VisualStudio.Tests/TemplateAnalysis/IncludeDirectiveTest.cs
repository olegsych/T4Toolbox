// <copyright file="IncludeDirectiveTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.TemplateAnalysis
{
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.VisualStudio.Text;
    using NSubstitute;

    [TestClass]
    public class IncludeDirectiveTest
    {
        [TestMethod]
        public void IncludeDirectiveIsSubclassOfDirective()
        {
            Assert.IsTrue(typeof(IncludeDirective).IsSubclassOf(typeof(Directive)));
        }

        [TestMethod]
        public void IncludeDirectiveIsSealed()
        {
            Assert.IsTrue(typeof(IncludeDirective).IsSealed);
        }

        [TestMethod]
        public void AcceptCallsVisitIncludeDirectiveMethodOfSyntaxNodeVisitor()
        {
            var visitor = Substitute.For<SyntaxNodeVisitor>();
            var directive = new IncludeDirective(new DirectiveBlockStart(0), new DirectiveName(4, "include"), new Attribute[0], new BlockEnd(12));
            directive.Accept(visitor);
            visitor.Received().VisitIncludeDirective(directive);
        }

        [TestMethod]
        public void FileReturnsValueOfFileAttribute()
        {
            var directive = new IncludeDirective(
                new DirectiveBlockStart(0),
                new DirectiveName(4, "include"),
                new[] { new Attribute(new AttributeName(12, "file"), new Equals(16), new DoubleQuote(17), new AttributeValue(18, "template.tt"), new DoubleQuote(29)) },
                new BlockEnd(30));
            Assert.AreEqual("template.tt", directive.File);
        }

        [TestMethod]
        public void FileReturnsEmptyStringWhenFileAttributeIsNotSpecified()
        {
            var directive = new IncludeDirective(new DirectiveBlockStart(0), new DirectiveName(4, "include"), new Attribute[0], new BlockEnd(12));
            Assert.AreEqual(string.Empty, directive.File);
        }

        [TestMethod]
        public void GetDescriptionReturnsDescriptionOfDirective()
        {
            var directive = new IncludeDirective(new DirectiveBlockStart(0), new DirectiveName(4, "include"), new Attribute[0], new BlockEnd(12));
            string description;
            Span applicableTo;
            Assert.IsTrue(directive.TryGetDescription(4, out description, out applicableTo));            
            StringAssert.Contains(description, "text from another file");
        }

        [TestMethod]
        public void GetDescriptionReturnsDescriptionOfNameAttribute()
        {
            var directive = new IncludeDirective(
                new DirectiveBlockStart(0),
                new DirectiveName(4, "include"),
                new[] { new Attribute(new AttributeName(12, "file"), new Equals(16), new DoubleQuote(17), new AttributeValue(18, "template.tt"), new DoubleQuote(29)) },
                new BlockEnd(30));
            string description;
            Span applicableTo;
            Assert.IsTrue(directive.TryGetDescription(12, out description, out applicableTo));
            StringAssert.Contains(description, "path to the included file");
        }

        [TestMethod]
        public void ValidateReturnsErrorWhenFileAttributeIsNotSpecified()
        {
            var directive = new IncludeDirective(new DirectiveBlockStart(0), new DirectiveName(4, "include"), new Attribute[0], new BlockEnd(12));
            TemplateError error = directive.Validate().Single();
            StringAssert.Contains(error.Message, "File");
        }
    }
}