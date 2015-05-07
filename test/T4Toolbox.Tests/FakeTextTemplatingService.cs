// <copyright file="FakeTextTemplatingService.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.Tests
{
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Text.RegularExpressions;
    using Microsoft.VisualStudio.TextTemplating;
    using Microsoft.VisualStudio.TextTemplating.VSHost;

    /// <summary>
    /// This is a test stub for testing of functionality that requires a
    /// <see cref="ITextTemplatingEngineHost"/>.
    /// </summary>
    internal class FakeTextTemplatingService : STextTemplating, ITextTemplating, ITextTemplatingEngineHost, ITextTemplatingComponents, IServiceProvider, ITransformationContextProvider
    {
        public FakeTextTemplatingService()
        {
            this.TemplateFile = string.Empty;
        }

        public Action<CompilerErrorCollection> LoggedErrors { get; set; }

        public Action<string, OutputFile[]> UpdatedOutputFiles { get; set; }

        public Func<object, string, string> GetPropertyValue { get; set; }

        public Func<object, string, string, string> GetMetadataValue { get; set; }

        #region ITextTemplatingEngineHost implementation

        /// <summary>
        /// Gets a list of assembly references. 
        /// </summary>
        IList<string> ITextTemplatingEngineHost.StandardAssemblyReferences
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets a list of namespaces.
        /// </summary>
        IList<string> ITextTemplatingEngineHost.StandardImports
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets or sets the path and file name of the text template that is being processed.
        /// </summary>
        public string TemplateFile { get; set; }

        /// <summary>
        /// Gets host-specific option.
        /// </summary>
        /// <param name="optionName">Name of the host-specific option.</param>
        /// <returns>Option value.</returns>
        object ITextTemplatingEngineHost.GetHostOption(string optionName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Loads the requested file name. The file name can be specified only as rooted file path, or path relative to the current directory.
        /// </summary>
        bool ITextTemplatingEngineHost.LoadIncludeText(string requestFileName, out string content, out string location)
        {
            if (File.Exists(requestFileName))
            {
                location = Path.GetFullPath(requestFileName);
                content = File.ReadAllText(location);
                return true;
            }

            location = null;
            content = null;
            return false;
        }

        /// <summary>
        /// Receives a collection of errors and warnings from the transformation 
        /// engine. 
        /// </summary>
        /// <param name="errors">
        /// The <see cref="CompilerErrorCollection"/> being passed to the host from 
        /// the engine.
        /// </param>
        void ITextTemplatingEngineHost.LogErrors(CompilerErrorCollection errors)
        {
            if (this.LoggedErrors != null)
            {
                this.LoggedErrors(errors);
            }
        }

        /// <summary>
        /// Provides an application domain to run the generated transformation class. 
        /// </summary>
        /// <param name="content">
        /// The contents of the text template file to be processed. 
        /// </param>
        /// <returns>
        /// An <see cref="AppDomain"/> that compiles and executes the generated 
        /// transformation class.
        /// </returns>
        AppDomain ITextTemplatingEngineHost.ProvideTemplatingAppDomain(string content)
        {
            throw new NotImplementedException();
        }

        private class AssemblyResolver : MarshalByRefObject
        {
            [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "This method needs to be an instance method to allow invocation accross AppDomain boundaries.")]
            public string ResolveAssemblyReference(string assemblyReference)
            {
                return Assembly.Load(assemblyReference).Location;
            }
        }

        /// <summary>
        /// Resolves full assembly references from GAC. Assembly reference must include name, version, public key token and processor architecture.
        /// </summary>
        string ITextTemplatingEngineHost.ResolveAssemblyReference(string assemblyReference)
        {
            AppDomain tempDomain = AppDomain.CreateDomain("AssemblyResolutionDomain");
            try
            {
                var resolver = (AssemblyResolver)tempDomain.CreateInstanceFromAndUnwrap(this.GetType().Assembly.Location, typeof(AssemblyResolver).FullName);
                string resolvedReference = resolver.ResolveAssemblyReference(assemblyReference);
                return resolvedReference;
            }
            finally
            {
                AppDomain.Unload(tempDomain);
            }
        }

        /// <summary>
        /// Returns the type of a directive processor, given its friendly name. 
        /// </summary>
        /// <param name="processorName">
        /// The name of the directive processor to be resolved.
        /// </param>
        /// <returns>
        /// The <see cref="Type"/> of the directive processor.
        /// </returns>
        Type ITextTemplatingEngineHost.ResolveDirectiveProcessor(string processorName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Resolves the value of a parameter for a directive processor if the parameter 
        /// is not specified in the template text. 
        /// </summary>
        /// <param name="directiveId">
        /// The ID of the directive call to which the parameter belongs. This ID 
        /// disambiguates repeated calls to the same directive from the same text template.
        /// </param>
        /// <param name="processorName">
        /// The name of the directive processor to which the directive belongs.
        /// </param>
        /// <param name="parameterName">
        /// The name of the parameter to be resolved.
        /// </param>
        /// <returns>
        /// A <see cref="String"/> that represents the resolved parameter value.
        /// </returns>
        string ITextTemplatingEngineHost.ResolveParameterValue(string directiveId, string processorName, string parameterName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Resolves relative path into an absolute path.
        /// </summary>
        /// <param name="path">Relative file path.</param>
        /// <returns>Absolute file path.</returns>
        string ITextTemplatingEngineHost.ResolvePath(string path)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Tells the host the file name extension that is expected for the generated 
        /// text output. 
        /// </summary>
        /// <param name="extension">
        /// The file name extension for the generated text output.
        /// </param>
        void ITextTemplatingEngineHost.SetFileExtension(string extension)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Tells the host the encoding that is expected for the generated text output. 
        /// </summary>
        /// <param name="encoding">
        /// The encoding for the generated text output.
        /// </param>
        /// <param name="fromOutputDirective">
        /// <code>true</code> to indicate that the user specified the encoding in the encoding parameter of the output directive.
        /// </param>
        void ITextTemplatingEngineHost.SetOutputEncoding(Encoding encoding, bool fromOutputDirective)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IServiceProvider implementation

        object IServiceProvider.GetService(Type serviceType)
        {
            if (serviceType == typeof(ITransformationContextProvider))
            {
                return this;
            }

            return null;
        }

        #endregion

        #region ITransformationContextProvider implementation

        string ITransformationContextProvider.GetMetadataValue(object hierarchy, string fileName, string metadataName)
        {
            if (this.GetMetadataValue != null)
            {
                return this.GetMetadataValue(hierarchy, fileName, metadataName);
            }

            return string.Empty;
        }

        string ITransformationContextProvider.GetPropertyValue(object hierarchy, string propertyName)
        {
            if (this.GetPropertyValue != null)
            {
                return this.GetPropertyValue(hierarchy, propertyName);
            }

            throw new NotImplementedException();
        }

        void ITransformationContextProvider.UpdateOutputFiles(string inputFile, OutputFile[] outputFiles)
        {
            if (this.UpdatedOutputFiles != null)
            {
                this.UpdatedOutputFiles(inputFile, outputFiles);
            }
        }

        #endregion

        #region ITextTemplatingComponents

        public ITextTemplatingCallback Callback { get; set; }

        public ITextTemplatingEngine Engine { get; set; }

        public object Hierarchy { get; set; }

        public ITextTemplatingEngineHost Host { get; set; }

        string ITextTemplatingComponents.InputFile { get; set; }

        #endregion

        #region ITextTemplating

        public void BeginErrorSession()
        {
            throw new NotImplementedException();
        }

        public bool EndErrorSession()
        {
            throw new NotImplementedException();
        }

        public string PreprocessTemplate(string inputFile, string content, ITextTemplatingCallback callback, string className, string classNamespace, out string[] references)
        {
            var assemblyExpression = new Regex("<\\#\\@\\s*assembly\\s+name\\s*=\\s*\"(?<Assembly>[^\"]*)\"\\s*\\#>", RegexOptions.IgnoreCase);
            references = assemblyExpression.Matches(content).Cast<Match>().Select(match => match.Groups["Assembly"].Value).ToArray();
            return string.Empty;
        }

        public string ProcessTemplate(string inputFile, string content, ITextTemplatingCallback callback, object hierarchy)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
