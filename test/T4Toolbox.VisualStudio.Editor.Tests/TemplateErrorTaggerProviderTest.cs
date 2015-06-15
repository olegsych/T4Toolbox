// <copyright file="TemplateErrorTaggerProviderTest.cs" company="Oleg Sych">
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
    public class TemplateErrorTaggerProviderTest
    {
        [TestMethod]
        public void TemplateErrorTaggerProviderIsInternalAndNotMeantForPublicConsumption()
        {
            Assert.IsFalse(typeof(TemplateErrorTaggerProvider).IsPublic);
        }

        [TestMethod]
        public void TemplateErrorTaggerProviderIsSealedAndNotMeantToHaveChildClasses()
        {
            Assert.IsTrue(typeof(TemplateErrorTaggerProvider).IsSealed);
        }

        [TestMethod]
        public void TemplateErrorTaggerProviderExportsITaggerProvider()
        {
            var export = (ExportAttribute)typeof(TemplateErrorTaggerProvider).GetCustomAttributes(typeof(ExportAttribute), false)[0];
            Assert.AreEqual(typeof(ITaggerProvider), export.ContractType);
        }

        [TestMethod]
        public void TemplateErrorTaggerProviderSpecifiesErrorTagType()
        {
            var tagType = (TagTypeAttribute)typeof(TemplateErrorTaggerProvider).GetCustomAttributes(typeof(TagTypeAttribute), false)[0];
            Assert.AreEqual(typeof(ErrorTag), tagType.TagTypes);
        }

        [TestMethod]
        public void TemplateErrorTaggerProviderAppliesOnlyToTextTemplateContentType()
        {
            var contentType = (ContentTypeAttribute)typeof(TemplateErrorTaggerProvider).GetCustomAttributes(typeof(ContentTypeAttribute), false)[0];
            Assert.AreEqual(TemplateContentType.Name, contentType.ContentTypes);
        }

        [TestMethod]
        public void CreateTaggerReturnsTemplateErrorTagger()
        {
            ITemplateEditorOptions options = OptionsWithErrorUnderliningEnabled(true);
            var target = new TemplateErrorTaggerProvider(options);
            Assert.IsNotNull(target.CreateTagger<ErrorTag>(new FakeTextBuffer(string.Empty)));
        }

        [TestMethod]
        public void CreateTaggerReturnsSameTaggerWhenCalledMultipleTimesForSameBuffer()
        {
            ITemplateEditorOptions options = OptionsWithErrorUnderliningEnabled(true);
            var target = new TemplateErrorTaggerProvider(options);
            var buffer = new FakeTextBuffer(string.Empty);
            var tagger1 = target.CreateTagger<ErrorTag>(buffer);
            var tagger2 = target.CreateTagger<ErrorTag>(buffer);
            Assert.AreSame(tagger1, tagger2);
        }

        [TestMethod]
        public void CreateTaggerDoesNotReturnTaggerWhenErrorUnderliningIsDisabled()
        {
            ITemplateEditorOptions options = OptionsWithErrorUnderliningEnabled(false);
            var target = new TemplateErrorTaggerProvider(options);
            Assert.IsNull(target.CreateTagger<ErrorTag>(new FakeTextBuffer(string.Empty)));
        }

        private static ITemplateEditorOptions OptionsWithErrorUnderliningEnabled(bool enabled)
        {
            var options = Substitute.For<ITemplateEditorOptions>();
            options.ErrorUnderliningEnabled.Returns(enabled);
            return options;
        }
    }
}