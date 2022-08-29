// <copyright file="Template.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using Microsoft.VisualStudio.TextTemplating;

    /// <summary>
    /// Abstract base class for nested template classes.
    /// </summary>
#pragma warning disable CS3009 // Base type is not CLS-compliant
    public abstract class Template : TextTransformation
#pragma warning restore CS3009 // Base type is not CLS-compliant
    {
        #region fields

        private readonly OutputItem output = new OutputItem();
        private TransformationContext context;
        private bool enabled = true;

        #endregion

        /// <summary>
        /// Occurs directly after <see cref="Render"/> method is called.
        /// </summary>
        /// <remarks>
        /// When implementing a composite <see cref="Generator"/>, use its constructor
        /// to specify an event handler to update the <see cref="Output"/> properties
        /// for individual templates. This will allow users of the generator to change
        /// how output is saved to fit their needs without modifying the generator itself.
        /// </remarks>
        public event EventHandler Rendering;

        /// <summary>
        /// Gets or sets <see cref="TransformationContext"/> of the template.
        /// </summary>
        /// <remarks>
        /// By default, this property returns the <see cref="TransformationContext.Current"/> context. 
        /// You can set this property in automated tests to isolate them from the global transformation context.
        /// </remarks>
        public TransformationContext Context
        {
            get
            {
                return this.context ?? (this.context = TransformationContext.Current);
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");                    
                }

                this.context = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Template"/> will be
        /// rendered.
        /// </summary>
        /// <value>
        /// A <see cref="Boolean"/> value. <code>true</code> if <see cref="Template"/> 
        /// will be rendered; otherwise, <code>false</code>. The default is <code>true</code>.
        /// </value>
        /// <remarks>
        /// This property allows users of complex code generators to turn off generation of
        /// a particular output type without having to re-implement the <see cref="Generator"/>.
        /// </remarks>
        public bool Enabled
        {
            get { return this.enabled; }
            set { this.enabled = value; }
        }

        /// <summary>
        /// Gets the object that determines where and how the output of this <see cref="Template"/>
        /// will be saved.
        /// </summary>
        /// <value>
        /// An <see cref="OutputItem"/> object.
        /// </value>
        /// <remarks>
        /// When implementing a composite <see cref="Generator"/>, use <see cref="Rendering"/>
        /// event to update <see cref="Output"/> properties each time when the <see cref="Template"/>
        /// is rendered.
        /// </remarks>
        public OutputItem Output
        {
            get { return this.output; }
        }

        /// <summary>
        /// Gets or sets the template transformation session.
        /// </summary>
        /// <exception cref="InvalidOperationException">When Session property is set.</exception>
        /// <value>
        /// An <see cref="IDictionary{String,Object}"/> object storing name/value pairs of session properties.
        /// </value>
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Cannot change definition of the inherited property.")]
        public override IDictionary<string, object> Session
        {
            get { return this.Context.Transformation.Session; }
            set { throw new InvalidOperationException("Transformation session should not be changed"); }
        }

        /// <summary>
        /// Adds a new error to the list of <see cref="TextTransformation.Errors"/> produced by the current template rendering.
        /// </summary>
        /// <param name="format">
        /// A <see cref="string.Format(string, object)"/> string of the error message.
        /// </param>
        /// <param name="args">
        /// An array of one or more <paramref name="format"/> arguments.
        /// </param>
        public void Error(string format, params object[] args)
        {
            base.Error(string.Format(CultureInfo.CurrentCulture, format, args));
        }

        /// <summary>
        /// Transforms the template and saves generated content based on <see cref="Output"/> settings.
        /// </summary>
        public void Render()
        {
            try
            {
                this.OnRendering(EventArgs.Empty);
                if (this.Enabled)
                {
                    string text = this.Transform();
                    this.Context.Write(this.Output, text);
                }
            }
            catch (TransformationException e)
            {
                // Report expected errors without exception call stack
                this.Error(e.Message);
            }
            finally
            {
                this.Context.ReportErrors(this.Errors);
            }
        }

        /// <summary>
        /// Transforms the template and saves generated content to the specified file.
        /// </summary>
        /// <param name="fileName">
        /// Name of the output file.
        /// </param>
        public void RenderToFile(string fileName)
        {
            this.Output.File = fileName;
            this.Render();
        }

        /// <summary>
        /// Transforms the template into output.
        /// </summary>
        /// <returns>
        /// A <see cref="string"/> with content generated by this template.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method emulates behavior of the T4's built-in TransformationRunner. 
        /// It initializes the template, validates, executes it and returns the generated 
        /// content. Unlike the <see cref="Render"/>, this method does not attempt to 
        /// save the generated content. 
        /// </para>
        /// <para>
        /// This method is low-level and intended for use in unit tests that verify 
        /// generated output. Use <see cref="Render"/> method when implementing 
        /// <see cref="Generator"/> code or code generation scripts.
        /// </para>
        /// </remarks>
        public string Transform()
        {
            // Clear results of previous transformation (if any)
            this.Errors.Clear();
            this.GenerationEnvironment.Remove(0, this.GenerationEnvironment.Length);

            // Run code generated by custom directive processors
            this.Initialize();

            // Verify pre-conditions 
            this.Validate();
            if (!this.Errors.HasErrors)
            {
                // Generate output
                return this.TransformText();
            }

            return this.GenerationEnvironment.ToString();
        }

        /// <summary>
        /// Adds a new warning to the list of <see cref="TextTransformation.Errors"/> produced by the current template rendering.
        /// </summary>
        /// <param name="format">
        /// A <see cref="string.Format(string, object)"/> string of the warning message.
        /// </param>
        /// <param name="args">
        /// An array of one or more <paramref name="format"/> arguments.
        /// </param>
        public void Warning(string format, params object[] args)
        {
            base.Warning(string.Format(CultureInfo.CurrentCulture, format, args));
        }

        /// <summary>
        /// Raises the <see cref="Rendering"/> event.
        /// </summary>
        /// <param name="e">
        /// An <see cref="EventArgs"/> that contains the event data. 
        /// </param>
        protected virtual void OnRendering(EventArgs e)
        {
            if (this.Rendering != null)
            {
                this.Rendering(this, e);
            }
        }

        /// <summary>
        /// When overridden in a derived class, validates parameters of the template.
        /// </summary>
        /// <remarks>
        /// Override this method in derived classes to validate required and optional
        /// parameters of this <see cref="Template"/>. Call <see cref="Error"/>, <see cref="Warning"/> 
        /// methods or throw <see cref="TransformationException"/> to report errors.
        /// </remarks>
        protected virtual void Validate()
        {
        }
    }
}