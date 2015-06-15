// <copyright file="T4ToolboxOptions.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Globalization;
    using System.Runtime.CompilerServices;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Shell.Interop;
    using Microsoft.Win32;
    using T4Toolbox.VisualStudio.Editor;

    /// <summary>
    /// Represents T4 Toolbox settings.
    /// </summary>
    internal class T4ToolboxOptions : IProfileManager, INotifyPropertyChanged, ITemplateEditorOptions
    {
        internal const string Category = T4Toolbox.AssemblyInfo.Product;
        internal const string Page = "General";

        private static T4ToolboxOptions instance;

        private bool completionListsEnabled;
        private bool errorReportingEnabled;
        private bool errorUnderliningEnabled;
        private bool syntaxColorizationEnabled;
        private bool quickInfoTooltipsEnabled;
        private bool templateOutliningEnabled;

        internal T4ToolboxOptions()
        {
            this.InitializeProperties();
        }

        /// <summary>
        /// Raised when a property of this object is changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets the global instance of the <see cref="T4ToolboxOptions"/>.
        /// </summary>
        /// <remarks>
        /// This object is used to subscribe to property change notifications by the tagger.
        /// </remarks>
        // TODO: Export and test
        public static T4ToolboxOptions Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new T4ToolboxOptions();
                    instance.LoadSettingsFromStorage();
                }

                return instance;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether completion lists are enabled for text templates in the Visual Studio editor.
        /// </summary>
        [DefaultValue(true), Category("Editor"), DisplayName("Enable Completion Lists"), Description("Enable completion lists for Text Templates")]
        public bool CompletionListsEnabled
        {
            get { return this.completionListsEnabled; }
            set { this.SetProperty(ref this.completionListsEnabled, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether text template errors are reported in the Error List window.
        /// </summary>
        [DefaultValue(true), Category("Editor"), DisplayName("Enable Error Reporting"), Description("Enable display of Text Template errors in the Error List window")]
        public bool ErrorReportingEnabled
        {
            get { return this.errorReportingEnabled; }
            set { this.SetProperty(ref this.errorReportingEnabled, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether error underlining is enabled for text templates. 
        /// </summary>
        /// <remarks>
        /// This property allows users to disable error underlining if it causes performance problems.
        /// </remarks>
        [DefaultValue(true), Category("Editor"), DisplayName("Enable Error Underlining"), Description("Enable error underlining for Text Templates")]
        public bool ErrorUnderliningEnabled
        {
            get { return this.errorUnderliningEnabled; }
            set { this.SetProperty(ref this.errorUnderliningEnabled, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether QuickInfo tooltips are enabled for text templates.
        /// </summary>
        [DefaultValue(true), Category("Editor"), DisplayName("Enable QuickInfo Tooltips"), Description("Enable QuickInfo tooltips for Text Templates")]
        public bool QuickInfoTooltipsEnabled
        {
            get { return this.quickInfoTooltipsEnabled; }
            set { this.SetProperty(ref this.quickInfoTooltipsEnabled, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether color syntax highlighting is enabled. 
        /// </summary>
        /// <remarks>
        /// This property allows users to disable color syntax highlighting if it causes performance problems.
        /// </remarks>
        [DefaultValue(true), Category("Editor"), DisplayName("Enable Syntax Colorization"), Description("Enable color syntax highlighting for Text Templates")]
        public bool SyntaxColorizationEnabled
        {
            get { return this.syntaxColorizationEnabled; }
            set { this.SetProperty(ref this.syntaxColorizationEnabled, value); } 
        }

        /// <summary>
        /// Gets or sets a value indicating whether outlining of code blocks is enabled.
        /// </summary>
        /// <remarks>
        /// This property allows users to disable template outlining if it causes performance problems or conflicts with 3rd party editor extensions.
        /// </remarks>
        [DefaultValue(true), Category("Editor"), DisplayName("Enable Template Outlining"), Description("Enable outlining of code blocks in Text Templates")]
        public bool TemplateOutliningEnabled
        {
            get { return this.templateOutliningEnabled; }
            set { this.SetProperty(ref this.templateOutliningEnabled, value); }
        }

        public void LoadSettingsFromStorage()
        {
            using (RegistryKey rootKey = this.GetSettingsRootKey())
            {
                if (rootKey == null)
                {
                    return;
                }

                using (RegistryKey settingsKey = rootKey.OpenSubKey(T4ToolboxOptions.Category))
                {
                    if (settingsKey == null)
                    {
                        return;
                    }

                    PropertyDescriptorCollection properties = this.GetProperties();
                    foreach (string valueName in settingsKey.GetValueNames())
                    {
                        PropertyDescriptor property = properties[valueName];
                        if (property != null)
                        {
                            object value = settingsKey.GetValue(valueName);

                            try
                            {
                                if (property.Converter.CanConvertFrom(value.GetType()))
                                {
                                    value = property.Converter.ConvertFrom(null, CultureInfo.InvariantCulture, value);                                
                                }
                                else
                                {
                                    TypeConverter valueConverter = TypeDescriptor.GetConverter(value);
                                    if (valueConverter.CanConvertTo(property.PropertyType))
                                    {
                                        value = valueConverter.ConvertTo(null, CultureInfo.InvariantCulture, value, property.PropertyType);
                                    }                                    
                                }
                            }
                            catch (FormatException)
                            {
                                // Ignore unexpected values of unexpected format
                            }

                            if (property.PropertyType.IsInstanceOfType(value))
                            {
                                property.SetValue(this, value);                                
                            }
                        }
                    }
                }
            }
        }

        public void LoadSettingsFromXml(IVsSettingsReader reader)
        {
            throw new System.NotImplementedException();
        }

        public void SaveSettingsToStorage()
        {
            using (RegistryKey rootKey = this.GetSettingsRootKey())
            {
                if (rootKey != null) 
                {
                    using (RegistryKey settingsKey = rootKey.CreateSubKey(T4ToolboxOptions.Category))
                    {
                        foreach (PropertyDescriptor property in this.GetProperties())
                        {
                            object value = property.GetValue(this);
                            string stringValue = property.Converter.ConvertToInvariantString(value);
                            settingsKey.SetValue(property.Name, stringValue);
                        }
                    }
                }
            }
        }

        public void SaveSettingsToXml(IVsSettingsWriter writer)
        {
            throw new System.NotImplementedException();
        }

        public void ResetSettings()
        {
            using (RegistryKey rootKey = this.GetSettingsRootKey())
            {
                if (rootKey != null)
                {
                    rootKey.DeleteSubKeyTree(T4ToolboxOptions.Category, throwOnMissingSubKey: false);
                }
            }

            this.InitializeProperties();
        }

        internal void CopyFrom(T4ToolboxOptions source)
        {
            Debug.Assert(source != null, "source");
            foreach (PropertyDescriptor property in this.GetProperties())
            {
                object value = property.GetValue(source);
                property.SetValue(this, value);
            }
        }

        protected virtual RegistryKey GetSettingsRootKey()
        {            
            return VSRegistry.RegistryRoot(__VsLocalRegistryType.RegType_UserSettings, writable: true);
        }
            
        private void InitializeProperties()
        {
            foreach (PropertyDescriptor property in this.GetProperties())
            {
                var attribute = (DefaultValueAttribute)property.Attributes[typeof(DefaultValueAttribute)];
                object defaultValue = attribute != null ? attribute.Value : Activator.CreateInstance(property.PropertyType);
                property.SetValue(this, defaultValue);
            }
        }

        private PropertyDescriptorCollection GetProperties()
        {
            return TypeDescriptor.GetProperties(this, new[] { DesignerSerializationVisibilityAttribute.Visible });
        }

        private void SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null) where T : IEquatable<T>
        {
            if (!field.Equals(value))
            {
                field = value;

                PropertyChangedEventHandler handler = this.PropertyChanged;
                if (handler != null)
                {
                    handler(this, new PropertyChangedEventArgs(propertyName));
                }
            }
        }
    }
}