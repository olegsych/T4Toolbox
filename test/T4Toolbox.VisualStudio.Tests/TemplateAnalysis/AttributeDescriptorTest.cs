// <copyright file="AttributeDescriptorTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.TemplateAnalysis
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using DescriptionAttribute = System.ComponentModel.DescriptionAttribute;

    [TestClass]
    public class AttributeDescriptorTest
    {
        [TestMethod]
        public void AttributeDescriptorIsInternalAndNotMeantForPublicConsumption()
        {
            Assert.IsFalse(typeof(AttributeDescriptor).IsPublic);
        }

        [TestMethod]
        public void AttributeDescriptorIsSealedAndNotMeantToHaveChildClasses()
        {
            Assert.IsTrue(typeof(AttributeDescriptor).IsSealed);
        }

        [TestMethod]
        public void DescriptionReturnsValueSpecifiedInDescriptionAttributeAppliedToAttributeProperty()
        {
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(TemplateDirective))["Language"];
            var attribute = (DescriptionAttribute)property.Attributes[typeof(DescriptionAttribute)];
            var descriptor = new AttributeDescriptor(property);
            Assert.AreEqual(attribute.Description, descriptor.Description);
        }

        [TestMethod]
        public void DisplayNameReturnsNameOfAttributeProperty()
        {
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(TemplateDirective))["Language"];
            var descriptor = new AttributeDescriptor(property);
            Assert.AreEqual("Language", descriptor.DisplayName);
        }

        [TestMethod]
        public void DisplayNameReturnsValueSpecifiedInDisplayNameAttributeAppliedToAttributeProperty()
        {
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(AssemblyDirective))["AssemblyName"];
            var descriptor = new AttributeDescriptor(property);
            Assert.AreEqual("Name", descriptor.DisplayName);
        }

        [TestMethod]
        public void ValuesReturnsEmptyDictionaryForPropertyWithoutWellKnownValues()
        {
            var attribute = new AttributeDescriptor(TypeDescriptor.GetProperties(typeof(TestDirective))["PropertyWithoutKnownValues"]);        
            Assert.AreEqual(0, attribute.Values.Count);
        }

        [TestMethod]
        public void ValuesReturnsDictionaryOfValuesForPropertyWithKnownValuesAttribute()
        {
            var attribute = new AttributeDescriptor(TypeDescriptor.GetProperties(typeof(TestDirective))["PropertyWithKnownValues"]);
            IReadOnlyDictionary<string, ValueDescriptor> values = attribute.Values;
            Assert.AreEqual(3, values.Count);
        }

        [TestMethod]
        public void ValueCachesResultToImprovePerformance()
        {
            var attribute = new AttributeDescriptor(TypeDescriptor.GetProperties(typeof(TestDirective))["PropertyWithKnownValues"]);
            Assert.AreSame(attribute.Values, attribute.Values);
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