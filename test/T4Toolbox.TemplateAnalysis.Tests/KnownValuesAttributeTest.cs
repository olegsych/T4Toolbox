// <copyright file="KnownValuesAttributeTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>
namespace T4Toolbox.TemplateAnalysis
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class KnownValuesAttributeTest
    {
        [TestMethod]
        public void ClassIsInternalAndNotMeantForPublicConsumption()
        {
            Assert.IsFalse(typeof(KnownValuesAttribute).IsPublic);
        }

        [TestMethod]
        public void ClassIsSealedAndNotMeantToHaveDerivedClasses()
        {
            Assert.IsTrue(typeof(KnownValuesAttribute).IsSealed);
        }

        [TestMethod]
        public void KnownValuesReturnsValuesProducedByTheFunctionSpecifiedInConstructor()
        {
            var attribute = new KnownValuesAttribute(this.GetType(), "TestValueFactory");
            Assert.AreEqual(2, attribute.KnownValues.Count);
            Assert.AreSame("Value1", attribute.KnownValues[0].DisplayName);
            Assert.AreSame("Value2", attribute.KnownValues[1].DisplayName);
        }

        #region Enum support

        [TestMethod]
        public void KnownValuesReturnsListOfValuesDefinedInEnumType()
        {
            var attribute = new KnownValuesAttribute(typeof(EnumWithThreeValues));
            Assert.AreEqual(3, attribute.KnownValues.Count);
        }

        [TestMethod]
        public void KnownValueCachesResultToImprovePerformance()
        {
            var attribute = new KnownValuesAttribute(typeof(EnumWithThreeValues));
            Assert.AreSame(attribute.KnownValues, attribute.KnownValues);
        }

        [TestMethod]
        public void KnownValueDisplayNameReturnsNameOfEnumValueWithoutDisplayAttribute()
        {
            var attribute = new KnownValuesAttribute(typeof(EnumWithoutDisplayAttribute));
            Assert.AreEqual("Value", attribute.KnownValues[0].DisplayName);
        }

        [TestMethod]
        public void KnownValueDisplayNameReturnsValueSpecifiedInDisplayAttribute()
        {
            var attribute = new KnownValuesAttribute(typeof(EnumWithDisplayName));
            Assert.AreEqual("Display.Name", attribute.KnownValues[0].DisplayName);
        }

        [TestMethod]
        public void KnownValueDisplayNameReturnsNameOfEnumValueWhenDisplayAttributeDoesNotSpecifyName()
        {
            var attribute = new KnownValuesAttribute(typeof(EnumWithDescription));
            Assert.AreEqual("Value", attribute.KnownValues[0].DisplayName);
        }

        [TestMethod]
        public void KnownValueDescriptionReturnsEmptyStringWhenEnumValueHasNoDisplayAttribute()
        {
            var attribute = new KnownValuesAttribute(typeof(EnumWithoutDisplayAttribute));
            Assert.AreEqual(string.Empty, attribute.KnownValues[0].Description);
        }

        [TestMethod]
        public void KnownValueDescriptionReturnsDescriptionSpecifiedInDisplayAttribute()
        {
            var attribute = new KnownValuesAttribute(typeof(EnumWithDescription));
            Assert.AreEqual("Display.Description", attribute.KnownValues[0].Description);
        }

        private enum EnumWithThreeValues
        {
            Value1,
            Value2,
            Value3
        }

        private enum EnumWithoutDisplayAttribute
        {
            Value
        }

        private enum EnumWithDisplayName
        {
            [Display(Name = "Display.Name")]
            Value
        }

        private enum EnumWithDescription
        {
            [Display(Description = "Display.Description")]
            Value
        }

        #endregion

        private static IEnumerable<ValueDescriptor> TestValueFactory()
        {
            yield return new ValueDescriptor("Value1");
            yield return new ValueDescriptor("Value2");
        }
    }
}
