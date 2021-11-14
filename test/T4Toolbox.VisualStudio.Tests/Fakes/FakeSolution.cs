// <copyright file="FakeSolution.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.Tests.Fakes
{
    using System;
    using System.Collections;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using EnvDTE;
    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.Shell.Interop;

    internal class FakeSolution : Solution, IVsSolution, SVsSolution
    {
        private DirectoryInfo testDirectory;

        public FakeSolution(FakeDte dte)
        {
            Debug.Assert(dte != null, "dte");

            this.Dte = dte;
            this.Projects = new FakeProjects();

            dte.Solution = this;
            dte.AddService(typeof(SVsSolution), this, false);
        }

        public FakeDte Dte { get; private set; }

        public FakeProjects Projects { get; private set; }

        public FakeProjectItem FindProjectItem(string fileName)
        {
            foreach (FakeProject project in this.Projects)
            {
                FakeProjectItem projectItem = project.ProjectItems.FirstOrDefault(pi => pi.TestFile.FullName == fileName);
                if (projectItem != null)
                {
                    return projectItem;
                }
            }

            return null;
        }

        public FakeProject GetProjectOfUniqueName(string uniqueName)
        {
            return this.Projects.Single(p => p.UniqueName == uniqueName);
        }

        internal DirectoryInfo TestDirectory
        {
            get { return this.testDirectory ?? (this.testDirectory = Directory.CreateDirectory(Path.Combine(this.Dte.TestDirectory.FullName, Path.GetRandomFileName()))); }
        }

        #region _Solution

        Project _Solution.AddFromFile(string fileName, bool exclusive)
        {
            throw new NotImplementedException();
        }

        Project _Solution.AddFromTemplate(string fileName, string destination, string projectName, bool exclusive)
        {
            throw new NotImplementedException();
        }

        AddIns _Solution.AddIns
        {
            get { throw new NotImplementedException(); }
        }

        void _Solution.Close(bool saveFirst)
        {
            throw new NotImplementedException();
        }

        int _Solution.Count
        {
            get { throw new NotImplementedException(); }
        }

        void _Solution.Create(string destination, string name)
        {
            throw new NotImplementedException();
        }

        DTE _Solution.DTE
        {
            get { return this.Dte; }
        }

        string _Solution.ExtenderCATID
        {
            get { throw new NotImplementedException(); }
        }

        object _Solution.ExtenderNames
        {
            get { throw new NotImplementedException(); }
        }

        string _Solution.FileName
        {
            get { throw new NotImplementedException(); }
        }

        ProjectItem _Solution.FindProjectItem(string fileName)
        {
            return this.FindProjectItem(fileName);
        }

        string _Solution.FullName
        {
            get { throw new NotImplementedException(); }
        }

        IEnumerator _Solution.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        Globals _Solution.Globals
        {
            get { throw new NotImplementedException(); }
        }

        bool _Solution.IsDirty
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        bool _Solution.IsOpen
        {
            get { throw new NotImplementedException(); }
        }

        Project _Solution.Item(object index)
        {
            throw new NotImplementedException();
        }

        void _Solution.Open(string fileName)
        {
            throw new NotImplementedException();
        }

        DTE _Solution.Parent
        {
            get { throw new NotImplementedException(); }
        }

        string _Solution.ProjectItemsTemplatePath(string projectKind)
        {
            throw new NotImplementedException();
        }

        Projects _Solution.Projects
        {
            get { return this.Projects; }
        }

        Properties _Solution.Properties
        {
            get { throw new NotImplementedException(); }
        }

        void _Solution.Remove(Project proj)
        {
            throw new NotImplementedException();
        }

        void _Solution.SaveAs(string fileName)
        {
            throw new NotImplementedException();
        }

        bool _Solution.Saved
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        SolutionBuild _Solution.SolutionBuild
        {
            get { throw new NotImplementedException(); }
        }

        object _Solution.get_Extender(string extenderName)
        {
            throw new NotImplementedException();
        }

        string _Solution.get_TemplatePath(string projectType)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion;

        #region IVsSolution

        int IVsSolution.AddVirtualProject(IVsHierarchy hierarchy, uint flags)
        {
            throw new NotImplementedException();
        }

        int IVsSolution.AddVirtualProjectEx(IVsHierarchy hierarchy, uint flags, ref Guid projectId)
        {
            throw new NotImplementedException();
        }

        int IVsSolution.AdviseSolutionEvents(IVsSolutionEvents sink, out uint cookie)
        {
            throw new NotImplementedException();
        }

        int IVsSolution.CanCreateNewProjectAtLocation(int createNewSolution, string fullProjectFilePath, out int canCreate)
        {
            throw new NotImplementedException();
        }

        int IVsSolution.CloseSolutionElement(uint closeOptions, IVsHierarchy hierarchy, uint cookie)
        {
            throw new NotImplementedException();
        }

        int IVsSolution.CreateNewProjectViaDlg(string expand, string select, uint reserved)
        {
            throw new NotImplementedException();
        }

        int IVsSolution.CreateProject(ref Guid projectType, string moniker, string location, string name, uint createFlags, ref Guid projectId, out IntPtr projectPtr)
        {
            throw new NotImplementedException();
        }

        int IVsSolution.CreateSolution(string location, string name, uint flags)
        {
            throw new NotImplementedException();
        }

        int IVsSolution.GenerateNextDefaultProjectName(string baseName, string location, out string projectName)
        {
            throw new NotImplementedException();
        }

        int IVsSolution.GenerateUniqueProjectName(string root, out string projectName)
        {
            throw new NotImplementedException();
        }

        int IVsSolution.GetGuidOfProject(IVsHierarchy hierarchy, out Guid projectId)
        {
            throw new NotImplementedException();
        }

        int IVsSolution.GetItemInfoOfProjref(string projref, int propid, out object result)
        {
            throw new NotImplementedException();
        }

        int IVsSolution.GetItemOfProjref(string projref, out IVsHierarchy hierarchy, out uint itemId, out string updatedProjref, VSUPDATEPROJREFREASON[] updateReason)
        {
            throw new NotImplementedException();
        }

        int IVsSolution.GetProjectEnum(uint enumFlags, ref Guid enumOnlyThisType, out IEnumHierarchies enumHierarchies)
        {
            throw new NotImplementedException();
        }

        int IVsSolution.GetProjectFactory(uint reserved, Guid[] projectType, string project, out IVsProjectFactory projectFactory)
        {
            throw new NotImplementedException();
        }

        int IVsSolution.GetProjectFilesInSolution(uint options, uint projects, string[] projectNames, out uint projectsFetched)
        {
            throw new NotImplementedException();
        }

        int IVsSolution.GetProjectInfoOfProjref(string projref, int propid, out object pvar)
        {
            throw new NotImplementedException();
        }

        int IVsSolution.GetProjectOfGuid(ref Guid projectId, out IVsHierarchy hierarchy)
        {
            throw new NotImplementedException();
        }

        int IVsSolution.GetProjectOfProjref(string projref, out IVsHierarchy hierarchy, out string updatedProjref, VSUPDATEPROJREFREASON[] updateReason)
        {
            throw new NotImplementedException();
        }

        int IVsSolution.GetProjectOfUniqueName(string uniqueName, out IVsHierarchy hierarchy)
        {
            hierarchy = this.GetProjectOfUniqueName(uniqueName);
            return VSConstants.S_OK;
        }

        int IVsSolution.GetProjectTypeGuid(uint reserved, string project, out Guid pguidProjectType)
        {
            throw new NotImplementedException();
        }

        int IVsSolution.GetProjrefOfItem(IVsHierarchy hierarchy, uint itemid, out string projref)
        {
            throw new NotImplementedException();
        }

        int IVsSolution.GetProjrefOfProject(IVsHierarchy hierarchy, out string projref)
        {
            throw new NotImplementedException();
        }

        int IVsSolution.GetProperty(int propid, out object pvar)
        {
            throw new NotImplementedException();
        }

        int IVsSolution.GetSolutionInfo(out string solutionDirectory, out string solutionFile, out string userOptsFile)
        {
            throw new NotImplementedException();
        }

        int IVsSolution.GetUniqueNameOfProject(IVsHierarchy hierarchy, out string uniqueName)
        {
            throw new NotImplementedException();
        }

        int IVsSolution.GetVirtualProjectFlags(IVsHierarchy hierarchy, out uint flags)
        {
            throw new NotImplementedException();
        }

        int IVsSolution.OnAfterRenameProject(IVsProject project, string oldName, string newName, uint reserved)
        {
            throw new NotImplementedException();
        }

        int IVsSolution.OpenSolutionFile(uint options, string filename)
        {
            throw new NotImplementedException();
        }

        int IVsSolution.OpenSolutionViaDlg(string startDirectory, int defaultToAllProjectsFilter)
        {
            throw new NotImplementedException();
        }

        int IVsSolution.QueryEditSolutionFile(out uint editResult)
        {
            throw new NotImplementedException();
        }

        int IVsSolution.QueryRenameProject(IVsProject project, string oldName, string newName, uint reserved, out int renameCanContinue)
        {
            throw new NotImplementedException();
        }

        int IVsSolution.RemoveVirtualProject(IVsHierarchy hierarchy, uint flags)
        {
            throw new NotImplementedException();
        }

        int IVsSolution.SaveSolutionElement(uint options, IVsHierarchy hierarchy, uint cookie)
        {
            throw new NotImplementedException();
        }

        int IVsSolution.SetProperty(int propid, object var)
        {
            throw new NotImplementedException();
        }

        int IVsSolution.UnadviseSolutionEvents(uint cookie)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
