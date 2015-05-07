// <copyright file="CustomToolParameterTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.Tests
{
    using System;
    using System.Globalization;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using T4Toolbox.VisualStudio.Tests.Fakes;

    [TestClass]
    public class CustomToolParameterTest : IDisposable
    {
        private const string ParameterName = "AnswerToLifeUniverseAndEverything";
        private const string ParameterValue = "42";

        private readonly FakeDte dte;
        private readonly FakeSolution solution;
        private readonly FakeProject project;
        private readonly FakeProjectItem projectItem;
        private readonly CustomToolParameters parent;

        public CustomToolParameterTest()
        {
            this.dte = new FakeDte();
            this.solution = new FakeSolution(this.dte);
            this.project = new FakeProject(this.solution);
            this.projectItem = new FakeProjectItem(this.project);
            this.parent = new CustomToolParameters(this.dte, this.project, this.projectItem.Id);
        }

        public void Dispose()
        {
            this.dte.Dispose();
        }

        [TestMethod]
        public void AttributesCollectionContainsDescriptionAttributeWhenDescriptionIsNotEmpty()
        {
            const string ParameterDescription = "Description of Parameter";
            var target = new CustomToolParameter(ParameterName, typeof(string), ParameterDescription);
            var descriptionAttribute = target.Attributes.OfType<System.ComponentModel.DescriptionAttribute>().Single();
            Assert.AreEqual(ParameterDescription, descriptionAttribute.Description);
        }

        [TestMethod]
        public void AttributesCollectionDoesNotContainDescriptionAttributeWhenDescriptionIsEmpty()
        {
            var target = new CustomToolParameter(ParameterName, typeof(string), string.Empty);
            Assert.IsFalse(target.Attributes.OfType<System.ComponentModel.DescriptionAttribute>().Any());
        }

        [TestMethod]
        public void ConstructorInitializesName()
        {
            var target = new CustomToolParameter(ParameterName, typeof(string), string.Empty);
            Assert.AreEqual(ParameterName, target.Name);
        }

        [TestMethod]
        public void ComponentTypeReturnsTypeOfCustomToolParametersClass()
        {
            var target = new CustomToolParameter(ParameterName, typeof(string), string.Empty);
            Assert.AreEqual(typeof(CustomToolParameters), target.ComponentType);
        }

        [TestMethod]
        public void IsReadOnlyReturnsFalse()
        {
            var target = new CustomToolParameter(ParameterName, typeof(string), string.Empty);
            Assert.IsFalse(target.IsReadOnly);            
        }

        [TestMethod]
        public void IsReadOnlyReturnsTrueWhenParameterTypeCannotBeConvertedFromString()
        {
            var target = new CustomToolParameter(ParameterName, typeof(void), string.Empty);
            Assert.IsTrue(target.IsReadOnly);                        
        }

        [TestMethod]
        public void PropertyTypeReturnsParameterTypePassedToConstructor()
        {
            var target = new CustomToolParameter(ParameterName, typeof(int?), string.Empty);
            Assert.AreEqual(typeof(int?), target.PropertyType);
        }

        [TestMethod]
        public void CanResetValueReturnsTrue()
        {
            var target = new CustomToolParameter(ParameterName, typeof(string), string.Empty);
            Assert.IsTrue(target.CanResetValue(this.parent));
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void GetValueThrowsArgumentNullExceptionWhenComponentIsNull()
        {
            var target = new CustomToolParameter(ParameterName, typeof(string), string.Empty);
            target.GetValue(null);
        }

        [TestMethod, ExpectedException(typeof(ArgumentException))]
        public void GetValueThrowsArgumentExceptionWhenComponentIsOfWrongType()
        {
            var target = new CustomToolParameter(ParameterName, typeof(string), string.Empty);
            target.GetValue(new object());
        }

        [TestMethod]
        public void GetValueRetrievesValueFromProjectItemMetadata()
        {
            this.projectItem.Metadata[ParameterName] = ParameterValue;
            var target = new CustomToolParameter(ParameterName, typeof(string), string.Empty);
            Assert.AreEqual(ParameterValue, target.GetValue(this.parent));
        }

        [TestMethod]
        public void GetValueReturnsDefaultReferenceTypeValueWhenProjectItemMetadataDoesNotExist()
        {
            var target = new CustomToolParameter(ParameterName, typeof(string), string.Empty);
            Assert.IsNull(target.GetValue(this.parent));
        }

        [TestMethod]
        public void GetValueReturnsDefaultValueTypeValueWhenProjectItemMetadataDoesNotExist()
        {
            var target = new CustomToolParameter(ParameterName, typeof(int), string.Empty);
            Assert.AreEqual(default(int), target.GetValue(this.parent));            
        }

        [TestMethod]
        public void GetValueReturnsNullWhenParameterTypeIsNotSpecified()
        {
            var target = new CustomToolParameter(ParameterName, typeof(void), string.Empty);
            Assert.IsNull(target.GetValue(this.parent));
        }

        [TestMethod]
        public void GetValueConvertsProjectItemMetadataToPropertyType()
        {
            this.projectItem.Metadata[ParameterName] = ParameterValue;
            var target = new CustomToolParameter(ParameterName, typeof(int), string.Empty);
            Assert.AreEqual(int.Parse(ParameterValue, CultureInfo.InvariantCulture), target.GetValue(this.parent));                        
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void ResetValueThrowsArgumentNullExceptionWhenComponentIsNull()
        {
            var target = new CustomToolParameter(ParameterName, typeof(string), string.Empty);
            target.ResetValue(null);
        }

        [TestMethod, ExpectedException(typeof(ArgumentException))]
        public void ResetValueThrowsArgumentExceptionWhenComponentIsOfWrongType()
        {
            var target = new CustomToolParameter(ParameterName, typeof(string), string.Empty);
            target.ResetValue(new object());
        }

        [TestMethod]
        public void ResetValueRemovesProjectItemMetadata()
        {
            this.projectItem.Metadata[ParameterName] = ParameterValue;
            var target = new CustomToolParameter(ParameterName, typeof(int), string.Empty);
            target.ResetValue(this.parent);
            Assert.IsFalse(this.projectItem.Metadata.ContainsKey(ParameterName));
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void SetValueThrowsArgumentNullExceptionWhenComponentIsNull()
        {
            var target = new CustomToolParameter(ParameterName, typeof(string), string.Empty);
            target.SetValue(null, "42");
        }

        [TestMethod, ExpectedException(typeof(ArgumentException))]
        public void SetValueThrowsArgumentExceptionWhenComponentIsOfWrongType()
        {
            var target = new CustomToolParameter(ParameterName, typeof(string), string.Empty);
            target.SetValue(new object(), "42");
        }

        [TestMethod]
        public void SetValueStoresValueInProjectItemMetadata()
        {
            var target = new CustomToolParameter(ParameterName, typeof(string), string.Empty);
            target.SetValue(this.parent, ParameterValue);
            Assert.AreEqual(ParameterValue, this.projectItem.Metadata[ParameterName]);            
        }

        [TestMethod]
        public void SetValueConvertsParameterValueToString()
        {
            var target = new CustomToolParameter(ParameterName, typeof(int), string.Empty);
            target.SetValue(this.parent, int.Parse(ParameterValue, CultureInfo.InvariantCulture));
            Assert.AreEqual(ParameterValue, this.projectItem.Metadata[ParameterName]);                        
        }

        [TestMethod]
        public void SetValueRemovesProjectItemMetadataWhenValueIsSameAsDefault()
        {
            this.projectItem.Metadata[ParameterName] = ParameterValue;
            var target = new CustomToolParameter(ParameterName, typeof(int), string.Empty);
            target.SetValue(this.parent, default(int));
            Assert.IsFalse(this.projectItem.Metadata.ContainsKey(ParameterName));
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void ShouldSerializeValueThrowsArgumentNullExceptionWhenComponentIsNull()
        {
            var target = new CustomToolParameter(ParameterName, typeof(string), string.Empty);
            target.ShouldSerializeValue(null);
        }

        [TestMethod, ExpectedException(typeof(ArgumentException))]
        public void ShouldSerializeValueThrowsArgumentExceptionWhenComponentIsOfWrongType()
        {
            var target = new CustomToolParameter(ParameterName, typeof(string), string.Empty);
            target.ShouldSerializeValue(new object());
        }

        [TestMethod]
        public void ShouldSerializeValueReturnsTrueWhenParameterValueIsDifferentFromDefault()
        {
            var target = new CustomToolParameter(ParameterName, typeof(int), string.Empty);
            target.SetValue(this.parent, int.Parse(ParameterValue, CultureInfo.InvariantCulture));
            Assert.IsTrue(target.ShouldSerializeValue(this.parent));            
        }

        [TestMethod]
        public void ShouldSerializeValueReturnsFalseWhenParameterValueIsSameAsDefault()
        {
            var target = new CustomToolParameter(ParameterName, typeof(int), string.Empty);
            target.SetValue(this.parent, default(int));
            Assert.IsFalse(target.ShouldSerializeValue(this.parent));
        }
    }
}