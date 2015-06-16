// <copyright file="TemplateErrorReporterProviderTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.Editor
{
    using System;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Tagging;
    using Microsoft.VisualStudio.Utilities;
    using NSubstitute;
    using Xunit;

    public static class TemplateErrorReporterProviderTest
    {
        [Fact]
        public static void ClassIsInternalAndNotMeantForPublicConsumption()
        {
            Assert.False(typeof(TemplateErrorReporterProvider).IsPublic);
        }

        [Fact]
        public static void ClassIsSealedAndNotMeantToHaveChildClasses()
        {
            Assert.True(typeof(TemplateErrorReporterProvider).IsSealed);
        }

        [Fact]
        public static void ClassExportsITaggerProviderToInstantiateTemplateErrorReporterAtTheSameTimeWithTemplateErrorTagger()
        {
            var export = (ExportAttribute)typeof(TemplateErrorReporterProvider).GetCustomAttributes(typeof(ExportAttribute), false)[0];
            Assert.Equal(typeof(ITaggerProvider), export.ContractType);
        }

        [Fact]
        public static void ClassSpecifiesErrorTagType()
        {
            var tagType = (TagTypeAttribute)typeof(TemplateErrorReporterProvider).GetCustomAttributes(typeof(TagTypeAttribute), false)[0];
            Assert.Equal(typeof(ErrorTag), tagType.TagTypes);
        }

        [Fact]
        public static void ClassAppliesOnlyToTextTemplateContentType()
        {
            var contentType = (ContentTypeAttribute)typeof(TemplateErrorReporterProvider).GetCustomAttributes(typeof(ContentTypeAttribute), false)[0];
            Assert.Equal(TemplateContentType.Name, contentType.ContentTypes);
        }

        [Fact]
        public static void ClassCanBeConstructedByVisualStudio()
        {
            using (var catalog = new TypeCatalog(
                typeof(TemplateErrorReporterProvider),
                typeof(SubstituteExporter<ITemplateEditorOptions>),
                typeof(SubstituteExporter<SVsServiceProvider>),
                typeof(SubstituteExporter<ITextDocumentFactoryService>)))
            using (var container = new CompositionContainer(catalog))
            {
                Lazy<ITaggerProvider> export = container.GetExport<ITaggerProvider>();
                Assert.IsType<TemplateErrorReporterProvider>(export.Value);
            }
        }

        [Fact]
        public static void ConstructorThrowsArgumentNullExceptionWhenOptionsIsNullToFailFast()
        {
            var e = Assert.Throws<ArgumentNullException>(() => new TemplateErrorReporterProvider(null, Substitute.For<SVsServiceProvider>(), Substitute.For<ITextDocumentFactoryService>()));
            Assert.Equal("options", e.ParamName);
        }

        [Fact]
        public static void ConstructorThrowsArgumentNullExceptionWhenServiceProviderIsNullToFailFast()
        {
            var e = Assert.Throws<ArgumentNullException>(() => new TemplateErrorReporterProvider(Substitute.For<ITemplateEditorOptions>(), null, Substitute.For<ITextDocumentFactoryService>()));
            Assert.Equal("serviceProvider", e.ParamName);
        }

        [Fact]
        public static void ConstructorThrowsArgumentNullExceptionWhenTextDocumentFactoryIsNullToFailFast()
        {
            var e = Assert.Throws<ArgumentNullException>(() => new TemplateErrorReporterProvider(Substitute.For<ITemplateEditorOptions>(), Substitute.For<SVsServiceProvider>(), null));
            Assert.Equal("textDocumentFactory", e.ParamName);
        }

        [Fact]
        public static void CreateTaggerAlwaysReturnsNullBecauseTemplateErrorReporterIsNotATagger()
        {
            ITemplateEditorOptions options = OptionsWithErrorReportingEnabled(true);
            var provider = new TemplateErrorReporterProvider(options, Substitute.For<SVsServiceProvider>(), Substitute.For<ITextDocumentFactoryService>());
            Assert.Null(provider.CreateTagger<ErrorTag>(new FakeTextBuffer(string.Empty)));
        }

        [Fact]
        public static void CreateTaggerCreatesTemplateErrorReporterWhenErrorReportingIsEnabled()
        {
            ITemplateEditorOptions options = OptionsWithErrorReportingEnabled(true);
            var provider = new TemplateErrorReporterProvider(options, Substitute.For<SVsServiceProvider>(), Substitute.For<ITextDocumentFactoryService>());

            ITextBuffer textBuffer = new FakeTextBuffer(string.Empty);
            provider.CreateTagger<ErrorTag>(textBuffer);

            Assert.True(textBuffer.Properties.ContainsProperty(typeof(TemplateErrorReporter)));
        }

        [Fact]
        public static void CreateTaggerDoesNotCreateTemplateErrorReporterWhenErrorReportingIsDisabled()
        {
            ITemplateEditorOptions options = OptionsWithErrorReportingEnabled(false);
            var provider = new TemplateErrorReporterProvider(options, Substitute.For<SVsServiceProvider>(), Substitute.For<ITextDocumentFactoryService>());

            ITextBuffer textBuffer = new FakeTextBuffer(string.Empty);
            provider.CreateTagger<ErrorTag>(textBuffer);

            Assert.False(textBuffer.Properties.ContainsProperty(typeof(TemplateErrorReporter)));
        }

        private static ITemplateEditorOptions OptionsWithErrorReportingEnabled(bool enabled)
        {
            var options = Substitute.For<ITemplateEditorOptions>();
            options.ErrorReportingEnabled.Returns(enabled);
            return options;
        }
    }
}
