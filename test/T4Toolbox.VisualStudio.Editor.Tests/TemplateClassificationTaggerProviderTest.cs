// <copyright file="TemplateClassificationTaggerProviderTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.Editor
{
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.VisualStudio.Text.Tagging;
    using Microsoft.VisualStudio.Utilities;
    using NSubstitute;

    [TestClass]
    public class TemplateClassificationTaggerProviderTest
    {
        [TestMethod]
        public void TemplateClassificationTaggerProviderIsInternalAndNotMeantForPublicConsumption()
        {
            Assert.IsFalse(typeof(TemplateClassificationTaggerProvider).IsPublic);
        }

        [TestMethod]
        public void TemplateClassificationTaggerProviderIsSealedAndNotMeantToHaveChildClasses()
        {
            Assert.IsTrue(typeof(TemplateClassificationTaggerProvider).IsSealed);
        }

        [TestMethod]
        public void TemplateClassificationTaggerProviderExportsITaggerProvider()
        {
            var export = (ExportAttribute)typeof(TemplateClassificationTaggerProvider).GetCustomAttributes(typeof(ExportAttribute), false)[0];
            Assert.AreEqual(typeof(ITaggerProvider), export.ContractType);
        }

        [TestMethod]
        public void TemplateClassificationTaggerProviderSpecifiesClassificationTagType()
        {
            var tagType = (TagTypeAttribute)typeof(TemplateClassificationTaggerProvider).GetCustomAttributes(typeof(TagTypeAttribute), false)[0];
            Assert.AreEqual(typeof(ClassificationTag), tagType.TagTypes);
        }

        [TestMethod]
        public void TemplateClassificationTaggerProviderAppliesOnlyToTextTemplateContentType()
        {
            var contentType = (ContentTypeAttribute)typeof(TemplateClassificationTaggerProvider).GetCustomAttributes(typeof(ContentTypeAttribute), false)[0];
            Assert.AreEqual(TemplateContentType.Name, contentType.ContentTypes);
        }

        [TestMethod]
        public void CreateTaggerReturnsTemplateClassificationTagger()
        {
            ITemplateEditorOptions options = OptionsWithSyntaxColorizationEnabled(true);
            var target = new TemplateClassificationTaggerProvider(options);
            target.ClassificationRegistry = new FakeClassificationTypeRegistryService();
            Assert.IsNotNull(target.CreateTagger<ClassificationTag>(new FakeTextBuffer(string.Empty)));
        }

        [TestMethod]
        public void CreateTaggerReturnsSameTaggerWhenCalledMultipleTimesForSameBuffer()
        {
            ITemplateEditorOptions options = OptionsWithSyntaxColorizationEnabled(true);
            var target = new TemplateClassificationTaggerProvider(options);
            target.ClassificationRegistry = new FakeClassificationTypeRegistryService();
            var buffer = new FakeTextBuffer(string.Empty);
            var tagger1 = target.CreateTagger<ClassificationTag>(buffer);
            var tagger2 = target.CreateTagger<ClassificationTag>(buffer);
            Assert.AreSame(tagger1, tagger2);
        }

        [TestMethod]
        public void CreateTaggerDoesNotReturnTaggerWhenSyntaxColorizationIsDisabled()
        {
            ITemplateEditorOptions options = OptionsWithSyntaxColorizationEnabled(false);
            var target = new TemplateClassificationTaggerProvider(options);
            target.ClassificationRegistry = new FakeClassificationTypeRegistryService();

            Assert.IsNull(target.CreateTagger<ClassificationTag>(new FakeTextBuffer(string.Empty)));
        }

        private static ITemplateEditorOptions OptionsWithSyntaxColorizationEnabled(bool enabled)
        {
            var options = Substitute.For<ITemplateEditorOptions>();
            options.SyntaxColorizationEnabled.Returns(enabled);
            return options;
        }
    }
}