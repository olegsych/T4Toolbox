// <copyright file="TemplateCompletionBuilderTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.Editor
{
    using System;
    using System.Globalization;
    using System.Linq;
    using Microsoft.VisualStudio.Language.Intellisense;
    using T4Toolbox.TemplateAnalysis;
    using Xunit;
    using Attribute = T4Toolbox.TemplateAnalysis.Attribute;

    public class TemplateCompletionBuilderTest
    {
        [Fact]
        public void TemplateCompletionBuilderIsInternalAndNotMeantForPublicConsumption()
        {
            Assert.False(typeof(TemplateCompletionBuilder).IsPublic);
        }

        [Fact]
        public void TemplateCompletionBuilderIsSealedAndNotMeantToHaveDerivedClasses()
        {
            Assert.True(typeof(TemplateCompletionBuilder).IsSealed);
        }

        #region Completions for Directive Names

        [Fact]
        public void CompletionsReturnsDirectiveNamesWhenPositionIsWithinDirectiveName()
        {
            var builder = new TemplateCompletionBuilder(42);
            builder.Visit(new DirectiveName(42, string.Empty));
            Assert.NotNull(builder.Completions);
            Assert.Equal(6, builder.Completions.Count);
            Assert.Contains(builder.Completions, c => c.DisplayText == "assembly");
            Assert.Contains(builder.Completions, c => c.DisplayText == "import");
            Assert.Contains(builder.Completions, c => c.DisplayText == "include");
            Assert.Contains(builder.Completions, c => c.DisplayText == "output");
            Assert.Contains(builder.Completions, c => c.DisplayText == "parameter");
            Assert.Contains(builder.Completions, c => c.DisplayText == "template");
        }

        [Fact]
        public void CompletionsReturnsDirectiveNamesWithDescriptions()
        {
            var builder = new TemplateCompletionBuilder(42);
            builder.Visit(new DirectiveName(42, string.Empty));
            foreach (Completion completion in builder.Completions)
            {
                Assert.False(string.IsNullOrEmpty(completion.Description), completion.DisplayText + " completion should have a description.");
            }
        }

        [Fact]
        public void CompletionsReturnsNullWhenPositionIsOutsideOfDirectiveName()
        {
            var builder = new TemplateCompletionBuilder(0);
            builder.Visit(new DirectiveName(42, string.Empty));
            Assert.Null(builder.Completions);
        }

        #endregion

        #region Completions for Attribute Names

        [Fact]
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
            Assert.NotNull(builder.Completions);
            Assert.Contains(builder.Completions, c => string.Equals(c.DisplayText, "encoding", StringComparison.OrdinalIgnoreCase));
            Assert.Contains(builder.Completions, c => string.Equals(c.DisplayText, "extension", StringComparison.OrdinalIgnoreCase));
        }

        [Fact]
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
                Assert.False(string.IsNullOrEmpty(completion.Description), completion.DisplayText + " completion should have a description.");
            }
        }

        [Fact]
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
            Assert.True(string.Equals(builder.Completions.Single().DisplayText, "encoding", StringComparison.OrdinalIgnoreCase));
        }

        [Fact]
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
            Assert.Null(builder.Completions);
        }

        [Fact]
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
            Assert.Null(builder.Completions);
        }

        #endregion

        #region Completions for Attribute Values

        [Fact]
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
            Assert.NotNull(builder.Completions);
            Assert.Equal("false", builder.Completions[0].DisplayText, StringComparer.InvariantCultureIgnoreCase);
            Assert.Equal("true", builder.Completions[1].DisplayText, StringComparer.InvariantCultureIgnoreCase);
        }

        [Fact]
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
                Assert.False(string.IsNullOrEmpty(completion.Description), completion.DisplayText + " completion should have a description.");
            }
        }

        [Fact]
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
            Assert.Null(builder.Completions);
        }

        [Fact]
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
            Assert.Null(builder.Completions);
        }

        #endregion

        [Fact]
        public void NodeReturnsDirectiveNameWhenPositionIsWithinDirectiveName()
        {
            var node = new DirectiveName(42, string.Empty);
            var builder = new TemplateCompletionBuilder(42);
            builder.Visit(node);
            Assert.Same(node, builder.Node);
        }

        [Fact]
        public void NodeReturnsNullWhenPositionIsOutsideOfDirectiveName()
        {
            var builder = new TemplateCompletionBuilder(0);
            builder.Visit(new DirectiveName(42, string.Empty));
            Assert.Null(builder.Node);
        }

        [Fact]
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
            Assert.Same(attributeName, builder.Node);
        }

        [Fact]
        public void NodeReturnsNullWhenPositionIsOutsideOfAttributeName()
        {
            var directive = new OutputDirective(
                new DirectiveBlockStart(0),
                new DirectiveName(4, "output"),
                new[] { new Attribute(new AttributeName(11, "e"), new Equals(12), new DoubleQuote(13), new AttributeValue(14, string.Empty), new DoubleQuote(14)) },
                new BlockEnd(16));
            var builder = new TemplateCompletionBuilder(13);
            builder.Visit(directive);
            Assert.Null(builder.Node);
        }

        [Fact]
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
            Assert.Same(attributeValue, builder.Node);
        }

        [Fact]
        public void NodeReturnsNullWhenPositionIsOutsideOfAttributeValue()
        {
            var directive = new TemplateDirective(
                new DirectiveBlockStart(0),
                new DirectiveName(4, "template"),
                new[] { new Attribute(new AttributeName(13, "debug"), new Equals(18), new DoubleQuote(19), new AttributeValue(20, string.Empty), new DoubleQuote(20)) },
                new BlockEnd(22));
            var builder = new TemplateCompletionBuilder(19);
            builder.Visit(directive);
            Assert.Null(builder.Node);
        }
    }
}
