// <copyright file="TemplateQuickInfoSourceTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.Editor
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

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
        public static async Task AugmentQuickInfoSessionReturnsNoContentOrApplicableSpanWhenBufferIsEmpty()
        {
            var buffer = new FakeTextBuffer(string.Empty);
            var session = new FakeQuickInfoSession();
            using (var source = new TemplateQuickInfoSource(buffer))
            {
                var result = await source.GetQuickInfoItemAsync(session, CancellationToken.None);

                Assert.Null(result?.ApplicableToSpan);
            }
        }

        [Fact]
        public static async Task AugmentQuickInfoSessionReturnsNoContentOrApplicableSpanSyntaxNodeAtTriggerPointHasNoDescription()
        {
            var buffer = new FakeTextBuffer("<# code #>");
            var session = new FakeQuickInfoSession { SnapshotTriggerPoint = new SnapshotPoint(buffer.CurrentSnapshot, 3) };
            using (var source = new TemplateQuickInfoSource(buffer))
            {
                var result = await source.GetQuickInfoItemAsync(session, CancellationToken.None);

                Assert.Null(result?.ApplicableToSpan);
            }
        }

        [Fact]
        public static async Task AugmentQuickInfoSessionReturnsNoContentOrApplicableSpanWhenBufferContainsTemplateThatCouldNotBeParsed()
        {
            var buffer = new FakeTextBuffer("<#");
            var session = new FakeQuickInfoSession { SnapshotTriggerPoint = new SnapshotPoint(buffer.CurrentSnapshot, 1) };
            using (var source = new TemplateQuickInfoSource(buffer))
            {
                var result = await source.GetQuickInfoItemAsync(session, CancellationToken.None);

                Assert.Null(result?.ApplicableToSpan);
            }
        }

        [Fact]
        public static async Task AugmentQuickInfoSessionReturnsDescriptionOfSyntaxNodeAtTriggerPoint()
        {
            var buffer = new FakeTextBuffer("<#@ assembly name=\"System\" #>");
            var session = new FakeQuickInfoSession { SnapshotTriggerPoint = new SnapshotPoint(buffer.CurrentSnapshot, 5) };
            using (var source = new TemplateQuickInfoSource(buffer))
            {
                var result = await source.GetQuickInfoItemAsync(session, CancellationToken.None);

                Assert.Contains("assembly", (string)result.Item, StringComparison.Ordinal);
            }
        }

        [Fact]
        public static async Task AugmentQuickInfoSessionReturnsSpanOfSyntaxNodeProvidingDescription()
        {
            var buffer = new FakeTextBuffer("<#@ assembly name=\"System\" #>");
            var session = new FakeQuickInfoSession { SnapshotTriggerPoint = new SnapshotPoint(buffer.CurrentSnapshot, 15) };
            using (var source = new TemplateQuickInfoSource(buffer))
            {
                var result = await source.GetQuickInfoItemAsync(session, CancellationToken.None);

                Assert.Equal(new Span(13, 13), result.ApplicableToSpan.GetSpan(buffer.CurrentSnapshot).Span);
            }
        }
    }
}