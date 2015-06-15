// <copyright file="TemplateErrorTaggerTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.Editor
{
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Tagging;

    [TestClass]
    public class TemplateErrorTaggerTest
    {
        [TestMethod]
        public void TemplateErrorTaggerIsInternalAndNotIntendedForConsumptionOutsideOfT4Toolbox()
        {
            Assert.IsTrue(typeof(TemplateErrorTagger).IsNotPublic);
        }

        [TestMethod]
        public void TemplateErrorTaggerIsSealedAndNotIntendedHaveChildClasses()
        {
            Assert.IsTrue(typeof(TemplateErrorTagger).IsSealed);
        }

        [TestMethod]
        public void GetTagsReturnsErrorSpanForSyntaxError()
        {
            var buffer = new FakeTextBuffer("<#");
            var tagger = new TemplateErrorTagger(buffer);

            var snapshotSpans = new NormalizedSnapshotSpanCollection(new SnapshotSpan(buffer.CurrentSnapshot, 0, buffer.CurrentSnapshot.Length));
            ITagSpan<ErrorTag> errorSpan = tagger.GetTags(snapshotSpans).Single();

            Assert.AreEqual(new Span(2, 0), errorSpan.Span);
            StringAssert.Contains((string)errorSpan.Tag.ToolTipContent, "#>");            
        }

        [TestMethod]
        public void GetTagsReturnsErrorSpanForSemanticError()
        {
            var buffer = new FakeTextBuffer("<#@ include file=\" \" #>");
            var tagger = new TemplateErrorTagger(buffer);

            var snapshotSpans = new NormalizedSnapshotSpanCollection(new SnapshotSpan(buffer.CurrentSnapshot, 0, buffer.CurrentSnapshot.Length));
            ITagSpan<ErrorTag> errorSpan = tagger.GetTags(snapshotSpans).Single();

            Assert.AreEqual(new Span(12, 8), errorSpan.Span);
            StringAssert.Contains((string)errorSpan.Tag.ToolTipContent, "File");                        
        }
    }
}