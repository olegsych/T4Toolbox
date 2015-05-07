// <copyright file="ItemMetadataWizard.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Linq;
    using EnvDTE;

    /// <summary>
    /// Sets properties or MSBuild metadata for project item.
    /// </summary>
    /// <remarks>
    /// This wizard is similar to the ItemPropertiesWizard defined in the 
    /// Microsoft.VSDesigner.ProjectWizard namespace of the Microsoft.VSDesigner assembly.
    /// It can be used for those project system implementations, such as SQLDB, that don't 
    /// support standard item properties.
    /// </remarks>
    public class ItemMetadataWizard : ProjectItemTemplateWizard
    {
        /// <summary>
        /// Sets properties or MSBuild metadata based on contents of the WizardData element of the .VSTemplate file.
        /// </summary>
        /// <param name="projectItem">A <see cref="ProjectItem"/> that finished generating.</param>
        public override void ProjectItemFinishedGenerating(ProjectItem projectItem)
        {
            if (projectItem == null)
            {
                throw new ArgumentNullException("projectItem");
            }

            foreach (XElement metadata in this.GetMetadataItems())
            {
                projectItem.SetItemAttribute(metadata.Name.LocalName, metadata.Value);
            }
        }

        /// <summary>
        /// Returns attribute values stored in the .VSTEMPLATE file.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{XElement}"/>, where each item represents a single metadata element.</returns>
        private IEnumerable<XElement> GetMetadataItems()
        {
            string wizardData = this.ReplacementParameters["$wizarddata$"];
            XElement root = XDocument.Parse(wizardData).Root;
            if (root == null)
            {
                throw new InvalidOperationException();
            }

            return root.Elements();
        }
    }
}