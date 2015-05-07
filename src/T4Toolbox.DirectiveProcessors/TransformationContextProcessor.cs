// <copyright file="TransformationContextProcessor.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.DirectiveProcessors
{
    using System.CodeDom;
    using System.Collections.Generic;

    /// <summary>
    /// Generates initialization and cleanup logic required for <see cref="TransformationContext"/>.
    /// </summary>
    public class TransformationContextProcessor : DirectiveProcessor
    {
        /// <summary>
        /// Name of the directive processor, as referenced by the templates.
        /// </summary>
        public const string Name = "T4Toolbox.TransformationContextProcessor";

        private bool directiveProcessed = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransformationContextProcessor"/> class.
        /// </summary>
        public TransformationContextProcessor()
        {
            // Force T4 generate Host property, even if the template directive does not say so explicitly.
            this.RequiresProcessingRunIsHostSpecific = true;
        }

        /// <summary>
        /// Gets the directive name as it is supposed to be used in template code.
        /// </summary>
        /// <value>
        /// A <see cref="string"/> that contains name of the TransformationContext directive.
        /// </value>
        protected override string DirectiveName
        {
            get { return "TransformationContext"; }
        }

        /// <summary>
        /// Generates code in the class area of the transformation class created by the T4 engine.
        /// </summary>
        /// <param name="arguments">The arguments for the directive.</param>
        protected override void Process(IDictionary<string, string> arguments)
        {
            // Don't generate the same code more then once if T4Toolbox.tt happens to be included multiple times
            if (this.directiveProcessed)
            {
                this.Warning("Multiple <#@ include file=\"T4Toolbox.tt\" #> directives were found in the template.");
                return;
            }

            this.References.Add(typeof(TransformationContext).Assembly.Location);

            // Add the following method call to the Initialize method of the generated text template.
            // T4Toolbox.TransformationContext.Initialize(this, this.GenerationEnvironment);
            var initialize = new CodeExpressionStatement(
                new CodeMethodInvokeExpression(
                    new CodeTypeReferenceExpression(typeof(TransformationContext).FullName),
                    "Initialize",
                    new CodeThisReferenceExpression(),
                    new CodePropertyReferenceExpression(new CodeThisReferenceExpression(), "GenerationEnvironment")));
            this.LanguageProvider.GenerateCodeFromStatement(initialize, this.PreInitializationCode, null);

            // Add the following method call to the Dispose(bool) method of the generated text template.
            // T4Toolbox.TransformationContext.Cleanup();
            var cleanup = new CodeExpressionStatement(
                new CodeMethodInvokeExpression(
                    new CodeTypeReferenceExpression(typeof(TransformationContext).FullName),
                    "Cleanup"));
            this.LanguageProvider.GenerateCodeFromStatement(cleanup, this.DisposeCode, null);

            this.directiveProcessed = true;
        }
    }
}
