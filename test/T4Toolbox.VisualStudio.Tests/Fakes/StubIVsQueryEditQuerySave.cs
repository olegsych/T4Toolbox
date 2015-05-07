// <copyright file="StubIVsQueryEditQuerySave.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.Tests.Fakes
{
    using System;
    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.Shell.Interop;

    internal class StubIVsQueryEditQuerySave : SVsQueryEditQuerySave, IVsQueryEditQuerySave2
    {
        internal delegate int QueryEditFilesCallback(tagVSQueryEditFlags editFlags, int fileCount, string[] fileNames, uint[] fileFlags, VSQEQS_FILE_ATTRIBUTE_DATA[] fileInfos, out tagVSQueryEditResult editResult, out tagVSQueryEditResultFlags moreInfo);

        internal delegate int QuerySaveFilesCallback(tagVSQuerySaveFlags saveFlags, int fileCount, string[] fileNames, uint[] fileFlags, VSQEQS_FILE_ATTRIBUTE_DATA[] fileInfos, out tagVSQuerySaveResult saveResult);

        internal QueryEditFilesCallback QueryEditFiles { get; set; }

        internal QuerySaveFilesCallback QuerySaveFiles { get; set; }

        #region IVsQueryEditQuerySave2

        int IVsQueryEditQuerySave2.QueryEditFiles(uint editFlags, int fileCount, string[] fileNames, uint[] fileFlags, VSQEQS_FILE_ATTRIBUTE_DATA[] fileInfos, out uint editResult, out uint moreInfo)
        {
            int result = VSConstants.S_OK;
            editResult = 0;
            moreInfo = 0;

            if (this.QueryEditFiles != null)
            {
                tagVSQueryEditResult callbackEditResult;
                tagVSQueryEditResultFlags callbackMoreInfo;
                result = this.QueryEditFiles((tagVSQueryEditFlags)editFlags, fileCount, fileNames, fileFlags, fileInfos, out callbackEditResult, out callbackMoreInfo);
                editResult = (uint)callbackEditResult;
                moreInfo = (uint)callbackMoreInfo;
            }

            return result;
        }

        int IVsQueryEditQuerySave2.QuerySaveFiles(uint saveFlags, int fileCount, string[] fileNames, uint[] fileFlags, VSQEQS_FILE_ATTRIBUTE_DATA[] fileInfos, out uint saveResult)
        {
            int result = VSConstants.S_OK;
            saveResult = 0;

            if (this.QuerySaveFiles != null)
            {
                tagVSQuerySaveResult callbackSaveResult;
                result = this.QuerySaveFiles((tagVSQuerySaveFlags)saveFlags, fileCount, fileNames, fileFlags, fileInfos, out callbackSaveResult);
                saveResult = (uint)callbackSaveResult;
            }

            return result;
        }

        int IVsQueryEditQuerySave2.QuerySaveFile(string fileName, uint fileFlags, VSQEQS_FILE_ATTRIBUTE_DATA[] fileInfo, out uint saveResult)
        {
            throw new NotImplementedException();
        }

        int IVsQueryEditQuerySave2.DeclareReloadableFile(string fileName, uint fileFlags, VSQEQS_FILE_ATTRIBUTE_DATA[] fileInfo)
        {
            throw new NotImplementedException();
        }

        int IVsQueryEditQuerySave2.DeclareUnreloadableFile(string fileName, uint fileFlags, VSQEQS_FILE_ATTRIBUTE_DATA[] fileInfo)
        {
            throw new NotImplementedException();
        }

        int IVsQueryEditQuerySave2.OnAfterSaveUnreloadableFile(string fileName, uint fileFlags, VSQEQS_FILE_ATTRIBUTE_DATA[] fileInfo)
        {
            throw new NotImplementedException();
        }

        int IVsQueryEditQuerySave2.IsReloadable(string fileName, out int result)
        {
            throw new NotImplementedException();
        }

        int IVsQueryEditQuerySave2.BeginQuerySaveBatch()
        {
            return VSConstants.S_OK;
        }

        int IVsQueryEditQuerySave2.EndQuerySaveBatch()
        {
            return VSConstants.S_OK;
        }

        #endregion
    }
}