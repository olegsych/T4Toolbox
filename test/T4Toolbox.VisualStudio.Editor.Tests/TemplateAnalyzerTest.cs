// <copyright file="TemplateAnalyzerTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.Editor
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.VisualStudio.Text;
    using Template = T4Toolbox.TemplateAnalysis.Template;

    [TestClass]
    public class TemplateAnalyzerTest
    {
        [TestMethod]
        public void TemplateAnalyzerIsInternalAndNotIntendedForConsumptionOutsideOfT4Toolbox()
        {
            Assert.IsTrue(typeof(TemplateAnalyzer).IsNotPublic);
        }

        [TestMethod]
        public void TemplateAnalyzerIsSealedAndNotIntendedToHaveChildClasses()
        {
            Assert.IsTrue(typeof(TemplateAnalyzer).IsSealed);
        }

        [TestMethod]
        public void CurrentAnalysisReturnsLastTemplateAnalysisResult()
        {
            var buffer = new FakeTextBuffer("<#@");
            var target = TemplateAnalyzer.GetOrCreate(buffer);
            TemplateAnalysis result = target.CurrentAnalysis;
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void CurrentAnalysisReturnsSyntaxErrorsDetectedInTextBuffer()
        {
            var buffer = new FakeTextBuffer("<#@");
            var target = TemplateAnalyzer.GetOrCreate(buffer);
            Assert.AreEqual(1, target.CurrentAnalysis.Errors.Count);
        }

        [TestMethod]
        public void CurrentAnalysisReturnsSemanticErrorsDetectedInTextBuffer()
        {
            var buffer = new FakeTextBuffer("<#@ template bad=\"puppy\" #>");
            var target = TemplateAnalyzer.GetOrCreate(buffer);
            Assert.AreEqual(1, target.CurrentAnalysis.Errors.Count);
        }

        [TestMethod]
        public void CurrentAnalysisReturnsUpdatedErrorsWhenTextBufferChanges()
        {
            var buffer = new FakeTextBuffer("<#@");
            var target = TemplateAnalyzer.GetOrCreate(buffer);
            Assert.AreEqual(1, target.CurrentAnalysis.Errors.Count); // Need to touch lazy property before buffer change for test to be valid 

            buffer.CurrentSnapshot = new FakeTextSnapshot(string.Empty);
            Assert.AreEqual(0, target.CurrentAnalysis.Errors.Count);
        }

        [TestMethod]
        public void CurrentAnalysisReturnsTemplateParsedFromTextBuffer()
        {
            var buffer = new FakeTextBuffer("<#@ template language=\"VB\" #>");
            var target = TemplateAnalyzer.GetOrCreate(buffer);
            Template template = target.CurrentAnalysis.Template;
            Assert.IsNotNull(template);
            Assert.AreEqual(1, template.ChildNodes().Count());
        }

        [TestMethod]
        public void CurrentAnalysisReturnsDefaultTemplateIfParserCouldNotCreateOne()
        {
            var buffer = new FakeTextBuffer("<#@ t");
            var target = TemplateAnalyzer.GetOrCreate(buffer);
            Template template = target.CurrentAnalysis.Template;
            Assert.IsNotNull(template);
            Assert.AreEqual(0, template.ChildNodes().Count());
        }

        [TestMethod]
        public void CurrentAnalysisReturnsUpdatedTemplateWhenTextBufferChanges()
        {
            var buffer = new FakeTextBuffer("<#@ template language=\"VB\" #>");
            var target = TemplateAnalyzer.GetOrCreate(buffer);
            Assert.AreEqual(1, target.CurrentAnalysis.Template.ChildNodes().Count()); // Need to touch lazy Template before buffer change for test to be valid

            buffer.CurrentSnapshot = new FakeTextSnapshot(string.Empty);
            Assert.AreEqual(0, target.CurrentAnalysis.Template.ChildNodes().Count());
        }

        [TestMethod]
        public void CurrentAnalysisReturnsTextSnapshotForWhichItWasCreated()
        {
            var buffer = new FakeTextBuffer(string.Empty);
            var analyzer = TemplateAnalyzer.GetOrCreate(buffer);
            ITextSnapshot analysisSnapshot = analyzer.CurrentAnalysis.TextSnapshot;
            Assert.AreSame(buffer.CurrentSnapshot, analysisSnapshot);
        }

        [TestMethod]
        public void GetOrCreateReturnsNewTemplateAnalyzerFirstTimeItIsRequestedForTextBuffer()
        {
            ITextBuffer buffer = new FakeTextBuffer(string.Empty);
            TemplateAnalyzer analyzer = TemplateAnalyzer.GetOrCreate(buffer);
            Assert.IsNotNull(analyzer);
        }

        [TestMethod]
        public void GetOrCreateReturnsExistingTemplateAnalyzerSecondTimeItIsRequestedForTextBuffer()
        {
            ITextBuffer buffer = new FakeTextBuffer(string.Empty);
            TemplateAnalyzer analyzer1 = TemplateAnalyzer.GetOrCreate(buffer);
            TemplateAnalyzer analyzer2 = TemplateAnalyzer.GetOrCreate(buffer);
            Assert.AreSame(analyzer1, analyzer2);
        }

        [TestMethod, SuppressMessage("Microsoft.Reliability", "CA2001:AvoidCallingProblematicMethods", MessageId = "System.GC.Collect", Justification = "This is a test of garbage collection")]
        public void GetOrCreateDoesNotPreventGarbageCollectionOfPreviouslyCreatedTemplateAnalyzers()
        {
            var analyzer = new WeakReference(TemplateAnalyzer.GetOrCreate(new FakeTextBuffer(string.Empty)));

            GC.Collect(2, GCCollectionMode.Forced);
            GC.WaitForPendingFinalizers();

            Assert.IsFalse(analyzer.IsAlive);
        }

        [TestMethod, SuppressMessage("Microsoft.Reliability", "CA2001:AvoidCallingProblematicMethods", MessageId = "System.GC.Collect", Justification = "This is a test of garbage collection")]
        public void GetOrCreateDoesNotPreventGarbageCollectionOfTextBuffers()
        {
            var buffer = new WeakReference(new FakeTextBuffer(string.Empty));
            TemplateAnalyzer.GetOrCreate((ITextBuffer)buffer.Target);

            GC.Collect(2, GCCollectionMode.Forced);
            GC.WaitForPendingFinalizers();

            Assert.IsFalse(buffer.IsAlive);
        }

        [TestMethod]
        public void TemplateChangedEventIsRaisedWhenTextBufferChanges()
        {
            var buffer = new FakeTextBuffer(string.Empty);
            var target = TemplateAnalyzer.GetOrCreate(buffer);
            bool templateChanged = false;
            target.TemplateChanged += (sender, args) => { templateChanged = true; };
            buffer.CurrentSnapshot = new FakeTextSnapshot("42");
            Assert.IsTrue(templateChanged);
        }

        [TestMethod]
        public void TemplateChangeEventArgumentSuppliesCurrentTemplateAnalysis()
        {
            var buffer = new FakeTextBuffer(string.Empty);
            var target = TemplateAnalyzer.GetOrCreate(buffer);
            TemplateAnalysis eventArgument = null;
            target.TemplateChanged += (sender, args) => { eventArgument = args; };
            buffer.CurrentSnapshot = new FakeTextSnapshot("42");
            Assert.AreSame(target.CurrentAnalysis, eventArgument);
        }
    }
}