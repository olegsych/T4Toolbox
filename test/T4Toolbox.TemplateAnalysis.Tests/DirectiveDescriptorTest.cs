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
    using Xunit;

    public static class DirectiveDescriptorTest
    {
        [Fact]
        public static void DirectiveDescriptorIsInternalAndNotMeantForPublicConsumption()
        {
            Assert.False(typeof(DirectiveDescriptor).IsPublic);
        }

        [Fact]
        public static void DirectiveDescriptorIsSealedAndNotMeantToBeExtended()
        {
            Assert.True(typeof(DirectiveDescriptor).IsSealed);
        }

        [Fact]
        public static void ConstructorIsPrivateBecauseDirectiveDescriptorsAreMeantToBeAccessedOnlyThroughStaticMethodsOfTheClass()
        {
            ConstructorInfo[] constructors = typeof(DirectiveDescriptor).GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.NotEqual(0, constructors.Length);
            Assert.True(constructors.All(c => c.IsPrivate));                
        }

        [Fact]
        public static void AttributesReturnsReadOnlyDictionaryOfAttributeDescriptorsRepresentingAttributesOfDirective()
        {
            DirectiveDescriptor directive = CreateDirectiveDescriptor(typeof(AssemblyDirective));
            IReadOnlyDictionary<string, AttributeDescriptor> attributes = directive.Attributes;
            Assert.Equal(1, attributes.Count);
        }

        [Fact]
        public static void AttributesDictionaryIsCaseInsensitive()
        {
            DirectiveDescriptor directive = CreateDirectiveDescriptor(typeof(AssemblyDirective));
            Assert.True(directive.Attributes.ContainsKey("NAME"));
            Assert.True(directive.Attributes.ContainsKey("name"));
        }

        [Fact]
        public static void AttributesReturnsTheSameDictionaryWhenCalledMultipleTimesToAvoidRebuildingItUnnecessarily()
        {
            DirectiveDescriptor directive = CreateDirectiveDescriptor(typeof(AssemblyDirective));
            Assert.Same(directive.Attributes, directive.Attributes);
        }

        [Fact]
        public static void DescriptionReturnsValueSpecifiedInDescriptionAttributeAppliedToDirectiveClass()
        {
            DirectiveDescriptor descriptor = CreateDirectiveDescriptor(typeof(AssemblyDirective));
            var attribute = TypeDescriptor.GetAttributes(typeof(AssemblyDirective)).OfType<DescriptionAttribute>().Single();
            Assert.Equal(attribute.Description, descriptor.Description);
        }

        [Fact]
        public static void DisplayNameReturnsValueSpecifiedInDisplayNameAttributeAppliedToDirectiveClass()
        {
            DirectiveDescriptor descriptor = CreateDirectiveDescriptor(typeof(AssemblyDirective));
            var attribute = TypeDescriptor.GetAttributes(typeof(AssemblyDirective)).OfType<DisplayNameAttribute>().Single();
            Assert.Equal(attribute.DisplayName, descriptor.DisplayName);
        }

        [Fact]
        public static void GetBuiltInDirectivesReturnsReadOnlyListOfBuiltInDirectiveDescriptors()
        {
            IEnumerable<DirectiveDescriptor> builtInDirectives = DirectiveDescriptor.GetBuiltInDirectives();
            Assert.Contains(builtInDirectives, d => d.DisplayName == "assembly");
            Assert.Contains(builtInDirectives, d => d.DisplayName == "import");
            Assert.Contains(builtInDirectives, d => d.DisplayName == "include");
            Assert.Contains(builtInDirectives, d => d.DisplayName == "output");
            Assert.Contains(builtInDirectives, d => d.DisplayName == "parameter");
            Assert.Contains(builtInDirectives, d => d.DisplayName == "template");
            Assert.Contains(builtInDirectives, d => d.DisplayName.Length == 0);
            Assert.Equal(7, builtInDirectives.Count());
        }

        [Fact]
        public static void GetBuiltInDirectivesReturnsTheSameListWhenCalledMultipleTimesToAvoidRebuildingItUnnecessarily()
        {
            IEnumerable<DirectiveDescriptor> list1 = DirectiveDescriptor.GetBuiltInDirectives();
            IEnumerable<DirectiveDescriptor> list2 = DirectiveDescriptor.GetBuiltInDirectives();
            Assert.Same(list1, list2);
        }

        [Fact]
        public static void GetDirectiveDescriptorReturnsDescriptorOfGivenType()
        {
            DirectiveDescriptor descriptor = DirectiveDescriptor.GetDirectiveDescriptor(typeof(AssemblyDirective));
            Assert.Equal("assembly", descriptor.DisplayName);
        }

        [Fact]
        public static void GetDirectiveDescriptorCachesDescriptorsToImprovePerformance()
        {
            DirectiveDescriptor descriptor1 = DirectiveDescriptor.GetDirectiveDescriptor(typeof(AssemblyDirective));
            DirectiveDescriptor descriptor2 = DirectiveDescriptor.GetDirectiveDescriptor(typeof(AssemblyDirective));
            Assert.Same(descriptor1, descriptor2);
        }

        private static DirectiveDescriptor CreateDirectiveDescriptor(Type directiveType)
        {
            return (DirectiveDescriptor)Activator.CreateInstance(typeof(DirectiveDescriptor), BindingFlags.NonPublic | BindingFlags.Instance, null, new object[] { directiveType }, null);
        }
    }
}