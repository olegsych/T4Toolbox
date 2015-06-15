// <copyright file="TemplateClassificationTaggerProviderTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.Editor
{
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Text.Tagging;
    using Microsoft.VisualStudio.Utilities;
    using NSubstitute;
    using Xunit;

    public class TemplateClassificationTaggerProviderTest
    {
        [Fact]
        public void TemplateClassificationTaggerProviderIsInternalAndNotMeantForPublicConsumption()
        {
            Assert.False(typeof(TemplateClassificationTaggerProvider).IsPublic);
        }

        [Fact]
        public void TemplateClassificationTaggerProviderIsSealedAndNotMeantToHaveChildClasses()
        {
            Assert.True(typeof(TemplateClassificationTaggerProvider).IsSealed);
        }

        [Fact]
        public void TemplateClassificationTaggerProviderExportsITaggerProvider()
        {
            var export = (ExportAttribute)typeof(TemplateClassificationTaggerProvider).GetCustomAttributes(typeof(ExportAttribute), false)[0];
            Assert.Equal(typeof(ITaggerProvider), export.ContractType);
        }

        [Fact]
        public void TemplateClassificationTaggerProviderSpecifiesClassificationTagType()
        {
            var tagType = (TagTypeAttribute)typeof(TemplateClassificationTaggerProvider).GetCustomAttributes(typeof(TagTypeAttribute), false)[0];
            Assert.Equal(typeof(ClassificationTag), tagType.TagTypes);
        }

        [Fact]
        public void TemplateClassificationTaggerProviderAppliesOnlyToTextTemplateContentType()
        {
            var contentType = (ContentTypeAttribute)typeof(TemplateClassificationTaggerProvider).GetCustomAttributes(typeof(ContentTypeAttribute), false)[0];
            Assert.Equal(TemplateContentType.Name, contentType.ContentTypes);
        }

        [Fact]
        public void CreateTaggerReturnsTemplateClassificationTagger()
        {
            ITemplateEditorOptions options = OptionsWithSyntaxColorizationEnabled(true);
            var target = new TemplateClassificationTaggerProvider(options);
            target.ClassificationRegistry = new FakeClassificationTypeRegistryService();
            Assert.NotNull(target.CreateTagger<ClassificationTag>(new FakeTextBuffer(string.Empty)));
        }

        [Fact]
        public void CreateTaggerReturnsSameTaggerWhenCalledMultipleTimesForSameBuffer()
        {
            ITemplateEditorOptions options = OptionsWithSyntaxColorizationEnabled(true);
            var target = new TemplateClassificationTaggerProvider(options);
            target.ClassificationRegistry = new FakeClassificationTypeRegistryService();
            var buffer = new FakeTextBuffer(string.Empty);
            var tagger1 = target.CreateTagger<ClassificationTag>(buffer);
            var tagger2 = target.CreateTagger<ClassificationTag>(buffer);
            Assert.Same(tagger1, tagger2);
        }

        [Fact]
        public void CreateTaggerDoesNotReturnTaggerWhenSyntaxColorizationIsDisabled()
        {
            ITemplateEditorOptions options = OptionsWithSyntaxColorizationEnabled(false);
            var target = new TemplateClassificationTaggerProvider(options);
            target.ClassificationRegistry = new FakeClassificationTypeRegistryService();

            Assert.Null(target.CreateTagger<ClassificationTag>(new FakeTextBuffer(string.Empty)));
        }

        private static ITemplateEditorOptions OptionsWithSyntaxColorizationEnabled(bool enabled)
        {
            var options = Substitute.For<ITemplateEditorOptions>();
            options.SyntaxColorizationEnabled.Returns(enabled);
            return options;
        }
    }
}