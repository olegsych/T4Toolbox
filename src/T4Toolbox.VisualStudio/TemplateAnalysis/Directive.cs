// <copyright file="Directive.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.TemplateAnalysis
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using Microsoft.VisualStudio.Text;

    internal abstract class Directive : NonterminalNode
    {
        private static readonly ConcurrentDictionary<Type, IReadOnlyDictionary<string, PropertyDescriptor>> PropertyCache = new ConcurrentDictionary<Type, IReadOnlyDictionary<string, PropertyDescriptor>>();

        private readonly DirectiveBlockStart start;
        private readonly DirectiveName name;
        private readonly IReadOnlyDictionary<string, Attribute> attributes;
        private readonly BlockEnd end;

        protected Directive(DirectiveBlockStart start, DirectiveName name, IEnumerable<Attribute> attributes, BlockEnd end)
        {
            Debug.Assert(start != null, "start");
            Debug.Assert(name != null, "name");
            Debug.Assert(attributes != null, "attributes");
            Debug.Assert(end != null, "end");

            this.start = start;
            this.name = name;
            this.attributes = attributes.ToDictionary(a => a.Name, a => a, StringComparer.OrdinalIgnoreCase);
            this.end = end;
        }

        public sealed override SyntaxKind Kind
        {
            get { return SyntaxKind.Directive; }
        }

        public string DirectiveName
        {
            get { return this.name.Text; }
        }

        public IReadOnlyDictionary<string, Attribute> Attributes
        {
            get { return this.attributes; }
        }

        public override Position Position
        {
            get { return this.start.Position; }
        }

        public override Span Span
        {
            get { return Span.FromBounds(this.start.Span.Start, this.end.Span.End); }
        }

        public static Directive Create(DirectiveBlockStart start, DirectiveName name, IEnumerable<Attribute> attributes, BlockEnd end)
        {
            switch (name.Text.ToUpperInvariant())
            {
                case "ASSEMBLY": return new AssemblyDirective(start, name, attributes, end);
                case "IMPORT": return new ImportDirective(start, name, attributes, end);
                case "INCLUDE": return new IncludeDirective(start, name, attributes, end);                
                case "OUTPUT": return new OutputDirective(start, name, attributes, end);
                case "PARAMETER": return new ParameterDirective(start, name, attributes, end);
                case "TEMPLATE": return new TemplateDirective(start, name, attributes, end);
                default: return new CustomDirective(start, name, attributes, end);
            }
        }

        public override IEnumerable<SyntaxNode> ChildNodes()
        {
            yield return this.start;
            yield return this.name;
            foreach (Attribute attribute in this.attributes.Values)
            {
                yield return attribute;
            }

            yield return this.end;
        }

        /// <summary>
        /// Extends the default behavior to return value of the <see cref="DescriptionAttribute"/> applied to a
        /// directive property when its <see cref="Attribute"/> contains the specified <paramref name="position"/>.
        /// </summary>
        public override bool TryGetDescription(int position, out string description, out Span applicableTo)
        {
            // Block start returns its own description
            if (this.start.TryGetDescription(position, out description, out applicableTo))
            {
                return true;
            }

            // Name returns description of the parent directive
            if (this.name.Span.Contains(position) && base.TryGetDescription(position, out description, out applicableTo))
            {
                applicableTo = this.name.Span;
                return true;
            }

            // Attributes return description of their respective properties
            IReadOnlyDictionary<string, PropertyDescriptor> properties = this.GetProperties();
            foreach (Attribute attribute in this.Attributes.Values)
            {
                PropertyDescriptor property;
                if (attribute.Span.Contains(position) && properties.TryGetValue(attribute.Name, out property))
                {
                    description = property.Description;
                    if (!string.IsNullOrEmpty(description))
                    {
                        applicableTo = attribute.Span;
                        return true;
                    }
                }
            }

            // Block end returns its own description
            if (this.end.TryGetDescription(position, out description, out applicableTo))
            {
                return true;
            }
            
            // No description for gaps between children to prevent directive's tooltip from sticking during horizontal mouse movement and blocking childrens' tooltips.
            description = string.Empty;
            applicableTo = default(Span);
            return false;
        }

        public override IEnumerable<TemplateError> Validate()
        {
            var context = new ValidationContext(this);
            var results = new List<ValidationResult>();
            Validator.TryValidateObject(this, context, results, validateAllProperties: true);
            foreach (ValidationResult result in results)
            {
                yield return this.CreateTemplateError(result);
            }

            foreach (TemplateError error in this.ValidateAttributes())
            {
                yield return error;
            }
        }

        protected virtual IEnumerable<TemplateError> ValidateAttributes()
        {
            DirectiveDescriptor directiveDescriptor = DirectiveDescriptor.GetDirectiveDescriptor(this.GetType());
            foreach (Attribute attribute in this.Attributes.Values)
            {
                AttributeDescriptor attributeDescriptor;
                if (directiveDescriptor.Attributes.TryGetValue(attribute.Name, out attributeDescriptor))
                {
                    if (attributeDescriptor.Values.Count > 0 && !attributeDescriptor.Values.ContainsKey(attribute.Value))
                    {
                        AttributeValue attributeValue = attribute.ChildNodes().OfType<AttributeValue>().First();
                        yield return new TemplateError(
                            string.Format("Unexpected {0} attribute value: {1}", attribute.Name, attribute.Value), 
                            attributeValue.Span, 
                            attributeValue.Position);
                    }
                }
                else 
                {
                    yield return new TemplateError("Unexpected attribute: " + attribute.Name, attribute.Span, attribute.Position);
                }
            }            
        }

        protected string GetAttributeValue([CallerMemberName]string propertyName = null)
        {
            Attribute attribute;
            if (this.Attributes.TryGetValue(propertyName, out attribute))
            {
                return attribute.Value;
            }

            PropertyDescriptor property = this.GetProperties()[propertyName];
            if (this.Attributes.TryGetValue(property.DisplayName, out attribute))
            {
                return attribute.Value;
            }

            return string.Empty;
        }

        private static IReadOnlyDictionary<string, PropertyDescriptor> GetProperties(Type directiveType)
        {
            var properties = new Dictionary<string, PropertyDescriptor>(StringComparer.OrdinalIgnoreCase);
            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(directiveType))
            {
                properties[property.Name] = property;
                properties[property.DisplayName] = property;
            }

            return properties;
        }

        private IReadOnlyDictionary<string, PropertyDescriptor> GetProperties()
        {
            return PropertyCache.GetOrAdd(this.GetType(), GetProperties);
        }

        private TemplateError CreateTemplateError(ValidationResult result)
        {
            Span errorSpan = this.Span;
            Position errorPosition = this.Position;

            foreach (string memberName in result.MemberNames)
            {
                Attribute attribute;
                if (this.Attributes.TryGetValue(memberName, out attribute))
                {
                    errorSpan = attribute.Span;
                    errorPosition = attribute.Position;
                }
            }

            return new TemplateError(result.ErrorMessage, errorSpan, errorPosition);
        }
    }
}