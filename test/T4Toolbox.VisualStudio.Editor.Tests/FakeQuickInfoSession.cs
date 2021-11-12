// <copyright file="FakeQuickInfoSession.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.Editor
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.Language.Intellisense;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.Utilities;

    internal class FakeQuickInfoSession : IAsyncQuickInfoSession
    {
        public SnapshotPoint? SnapshotTriggerPoint { get; set; }

        #region IAsyncQuickInfoSession

        public ITrackingSpan ApplicableToSpan
        {
            get { throw new NotImplementedException(); }
        }

        public IEnumerable<object> Content
        {
            get { throw new NotImplementedException(); }
        }

        public bool HasInteractiveContent
        {
            get { throw new NotImplementedException(); }
        }

        public QuickInfoSessionOptions Options
        {
            get { throw new NotImplementedException(); }
        }

        public QuickInfoSessionState State
        {
            get { throw new NotImplementedException(); }
        }

        public ITextView TextView
        {
            get { throw new NotImplementedException(); }
        }

        public PropertyCollection Properties
        {
            get { throw new NotImplementedException(); }
        }

        public event EventHandler<QuickInfoSessionStateChangedEventArgs> StateChanged
        {
            add { throw new NotImplementedException(); }
            remove { throw new NotImplementedException(); }
        }

        public Task DismissAsync()
        {
            throw new NotImplementedException();
        }

        public ITrackingPoint GetTriggerPoint(ITextBuffer textBuffer)
        {
            throw new NotImplementedException();
        }

        public SnapshotPoint? GetTriggerPoint(ITextSnapshot snapshot)
        {
            return this.SnapshotTriggerPoint;
        }

        #endregion
    }
}