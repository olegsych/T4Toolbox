// <copyright file="KnownValuesAttribute.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.TemplateAnalysis
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    [AttributeUsage(AttributeTargets.Property)]
    internal sealed class KnownValuesAttribute : System.Attribute
    {
        private readonly Func<IEnumerable<ValueDescriptor>> getKnownValues;
        private IReadOnlyList<ValueDescriptor> knownValues;

        public KnownValuesAttribute(Type enumType)
        {
            this.getKnownValues = () => GetKnownValues(enumType);
        }

        public KnownValuesAttribute(Type factoryType, string methodName)
        {
            Debug.Assert(factoryType != null, "factoryType");
            Debug.Assert(methodName != null, "methodName");

            MethodInfo factoryMethod = factoryType.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Static);

            Debug.Assert(factoryMethod != null, "factoryMethod");

            this.getKnownValues = Expression
                .Lambda<Func<IEnumerable<ValueDescriptor>>>(Expression.Call(factoryMethod))
                .Compile();
        }

        public IReadOnlyList<ValueDescriptor> KnownValues
        {
            get { return this.knownValues ?? (this.knownValues = this.getKnownValues().ToList()); }
        }

        private static ValueDescriptor CreateValueDescriptor(FieldInfo enumValue)
        {
            Debug.Assert(enumValue != null, "enumValue");

            string displayName = null;
            string description = null;

            var displayAttribute = enumValue.GetCustomAttribute<DisplayAttribute>();
            if (displayAttribute != null)
            {
                displayName = displayAttribute.Name;
                description = displayAttribute.Description;
            }

            displayName = displayName ?? enumValue.Name;

            return new ValueDescriptor(displayName, description);
        }

        private static IEnumerable<ValueDescriptor> GetKnownValues(Type enumType)
        {
            Debug.Assert(enumType != null, "null");
            Debug.Assert(enumType.IsEnum, "IsEnum");
            return enumType
                .GetFields(BindingFlags.Static | BindingFlags.GetField | BindingFlags.Public)
                .Select(CreateValueDescriptor);
        }
    }
}
