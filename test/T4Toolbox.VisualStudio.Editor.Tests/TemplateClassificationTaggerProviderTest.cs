// <copyright file="TemplateClassificationTaggerProviderTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.Editor
{
    using System;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using Microsoft.VisualStudio.Text.Classification;
    using Microsoft.VisualStudio.Text.Tagging;
    using Microsoft.VisualStudio.Utilities;
    using NSubstitute;
    using Xunit;

    public static class TemplateClassificationTaggerProviderTest
    {
        [Fact]
        public static void TemplateClassificationTaggerProviderIsInternalAndNotMeantForPublicConsumption()
        {
            Assert.False(typeof(TemplateClassificationTaggerProvider).IsPublic);
        }

        [Fact]
        public static void TemplateClassificationTaggerProviderIsSealedAndNotMeantToHaveChildClasses()
        {
            Assert.True(typeof(TemplateClassificationTaggerProvider).IsSealed);
        }

        [Fact]
        public static void TemplateClassificationTaggerProviderExportsITaggerProvider()
        {
            var export = (ExportAttribute)typeof(TemplateClassificationTaggerProvider).GetCustomAttributes(typeof(ExportAttribute), false)[0];
            Assert.Equal(typeof(ITaggerProvider), export.ContractType);
        }

        [Fact]
        public static void TemplateClassificationTaggerProviderSpecifiesClassificationTagType()
        {
            var tagType = (TagTypeAttribute)typeof(TemplateClassificationTaggerProvider).GetCustomAttributes(typeof(TagTypeAttribute), false)[0];
            Assert.Equal(typeof(ClassificationTag), tagType.TagTypes);
        }

        [Fact]
        public static void TemplateClassificationTaggerProviderAppliesOnlyToTextTemplateContentType()
        {
            var contentType = (ContentTypeAttribute)typeof(TemplateClassificationTaggerProvider).GetCustomAttributes(typeof(ContentTypeAttribute), false)[0];
            Assert.Equal(TemplateContentType.Name, contentType.ContentTypes);
        }

        [Fact]
        public static void TemplateClassificationTaggerProviderCanBeConstructedByVisualStudio()
        {
            using (var catalog = new TypeCatalog(
                typeof(TemplateClassificationTaggerProvider),
                typeof(SubstituteExporter<ITemplateEditorOptions>),
                typeof(SubstituteExporter<IClassificationTypeRegistryService>)))
            using (var container = new CompositionContainer(catalog))
            {
                Lazy<ITaggerProvider> export = container.GetExport<ITaggerProvider>();
                Assert.IsType<TemplateClassificationTaggerProvider>(export.Value);
            }
        }

        [Fact]
        public static void ConstructorThrowsArgumentNullExceptionWhenOptionsIsNullToFailFast()
        {
            var e = Assert.Throws<ArgumentNullException>(() => new TemplateClassificationTaggerProvider(null, Substitute.For<IClassificationTypeRegistryService>()));
            Assert.Equal("options", e.ParamName);
        }

        [Fact]
        public static void ConstructorThrowsArgumentNullExceptionWhenClassificationTypeRegistryIsNullToFailFast()
        {
            var e = Assert.Throws<ArgumentNullException>(() => new TemplateClassificationTaggerProvider(Substitute.For<ITemplateEditorOptions>(), null));
            Assert.Equal("classificationTypeRegistry", e.ParamName);
        }

        [Fact]
        public static void CreateTaggerReturnsTemplateClassificationTagger()
        {
            ITemplateEditorOptions options = OptionsWithSyntaxColorizationEnabled(true);
            var target = new TemplateClassificationTaggerProvider(options, new FakeClassificationTypeRegistryService());
            Assert.NotNull(target.CreateTagger<ClassificationTag>(new FakeTextBuffer(string.Empty)));
        }

        [Fact]
        public static void CreateTaggerReturnsSameTaggerWhenCalledMultipleTimesForSameBuffer()
        {
            ITemplateEditorOptions options = OptionsWithSyntaxColorizationEnabled(true);
            var target = new TemplateClassificationTaggerProvider(options, new FakeClassificationTypeRegistryService());
            var buffer = new FakeTextBuffer(string.Empty);
            var tagger1 = target.CreateTagger<ClassificationTag>(buffer);
            var tagger2 = target.CreateTagger<ClassificationTag>(buffer);
            Assert.Same(tagger1, tagger2);
        }

        [Fact]
        public static void CreateTaggerDoesNotReturnTaggerWhenSyntaxColorizationIsDisabled()
        {
            ITemplateEditorOptions options = OptionsWithSyntaxColorizationEnabled(false);
            var target = new TemplateClassificationTaggerProvider(options, new FakeClassificationTypeRegistryService());

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