// <copyright file="KnownValuesAttributeTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>
namespace T4Toolbox.TemplateAnalysis
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Xunit;

    public static class KnownValuesAttributeTest
    {
        [Fact]
        public static void ClassIsInternalAndNotMeantForPublicConsumption()
        {
            Assert.False(typeof(KnownValuesAttribute).IsPublic);
        }

        [Fact]
        public static void ClassIsSealedAndNotMeantToHaveDerivedClasses()
        {
            Assert.True(typeof(KnownValuesAttribute).IsSealed);
        }

        [Fact]
        public static void KnownValuesReturnsValuesProducedByTheFunctionSpecifiedInConstructor()
        {
            var attribute = new KnownValuesAttribute(typeof(KnownValuesAttributeTest), "TestValueFactory");
            Assert.Equal(2, attribute.KnownValues.Count);
            Assert.Same("Value1", attribute.KnownValues[0].DisplayName);
            Assert.Same("Value2", attribute.KnownValues[1].DisplayName);
        }

        #region Enum support

        [Fact]
        public static void KnownValuesReturnsListOfValuesDefinedInEnumType()
        {
            var attribute = new KnownValuesAttribute(typeof(EnumWithThreeValues));
            Assert.Equal(3, attribute.KnownValues.Count);
        }

        [Fact]
        public static void KnownValueCachesResultToImprovePerformance()
        {
            var attribute = new KnownValuesAttribute(typeof(EnumWithThreeValues));
            Assert.Same(attribute.KnownValues, attribute.KnownValues);
        }

        [Fact]
        public static void KnownValueDisplayNameReturnsNameOfEnumValueWithoutDisplayAttribute()
        {
            var attribute = new KnownValuesAttribute(typeof(EnumWithoutDisplayAttribute));
            Assert.Equal("Value", attribute.KnownValues[0].DisplayName);
        }

        [Fact]
        public static void KnownValueDisplayNameReturnsValueSpecifiedInDisplayAttribute()
        {
            var attribute = new KnownValuesAttribute(typeof(EnumWithDisplayName));
            Assert.Equal("Display.Name", attribute.KnownValues[0].DisplayName);
        }

        [Fact]
        public static void KnownValueDisplayNameReturnsNameOfEnumValueWhenDisplayAttributeDoesNotSpecifyName()
        {
            var attribute = new KnownValuesAttribute(typeof(EnumWithDescription));
            Assert.Equal("Value", attribute.KnownValues[0].DisplayName);
        }

        [Fact]
        public static void KnownValueDescriptionReturnsEmptyStringWhenEnumValueHasNoDisplayAttribute()
        {
            var attribute = new KnownValuesAttribute(typeof(EnumWithoutDisplayAttribute));
            Assert.Equal(string.Empty, attribute.KnownValues[0].Description);
        }

        [Fact]
        public static void KnownValueDescriptionReturnsDescriptionSpecifiedInDisplayAttribute()
        {
            var attribute = new KnownValuesAttribute(typeof(EnumWithDescription));
            Assert.Equal("Display.Description", attribute.KnownValues[0].Description);
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
