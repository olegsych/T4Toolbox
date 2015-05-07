// <copyright file="TemplateCompletionHandlerProviderTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.Editor
{
    using System.ComponentModel.Composition;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.Editor;
    using Microsoft.VisualStudio.OLE.Interop;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.TextManager.Interop;
    using Microsoft.VisualStudio.Utilities;
    using NSubstitute;

    [TestClass]
    public class TemplateCompletionHandlerProviderTest
    {
        [TestMethod]
        public void TemplateCompletionHandlerProviderIsInternalBecauseItIsOnlyMeantToBeImportedByVisualStudio()
        {
            Assert.IsFalse(typeof(TemplateCompletionHandlerProvider).IsPublic);
        }

        [TestMethod]
        public void TemplateCompletionHandlerProviderIsSealedBecauseItIsNotMeantToHaveChildClasses()
        {
            Assert.IsTrue(typeof(TemplateCompletionHandlerProvider).IsSealed);
        }

        [TestMethod]
        public void TemplateCompletionHandlerProviderImplementsIVsTextViewCreationListenerToDetectWhenTemplateTextViewIsCreated()
        {
            Assert.AreEqual(typeof(IVsTextViewCreationListener), typeof(TemplateCompletionHandlerProvider).GetInterfaces().Single());
        }

        [TestMethod]
        public void TemplateCompletionHandlerExportsIVsTextViewCreationListener()
        {
            ExportAttribute attribute = typeof(TemplateCompletionHandlerProvider).GetCustomAttributes(false).OfType<ExportAttribute>().Single();
            Assert.AreEqual(typeof(IVsTextViewCreationListener), attribute.ContractType);
        }

        [TestMethod]
        public void TemplateCompletionHandlerAppliesOnlyToTextTemplateContentType()
        {
            ContentTypeAttribute attribute = typeof(TemplateCompletionHandlerProvider).GetCustomAttributes(false).OfType<ContentTypeAttribute>().Single();
            Assert.AreEqual(TemplateContentType.Name, attribute.ContentTypes);
        }

        [TestMethod]
        public void TemplateCompletionHandlerSpecifiesEditableTextViewRoleRequiredByVisualStudioForTextViewCreationListeners()
        {
            TextViewRoleAttribute attribute = typeof(TemplateCompletionHandlerProvider).GetCustomAttributes(false).OfType<TextViewRoleAttribute>().Single();
            Assert.AreEqual(PredefinedTextViewRoles.Editable, attribute.TextViewRoles);
        }

        [TestMethod]
        public void TextViewCreatedCreatesTemplateCompletionHandlerWhenViewAdapterHasTextView()
        {
            var viewProperties = new PropertyCollection();
            
            var view = Substitute.For<IWpfTextView>();
            view.Properties.Returns(viewProperties);

            var viewAdapter = Substitute.For<IVsTextView>();

            var adapterFactory = Substitute.For<IVsEditorAdaptersFactoryService>();
            adapterFactory.GetWpfTextView(viewAdapter).Returns(view);

            var provider = new TemplateCompletionHandlerProvider { AdapterFactory = adapterFactory };

            provider.VsTextViewCreated(viewAdapter);

            Assert.IsTrue(viewProperties.ContainsProperty(typeof(TemplateCompletionHandler)));
        }

        [SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "Microsoft.VisualStudio.TextManager.Interop.IVsTextView.AddCommandFilter(Microsoft.VisualStudio.OLE.Interop.IOleCommandTarget,Microsoft.VisualStudio.OLE.Interop.IOleCommandTarget@)", Justification = "This test does not call AddCommandFilter method; it only asserts it was.")]
        [TestMethod]
        public void TextViewCreatedAddsTemplateCompletionHandlerToTextViewCommandFilters()
        {
            var viewProperties = new PropertyCollection();
         
            var view = Substitute.For<IWpfTextView>();
            view.Properties.Returns(viewProperties);

            var viewAdapter = Substitute.For<IVsTextView>();

            var adapterFactory = Substitute.For<IVsEditorAdaptersFactoryService>();
            adapterFactory.GetWpfTextView(viewAdapter).Returns(view);

            var provider = new TemplateCompletionHandlerProvider { AdapterFactory = adapterFactory };
            provider.VsTextViewCreated(viewAdapter);

            IOleCommandTarget nextTarget;
            viewAdapter.Received().AddCommandFilter(Arg.Any<TemplateCompletionHandler>(), out nextTarget);
        }

        [SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "Microsoft.VisualStudio.TextManager.Interop.IVsTextView.AddCommandFilter(Microsoft.VisualStudio.OLE.Interop.IOleCommandTarget,Microsoft.VisualStudio.OLE.Interop.IOleCommandTarget@)", Justification = "This test does not call AddCommandFilter method; it only asserts it was.")] 
        [TestMethod]
        public void TestViewCreatedSetsNextHandlerOfTemplateCompletionHandler()
        {
            var viewProperties = new PropertyCollection();

            var view = Substitute.For<IWpfTextView>();
            view.Properties.Returns(viewProperties);

            var viewAdapter = Substitute.For<IVsTextView>();

            var adapterFactory = Substitute.For<IVsEditorAdaptersFactoryService>();
            adapterFactory.GetWpfTextView(viewAdapter).Returns(view);

            var provider = new TemplateCompletionHandlerProvider { AdapterFactory = adapterFactory };
            provider.VsTextViewCreated(viewAdapter);

            var handler = (TemplateCompletionHandler)viewProperties.GetProperty(typeof(TemplateCompletionHandler));
            viewAdapter.Received().AddCommandFilter(handler, out handler.NextHandler);
        }

        [TestMethod]
        public void TextViewCreatedDoesNotCreateTemplateCompletionHandlerWhenCompletionListsAreDisabled()
        {
            T4ToolboxOptions.Instance.CompletionListsEnabled = false;
            try
            {
                var adapterFactory = Substitute.For<IVsEditorAdaptersFactoryService>();
                var provider = new TemplateCompletionHandlerProvider { AdapterFactory = adapterFactory };

                var viewAdapter = Substitute.For<IVsTextView>();
                provider.VsTextViewCreated(viewAdapter);

                adapterFactory.DidNotReceive().GetWpfTextView(viewAdapter);
            }
            finally
            {
                new PrivateType(typeof(T4ToolboxOptions)).SetStaticField("instance", null);
            }
        }

        [TestMethod]
        public void TextViewCreatedDoesNotCreateTemplateCompletionHandlerWhenViewAdapterDoesNotHaveTextView()
        {
            var adapterFactory = Substitute.For<IVsEditorAdaptersFactoryService>();
            adapterFactory.GetWpfTextView(Arg.Any<IVsTextView>()).Returns((IWpfTextView)null);

            var provider = new TemplateCompletionHandlerProvider { AdapterFactory = adapterFactory };

            provider.VsTextViewCreated(Substitute.For<IVsTextView>());

            // No exception expected            
        }
    }
}