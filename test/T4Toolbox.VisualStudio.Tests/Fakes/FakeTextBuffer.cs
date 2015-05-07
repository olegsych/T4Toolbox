// <copyright file="FakeTextBuffer.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.Tests.Fakes
{
    using System;

    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Utilities;

    internal sealed class FakeTextBuffer : ITextBuffer
    {
        private FakeTextSnapshot currentSnapshot;
        private PropertyCollection properties;

        public FakeTextBuffer(string text)
        {
            this.currentSnapshot = new FakeTextSnapshot(text);
        }

        public FakeTextSnapshot CurrentSnapshot
        {
            get
            {
                return this.currentSnapshot;
            }

            set
            {
                ITextSnapshot oldSnapshot = this.currentSnapshot;
                this.currentSnapshot = value;
                this.OnChanged(new TextContentChangedEventArgs(oldSnapshot, this.currentSnapshot, new EditOptions(), new object()));
            }
        }

        public EventHandler<TextContentChangedEventArgs> Changed { get; set; }

        #region ITextBuffer

        PropertyCollection IPropertyOwner.Properties
        {
            get { return this.properties ?? (this.properties = new PropertyCollection()); }
        }

        ITextEdit ITextBuffer.CreateEdit(EditOptions options, int? reiteratedVersionNumber, object editTag)
        {
            throw new NotImplementedException();
        }

        ITextEdit ITextBuffer.CreateEdit()
        {
            throw new NotImplementedException();
        }

        IReadOnlyRegionEdit ITextBuffer.CreateReadOnlyRegionEdit()
        {
            throw new NotImplementedException();
        }

        void ITextBuffer.TakeThreadOwnership()
        {
            throw new NotImplementedException();
        }

        bool ITextBuffer.CheckEditAccess()
        {
            throw new NotImplementedException();
        }

        void ITextBuffer.ChangeContentType(IContentType newContentType, object editTag)
        {
            throw new NotImplementedException();
        }

        ITextSnapshot ITextBuffer.Insert(int position, string text)
        {
            throw new NotImplementedException();
        }

        ITextSnapshot ITextBuffer.Delete(Span deleteSpan)
        {
            throw new NotImplementedException();
        }

        ITextSnapshot ITextBuffer.Replace(Span replaceSpan, string replaceWith)
        {
            throw new NotImplementedException();
        }

        bool ITextBuffer.IsReadOnly(int position)
        {
            throw new NotImplementedException();
        }

        bool ITextBuffer.IsReadOnly(int position, bool isEdit)
        {
            throw new NotImplementedException();
        }

        bool ITextBuffer.IsReadOnly(Span span)
        {
            throw new NotImplementedException();
        }

        bool ITextBuffer.IsReadOnly(Span span, bool isEdit)
        {
            throw new NotImplementedException();
        }

        NormalizedSpanCollection ITextBuffer.GetReadOnlyExtents(Span span)
        {
            throw new NotImplementedException();
        }

        IContentType ITextBuffer.ContentType
        {
            get { throw new NotImplementedException(); }
        }

        ITextSnapshot ITextBuffer.CurrentSnapshot
        {
            get { return this.CurrentSnapshot; }
        }

        bool ITextBuffer.EditInProgress
        {
            get { throw new NotImplementedException(); }
        }

        event EventHandler<SnapshotSpanEventArgs> ITextBuffer.ReadOnlyRegionsChanged
        {
            add { throw new NotImplementedException(); }
            remove { throw new NotImplementedException(); }
        }

        event EventHandler<TextContentChangedEventArgs> ITextBuffer.Changed
        {
            add { this.Changed += value; }
            remove { this.Changed -= value; }
        }

        event EventHandler<TextContentChangedEventArgs> ITextBuffer.ChangedLowPriority
        {
            add { throw new NotImplementedException(); }
            remove { throw new NotImplementedException(); }
        }

        event EventHandler<TextContentChangedEventArgs> ITextBuffer.ChangedHighPriority
        {
            add { throw new NotImplementedException(); }
            remove { throw new NotImplementedException(); }
        }

        event EventHandler<TextContentChangingEventArgs> ITextBuffer.Changing
        {
            add { throw new NotImplementedException(); }
            remove { throw new NotImplementedException(); }
        }

        event EventHandler ITextBuffer.PostChanged
        {
            add { throw new NotImplementedException(); }
            remove { throw new NotImplementedException(); }
        }

        event EventHandler<ContentTypeChangedEventArgs> ITextBuffer.ContentTypeChanged
        {
            add { throw new NotImplementedException(); }
            remove { throw new NotImplementedException(); }
        }

        #endregion

        private void OnChanged(TextContentChangedEventArgs args)
        {
            if (this.Changed != null)
            {
                this.Changed(this, args);
            }
        }
    }
}