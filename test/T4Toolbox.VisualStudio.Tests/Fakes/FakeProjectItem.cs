// <copyright file="FakeProjectItem.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.Tests.Fakes
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using EnvDTE;
    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.Shell.Interop;

    internal class FakeProjectItem : ProjectItem, IVsBrowseObject
    {
        private FileInfo testFile;

        public FakeProjectItem()
        {
            this.Properties = new FakeProperties();
            this.Metadata = new Dictionary<string, string>();
            this.ProjectItems = new FakeProjectItems(this);
        }

        public FakeProjectItem(FakeProject project) : this()
        {
            Debug.Assert(project != null, "project");

            this.ContainingProject = project;
            this.Dte = project.Dte;
            this.Id = project.ProjectItems.Select(item => item.Id).DefaultIfEmpty().Max() + 1;
            project.ProjectItems.Add(this);
        }

        public FakeProjectItem(FakeProjectItem parent) : this()
        {
            Debug.Assert(parent != null, "parent");

            this.ContainingProject = parent.ContainingProject;
            this.Dte = parent.Dte;
            this.Id = this.ContainingProject.ProjectItems.Select(item => item.Id).DefaultIfEmpty().Max() + 1;
            parent.ProjectItems.Add(this);
        }

        public FakeProjectItem(FakeProject project, string fileName) : this(project)
        {
            Debug.Assert(!string.IsNullOrEmpty(fileName), "fileName");

            this.testFile = this.CreateTestFile(fileName);
        }

        public FakeProject ContainingProject { get; private set; }

        public FakeDte Dte { get; private set; }

        public uint Id { get; set; }
        
        public IDictionary<string, string> Metadata { get; private set; }

        public string Name
        {
            get { return this.TestFile.Name; }
        }

        public FakeProjectItems ProjectItems { get; private set; }

        public FakeProperties Properties { get; private set; }

        #region ProjectItem

        ProjectItems ProjectItem.Collection
        {
            get { throw new NotImplementedException(); }
        }

        ConfigurationManager ProjectItem.ConfigurationManager
        {
            get { throw new NotImplementedException(); }
        }

        Project ProjectItem.ContainingProject
        {
            get { return this.ContainingProject; }
        }

        DTE ProjectItem.DTE
        {
            get { return this.Dte; }
        }

        void ProjectItem.Delete()
        {
            throw new NotImplementedException();
        }

        Document ProjectItem.Document
        {
            get { throw new NotImplementedException(); }
        }

        void ProjectItem.ExpandView()
        {
            throw new NotImplementedException();
        }

        string ProjectItem.ExtenderCATID
        {
            get { throw new NotImplementedException(); }
        }

        object ProjectItem.ExtenderNames
        {
            get { throw new NotImplementedException(); }
        }

        FileCodeModel ProjectItem.FileCodeModel
        {
            get { throw new NotImplementedException(); }
        }

        short ProjectItem.FileCount
        {
            get { throw new NotImplementedException(); }
        }

        bool ProjectItem.IsDirty
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        string ProjectItem.Kind
        {
            get { throw new NotImplementedException(); }
        }

        string ProjectItem.Name
        {
            get { return this.Name; }
            set { throw new NotImplementedException(); }
        }

        object ProjectItem.Object
        {
            get { throw new NotImplementedException(); }
        }

        Window ProjectItem.Open(string viewKind)
        {
            throw new NotImplementedException();
        }

        ProjectItems ProjectItem.ProjectItems
        {
            get { return this.ProjectItems; }
        }

        Properties ProjectItem.Properties
        {
            get { return this.Properties; }
        }

        void ProjectItem.Remove()
        {
            throw new NotImplementedException();
        }

        void ProjectItem.Save(string fileName)
        {
            throw new NotImplementedException();
        }

        bool ProjectItem.SaveAs(string newFileName)
        {
            throw new NotImplementedException();
        }

        bool ProjectItem.Saved
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        Project ProjectItem.SubProject
        {
            get { throw new NotImplementedException(); }
        }

        object ProjectItem.get_Extender(string extenderName)
        {
            throw new NotImplementedException();
        }

        string ProjectItem.get_FileNames(short index)
        {
            return this.TestFile.FullName;
        }

        bool ProjectItem.get_IsOpen(string viewKind)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IVsBrowseObject

        int IVsBrowseObject.GetProjectItem(out IVsHierarchy hierarhcy, out uint itemId)
        {
            hierarhcy = this.ContainingProject;
            itemId = this.Id;
            return VSConstants.S_OK;
        }

        #endregion

        internal FileInfo TestFile
        {
            get { return this.testFile ?? (this.testFile = this.CreateTestFile(Path.GetRandomFileName())); }
        }

        private FileInfo CreateTestFile(string fileName)
        {
            var file = new FileInfo(Path.Combine(this.ContainingProject.TestDirectory.FullName, fileName));
            File.WriteAllText(file.FullName, string.Empty);
            return file;
        }
    }
}
