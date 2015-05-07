// <copyright file="DirectiveProcessor.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.DirectiveProcessors
{
    using System;
    using System.CodeDom;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using Microsoft.VisualStudio.TextTemplating;

    /// <summary>
    /// Base class for directive processors.
    /// </summary>
    public abstract class DirectiveProcessor : IDirectiveProcessor, IDisposable
    {
        /// <summary>
        /// Gets the collection to which the directive processor can add errors and warnings.
        /// </summary>
        /// <value>A <see cref="CompilerErrorCollection"/> instance.</value>
        public CompilerErrorCollection Errors { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether the directive processor requires the run to be host-specific.
        /// </summary>
        /// <value>
        /// True if the directive processor requires a host-specific processing run.
        /// </value>
        public bool RequiresProcessingRunIsHostSpecific { get; protected set; }

        /// <summary>
        /// Gets a T4 engine host.
        /// </summary>
        /// <value>
        /// A <see cref="ITextTemplatingEngineHost"/> object obtained by the <see cref="IDirectiveProcessor.Initialize"/> method.
        /// </value>
        protected ITextTemplatingEngineHost Host { get; private set; }

        /// <summary>
        /// Gets a collection of attribute declarations that will be applied to the generated transformation class.
        /// </summary>
        /// <value>
        /// A collection of <see cref="CodeAttributeDeclaration"/> objects.
        /// </value>
        protected CodeAttributeDeclarationCollection ClassAttributes { get; private set; }

        /// <summary>
        /// Gets the class code buffer.
        /// </summary>
        /// <value>
        /// A <see cref="StringWriter"/> object that serves as a buffer for generated code.
        /// </value>
        protected StringWriter ClassCode { get; private set; }

        /// <summary>
        /// Gets the directive name.
        /// </summary>
        /// <value> A <see cref="String"/> that contains directive name.</value>
        /// <remarks>
        /// Override this property in the derived class to indicate which directive 
        /// the processor will handle.
        /// </remarks>
        protected abstract string DirectiveName { get; }

        /// <summary>
        /// Gets dispose code buffer.
        /// </summary>
        /// <value>
        /// A <see cref="StringWriter"/> object that serves as a buffer for generated code.
        /// </value>
        protected StringWriter DisposeCode { get; private set; }

        /// <summary>
        /// Gets the namespace imports buffer.
        /// </summary>
        /// <value>
        /// A <see cref="ICollection{String}"/> of namespaces.
        /// </value>
        protected ICollection<string> Imports { get; private set; }

        /// <summary>
        /// Gets the post-initialization code buffer.
        /// </summary>
        /// <value>
        /// A <see cref="StringWriter"/> object that serves as a buffer for generated code.
        /// </value>
        protected StringWriter PostInitializationCode { get; private set; }

        /// <summary>
        /// Gets the pre-initialization code buffer.
        /// </summary>
        /// <value>
        /// A <see cref="StringWriter"/> object that serves as a buffer for generated code.
        /// </value>
        protected StringWriter PreInitializationCode { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the current processing run is host-specific.
        /// </summary>
        /// <value>
        /// True, if the current processing run is host-specific.
        /// </value>
        protected bool ProcessingRunIsHostSpecific { get; private set; }

        /// <summary>
        /// Gets the assembly references buffer.
        /// </summary>
        /// <value>
        /// A <see cref="ICollection{String}"/> of assembly references.
        /// </value>
        protected ICollection<string> References { get; private set; }

        /// <summary>
        /// Gets a language provider.
        /// </summary>
        /// <value>
        /// A <see cref="CodeDomProvider"/> object obtained by the <see cref="IDirectiveProcessor.StartProcessingRun"/> method.
        /// </value>
        protected CodeDomProvider LanguageProvider { get; private set; }

        /// <summary>
        /// Releases disposable resources owned by the directive processor.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        #region IDirectiveProcessor

        /// <summary>
        /// Notifies the directive processor that the processing run is finished.
        /// </summary>
        public void FinishProcessingRun()
        {
            if (this.DisposeCode.GetStringBuilder().Length > 0)
            {
                this.GenerateDisposeMethod();
            }

            // Release external references received from T4
            this.LanguageProvider = null;

            // DO NOT release the internal buffers. T4 Engine accesses them between the runs.
        }

        /// <summary>
        /// This method is not used and returns an empty string.
        /// </summary>
        /// <returns>
        /// A <see cref="string"/> that contains the code to add to the generated 
        /// transformation class.
        /// </returns>
        public string GetClassCodeForProcessingRun()
        {
            return this.ClassCode.ToString();
        }

        /// <summary>
        /// This method is not used and returns an empty array.
        /// </summary>
        /// <returns>
        /// An array of type <see cref="string"/> that contains the namespaces. 
        /// </returns>
        public string[] GetImportsForProcessingRun()
        {
            return this.Imports.Distinct().ToArray();
        }

        /// <summary>
        /// Gets a collection of custom attribute declarations to add to the generated template class.
        /// </summary>
        /// <returns><see cref="ClassAttributes"/>.</returns>
        public CodeAttributeDeclarationCollection GetTemplateClassCustomAttributes()
        {
            return this.ClassAttributes;
        }

        /// <summary>
        /// This method is not used and returns an empty string.
        /// </summary>
        /// <returns>
        /// A <see cref="String"/> that contains the code to add to the generated 
        /// transformation class. 
        /// </returns>
        public string GetPostInitializationCodeForProcessingRun()
        {
            return this.PostInitializationCode.ToString();
        }

        /// <summary>
        /// This method is not used and returns an empty string.
        /// </summary>
        /// <returns>
        /// A <see cref="String"/> that contains the code to add to the generated 
        /// transformation class. 
        /// </returns>
        public string GetPreInitializationCodeForProcessingRun()
        {
            return this.PreInitializationCode.ToString();
        }

        /// <summary>
        /// This method is not used and returns an empty array.
        /// </summary>
        /// <returns>
        /// An array of type <see cref="String"/> that contains the references.
        /// </returns>
        public string[] GetReferencesForProcessingRun()
        {
            return this.References.Distinct().ToArray();
        }

        /// <summary>
        /// T4 engine calls this method in the beginning of template transformation.
        /// </summary>
        /// <param name="host">
        /// The <see cref="ITextTemplatingEngineHost"/> object hosting the transformation.
        /// </param>
        public void Initialize(ITextTemplatingEngineHost host)
        {
            this.Host = host;
        }

        /// <summary>
        /// Returns <c>true</c> when <paramref name="directiveName"/> is "t4toolbox".
        /// </summary>
        /// <param name="directiveName">Name of the directive.</param>
        /// <returns>
        /// <c>true</c> if the directive is supported by the processor; otherwise, <c>false</c>. 
        /// </returns>
        public bool IsDirectiveSupported(string directiveName)
        {
            if (directiveName == null)
            {
                throw new ArgumentNullException("directiveName");
            }

            return string.Compare(this.DirectiveName, directiveName, StringComparison.OrdinalIgnoreCase) == 0;
        }

        /// <summary>
        /// This method is not used and left blank.
        /// </summary>
        /// <param name="directiveName">
        /// The name of the directive to process. 
        /// </param>
        /// <param name="arguments">
        /// The arguments for the directive. 
        /// </param>
        public void ProcessDirective(string directiveName, IDictionary<string, string> arguments)
        {
            // Validate directiveName argument
            if (!this.IsDirectiveSupported(directiveName))
            {
                throw new ArgumentException(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        "Unsupported directive name: '{0}'. Please use '{1}' instead.",
                        directiveName,
                        this.DirectiveName),
                    "directiveName");
            }

            // Validate arguments argument
            if (arguments == null)
            {
                throw new ArgumentNullException("arguments");
            }

            this.Process(arguments);
        }

        /// <summary>
        /// Informs the directive processor whether the run is host-specific.
        /// </summary>
        /// <param name="hostSpecific">True, if the processing run is host-specific.</param>
        public void SetProcessingRunIsHostSpecific(bool hostSpecific)
        {
            this.ProcessingRunIsHostSpecific = hostSpecific;
        }

        /// <summary>
        /// Begins a round of directive processing.
        /// </summary>
        /// <param name="languageProvider">CodeDom language provider for generating code.</param>
        /// <param name="templateContents">Contents of the T4 template.</param>
        /// <param name="errors">Compiler Errors.</param>
        public void StartProcessingRun(CodeDomProvider languageProvider, string templateContents, CompilerErrorCollection errors)
        {
            // Validate parameters
            if (languageProvider == null)
            {
                throw new ArgumentNullException("languageProvider");
            }

            if (errors == null)
            {
                throw new ArgumentNullException("errors");
            }

            // Initialize references to external objects provided by T4
            this.LanguageProvider = languageProvider;
            this.Errors = errors;

            // Clean up buffers here instead of FinishProcessingRun because T4 uses them between the runs
            this.CleanupBuffers();
            this.InitializeBuffers();
        }

        #endregion

        /// <summary>
        /// Disposes managed resources owned by this directive processor.
        /// </summary>
        /// <param name="disposing">
        /// This parameter is always true. It is provided for consistency with <see cref="IDisposable"/> pattern.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.CleanupBuffers();
            }
        }

        /// <summary>
        /// Processes the directive.
        /// </summary>
        /// <param name="arguments">
        /// A dictionary of arguments specified for the directive in the template.
        /// </param>
        protected abstract void Process(IDictionary<string, string> arguments);

        /// <summary>
        /// Reports a warning with the specified <paramref name="message"/>.
        /// </summary>
        protected void Warning(string message)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }

            this.Errors.Add(new CompilerError { ErrorText = message, FileName = this.Host.TemplateFile, IsWarning = true });
        }
        
        private void CleanupBuffers()
        {
            this.ClassAttributes = null;
            this.Imports = null;
            this.References = null;

            if (this.ClassCode != null)
            {
                this.ClassCode.Dispose();
                this.ClassCode = null;
            }

            if (this.DisposeCode != null)
            {
                this.DisposeCode.Dispose();
                this.DisposeCode = null;
            }

            if (this.PostInitializationCode != null)
            {
                this.PostInitializationCode.Dispose();
                this.PostInitializationCode = null;
            }

            if (this.PreInitializationCode != null)
            {
                this.PreInitializationCode.Dispose();
                this.PreInitializationCode = null;
            }
        }

        private void GenerateDisposeMethod()
        {
            //// protected override void Dispose(bool disposing) {
            var disposeMethod = new CodeMemberMethod { Name = "Dispose", Attributes = MemberAttributes.Family | MemberAttributes.Override };
            var disposingArgument = new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(bool)), "disposing");
            disposeMethod.Parameters.Add(disposingArgument);

            ////    base.Dispose(disposing);
            disposeMethod.Statements.Add(new CodeMethodInvokeExpression(
                new CodeBaseReferenceExpression(), 
                "Dispose", 
                new CodeArgumentReferenceExpression(disposingArgument.Name)));

            ////    if (disposing) {
            ////       // DisposeCode
            disposeMethod.Statements.Add(new CodeConditionStatement(
                new CodeArgumentReferenceExpression(disposingArgument.Name),
                new CodeSnippetStatement(this.DisposeCode.ToString())));

            ////    }
            //// }            
            this.LanguageProvider.GenerateCodeFromMember(disposeMethod, this.ClassCode, null);
        }

        private void InitializeBuffers()
        {
            this.ClassAttributes = new CodeAttributeDeclarationCollection();
            this.Imports = new List<string>();
            this.References = new List<string>();

            this.ClassCode = new StringWriter(CultureInfo.InvariantCulture);
            this.DisposeCode = new StringWriter(CultureInfo.InvariantCulture);
            this.PostInitializationCode = new StringWriter(CultureInfo.InvariantCulture);
            this.PreInitializationCode = new StringWriter(CultureInfo.InvariantCulture);
        }
    }
}
