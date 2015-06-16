// <copyright file="T4ToolboxOptionsTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.Tests
{
    using System;
    using System.ComponentModel.Composition.Hosting;
    using System.IO;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Win32;
    using T4Toolbox.VisualStudio.Editor;

    [TestClass]
    public sealed class T4ToolboxOptionsTest : IDisposable
    {
        private string tempRegistryKeyName;

        public void Dispose()
        {
            if (this.tempRegistryKeyName != null)
            {
                Registry.CurrentUser.DeleteSubKeyTree(this.tempRegistryKeyName, throwOnMissingSubKey: false);                
            }
        }

        [TestMethod]
        public void ConstructorInitializesPropertiesWithValuesSpecifiedInDefaultValueAttribute()
        {
            var options = new T4ToolboxOptions();
            Assert.IsTrue(options.SyntaxColorizationEnabled);
        }

        [TestMethod]
        public void ConstructorInitializesPropertiesWithoutDefaultValueAttribute()
        {
            var options = new TestableT4ToolboxOptions(this.GetTestRegistryKey);
            Assert.IsFalse(options.BoolPropertyWithoutDefaultValueAttribute);
        }

        [TestMethod]
        public void ClassExportsITemplateEditorOptionsForConsumptionByEditorClasses()
        {
            var catalog = new TypeCatalog(typeof(T4ToolboxOptions));
            var container = new CompositionContainer(catalog);

            Lazy<ITemplateEditorOptions> export = container.GetExport<ITemplateEditorOptions>();
            Assert.IsInstanceOfType(export.Value, typeof(T4ToolboxOptions));
        }

        #region CompletionListsEnabled

        [TestMethod]
        public void CompletionListsEnabledIsTrueByDefault()
        {
            var options = new T4ToolboxOptions();
            Assert.IsTrue(options.CompletionListsEnabled);
        }

        [TestMethod]
        public void CompletionListsEnabledRaisesPropertyChangedEvent()
        {
            var options = new T4ToolboxOptions();
            string changedProperty = string.Empty;
            options.PropertyChanged += (sender, args) => { changedProperty = args.PropertyName; };
            options.CompletionListsEnabled = !options.CompletionListsEnabled;
            Assert.AreEqual("CompletionListsEnabled", changedProperty);
        }

        [TestMethod]
        public void CompletionListsEnabledDoesNotRaisePropertyChangedEventWhenNewValueIsSameAsOld()
        {
            var options = new T4ToolboxOptions();
            string changedProperty = string.Empty;
            options.PropertyChanged += (sender, args) => { changedProperty = args.PropertyName; };
            options.CompletionListsEnabled = options.CompletionListsEnabled;
            Assert.IsTrue(string.IsNullOrEmpty(changedProperty));
        }

        #endregion

        [TestMethod]
        public void CopyFromCopiesPropertyValues()
        {
            var source = new T4ToolboxOptions();
            source.SyntaxColorizationEnabled = false;

            var target = new T4ToolboxOptions();
            target.CopyFrom(source);

            Assert.AreEqual(source.SyntaxColorizationEnabled, target.SyntaxColorizationEnabled);
        }

        [TestMethod]
        public void ErrorReportingEnabledIsTrueByDefault()
        {
            var options = new T4ToolboxOptions();
            Assert.IsTrue(options.ErrorReportingEnabled);
        }

        [TestMethod]
        public void ErrorReportingEnabledRaisesPropertyChangedEvent()
        {
            var options = new T4ToolboxOptions();
            string changedProperty = string.Empty;
            options.PropertyChanged += (sender, args) => { changedProperty = args.PropertyName; };
            options.ErrorReportingEnabled = !options.ErrorReportingEnabled;
            Assert.AreEqual("ErrorReportingEnabled", changedProperty);
        }

        [TestMethod]
        public void ErrorReportingEnabledDoesNotRaisePropertyChangedEventWhenNewValueIsSameAsOld()
        {
            var options = new T4ToolboxOptions();
            string changedProperty = string.Empty;
            options.PropertyChanged += (sender, args) => { changedProperty = args.PropertyName; };
            options.ErrorReportingEnabled = options.ErrorReportingEnabled;
            Assert.IsTrue(string.IsNullOrEmpty(changedProperty));
        }        

        [TestMethod]
        public void ErrorUnderliningEnableIsTrueByDefault()
        {
            var options = new T4ToolboxOptions();
            Assert.IsTrue(options.ErrorUnderliningEnabled);
        }

        [TestMethod]
        public void ErrorUnderliningEnabledRaisesPropertyChangedEvent()
        {
            var options = new T4ToolboxOptions();
            string changedProperty = string.Empty;
            options.PropertyChanged += (sender, args) => { changedProperty = args.PropertyName; };
            options.ErrorUnderliningEnabled = !options.ErrorUnderliningEnabled;
            Assert.AreEqual("ErrorUnderliningEnabled", changedProperty);
        }

        [TestMethod]
        public void ErrorUnderliningEnabledDoesNotRaisePropertyChangedEventWhenNewValueIsSameAsOld()
        {
            var options = new T4ToolboxOptions();
            string changedProperty = string.Empty;
            options.PropertyChanged += (sender, args) => { changedProperty = args.PropertyName; };
            options.ErrorUnderliningEnabled = options.ErrorUnderliningEnabled;
            Assert.IsTrue(string.IsNullOrEmpty(changedProperty));
        }

        [TestMethod]
        public void QuickInfoTooltipsEnabledIsTrueByDefault()
        {
            var options = new T4ToolboxOptions();
            Assert.IsTrue(options.QuickInfoTooltipsEnabled);
        }

        [TestMethod]
        public void QuickInfoTooltipsEnabledRaisesPropertyChangedEvent()
        {
            var options = new T4ToolboxOptions();
            string changedProperty = string.Empty;
            options.PropertyChanged += (sender, args) => { changedProperty = args.PropertyName; };
            options.QuickInfoTooltipsEnabled = !options.QuickInfoTooltipsEnabled;
            Assert.AreEqual("QuickInfoTooltipsEnabled", changedProperty);
        }

        [TestMethod]
        public void QuickInfoTooltipsEnabledDoesNotRaisePropertyChangedEventWhenNewValueIsSameAsOld()
        {
            var options = new T4ToolboxOptions();
            string changedProperty = string.Empty;
            options.PropertyChanged += (sender, args) => { changedProperty = args.PropertyName; };
            options.QuickInfoTooltipsEnabled = options.QuickInfoTooltipsEnabled;
            Assert.IsTrue(string.IsNullOrEmpty(changedProperty));
        }

        [TestMethod]
        public void SyntaxColorizationEnabledIsTrueByDefault()
        {
            var options = new T4ToolboxOptions();
            Assert.IsTrue(options.SyntaxColorizationEnabled);
        }

        [TestMethod]
        public void SyntaxColorizationEnabledRaisesPropertyChangedEvent()
        {
            var options = new T4ToolboxOptions();
            string changedProperty = string.Empty;
            options.PropertyChanged += (sender, args) => { changedProperty = args.PropertyName; };
            options.SyntaxColorizationEnabled = !options.SyntaxColorizationEnabled;
            Assert.AreEqual("SyntaxColorizationEnabled", changedProperty);
        }

        [TestMethod]
        public void SyntaxColorizationEnabledDoesNotRaisePropertyChangedEventWhenNewValueIsSameAsOld()
        {
            var options = new T4ToolboxOptions();
            string changedProperty = string.Empty;
            options.PropertyChanged += (sender, args) => { changedProperty = args.PropertyName; };
            options.SyntaxColorizationEnabled = options.SyntaxColorizationEnabled;
            Assert.IsTrue(string.IsNullOrEmpty(changedProperty));            
        }

        [TestMethod]
        public void TemplateOutliningEnabledIsTrueByDefault()
        {
            var options = new T4ToolboxOptions();
            Assert.IsTrue(options.TemplateOutliningEnabled);
        }

        [TestMethod]
        public void TemplateOutliningEnabledRaisesPropertyChangedEvent()
        {
            var options = new T4ToolboxOptions();
            string changedProperty = string.Empty;
            options.PropertyChanged += (sender, args) => { changedProperty = args.PropertyName; };
            options.TemplateOutliningEnabled = !options.TemplateOutliningEnabled;
            Assert.AreEqual("TemplateOutliningEnabled", changedProperty);
        }

        [TestMethod]
        public void TemplateOutliningEnabledDoesNotRaisePropertyChangedEventWhenNewValueIsSameAsOld()
        {
            var options = new T4ToolboxOptions();
            string changedProperty = string.Empty;
            options.PropertyChanged += (sender, args) => { changedProperty = args.PropertyName; };
            options.TemplateOutliningEnabled = options.TemplateOutliningEnabled;
            Assert.IsTrue(string.IsNullOrEmpty(changedProperty));
        }        

        [TestMethod]
        public void SaveSettingsToStorageCreatesNewRegistryKey()
        {
            var options = new TestableT4ToolboxOptions(this.GetTestRegistryKey);
            options.SaveSettingsToStorage();
            Assert.IsTrue(this.GetTestRegistryKey().GetSubKeyNames().Single() == "T4 Toolbox");
        }

        [TestMethod]
        public void SaveSettingsToStorageUsesExistingRegistryKey()
        {
            using (RegistryKey existingKey = this.GetTestRegistryKey().CreateSubKey("T4 Toolbox"))
            {
                var options = new TestableT4ToolboxOptions(this.GetTestRegistryKey);
                options.SaveSettingsToStorage();
                Assert.IsTrue(existingKey.GetValueNames().Any());
            }
        }

        [TestMethod]
        public void SaveSettingsToStorageWritesPropertyValuesToRegistryKey()
        {
            var options = new TestableT4ToolboxOptions(this.GetTestRegistryKey);
            options.SaveSettingsToStorage();
            using (RegistryKey rootKey = this.GetTestRegistryKey())
            using (RegistryKey settingsKey = rootKey.OpenSubKey("T4 Toolbox"))
            {
                Assert.AreEqual(options.SyntaxColorizationEnabled.ToString(), settingsKey.GetValue("SyntaxColorizationEnabled"));
            }
        }

        [TestMethod, ExpectedException(typeof(ObjectDisposedException))]
        public void SaveSettingsToStorageDisposesRootKey()
        {
            RegistryKey rootKey = this.GetTestRegistryKey();
            var options = new TestableT4ToolboxOptions(() => rootKey);
            options.SaveSettingsToStorage();
            rootKey.GetValueNames(); // ObjectDisposedException here
        }

        [TestMethod]
        public void LoadSettingsFromStorageReadsPropertyValuesFromRegistryKey()
        {
            using (RegistryKey rootKey = this.GetTestRegistryKey())
            using (RegistryKey settingsKey = rootKey.CreateSubKey("T4 Toolbox"))
            {
                settingsKey.SetValue("SyntaxColorizationEnabled", "False");
                var options = new TestableT4ToolboxOptions(this.GetTestRegistryKey);
                options.LoadSettingsFromStorage();
                Assert.IsFalse(options.SyntaxColorizationEnabled);
            }
        }

        [TestMethod]
        public void LoadSettingsFromStorageConvertsRegistryValuesToPropertyTypeForBackwardCompatibility()
        {
            using (RegistryKey rootKey = this.GetTestRegistryKey())
            using (RegistryKey settingsKey = rootKey.CreateSubKey("T4 Toolbox"))
            {
                settingsKey.SetValue("SyntaxColorizationEnabled", 0, RegistryValueKind.DWord);
                var options = new TestableT4ToolboxOptions(this.GetTestRegistryKey);
                options.LoadSettingsFromStorage();
                Assert.IsFalse(options.SyntaxColorizationEnabled);
            }
        }
        
        [TestMethod]
        public void LoadSettingsFromStorageDoesNotChangePropertyWhenRegistryValueDoesNotExist()
        {
            using (RegistryKey rootKey = this.GetTestRegistryKey())
            using (RegistryKey settingsKey = rootKey.CreateSubKey("T4 Toolbox"))
            {
                var options = new TestableT4ToolboxOptions(this.GetTestRegistryKey);
                options.LoadSettingsFromStorage();
                Assert.IsTrue(options.SyntaxColorizationEnabled);
            }
        }

        [TestMethod]
        public void LoadSettingsFromStorageDoesNotChangePropertiesWhenRegistryKeyDoesNotExist()
        {
            var options = new TestableT4ToolboxOptions(this.GetTestRegistryKey);
            options.LoadSettingsFromStorage();
            Assert.IsTrue(options.SyntaxColorizationEnabled);
        }

        [TestMethod, ExpectedException(typeof(ObjectDisposedException))]
        public void LoadSettingsFromStorageDisposesRootKey()
        {
            RegistryKey rootKey = this.GetTestRegistryKey();
            var options = new TestableT4ToolboxOptions(() => rootKey);
            options.LoadSettingsFromStorage();
            rootKey.GetValueNames(); // ObjectDisposedException here
        }

        [TestMethod]
        public void LoadSettingsFromStorageRaisesPropertyChangedEvent()
        {
            var options = new TestableT4ToolboxOptions(this.GetTestRegistryKey);

            using (RegistryKey rootKey = this.GetTestRegistryKey())
            using (RegistryKey settingsKey = rootKey.CreateSubKey("T4 Toolbox"))
            {
                settingsKey.SetValue("SyntaxColorizationEnabled", false, RegistryValueKind.DWord);                
            }

            bool propertyChanged = false;
            options.PropertyChanged += (sender, args) => propertyChanged = true;
            options.LoadSettingsFromStorage();

            Assert.IsTrue(propertyChanged);
        }

        [TestMethod]
        public void ResetSettingsChangesPropertiesToValuesSpecifiedInDefaultValueAttribute()
        {
            var options = new TestableT4ToolboxOptions(this.GetTestRegistryKey);
            options.SyntaxColorizationEnabled = false;
            options.ResetSettings();
            Assert.IsTrue(options.SyntaxColorizationEnabled);
        }

        [TestMethod]
        public void ResetSettingsChangesPropertiesWithoutDefaultValueAttributeToDefaultTypeValue()
        {
            var options = new TestableT4ToolboxOptions(this.GetTestRegistryKey);
            options.BoolPropertyWithoutDefaultValueAttribute = true;
            options.ResetSettings();
            Assert.IsFalse(options.BoolPropertyWithoutDefaultValueAttribute);            
        }

        [TestMethod]
        public void ResetSettingsDeletesRegistryKey()
        {
            using (RegistryKey rootKey = this.GetTestRegistryKey())
            {
                rootKey.CreateSubKey("T4 Toolbox");
            }

            var options = new TestableT4ToolboxOptions(this.GetTestRegistryKey);
            options.ResetSettings();

            using (RegistryKey root = this.GetTestRegistryKey())
            {
                Assert.IsFalse(root.GetSubKeyNames().Any());
            }
        }

        [TestMethod]
        public void ResetSettingsDoesNotThrowWhenRegistryKeyDoesNotExist()
        {
            var options = new TestableT4ToolboxOptions(this.GetTestRegistryKey);
            options.ResetSettings();
        }

        [TestMethod, ExpectedException(typeof(ObjectDisposedException))]
        public void ResetSettingsDisposesRootKey()
        {
            RegistryKey rootKey = this.GetTestRegistryKey();
            var options = new TestableT4ToolboxOptions(() => rootKey);
            options.ResetSettings();
            rootKey.GetValueNames(); // ObjectDisposedException here            
        }

        private RegistryKey GetTestRegistryKey()
        {
            if (this.tempRegistryKeyName == null)
            {
                this.tempRegistryKeyName = Path.GetRandomFileName();
            }

            return Registry.CurrentUser.CreateSubKey(this.tempRegistryKeyName);
        }

        private class TestableT4ToolboxOptions : T4ToolboxOptions
        {
            private readonly Func<RegistryKey> getSettingsRootKey;

            public TestableT4ToolboxOptions(Func<RegistryKey> getSettingsRootKey)
            {
                this.getSettingsRootKey = getSettingsRootKey;
            }

            public bool BoolPropertyWithoutDefaultValueAttribute { get; set; }

            protected override RegistryKey GetSettingsRootKey()
            {
                return this.getSettingsRootKey();
            }
        }
    }
}