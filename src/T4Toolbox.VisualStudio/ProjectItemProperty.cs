// <copyright file="ProjectItemProperty.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio
{
    /// <summary>
    /// Defines constants for commonly-used VisualStudio project item properties.
    /// </summary>
    internal static class ProjectItemProperty
    {
        /// <summary>
        /// Internal name of the "Copy to Output Directory" project item property.
        /// </summary>
        public const string CopyToOutputDirectory = "CopyToOutputDirectory";

        /// <summary>
        /// Internal name of the "Custom Tool" project item property.
        /// </summary>
        public const string CustomTool = "CustomTool";

        /// <summary>
        /// Internal name of the "Custom Tool Namespace" project item property.
        /// </summary>
        public const string CustomToolNamespace = "CustomToolNamespace";

        /// <summary>
        /// Internal name of the "Custom Tool Parameters" project item property provided by the T4 Toolbox.
        /// </summary>
        public const string CustomToolParameters = BrowseObjectExtender.Name + ".CustomToolParameters";

        /// <summary>
        /// Internal name of the "Custom Tool Template" project item property provided by the T4 Toolbox.
        /// </summary>
        public const string CustomToolTemplate = BrowseObjectExtender.Name + ".CustomToolTemplate";

        /// <summary>
        /// Internal name of the "Build Action" project item property.
        /// </summary>
        public const string ItemType = "ItemType";
    }
}