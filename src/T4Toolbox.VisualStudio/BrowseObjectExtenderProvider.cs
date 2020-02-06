﻿// <copyright file="BrowseObjectExtenderProvider.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio
{
    using System;

    using EnvDTE;

    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Shell.Interop;

    /// <summary>
    /// Provides a "Template" property extender for C# and Visual Basic project items.
    /// </summary>
    internal sealed class BrowseObjectExtenderProvider : IExtenderProvider, IDisposable
    {
        private const string ExtenderName = "T4Toolbox";

        private readonly string extenderCategory;
        private readonly ObjectExtenders objectExtenders;
        private readonly int providerCookie;
        private readonly IAsyncServiceProvider2 serviceProvider;

        public BrowseObjectExtenderProvider(IAsyncServiceProvider2 serviceProvider, ObjectExtenders objectExtenders, string extenderCategory)
        {
            this.serviceProvider = serviceProvider;
            this.objectExtenders = objectExtenders;
            this.extenderCategory = extenderCategory;
            this.providerCookie = this.objectExtenders.RegisterExtenderProvider(extenderCategory, BrowseObjectExtenderProvider.ExtenderName, this);
        }

        public bool CanExtend(string extenderCategory, string extenderName, object extendee)
        {
            return extenderCategory == this.extenderCategory && extenderName == ExtenderName && extendee is IVsBrowseObject;
        }

        public void Dispose()
        {
            this.objectExtenders.UnregisterExtenderProvider(this.providerCookie);
        }

        public object GetExtender(string extenderCategory, string extenderName, object extendee, IExtenderSite site, int extenderCookie)
        {
            return new BrowseObjectExtender(this.serviceProvider, (IVsBrowseObject)extendee, site, extenderCookie);
        }
    }
}