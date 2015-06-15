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
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.TextManager.Interop;
    using Microsoft.VisualStudio.Utilities;
    using NSubstitute;
    using Xunit;

    public class TemplateCompletionHandlerProviderTest
    {
        [Fact]
        public void TemplateCompletionHandlerProviderIsInternalBecauseItIsOnlyMeantToBeImportedByVisualStudio()
        {
            Assert.False(typeof(TemplateCompletionHandlerProvider).IsPublic);
        }

        [Fact]
        public void TemplateCompletionHandlerProviderIsSealedBecauseItIsNotMeantToHaveChildClasses()
        {
            Assert.True(typeof(TemplateCompletionHandlerProvider).IsSealed);
        }

        [Fact]
        public void TemplateCompletionHandlerProviderImplementsIVsTextViewCreationListenerToDetectWhenTemplateTextViewIsCreated()
        {
            Assert.Equal(typeof(IVsTextViewCreationListener), typeof(TemplateCompletionHandlerProvider).GetInterfaces().Single());
        }

        [Fact]
        public void TemplateCompletionHandlerExportsIVsTextViewCreationListener()
        {
            ExportAttribute attribute = typeof(TemplateCompletionHandlerProvider).GetCustomAttributes(false).OfType<ExportAttribute>().Single();
            Assert.Equal(typeof(IVsTextViewCreationListener), attribute.ContractType);
        }

        [Fact]
        public void TemplateCompletionHandlerAppliesOnlyToTextTemplateContentType()
        {
            ContentTypeAttribute attribute = typeof(TemplateCompletionHandlerProvider).GetCustomAttributes(false).OfType<ContentTypeAttribute>().Single();
            Assert.Equal(TemplateContentType.Name, attribute.ContentTypes);
        }

        [Fact]
        public void TemplateCompletionHandlerSpecifiesEditableTextViewRoleRequiredByVisualStudioForTextViewCreationListeners()
        {
            TextViewRoleAttribute attribute = typeof(TemplateCompletionHandlerProvider).GetCustomAttributes(false).OfType<TextViewRoleAttribute>().Single();
            Assert.Equal(PredefinedTextViewRoles.Editable, attribute.TextViewRoles);
        }

        [Fact]
        public void TextViewCreatedCreatesTemplateCompletionHandlerWhenViewAdapterHasTextView()
        {
            var viewProperties = new PropertyCollection();
            
            var view = Substitute.For<IWpfTextView>();
            view.Properties.Returns(viewProperties);

            var viewAdapter = Substitute.For<IVsTextView>();

            var adapterFactory = Substitute.For<IVsEditorAdaptersFactoryService>();
            adapterFactory.GetWpfTextView(viewAdapter).Returns(view);

            ITemplateEditorOptions options = OptionsWithCompletionListsEnabled(true);

            var provider = new TemplateCompletionHandlerProvider(options) { AdapterFactory = adapterFactory };

            provider.VsTextViewCreated(viewAdapter);

            Assert.True(viewProperties.ContainsProperty(typeof(TemplateCompletionHandler)));
        }

        [SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "Microsoft.VisualStudio.TextManager.Interop.IVsTextView.AddCommandFilter(Microsoft.VisualStudio.OLE.Interop.IOleCommandTarget,Microsoft.VisualStudio.OLE.Interop.IOleCommandTarget@)", Justification = "This test does not call AddCommandFilter method; it only asserts it was.")]
        [Fact]
        public void TextViewCreatedAddsTemplateCompletionHandlerToTextViewCommandFilters()
        {
            var viewProperties = new PropertyCollection();
         
            var view = Substitute.For<IWpfTextView>();
            view.Properties.Returns(viewProperties);

            var viewAdapter = Substitute.For<IVsTextView>();

            var adapterFactory = Substitute.For<IVsEditorAdaptersFactoryService>();
            adapterFactory.GetWpfTextView(viewAdapter).Returns(view);

            ITemplateEditorOptions options = OptionsWithCompletionListsEnabled(true);

            var provider = new TemplateCompletionHandlerProvider(options) { AdapterFactory = adapterFactory };
            provider.VsTextViewCreated(viewAdapter);

            IOleCommandTarget nextTarget;
            viewAdapter.Received().AddCommandFilter(Arg.Any<TemplateCompletionHandler>(), out nextTarget);
        }

        [SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "Microsoft.VisualStudio.TextManager.Interop.IVsTextView.AddCommandFilter(Microsoft.VisualStudio.OLE.Interop.IOleCommandTarget,Microsoft.VisualStudio.OLE.Interop.IOleCommandTarget@)", Justification = "This test does not call AddCommandFilter method; it only asserts it was.")] 
        [Fact]
        public void TestViewCreatedSetsNextHandlerOfTemplateCompletionHandler()
        {
            var viewProperties = new PropertyCollection();

            var view = Substitute.For<IWpfTextView>();
            view.Properties.Returns(viewProperties);

            var viewAdapter = Substitute.For<IVsTextView>();

            var adapterFactory = Substitute.For<IVsEditorAdaptersFactoryService>();
            adapterFactory.GetWpfTextView(viewAdapter).Returns(view);

            ITemplateEditorOptions options = OptionsWithCompletionListsEnabled(true);

            var provider = new TemplateCompletionHandlerProvider(options) { AdapterFactory = adapterFactory };
            provider.VsTextViewCreated(viewAdapter);

            var handler = (TemplateCompletionHandler)viewProperties.GetProperty(typeof(TemplateCompletionHandler));
            viewAdapter.Received().AddCommandFilter(handler, out handler.NextHandler);
        }

        [Fact]
        public void TextViewCreatedDoesNotCreateTemplateCompletionHandlerWhenCompletionListsAreDisabled()
        {
            ITemplateEditorOptions options = OptionsWithCompletionListsEnabled(false);
            var adapterFactory = Substitute.For<IVsEditorAdaptersFactoryService>();
            var provider = new TemplateCompletionHandlerProvider(options) { AdapterFactory = adapterFactory };

            var viewAdapter = Substitute.For<IVsTextView>();
            provider.VsTextViewCreated(viewAdapter);

            adapterFactory.DidNotReceive().GetWpfTextView(viewAdapter);
        }

        [Fact]
        public void TextViewCreatedDoesNotCreateTemplateCompletionHandlerWhenViewAdapterDoesNotHaveTextView()
        {
            ITemplateEditorOptions options = OptionsWithCompletionListsEnabled(true);
            var adapterFactory = Substitute.For<IVsEditorAdaptersFactoryService>();
            adapterFactory.GetWpfTextView(Arg.Any<IVsTextView>()).Returns((IWpfTextView)null);

            var provider = new TemplateCompletionHandlerProvider(options) { AdapterFactory = adapterFactory };

            provider.VsTextViewCreated(Substitute.For<IVsTextView>());

            // No exception expected            
        }

        private static ITemplateEditorOptions OptionsWithCompletionListsEnabled(bool enabled)
        {
            var options = Substitute.For<ITemplateEditorOptions>();
            options.CompletionListsEnabled.Returns(enabled);
            return options;
        }
    }
}