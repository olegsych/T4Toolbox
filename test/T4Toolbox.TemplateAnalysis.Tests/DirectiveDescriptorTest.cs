// <copyright file="DirectiveDescriptorTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.TemplateAnalysis
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using DescriptionAttribute = System.ComponentModel.DescriptionAttribute;

    [TestClass]
    public class DirectiveDescriptorTest
    {
        [TestMethod]
        public void DirectiveDescriptorIsInternalAndNotMeantForPublicConsumption()
        {
            Assert.IsFalse(typeof(DirectiveDescriptor).IsPublic);
        }

        [TestMethod]
        public void DirectiveDescriptorIsSealedAndNotMeantToBeExtended()
        {
            Assert.IsTrue(typeof(DirectiveDescriptor).IsSealed);
        }

        [TestMethod]
        public void ConstructorIsPrivateBecauseDirectiveDescriptorsAreMeantToBeAccessedOnlyThroughStaticMethodsOfTheClass()
        {
            ConstructorInfo[] constructors = typeof(DirectiveDescriptor).GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.AreNotEqual(0, constructors.Length);
            Assert.IsTrue(constructors.All(c => c.IsPrivate));                
        }

        [TestMethod]
        public void AttributesReturnsReadOnlyDictionaryOfAttributeDescriptorsRepresentingAttributesOfDirective()
        {
            DirectiveDescriptor directive = CreateDirectiveDescriptor(typeof(AssemblyDirective));
            IReadOnlyDictionary<string, AttributeDescriptor> attributes = directive.Attributes;
            Assert.AreEqual(1, attributes.Count);
        }

        [TestMethod]
        public void AttributesDictionaryIsCaseInsensitive()
        {
            DirectiveDescriptor directive = CreateDirectiveDescriptor(typeof(AssemblyDirective));
            Assert.IsTrue(directive.Attributes.ContainsKey("NAME"));
            Assert.IsTrue(directive.Attributes.ContainsKey("name"));
        }

        [TestMethod]
        public void AttributesReturnsTheSameDictionaryWhenCalledMultipleTimesToAvoidRebuildingItUnnecessarily()
        {
            DirectiveDescriptor directive = CreateDirectiveDescriptor(typeof(AssemblyDirective));
            Assert.AreSame(directive.Attributes, directive.Attributes);
        }

        [TestMethod]
        public void DescriptionReturnsValueSpecifiedInDescriptionAttributeAppliedToDirectiveClass()
        {
            DirectiveDescriptor descriptor = CreateDirectiveDescriptor(typeof(AssemblyDirective));
            var attribute = TypeDescriptor.GetAttributes(typeof(AssemblyDirective)).OfType<DescriptionAttribute>().Single();
            Assert.AreEqual(attribute.Description, descriptor.Description);
        }

        [TestMethod]
        public void DisplayNameReturnsValueSpecifiedInDisplayNameAttributeAppliedToDirectiveClass()
        {
            DirectiveDescriptor descriptor = CreateDirectiveDescriptor(typeof(AssemblyDirective));
            var attribute = TypeDescriptor.GetAttributes(typeof(AssemblyDirective)).OfType<DisplayNameAttribute>().Single();
            Assert.AreEqual(attribute.DisplayName, descriptor.DisplayName);
        }

        [TestMethod]
        public void GetBuiltInDirectivesReturnsReadOnlyListOfBuiltInDirectiveDescriptors()
        {
            IEnumerable<DirectiveDescriptor> builtInDirectives = DirectiveDescriptor.GetBuiltInDirectives();
            Assert.AreEqual(1, builtInDirectives.Count(d => d.DisplayName == "assembly"),  "assembly");
            Assert.AreEqual(1, builtInDirectives.Count(d => d.DisplayName == "import"),    "import");
            Assert.AreEqual(1, builtInDirectives.Count(d => d.DisplayName == "include"),   "include");
            Assert.AreEqual(1, builtInDirectives.Count(d => d.DisplayName == "output"),    "output");
            Assert.AreEqual(1, builtInDirectives.Count(d => d.DisplayName == "parameter"), "parameter");
            Assert.AreEqual(1, builtInDirectives.Count(d => d.DisplayName == "template"),  "template");
            Assert.AreEqual(1, builtInDirectives.Count(d => d.DisplayName.Length == 0),    "custom");
            Assert.AreEqual(7, builtInDirectives.Count());
        }

        [TestMethod]
        public void GetBuiltInDirectivesReturnsTheSameListWhenCalledMultipleTimesToAvoidRebuildingItUnnecessarily()
        {
            IEnumerable<DirectiveDescriptor> list1 = DirectiveDescriptor.GetBuiltInDirectives();
            IEnumerable<DirectiveDescriptor> list2 = DirectiveDescriptor.GetBuiltInDirectives();
            Assert.AreSame(list1, list2);
        }

        [TestMethod]
        public void GetDirectiveDescriptorReturnsDescriptorOfGivenType()
        {
            DirectiveDescriptor descriptor = DirectiveDescriptor.GetDirectiveDescriptor(typeof(AssemblyDirective));
            Assert.AreEqual("assembly", descriptor.DisplayName);
        }

        [TestMethod]
        public void GetDirectiveDescriptorCachesDescriptorsToImprovePerformance()
        {
            DirectiveDescriptor descriptor1 = DirectiveDescriptor.GetDirectiveDescriptor(typeof(AssemblyDirective));
            DirectiveDescriptor descriptor2 = DirectiveDescriptor.GetDirectiveDescriptor(typeof(AssemblyDirective));
            Assert.AreSame(descriptor1, descriptor2);
        }

        private static DirectiveDescriptor CreateDirectiveDescriptor(Type directiveType)
        {
            return (DirectiveDescriptor)Activator.CreateInstance(typeof(DirectiveDescriptor), BindingFlags.NonPublic | BindingFlags.Instance, null, new object[] { directiveType }, null);
        }
    }
}