// <copyright file="TemplateErrorTaggerProviderTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.Editor
{
    using System.ComponentModel.Composition;
    using System.Reflection;
    using Microsoft.VisualStudio.Text.Tagging;
    using Microsoft.VisualStudio.Utilities;
    using NSubstitute;
    using Xunit;

    public class TemplateErrorTaggerProviderTest
    {
        [Fact]
        public void TemplateErrorTaggerProviderIsInternalAndNotMeantForPublicConsumption()
        {
            Assert.False(typeof(TemplateErrorTaggerProvider).IsPublic);
        }

        [Fact]
        public void TemplateErrorTaggerProviderIsSealedAndNotMeantToHaveChildClasses()
        {
            Assert.True(typeof(TemplateErrorTaggerProvider).IsSealed);
        }

        [Fact]
        public void TemplateErrorTaggerProviderExportsITaggerProvider()
        {
            var export = (ExportAttribute)typeof(TemplateErrorTaggerProvider).GetCustomAttributes(typeof(ExportAttribute), false)[0];
            Assert.Equal(typeof(ITaggerProvider), export.ContractType);
        }

        [Fact]
        public void TemplateErrorTaggerProviderSpecifiesErrorTagType()
        {
            var tagType = (TagTypeAttribute)typeof(TemplateErrorTaggerProvider).GetCustomAttributes(typeof(TagTypeAttribute), false)[0];
            Assert.Equal(typeof(ErrorTag), tagType.TagTypes);
        }

        [Fact]
        public void TemplateErrorTaggerProviderAppliesOnlyToTextTemplateContentType()
        {
            var contentType = (ContentTypeAttribute)typeof(TemplateErrorTaggerProvider).GetCustomAttributes(typeof(ContentTypeAttribute), false)[0];
            Assert.Equal(TemplateContentType.Name, contentType.ContentTypes);
        }

        [Fact]
        public void CreateTaggerReturnsTemplateErrorTagger()
        {
            ITemplateEditorOptions options = OptionsWithErrorUnderliningEnabled(true);
            var target = new TemplateErrorTaggerProvider(options);
            Assert.NotNull(target.CreateTagger<ErrorTag>(new FakeTextBuffer(string.Empty)));
        }

        [Fact]
        public void CreateTaggerReturnsSameTaggerWhenCalledMultipleTimesForSameBuffer()
        {
            ITemplateEditorOptions options = OptionsWithErrorUnderliningEnabled(true);
            var target = new TemplateErrorTaggerProvider(options);
            var buffer = new FakeTextBuffer(string.Empty);
            var tagger1 = target.CreateTagger<ErrorTag>(buffer);
            var tagger2 = target.CreateTagger<ErrorTag>(buffer);
            Assert.Same(tagger1, tagger2);
        }

        [Fact]
        public void CreateTaggerDoesNotReturnTaggerWhenErrorUnderliningIsDisabled()
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