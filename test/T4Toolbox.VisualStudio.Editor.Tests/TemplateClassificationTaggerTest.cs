// <copyright file="TemplateClassificationTaggerTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.Editor
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Classification;
    using Microsoft.VisualStudio.Text.Tagging;
    using Xunit;

    public class TemplateClassificationTaggerTest
    {
        private readonly FakeClassificationTypeRegistryService registry;

        private readonly IClassificationType codeBlockClassification;
        private readonly IClassificationType delimiterClassification;
        private readonly IClassificationType directiveNameClassification;
        private readonly IClassificationType attributeNameClassification;
        private readonly IClassificationType attributeValueClassification;

        public TemplateClassificationTaggerTest()
        {
            this.registry = new FakeClassificationTypeRegistryService();
            this.codeBlockClassification = this.registry.CreateClassificationType("TextTemplate.CodeBlock", new IClassificationType[0]);
            this.delimiterClassification = this.registry.CreateClassificationType("TextTemplate.Delimiter", new IClassificationType[0]);
            this.directiveNameClassification = this.registry.CreateClassificationType("TextTemplate.DirectiveName", new IClassificationType[0]);
            this.attributeNameClassification = this.registry.CreateClassificationType("TextTemplate.AttributeName", new IClassificationType[0]);
            this.attributeValueClassification = this.registry.CreateClassificationType("TextTemplate.AttributeValue", new IClassificationType[0]);
        }

        #region TextBuffer change notification

        [Fact]
        public void TextBufferChangeCreatesNewTagSpans()
        {
            var buffer = new FakeTextBuffer(string.Empty);
            var tagger = new TemplateClassificationTagger(buffer, this.registry);
            buffer.CurrentSnapshot = new FakeTextSnapshot("<#");
            Assert.True(tagger.GetTaggedSpans(new SnapshotSpan(buffer.CurrentSnapshot, 0, buffer.CurrentSnapshot.Length)).Any());
        }

        [Fact]
        public void TextBufferChangeRemovesOldTagSpans()
        {
            var buffer = new FakeTextBuffer("<#");
            var tagger = new TemplateClassificationTagger(buffer, this.registry);
            buffer.CurrentSnapshot = new FakeTextSnapshot(string.Empty);
            Assert.False(tagger.GetTaggedSpans(new SnapshotSpan(buffer.CurrentSnapshot, 0, buffer.CurrentSnapshot.Length)).Any());
        }

        [Fact]
        public void TextBufferChangeRaisesTagsChangedEvent()
        {
            var buffer = new FakeTextBuffer(string.Empty);
            var tagger = new TemplateClassificationTagger(buffer, this.registry);

            bool tagsChangedEventRaised = false;
            tagger.TagsChanged += (sender, args) => tagsChangedEventRaised = true;

            buffer.CurrentSnapshot = new FakeTextSnapshot("<#");
            Assert.True(tagsChangedEventRaised);
        }

        #endregion

        #region Token to Classification mapping

        [Fact]
        public void GetTagsReturnsCodeBlockSpanForCodeBlockToken()
        {
            ITagSpan<ClassificationTag> span = this.GetTags("<# statement(); #>").ElementAt(1);
            Assert.Equal(this.codeBlockClassification, span.Tag.ClassificationType);
        }

        [Fact]
        public void GetTagsReturnsDelimiterSpanForStatementBlockStartToken()
        {
            ITagSpan<ClassificationTag> span = this.GetTags("<#").Single();
            Assert.Equal(this.delimiterClassification, span.Tag.ClassificationType);
        }

        [Fact]
        public void GetTagsReturnsDelimiterSpanForExpressionBlockStartToken()
        {
            ITagSpan<ClassificationTag> span = this.GetTags("<#=").Single();
            Assert.Equal(this.delimiterClassification, span.Tag.ClassificationType);
        }

        [Fact]
        public void GetTagsReturnsDelimiterSpanForClassBlockStartToken()
        {
            ITagSpan<ClassificationTag> span = this.GetTags("<#+").Single();
            Assert.Equal(this.delimiterClassification, span.Tag.ClassificationType);
        }

        [Fact]
        public void GetTagsReturnsDelimiterSpanForDirectiveBlockStartToken()
        {
            ITagSpan<ClassificationTag> span = this.GetTags("<#@").Single();
            Assert.Equal(this.delimiterClassification, span.Tag.ClassificationType);
        }

        [Fact]
        public void GetTagsReturnsDelimiterSpanForBlockEndToken()
        {
            ITagSpan<ClassificationTag> span = this.GetTags("#>").Single();
            Assert.Equal(this.delimiterClassification, span.Tag.ClassificationType);
        }

        [Fact]
        public void GetTagsReturnsDirectiveNameSpanForDirectiveNameToken()
        {
            ITagSpan<ClassificationTag> span = this.GetTags("<#@ directive #>").ElementAt(1);
            Assert.Equal(this.directiveNameClassification, span.Tag.ClassificationType);
        }

        [Fact]
        public void GetTagsReturnsAttributeNameSpanForAttributeNameToken()
        {
            ITagSpan<ClassificationTag> span = this.GetTags("<#@ directive attribute #>").ElementAt(2);
            Assert.Equal(this.attributeNameClassification, span.Tag.ClassificationType);
        }

        [Fact]
        public void GetTagsIgnoresEqualsBetweenAttributeNameAndAttributeValue()
        {
            ITagSpan<ClassificationTag> lastSpan = this.GetTags("<#@ directive attribute =").Last();
            Assert.Equal(this.attributeNameClassification, lastSpan.Tag.ClassificationType);
            Assert.Equal("attribute", lastSpan.Span.GetText());
        }

        [Fact]
        public void GetTagsReturnsAttributeValueSpanForAttributeValueToken()
        {
            ITagSpan<ClassificationTag> span = this.GetTags("<#@ \"value\" #>").ElementAt(1);
            Assert.Equal(this.attributeValueClassification, span.Tag.ClassificationType);
            Assert.Equal("value", span.Span.GetText());
        }

        [Fact]
        public void GetTagsIgnoresDoubleQuotesAroundAttributeValue()
        {
            ITagSpan<ClassificationTag> lastSpan = this.GetTags("<#@ \"value\"").Last();
            Assert.Equal(this.attributeValueClassification, lastSpan.Tag.ClassificationType);
            Assert.Equal("value", lastSpan.Span.GetText());
        }

        #endregion

        private IEnumerable<ITagSpan<ClassificationTag>> GetTags(string text)
        {
            var buffer = new FakeTextBuffer(text);
            var tagger = new TemplateClassificationTagger(buffer, this.registry);
            var spans = new NormalizedSnapshotSpanCollection(new SnapshotSpan(buffer.CurrentSnapshot, 0, buffer.CurrentSnapshot.Length));
            return tagger.GetTags(spans);
        }
    }
}