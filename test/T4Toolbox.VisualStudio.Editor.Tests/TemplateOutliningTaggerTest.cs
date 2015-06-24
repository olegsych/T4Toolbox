// <copyright file="TemplateOutliningTaggerTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.Editor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Tagging;
    using Xunit;

    public static class TemplateOutliningTaggerTest
    {
        [Fact]
        public static void TemplateOutliningTaggerIsInternalAndNotMeantForUseOutsideOfT4Toolbox()
        {
            Assert.False(typeof(TemplateOutliningTagger).IsPublic);
        }

        [Fact]
        public static void TemplateOutliningTaggerIsSealedAndNotMeantToHaveChildClasses()
        {
            Assert.True(typeof(TemplateOutliningTagger).IsSealed);
        }

        [Fact]
        public static void GetTagsReturnsOutliningRegionSpanForCodeBlocks()
        {
            ITagSpan<OutliningRegionTag> outliningSpan = GetTags("<# code #>").Single();
            Assert.Equal(new Span(0, 10), outliningSpan.Span);
        }

        [Fact]
        public static void GetTagsReturnsOutliningRegionSpanWithCollapsedFormWhereEllipsisReplacesCode()
        {
            ITagSpan<OutliningRegionTag> outliningSpan = GetTags("<# code #>").Single();
            Assert.Equal("<#...#>", outliningSpan.Tag.CollapsedForm);
        }

        [Fact]
        public static void GetTagsReturnsOutliningRegionSpanWithCollapsedHintFormShowingFullCodeBlock()
        {
            ITagSpan<OutliningRegionTag> outliningSpan = GetTags("<# code #>").Single();
            Assert.Equal("<# code #>", outliningSpan.Tag.CollapsedHintForm);
        }

        [Fact]
        public static void GetTagsReturnsOutliningRegionsSpanWithCollapsedHintFormShowingEllipsisAfter10Lines()
        {
            string codeBlock = "<#" + Environment.NewLine;

            for (int i = 1; i <= 10; i++)
            {
                codeBlock += "Line" + i + Environment.NewLine;
            }

            codeBlock += "#>";

            ITagSpan<OutliningRegionTag> outliningSpan = GetTags(codeBlock).Single();

            var expectedHint = string.Join(
                Environment.NewLine,
                codeBlock.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).Take(10).Concat(new[] { "..." }));

            Assert.Equal(expectedHint, outliningSpan.Tag.CollapsedHintForm);
        }

        [Fact]
        public static void CreateTagSpansDoesNotThrowWhenTemplateCouldNotBeParsedFromText()
        {
            Assert.False(GetTags("<#").Any());
        }

        private static IEnumerable<ITagSpan<OutliningRegionTag>> GetTags(string input)
        {
            var buffer = new FakeTextBuffer(input);
            var tagger = new TemplateOutliningTagger(buffer);

            var snapshotSpans = new NormalizedSnapshotSpanCollection(new SnapshotSpan(buffer.CurrentSnapshot, 0, buffer.CurrentSnapshot.Length));
            return tagger.GetTags(snapshotSpans);
        }
    }
}