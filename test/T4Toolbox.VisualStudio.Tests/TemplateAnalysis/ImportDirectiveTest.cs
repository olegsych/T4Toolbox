// <copyright file="ImportDirectiveTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.TemplateAnalysis
{
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.VisualStudio.Text;
    using NSubstitute;

    [TestClass]
    public class ImportDirectiveTest
    {
        [TestMethod]
        public void ImportDirectiveIsSubclassOfDirective()
        {
            Assert.IsTrue(typeof(ImportDirective).IsSubclassOf(typeof(Directive)));
        }

        [TestMethod]
        public void ImportDirectiveIsSealed()
        {
            Assert.IsTrue(typeof(ImportDirective).IsSealed);
        }

        [TestMethod]
        public void AcceptCallsVisitImportDirectiveMethodOfSyntaxNodeVisitor()
        {
            var visitor = Substitute.For<SyntaxNodeVisitor>();
            var directive = new ImportDirective(new DirectiveBlockStart(0), new DirectiveName(4, "import"), new Attribute[0], new BlockEnd(18));
            directive.Accept(visitor);
            visitor.Received().VisitImportDirective(directive);
        }

        [TestMethod]
        public void GetDescriptionReturnsDescriptionOfDirective()
        {
            var directive = new ImportDirective(new DirectiveBlockStart(0), new DirectiveName(4, "import"), new Attribute[0], new BlockEnd(18));
            string description;
            Span applicableTo;
            Assert.IsTrue(directive.TryGetDescription(4, out description, out applicableTo));
            StringAssert.Contains(description, "imports");
            StringAssert.Contains(description, "using");
        }

        [TestMethod]
        public void GetDescriptionReturnsDescriptionOfNamespaceAttribute()
        {
            var directive = new ImportDirective(
                new DirectiveBlockStart(0),
                new DirectiveName(4, "import"),
                new[] { new Attribute(new AttributeName(11, "namespace"), new Equals(12), new DoubleQuote(13), new AttributeValue(14, "42"), new DoubleQuote(16)) },
                new BlockEnd(18));
            string description;
            Span applicableTo;
            Assert.IsTrue(directive.TryGetDescription(11, out description, out applicableTo));
            StringAssert.Contains(description, "fully-qualified name of the namespace being imported");
        }

        [TestMethod]
        public void NamespaceReturnsValueOfNamespaceAttribute()
        {
            var directive = new ImportDirective(
                new DirectiveBlockStart(0),
                new DirectiveName(4, "import"),
                new[] { new Attribute(new AttributeName(11, "namespace"), new Equals(12), new DoubleQuote(13), new AttributeValue(14, "42"), new DoubleQuote(16)) },
                new BlockEnd(18));
            Assert.AreEqual("42", directive.Namespace);
        }

        [TestMethod]
        public void NamespaceReturnsEmptyStringWhenNamespaceAttributeIsNotSpecified()
        {
            var directive = new ImportDirective(new DirectiveBlockStart(0), new DirectiveName(4, "import"), new Attribute[0], new BlockEnd(18));
            Assert.AreEqual(string.Empty, directive.Namespace);
        }

        [TestMethod]
        public void ValidateReturnsErrorWhenNamespaceAttributeIsNotSpecified()
        {
            var directive = new ImportDirective(new DirectiveBlockStart(0), new DirectiveName(4, "import"), new Attribute[0], new BlockEnd(18));
            TemplateError error = directive.Validate().Single();
            StringAssert.Contains(error.Message, "Namespace");
        }
    }
}