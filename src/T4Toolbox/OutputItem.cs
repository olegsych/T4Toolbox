// <copyright file="OutputItem.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text;

    /// <summary>
    /// Stores information about a code generation output.
    /// </summary>
    [Serializable]
    public class OutputItem
    {
        #region fields

        private readonly IDictionary<string, string> metadata = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        private readonly ICollection<string> references = new List<string>();

        private string itemType;

        private string directory = string.Empty;

        private Encoding encoding;

        private string file = string.Empty;

        private string project = string.Empty;

        #endregion

        /// <summary>
        /// Gets or sets the action for copying a file to the output directory.
        /// </summary>
        /// <value>
        /// A <see cref="CopyToOutputDirectory"/> representing if or when a file is copied to the output directory.
        /// </value>
        public CopyToOutputDirectory CopyToOutputDirectory
        {
            get
            {
                string value;
                if (this.Metadata.TryGetValue(ItemMetadata.CopyToOutputDirectory, out value))
                {
                    if (string.Equals(value, "Always", StringComparison.OrdinalIgnoreCase))
                    {
                        return T4Toolbox.CopyToOutputDirectory.CopyAlways;
                    }
                    else if (string.Equals(value, "PreserveNewest", StringComparison.OrdinalIgnoreCase))
                    {
                        return T4Toolbox.CopyToOutputDirectory.CopyIfNewer;
                    }
                }

                return T4Toolbox.CopyToOutputDirectory.DoNotCopy;
            }

            set
            {
                switch (value)
                {
                    case T4Toolbox.CopyToOutputDirectory.CopyAlways:
                        this.Metadata[ItemMetadata.CopyToOutputDirectory] = "Always";
                        break;

                    case T4Toolbox.CopyToOutputDirectory.CopyIfNewer:
                        this.Metadata[ItemMetadata.CopyToOutputDirectory] = "PreserveNewest";
                        break;

                    case T4Toolbox.CopyToOutputDirectory.DoNotCopy:
                        this.Metadata[ItemMetadata.CopyToOutputDirectory] = string.Empty;
                        break;

                    default:
                        throw new ArgumentOutOfRangeException("value");
                }
            }
        }

        /// <summary>
        /// Gets or sets the name of the tool that transforms a file at design time and places
        /// the output of that transformation into another file.
        /// </summary>
        /// <value>
        /// A <see cref="String"/> containing the name of the custom tool to run for a project item.
        /// </value>
        public string CustomTool
        {
            get
            {
                string value;
                if (this.Metadata.TryGetValue(ItemMetadata.Generator, out value))
                {
                    return value;
                }

                return string.Empty;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                this.Metadata[ItemMetadata.Generator] = value;
            }
        }

        /// <summary>
        /// Gets or sets the namespace into which the output of the custom tool is placed.
        /// </summary>
        /// <value>
        /// A <see cref="String"/> containing the namespace for the output of the custom tool.
        /// </value>
        public string CustomToolNamespace
        {
            get
            {
                string value;
                if (this.Metadata.TryGetValue(ItemMetadata.CustomToolNamespace, out value))
                {
                    return value;
                }

                return string.Empty;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                this.Metadata[ItemMetadata.CustomToolNamespace] = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of directory where output file will be created.
        /// </summary>
        /// <value>
        /// A <see cref="String"/> containing relative or absolute directory name.
        /// </value>
        /// <remarks>
        /// This property is automatically stores the directory name of the path assigned to the <see cref="File"/> 
        /// property. A single <see cref="Template"/> can be rendered multiple times to generate several output files. 
        /// This property allows changing the directory where all output files will be saved without affecting the file 
        /// names. It is typically used by developers configuring a reusable <see cref="Generator"/> developed separately.
        /// </remarks>
        public string Directory
        {
            get
            {
                return this.directory;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                this.directory = value;
            }
        }

        /// <summary>
        /// Gets or sets encoding of the output file.
        /// </summary>
        /// <value>
        /// An <see cref="Encoding"/> object.
        /// </value>
        public Encoding Encoding
        {
            get
            {
                return this.encoding ?? Encoding.UTF8;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                this.encoding = value;
            }
        }

        /// <summary>
        /// Gets or sets path to the output file.
        /// </summary>
        /// <value>
        /// A <see cref="String"/> containing relative or absolute file path.
        /// </value>
        /// <remarks>
        /// <para>
        /// When the <see cref="File"/> property is set, the directory name is automatically separated and stored in 
        /// the <see cref="Directory"/> property. Only the file name is stored in this property. This allows developers 
        /// implementing a <see cref="Template"/> to specify the name of the output file, which is often based on the 
        /// input parameters (i.e. metadata) of code generation. On the other hand, developers reusing an existing
        /// <see cref="Template"/> in their project, can use use the <see cref="Directory"/> property to change the 
        /// default location of output files to fit their specific needs.
        /// </para>
        /// <para>
        /// When <see cref="File"/> is  <see cref="string.Empty"/>, the template will add generated content to 
        /// the output of the main T4 file.
        /// </para>
        /// </remarks>
        public string File
        {
            get
            {
                return this.file;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                if (!string.IsNullOrEmpty(value))
                {
                    string directory = System.IO.Path.GetDirectoryName(value);
                    if (!string.IsNullOrEmpty(directory))
                    {
                        this.Directory = directory;
                    }

                    this.file = System.IO.Path.GetFileName(value);
                }
                else
                {
                    this.file = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the MSBuild item type of the output item.
        /// </summary>
        /// <value>
        /// A <see cref="String"/> containing the item type of the target item.
        /// </value>
        public string ItemType
        {
            get
            {
                return this.itemType ?? string.Empty;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                this.itemType = value;
            }
        }

        /// <summary>
        /// Gets a dictionary of MSBuild metadata of the output file.
        /// </summary>
        /// <value>
        /// An <see cref="IDictionary{String,String}"/> where each metadata value is represented by a key/value pair.
        /// </value>
        public IDictionary<string, string> Metadata
        {
            get { return this.metadata; }
        }

        /// <summary>
        /// Gets a computed path of the output file.
        /// </summary>
        /// <value>
        /// A <see cref="String"/> that defines output file path based on <see cref="Project"/>, 
        /// <see cref="Directory"/> within the project and the <see cref="File"/> within the 
        /// directory.
        /// </value>
        /// <para>
        /// When <see cref="Directory"/> is a relative path, it is interpreted differently depending on the 
        /// <see cref="Project"/> property. If <see cref="Project"/> is specified, the <see cref="Directory"/> is relative 
        /// to the location of the target project. Otherwise, the <see cref="Directory"/> is relative to the location of the 
        /// input file. 
        /// </para>
        public string Path
        {
            get
            {
                var projectDirectory = string.Empty;
                if (!string.IsNullOrEmpty(this.Project))
                {
                    projectDirectory = System.IO.Path.GetDirectoryName(this.Project) ?? string.Empty;
                }

                return System.IO.Path.Combine(projectDirectory, this.Directory, this.File);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether existing output file 
        /// should be preserved during code generation.
        /// </summary>
        /// <value>
        /// <c>True</c> when existing file should be preserved or <c>False</c> otherwise.
        /// </value>
        /// <remarks>
        /// Set <see cref="PreserveExistingFile"/> to <c>true</c> when generating initial versions of the 
        /// source files that will be edited manually. This will cause the code generator to preserve the 
        /// existing file when the code generator runs for the first time as well as when the output file
        /// is no longer re-generated. 
        /// </remarks>
        public bool PreserveExistingFile { get; set; }

        /// <summary>
        /// Gets a list of assembly references required by the output file.
        /// </summary>
        /// <value>
        /// An <see cref="ICollection{String}"/> object where each item is an assembly name or file path.
        /// </value>
        /// <remarks>
        /// References are automatically added to the target <see cref="Project"/>.
        /// </remarks>
        public ICollection<string> References
        {
            get { return this.references; }
        }

        /// <summary>
        /// Gets or sets path to the target project.
        /// </summary>
        /// <value>
        /// A <see cref="String"/> containing relative or absolute project file path.
        /// </value>
        /// <remarks>
        /// Set the <see cref="Project"/> property to have generated files added to another project in the solution.
        /// If <see cref="Project"/> is <see cref="String.Empty"/>, the output file will be added to the project that 
        /// contains the input file. In this case the <see cref="Directory"/> is relative to the location of the input 
        /// file. If <see cref="Project"/> is specified, the <see cref="Directory"/> is relative to the target project.
        /// </remarks>
        public string Project 
        {
            get
            {
                return this.project;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                this.project = value;
            }
        }

        /// <summary>
        /// Validates attributes for correctness.
        /// </summary>
        internal void Validate()
        {
            if (string.IsNullOrEmpty(this.File))
            {
                if (!string.IsNullOrEmpty(this.Directory))
                {
                    throw new InvalidOperationException("Output Directory cannot be specified without File.");
                }

                if (!string.IsNullOrEmpty(this.Project))
                {
                    throw new InvalidOperationException("Output Project cannot be specified without File.");
                }

                if (this.PreserveExistingFile)
                {
                    throw new InvalidOperationException("Default output file of the transformation cannot be preserved.");
                }
            }
        }

        internal void CopyPropertiesFrom(OutputItem item)
        {
            this.File = item.File;
            this.Directory = item.Directory;
            this.Project = item.Project;
            this.Encoding = item.Encoding;
            this.ItemType = item.ItemType;
            this.PreserveExistingFile = item.PreserveExistingFile;

            foreach (string reference in item.References)
            {
                if (!this.References.Contains(reference))
                {
                    this.References.Add(reference);
                }
            }

            foreach (KeyValuePair<string, string> metadata in item.Metadata)
            {
                if (!this.Metadata.ContainsKey(metadata.Key))
                {
                    this.Metadata.Add(metadata.Key, metadata.Value);
                }
            }
        }

        internal void Validate(OutputItem item)
        {
            if (this.encoding != null && !object.Equals(item.Encoding, this.Encoding))
            {
                throw this.PropertyDiscrepancyException("Encoding", item.Encoding.EncodingName, this.Encoding.EncodingName);
            }

            if (this.itemType != null && !string.Equals(item.ItemType, this.ItemType, StringComparison.OrdinalIgnoreCase))
            {
                throw this.PropertyDiscrepancyException("ItemType", item.ItemType, this.ItemType); 
            }

            foreach (KeyValuePair<string, string> metadata in item.Metadata)
            {
                string previousValue;
                bool metadataExists = this.Metadata.TryGetValue(metadata.Key, out previousValue);
                if (metadataExists && metadata.Value != previousValue)
                {
                    throw this.PropertyDiscrepancyException(
                        string.Format(CultureInfo.InvariantCulture, "Metadata '{0}'", metadata.Key),
                        metadata.Value, 
                        previousValue);
                }
            }

            if (item.PreserveExistingFile != this.PreserveExistingFile)
            {
                throw this.PropertyDiscrepancyException("PreserveExistingFile", item.PreserveExistingFile.ToString(), this.PreserveExistingFile.ToString()); 
            }
        }

        private InvalidOperationException PropertyDiscrepancyException(string propertyName, string newValue, string oldValue)
        {
            return new InvalidOperationException(
                string.Format(
                    CultureInfo.CurrentCulture, 
                    "{0} value '{1}' does not match value '{2}' set previously for output file '{3}'.",
                    new object[] { propertyName, newValue, oldValue, this.Path }));
        }
    }
}