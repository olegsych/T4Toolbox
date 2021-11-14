// <copyright file="FakeProperty.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.Tests.Fakes
{
    using System;

    using EnvDTE;

    internal class FakeProperty : Property
    {
        public string Name { get; set; }

        public object Value { get; set; }

        #region Property

        object Property.Application
        {
            get { throw new NotImplementedException(); }
        }

        Properties Property.Collection
        {
            get { throw new NotImplementedException(); }
        }

        DTE Property.DTE
        {
            get { throw new NotImplementedException(); }
        }

        string Property.Name
        {
            get { return this.Name; }
        }

        short Property.NumIndices
        {
            get { throw new NotImplementedException(); }
        }

        object Property.Object
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        Properties Property.Parent
        {
            get { throw new NotImplementedException(); }
        }

        object Property.Value
        {
            get { return this.Value; }
            set { this.Value = value; }
        }

        object Property.get_IndexedValue(object index1, object index2, object index3, object index4)
        {
            throw new NotImplementedException();
        }

        void Property.let_Value(object value)
        {
            throw new NotImplementedException();
        }

        void Property.set_IndexedValue(object index1, object index2, object index3, object index4, object value)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
