// <copyright file="FakeVsActivityLog.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.Fakes
{
    using System;
    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.Shell.Interop;

    internal class FakeVsActivityLog : SVsActivityLog, IVsActivityLog
    {
        public int LogEntry(uint activityType, string source, string description)
        {
            return this.LogEntryGuidHrPath(activityType, source, description, Guid.Empty, 0, string.Empty);
        }

        public int LogEntryGuid(uint activityType, string source, string description, Guid guid)
        {
            return this.LogEntryGuidHrPath(activityType, source, description, guid, 0, string.Empty);
        }

        public int LogEntryGuidHr(uint activityType, string source, string description, Guid guid, int result)
        {
            return this.LogEntryGuidHrPath(activityType, source, description, guid, result, string.Empty);
        }

        public int LogEntryGuidHrPath(uint activityType, string source, string description, Guid guid, int result, string path)
        {
            // Intentionally left blank.
            // Implement this method if/when verifying activity log entries becomes necessary.
            return VSConstants.S_OK;
        }

        public int LogEntryGuidPath(uint activityType, string source, string description, Guid guid, string path)
        {
            return this.LogEntryGuidHrPath(activityType, source, description, guid, 0, path);
        }

        public int LogEntryHr(uint activityType, string source, string description, int result)
        {
            return this.LogEntryGuidHrPath(activityType, source, description, Guid.Empty, 0, string.Empty);
        }

        public int LogEntryHrPath(uint activityType, string source, string description, int result, string path)
        {
            return this.LogEntryGuidHrPath(activityType, source, description, Guid.Empty, result, path);
        }

        public int LogEntryPath(uint activityType, string source, string description, string path)
        {
            return this.LogEntryGuidHrPath(activityType, source, description, Guid.Empty, 0, path);
        }
    }
}
