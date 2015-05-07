// <copyright file="FakeProperties.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.Tests.Fakes
{
    using System;
    using System.Collections;
    using System.Collections.ObjectModel;
    using System.Linq;

    using EnvDTE;

    internal class FakeProperties : Collection<FakeProperty>, Properties
    {
        public FakeProperty Item(string name)
        {
            return this.FirstOrDefault(i => i.Name == name);
        }

        #region Properties

        object Properties.Application
        {
            get { throw new NotImplementedException(); }
        }

        int Properties.Count
        {
            get { throw new NotImplementedException(); }
        }

        DTE Properties.DTE
        {
            get { throw new NotImplementedException(); }
        }

        IEnumerator Properties.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        Property Properties.Item(object index)
        {
            return this.Item((string)index);
        }

        object Properties.Parent
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
