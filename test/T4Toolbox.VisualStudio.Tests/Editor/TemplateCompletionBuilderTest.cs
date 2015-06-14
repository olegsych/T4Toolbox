// <copyright file="TemplateCompletionBuilderTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.Editor
{
    using System;
    using System.Globalization;
    using System.Linq;
    using Microsoft.VisualStudio.Language.Intellisense;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using T4Toolbox.TemplateAnalysis;
    using Attribute = T4Toolbox.TemplateAnalysis.Attribute;

    [TestClass]
    public class TemplateCompletionBuilderTest
    {
        [TestMethod]
        public void TemplateCompletionBuilderIsInternalAndNotMeantForPublicConsumption()
        {
            Assert.IsFalse(typeof(TemplateCompletionBuilder).IsPublic);
        }

        [TestMethod]
        public void TemplateCompletionBuilderIsSealedAndNotMeantToHaveDerivedClasses()
        {
            Assert.IsTrue(typeof(TemplateCompletionBuilder).IsSealed);
        }

        #region Completions for Directive Names

        [TestMethod]
        public void CompletionsReturnsDirectiveNamesWhenPositionIsWithinDirectiveName()
        {
            var builder = new TemplateCompletionBuilder(42);
            builder.Visit(new DirectiveName(42, string.Empty));
            Assert.IsNotNull(builder.Completions);
            Assert.AreEqual(6, builder.Completions.Count);
            Assert.AreEqual(1, builder.Completions.Count(c => c.DisplayText == "assembly"),  "assembly");
            Assert.AreEqual(1, builder.Completions.Count(c => c.DisplayText == "import"),    "import");
            Assert.AreEqual(1, builder.Completions.Count(c => c.DisplayText == "include"),   "include");
            Assert.AreEqual(1, builder.Completions.Count(c => c.DisplayText == "output"),    "output");
            Assert.AreEqual(1, builder.Completions.Count(c => c.DisplayText == "parameter"), "parameter");
            Assert.AreEqual(1, builder.Completions.Count(c => c.DisplayText == "template"),  "template");
        }

        [TestMethod]
        public void CompletionsReturnsDirectiveNamesWithDescriptions()
        {
            var builder = new TemplateCompletionBuilder(42);
            builder.Visit(new DirectiveName(42, string.Empty));
            foreach (Completion completion in builder.Completions)
            {
                Assert.IsFalse(string.IsNullOrEmpty(completion.Description), completion.DisplayText + " completion should have a description.");
            }
        }

        [TestMethod]
        public void CompletionsReturnsNullWhenPositionIsOutsideOfDirectiveName()
        {
            var builder = new TemplateCompletionBuilder(0);
            builder.Visit(new DirectiveName(42, string.Empty));
            Assert.IsNull(builder.Completions);
        }

        #endregion

        #region Completions for Attribute Names

        [TestMethod]
        public void CompletionsReturnsAttributeNamesWhenPositionIsWithinAttributeName()
        {
            // <#@ output e="" #>
            var directive = new OutputDirective(
                new DirectiveBlockStart(0), 
                new DirectiveName(4, "output"), 
                new[] { new Attribute(new AttributeName(11, "e"), new Equals(12), new DoubleQuote(13), new AttributeValue(14, string.Empty), new DoubleQuote(14)) }, 
                new BlockEnd(16));
            var builder = new TemplateCompletionBuilder(12);
            builder.Visit(directive);
            Assert.IsNotNull(builder.Completions);
            Assert.AreEqual(1, builder.Completions.Count(c => string.Equals(c.DisplayText, "extension", StringComparison.OrdinalIgnoreCase)), "extension");
            Assert.AreEqual(1, builder.Completions.Count(c => string.Equals(c.DisplayText, "encoding", StringComparison.OrdinalIgnoreCase)), "encoding");
        }

        [TestMethod]
        public void CompletionsReturnsAttributeNamesWithDescriptions()
        {
            var directive = new OutputDirective(
                new DirectiveBlockStart(0),
                new DirectiveName(4, "output"),
                new[] { new Attribute(new AttributeName(11, "e"), new Equals(12), new DoubleQuote(13), new AttributeValue(14, string.Empty), new DoubleQuote(14)) },
                new BlockEnd(16));
            var builder = new TemplateCompletionBuilder(12);
            builder.Visit(directive);
            foreach (Completion completion in builder.Completions)
            {
                Assert.IsFalse(string.IsNullOrEmpty(completion.Description), completion.DisplayText + " completion should have a description.");
            }
        }

        [TestMethod]
        public void CompletionsExcludeAttributesAlreadyPresentInDirective()
        {
            // <#@ output extension="txt" e="" #>
            var attributes = new[] 
            { 
                new Attribute(new AttributeName(11, "extension"), new Equals(20), new DoubleQuote(21), new AttributeValue(22, "txt"), new DoubleQuote(25)),
                new Attribute(new AttributeName(27, "e"), new Equals(28), new DoubleQuote(29), new AttributeValue(30, string.Empty), new DoubleQuote(30)) 
            };
            var directive = new OutputDirective(new DirectiveBlockStart(0), new DirectiveName(4, "output"), attributes, new BlockEnd(32));
            var builder = new TemplateCompletionBuilder(28);
            builder.Visit(directive);
            Assert.IsTrue(string.Equals(builder.Completions.Single().DisplayText, "encoding", StringComparison.OrdinalIgnoreCase));
        }

        [TestMethod]
        public void CompletionsReturnsNullWhenAllAttributesAreAlreadyPresentInDirective()
        {
            // <#@ output extension="txt" encoding="UTF8" #>
            var attributes = new[] 
            { 
                new Attribute(new AttributeName(11, "extension"), new Equals(20), new DoubleQuote(21), new AttributeValue(22, "txt"), new DoubleQuote(25)),
                new Attribute(new AttributeName(27, "encoding"), new Equals(35), new DoubleQuote(36), new AttributeValue(37, "UTF8"), new DoubleQuote(41)) 
            };
            var directive = new OutputDirective(new DirectiveBlockStart(0), new DirectiveName(4, "output"), attributes, new BlockEnd(43));
            var builder = new TemplateCompletionBuilder(35);
            builder.Visit(directive);
            Assert.IsNull(builder.Completions);
        }

        [TestMethod]
        public void CompletionsReturnsNullWhenPositionIsOutsideOfAttributeName()
        {
            // <#@ output e="" #>
            var directive = new OutputDirective(
                new DirectiveBlockStart(0),
                new DirectiveName(4, "output"),
                new[] { new Attribute(new AttributeName(11, "e"), new Equals(12), new DoubleQuote(13), new AttributeValue(14, string.Empty), new DoubleQuote(14)) },
                new BlockEnd(16));
            var builder = new TemplateCompletionBuilder(13);
            builder.Visit(directive);
            Assert.IsNull(builder.Completions);
        }

        #endregion

        #region Completions for Attribute Values

        [TestMethod]
        public void CompletionsReturnsWellKnownValuesWhenPositionIsWithinAttributeValue()
        {
            // <#@ template debug="" #>
            var directive = new TemplateDirective(
                new DirectiveBlockStart(0),
                new DirectiveName(4, "template"),
                new[] { new Attribute(new AttributeName(13, "debug"), new Equals(18), new DoubleQuote(19), new AttributeValue(20, string.Empty), new DoubleQuote(20)) },
                new BlockEnd(22));
            var builder = new TemplateCompletionBuilder(20);
            builder.Visit(directive);
            Assert.IsNotNull(builder.Completions);
            Assert.AreEqual("false", builder.Completions[0].DisplayText, true, CultureInfo.InvariantCulture);
            Assert.AreEqual("true", builder.Completions[1].DisplayText, true, CultureInfo.InvariantCulture);
        }

        [TestMethod]
        public void CompletionsReturnsAttributeValuesWithDescriptions()
        {
            // <#@ template debug="" #>
            var directive = new TemplateDirective(
                new DirectiveBlockStart(0),
                new DirectiveName(4, "template"),
                new[] { new Attribute(new AttributeName(13, "debug"), new Equals(18), new DoubleQuote(19), new AttributeValue(20, string.Empty), new DoubleQuote(20)) },
                new BlockEnd(22));
            var builder = new TemplateCompletionBuilder(20);
            builder.Visit(directive);
            foreach (Completion completion in builder.Completions)
            {
                Assert.IsFalse(string.IsNullOrEmpty(completion.Description), completion.DisplayText + " completion should have a description.");
            }
        }

        [TestMethod]
        public void CompletionsReturnsNullWhenPositionIsOutsideOfAttributeValue()
        {
            // <#@ template debug="" #>
            var directive = new TemplateDirective(
                new DirectiveBlockStart(0),
                new DirectiveName(4, "template"),
                new[] { new Attribute(new AttributeName(13, "debug"), new Equals(18), new DoubleQuote(19), new AttributeValue(20, string.Empty), new DoubleQuote(20)) },
                new BlockEnd(22));
            var builder = new TemplateCompletionBuilder(19);
            builder.Visit(directive);
            Assert.IsNull(builder.Completions);
        }

        [TestMethod]
        public void CompletionReturnsNullWhenPositionIsInsideValueOfUnrecognizedAttribute()
        {
            // <#@ template debug="" #>
            var directive = new TemplateDirective(
                new DirectiveBlockStart(0),
                new DirectiveName(4, "template"),
                new[] { new Attribute(new AttributeName(13, "foo"), new Equals(16), new DoubleQuote(17), new AttributeValue(18, string.Empty), new DoubleQuote(18)) },
                new BlockEnd(20));
            var builder = new TemplateCompletionBuilder(18);
            builder.Visit(directive);
            Assert.IsNull(builder.Completions);
        }

        #endregion

        [TestMethod]
        public void NodeReturnsDirectiveNameWhenPositionIsWithinDirectiveName()
        {
            var node = new DirectiveName(42, string.Empty);
            var builder = new TemplateCompletionBuilder(42);
            builder.Visit(node);
            Assert.AreSame(node, builder.Node);
        }

        [TestMethod]
        public void NodeReturnsNullWhenPositionIsOutsideOfDirectiveName()
        {
            var builder = new TemplateCompletionBuilder(0);
            builder.Visit(new DirectiveName(42, string.Empty));
            Assert.IsNull(builder.Node);
        }

        [TestMethod]
        public void NodeReturnsAttributeNameWhenPositionIsWithinAttributeName()
        {
            AttributeName attributeName;
            var directive = new OutputDirective(
                new DirectiveBlockStart(0),
                new DirectiveName(4, "output"),
                new[] { new Attribute(attributeName = new AttributeName(11, "e"), new Equals(12), new DoubleQuote(13), new AttributeValue(14, string.Empty), new DoubleQuote(14)) },
                new BlockEnd(16));
            var builder = new TemplateCompletionBuilder(12);
            builder.Visit(directive);
            Assert.AreSame(attributeName, builder.Node);
        }

        [TestMethod]
        public void NodeReturnsNullWhenPositionIsOutsideOfAttributeName()
        {
            var directive = new OutputDirective(
                new DirectiveBlockStart(0),
                new DirectiveName(4, "output"),
                new[] { new Attribute(new AttributeName(11, "e"), new Equals(12), new DoubleQuote(13), new AttributeValue(14, string.Empty), new DoubleQuote(14)) },
                new BlockEnd(16));
            var builder = new TemplateCompletionBuilder(13);
            builder.Visit(directive);
            Assert.IsNull(builder.Node);
        }

        [TestMethod]
        public void NodeReturnsAttributeValueWhenPositionIsWithinAttributeValue()
        {
            AttributeValue attributeValue;
            var directive = new TemplateDirective(
                new DirectiveBlockStart(0),
                new DirectiveName(4, "template"),
                new[] { new Attribute(new AttributeName(13, "debug"), new Equals(18), new DoubleQuote(19), attributeValue = new AttributeValue(20, string.Empty), new DoubleQuote(20)) },
                new BlockEnd(22));
            var builder = new TemplateCompletionBuilder(20);
            builder.Visit(directive);
            Assert.AreSame(attributeValue, builder.Node);
        }

        [TestMethod]
        public void NodeReturnsNullWhenPositionIsOutsideOfAttributeValue()
        {
            var directive = new TemplateDirective(
                new DirectiveBlockStart(0),
                new DirectiveName(4, "template"),
                new[] { new Attribute(new AttributeName(13, "debug"), new Equals(18), new DoubleQuote(19), new AttributeValue(20, string.Empty), new DoubleQuote(20)) },
                new BlockEnd(22));
            var builder = new TemplateCompletionBuilder(19);
            builder.Visit(directive);
            Assert.IsNull(builder.Node);
        }
    }
}
