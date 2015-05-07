// <copyright file="TransformationContextProvider.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio
{
    using System;
    using System.ComponentModel.Design;
    using System.IO;
    using System.Linq;
    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Shell.Interop;
    using Microsoft.VisualStudio.TextTemplating.VSHost;
    using T4Toolbox;

    /// <summary>
    /// Implements the <see cref="ITransformationContextProvider"/> service.
    /// </summary>
    internal class TransformationContextProvider : MarshalByRefObject, ITransformationContextProvider
    {
        private readonly IServiceProvider serviceProvider;

        private TransformationContextProvider(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        string ITransformationContextProvider.GetMetadataValue(object hierarchyObject, string fileName, string metadataName)
        {
            uint fileItemId;
            var hierarchy = (IVsHierarchy)hierarchyObject;
            ErrorHandler.ThrowOnFailure(hierarchy.ParseCanonicalName(fileName, out fileItemId));

            // Try getting metadata from the file itself
            string metadataValue;
            var propertyStorage = (IVsBuildPropertyStorage)hierarchyObject;
            if (ErrorHandler.Succeeded(propertyStorage.GetItemAttribute(fileItemId, metadataName, out metadataValue)))
            {
                return metadataValue;
            }

            // Try getting metadata from the parent
            object parentItemId;
            ErrorHandler.ThrowOnFailure(hierarchy.GetProperty(fileItemId, (int)__VSHPROPID.VSHPROPID_Parent, out parentItemId));
            if (ErrorHandler.Succeeded(propertyStorage.GetItemAttribute((uint)(int)parentItemId, metadataName, out metadataValue)))
            {
                return metadataValue;
            }

            return string.Empty;
        }

        string ITransformationContextProvider.GetPropertyValue(object hierarchy, string propertyName)
        {
            var propertyStorage = (IVsBuildPropertyStorage)hierarchy;

            string propertyValue;
            if (ErrorHandler.Failed(propertyStorage.GetPropertyValue(propertyName, null, (uint)_PersistStorageType.PST_PROJECT_FILE, out propertyValue)))
            {
                // Property does not exist. Return an empty string.
                propertyValue = string.Empty;
            }

            return propertyValue;
        }

        /// <summary>
        /// Writes generated content to output files and deletes the old files that were not regenerated.
        /// </summary>
        /// <param name="inputFile">
        /// Full path to the template file.
        /// </param>
        /// <param name="outputFiles">
        /// A collection of <see cref="OutputFile"/> objects produced by the template.
        /// </param>
        void ITransformationContextProvider.UpdateOutputFiles(string inputFile, OutputFile[] outputFiles)
        {
            if (inputFile == null)
            {
                throw new ArgumentNullException("inputFile");
            }

            if (!Path.IsPathRooted(inputFile))
            {
                throw new ArgumentException(Resources.InputFilePathMustBeAbsoluteMessage, "inputFile");
            }

            if (outputFiles == null)
            {
                throw new ArgumentNullException("outputFiles");
            }

            foreach (OutputFile newOutput in outputFiles)
            {
                if (newOutput == null)
                {
                    throw new ArgumentException(Resources.OutputFileIsNullMessage, "outputFiles");
                }
            }

            // Validate the output files immediately. Exceptions will be reported by the templating service.
            var manager = new OutputFileManager(this.serviceProvider, inputFile, outputFiles);
            manager.Validate();

            // Wait for the default output file to be generated
            var watcher = new FileSystemWatcher();
            watcher.Path = Path.GetDirectoryName(inputFile);
            watcher.Filter = Path.GetFileNameWithoutExtension(inputFile) + "*." + this.GetTransformationOutputExtensionFromHost();

            FileSystemEventHandler runManager = (sender, args) =>
            {
                watcher.Dispose();

                // Store the actual output file name
                OutputFile defaultOutput = outputFiles.FirstOrDefault(output => string.IsNullOrEmpty(output.File));
                if (defaultOutput != null)
                {
                    defaultOutput.File = Path.GetFileName(args.FullPath);
                }

                // Finish updating the output files on the UI thread
                ThreadHelper.Generic.BeginInvoke(manager.DoWork);
            };

            watcher.Created += runManager;
            watcher.Changed += runManager;
            watcher.EnableRaisingEvents = true;
        }

        /// <summary>
        /// Adds the <see cref="ITransformationContextProvider"/> service in the specified <paramref name="container"/>.
        /// </summary>
        /// <param name="container">
        /// An <see cref="IServiceContainer"/> object that will be providing the <see cref="ITransformationContextProvider"/> service.
        /// </param>
        internal static void Register(IServiceContainer container)
        {
            container.AddService(typeof(ITransformationContextProvider), CreateService, promote: true);
        }

        private static object CreateService(IServiceContainer container, Type serviceType)
        {
            if (serviceType == typeof(ITransformationContextProvider))
            {
                return new TransformationContextProvider(container);
            }

            return null;
        }

        private string GetTransformationOutputExtensionFromHost()
        {
            var components = (ITextTemplatingComponents)this.serviceProvider.GetService(typeof(STextTemplating));
            var callback = components.Callback as TextTemplatingCallback; // Callback can be passed to ITextTemplating.ProcessTemplate by user code.
            if (callback == null)
            {
                throw new InvalidOperationException("A TextTemplatingCallback is expected from ITextTemplatingComponents.Callback.");
            }

            if (callback.Extension == null)
            {
                throw new InvalidOperationException("TextTemplatingCallback was not initialized (Extension is null).");
            }

            return callback.Extension.TrimStart('.');
        }
    }
}
