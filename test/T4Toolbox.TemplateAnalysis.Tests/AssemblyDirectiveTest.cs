// <copyright file="AssemblyDirectiveTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.TemplateAnalysis
{
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.VisualStudio.Text;
    using NSubstitute;

    [TestClass]
    public class AssemblyDirectiveTest
    {
        [TestMethod]
        public void AssemblyDirectiveIsSubclassOfDirective()
        {
            Assert.IsTrue(typeof(AssemblyDirective).IsSubclassOf(typeof(Directive)));
        }

        [TestMethod]
        public void AssemblyDirectiveIsSealed()
        {
            Assert.IsTrue(typeof(AssemblyDirective).IsSealed);
        }

        [TestMethod]
        public void AssemblyNameReturnsValueOfNameAttribute()
        {
            var directive = new AssemblyDirective(
                new DirectiveBlockStart(0),
                new DirectiveName(4, "assembly"),
                new[] { new Attribute(new AttributeName(13, "name"), new Equals(17), new DoubleQuote(18), new AttributeValue(19, "42"), new DoubleQuote(21)) },
                new BlockEnd(23));
            Assert.AreEqual("42", directive.AssemblyName);
        }

        [TestMethod]
        public void AssemblyNameReturnsEmptyStringWhenNameAttributeIsNotSpecified()
        {
            var directive = new AssemblyDirective(new DirectiveBlockStart(0), new DirectiveName(4, "assembly"), new Attribute[0], new BlockEnd(23));
            Assert.AreEqual(string.Empty, directive.AssemblyName);            
        }

        [TestMethod]
        public void AcceptCallsVisitAssemblyDirectiveMethodOfSyntaxNodeVisitor()
        {
            var visitor = Substitute.For<SyntaxNodeVisitor>();
            var directive = new AssemblyDirective(new DirectiveBlockStart(0), new DirectiveName(0, "assembly"), new Attribute[0], new BlockEnd(0));
            directive.Accept(visitor);
            visitor.Received().VisitAssemblyDirective(directive);
        }

        [TestMethod]
        public void GetDescriptionReturnsDescriptionOfTheDirective()
        {
            var directive = new AssemblyDirective(new DirectiveBlockStart(0), new DirectiveName(4, "assembly"), new Attribute[0], new BlockEnd(23));
            string description;
            Span applicableTo;
            Assert.IsTrue(directive.TryGetDescription(4, out description, out applicableTo));
            StringAssert.StartsWith(description, "Loads an assembly");
        }

        [TestMethod]
        public void GetDescriptionReturnsDescriptionOfTheNameAttribute()
        {
            var directive = new AssemblyDirective(
                new DirectiveBlockStart(0),
                new DirectiveName(4, "assembly"),
                new[] { new Attribute(new AttributeName(13, "name"), new Equals(17), new DoubleQuote(18), new AttributeValue(19, "42"), new DoubleQuote(21)) },
                new BlockEnd(23));
            string description;
            Span applicableTo;
            Assert.IsTrue(directive.TryGetDescription(13, out description, out applicableTo));
            StringAssert.StartsWith(description, "Name of an assembly");
        }

        [TestMethod]
        public void ValidateReturnsErrorWhenNameAttributeIsNotSpecified()
        {
            var directive = new AssemblyDirective(new DirectiveBlockStart(0), new DirectiveName(4, "assembly"), new Attribute[0], new BlockEnd(23));
            TemplateError error = directive.Validate().Single();
            StringAssert.Contains(error.Message, "Name");
        }
    }
}