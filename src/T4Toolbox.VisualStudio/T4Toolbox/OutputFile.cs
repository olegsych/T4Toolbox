// <copyright file="OutputFile.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox
{
    using System;
    using System.Text;

    /// <summary>
    /// This class contains information about generated output file.
    /// </summary>
    [Serializable]
    public class OutputFile : OutputItem
    {
        /// <summary>
        /// Stores content of the output file.
        /// </summary>
        private readonly StringBuilder content;

        /// <summary>
        /// Initializes a new instance of the <see cref="OutputFile"/> class.
        /// </summary>
        public OutputFile()
        {
            this.content = new StringBuilder();
        }

        internal OutputFile(StringBuilder content)
        {
            this.content = content;
        }

        /// <summary>
        /// Gets content of the output file.
        /// </summary>
        /// <value>
        /// A <see cref="StringBuilder"/> storing content of the <see cref="OutputFile"/>.
        /// </value>
        public StringBuilder Content
        {
            get { return this.content; }
        }
    }
}
