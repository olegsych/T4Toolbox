// <copyright file="TemplateQuickInfoSourceTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.Editor
{
    using System.Collections.Generic;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.VisualStudio.Text;

    [TestClass]
    public class TemplateQuickInfoSourceTest
    {
        [TestMethod]
        public void TemplateQuickInfoSourceIsInternalAndConsumedOnlyByVisualStudioEditor()
        {
            Assert.IsFalse(typeof(TemplateQuickInfoSource).IsPublic);
        }

        [TestMethod]
        public void TemplateQuickInfoSourceIsSealedAndNotMeantToBeExtended()
        {
            Assert.IsTrue(typeof(TemplateQuickInfoSource).IsSealed);
        }

        [TestMethod]
        public void AugmentQuickInfoSessionReturnsNoContentOrApplicableSpanWhenBufferIsEmpty()
        {
            var buffer = new FakeTextBuffer(string.Empty);
            var session = new FakeQuickInfoSession();
            using (var source = new TemplateQuickInfoSource(buffer))
            {
                var quickInfoContent = new List<object>();
                ITrackingSpan applicableToSpan;
                source.AugmentQuickInfoSession(session, quickInfoContent, out applicableToSpan);

                Assert.AreEqual(0, quickInfoContent.Count);
                Assert.IsNull(applicableToSpan);
            }
        }

        [TestMethod]
        public void AugmentQuickInfoSessionReturnsNoContentOrApplicableSpanSyntaxNodeAtTriggerPointHasNoDescription()
        {
            var buffer = new FakeTextBuffer("<# code #>");
            var session = new FakeQuickInfoSession { SnapshotTriggerPoint = new SnapshotPoint(buffer.CurrentSnapshot, 3) };
            using (var source = new TemplateQuickInfoSource(buffer))
            {
                var quickInfoContent = new List<object>();
                ITrackingSpan applicableToSpan;
                source.AugmentQuickInfoSession(session, quickInfoContent, out applicableToSpan);

                Assert.AreEqual(0, quickInfoContent.Count);
                Assert.IsNull(applicableToSpan);
            }
        }

        [TestMethod]
        public void AugmentQuickInfoSessionReturnsNoContentOrApplicableSpanWhenBufferContainsTemplateThatCouldNotBeParsed()
        {
            var buffer = new FakeTextBuffer("<#");
            var session = new FakeQuickInfoSession { SnapshotTriggerPoint = new SnapshotPoint(buffer.CurrentSnapshot, 1) };
            using (var source = new TemplateQuickInfoSource(buffer))
            {
                var quickInfoContent = new List<object>();
                ITrackingSpan applicableToSpan;
                source.AugmentQuickInfoSession(session, quickInfoContent, out applicableToSpan);

                Assert.AreEqual(0, quickInfoContent.Count);
                Assert.IsNull(applicableToSpan);
            }
        }

        [TestMethod]
        public void AugmentQuickInfoSessionReturnsDescriptionOfSyntaxNodeAtTriggerPoint()
        {
            var buffer = new FakeTextBuffer("<#@ assembly name=\"System\" #>");
            var session = new FakeQuickInfoSession { SnapshotTriggerPoint = new SnapshotPoint(buffer.CurrentSnapshot, 5) };
            using (var source = new TemplateQuickInfoSource(buffer))
            {
                var quickInfoContent = new List<object>();
                ITrackingSpan applicableToSpan;
                source.AugmentQuickInfoSession(session, quickInfoContent, out applicableToSpan);

                StringAssert.Contains((string)quickInfoContent[0], "assembly");
            }
        }

        [TestMethod]
        public void AugmentQuickInfoSessionReturnsSpanOfSyntaxNodeProvidingDescription()
        {
            var buffer = new FakeTextBuffer("<#@ assembly name=\"System\" #>");
            var session = new FakeQuickInfoSession { SnapshotTriggerPoint = new SnapshotPoint(buffer.CurrentSnapshot, 15) };
            using (var source = new TemplateQuickInfoSource(buffer))
            {
                var quickInfoContent = new List<object>();
                ITrackingSpan applicableToSpan;
                source.AugmentQuickInfoSession(session, quickInfoContent, out applicableToSpan);

                Assert.AreEqual(new Span(13, 13), applicableToSpan.GetSpan(buffer.CurrentSnapshot).Span);
            }
        }
    }
}