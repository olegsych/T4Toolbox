// <copyright file="DirectiveDescriptor.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.TemplateAnalysis
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Linq;

    /// <summary>
    /// Provides access to metadata describing a single <see cref="Directive"/>.
    /// </summary>
    /// <remarks>
    /// Directive metadata is inferred from type information and .NET attributes applied to the <see cref="Directive"/> 
    /// classes like <see cref="AssemblyDirective"/> and <see cref="TemplateDirective"/>. This class encapsulates the 
    /// logic required to access this information via reflection and caches results to improve performance.
    /// </remarks>
    internal sealed class DirectiveDescriptor
    {
        private static IReadOnlyDictionary<Type, DirectiveDescriptor> builtInDirectives;

        private readonly Type directiveType;

        private IReadOnlyDictionary<string, AttributeDescriptor> attributes;
        private AttributeCollection metadataAttributes;
        private string description;
        private string displayName;

        private DirectiveDescriptor(Type directiveType)
        {
            Debug.Assert(directiveType != null, "directiveType");
            this.directiveType = directiveType;
        }

        /// <summary>
        /// Gets a read-only dictionary of <see cref="AttributeDescriptor"/> objects describing attributes of the directive.
        /// </summary>
        public IReadOnlyDictionary<string, AttributeDescriptor> Attributes
        {
            get
            {
                if (this.attributes == null)
                {
                    this.attributes = TypeDescriptor.GetProperties(this.directiveType)
                        .Cast<PropertyDescriptor>()
                        .Where(p => p.ComponentType == this.directiveType)
                        .Select(p => new AttributeDescriptor(p))
                        .ToDictionary(a => a.DisplayName, StringComparer.OrdinalIgnoreCase);
                }

                return this.attributes;
            }
        }

        /// <summary>
        /// Gets the directive name as it appears in IntelliSense.
        /// </summary>
        /// <remarks>
        /// Display name of the directive is determined from the <see cref="DisplayNameAttribute"/> applied to its class.
        /// </remarks>
        public string DisplayName
        {
            get { return this.displayName ?? (this.displayName = this.GetAttribute<DisplayNameAttribute>().DisplayName); }
        }

        /// <summary>
        /// Gets description of the directive.
        /// </summary>
        /// <remarks>
        /// Description of the directive is determined from the <see cref="DescriptionAttribute"/> applied to its class.
        /// </remarks>
        public string Description
        {
            get { return this.description ?? (this.description = this.GetAttribute<DescriptionAttribute>().Description); }
        }

        /// <summary>
        /// Returns a list of <see cref="DirectiveDescriptor"/> objects representing all built-in T4 directives.
        /// </summary>
        public static IEnumerable<DirectiveDescriptor> GetBuiltInDirectives()
        {
            if (DirectiveDescriptor.builtInDirectives == null)
            {
                var knownDirectives = new Dictionary<Type, DirectiveDescriptor>();
                foreach (Type type in typeof(Directive).Assembly.GetTypes())
                {
                    if (type.IsSubclassOf(typeof(Directive)))
                    {
                        knownDirectives.Add(type, new DirectiveDescriptor(type));
                    }
                }

                DirectiveDescriptor.builtInDirectives = knownDirectives;
            }

            return DirectiveDescriptor.builtInDirectives.Values;
        }

        public static DirectiveDescriptor GetDirectiveDescriptor(Type directiveType)
        {
            GetBuiltInDirectives();

            DirectiveDescriptor descriptor;
            if (!DirectiveDescriptor.builtInDirectives.TryGetValue(directiveType, out descriptor))
            {
                // Unknown directive. Assume it's defined by a unit test and don't cache.
                descriptor = new DirectiveDescriptor(directiveType);
            }

            return descriptor;
        }

        private T GetAttribute<T>() where T : System.Attribute
        {
            if (this.metadataAttributes == null)
            {
                this.metadataAttributes = TypeDescriptor.GetAttributes(this.directiveType);
            }

            return (T)this.metadataAttributes[typeof(T)];
        }
    }
}