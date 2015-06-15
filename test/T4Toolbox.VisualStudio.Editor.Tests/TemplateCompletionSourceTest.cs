// <copyright file="TemplateCompletionSourceTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.Editor
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.VisualStudio.Language.Intellisense;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.VisualStudio.Text;

    [TestClass]
    public class TemplateCompletionSourceTest
    {
        [TestMethod]
        public void TemplateCompletionSourceIsInternalAndNotMeantForPublicConsumption()
        {
            Assert.IsFalse(typeof(TemplateCompletionSource).IsPublic);
        }

        [TestMethod]
        public void TemplateCompletionSourceIsSealedAndNotMeantToHaveChildClasses()
        {
            Assert.IsTrue(typeof(TemplateCompletionSource).IsSealed);
        }

        [TestMethod]
        public void TemplateCompletionSourceImplementsICompletionSourceExpectedByVisualStudioEditor()
        {
            Assert.AreEqual(typeof(ICompletionSource), typeof(TemplateCompletionSource).GetInterfaces()[0]);
        }

        [TestMethod]
        public void AugmentCompletionSessionReturnsCompletionSetWhenTriggerPointIsInsideSyntaxNodeThatSupportsCompletions()
        {
            IList<CompletionSet> completionSets = AugmentCompletionSession("<#@ t #>", 5);
            Assert.AreEqual(1, completionSets.Count);
        }

        [TestMethod]
        public void AugmentCompletionSessionSortsCompletionsByDisplayTextToMakeThemEasierToRead()
        {
            CompletionSet completionSet = AugmentCompletionSession("<#@ template culture=\"en\" #>", 23)[0];
            CollectionAssert.AreEqual(completionSet.Completions.OrderBy(c => c.DisplayText).ToList(), completionSet.Completions.ToList());
        }

        [TestMethod]
        public void AugmentCompletionSessionDoesNotReturnCompletionSetWhenTriggerPointIsInsideSyntaxNodeThatDoesNotSupportCompletions()
        {
            IList<CompletionSet> completionSets = AugmentCompletionSession("<#@ t #>", 1);
            Assert.AreEqual(0, completionSets.Count);
        }

        [TestMethod]
        public void AugmentCompletionSessionReturnsCompletionSetApplicableToTheSpanOfSyntaxNodeThatContainsTriggerPoint()
        {
            ITextSnapshot snapshot;
            CompletionSet completionSet = AugmentCompletionSession("<#@ t #>", 5, out snapshot).Single();
            Assert.AreEqual(new Span(4, 1), completionSet.ApplicableTo.GetSpan(snapshot).Span);
        }

        [TestMethod]
        public void DisposeRemovesCompletionSourceFromTextBufferProperties()
        {
            ITextBuffer buffer = new FakeTextBuffer(string.Empty);
            var completionSource = buffer.Properties.GetOrCreateSingletonProperty(() => new TemplateCompletionSource(buffer));
            completionSource.Dispose();
            Assert.IsFalse(buffer.Properties.ContainsProperty(typeof(TemplateCompletionSource)));
        }

        private static IList<CompletionSet> AugmentCompletionSession(string text, int triggerPosition)
        {
            ITextSnapshot snapshot;
            return AugmentCompletionSession(text, triggerPosition, out snapshot);
        }

        private static IList<CompletionSet> AugmentCompletionSession(string text, int triggerPosition, out ITextSnapshot snapshot)
        {
            var buffer = new FakeTextBuffer(text);
            var completionSession = new FakeCompletionSession { TriggerPosition = triggerPosition };
            using (var completionSource = new TemplateCompletionSource(buffer))
            {
                var completionSets = new List<CompletionSet>();
                completionSource.AugmentCompletionSession(completionSession, completionSets);
                snapshot = buffer.CurrentSnapshot;
                return completionSets;
            }
        }
    }
}