// <copyright file="TemplateErrorTaggerTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.Editor
{
    using System.Linq;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Tagging;
    using Xunit;

    public class TemplateErrorTaggerTest
    {
        [Fact]
        public void TemplateErrorTaggerIsInternalAndNotIntendedForConsumptionOutsideOfT4Toolbox()
        {
            Assert.True(typeof(TemplateErrorTagger).IsNotPublic);
        }

        [Fact]
        public void TemplateErrorTaggerIsSealedAndNotIntendedHaveChildClasses()
        {
            Assert.True(typeof(TemplateErrorTagger).IsSealed);
        }

        [Fact]
        public void GetTagsReturnsErrorSpanForSyntaxError()
        {
            var buffer = new FakeTextBuffer("<#");
            var tagger = new TemplateErrorTagger(buffer);

            var snapshotSpans = new NormalizedSnapshotSpanCollection(new SnapshotSpan(buffer.CurrentSnapshot, 0, buffer.CurrentSnapshot.Length));
            ITagSpan<ErrorTag> errorSpan = tagger.GetTags(snapshotSpans).Single();

            Assert.Equal(new Span(2, 0), errorSpan.Span);
            Assert.Contains("#>", (string)errorSpan.Tag.ToolTipContent);
        }

        [Fact]
        public void GetTagsReturnsErrorSpanForSemanticError()
        {
            var buffer = new FakeTextBuffer("<#@ include file=\" \" #>");
            var tagger = new TemplateErrorTagger(buffer);

            var snapshotSpans = new NormalizedSnapshotSpanCollection(new SnapshotSpan(buffer.CurrentSnapshot, 0, buffer.CurrentSnapshot.Length));
            ITagSpan<ErrorTag> errorSpan = tagger.GetTags(snapshotSpans).Single();

            Assert.Equal(new Span(12, 8), errorSpan.Span);
            Assert.Contains("File", (string)errorSpan.Tag.ToolTipContent);
        }
    }
}