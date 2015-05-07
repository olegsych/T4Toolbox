// <copyright file="ProjectItemTemplateWizard.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio
{
    using System;
    using System.Collections.Generic;
    using EnvDTE;
    using Microsoft.VisualStudio.TemplateWizard;

    /// <summary>
    /// Serves as a base class for project item template wizards. Provides empty 
    /// implementations for all methods except <see cref="ProjectItemFinishedGenerating"/>.
    /// </summary>
    public abstract class ProjectItemTemplateWizard : IWizard
    {
        /// <summary>
        /// Stores the replacements dictionary obtained in the <see cref="RunStarted"/> method.
        /// </summary>
        private Dictionary<string, string> replacementParameters;

        /// <summary>
        /// Gets a dictionary of replacement parameters.
        /// </summary>
        /// <value>
        /// A <see cref="Dictionary{String, String}"/> of parameter values indexed by parameter name.
        /// </value>
        protected Dictionary<string, string> ReplacementParameters
        {
            get { return this.replacementParameters; }
        }

        /// <summary>
        /// Runs custom wizard logic before opening an item in the template.
        /// </summary>
        /// <param name="projectItem">
        /// The <see cref="ProjectItem"/> that will be opened.
        /// </param>
        /// <remarks>
        /// This method is intentionally left blank.
        /// </remarks>
        public void BeforeOpeningFile(ProjectItem projectItem)
        {
        }

        /// <summary>
        /// Runs custom wizard logic when a project has finished generating.
        /// </summary>
        /// <param name="project">
        /// The <see cref="Project"/> that finished generating.
        /// </param>
        /// <remarks>
        /// This method is intentionally left blank.
        /// </remarks>
        public void ProjectFinishedGenerating(Project project)
        {           
        }

        /// <summary>
        /// Runs custom wizard logic when a project item has finished generating.
        /// </summary>
        /// <param name="projectItem">
        /// The project item that finished generating.
        /// </param>
        public abstract void ProjectItemFinishedGenerating(ProjectItem projectItem);

        /// <summary>
        /// Runs custom wizard logic when the wizard has completed all tasks.
        /// </summary>
        /// <remarks>
        /// This method performs cleanup at the end of template unfolding.
        /// </remarks>
        public virtual void RunFinished()
        {
            this.replacementParameters = null;
        }

        /// <summary>
        /// Runs custom wizard logic at the beginning of a template wizard run.
        /// </summary>
        /// <param name="automationObject">
        /// The automation object being used by the template wizard.
        /// </param>
        /// <param name="replacementsDictionary">
        /// The list of standard parameters to be replaced.
        /// </param>
        /// <param name="runKind">
        ///  Indicating the type of wizard run.
        /// </param>
        /// <param name="customParams">
        /// The custom parameters with which to perform parameter replacement in the project.
        /// </param>
        /// <remarks>
        /// This method initializes <see cref="ReplacementParameters"/> dictionary.
        /// </remarks>
        public virtual void RunStarted(object automationObject, Dictionary<string, string> replacementsDictionary, WizardRunKind runKind, object[] customParams)
        {
            this.replacementParameters = replacementsDictionary;
        }

        /// <summary>
        /// Indicates whether the specified project item should be added to the project.
        /// </summary>
        /// <param name="filePath">
        /// The path to the project item.
        /// </param>
        /// <returns>
        /// True if the project item should be added to the project; otherwise, false.
        /// </returns>
        /// <remarks>
        /// This method always returns true.
        /// </remarks>
        public bool ShouldAddProjectItem(string filePath)
        {
            return true;
        }
    }
}
