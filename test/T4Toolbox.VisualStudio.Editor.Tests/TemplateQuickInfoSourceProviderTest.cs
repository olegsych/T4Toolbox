// <copyright file="TemplateQuickInfoSourceProviderTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.Editor
{
    using System.ComponentModel.Composition;
    using System.Linq;
    using System.Reflection;
    using Microsoft.VisualStudio.Language.Intellisense;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.VisualStudio.Utilities;
    using NSubstitute;

    [TestClass]
    public class TemplateQuickInfoSourceProviderTest
    {
        [TestMethod]
        public void TemplateQuickInfoSourceProviderIsInternalAndNotMeantForPublicConsumption()
        {
            Assert.IsFalse(typeof(TemplateQuickInfoSourceProvider).IsPublic);
        }

        [TestMethod]
        public void TemplateQuickInfoSourceProviderIsSealedAndNotMeantToHaveChildClasses()
        {
            Assert.IsTrue(typeof(TemplateQuickInfoSourceProvider).IsSealed);
        }

        [TestMethod]
        public void TemplateQuickInfoSourceProviderImplementsIQuickSourceProviderInterface()
        {
            Assert.AreEqual(typeof(IQuickInfoSourceProvider), typeof(TemplateQuickInfoSourceProvider).GetInterfaces()[0]);
        }

        [TestMethod]
        public void TemplateQuickInfoSourceProviderExportsIQuickSourceProviderInterface()
        {
            ExportAttribute attribute = typeof(TemplateQuickInfoSourceProvider).GetCustomAttributes(false).OfType<ExportAttribute>().Single();
            Assert.AreEqual(typeof(IQuickInfoSourceProvider), attribute.ContractType);
        }

        [TestMethod]
        public void TemplateQuickInfoSourceProviderSpecifiesTextTemplateContentType()
        {
            ContentTypeAttribute attribute = typeof(TemplateQuickInfoSourceProvider).GetCustomAttributes(false).OfType<ContentTypeAttribute>().Single();
            Assert.AreEqual(TemplateContentType.Name, attribute.ContentTypes);
        }

        [TestMethod]
        public void TemplateQuickInfoSourceProviderSpecifiesNameAttributeRequiredByVisualStudio()
        {
            Assert.AreEqual(1, typeof(TemplateQuickInfoSourceProvider).GetCustomAttributes(false).OfType<NameAttribute>().Count());
        }

        [TestMethod]
        public void TemplateQuickInfoSourceProviderSpecifiesOrderAttributeRequiredByVisualStudio()
        {
            Assert.AreEqual(1, typeof(TemplateQuickInfoSourceProvider).GetCustomAttributes(false).OfType<OrderAttribute>().Count());
        }

        [TestMethod]
        public void TryCreateQuickInfoSourceReturnsTemplateQuickInfoSource()
        {
            ITemplateEditorOptions options = OptionsWithQuickInfoTooltipsEnabled(true);
            var provider = new TemplateQuickInfoSourceProvider(options);
            var textBuffer = new FakeTextBuffer(string.Empty);
            IQuickInfoSource quickInfoSource = provider.TryCreateQuickInfoSource(textBuffer);
            Assert.AreEqual(typeof(TemplateQuickInfoSource), quickInfoSource.GetType());
        }

        [TestMethod]
        public void TryCreateQuickInfoSourceReturnsSameObjectWhenCalledMultipleTimesForSameBuffer()
        {
            ITemplateEditorOptions options = OptionsWithQuickInfoTooltipsEnabled(true);
            var provider = new TemplateQuickInfoSourceProvider(options);
            var textBuffer = new FakeTextBuffer(string.Empty);
            IQuickInfoSource source1 = provider.TryCreateQuickInfoSource(textBuffer);
            IQuickInfoSource source2 = provider.TryCreateQuickInfoSource(textBuffer);
            Assert.AreSame(source1, source2);
        }

        [TestMethod]
        public void TryCreateQuickInfoSourceReturnsNullWhenQuickInfoTooltipsAreDisabled()
        {
            ITemplateEditorOptions options = OptionsWithQuickInfoTooltipsEnabled(false);
            var provider = new TemplateQuickInfoSourceProvider(options);
            Assert.IsNull(provider.TryCreateQuickInfoSource(new FakeTextBuffer(string.Empty)));
        }

        private static ITemplateEditorOptions OptionsWithQuickInfoTooltipsEnabled(bool enabled)
        {
            var options = Substitute.For<ITemplateEditorOptions>();
            options.QuickInfoTooltipsEnabled.Returns(enabled);
            return options;
        }
    }
}