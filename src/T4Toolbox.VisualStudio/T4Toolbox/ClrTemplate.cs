// <copyright file="ClrTemplate.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox
{
    using System;
    using System.IO;
    using System.Text;

    /// <summary>
    /// Serves as a base class for templates that produce code in CLR languages, such as C# or Visual Basic.
    /// </summary>
    public abstract class ClrTemplate : Template
    {
        /// <summary>
        /// Gets default namespace based on location of the T4 script file in the project.
        /// </summary>
        public string DefaultNamespace
        {
            get
            {
                // Start with the root namespace specified for the project
                var defaultNamespace = new StringBuilder(this.RootNamespace);

                // Determine relative location of the template file within the project

                // If the file is added to the project as a link from another folder, use the link path instead of the actual file path.
                string relativeInputFilePath = this.Context.GetMetadataValue("Link");
                if (string.IsNullOrEmpty(relativeInputFilePath))
                {
                    // Otherwise determine it based on the location of the input file relative to the location of the project file
                    string projectFile = this.Context.GetPropertyValue("MSBuildProjectFullPath");
                    relativeInputFilePath = FileMethods.GetRelativePath(projectFile, this.Context.InputFile);
                }

                // Append folder names to the root namespace
                string relativeDirectory = Path.GetDirectoryName(relativeInputFilePath) ?? string.Empty;
                foreach (string folderName in relativeDirectory.Split(new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries))
                {
                    if (folderName != ".")
                    {
                        defaultNamespace.Append('.');
                        defaultNamespace.Append(folderName.Replace(" ", string.Empty));
                    }
                }

                return defaultNamespace.ToString();
            }
        }

        /// <summary>
        /// Gets the default namespace of the project where the T4 script file is located.
        /// </summary>
        public string RootNamespace
        {
            get { return this.Context.GetPropertyValue("RootNamespace"); }
        }
    }
}