// <copyright file="AttributeDescriptor.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.TemplateAnalysis
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Provides access to metadata describing a single directive <see cref="Attribute"/>.
    /// </summary>
    /// <remarks>
    /// Attribute metadata is inferred from the type information and .NET attributes applied 
    /// to the attribute property defined in a strongly-typed <see cref="Directive"/> class,
    /// such as <see cref="TemplateDirective.Language"/>.
    /// </remarks>
    internal sealed class AttributeDescriptor
    {
        private readonly PropertyDescriptor property;
        private IReadOnlyDictionary<string, ValueDescriptor> values;

        internal AttributeDescriptor(PropertyDescriptor property)
        {
            Debug.Assert(property != null, "property");
            this.property = property;
        }

        /// <summary>
        /// Gets the attribute name as it appears in IntelliSense.
        /// </summary>
        /// <remarks>
        /// Display name of the attribute is determined from the <see cref="DisplayNameAttribute"/>
        /// applied to the attribute property or from the property name itself.
        /// </remarks>
        public string DisplayName
        {
            get { return this.property.DisplayName; }
        }

        /// <summary>
        /// Gets description of the attribute.
        /// </summary>
        /// <remarks>
        /// Description of the attribute is determined from the <see cref="DescriptionAttribute"/>
        /// applied to the attribute property.
        /// </remarks>
        public string Description
        {
            get { return this.property.Description; }
        }

        /// <summary>
        /// Gets a dictionary of <see cref="ValueDescriptor"/> objects keyed by their display name.
        /// </summary>
        public IReadOnlyDictionary<string, ValueDescriptor> Values
        {
            get { return this.values ?? (this.values = this.CalculateValues()); }
        }

        private IReadOnlyDictionary<string, ValueDescriptor> CalculateValues()
        {
            var values = new Dictionary<string, ValueDescriptor>(StringComparer.OrdinalIgnoreCase);

            var attribute = (KnownValuesAttribute)this.property.Attributes[typeof(KnownValuesAttribute)];
            if (attribute != null)
            {
                foreach (ValueDescriptor knownValue in attribute.KnownValues)
                {
                    values.Add(knownValue.DisplayName, knownValue);
                }
            }

            return values;
        }        
    }
}