// <copyright file="TemplateCompletionHandlerTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.Editor
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.OLE.Interop;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using NSubstitute;

    [TestClass]
    public class TemplateCompletionHandlerTest
    {
        [TestMethod]
        public void TemplateCompletionHandlerIsInternalBecauseItIsOnlyMeantToBeImportedByVisualStudio()
        {
            Assert.IsFalse(typeof(TemplateCompletionHandler).IsPublic);
        }

        [TestMethod]
        public void TemplateCompletionHandlerIsSealedBecauseItIsNotMeantToHaveChildClasses()
        {
            Assert.IsTrue(typeof(TemplateCompletionHandler).IsSealed);
        }

        [TestMethod]
        public void TemplateCompletionHandlerImplementsIOleCommandTargetToInterceptKeystrokes()
        {
            Assert.AreEqual(typeof(IOleCommandTarget), typeof(TemplateCompletionHandler).GetInterfaces()[0]);
        }

        [TestMethod, Ignore]
        public void TestExecMethod()
        {
            // TODO
        }

        [SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "Microsoft.VisualStudio.OLE.Interop.IOleCommandTarget.QueryStatus(System.Guid@,System.UInt32,Microsoft.VisualStudio.OLE.Interop.OLECMD[],System.IntPtr)", Justification = "This test does not call QueryStatus, it only asserts it was called.")]
        [TestMethod]
        public void QueryStatusCallsNextCommandTargetToSupportDefaultCommandHandling()
        {
            var nextHandler = Substitute.For<IOleCommandTarget>();
            var handler = new TemplateCompletionHandler { NextHandler = nextHandler };

            Guid commandGroup = Guid.Empty;
            handler.QueryStatus(ref commandGroup, 0, null, IntPtr.Zero);

            nextHandler.Received().QueryStatus(ref commandGroup, 0, null, IntPtr.Zero);
        }
    }
}