// <copyright file="StubIVsRunningDocumentTable.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.Tests.Fakes
{
    using System;
    using System.Runtime.InteropServices;

    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.Shell.Interop;

    internal delegate int FindAndLockDocumentCallback(_VSRDTFLAGS lockType, string fileName, out IVsHierarchy hierarchy, out uint itemId, out IVsPersistDocData documentData, out uint cookie);

    internal class StubIVsRunningDocumentTable : SVsRunningDocumentTable, IVsRunningDocumentTable
    {
        internal FindAndLockDocumentCallback FindAndLockDocument { get; set; }

        #region IVsRunningDocumentTable

        int IVsRunningDocumentTable.RegisterAndLockDocument(uint lockType, string fileName, IVsHierarchy hierarchy, uint itemId, IntPtr documentData, out uint cookie)
        {
            throw new NotImplementedException();
        }

        int IVsRunningDocumentTable.LockDocument(uint lockType, uint cookie)
        {
            throw new NotImplementedException();
        }

        int IVsRunningDocumentTable.UnlockDocument(uint lockType, uint cookie)
        {
            throw new NotImplementedException();
        }

        int IVsRunningDocumentTable.FindAndLockDocument(uint lockType, string fileName, out IVsHierarchy hierarchy, out uint itemId, out IntPtr documentDataPointer, out uint cookie)
        {
            hierarchy = null;
            itemId = 0;
            documentDataPointer = IntPtr.Zero;
            cookie = 0;
            int result = VSConstants.S_OK;

            if (this.FindAndLockDocument != null)
            {
                IVsPersistDocData documentData;
                result = this.FindAndLockDocument((_VSRDTFLAGS)lockType, fileName, out hierarchy, out itemId, out documentData, out cookie);
                documentDataPointer = (documentData == null) ? IntPtr.Zero : Marshal.GetComInterfaceForObject(documentData, typeof(IVsPersistDocData));
            }

            return result;
        }

        int IVsRunningDocumentTable.RenameDocument(string oldFileName, string newFileName, IntPtr hierarchy, uint newItemId)
        {
            throw new NotImplementedException();
        }

        int IVsRunningDocumentTable.AdviseRunningDocTableEvents(IVsRunningDocTableEvents sink, out uint cookie)
        {
            throw new NotImplementedException();
        }

        int IVsRunningDocumentTable.UnadviseRunningDocTableEvents(uint cookie)
        {
            throw new NotImplementedException();
        }

        int IVsRunningDocumentTable.GetDocumentInfo(uint cookie, out uint flags, out uint readLocks, out uint editLocks, out string fileName, out IVsHierarchy hierarchy, out uint itemId, out IntPtr documentData)
        {
            throw new NotImplementedException();
        }

        int IVsRunningDocumentTable.NotifyDocumentChanged(uint cookie, uint documentChanged)
        {
            throw new NotImplementedException();
        }

        int IVsRunningDocumentTable.NotifyOnAfterSave(uint cookie)
        {
            throw new NotImplementedException();
        }

        int IVsRunningDocumentTable.GetRunningDocumentsEnum(out IEnumRunningDocuments runningDocuments)
        {
            throw new NotImplementedException();
        }

        int IVsRunningDocumentTable.SaveDocuments(uint options, IVsHierarchy hierarchy, uint itemId, uint cookie)
        {
            throw new NotImplementedException();
        }

        int IVsRunningDocumentTable.NotifyOnBeforeSave(uint cookie)
        {
            throw new NotImplementedException();
        }

        int IVsRunningDocumentTable.RegisterDocumentLockHolder(uint flags, uint cookie, IVsDocumentLockHolder lockHolder, out uint lockHolderCookie)
        {
            throw new NotImplementedException();
        }

        int IVsRunningDocumentTable.UnregisterDocumentLockHolder(uint lockHolderCookie)
        {
            throw new NotImplementedException();
        }

        int IVsRunningDocumentTable.ModifyDocumentFlags(uint documentCookie, uint flags, int set)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}