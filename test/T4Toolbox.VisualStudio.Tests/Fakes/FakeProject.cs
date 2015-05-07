// <copyright file="FakeProject.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.Tests.Fakes
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;

    using EnvDTE;

    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.Shell.Interop;

    internal class FakeProject : Project, IVsHierarchy, IVsBuildPropertyStorage
    {
        private readonly FakeSolution solution;
        private DirectoryInfo testDirectory;
        private FileInfo testFile;

        public FakeProject(FakeSolution solution)
        {
            this.solution = solution;
            this.Dte = solution.Dte;
            this.ProjectItems = new FakeProjectItems(this);
            this.UniqueName = Guid.NewGuid().ToString(); // Is this right?

            solution.Projects.Add(this);
        }

        public FakeDte Dte { get; private set; }

        public string Kind { get; set; }

        public FakeProjectItems ProjectItems { get; private set; }

        public string UniqueName { get; private set; }

        public string GetCanonicalName(uint itemId)
        {
            return this.ProjectItems.First(pi => pi.Id == itemId).TestFile.FullName;
        }

        public string GetItemAttribute(uint itemId, string attributeName)
        {
            return this.ProjectItems.First(pi => pi.Id == itemId).Metadata[attributeName];
        }

        public FakeProjectItem ParseCanonicalName(string name)
        {
            return this.ProjectItems.First(pi => pi.TestFile.FullName == name);
        }

        public void SetItemAttribute(uint itemId, string attributeName, string attributeValue)
        {
            FakeProjectItem item = this.ProjectItems.First(pi => pi.Id == itemId);
            if (string.IsNullOrEmpty(attributeValue))
            {
                item.Metadata.Remove(attributeName);
            }
            else
            {
                item.Metadata[attributeName] = attributeValue;                
            }
        }

        internal DirectoryInfo TestDirectory
        {
            get { return this.testDirectory ?? (this.testDirectory = Directory.CreateDirectory(Path.Combine(this.solution.TestDirectory.FullName, Path.GetRandomFileName()))); }
        }

        internal FileInfo TestFile
        {
            get 
            {
                if (this.testFile == null)
                {
                    this.testFile = new FileInfo(Path.Combine(this.TestDirectory.FullName, Path.GetRandomFileName()));
                    File.WriteAllText(this.testFile.FullName, string.Empty);
                }

                return this.testFile;
            }
        }

        #region Project

        CodeModel Project.CodeModel
        {
            get { throw new NotImplementedException(); }
        }

        Projects Project.Collection
        {
            get { throw new NotImplementedException(); }
        }

        ConfigurationManager Project.ConfigurationManager
        {
            get { throw new NotImplementedException(); }
        }

        DTE Project.DTE
        {
            get { return this.Dte; }
        }

        void Project.Delete()
        {
            throw new NotImplementedException();
        }

        string Project.ExtenderCATID
        {
            get { throw new NotImplementedException(); }
        }

        object Project.ExtenderNames
        {
            get { throw new NotImplementedException(); }
        }

        string Project.FileName
        {
            get { throw new NotImplementedException(); }
        }

        string Project.FullName
        {
            get { return this.TestFile.FullName; }
        }

        Globals Project.Globals
        {
            get { throw new NotImplementedException(); }
        }

        bool Project.IsDirty
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        string Project.Kind
        {
            get { return this.Kind; }
        }

        string Project.Name
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        object Project.Object
        {
            get { throw new NotImplementedException(); }
        }

        ProjectItem Project.ParentProjectItem
        {
            get { throw new NotImplementedException(); }
        }

        ProjectItems Project.ProjectItems
        {
            get { return this.ProjectItems; }
        }

        Properties Project.Properties
        {
            get { throw new NotImplementedException(); }
        }

        void Project.Save(string fileName)
        {
            throw new NotImplementedException();
        }

        void Project.SaveAs(string newFileName)
        {
            throw new NotImplementedException();
        }

        bool Project.Saved
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        string Project.UniqueName
        {
            get { return this.UniqueName; }
        }

        object Project.get_Extender(string extenderName)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IVsHierarchy

        int IVsHierarchy.AdviseHierarchyEvents(IVsHierarchyEvents eventSink, out uint cookie)
        {
            throw new NotImplementedException();
        }

        int IVsHierarchy.Close()
        {
            throw new NotImplementedException();
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "This method needs to return an HResult instead of letting exceptions escape.")]
        int IVsHierarchy.GetCanonicalName(uint itemId, out string name)
        {
            try
            {
                name = this.GetCanonicalName(itemId);
                return VSConstants.S_OK;
            }
            catch (Exception e)
            {
                name = null;
                return e.HResult;
            }
        }

        int IVsHierarchy.GetGuidProperty(uint itemid, int propid, out Guid guid)
        {
            throw new NotImplementedException();
        }

        int IVsHierarchy.GetNestedHierarchy(uint itemid, ref Guid nestedHierarchyGuid, out IntPtr nestedHierarchyPtr, out uint nestedItemId)
        {
            throw new NotImplementedException();
        }

        int IVsHierarchy.GetProperty(uint itemid, int propid, out object pvar)
        {
            throw new NotImplementedException();
        }

        int IVsHierarchy.GetSite(out Microsoft.VisualStudio.OLE.Interop.IServiceProvider serviceProvider)
        {
            throw new NotImplementedException();
        }

        int IVsHierarchy.ParseCanonicalName(string name, out uint itemId)
        {
            FakeProjectItem item = this.ParseCanonicalName(name);
            itemId = item.Id;
            return VSConstants.S_OK;
        }

        int IVsHierarchy.QueryClose(out int canClose)
        {
            throw new NotImplementedException();
        }

        int IVsHierarchy.SetGuidProperty(uint itemid, int propid, ref Guid guid)
        {
            throw new NotImplementedException();
        }

        int IVsHierarchy.SetProperty(uint itemid, int propid, object var)
        {
            throw new NotImplementedException();
        }

        int IVsHierarchy.SetSite(Microsoft.VisualStudio.OLE.Interop.IServiceProvider serviceProvider)
        {
            throw new NotImplementedException();
        }

        int IVsHierarchy.UnadviseHierarchyEvents(uint cookie)
        {
            throw new NotImplementedException();
        }

        int IVsHierarchy.Unused0()
        {
            throw new NotImplementedException();
        }

        int IVsHierarchy.Unused1()
        {
            throw new NotImplementedException();
        }

        int IVsHierarchy.Unused2()
        {
            throw new NotImplementedException();
        }

        int IVsHierarchy.Unused3()
        {
            throw new NotImplementedException();
        }

        int IVsHierarchy.Unused4()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IVsBuildPropertyStorage

        int IVsBuildPropertyStorage.GetItemAttribute(uint item, string attributeName, out string attributeValue)
        {
            try
            {
                attributeValue = this.GetItemAttribute(item, attributeName);
                return VSConstants.S_OK;
            }
            catch (KeyNotFoundException)
            {
                attributeValue = null;
                return -1; // what is the actual code VS returns here?
            }
        }

        int IVsBuildPropertyStorage.GetPropertyValue(string propName, string configName, uint storage, out string propValue)
        {
            throw new NotImplementedException();
        }

        int IVsBuildPropertyStorage.RemoveProperty(string propName, string configName, uint storage)
        {
            throw new NotImplementedException();
        }

        int IVsBuildPropertyStorage.SetItemAttribute(uint itemId, string attributeName, string attributeValue)
        {
            this.SetItemAttribute(itemId, attributeName, attributeValue);
            return VSConstants.S_OK;
        }

        int IVsBuildPropertyStorage.SetPropertyValue(string propName, string configName, uint storage, string propValue)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
