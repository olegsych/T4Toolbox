// <copyright file="OutputFileManagerTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.Tests
{
    using System;
    using System.IO;
    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.Shell.Interop;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.VisualStudio.TextTemplating.VSHost;
    using T4Toolbox.VisualStudio.Tests.Fakes;

    [TestClass]
    public class OutputFileManagerTest : IDisposable
    {
        private const string TestOutputContent = "42";

        private readonly FakeDte dte;
        private readonly FakeSolution solution;
        private readonly FakeProject project;
        private readonly FakeProjectItem projectItem;
        private readonly FakeTextTemplating textTemplating;
        private readonly StubIVsQueryEditQuerySave queryEditQuerySave;
        private readonly StubIVsRunningDocumentTable runningDocumentTable;

        public OutputFileManagerTest()
        {
            this.dte = new FakeDte();
            this.solution = new FakeSolution(this.dte);
            this.project = new FakeProject(this.solution);
            this.projectItem = new FakeProjectItem(this.project);
            this.textTemplating = new FakeTextTemplating();
            this.queryEditQuerySave = new StubIVsQueryEditQuerySave();
            this.runningDocumentTable = new StubIVsRunningDocumentTable();

            this.dte.AddService(typeof(STextTemplating), this.textTemplating, false);
            this.dte.AddService(typeof(SVsQueryEditQuerySave), this.queryEditQuerySave, false);
            this.dte.AddService(typeof(SVsRunningDocumentTable), this.runningDocumentTable, false);
        }

        public void Dispose()
        {
            this.dte.Dispose();
        }

        #region Source Control (Query Edit / QuerySave)

        [TestMethod]
        public void SaveFilesDoesNotThrowExceptionWhenSVsQueryEditQuerySaveServiceIsNotAvailable()
        {
            this.dte.RemoveService(typeof(SVsQueryEditQuerySave));
            new OutputFileManager(this.dte, this.projectItem.TestFile.FullName, new[] { new OutputFile { File = Path.GetRandomFileName() } }).DoWork();
            Assert.AreEqual(0, this.textTemplating.Errors.Count);
        }

        [TestMethod]
        public void SaveFilesPromptsUserToPerformSourceControlEditActionForOutputFiles()
        {
            string[] queryEditFileNames = null;
            this.queryEditQuerySave.QueryEditFiles = delegate(tagVSQueryEditFlags flags, int count, string[] fileNames, uint[] fileFlags, VSQEQS_FILE_ATTRIBUTE_DATA[] fileInfos, out tagVSQueryEditResult result, out tagVSQueryEditResultFlags moreInfo)
            { 
                queryEditFileNames = fileNames;
                result = tagVSQueryEditResult.QER_EditOK;
                moreInfo = tagVSQueryEditResultFlags.QER_MaybeCheckedout;
                return VSConstants.S_OK;
            };

            var outputFiles = new[] { new OutputFile { File = Path.GetRandomFileName() }, new OutputFile { File = Path.GetRandomFileName() } };
            new OutputFileManager(this.dte, this.projectItem.TestFile.FullName, outputFiles).DoWork();

            Assert.AreEqual(0, this.textTemplating.Errors.Count);
            Assert.AreEqual(2, queryEditFileNames.Length);
            Assert.AreEqual(outputFiles[0].File, Path.GetFileName(queryEditFileNames[0]));
            Assert.AreEqual(outputFiles[1].File, Path.GetFileName(queryEditFileNames[1]));
        }

        [TestMethod]
        public void SaveFilesPromptsUserToPerformSourceControlSaveActionWhenEditActionIsUnsuccessful()
        {
            this.queryEditQuerySave.QueryEditFiles = delegate(tagVSQueryEditFlags flags, int count, string[] fileNames, uint[] fileFlags, VSQEQS_FILE_ATTRIBUTE_DATA[] fileInfos, out tagVSQueryEditResult result, out tagVSQueryEditResultFlags moreInfo)
            {
                result = tagVSQueryEditResult.QER_EditNotOK;
                moreInfo = tagVSQueryEditResultFlags.QER_ReadOnlyUnderScc;
                return VSConstants.S_OK;
            };

            string[] querySaveFileNames = null;
            this.queryEditQuerySave.QuerySaveFiles = delegate(tagVSQuerySaveFlags flags, int count, string[] fileNames, uint[] fileFlags, VSQEQS_FILE_ATTRIBUTE_DATA[] fileIinfos, out tagVSQuerySaveResult result)
            {
                querySaveFileNames = fileNames;
                result = tagVSQuerySaveResult.QSR_SaveOK;
                return VSConstants.S_OK;
            };            

            var outputFiles = new[] { new OutputFile { File = Path.GetRandomFileName() }, new OutputFile { File = Path.GetRandomFileName() } };
            new OutputFileManager(this.dte, this.projectItem.TestFile.FullName, outputFiles).DoWork();

            Assert.AreEqual(0, this.textTemplating.Errors.Count);
            Assert.AreEqual(2, querySaveFileNames.Length);
            Assert.AreEqual(outputFiles[0].File, Path.GetFileName(querySaveFileNames[0]));
            Assert.AreEqual(outputFiles[1].File, Path.GetFileName(querySaveFileNames[1]));
        }

        [TestMethod]
        public void SaveFilesDoesNotPromptUserToPerformSourceControlSaveActionWhenEditActionIsCanceledByUser()
        {
            this.queryEditQuerySave.QueryEditFiles = delegate(tagVSQueryEditFlags flags, int count, string[] names, uint[] fileFlags, VSQEQS_FILE_ATTRIBUTE_DATA[] infos, out tagVSQueryEditResult result, out tagVSQueryEditResultFlags info)
            {
                result = tagVSQueryEditResult.QER_NoEdit_UserCanceled;
                info = tagVSQueryEditResultFlags.QER_CheckoutCanceledOrFailed;
                return VSConstants.S_OK;
            };

            bool querySaveCalled = false;
            this.queryEditQuerySave.QuerySaveFiles = delegate(tagVSQuerySaveFlags flags, int count, string[] names, uint[] fileFlags, VSQEQS_FILE_ATTRIBUTE_DATA[] infos, out tagVSQuerySaveResult result)
            { 
                querySaveCalled = true;
                result = tagVSQuerySaveResult.QSR_SaveOK;
                return VSConstants.S_OK;
            };

            new OutputFileManager(this.dte, this.projectItem.TestFile.FullName, new[] { new OutputFile { File = Path.GetRandomFileName() } }).DoWork();

            Assert.IsFalse(querySaveCalled);
        }

        [TestMethod]
        public void SaveFilesReportsErrorWhenSourceControlEditActionIsCanceledByUser()
        {
            this.queryEditQuerySave.QueryEditFiles = delegate(tagVSQueryEditFlags flags, int count, string[] names, uint[] fileFlags, VSQEQS_FILE_ATTRIBUTE_DATA[] infos, out tagVSQueryEditResult result, out tagVSQueryEditResultFlags info)
            {
                result = tagVSQueryEditResult.QER_NoEdit_UserCanceled;
                info = tagVSQueryEditResultFlags.QER_CheckoutCanceledOrFailed;
                return VSConstants.S_OK;
            };

            new OutputFileManager(this.dte, this.projectItem.TestFile.FullName, new[] { new OutputFile { File = Path.GetRandomFileName() } }).DoWork();

            Assert.AreEqual(1, this.textTemplating.Errors.Count);
            Assert.IsFalse(this.textTemplating.Errors[0].IsWarning);
        }

        [TestMethod]
        public void SaveFilesReportsErrorWhenSourceControlSaveActionIsCanceledByUser()
        {
            this.queryEditQuerySave.QueryEditFiles = delegate(tagVSQueryEditFlags flags, int count, string[] names, uint[] fileFlags, VSQEQS_FILE_ATTRIBUTE_DATA[] infos, out tagVSQueryEditResult result, out tagVSQueryEditResultFlags info)
            {
                result = tagVSQueryEditResult.QER_EditNotOK;
                info = tagVSQueryEditResultFlags.QER_ReadOnlyUnderScc;
                return VSConstants.S_OK;
            };

            this.queryEditQuerySave.QuerySaveFiles = delegate(tagVSQuerySaveFlags flags, int count, string[] names, uint[] fileFlags, VSQEQS_FILE_ATTRIBUTE_DATA[] infos, out tagVSQuerySaveResult result)
            {
                result = tagVSQuerySaveResult.QSR_NoSave_UserCanceled;
                return VSConstants.S_OK;
            };

            new OutputFileManager(this.dte, this.projectItem.TestFile.FullName, new[] { new OutputFile { File = Path.GetRandomFileName() } }).DoWork();

            Assert.AreEqual(1, this.textTemplating.Errors.Count);
            Assert.IsFalse(this.textTemplating.Errors[0].IsWarning);            
        }

        [TestMethod]
        public void SaveFilesWritesFilesToDiskWhenSourceControlEditActionIsSuccessful()
        {
            this.queryEditQuerySave.QueryEditFiles = delegate(tagVSQueryEditFlags flags, int count, string[] names, uint[] fileFlags, VSQEQS_FILE_ATTRIBUTE_DATA[] infos, out tagVSQueryEditResult result, out tagVSQueryEditResultFlags info)
            {
                result = tagVSQueryEditResult.QER_EditOK;
                info = tagVSQueryEditResultFlags.QER_MaybeCheckedout;
                return VSConstants.S_OK;
            };

            var output = new OutputFile { File = Path.GetRandomFileName() };
            output.Content.Append(TestOutputContent);
            new OutputFileManager(this.dte, this.projectItem.TestFile.FullName, new[] { output }).DoWork();

            Assert.AreEqual(TestOutputContent, File.ReadAllText(Path.Combine(Path.GetDirectoryName(this.projectItem.TestFile.FullName), output.File)));
        }

        [TestMethod]
        public void SaveFilesDoesNotWriteFilesToDiskWhenSourceControlEditActionIsCanceled()
        {
            this.queryEditQuerySave.QueryEditFiles = delegate(tagVSQueryEditFlags flags, int count, string[] names, uint[] fileFlags, VSQEQS_FILE_ATTRIBUTE_DATA[] infos, out tagVSQueryEditResult result, out tagVSQueryEditResultFlags info)
            {
                result = tagVSQueryEditResult.QER_NoEdit_UserCanceled;
                info = tagVSQueryEditResultFlags.QER_CheckoutCanceledOrFailed;
                return VSConstants.S_OK;
            };

            var output = new OutputFile { File = Path.GetRandomFileName() };
            output.Content.Append(TestOutputContent);
            new OutputFileManager(this.dte, this.projectItem.TestFile.FullName, new[] { output }).DoWork();

            Assert.IsFalse(File.Exists(Path.Combine(Path.GetDirectoryName(this.projectItem.TestFile.FullName), output.File)));            
        }

        [TestMethod]
        public void SaveFilesWriteFilesToDiskWhenSourceControlSaveActionIsSuccessful()
        {
            this.queryEditQuerySave.QueryEditFiles = delegate(tagVSQueryEditFlags flags, int count, string[] names, uint[] fileFlags, VSQEQS_FILE_ATTRIBUTE_DATA[] infos, out tagVSQueryEditResult result, out tagVSQueryEditResultFlags info)
            {
                result = tagVSQueryEditResult.QER_EditNotOK;
                info = tagVSQueryEditResultFlags.QER_ReadOnlyUnderScc;
                return VSConstants.S_OK;
            };

            this.queryEditQuerySave.QuerySaveFiles = delegate(tagVSQuerySaveFlags flags, int count, string[] names, uint[] fileFlags, VSQEQS_FILE_ATTRIBUTE_DATA[] infos, out tagVSQuerySaveResult result)
            {
                result = tagVSQuerySaveResult.QSR_SaveOK;
                return VSConstants.S_OK;
            };

            var output = new OutputFile { File = Path.GetRandomFileName() };
            output.Content.Append(TestOutputContent);
            new OutputFileManager(this.dte, this.projectItem.TestFile.FullName, new[] { output }).DoWork();

            Assert.AreEqual(TestOutputContent, File.ReadAllText(Path.Combine(Path.GetDirectoryName(this.projectItem.TestFile.FullName), output.File)));
        }

        [TestMethod]
        public void SaveFilesDoesNotWriteFilesToDiskWhenSourceControlSaveActionIsUnsuccessful()
        {
            this.queryEditQuerySave.QueryEditFiles = delegate(tagVSQueryEditFlags flags, int count, string[] names, uint[] fileFlags, VSQEQS_FILE_ATTRIBUTE_DATA[] infos, out tagVSQueryEditResult result, out tagVSQueryEditResultFlags info)
            {
                result = tagVSQueryEditResult.QER_EditNotOK;
                info = tagVSQueryEditResultFlags.QER_ReadOnlyUnderScc;
                return VSConstants.S_OK;
            };

            this.queryEditQuerySave.QuerySaveFiles = delegate(tagVSQuerySaveFlags flags, int count, string[] names, uint[] fileFlags, VSQEQS_FILE_ATTRIBUTE_DATA[] infos, out tagVSQuerySaveResult result)
            {
                result = tagVSQuerySaveResult.QSR_NoSave_UserCanceled;
                return VSConstants.S_OK;
            };

            var output = new OutputFile { File = Path.GetRandomFileName() };
            output.Content.Append(TestOutputContent);
            new OutputFileManager(this.dte, this.projectItem.TestFile.FullName, new[] { output }).DoWork();

            Assert.IsFalse(File.Exists(Path.Combine(Path.GetDirectoryName(this.projectItem.TestFile.FullName), output.File)));
        }

        #endregion

        #region Document Reloading

        [TestMethod]
        public void DocumentIsNotReloadedWhenSVsRunningDocumentTableServiceIsNotAvailable()
        {
            this.dte.RemoveService(typeof(SVsRunningDocumentTable));
            new OutputFileManager(this.dte, this.projectItem.TestFile.FullName, new[] { new OutputFile { File = Path.GetRandomFileName() } }).DoWork();
            Assert.AreEqual(0, this.textTemplating.Errors.Count);            
        }

        [TestMethod]
        public void DocumentNotReloadedWhenGeneratedOutputIsNotOpenInVisualStudioEditor()
        {
            string attemptedFileName = string.Empty;
            this.runningDocumentTable.FindAndLockDocument = delegate(_VSRDTFLAGS flags, string fileName, out IVsHierarchy hierarchy, out uint itemId, out IVsPersistDocData docData, out uint cookie)
            {
                attemptedFileName = fileName;

                hierarchy = null;
                itemId = 0;
                docData = null;
                cookie = 0;
                return VSConstants.S_OK;
            };

            var output = new OutputFile { File = Path.GetRandomFileName() };
            new OutputFileManager(this.dte, this.projectItem.TestFile.FullName, new[] { output }).DoWork();

            Assert.AreEqual(output.File, Path.GetFileName(attemptedFileName));
            Assert.AreEqual(0, this.textTemplating.Errors.Count);
        }

        [TestMethod]
        public void DocumentIsReloadedWhenGeneratedOutputIsOpenInVisualStudioEditor()
        {
            bool documentReloaded = false;
            var stubDocData = new StubIPersistDocData
            {
                ReloadDocData = delegate(uint flags)
                {
                    documentReloaded = true;
                    return VSConstants.S_OK;
                }
            };

            this.runningDocumentTable.FindAndLockDocument = delegate(_VSRDTFLAGS flags, string fileName, out IVsHierarchy hierarchy, out uint itemId, out IVsPersistDocData docData, out uint cookie)
            {
                hierarchy = null;
                itemId = 0;
                docData = stubDocData;
                cookie = 0;
                return VSConstants.S_OK;
            };

            new OutputFileManager(this.dte, this.projectItem.TestFile.FullName, new[] { new OutputFile { File = Path.GetRandomFileName() } }).DoWork();

            Assert.IsTrue(documentReloaded);
        }

        #endregion
    }
}