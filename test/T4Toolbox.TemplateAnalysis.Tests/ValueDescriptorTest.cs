// <copyright file="ValueDescriptorTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.TemplateAnalysis
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ValueDescriptorTest
    {
        [TestMethod]
        public void ClassIsInternalAndNotMeantForPublicConsumption()
        {
            Assert.IsFalse(typeof(ValueDescriptor).IsPublic);
        }

        [TestMethod]
        public void ClassIsSealedAndNotMeantToHaveDerivedClasses()
        {
            Assert.IsTrue(typeof(ValueDescriptor).IsSealed);
        }

        [TestMethod]
        public void DisplayNameReturnsValueSpecifiedInConstructor()
        {
            var value = new ValueDescriptor("DisplayName");
            Assert.AreEqual("DisplayName", value.DisplayName);
        }

        [TestMethod]
        public void DescriptionIsEmptyStringByDefault()
        {
            var value = new ValueDescriptor("DisplayName");
            Assert.IsNotNull(value.Description);
            Assert.AreEqual("DisplayName", value.DisplayName);
        }

        [TestMethod]
        public void DescriptionReturnsValueSpecifiedInConstructor()
        {
            var value = new ValueDescriptor("DisplayName", "Description");
            Assert.AreEqual("Description", value.Description);
        }
    }
}
