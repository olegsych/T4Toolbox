// <copyright file="FakeCompletionSession.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.Editor
{
    using System;
    using System.Collections.ObjectModel;
    using Microsoft.VisualStudio.Language.Intellisense;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.Utilities;

    internal class FakeCompletionSession : ICompletionSession
    {
        public int? TriggerPosition { get; set; }

        #region ICompletionSession

        event EventHandler ICompletionSession.Committed
        {
            add { throw new NotImplementedException(); }
            remove { throw new NotImplementedException(); }
        }

        event EventHandler IIntellisenseSession.Dismissed
        {
            add { throw new NotImplementedException(); }
            remove { throw new NotImplementedException(); }
        }

        event EventHandler IIntellisenseSession.PresenterChanged
        {
            add { throw new NotImplementedException(); }
            remove { throw new NotImplementedException(); }
        }

        event EventHandler IIntellisenseSession.Recalculated
        {
            add { throw new NotImplementedException(); }
            remove { throw new NotImplementedException(); }
        }

        event EventHandler<ValueChangedEventArgs<CompletionSet>> ICompletionSession.SelectedCompletionSetChanged
        {
            add { throw new NotImplementedException(); }
            remove { throw new NotImplementedException(); }
        }

        ReadOnlyObservableCollection<CompletionSet> ICompletionSession.CompletionSets
        {
            get { throw new NotImplementedException(); }
        }

        bool IIntellisenseSession.IsDismissed
        {
            get { throw new NotImplementedException(); }
        }

        bool ICompletionSession.IsStarted
        {
            get { throw new NotImplementedException(); }
        }

        IIntellisensePresenter IIntellisenseSession.Presenter
        {
            get { throw new NotImplementedException(); }
        }

        PropertyCollection IPropertyOwner.Properties
        {
            get { throw new NotImplementedException(); }
        }

        CompletionSet ICompletionSession.SelectedCompletionSet
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        ITextView IIntellisenseSession.TextView
        {
            get { throw new NotImplementedException(); }
        }

        void IIntellisenseSession.Collapse()
        {
            throw new NotImplementedException();
        }

        void ICompletionSession.Commit()
        {
            throw new NotImplementedException();
        }

        void IIntellisenseSession.Dismiss()
        {
            throw new NotImplementedException();
        }

        void ICompletionSession.Filter()
        {
            throw new NotImplementedException();
        }

        SnapshotPoint? IIntellisenseSession.GetTriggerPoint(ITextSnapshot textSnapshot)
        {
            return this.TriggerPosition != null ? (SnapshotPoint?)new SnapshotPoint(textSnapshot, this.TriggerPosition.Value) : null;
        }

        ITrackingPoint IIntellisenseSession.GetTriggerPoint(ITextBuffer textBuffer)
        {
            throw new NotImplementedException();
        }

        bool IIntellisenseSession.Match()
        {
            throw new NotImplementedException();
        }

        void IIntellisenseSession.Recalculate()
        {
            throw new NotImplementedException();
        }

        void IIntellisenseSession.Start()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}