// <copyright file="FakeProjects.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.Tests.Fakes
{
    using System;
    using System.Collections;
    using System.Collections.ObjectModel;
    using EnvDTE;

    internal class FakeProjects : Collection<FakeProject>, Projects
    {
        #region Projects

        int Projects.Count
        {
            get { throw new NotImplementedException(); }
        }

        DTE Projects.DTE
        {
            get { throw new NotImplementedException(); }
        }

        IEnumerator Projects.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        Project Projects.Item(object index)
        {
            throw new NotImplementedException();
        }

        string Projects.Kind
        {
            get { throw new NotImplementedException(); }
        }

        DTE Projects.Parent
        {
            get { throw new NotImplementedException(); }
        }

        Properties Projects.Properties
        {
            get { throw new NotImplementedException(); }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
