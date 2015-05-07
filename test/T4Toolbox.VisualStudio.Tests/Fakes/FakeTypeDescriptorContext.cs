// <copyright file="FakeTypeDescriptorContext.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.Tests.Fakes
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;

    internal class FakeTypeDescriptorContext : ITypeDescriptorContext
    {
        private readonly IServiceProvider serviceProvider;
        private readonly object instance;

        public FakeTypeDescriptorContext(IServiceProvider serviceProvider, object instance)
        {
            Debug.Assert(serviceProvider != null, "serviceProvider");
            Debug.Assert(instance != null, "instance");

            this.serviceProvider = serviceProvider;
            this.instance = instance;
        }

        IContainer ITypeDescriptorContext.Container
        {
            get { throw new NotImplementedException(); }
        }

        object ITypeDescriptorContext.Instance
        {
            get { return this.instance; }
        }

        PropertyDescriptor ITypeDescriptorContext.PropertyDescriptor
        {
            get { throw new NotImplementedException(); }
        }

        object IServiceProvider.GetService(Type serviceType)
        {
            return this.serviceProvider.GetService(serviceType);
        }

        void ITypeDescriptorContext.OnComponentChanged()
        {
            throw new NotImplementedException();
        }

        bool ITypeDescriptorContext.OnComponentChanging()
        {
            throw new NotImplementedException();
        }
    }
}