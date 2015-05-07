// <copyright file="ScriptFileGenerator.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;
    using EnvDTE;
    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.Designer.Interfaces;
    using Microsoft.VisualStudio.Shell.Interop;
    using Microsoft.VisualStudio.TextTemplating.VSHost;

    /// <summary>
    /// Two-stage template-based file generator.
    /// </summary>
    /// <remarks>
    /// When associated with any file, this generator will produce an empty text template 
    /// with the same name as the file and .tt extension. This template will be then 
    /// transformed by the standard TextTemplatingFileGenerator. If the template already 
    /// exist, this generator will preserve its content and still trigger the second 
    /// code generation stage. 
    /// </remarks>
    [Guid("8CAB1895-2287-463F-BE14-1ADB873B4741")]
    public class ScriptFileGenerator : BaseCodeGeneratorWithSite
    {
        internal const string Name = "T4Toolbox.ScriptFileGenerator";
        internal const string Description = "Generator that creates a new or transforms existing Text Template";

        /// <summary>
        /// Returns extension of the output file this generator produces.
        /// </summary>
        public override string GetDefaultExtension()
        {
            return ".tt";
        }

        /// <summary>
        /// Generates new or transforms existing T4 script.
        /// </summary>
        protected override byte[] GenerateCode(string inputFileName, string inputFileContent)
        {
            return
                this.GenerateFromAssociatedTemplateFile(inputFileName) ??
                this.GenerateFromExistingScriptFile(inputFileName) ?? 
                this.GenerateNewScriptFile(inputFileName);
        }

        private byte[] GenerateFromExistingScriptFile(string inputFileName)
        {
            string outputFileName = Path.ChangeExtension(inputFileName, this.GetDefaultExtension());
            if (File.Exists(outputFileName))
            {
                // If the output file is opened in Visual Studio editor, save it to prevent the "Run Custom Tool" implementation from silently discarding changes.
                Document outputDocument = this.Dte.Documents.Cast<Document>().SingleOrDefault(d => d.FullName == outputFileName);
                if (outputDocument != null && !outputDocument.Saved)
                {
                    // Save the script file if it was modified
                    outputDocument.Save(string.Empty);
                }

                // Read it from disk. The "Run Custom Tool" implementation always overwrites it.
                return File.ReadAllBytes(outputFileName);
            }

            return null;
        }

        private byte[] GenerateFromAssociatedTemplateFile(string inputFileName)
        {
            var hierarchy = (IVsHierarchy)this.GetService(typeof(IVsHierarchy));

            uint inputItemId;
            ErrorHandler.ThrowOnFailure(hierarchy.ParseCanonicalName(inputFileName, out inputItemId));

            string templatePath;
            var propertyStorage = (IVsBuildPropertyStorage)hierarchy;
            if (ErrorHandler.Failed(propertyStorage.GetItemAttribute(inputItemId, ItemMetadata.Template, out templatePath)))
            {
                return null;
            }

            // Remove <Template> metadata from the project item and refresh the properties window
            ErrorHandler.ThrowOnFailure(propertyStorage.SetItemAttribute(inputItemId, ItemMetadata.Template, null));
            var propertyBrowser = (IVSMDPropertyBrowser)this.GlobalServiceProvider.GetService(typeof(SVSMDPropertyBrowser));
            propertyBrowser.Refresh();

            var templateLocator = (TemplateLocator)this.GlobalServiceProvider.GetService(typeof(TemplateLocator));
            if (!templateLocator.LocateTemplate(inputFileName, ref templatePath))
            {
                return null;
            }

            return File.ReadAllBytes(templatePath);
        }

        private byte[] GenerateNewScriptFile(string inputFileName)
        {
            string language;
            string extension;
            const string VisualBasicProject = "{F184B08F-C81C-45F6-A57F-5ABD9991F28F}";
            Project currentProject = this.Dte.Solution.FindProjectItem(inputFileName).ContainingProject;
            if (string.Equals(currentProject.Kind, VisualBasicProject, StringComparison.OrdinalIgnoreCase))
            {
                language = "VB";
                extension = "vb";
            }
            else
            {
                language = "C#";
                extension = "cs";
            }

            string outputFileContent =
                "<#@ template language=\"" + language + "\" debug=\"True\" #>" + Environment.NewLine +
                "<#@ output extension=\"" + extension + "\" #>" + Environment.NewLine +
                "<#@ include file=\"T4Toolbox.tt\" #>" + Environment.NewLine;

            return Encoding.UTF8.GetBytes(outputFileContent);            
        }
    }
}
