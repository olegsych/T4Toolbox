// <copyright file="TransformationContext.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox
{
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Remoting.Messaging;
    using System.Text;
    using Microsoft.VisualStudio.TextTemplating;
    using Microsoft.VisualStudio.TextTemplating.VSHost;

    /// <summary>
    /// Provides information about template transformation environment and additional services such as 
    /// generation of multiple output files.
    /// </summary>
    public class TransformationContext : IServiceProvider, IDisposable
    {
        internal const string InputFileNameKey = "InputFileName";
        internal const string InputFileContentKey = "InputFileContent";

        private readonly List<OutputFile> outputFiles = new List<OutputFile>();
        private readonly ITransformationContextProvider provider;

        internal TransformationContext(TextTransformation transformation, StringBuilder generationEnvironment)
        {
            if (transformation == null)
            {
                throw new ArgumentNullException("transformation");
            }

            if (generationEnvironment == null)
            {
                throw new ArgumentNullException("generationEnvironment");
            }

            PropertyInfo hostProperty = transformation.GetType().GetProperty("Host");
            if (hostProperty == null)
            {
                throw new ArgumentException("TextTransformation does not have Host property");
            }

            this.Host = (ITextTemplatingEngineHost)hostProperty.GetValue(transformation);
            if (this.Host == null)
            {
                throw new ArgumentException("TextTransformation.Host is null.");
            }

            this.Transformation = transformation;

            // Create a special output file for the transformation output.
            this.outputFiles.Add(new OutputFile(generationEnvironment));

            this.provider = (ITransformationContextProvider)this.GetService(typeof(ITransformationContextProvider));
            if (this.provider == null)
            {
                throw new InvalidOperationException("ITransformationContextProvider service is not available.");
            }

            this.InitializeParameters();
        }

        /// <summary>
        /// Occurs when the <see cref="TransformationContext"/> is disposed at the end of the transformation.
        /// </summary>
        public event EventHandler Disposed;

        /// <summary>
        /// Gets the current <see cref="TransformationContext"/> instance.
        /// </summary>
        /// <value>
        /// The <see cref="TransformationContext"/> for the current <see cref="TextTransformation"/>.
        /// </value>>
        public static TransformationContext Current
        {
            get
            {
                var current = CallContext.HostContext as TransformationContext;
                if (current == null)
                {
                    throw new TransformationException(
                        "T4 Toolbox transformation context was not properly initialized. " +
                        "Add <#@ include file=\"T4Toolbox.tt\" #> directive to your text template.");                    
                }

                return current;
            }
        }

        /// <summary>
        /// Gets <see cref="ITextTemplatingEngineHost"/> which is running the 
        /// <see cref="Transformation"/>.
        /// </summary>
        /// <value>
        /// An <see cref="ITextTemplatingEngineHost"/> instance.
        /// </value>
        /// <exception cref="TransformationException">
        /// When <see cref="TransformationContext"/> has not been properly initialized;
        /// or when currently running <see cref="TextTransformation"/> is not host-specific.
        /// </exception>
        public ITextTemplatingEngineHost Host { get; private set; }

        /// <summary>
        /// Gets the currently running, generated <see cref="TextTransformation"/> object.
        /// </summary>
        /// <value>A <see cref="TextTransformation"/> object.</value>
        /// <exception cref="TransformationException">
        /// When <see cref="TransformationContext"/> has not been properly initialized.
        /// </exception>
        public TextTransformation Transformation { get; private set; }

        internal string InputFile
        {
            get
            {
                object inputFile;
                if (this.Transformation.Session != null && this.Transformation.Session.TryGetValue(InputFileNameKey, out inputFile))
                {
                    return (string)inputFile;
                }

                return this.Host.TemplateFile;
            }
        }

        /// <summary>
        /// T4 Toolbox infrastructure. Do not call.
        /// </summary>
        /// <param name="transformation">
        /// Instance of the <see cref="TextTransformation"/> class generated by T4 engine.
        /// </param>
        /// <param name="generationEnvironment">
        /// The <see cref="TextTransformation.GenerationEnvironment"/> of the <paramref name="transformation"/>.
        /// </param>
        /// <remarks>
        /// Initializes the <see cref="Current"/> context.
        /// </remarks>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void Initialize(TextTransformation transformation, StringBuilder generationEnvironment)
        {
            CallContext.HostContext = new TransformationContext(transformation, generationEnvironment);
        }

        /// <summary>
        /// T4 Toolbox infrastructure. Do not call.
        /// </summary>
        /// <remarks>
        /// Disposes the <see cref="Current"/> context and clears the property.
        /// </remarks>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void Cleanup()
        {
            // Dispose current context only if was properly initialized. 
            // Any exceptions thrown here may obscure initialization exceptions.
            var currentContext = CallContext.HostContext as TransformationContext;
            if (currentContext != null) 
            {
                currentContext.Dispose();
                CallContext.HostContext = null;
            }
        }

        /// <summary>
        /// Disposes the <see cref="TransformationContext"/> instance.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Returns value of an MSBuild metadata element with the specified <paramref name="metadataName"/> from the current template file.
        /// </summary>
        /// <returns>
        /// A <see cref="String"/> containing value of the the metadata element with the specified name or an <see cref="String.Empty"/>
        /// string if the the metadata element does not exist.
        /// </returns>
        public string GetMetadataValue(string metadataName)
        {
            if (metadataName == null)
            {
                throw new ArgumentNullException("metadataName");
            }

            if (string.IsNullOrWhiteSpace(metadataName))
            {
                throw new ArgumentException("Metadata name should not be empty.");
            }

            var components = (ITextTemplatingComponents)this.Host;
            return this.provider.GetMetadataValue(components.Hierarchy, this.InputFile, metadataName);
        }

        /// <summary>
        /// Returns value of an MSBuild property with the specified <paramref name="propertyName"/> from the current project.
        /// </summary>
        /// <returns>
        /// A <see cref="String"/> containing value of the property with the specified name or an <see cref="String.Empty"/> string
        /// if the property does not exist.
        /// </returns>
        public string GetPropertyValue(string propertyName)
        {
            if (propertyName == null)
            {
                throw new ArgumentNullException("propertyName");
            }

            if (string.IsNullOrWhiteSpace(propertyName))
            {
                throw new ArgumentException("Property name should not be empty.");
            }

            var components = (ITextTemplatingComponents)this.Host;
            return this.provider.GetPropertyValue(components.Hierarchy, propertyName);
        }

        /// <summary>
        /// Gets the service object of the specified type.
        /// </summary>
        /// <param name="serviceType">
        /// An object that specifies the type of service object to get. 
        /// </param>
        /// <returns>
        /// A service object of type <paramref name="serviceType"/>, or null if the specified service type is not available.
        /// </returns>
        public object GetService(Type serviceType)
        {
            var serviceProvider = (IServiceProvider)this.Host;
            return serviceProvider.GetService(serviceType);
        }

        /// <summary>
        /// Writes text to the output of this transformation.
        /// </summary>
        /// <param name="output">An <see cref="OutputItem"/> object that specifies how the text must be saved.</param>
        /// <param name="text">A <see cref="String"/> that contains generated content.</param>
        public void Write(OutputItem output, string text)
        {
            if (output == null)
            {
                throw new ArgumentNullException("output");
            }

            output.Validate();

            OutputFile outputFile = this.outputFiles.FirstOrDefault(o => string.Equals(o.Path, output.Path, StringComparison.OrdinalIgnoreCase));

            // If content was previously generated for this file
            if (outputFile != null)
            {
                // Validate new output attributes for consistency
                outputFile.Validate(output);
            }
            else
            {
                // create a new output file
                outputFile = new OutputFile();
                this.outputFiles.Add(outputFile);
            }

            outputFile.CopyPropertiesFrom(output); 
            
            if (string.IsNullOrEmpty(output.File))
            {
                // Append text to transformation output
                this.WriteToTransformation(text);
            }
            else
            {
                // Append text to the output file
                outputFile.Content.Append(text);                
            }
        }

        /// <summary>
        /// Copies errors from the specified collection of <paramref name="errors"/> to the
        /// list of <see cref="TextTransformation.Errors"/> that will be displayed in Visual
        /// Studio error window.
        /// </summary>
        /// <param name="errors">A collection of <see cref="CompilerError"/> objects.</param>
        internal void ReportErrors(CompilerErrorCollection errors)
        {
            foreach (CompilerError error in errors)
            {
                if (string.IsNullOrEmpty(error.FileName))
                {
                    error.FileName = this.InputFile;
                }
            }

            this.Transformation.Errors.AddRange(errors);
        }

        /// <summary>
        /// Disposes the <see cref="TransformationContext"/> instance.
        /// </summary>
        /// <param name="disposing">
        /// A <see cref="bool"/> that indicates whether the object is being disposed or finalized.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                try
                {
                    this.provider.UpdateOutputFiles(this.InputFile, this.outputFiles.ToArray());

                    if (this.Disposed != null)
                    {
                        this.Disposed(this, EventArgs.Empty);
                    }
                }
                catch (TransformationException e)
                {
                    this.LogError(e.Message);
                }
                finally
                {
                    this.outputFiles.Clear();
                }
            }
        }

        /// <summary>
        /// Initializes properties generated by T4 based on parameter directives.
        /// </summary>
        private void InitializeParameters()
        {
            Type transformationType = this.Transformation.GetType();

            foreach (PropertyInfo property in transformationType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                if (property.DeclaringType != transformationType)
                {
                    // Properties declared in the base classes are not parameters
                    continue;
                }

                if ((this.Transformation.Session != null && this.Transformation.Session.ContainsKey(property.Name)) || 
                    CallContext.LogicalGetData(property.Name) != null)
                {
                    // Parameter values specified explicitly by the code transforming the template take precedence over values specified in the metadata
                    continue;
                }

                string metadataValue = this.GetMetadataValue(property.Name);
                if (string.IsNullOrEmpty(metadataValue))
                {
                    continue;
                }

                TypeConverter propertyTypeConverter = TypeDescriptor.GetConverter(property.PropertyType);
                if (!propertyTypeConverter.CanConvertFrom(typeof(string)))
                {
                    this.LogError(string.Format(CultureInfo.CurrentCulture, "The metadata value '{0}' could not be converted to type {1} of the template parameter {2}.", metadataValue, property.PropertyType, property.Name));
                    continue;
                }

                if (this.Transformation.Session == null)
                {
                    // ModelingTextTransformation overrides TextTransformation.Session property and returns null by default. We need to handle this scenario gracefully.
                    this.LogError(string.Format(CultureInfo.CurrentCulture, "Template parameter {0} could not be initialized from metadata because transformation session is not available.", property.Name));
                    continue;
                }

                this.Transformation.Session[property.Name] = propertyTypeConverter.ConvertFrom(metadataValue);
            }
        }

        private void LogError(string message)
        {
            this.Host.LogErrors(new CompilerErrorCollection { new CompilerError { ErrorText = message, FileName = this.InputFile } });
        }

        /// <summary>
        /// Writes text to the <see cref="Transformation"/>, preserving its current indentation.
        /// </summary>
        private void WriteToTransformation(string text)
        {
            using (var reader = new StringReader(text))
            {
                while (true)
                {
                    string line = reader.ReadLine();
                    if (line == null)
                    {
                        break;
                    }

                    // Is this the last line?
                    if (reader.Peek() == -1)
                    {
                        this.Transformation.Write(line);
                    }
                    else
                    {
                        this.Transformation.WriteLine(line);
                    }
                }
            }
        }
    }
}