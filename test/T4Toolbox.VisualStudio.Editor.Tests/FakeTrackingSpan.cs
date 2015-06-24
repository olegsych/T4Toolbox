// <copyright file="FakeTrackingSpan.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.Editor
{
    using Microsoft.VisualStudio.Text;

    internal class FakeTrackingSpan : ITrackingSpan
    {
        private Span span;
        private SpanTrackingMode trackingMode;

        public FakeTrackingSpan(Span span, SpanTrackingMode trackingMode)
        {
            this.span = span;
            this.trackingMode = trackingMode;
        }

        #region ITrackingSpan

        SnapshotSpan ITrackingSpan.GetSpan(ITextSnapshot snapshot)
        {
            return new SnapshotSpan(snapshot, this.span);
        }

        Span ITrackingSpan.GetSpan(ITextVersion version)
        {
            throw new System.NotImplementedException();
        }

        string ITrackingSpan.GetText(ITextSnapshot snapshot)
        {
            throw new System.NotImplementedException();
        }

        SnapshotPoint ITrackingSpan.GetStartPoint(ITextSnapshot snapshot)
        {
            throw new System.NotImplementedException();
        }

        SnapshotPoint ITrackingSpan.GetEndPoint(ITextSnapshot snapshot)
        {
            throw new System.NotImplementedException();
        }

        ITextBuffer ITrackingSpan.TextBuffer
        {
            get { throw new System.NotImplementedException(); }
        }

        SpanTrackingMode ITrackingSpan.TrackingMode
        {
            get { return this.trackingMode; }
        }

        TrackingFidelityMode ITrackingSpan.TrackingFidelity
        {
            get { throw new System.NotImplementedException(); }
        }

        #endregion
    }
}