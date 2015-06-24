// <copyright file="TemplateQuickInfoSourceTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.Editor
{
    using System;
    using System.Collections.Generic;
    using Microsoft.VisualStudio.Text;
    using Xunit;

    public static class TemplateQuickInfoSourceTest
    {
        [Fact]
        public static void TemplateQuickInfoSourceIsInternalAndConsumedOnlyByVisualStudioEditor()
        {
            Assert.False(typeof(TemplateQuickInfoSource).IsPublic);
        }

        [Fact]
        public static void TemplateQuickInfoSourceIsSealedAndNotMeantToBeExtended()
        {
            Assert.True(typeof(TemplateQuickInfoSource).IsSealed);
        }

        [Fact]
        public static void AugmentQuickInfoSessionReturnsNoContentOrApplicableSpanWhenBufferIsEmpty()
        {
            var buffer = new FakeTextBuffer(string.Empty);
            var session = new FakeQuickInfoSession();
            using (var source = new TemplateQuickInfoSource(buffer))
            {
                var quickInfoContent = new List<object>();
                ITrackingSpan applicableToSpan;
                source.AugmentQuickInfoSession(session, quickInfoContent, out applicableToSpan);

                Assert.Equal(0, quickInfoContent.Count);
                Assert.Null(applicableToSpan);
            }
        }

        [Fact]
        public static void AugmentQuickInfoSessionReturnsNoContentOrApplicableSpanSyntaxNodeAtTriggerPointHasNoDescription()
        {
            var buffer = new FakeTextBuffer("<# code #>");
            var session = new FakeQuickInfoSession { SnapshotTriggerPoint = new SnapshotPoint(buffer.CurrentSnapshot, 3) };
            using (var source = new TemplateQuickInfoSource(buffer))
            {
                var quickInfoContent = new List<object>();
                ITrackingSpan applicableToSpan;
                source.AugmentQuickInfoSession(session, quickInfoContent, out applicableToSpan);

                Assert.Equal(0, quickInfoContent.Count);
                Assert.Null(applicableToSpan);
            }
        }

        [Fact]
        public static void AugmentQuickInfoSessionReturnsNoContentOrApplicableSpanWhenBufferContainsTemplateThatCouldNotBeParsed()
        {
            var buffer = new FakeTextBuffer("<#");
            var session = new FakeQuickInfoSession { SnapshotTriggerPoint = new SnapshotPoint(buffer.CurrentSnapshot, 1) };
            using (var source = new TemplateQuickInfoSource(buffer))
            {
                var quickInfoContent = new List<object>();
                ITrackingSpan applicableToSpan;
                source.AugmentQuickInfoSession(session, quickInfoContent, out applicableToSpan);

                Assert.Equal(0, quickInfoContent.Count);
                Assert.Null(applicableToSpan);
            }
        }

        [Fact]
        public static void AugmentQuickInfoSessionReturnsDescriptionOfSyntaxNodeAtTriggerPoint()
        {
            var buffer = new FakeTextBuffer("<#@ assembly name=\"System\" #>");
            var session = new FakeQuickInfoSession { SnapshotTriggerPoint = new SnapshotPoint(buffer.CurrentSnapshot, 5) };
            using (var source = new TemplateQuickInfoSource(buffer))
            {
                var quickInfoContent = new List<object>();
                ITrackingSpan applicableToSpan;
                source.AugmentQuickInfoSession(session, quickInfoContent, out applicableToSpan);

                Assert.Contains("assembly", (string)quickInfoContent[0], StringComparison.Ordinal);
            }
        }

        [Fact]
        public static void AugmentQuickInfoSessionReturnsSpanOfSyntaxNodeProvidingDescription()
        {
            var buffer = new FakeTextBuffer("<#@ assembly name=\"System\" #>");
            var session = new FakeQuickInfoSession { SnapshotTriggerPoint = new SnapshotPoint(buffer.CurrentSnapshot, 15) };
            using (var source = new TemplateQuickInfoSource(buffer))
            {
                var quickInfoContent = new List<object>();
                ITrackingSpan applicableToSpan;
                source.AugmentQuickInfoSession(session, quickInfoContent, out applicableToSpan);

                Assert.Equal(new Span(13, 13), applicableToSpan.GetSpan(buffer.CurrentSnapshot).Span);
            }
        }
    }
}