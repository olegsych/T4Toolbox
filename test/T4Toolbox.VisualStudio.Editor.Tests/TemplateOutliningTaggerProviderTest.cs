// <copyright file="TemplateOutliningTaggerProviderTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.Editor
{
    using System;
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Text.Tagging;
    using Microsoft.VisualStudio.Utilities;
    using NSubstitute;
    using Xunit;

    public class TemplateOutliningTaggerProviderTest
    {
        [Fact]
        public void TemplateOutliningTaggerProviderIsInternalAndNotMeantForPublicConsumption()
        {
            Assert.False(typeof(TemplateOutliningTaggerProvider).IsPublic);
        }

        [Fact]
        public void TemplateOutliningTaggerProviderIsSealedAndNotMeantToHaveChildClasses()
        {
            Assert.True(typeof(TemplateOutliningTaggerProvider).IsSealed);
        }

        [Fact]
        public void TemplateOutliningTaggerProviderImplementsITaggerProvider()
        {
            Assert.Equal(typeof(ITaggerProvider), typeof(TemplateOutliningTaggerProvider).GetInterfaces()[0]);
        }

        [Fact]
        public void TemplateOutliningTaggerProviderExportsITaggerProvider()
        {
            var export = (ExportAttribute)typeof(TemplateOutliningTaggerProvider).GetCustomAttributes(typeof(ExportAttribute), false)[0];
            Assert.Equal(typeof(ITaggerProvider), export.ContractType);
        }

        [Fact]
        public void TemplateOutliningTaggerProviderExportSpecifiesOutliningTagType()
        {
            var tagType = (TagTypeAttribute)typeof(TemplateOutliningTaggerProvider).GetCustomAttributes(typeof(TagTypeAttribute), false)[0];
            Assert.Equal(typeof(OutliningRegionTag), tagType.TagTypes);
        }

        [Fact]
        public void TemplateOutliningTaggerProviderAppliesOnlyToTextTemplateContentType()
        {
            var contentType = (ContentTypeAttribute)typeof(TemplateOutliningTaggerProvider).GetCustomAttributes(typeof(ContentTypeAttribute), false)[0];
            Assert.Equal(TemplateContentType.Name, contentType.ContentTypes);
        }

        [Fact]
        public void ConstructorThrowsArgumentNullExceptionWhenOptionsIsNullToFailFast()
        {
            var e = Assert.Throws<ArgumentNullException>(() => new TemplateOutliningTaggerProvider(null));
            Assert.Equal("options", e.ParamName);
        }

        [Fact]
        public void CreateTaggerThrowsArgumentNullExceptionWhenBufferIsNullToFailFast()
        {
            var provider = new TemplateOutliningTaggerProvider(Substitute.For<ITemplateEditorOptions>());
            var e = Assert.Throws<ArgumentNullException>(() => provider.CreateTagger<OutliningRegionTag>(null));
            Assert.Equal("buffer", e.ParamName);
        }

        [Fact]
        public void CreateTaggerReturnsTemplateOutliningTagger()
        {
            ITemplateEditorOptions options = OptionsWithTemplateOutliningEnabled(true);
            var provider = new TemplateOutliningTaggerProvider(options);
            var tagger = provider.CreateTagger<OutliningRegionTag>(new FakeTextBuffer(string.Empty));
            Assert.Equal(typeof(TemplateOutliningTagger), tagger.GetType());
        }

        [Fact]
        public void CreateTaggerReturnsSingleTaggerWhenCalledMultipleTimesForSameBuffer()
        {
            ITemplateEditorOptions options = OptionsWithTemplateOutliningEnabled(true);
            var provider = new TemplateOutliningTaggerProvider(options);
            var buffer = new FakeTextBuffer(string.Empty);
            var tagger1 = provider.CreateTagger<OutliningRegionTag>(buffer);
            var tagger2 = provider.CreateTagger<OutliningRegionTag>(buffer);
            Assert.Same(tagger1, tagger2);
        }

        [Fact]
        public void CreateTaggerDoesNotReturnTaggerWhenTemplateOutliningIsDisabled()
        {
            ITemplateEditorOptions options = OptionsWithTemplateOutliningEnabled(false);
            var provider = new TemplateOutliningTaggerProvider(options);
            Assert.Null(provider.CreateTagger<OutliningRegionTag>(new FakeTextBuffer(string.Empty)));
        }

        private static ITemplateEditorOptions OptionsWithTemplateOutliningEnabled(bool enabled)
        {
            var options = Substitute.For<ITemplateEditorOptions>();
            options.TemplateOutliningEnabled.Returns(enabled);
            return options;
        }
    }
}