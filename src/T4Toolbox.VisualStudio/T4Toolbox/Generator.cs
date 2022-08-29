// <copyright file="Generator.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox
{
    using System;
    using System.CodeDom.Compiler;
    using System.Globalization;

    /// <summary>
    /// Abstract base class for code generators.
    /// </summary>
    public abstract class Generator
    {
        private readonly CompilerErrorCollection errors = new CompilerErrorCollection();
        private TransformationContext context;

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
        /// Gets collections of errors and warnings produced by the <see cref="Run"/> method.
        /// </summary>
        /// <value>
        /// A collection of <see cref="CompilerError"/> objects.
        /// </value>
        public CompilerErrorCollection Errors
        {
            get { return this.errors; }
        }

        /// <summary>
        /// Adds a new error to the list of <see cref="Errors"/> produced by the current <see cref="Run"/>.
        /// </summary>
        /// <param name="message">
        /// Error message.
        /// </param>
        public void Error(string message)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }

            this.Errors.Add(new CompilerError { ErrorText = message });
        }

        /// <summary>
        /// Adds a new error to the list of <see cref="Errors"/> produced by the current <see cref="Run"/>.
        /// </summary>
        /// <param name="format">
        /// A <see cref="string.Format(string, object)"/> string of the error message.
        /// </param>
        /// <param name="args">
        /// An array of one or more <paramref name="format"/> arguments.
        /// </param>
        public void Error(string format, params object[] args)
        {
            this.Error(string.Format(CultureInfo.CurrentCulture, format, args));
        }

        /// <summary>
        /// Validates and runs the generator.
        /// </summary>
        public void Run()
        {
            this.Errors.Clear();

            try
            {
                this.Validate();
                if (!this.Errors.HasErrors)
                {
                    this.RunCore();
                }
            }
            catch (TransformationException e)
            {
                this.Error(e.Message);
            }
            finally
            {
                this.Context.ReportErrors(this.Errors);                
            }
        }

        /// <summary>
        /// Adds a new warning to the list of <see cref="Errors"/> produced by the current <see cref="Run"/>.
        /// </summary>
        /// <param name="message">
        /// Warning message.
        /// </param>
        public void Warning(string message)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }

            this.Errors.Add(new CompilerError { IsWarning = true, ErrorText = message });
        }

        /// <summary>
        /// Adds a new warning to the list of <see cref="Errors"/> produced by the current <see cref="Run"/>.
        /// </summary>
        /// <param name="format">
        /// A <see cref="string.Format(string, object)"/> string of the warning message.
        /// </param>
        /// <param name="args">
        /// An array of one or more <paramref name="format"/> arguments.
        /// </param>
        public void Warning(string format, params object[] args)
        {
            this.Warning(string.Format(CultureInfo.CurrentCulture, format, args));
        }

        /// <summary>
        /// When overridden in a derived class, generates output files.
        /// </summary>
        /// <remarks>
        /// Override this method in derived classes to <see cref="Template.Render"/> 
        /// one or more <see cref="Template"/>s. Note that this method will not be executed
        /// if <see cref="Validate"/> method produces one or more <see cref="Errors"/>.
        /// </remarks>
        protected abstract void RunCore();

        /// <summary>
        /// When overridden in a derived class, validates parameters of the generator.
        /// </summary>
        /// <remarks>
        /// Override this method in derived classes to validate required and optional
        /// parameters of this <see cref="Generator"/>. Call <see cref="Error(string)"/>, 
        /// <see cref="Warning(string)"/> methods or throw <see cref="TransformationException"/> 
        /// to report errors.
        /// </remarks>
        protected virtual void Validate()
        {
            // This method is intentionally left blank.
        }
    }
}