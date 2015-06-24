// <copyright file="ValueDescriptorTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.TemplateAnalysis
{
    using Xunit;

    public static class ValueDescriptorTest
    {
        [Fact]
        public static void ClassIsInternalAndNotMeantForPublicConsumption()
        {
            Assert.False(typeof(ValueDescriptor).IsPublic);
        }

        [Fact]
        public static void ClassIsSealedAndNotMeantToHaveDerivedClasses()
        {
            Assert.True(typeof(ValueDescriptor).IsSealed);
        }

        [Fact]
        public static void DisplayNameReturnsValueSpecifiedInConstructor()
        {
            var value = new ValueDescriptor("DisplayName");
            Assert.Equal("DisplayName", value.DisplayName);
        }

        [Fact]
        public static void DescriptionIsEmptyStringByDefault()
        {
            var value = new ValueDescriptor("DisplayName");
            Assert.Empty(value.Description);
        }

        [Fact]
        public static void DescriptionReturnsValueSpecifiedInConstructor()
        {
            var value = new ValueDescriptor("DisplayName", "Description");
            Assert.Equal("Description", value.Description);
        }
    }
}
