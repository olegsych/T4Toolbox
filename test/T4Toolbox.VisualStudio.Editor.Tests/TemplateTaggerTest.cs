// <copyright file="TemplateTaggerTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.Editor
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Tagging;
    using Xunit;

    public class TemplateTaggerTest
    {
        [Fact]
        public void TemplateTaggerIsInternalAndNotIntendedForPublicConsumption()
        {
            Assert.True(typeof(TemplateTagger<ITag>).IsNotPublic);
        }

        [Fact]
        public void TemplateChangeCreatesNewTagSpans()
        {
            var buffer = new FakeTextBuffer(string.Empty);
            var tagger = new TestableTemplateTagger(buffer);

            bool tagsCreated = false;
            tagger.CreateTagSpansMethod = snapshot => tagsCreated = true;
            buffer.CurrentSnapshot = new FakeTextSnapshot(string.Empty);

            Assert.True(tagsCreated);
        }

        [Fact]
        public void TemplateChangeRemovesOldTagSpans()
        {
            var buffer = new FakeTextBuffer(string.Empty);
            var tagger = new TestableTemplateTagger(buffer);
            tagger.CreateTagSpan(buffer.CurrentSnapshot.CreateTrackingSpan(new Span(), SpanTrackingMode.EdgeNegative), new ErrorTag());

            buffer.CurrentSnapshot = new FakeTextSnapshot(string.Empty);

            Assert.False(tagger.GetTaggedSpans(new SnapshotSpan(buffer.CurrentSnapshot, 0, buffer.CurrentSnapshot.Length)).Any());
        }

        [Fact]
        public void TemplateChangeRaisesTagsChangedEvent()
        {
            var buffer = new FakeTextBuffer(string.Empty);
            var tagger = new TestableTemplateTagger(buffer);
            tagger.CreateTagSpan(buffer.CurrentSnapshot.CreateTrackingSpan(new Span(), SpanTrackingMode.EdgeNegative), new ErrorTag());

            bool tagsChangedEventRaised = false;
            tagger.TagsChanged += (sender, args) => tagsChangedEventRaised = true;
            buffer.CurrentSnapshot = new FakeTextSnapshot(string.Empty);

            Assert.True(tagsChangedEventRaised);
        }

        [Fact]
        public void UpdateTagSpansCreatesNewTagSpans()
        {
            var buffer = new FakeTextBuffer(string.Empty);
            var tagger = new TestableTemplateTagger(buffer);

            bool tagsCreated = false;
            tagger.CreateTagSpansMethod = snapshot => tagsCreated = true;
            tagger.UpdateTagSpans(new TemplateAnalysis(null, null, null));

            Assert.True(tagsCreated);            
        }

        [Fact]
        public void UpdateTagSpansRemovesOldSpans()
        {
            var buffer = new FakeTextBuffer(string.Empty);
            var tagger = new TestableTemplateTagger(buffer);
            tagger.CreateTagSpan(buffer.CurrentSnapshot.CreateTrackingSpan(new Span(), SpanTrackingMode.EdgeNegative), new ErrorTag());

            tagger.UpdateTagSpans(new TemplateAnalysis(null, null, null));

            Assert.Equal(0, tagger.GetTags(new NormalizedSnapshotSpanCollection(new SnapshotSpan(buffer.CurrentSnapshot, 0, buffer.CurrentSnapshot.Length))).Count());                        
        }

        [Fact]
        public void UpdateTagSpansRaisesTagsChangedEventOnlyOnce()
        {
            var buffer = new FakeTextBuffer(string.Empty);
            var tagger = new TestableTemplateTagger(buffer);
            tagger.CreateTagSpansMethod = analysis =>
            {
                tagger.CreateTagSpan(analysis.TextSnapshot.CreateTrackingSpan(new Span(), SpanTrackingMode.EdgeNegative), new ErrorTag());
                tagger.CreateTagSpan(analysis.TextSnapshot.CreateTrackingSpan(new Span(), SpanTrackingMode.EdgeNegative), new ErrorTag());
            };

            int tagsChangedEventCount = 0;
            tagger.TagsChanged += (sender, args) => tagsChangedEventCount++;
            tagger.UpdateTagSpans(new TemplateAnalysis(new FakeTextSnapshot(string.Empty), null, null));

            Assert.Equal(1, tagsChangedEventCount);
        }

        [Fact, SuppressMessage("Microsoft.Reliability", "CA2001:AvoidCallingProblematicMethods", MessageId = "System.GC.Collect", Justification = "This is a test of garbage collection")]
        public void TemplateAnalyzerDoesNotPreventTemplateErrorTaggerFromGarbageCollection()
        {
            var buffer = new FakeTextBuffer(string.Empty);
            var analyzer = TemplateAnalyzer.GetOrCreate(buffer); 
            var tagger = new WeakReference(new TestableTemplateTagger(buffer));

            GC.Collect(2, GCCollectionMode.Forced);
            GC.WaitForPendingFinalizers();

            Assert.NotNull(analyzer);
            Assert.False(tagger.IsAlive);
        }

        private class TestableTemplateTagger : TemplateTagger<ITag>
        {
            public TestableTemplateTagger(ITextBuffer buffer) : base(buffer)
            {                
            }

            public Action<TemplateAnalysis> CreateTagSpansMethod { get; set; }

            public new void UpdateTagSpans(TemplateAnalysis analysis)
            {
                base.UpdateTagSpans(analysis);
            }

            protected override void CreateTagSpans(TemplateAnalysis analysis)
            {
                if (this.CreateTagSpansMethod != null)
                {
                    this.CreateTagSpansMethod(analysis);
                }
            }
        }
    }
}