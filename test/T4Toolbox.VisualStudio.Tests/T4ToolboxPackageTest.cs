// <copyright file="T4ToolboxPackageTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.Tests
{
    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.Shell.Interop;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using T4Toolbox.VisualStudio.Fakes;
    using T4Toolbox.VisualStudio.Tests.Fakes;

    [TestClass]
    public class T4ToolboxPackageTest
    {
        [TestMethod]
        public void SetSiteCompletesSuccessfully()
        {
            using (var serviceProvider = new FakeDte())
            using (var package = new T4ToolboxPackage())
            {
                serviceProvider.AddService(typeof(SVsActivityLog), new FakeVsActivityLog(), false);
                IVsPackage vspackage = package;
                Assert.AreEqual(VSConstants.S_OK, vspackage.SetSite(serviceProvider));
            }
        }
    }
}
