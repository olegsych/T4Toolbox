// <copyright file="AttributeDescriptorTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.TemplateAnalysis
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using Xunit;

    public static class AttributeDescriptorTest
    {
        [Fact]
        public static void AttributeDescriptorIsInternalAndNotMeantForPublicConsumption()
        {
            Assert.False(typeof(AttributeDescriptor).IsPublic);
        }

        [Fact]
        public static void AttributeDescriptorIsSealedAndNotMeantToHaveChildClasses()
        {
            Assert.True(typeof(AttributeDescriptor).IsSealed);
        }

        [Fact]
        public static void DescriptionReturnsValueSpecifiedInDescriptionAttributeAppliedToAttributeProperty()
        {
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(TemplateDirective))["Language"];
            var attribute = (DescriptionAttribute)property.Attributes[typeof(DescriptionAttribute)];
            var descriptor = new AttributeDescriptor(property);
            Assert.Equal(attribute.Description, descriptor.Description);
        }

        [Fact]
        public static void DisplayNameReturnsNameOfAttributeProperty()
        {
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(TemplateDirective))["Language"];
            var descriptor = new AttributeDescriptor(property);
            Assert.Equal("Language", descriptor.DisplayName);
        }

        [Fact]
        public static void DisplayNameReturnsValueSpecifiedInDisplayNameAttributeAppliedToAttributeProperty()
        {
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(AssemblyDirective))["AssemblyName"];
            var descriptor = new AttributeDescriptor(property);
            Assert.Equal("Name", descriptor.DisplayName);
        }

        [Fact]
        public static void ValuesReturnsEmptyDictionaryForPropertyWithoutWellKnownValues()
        {
            var attribute = new AttributeDescriptor(TypeDescriptor.GetProperties(typeof(TestDirective))["PropertyWithoutKnownValues"]);        
            Assert.Equal(0, attribute.Values.Count);
        }

        [Fact]
        public static void ValuesReturnsDictionaryOfValuesForPropertyWithKnownValuesAttribute()
        {
            var attribute = new AttributeDescriptor(TypeDescriptor.GetProperties(typeof(TestDirective))["PropertyWithKnownValues"]);
            IReadOnlyDictionary<string, ValueDescriptor> values = attribute.Values;
            Assert.Equal(3, values.Count);
        }

        [Fact]
        public static void ValueCachesResultToImprovePerformance()
        {
            var attribute = new AttributeDescriptor(TypeDescriptor.GetProperties(typeof(TestDirective))["PropertyWithKnownValues"]);
            Assert.Same(attribute.Values, attribute.Values);
        }

        private enum TestEnum
        {
            Value1,
            Value2,
            Value3
        }

        private class TestDirective
        {
            [KnownValues(typeof(TestEnum))]
            public string PropertyWithKnownValues { get; set; }
            
            public string PropertyWithoutKnownValues { get; set; }
        }
    }
}