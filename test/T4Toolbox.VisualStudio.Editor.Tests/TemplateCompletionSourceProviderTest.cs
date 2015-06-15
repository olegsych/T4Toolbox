// <copyright file="TemplateCompletionSourceProviderTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.Editor
{
    using System.ComponentModel.Composition;
    using System.Linq;
    using Microsoft.VisualStudio.Language.Intellisense;
    using Microsoft.VisualStudio.Utilities;
    using Xunit;

    public class TemplateCompletionSourceProviderTest
    {
        [Fact]
        public void TemplateCompletionSourceProviderIsInternalAndNotMeantForPublicConsumption()
        {
            Assert.False(typeof(TemplateCompletionSourceProvider).IsPublic);
        }

        [Fact]
        public void TemplateCompletionSourceProviderIsSealedAndNotMeantToHaveChildClasses()
        {
            Assert.True(typeof(TemplateCompletionSourceProvider).IsSealed);
        }

        [Fact]
        public void TemplateCompletionSourceProviderImplementsICompletionSourceProviderInterfaceExpectedByVisualStudioEditor()
        {
            Assert.Equal(typeof(ICompletionSourceProvider), typeof(TemplateCompletionSourceProvider).GetInterfaces()[0]);
        }

        [Fact]
        public void TemplateCompletionSourceProviderExportsICompletionSourceProviderInterfaceExpectedByVisualStudioEditor()
        {
            Assert.Equal(typeof(ICompletionSourceProvider), typeof(TemplateCompletionSourceProvider).GetCustomAttributes(false).OfType<ExportAttribute>().Single().ContractType);
        }

        [Fact]
        public void TemplateCompletionSourceProviderSpecifiesTextTemplateContentType()
        {
            Assert.Equal(TemplateContentType.Name, typeof(TemplateCompletionSourceProvider).GetCustomAttributes(false).OfType<ContentTypeAttribute>().Single().ContentTypes);
        }

        [Fact]
        public void TemplateCompletionSourceProviderSpecifiesNameRequiredByVisualStudioEditor()
        {
            Assert.False(string.IsNullOrEmpty(typeof(TemplateCompletionSourceProvider).GetCustomAttributes(false).OfType<NameAttribute>().Single().Name));
        }

        [Fact]
        public void TryCreateCompletionSourceReturnsTemplateCompletionSource()
        {
            var provider = new TemplateCompletionSourceProvider();
            var buffer = new FakeTextBuffer(string.Empty);
            Assert.NotNull(provider.TryCreateCompletionSource(buffer));
        }

        [Fact]
        public void TryCreateCompletionSourceReturnsSameCompletionSourceForSameBuffer()
        {
            var provider = new TemplateCompletionSourceProvider();
            var buffer = new FakeTextBuffer(string.Empty);
            var completionSource1 = provider.TryCreateCompletionSource(buffer);
            var completionSource2 = provider.TryCreateCompletionSource(buffer);
            Assert.Same(completionSource1, completionSource2);
        }
    }
}