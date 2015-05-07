// <copyright file="FakeProjectItems.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.Tests.Fakes
{
    using System;
    using System.Collections;
    using System.Collections.ObjectModel;
    using System.Diagnostics;

    using EnvDTE;

    internal class FakeProjectItems : Collection<FakeProjectItem>, ProjectItems
    {
        private object parent;

        public FakeProjectItems(object parent)
        {
            Debug.Assert(parent != null, "parent");
            this.parent = parent;
        }

        public FakeProjectItem AddFromFile(string fileName)
        {
            var project = this.parent as FakeProject;
            if (project != null)
            {
                return new FakeProjectItem(project, fileName);                
            }

            return new FakeProjectItem((FakeProjectItem)this.parent);
        }

        #region ProjectItems

        Project ProjectItems.ContainingProject
        {
            get { throw new NotImplementedException(); }
        }

        int ProjectItems.Count
        {
            get { throw new NotImplementedException(); }
        }

        DTE ProjectItems.DTE
        {
            get { throw new NotImplementedException(); }
        }

        string ProjectItems.Kind
        {
            get { throw new NotImplementedException(); }
        }

        object ProjectItems.Parent
        {
            get { return this.parent; }
        }

        ProjectItem ProjectItems.AddFolder(string name, string kind)
        {
            throw new NotImplementedException();
        }

        ProjectItem ProjectItems.AddFromDirectory(string directory)
        {
            throw new NotImplementedException();
        }

        ProjectItem ProjectItems.AddFromFile(string fileName)
        {
            return this.AddFromFile(fileName);
        }

        ProjectItem ProjectItems.AddFromFileCopy(string filePath)
        {
            throw new NotImplementedException();
        }

        ProjectItem ProjectItems.AddFromTemplate(string fileName, string name)
        {
            throw new NotImplementedException();
        }

        IEnumerator ProjectItems.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        ProjectItem ProjectItems.Item(object index)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
