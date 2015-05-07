// <copyright file="FakeQuickInfoSession.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.Fakes
{
    using System;
    using Microsoft.VisualStudio.Language.Intellisense;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.Utilities;

    internal class FakeQuickInfoSession : IQuickInfoSession
    {
        public SnapshotPoint? SnapshotTriggerPoint { get; set; }

        #region IQuickInfoSession

        ITrackingSpan IQuickInfoSession.ApplicableToSpan
        {
            get { throw new NotImplementedException(); }
        }

        event EventHandler IQuickInfoSession.ApplicableToSpanChanged
        {
            add { throw new NotImplementedException(); }
            remove { throw new NotImplementedException(); }
        }

        BulkObservableCollection<object> IQuickInfoSession.QuickInfoContent
        {
            get { throw new NotImplementedException(); }
        }

        bool IQuickInfoSession.TrackMouse
        {
            get { throw new NotImplementedException(); }
        }

        void IIntellisenseSession.Collapse()
        {
            throw new NotImplementedException();
        }

        void IIntellisenseSession.Dismiss()
        {
            throw new NotImplementedException();
        }

        event EventHandler IIntellisenseSession.Dismissed
        {
            add { throw new NotImplementedException(); }
            remove { throw new NotImplementedException(); }
        }

        SnapshotPoint? IIntellisenseSession.GetTriggerPoint(ITextSnapshot textSnapshot)
        {
            return this.SnapshotTriggerPoint;
        }

        ITrackingPoint IIntellisenseSession.GetTriggerPoint(ITextBuffer textBuffer)
        {
            throw new NotImplementedException();
        }

        bool IIntellisenseSession.IsDismissed
        {
            get { throw new NotImplementedException(); }
        }

        bool IIntellisenseSession.Match()
        {
            throw new NotImplementedException();
        }

        IIntellisensePresenter IIntellisenseSession.Presenter
        {
            get { throw new NotImplementedException(); }
        }

        event EventHandler IIntellisenseSession.PresenterChanged
        {
            add { throw new NotImplementedException(); }
            remove { throw new NotImplementedException(); }
        }

        void IIntellisenseSession.Recalculate()
        {
            throw new NotImplementedException();
        }

        event EventHandler IIntellisenseSession.Recalculated
        {
            add { throw new NotImplementedException(); }
            remove { throw new NotImplementedException(); }
        }

        void IIntellisenseSession.Start()
        {
            throw new NotImplementedException();
        }

        ITextView IIntellisenseSession.TextView
        {
            get { throw new NotImplementedException(); }
        }

        PropertyCollection IPropertyOwner.Properties
        {
            get { throw new NotImplementedException(); }
        }

        #endregion
    }
}