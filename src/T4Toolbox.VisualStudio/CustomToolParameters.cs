// <copyright file="CustomToolParameters.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.Shell.Interop;
    using Microsoft.VisualStudio.TextTemplating;
    using Microsoft.VisualStudio.TextTemplating.VSHost;

    /// <summary>
    /// Represents a collection of text template parameters in the Properties window of Visual Studio.
    /// </summary>
    /// <remarks>
    /// This class implements the <see cref="ICustomTypeDescriptor"/> interface to present template parameters
    /// as a dynamic list of <see cref="PropertyDescriptor"/> objects.
    /// </remarks>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class CustomToolParameters : ICustomTypeDescriptor
    {
        private const string FileGroup = "F";
        private const string NameGroup = "N";
        private const string TypeGroup = "T";

        private static readonly Regex IncludeExpression = new Regex(
            "<\\#\\@\\s*include\\s+file\\s*=\\s*\"(?<" + FileGroup + ">[^\"]*)\"\\s*\\#>",
            RegexOptions.IgnoreCase);

        private static readonly Regex ParameterExpression = new Regex(
            "<\\#\\@\\s*parameter\\s+name\\s*=\\s*\"(?<" + NameGroup + ">[^\"]*)\"\\s+type\\s*=\\s*\"(?<" + TypeGroup + ">[^\"]*)\"\\s*\\#>",
            RegexOptions.IgnoreCase);

        private readonly IServiceProvider serviceProvider;
        private readonly IVsHierarchy project;
        private readonly uint projectItemId;

        private ITextTemplatingEngineHost templatingHost;
        private ITextTemplating templatingService;
        private string[] assemblyReferences;

        internal CustomToolParameters(IServiceProvider serviceProvider, IVsHierarchy project, uint projectItemId)
        {
            Debug.Assert(serviceProvider != null, "serviceProvider");
            Debug.Assert(project != null, "project");
            Debug.Assert(projectItemId != 0, "projectItemId");

            this.serviceProvider = serviceProvider;
            this.project = project;
            this.projectItemId = projectItemId;
        }

        /// <summary>
        /// Returns a default collection of attributes applied to this class.
        /// </summary>
        public AttributeCollection GetAttributes()
        {
            return TypeDescriptor.GetAttributes(this, noCustomTypeDesc: true);
        }

        /// <summary>
        /// Returns the default name of this class.
        /// </summary>
        public string GetClassName()
        {
            return TypeDescriptor.GetClassName(this, noCustomTypeDesc: true);
        }

        /// <summary>
        /// Returns the default name of this component.
        /// </summary>
        public string GetComponentName()
        {
            return TypeDescriptor.GetComponentName(this, noCustomTypeDesc: true);
        }

        /// <summary>
        /// Returns a <see cref="TypeConverter"/> specified in a <see cref="TypeConverterAttribute"/> applied to this class.
        /// </summary>
        public TypeConverter GetConverter()
        {
            return TypeDescriptor.GetConverter(this, noCustomTypeDesc: true);
        }

        /// <summary>
        /// Returns a default event defined in this class.
        /// </summary>
        public EventDescriptor GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(this, noCustomTypeDesc: true);
        }

        /// <summary>
        /// Returns a default property defined in this class.
        /// </summary>
        public PropertyDescriptor GetDefaultProperty()
        {
            return TypeDescriptor.GetDefaultProperty(this, noCustomTypeDesc: true);
        }

        /// <summary>
        /// Returns a default editor of this class.
        /// </summary>
        public object GetEditor(Type editorBaseType)
        {
            return TypeDescriptor.GetEditor(this, editorBaseType, noCustomTypeDesc: true);
        }

        /// <summary>
        /// Returns a default collection of events with the given attributes defined in this class.
        /// </summary>
        public EventDescriptorCollection GetEvents(Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(this, attributes, noCustomTypeDesc: true);
        }

        /// <summary>
        /// Returns a default collection of events defined in this class.
        /// </summary>
        public EventDescriptorCollection GetEvents()
        {
            return TypeDescriptor.GetEvents(this, noCustomTypeDesc: true);
        }

        /// <summary>
        /// Returns a collection of <see cref="CustomToolParameter"/> objects representing parameters defined in a text template.
        /// </summary>
        public PropertyDescriptorCollection GetProperties()
        {
            return this.GetProperties(null);
        }

        /// <summary>
        /// Returns a collection of <see cref="CustomToolParameter"/> objects representing parameters defined in a text template.
        /// </summary>
        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            this.templatingService = (ITextTemplating)this.serviceProvider.GetService(typeof(STextTemplating));
            this.templatingHost = (ITextTemplatingEngineHost)this.templatingService;

            string templateFileName;
            if (this.ResolveTemplate(out templateFileName))
            {
                string templateContent = File.ReadAllText(templateFileName, EncodingHelper.GetEncoding(templateFileName));

                this.templatingService.PreprocessTemplate(templateFileName, templateContent, null, "TemporaryClass", "T4Toolbox", out this.assemblyReferences);
                for (int i = 0; i < this.assemblyReferences.Length; i++)
                {
                    this.assemblyReferences[i] = this.templatingHost.ResolveAssemblyReference(this.assemblyReferences[i]);
                }

                var parameters = new List<CustomToolParameter>();
                this.ParseParameters(templateContent, parameters);
                return new PropertyDescriptorCollection(parameters.Cast<PropertyDescriptor>().ToArray());                
            }

            return PropertyDescriptorCollection.Empty;
        }

        /// <summary>
        /// Returns an owner of the specified <see cref="PropertyDescriptor"/>.
        /// </summary>
        public object GetPropertyOwner(PropertyDescriptor pd)
        {
            return this;
        }

        /// <summary>
        /// Returns an empty string to prevent the type name from being displayed in Visual Studio Properties window.
        /// </summary>
        public override string ToString()
        {
            return string.Empty;
        }

        internal void GetProjectItem(out IVsBuildPropertyStorage project, out uint itemId)
        {
            project = (IVsBuildPropertyStorage)this.project;
            itemId = this.projectItemId;
        }

        private void ParseParameters(string templateContent, List<CustomToolParameter> parameters)
        {
            // Parse any <#@ include #> directives from the template
            MatchCollection includeMatches = IncludeExpression.Matches(templateContent);
            foreach (Match includeMatch in includeMatches)
            {
                string includedFile = includeMatch.Groups[FileGroup].Value;
                string loadedContent, loadedFile;
                if (this.templatingHost.LoadIncludeText(includedFile, out loadedContent, out loadedFile))
                {
                    this.ParseParameters(loadedContent, parameters);
                }
            }

            // Parse any <#@ parameter #> directives from the template
            MatchCollection matches = ParameterExpression.Matches(templateContent);
            foreach (Match parameterMatch in matches)
            {
                parameters.Add(this.CreateParameter(parameterMatch));
            }
        }

        private bool ResolveTemplate(out string templateFileName)
        {
            templateFileName = string.Empty;

            string inputFileName;
            ErrorHandler.ThrowOnFailure(this.project.GetCanonicalName(this.projectItemId, out inputFileName));

            var propertyStorage = (IVsBuildPropertyStorage)this.project;

            string generator;
            if (ErrorHandler.Failed(propertyStorage.GetItemAttribute(this.projectItemId, ItemMetadata.Generator, out generator)))
            {
                return false;
            }

            if (string.Equals(generator, "TextTemplatingFileGenerator", StringComparison.OrdinalIgnoreCase))
            {
                templateFileName = inputFileName;
            }
            else if (string.Equals(generator, ScriptFileGenerator.Name, StringComparison.OrdinalIgnoreCase))
            {
                if (ErrorHandler.Failed(propertyStorage.GetItemAttribute(this.projectItemId, ItemMetadata.LastGenOutput, out templateFileName)))
                {
                    return false;
                }

                templateFileName = Path.Combine(Path.GetDirectoryName(inputFileName), templateFileName);
            }
            else if (string.Equals(generator, TemplatedFileGenerator.Name, StringComparison.OrdinalIgnoreCase))
            {
                if (ErrorHandler.Failed(propertyStorage.GetItemAttribute(this.projectItemId, ItemMetadata.Template, out templateFileName)))
                {
                    return false;
                }

                var templateLocator = (TemplateLocator)this.serviceProvider.GetService(typeof(TemplateLocator));
                if (!templateLocator.LocateTemplate(inputFileName, ref templateFileName))
                {
                    return false;
                }
            }

            return File.Exists(templateFileName);
        }

        private CustomToolParameter CreateParameter(Match match)
        {
            // Resolve parameter type
            string typeName = match.Groups[TypeGroup].Value;
            string description = string.Empty;
            Type type = null;
            try
            {
                type = Type.GetType(typeName, throwOnError: true, assemblyResolver: null, typeResolver: this.ResolveType);
            }
            catch (TypeLoadException e)
            {
                type = typeof(void);
                description = e.Message;
            }

            string name = match.Groups[NameGroup].Value;
            return new CustomToolParameter(name, type, description);            
        }

        [SuppressMessage("Microsoft.Reliability", "CA2001:AvoidCallingProblematicMethods", MessageId = "System.Reflection.Assembly.LoadFrom", Justification = "That's how the T4 Engine loads assemblies.")]
        private Type ResolveType(Assembly assembly, string typeName, bool ignoreCase)
        {
            // Try among assemblies already loaded in the current AppDomain
            Assembly[] loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly loadedAssembly in loadedAssemblies)
            {
                Type type = loadedAssembly.GetType(typeName, false, ignoreCase);
                if (type != null)
                {
                    return type;
                }
            }

            // Try among assemblies referenced by the template
            foreach (string assemblyFileName in this.assemblyReferences)
            {
                Assembly referencedAssembly = Assembly.LoadFrom(assemblyFileName);
                Type type = referencedAssembly.GetType(typeName, false, ignoreCase);
                if (type != null)
                {
                    return type;
                }
            }

            return null;
        }
    }
}