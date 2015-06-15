// <copyright file="TemplateOutliningTaggerProviderTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.Editor
{
    using System.ComponentModel.Composition;
    using System.Reflection;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.VisualStudio.Text.Tagging;
    using Microsoft.VisualStudio.Utilities;
    using NSubstitute;

    [TestClass]
    public class TemplateOutliningTaggerProviderTest
    {
        [TestMethod]
        public void TemplateOutliningTaggerProviderIsInternalAndNotMeantForPublicConsumption()
        {
            Assert.IsFalse(typeof(TemplateOutliningTaggerProvider).IsPublic);
        }

        [TestMethod]
        public void TemplateOutliningTaggerProviderIsSealedAndNotMeantToHaveChildClasses()
        {
            Assert.IsTrue(typeof(TemplateOutliningTaggerProvider).IsSealed);
        }

        [TestMethod]
        public void TemplateOutliningTaggerProviderImplementsITaggerProvider()
        {
            Assert.AreEqual(typeof(ITaggerProvider), typeof(TemplateOutliningTaggerProvider).GetInterfaces()[0]);
        }

        [TestMethod]
        public void TemplateOutliningTaggerProviderExportsITaggerProvider()
        {
            var export = (ExportAttribute)typeof(TemplateOutliningTaggerProvider).GetCustomAttributes(typeof(ExportAttribute), false)[0];
            Assert.AreEqual(typeof(ITaggerProvider), export.ContractType);
        }

        [TestMethod]
        public void TemplateOutliningTaggerProviderExportSpecifiesOutliningTagType()
        {
            var tagType = (TagTypeAttribute)typeof(TemplateOutliningTaggerProvider).GetCustomAttributes(typeof(TagTypeAttribute), false)[0];
            Assert.AreEqual(typeof(OutliningRegionTag), tagType.TagTypes);
        }

        [TestMethod]
        public void TemplateOutliningTaggerProviderAppliesOnlyToTextTemplateContentType()
        {
            var contentType = (ContentTypeAttribute)typeof(TemplateOutliningTaggerProvider).GetCustomAttributes(typeof(ContentTypeAttribute), false)[0];
            Assert.AreEqual(TemplateContentType.Name, contentType.ContentTypes);
        }

        [TestMethod]
        public void CreateTaggerReturnsTemplateOutliningTagger()
        {
            ITemplateEditorOptions options = OptionsWithTemplateOutliningEnabled(true);
            var provider = new TemplateOutliningTaggerProvider(options);
            var tagger = provider.CreateTagger<OutliningRegionTag>(new FakeTextBuffer(string.Empty));
            Assert.AreEqual(typeof(TemplateOutliningTagger), tagger.GetType());
        }

        [TestMethod]
        public void CreateTaggerReturnsSingleTaggerWhenCalledMultipleTimesForSameBuffer()
        {
            ITemplateEditorOptions options = OptionsWithTemplateOutliningEnabled(true);
            var provider = new TemplateOutliningTaggerProvider(options);
            var buffer = new FakeTextBuffer(string.Empty);
            var tagger1 = provider.CreateTagger<OutliningRegionTag>(buffer);
            var tagger2 = provider.CreateTagger<OutliningRegionTag>(buffer);
            Assert.AreSame(tagger1, tagger2);
        }

        [TestMethod]
        public void CreateTaggerDoesNotReturnTaggerWhenTemplateOutliningIsDisabled()
        {
            ITemplateEditorOptions options = OptionsWithTemplateOutliningEnabled(false);
            var provider = new TemplateOutliningTaggerProvider(options);
            Assert.IsNull(provider.CreateTagger<OutliningRegionTag>(new FakeTextBuffer(string.Empty)));
        }

        private static ITemplateEditorOptions OptionsWithTemplateOutliningEnabled(bool enabled)
        {
            var options = Substitute.For<ITemplateEditorOptions>();
            options.TemplateOutliningEnabled.Returns(enabled);
            return options;
        }
    }
}