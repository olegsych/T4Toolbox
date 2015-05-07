// <copyright file="TemplateClassificationTaggerTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.Editor
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Classification;
    using Microsoft.VisualStudio.Text.Tagging;
    using T4Toolbox.VisualStudio.Tests.Fakes;

    [TestClass]
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

        [TestMethod]
        public void TextBufferChangeCreatesNewTagSpans()
        {
            var buffer = new FakeTextBuffer(string.Empty);
            var tagger = new TemplateClassificationTagger(buffer, this.registry);
            buffer.CurrentSnapshot = new FakeTextSnapshot("<#");
            Assert.IsTrue(tagger.GetTaggedSpans(new SnapshotSpan(buffer.CurrentSnapshot, 0, buffer.CurrentSnapshot.Length)).Any());
        }

        [TestMethod]
        public void TextBufferChangeRemovesOldTagSpans()
        {
            var buffer = new FakeTextBuffer("<#");
            var tagger = new TemplateClassificationTagger(buffer, this.registry);
            buffer.CurrentSnapshot = new FakeTextSnapshot(string.Empty);
            Assert.IsFalse(tagger.GetTaggedSpans(new SnapshotSpan(buffer.CurrentSnapshot, 0, buffer.CurrentSnapshot.Length)).Any());
        }

        [TestMethod]
        public void TextBufferChangeRaisesTagsChangedEvent()
        {
            var buffer = new FakeTextBuffer(string.Empty);
            var tagger = new TemplateClassificationTagger(buffer, this.registry);

            bool tagsChangedEventRaised = false;
            tagger.TagsChanged += (sender, args) => tagsChangedEventRaised = true;

            buffer.CurrentSnapshot = new FakeTextSnapshot("<#");
            Assert.IsTrue(tagsChangedEventRaised);
        }

        #endregion

        #region Token to Classification mapping

        [TestMethod]
        public void GetTagsReturnsCodeBlockSpanForCodeBlockToken()
        {
            ITagSpan<ClassificationTag> span = this.GetTags("<# statement(); #>").ElementAt(1);
            Assert.AreEqual(this.codeBlockClassification, span.Tag.ClassificationType);
        }

        [TestMethod]
        public void GetTagsReturnsDelimiterSpanForStatementBlockStartToken()
        {
            ITagSpan<ClassificationTag> span = this.GetTags("<#").Single();
            Assert.AreEqual(this.delimiterClassification, span.Tag.ClassificationType);
        }

        [TestMethod]
        public void GetTagsReturnsDelimiterSpanForExpressionBlockStartToken()
        {
            ITagSpan<ClassificationTag> span = this.GetTags("<#=").Single();
            Assert.AreEqual(this.delimiterClassification, span.Tag.ClassificationType);
        }

        [TestMethod]
        public void GetTagsReturnsDelimiterSpanForClassBlockStartToken()
        {
            ITagSpan<ClassificationTag> span = this.GetTags("<#+").Single();
            Assert.AreEqual(this.delimiterClassification, span.Tag.ClassificationType);
        }

        [TestMethod]
        public void GetTagsReturnsDelimiterSpanForDirectiveBlockStartToken()
        {
            ITagSpan<ClassificationTag> span = this.GetTags("<#@").Single();
            Assert.AreEqual(this.delimiterClassification, span.Tag.ClassificationType);
        }

        [TestMethod]
        public void GetTagsReturnsDelimiterSpanForBlockEndToken()
        {
            ITagSpan<ClassificationTag> span = this.GetTags("#>").Single();
            Assert.AreEqual(this.delimiterClassification, span.Tag.ClassificationType);
        }

        [TestMethod]
        public void GetTagsReturnsDirectiveNameSpanForDirectiveNameToken()
        {
            ITagSpan<ClassificationTag> span = this.GetTags("<#@ directive #>").ElementAt(1);
            Assert.AreEqual(this.directiveNameClassification, span.Tag.ClassificationType);
        }

        [TestMethod]
        public void GetTagsReturnsAttributeNameSpanForAttributeNameToken()
        {
            ITagSpan<ClassificationTag> span = this.GetTags("<#@ directive attribute #>").ElementAt(2);
            Assert.AreEqual(this.attributeNameClassification, span.Tag.ClassificationType);
        }

        [TestMethod]
        public void GetTagsIgnoresEqualsBetweenAttributeNameAndAttributeValue()
        {
            ITagSpan<ClassificationTag> lastSpan = this.GetTags("<#@ directive attribute =").Last();
            Assert.AreEqual(this.attributeNameClassification, lastSpan.Tag.ClassificationType);
            Assert.AreEqual("attribute", lastSpan.Span.GetText());
        }

        [TestMethod]
        public void GetTagsReturnsAttributeValueSpanForAttributeValueToken()
        {
            ITagSpan<ClassificationTag> span = this.GetTags("<#@ \"value\" #>").ElementAt(1);
            Assert.AreEqual(this.attributeValueClassification, span.Tag.ClassificationType);
            Assert.AreEqual("value", span.Span.GetText());
        }

        [TestMethod]
        public void GetTagsIgnoresDoubleQuotesAroundAttributeValue()
        {
            ITagSpan<ClassificationTag> lastSpan = this.GetTags("<#@ \"value\"").Last();
            Assert.AreEqual(this.attributeValueClassification, lastSpan.Tag.ClassificationType);
            Assert.AreEqual("value", lastSpan.Span.GetText());
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