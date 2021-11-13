// <copyright file="EnvDteExtensions.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio
{
    using System.Globalization;
    using EnvDTE;
    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.OLE.Interop;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Shell.Interop;

    /// <summary>
    /// Extension methods for types in the EnvDTE namespace.
    /// </summary>
    internal static class EnvDteExtensions 
    {
        public static IVsHierarchy AsHierarchy(this Project project)
        {
            using (var serviceProvider = new ServiceProvider((IServiceProvider)project.DTE))
            {
                var solution = (IVsSolution)serviceProvider.GetService(typeof(SVsSolution));

                IVsHierarchy hierarchy;
                ErrorHandler.ThrowOnFailure(solution.GetProjectOfUniqueName(project.UniqueName, out hierarchy));

                return hierarchy;
            }
        }

        /// <summary>
        /// Gets MSBuild metadata element of the specified project item.
        /// </summary>
        public static string GetItemAttribute(this ProjectItem projectItem, string attributeName)
        {
            IVsBuildPropertyStorage propertyStorage;
            uint projectItemId;
            GetBuildPropertyStorage(projectItem, out propertyStorage, out projectItemId);

            string value;
            if (ErrorHandler.Failed(propertyStorage.GetItemAttribute(projectItemId, attributeName, out value)))
            {
                // Attribute doesn't exist
                value = string.Empty;
            }

            return value;
        }

        public static uint GetItemId(this ProjectItem projectItem)
        {
            IVsHierarchy hierarchy = projectItem.ContainingProject.AsHierarchy();
            uint itemId;
            ErrorHandler.ThrowOnFailure(hierarchy.ParseCanonicalName(projectItem.FileNames[1], out itemId));
            return itemId;
        }

        /// <summary>
        /// Sets MSBuild metadata element for the specified project item.
        /// </summary>
        public static void SetItemAttribute(this ProjectItem projectItem, string attributeName, string attributeValue)
        {
            IVsBuildPropertyStorage propertyStorage;
            uint projectItemId;
            GetBuildPropertyStorage(projectItem, out propertyStorage, out projectItemId);

            ErrorHandler.ThrowOnFailure(propertyStorage.SetItemAttribute(projectItemId, attributeName, attributeValue));            
        }

        /// <summary>
        /// Sets property value for the <paramref name="projectItem"/>.
        /// </summary>
        /// <exception cref="TransformationException">
        /// When the <paramref name="projectItem"/> doesn't have a property with the specified <paramref name="propertyName"/>.
        /// </exception>
        public static void SetPropertyValue(this ProjectItem projectItem, string propertyName, object propertyValue)
        {
            Property property = projectItem.Properties.Item(propertyName);
            if (property == null)
            {
                throw new TransformationException(
                    string.Format(CultureInfo.CurrentCulture, "Property {0} is not supported for {1}", propertyName, projectItem.Name));
            }

            property.Value = propertyValue;
        }

        private static void GetBuildPropertyStorage(ProjectItem projectItem, out IVsBuildPropertyStorage propertyStorage, out uint projectItemId)
        {
            IVsHierarchy hierarchy = projectItem.ContainingProject.AsHierarchy();
            propertyStorage = (IVsBuildPropertyStorage)hierarchy;
            ErrorHandler.ThrowOnFailure(hierarchy.ParseCanonicalName(projectItem.FileNames[1], out projectItemId));
        }
    }
}