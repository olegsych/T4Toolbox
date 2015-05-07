// <copyright file="TemplateCompletionSourceProviderTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.Editor
{
    using System.ComponentModel.Composition;
    using System.Linq;
    using Microsoft.VisualStudio.Language.Intellisense;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.VisualStudio.Utilities;
    using T4Toolbox.VisualStudio.Tests.Fakes;

    [TestClass]
    public class TemplateCompletionSourceProviderTest
    {
        [TestMethod]
        public void TemplateCompletionSourceProviderIsInternalAndNotMeantForPublicConsumption()
        {
            Assert.IsFalse(typeof(TemplateCompletionSourceProvider).IsPublic);
        }

        [TestMethod]
        public void TemplateCompletionSourceProviderIsSealedAndNotMeantToHaveChildClasses()
        {
            Assert.IsTrue(typeof(TemplateCompletionSourceProvider).IsSealed);
        }

        [TestMethod]
        public void TemplateCompletionSourceProviderImplementsICompletionSourceProviderInterfaceExpectedByVisualStudioEditor()
        {
            Assert.AreEqual(typeof(ICompletionSourceProvider), typeof(TemplateCompletionSourceProvider).GetInterfaces()[0]);
        }

        [TestMethod]
        public void TemplateCompletionSourceProviderExportsICompletionSourceProviderInterfaceExpectedByVisualStudioEditor()
        {
            Assert.AreEqual(typeof(ICompletionSourceProvider), typeof(TemplateCompletionSourceProvider).GetCustomAttributes(false).OfType<ExportAttribute>().Single().ContractType);
        }

        [TestMethod]
        public void TemplateCompletionSourceProviderSpecifiesTextTemplateContentType()
        {
            Assert.AreEqual(TemplateContentType.Name, typeof(TemplateCompletionSourceProvider).GetCustomAttributes(false).OfType<ContentTypeAttribute>().Single().ContentTypes);
        }

        [TestMethod]
        public void TemplateCompletionSourceProviderSpecifiesNameRequiredByVisualStudioEditor()
        {
            Assert.IsFalse(string.IsNullOrEmpty(typeof(TemplateCompletionSourceProvider).GetCustomAttributes(false).OfType<NameAttribute>().Single().Name));
        }

        [TestMethod]
        public void TryCreateCompletionSourceReturnsTemplateCompletionSource()
        {
            var provider = new TemplateCompletionSourceProvider();
            var buffer = new FakeTextBuffer(string.Empty);
            Assert.IsNotNull(provider.TryCreateCompletionSource(buffer));
        }

        [TestMethod]
        public void TryCreateCompletionSourceReturnsSameCompletionSourceForSameBuffer()
        {
            var provider = new TemplateCompletionSourceProvider();
            var buffer = new FakeTextBuffer(string.Empty);
            var completionSource1 = provider.TryCreateCompletionSource(buffer);
            var completionSource2 = provider.TryCreateCompletionSource(buffer);
            Assert.AreSame(completionSource1, completionSource2);
        }
    }
}