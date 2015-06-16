// <copyright file="TemplateErrorTaggerProviderTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.Editor
{
    using System;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using Microsoft.VisualStudio.Text.Tagging;
    using Microsoft.VisualStudio.Utilities;
    using NSubstitute;
    using Xunit;

    public static class TemplateErrorTaggerProviderTest
    {
        [Fact]
        public static void TemplateErrorTaggerProviderIsInternalAndNotMeantForPublicConsumption()
        {
            Assert.False(typeof(TemplateErrorTaggerProvider).IsPublic);
        }

        [Fact]
        public static void TemplateErrorTaggerProviderIsSealedAndNotMeantToHaveChildClasses()
        {
            Assert.True(typeof(TemplateErrorTaggerProvider).IsSealed);
        }

        [Fact]
        public static void TemplateErrorTaggerProviderExportsITaggerProvider()
        {
            var export = (ExportAttribute)typeof(TemplateErrorTaggerProvider).GetCustomAttributes(typeof(ExportAttribute), false)[0];
            Assert.Equal(typeof(ITaggerProvider), export.ContractType);
        }

        [Fact]
        public static void TemplateErrorTaggerProviderSpecifiesErrorTagType()
        {
            var tagType = (TagTypeAttribute)typeof(TemplateErrorTaggerProvider).GetCustomAttributes(typeof(TagTypeAttribute), false)[0];
            Assert.Equal(typeof(ErrorTag), tagType.TagTypes);
        }

        [Fact]
        public static void TemplateErrorTaggerProviderAppliesOnlyToTextTemplateContentType()
        {
            var contentType = (ContentTypeAttribute)typeof(TemplateErrorTaggerProvider).GetCustomAttributes(typeof(ContentTypeAttribute), false)[0];
            Assert.Equal(TemplateContentType.Name, contentType.ContentTypes);
        }

        [Fact]
        public static void TemplateErrorTaggerProviderCanBeConstructedByVisualStudio()
        {
            using (var catalog = new TypeCatalog(
                typeof(TemplateErrorTaggerProvider),
                typeof(SubstituteExporter<ITemplateEditorOptions>)))
            using (var container = new CompositionContainer(catalog))
            {
                Lazy<ITaggerProvider> export = container.GetExport<ITaggerProvider>();
                Assert.IsType<TemplateErrorTaggerProvider>(export.Value);
            }
        }

        [Fact]
        public static void ConstructorThrowsArgumentNullExceptionWhenOptionsIsNullToFailFast()
        {
            var e = Assert.Throws<ArgumentNullException>(() => new TemplateErrorTaggerProvider(null));
            Assert.Equal("options", e.ParamName);
        }

        [Fact]
        public static void CreateTaggerThrowsArgumentNullExceptionWhenBufferIsNullToFailFast()
        {
            var provider = new TemplateErrorTaggerProvider(Substitute.For<ITemplateEditorOptions>());
            var e = Assert.Throws<ArgumentNullException>(() => provider.CreateTagger<ErrorTag>(null));
            Assert.Equal("buffer", e.ParamName);
        }

        [Fact]
        public static void CreateTaggerReturnsTemplateErrorTagger()
        {
            ITemplateEditorOptions options = OptionsWithErrorUnderliningEnabled(true);
            var target = new TemplateErrorTaggerProvider(options);
            Assert.NotNull(target.CreateTagger<ErrorTag>(new FakeTextBuffer(string.Empty)));
        }

        [Fact]
        public static void CreateTaggerReturnsSameTaggerWhenCalledMultipleTimesForSameBuffer()
        {
            ITemplateEditorOptions options = OptionsWithErrorUnderliningEnabled(true);
            var target = new TemplateErrorTaggerProvider(options);
            var buffer = new FakeTextBuffer(string.Empty);
            var tagger1 = target.CreateTagger<ErrorTag>(buffer);
            var tagger2 = target.CreateTagger<ErrorTag>(buffer);
            Assert.Same(tagger1, tagger2);
        }

        [Fact]
        public static void CreateTaggerDoesNotReturnTaggerWhenErrorUnderliningIsDisabled()
        {
            ITemplateEditorOptions options = OptionsWithErrorUnderliningEnabled(false);
            var target = new TemplateErrorTaggerProvider(options);
            Assert.Null(target.CreateTagger<ErrorTag>(new FakeTextBuffer(string.Empty)));
        }

        private static ITemplateEditorOptions OptionsWithErrorUnderliningEnabled(bool enabled)
        {
            var options = Substitute.For<ITemplateEditorOptions>();
            options.ErrorUnderliningEnabled.Returns(enabled);
            return options;
        }
    }
}