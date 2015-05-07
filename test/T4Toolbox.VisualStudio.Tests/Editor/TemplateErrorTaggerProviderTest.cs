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
    using T4Toolbox.VisualStudio.Tests.Fakes;

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
            var target = new TemplateErrorTaggerProvider();
            Assert.IsNotNull(target.CreateTagger<ErrorTag>(new FakeTextBuffer(string.Empty)));
        }

        [TestMethod]
        public void CreateTaggerReturnsSameTaggerWhenCalledMultipleTimesForSameBuffer()
        {
            var target = new TemplateErrorTaggerProvider();
            var buffer = new FakeTextBuffer(string.Empty);
            var tagger1 = target.CreateTagger<ErrorTag>(buffer);
            var tagger2 = target.CreateTagger<ErrorTag>(buffer);
            Assert.AreSame(tagger1, tagger2);
        }

        [TestMethod]
        public void CreateTaggerDoesNotReturnTaggerWhenErrorUnderliningIsDisabled()
        {
            var target = new TemplateErrorTaggerProvider();

            T4ToolboxOptions.Instance.ErrorUnderliningEnabled = false;
            try
            {
                Assert.IsNull(target.CreateTagger<ErrorTag>(new FakeTextBuffer(string.Empty)));
            }
            finally
            {
                typeof(T4ToolboxOptions).GetField("instance", BindingFlags.Static | BindingFlags.NonPublic).SetValue(null, null);
            }
        }
    }
}