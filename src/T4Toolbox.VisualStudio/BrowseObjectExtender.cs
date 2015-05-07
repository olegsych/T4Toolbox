// <copyright file="BrowseObjectExtender.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Drawing.Design;
    using System.Globalization;
    using System.Runtime.InteropServices;
    using EnvDTE;
    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.Shell.Interop;

    /// <summary>
    /// Adds "Custom Tool Template" and "Custom ToolParameters" properties to the C# and Visual Basic file properties.
    /// </summary>
    [ComVisible(true), ClassInterface(ClassInterfaceType.AutoDispatch)]
    [SuppressMessage("Microsoft.Interoperability", "CA1409:ComVisibleTypesShouldBeCreatable", Justification = "Instances of this type are created only by TemplatePropertyProvider.")]
    public class BrowseObjectExtender
    {
        /// <summary>
        /// Name used to register the extender with Visual Studio. 
        /// </summary>
        /// <remarks>
        /// Visual Studio uses this name to determine name of the <see cref="Property"/> objects it creates. 
        /// Choosing this value allows us to create properties with clean names like T4Toolbox.CustomToolTemplate.
        /// </remarks>
        internal const string Name = "T4Toolbox"; 

        private const string TemplatePropertyDisplayName = "Custom Tool Template";

        private readonly uint itemId;
        private readonly IVsHierarchy hierarchy;
        private readonly IVsBuildPropertyStorage propertyStorage;
        private readonly IServiceProvider serviceProvider;
        private readonly int cookie;
        private readonly IExtenderSite site;
        private ProjectItem projectItem;

        internal BrowseObjectExtender(IServiceProvider serviceProvider, IVsBrowseObject browseObject, IExtenderSite site, int cookie)
        {
            Debug.Assert(serviceProvider != null, "serviceProvider");
            Debug.Assert(browseObject != null, "browseObject");
            Debug.Assert(site != null, "site");
            Debug.Assert(cookie != 0, "cookie");

            this.site = site;
            this.cookie = cookie;
            this.serviceProvider = serviceProvider;
            ErrorHandler.ThrowOnFailure(browseObject.GetProjectItem(out this.hierarchy, out this.itemId));
            this.propertyStorage = (IVsBuildPropertyStorage)this.hierarchy;
            this.CustomToolParameters = new CustomToolParameters(this.serviceProvider, this.hierarchy, this.itemId);
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="BrowseObjectExtender"/> class.
        /// </summary>
        /// <remarks>
        /// This method notifies the extender site that the extender has been deleted in order to prevent Visual Studio 
        /// from crashing with an Access Violation exception.
        /// </remarks>
        ~BrowseObjectExtender()
        {
            try
            {
                this.site.NotifyDelete(this.cookie);
            }
            catch (InvalidComObjectException)
            {
                // This exception occurs when the Runtime-Callable Wrapper (RCW) was already disconnected from the COM object.
                // This typically happens when the extender is disposed when Visual Studio shuts down.
            }
        }

        /// <summary>
        /// Gets the <see cref="CustomToolParameters"/> object that represents parameters defined in a text template.
        /// </summary>
        [DisplayName("Custom Tool Parameters"), Category("Advanced")]
        [Description("Specifies values for parameters defined in a T4 template transformed by the TextTemplatingFileGenerator or the " + TemplatedFileGenerator.Name + ".")]
        public CustomToolParameters CustomToolParameters { get; private set; }

        /// <summary>
        /// Gets or sets the file name of the template used by the <see cref="TemplatedFileGenerator"/>.
        /// </summary>
        [DisplayName(TemplatePropertyDisplayName), Category("Advanced")]
        [Description("A T4 template used by the " + TemplatedFileGenerator.Name + " to generate code from this file.")]
        [Editor(typeof(CustomToolTemplateEditor), typeof(UITypeEditor))]
        public string CustomToolTemplate
        {
            get
            {
                string value;
                if (ErrorHandler.Failed(this.propertyStorage.GetItemAttribute(this.itemId, ItemMetadata.Template, out value)))
                {
                    // Metadata element is not defined. Return an empty string.
                    value = string.Empty;
                }

                return value;
            }

            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    // Report an error if the user tries to specify template for an incompatible custom tool.
                    if (!string.IsNullOrWhiteSpace((string)this.ProjectItem.Properties.Item(ProjectItemProperty.CustomTool).Value) &&
                        TemplatedFileGenerator.Name != (string)this.ProjectItem.Properties.Item(ProjectItemProperty.CustomTool).Value)
                    {
                        throw new InvalidOperationException(
                            string.Format(
                                CultureInfo.CurrentCulture,
                                "The '{0}' property is supported only by the {1}. Set the 'Custom Tool' property first.",
                                TemplatePropertyDisplayName, 
                                TemplatedFileGenerator.Name));
                    }

                    // Report an error if the template cannot be found
                    string fullPath = value;
                    var templateLocator = (TemplateLocator)this.serviceProvider.GetService(typeof(TemplateLocator));
                    if (!templateLocator.LocateTemplate(this.ProjectItem.FileNames[1], ref fullPath))
                    {
                        throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "Template '{0}' could not be found", value));
                    }
                }

                ErrorHandler.ThrowOnFailure(this.propertyStorage.SetItemAttribute(this.itemId, ItemMetadata.Template, value));

                // If the file does not have a custom tool yet, assume that by specifying the template user wants to use the T4Toolbox.TemplatedFileGenerator.
                if (!string.IsNullOrWhiteSpace(value) && 
                    string.IsNullOrWhiteSpace((string)this.ProjectItem.Properties.Item(ProjectItemProperty.CustomTool).Value))
                {
                    this.ProjectItem.Properties.Item(ProjectItemProperty.CustomTool).Value = TemplatedFileGenerator.Name;
                }
            }
        }

        private ProjectItem ProjectItem
        {
            get
            {
                if (this.projectItem == null)
                {
                    object extObject;
                    ErrorHandler.ThrowOnFailure(this.hierarchy.GetProperty(this.itemId, (int)__VSHPROPID.VSHPROPID_ExtObject, out extObject));
                    this.projectItem = (ProjectItem)extObject;
                }

                return this.projectItem;
            }
        }
    }
}