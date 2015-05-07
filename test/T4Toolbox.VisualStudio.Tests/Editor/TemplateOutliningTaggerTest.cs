// <copyright file="TemplateOutliningTaggerTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.Editor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Tagging;
    using T4Toolbox.VisualStudio.Tests.Fakes;

    [TestClass]
    public class TemplateOutliningTaggerTest
    {
        [TestMethod]
        public void TemplateOutliningTaggerIsInternalAndNotMeantForUseOutsideOfT4Toolbox()
        {
            Assert.IsFalse(typeof(TemplateOutliningTagger).IsPublic);
        }

        [TestMethod]
        public void TemplateOutliningTaggerIsSealedAndNotMeantToHaveChildClasses()
        {
            Assert.IsTrue(typeof(TemplateOutliningTagger).IsSealed);
        }

        [TestMethod]
        public void GetTagsReturnsOutliningRegionSpanForCodeBlocks()
        {
            ITagSpan<OutliningRegionTag> outliningSpan = GetTags("<# code #>").Single();
            Assert.AreEqual(new Span(0, 10), outliningSpan.Span);
        }

        [TestMethod]
        public void GetTagsReturnsOutliningRegionSpanWithCollapsedFormWhereEllipsisReplacesCode()
        {
            ITagSpan<OutliningRegionTag> outliningSpan = GetTags("<# code #>").Single();
            Assert.AreEqual("<#...#>", outliningSpan.Tag.CollapsedForm);
        }

        [TestMethod]
        public void GetTagsReturnsOutliningRegionSpanWithCollapsedHintFormShowingFullCodeBlock()
        {
            ITagSpan<OutliningRegionTag> outliningSpan = GetTags("<# code #>").Single();
            Assert.AreEqual("<# code #>", outliningSpan.Tag.CollapsedHintForm);
        }

        [TestMethod]
        public void GetTagsReturnsOutliningRegionsSpanWithCollapsedHintFormShowingEllipsisAfter10Lines()
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

            Assert.AreEqual(expectedHint, outliningSpan.Tag.CollapsedHintForm);
        }

        [TestMethod]
        public void CreateTagSpansDoesNotThrowWhenTemplateCouldNotBeParsedFromText()
        {
            Assert.IsFalse(GetTags("<#").Any());
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