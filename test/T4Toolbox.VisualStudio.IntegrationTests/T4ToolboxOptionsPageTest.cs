// <copyright file="T4ToolboxOptionsPageTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.IntegrationTests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public sealed class T4ToolboxOptionsPageTest
    {
        [TestMethod]
        public void AutomationObjectReturnsCloneOfGlobalOptions()
        {
            using (var page = new T4ToolboxOptionsPage())
            {
                Assert.AreNotSame(T4ToolboxOptions.Instance, page.AutomationObject);                
            }
        }

        [TestMethod]
        public void SaveSettingsToStorageUpdatesGlobalOptions()
        {
            var globalOptions = new T4ToolboxOptions();
            using (var page = new TestableT4ToolboxOptionsPage(globalOptions))
            {
                var localOptions = (T4ToolboxOptions)page.AutomationObject;
                localOptions.SyntaxColorizationEnabled = !globalOptions.SyntaxColorizationEnabled;
                page.SaveSettingsToStorage();
                Assert.AreEqual(localOptions.SyntaxColorizationEnabled, globalOptions.SyntaxColorizationEnabled);
            }
        }

        [TestMethod]
        public void LoadSettingsFromStorageUpdatesLocalOptions()
        {
            var globalOptions = new T4ToolboxOptions();
            using (var page = new TestableT4ToolboxOptionsPage(globalOptions))
            {
                var localOptions = (T4ToolboxOptions)page.AutomationObject;
                localOptions.SyntaxColorizationEnabled = !globalOptions.SyntaxColorizationEnabled;
                page.LoadSettingsFromStorage();
                Assert.AreEqual(globalOptions.SyntaxColorizationEnabled, localOptions.SyntaxColorizationEnabled);
            }            
        }

        private class TestableT4ToolboxOptionsPage : T4ToolboxOptionsPage
        {
            private readonly T4ToolboxOptions globalOptions;

            public TestableT4ToolboxOptionsPage(T4ToolboxOptions globalOptions)
            {
                this.globalOptions = globalOptions;
            }

            protected override T4ToolboxOptions GetGlobalOptions()
            {
                return this.globalOptions;
            }
        }
    } 
}