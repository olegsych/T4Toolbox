// <copyright file="CustomToolParameter.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Globalization;

    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.Shell.Interop;

    /// <summary>
    /// Represents a single parameter defined in a text template in the Properties window of Visual Studio.
    /// </summary>
    internal class CustomToolParameter : PropertyDescriptor
    {
        private readonly Type parameterType;
        private readonly string description;
        private readonly TypeConverter converter;

        public CustomToolParameter(string parameterName, Type parameterType, string description)
            : base(parameterName, null)
        {
            Debug.Assert(!string.IsNullOrEmpty(parameterName), "parameterName");
            Debug.Assert(parameterType != null, "parameterType");
            Debug.Assert(description != null, "description");

            this.parameterType = parameterType;
            this.description = description;
            this.converter = TypeDescriptor.GetConverter(parameterType);
        }

        public override Type ComponentType
        {
            get { return typeof(CustomToolParameters); }
        }

        public override bool IsReadOnly
        {
            get { return !this.converter.CanConvertTo(typeof(string)) || !this.converter.CanConvertFrom(typeof(string)); }
        }

        public override Type PropertyType
        {
            get { return this.parameterType; }
        }

        public override bool CanResetValue(object component)
        {
            return true;
        }

        public override object GetValue(object component)
        {
            IVsBuildPropertyStorage project;
            uint itemId;
            GetProjectItem(component, out project, out itemId);

            string stringValue;
            if (ErrorHandler.Failed(project.GetItemAttribute(itemId, this.Name, out stringValue)))
            {
                return this.GetDefaultValue();
            }

            return this.converter.ConvertFrom(stringValue);
        }

        public override void ResetValue(object component)
        {
            IVsBuildPropertyStorage project;
            uint itemId;
            GetProjectItem(component, out project, out itemId);

            ErrorHandler.ThrowOnFailure(project.SetItemAttribute(itemId, this.Name, null));
        }

        public override void SetValue(object component, object value)
        {
            IVsBuildPropertyStorage project;
            uint itemId;
            GetProjectItem(component, out project, out itemId);

            if (object.Equals(value, this.GetDefaultValue()))
            {
                ErrorHandler.ThrowOnFailure(project.SetItemAttribute(itemId, this.Name, null));                
            }
            else
            {
                string stringValue = this.converter.ConvertToInvariantString(value);
                ErrorHandler.ThrowOnFailure(project.SetItemAttribute(itemId, this.Name, stringValue));                
            }
        }

        /// <summary>
        /// Returns true when property value is different than the default. 
        /// </summary>
        /// <remarks>
        /// This is used by the PropertyGrid in Visual Studio to display values that are actually stored in bold font.
        /// </remarks>
        public override bool ShouldSerializeValue(object component)
        {
            return !object.Equals(this.GetValue(component), this.GetDefaultValue());
        }

        protected override AttributeCollection CreateAttributeCollection()
        {
            if (!string.IsNullOrWhiteSpace(this.description))
            {
                return AttributeCollection.FromExisting(base.CreateAttributeCollection(), new DescriptionAttribute(this.description));
            }

            return base.CreateAttributeCollection();
        }

        private static void GetProjectItem(object component, out IVsBuildPropertyStorage project, out uint itemId)
        {
            if (component == null)
            {
                throw new ArgumentNullException("component");
            }

            var parent = component as CustomToolParameters;
            if (parent == null)
            {
                throw new ArgumentException(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        "Object of type {0} is expected, actual object is of type {1}.",
                        typeof(CustomToolParameters).FullName,
                        component.GetType().FullName),
                    "component");
            }

            parent.GetProjectItem(out project, out itemId);            
        }

        private object GetDefaultValue()
        {
            return this.parameterType.IsValueType && this.parameterType != typeof(void) ? Activator.CreateInstance(this.parameterType) : null;
        }
    }
}