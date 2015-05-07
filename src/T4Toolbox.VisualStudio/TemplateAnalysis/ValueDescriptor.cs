// <copyright file="ValueDescriptor.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.TemplateAnalysis
{
    using System.Diagnostics;

    /// <summary>
    /// Encapsulates metadata describing an attribute value.
    /// </summary>
    internal sealed class ValueDescriptor
    {
        public ValueDescriptor(string displayName, string description = null)
        {
            Debug.Assert(!string.IsNullOrWhiteSpace(displayName), "displayName");
            this.DisplayName = displayName;
            this.Description = description ?? string.Empty;
        }

        public string DisplayName { get; private set; }

        public string Description { get; private set; }
    }
}
