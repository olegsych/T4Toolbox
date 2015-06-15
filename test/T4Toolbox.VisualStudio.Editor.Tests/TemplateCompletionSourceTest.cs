// <copyright file="TemplateCompletionSourceTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.Editor
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.VisualStudio.Language.Intellisense;
    using Microsoft.VisualStudio.Text;
    using Xunit;

    public class TemplateCompletionSourceTest
    {
        [Fact]
        public void TemplateCompletionSourceIsInternalAndNotMeantForPublicConsumption()
        {
            Assert.False(typeof(TemplateCompletionSource).IsPublic);
        }

        [Fact]
        public void TemplateCompletionSourceIsSealedAndNotMeantToHaveChildClasses()
        {
            Assert.True(typeof(TemplateCompletionSource).IsSealed);
        }

        [Fact]
        public void TemplateCompletionSourceImplementsICompletionSourceExpectedByVisualStudioEditor()
        {
            Assert.Equal(typeof(ICompletionSource), typeof(TemplateCompletionSource).GetInterfaces()[0]);
        }

        [Fact]
        public void AugmentCompletionSessionReturnsCompletionSetWhenTriggerPointIsInsideSyntaxNodeThatSupportsCompletions()
        {
            IList<CompletionSet> completionSets = AugmentCompletionSession("<#@ t #>", 5);
            Assert.Equal(1, completionSets.Count);
        }

        [Fact]
        public void AugmentCompletionSessionSortsCompletionsByDisplayTextToMakeThemEasierToRead()
        {
            CompletionSet completionSet = AugmentCompletionSession("<#@ template culture=\"en\" #>", 23)[0];
            Assert.Equal(completionSet.Completions.OrderBy(c => c.DisplayText).ToList(), completionSet.Completions.ToList());
        }

        [Fact]
        public void AugmentCompletionSessionDoesNotReturnCompletionSetWhenTriggerPointIsInsideSyntaxNodeThatDoesNotSupportCompletions()
        {
            IList<CompletionSet> completionSets = AugmentCompletionSession("<#@ t #>", 1);
            Assert.Equal(0, completionSets.Count);
        }

        [Fact]
        public void AugmentCompletionSessionReturnsCompletionSetApplicableToTheSpanOfSyntaxNodeThatContainsTriggerPoint()
        {
            ITextSnapshot snapshot;
            CompletionSet completionSet = AugmentCompletionSession("<#@ t #>", 5, out snapshot).Single();
            Assert.Equal(new Span(4, 1), completionSet.ApplicableTo.GetSpan(snapshot).Span);
        }

        [Fact]
        public void DisposeRemovesCompletionSourceFromTextBufferProperties()
        {
            ITextBuffer buffer = new FakeTextBuffer(string.Empty);
            var completionSource = buffer.Properties.GetOrCreateSingletonProperty(() => new TemplateCompletionSource(buffer));
            completionSource.Dispose();
            Assert.False(buffer.Properties.ContainsProperty(typeof(TemplateCompletionSource)));
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