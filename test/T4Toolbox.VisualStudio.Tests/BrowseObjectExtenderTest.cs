// <copyright file="BrowseObjectExtenderTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.Tests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.InteropServices;
    using EnvDTE;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using T4Toolbox.VisualStudio.Tests.Fakes;

    [TestClass]
    public sealed class BrowseObjectExtenderTest : IDisposable
    {
        private readonly FakeDte dte;
        private readonly FakeSolution solution;
        private readonly FakeProject project;
        private readonly FakeProjectItem projectItem;
        private readonly StubExtenderSite extenderSite;

        public BrowseObjectExtenderTest()
        {
            this.dte = new FakeDte();
            this.solution = new FakeSolution(this.dte);
            this.project = new FakeProject(this.solution);
            this.projectItem = new FakeProjectItem(this.project);
            this.extenderSite = new StubExtenderSite();
        }

        public void Dispose()
        {
            this.dte.Dispose();
        }

        [TestMethod]
        [SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "T4Toolbox.VisualStudio.BrowseObjectExtender", Justification = "We need to create a new instance to trigger its finalizer.")]
        [SuppressMessage("Microsoft.Reliability", "CA2001:AvoidCallingProblematicMethods", MessageId = "System.GC.Collect", Justification = "This method tests the finalizer logic.")]
        public void FinalizerNotifiesSiteToPreventVisualStudioFromCrashing()
        {
            int deletedExtenderCookie = 0;
            this.extenderSite.NotifyDelete = cookie => deletedExtenderCookie = cookie;

            new BrowseObjectExtender(this.dte, this.projectItem, this.extenderSite, 42);
            GC.Collect(2, GCCollectionMode.Forced, blocking: true);
            GC.WaitForPendingFinalizers();

            Assert.AreEqual(42, deletedExtenderCookie);
        }

        [TestMethod]
        [SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "T4Toolbox.VisualStudio.BrowseObjectExtender", Justification = "We need to create a new instance to trigger its finalizer.")]
        [SuppressMessage("Microsoft.Reliability", "CA2001:AvoidCallingProblematicMethods", MessageId = "System.GC.Collect", Justification = "This method tests the finalizer logic.")]
        public void FinalizerCatchesInvalidComObjectExceptionToPreventErrorsWhenVisualStudioShutsDown()
        {
            object exceptionObject = null;

            UnhandledExceptionEventHandler unhandledExceptionHandler = (sender, args) => exceptionObject = args.ExceptionObject;
            AppDomain.CurrentDomain.UnhandledException += unhandledExceptionHandler;
            try
            {
                this.extenderSite.NotifyDelete = cookie => { throw new InvalidComObjectException(); };
                new BrowseObjectExtender(this.dte, this.projectItem, this.extenderSite, 42);
                GC.Collect(2, GCCollectionMode.Forced, blocking: true);
                GC.WaitForPendingFinalizers();
            }
            finally
            {
                AppDomain.CurrentDomain.UnhandledException -= unhandledExceptionHandler;
            }

            Assert.IsNull(exceptionObject);
        }

        private class StubExtenderSite : IExtenderSite
        {
            public Action<int> NotifyDelete { get; set; }

            void IExtenderSite.NotifyDelete(int cookie)
            {
                if (this.NotifyDelete != null)
                {
                    this.NotifyDelete(cookie);
                }
            }

            object IExtenderSite.GetObject(string name)
            {
                throw new System.NotImplementedException();
            }
        }
    }
}