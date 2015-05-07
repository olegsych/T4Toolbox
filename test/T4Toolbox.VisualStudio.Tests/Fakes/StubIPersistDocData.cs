// <copyright file="StubIPersistDocData.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.Tests.Fakes
{
    using System;
    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.Shell.Interop;

    internal class StubIPersistDocData : IVsPersistDocData
    {
        internal Func<uint, int> ReloadDocData { get; set; }

        #region IVsPersistDocData

        int IVsPersistDocData.GetGuidEditorType(out Guid classId)
        {
            throw new NotImplementedException();
        }

        int IVsPersistDocData.IsDocDataDirty(out int isDirty)
        {
            throw new NotImplementedException();
        }

        int IVsPersistDocData.SetUntitledDocPath(string docDataPath)
        {
            throw new NotImplementedException();
        }

        int IVsPersistDocData.LoadDocData(string fileName)
        {
            throw new NotImplementedException();
        }

        int IVsPersistDocData.SaveDocData(VSSAVEFLAGS saveFlags, out string fileName, out int saveCanceled)
        {
            throw new NotImplementedException();
        }

        int IVsPersistDocData.Close()
        {
            throw new NotImplementedException();
        }

        int IVsPersistDocData.OnRegisterDocData(uint docCookie, IVsHierarchy hierarchy, uint itemId)
        {
            throw new NotImplementedException();
        }

        int IVsPersistDocData.RenameDocData(uint attributes, IVsHierarchy hierarchy, uint itemId, string fileName)
        {
            throw new NotImplementedException();
        }

        int IVsPersistDocData.IsDocDataReloadable(out int isReloadable)
        {
            throw new NotImplementedException();
        }

        int IVsPersistDocData.ReloadDocData(uint flags)
        {
            if (this.ReloadDocData != null)
            {
                return this.ReloadDocData(flags);
            }

            return VSConstants.S_OK;
        }

        #endregion
    }
}