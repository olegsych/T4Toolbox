// <copyright file="TemplateErrorTaggerTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.Editor
{
    using System;
    using System.Linq;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Tagging;
    using Xunit;

    public static class TemplateErrorTaggerTest
    {
        [Fact]
        public static void TemplateErrorTaggerIsInternalAndNotIntendedForConsumptionOutsideOfT4Toolbox()
        {
            Assert.True(typeof(TemplateErrorTagger).IsNotPublic);
        }

        [Fact]
        public static void TemplateErrorTaggerIsSealedAndNotIntendedHaveChildClasses()
        {
            Assert.True(typeof(TemplateErrorTagger).IsSealed);
        }

        [Fact]
        public static void GetTagsReturnsErrorSpanForSyntaxError()
        {
            var buffer = new FakeTextBuffer("<#");
            var tagger = new TemplateErrorTagger(buffer);

            var snapshotSpans = new NormalizedSnapshotSpanCollection(new SnapshotSpan(buffer.CurrentSnapshot, 0, buffer.CurrentSnapshot.Length));
            ITagSpan<ErrorTag> errorSpan = tagger.GetTags(snapshotSpans).Single();

            Assert.Equal(new Span(2, 0), errorSpan.Span);
            Assert.Contains("#>", (string)errorSpan.Tag.ToolTipContent, StringComparison.Ordinal);
        }

        [Fact]
        public static void GetTagsReturnsErrorSpanForSemanticError()
        {
            var buffer = new FakeTextBuffer("<#@ include file=\" \" #>");
            var tagger = new TemplateErrorTagger(buffer);

            var snapshotSpans = new NormalizedSnapshotSpanCollection(new SnapshotSpan(buffer.CurrentSnapshot, 0, buffer.CurrentSnapshot.Length));
            ITagSpan<ErrorTag> errorSpan = tagger.GetTags(snapshotSpans).Single();

            Assert.Equal(new Span(12, 8), errorSpan.Span);
            Assert.Contains("File", (string)errorSpan.Tag.ToolTipContent, StringComparison.OrdinalIgnoreCase);
        }
    }
}