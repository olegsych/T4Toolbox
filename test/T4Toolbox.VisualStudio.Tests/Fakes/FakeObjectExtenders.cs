// <copyright file="FakeObjectExtenders.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.Tests.Fakes
{
    using System;
    using System.Collections.Generic;
    using EnvDTE;

    internal class FakeObjectExtenders : ObjectExtenders
    {
        private readonly Dictionary<int, IExtenderProvider> extenderProviders = new Dictionary<int, IExtenderProvider>();
        private int lastCookie = 0;

        public FakeObjectExtenders(FakeDte dte)
        {
            this.Dte = dte;
            dte.AddService(typeof(ObjectExtenders), this, false);
        }

        public FakeDte Dte { get; private set; }

        public int RegisterExtenderProvider(IExtenderProvider extenderProvider)
        {
            int cookie = this.lastCookie + 1;
            this.extenderProviders.Add(cookie, extenderProvider);
            this.lastCookie = cookie;
            return cookie;
        }

        public void UnregisterExtenderProvider(int cookie)
        {
            this.extenderProviders.Remove(cookie);
        }

        #region EnvDTE.ObjectExtenders

        int ObjectExtenders.RegisterExtenderProvider(string extenderCategory, string extenderName, IExtenderProvider extenderProvider, string localizedName)
        {
            return this.RegisterExtenderProvider(extenderProvider);
        }

        void ObjectExtenders.UnregisterExtenderProvider(int cookie)
        {
            this.UnregisterExtenderProvider(cookie);
        }

        object ObjectExtenders.GetExtender(string extenderCategory, string extenderName, object extendeeObject)
        {
            throw new NotImplementedException();
        }

        object ObjectExtenders.GetExtenderNames(string extenderCategory, object extendeeObject)
        {
            throw new NotImplementedException();
        }

        object ObjectExtenders.GetContextualExtenderCATIDs()
        {
            throw new NotImplementedException();
        }

        string ObjectExtenders.GetLocalizedExtenderName(string extenderCategory, string extenderName)
        {
            throw new NotImplementedException();
        }

        int ObjectExtenders.RegisterExtenderProviderUnk(string extenderCategory, string extenderName, IExtenderProviderUnk extenderProvider, string localizedName)
        {
            throw new NotImplementedException();
        }

        DTE ObjectExtenders.DTE
        {
            get { return this.Dte; }
        }

        DTE ObjectExtenders.Parent
        {
            get { return this.Dte; }
        }

        #endregion
    }
}