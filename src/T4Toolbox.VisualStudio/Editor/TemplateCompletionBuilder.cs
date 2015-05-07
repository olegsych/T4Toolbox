// <copyright file="TemplateCompletionBuilder.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.Editor
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.VisualStudio.Language.Intellisense;
    using T4Toolbox.VisualStudio.TemplateAnalysis;
    using Attribute = T4Toolbox.VisualStudio.TemplateAnalysis.Attribute;

    /// <summary>
    /// Worker for calculating a set of <see cref="Completion"/> applicable to a syntax <see cref="Node"/>
    /// at the specified position.
    /// </summary>
    internal sealed class TemplateCompletionBuilder : SyntaxNodeVisitor
    {
        private readonly int position;
        private Attribute currentAttribute;
        private Directive currentDirective;

        public TemplateCompletionBuilder(int position)
        {
            Debug.Assert(position >= 0, "position");
            this.position = position;
        }

        /// <summary>
        /// Gets the list of <see cref="Completion"/> objects applicable to the <see cref="Node"/>. 
        /// Returns null if position was not visited or if node does not support completions.
        /// </summary>
        public IReadOnlyList<Completion> Completions { get; private set; }

        /// <summary>
        /// Gets the <see cref="SyntaxNode"/> at position specified in the constructor. 
        /// Returns null if position was not visited.
        /// </summary>
        public SyntaxNode Node { get; private set; }

        protected internal override void VisitAttribute(Attribute node)
        {
            this.currentAttribute = node;
            base.VisitAttribute(node);
        }
        
        protected internal override void VisitAttributeName(AttributeName node)
        {
            base.VisitAttributeName(node);

            if (node.Span.Start <= this.position && this.position <= node.Span.End)
            {
                Debug.Assert(this.currentDirective != null, "currentDirective");
                var directiveDescriptor = DirectiveDescriptor.GetDirectiveDescriptor(this.currentDirective.GetType());

                var completions = new List<Completion>();
                foreach (AttributeDescriptor attribute in directiveDescriptor.Attributes.Values)
                {
                    if (!this.currentDirective.Attributes.ContainsKey(attribute.DisplayName))
                    {
                        completions.Add(CreateAttributeCompletion(attribute));
                    }
                }

                if (completions.Count > 0)
                {
                    this.Completions = completions;
                    this.Node = node;
                }
            }
        }

        protected internal override void VisitAttributeValue(AttributeValue node)
        {
            base.VisitAttributeValue(node);

            if (node.Span.Start <= this.position && this.position <= node.Span.End)
            {
                Debug.Assert(this.currentDirective != null, "currentDirective");
                Debug.Assert(this.currentAttribute != null, "currentAttribute");
                DirectiveDescriptor directiveDescriptor = DirectiveDescriptor.GetDirectiveDescriptor(this.currentDirective.GetType());
                AttributeDescriptor attributeDescriptor;
                if (directiveDescriptor.Attributes.TryGetValue(this.currentAttribute.Name, out attributeDescriptor))
                {
                    this.Completions = new List<Completion>(attributeDescriptor.Values.Values.Select(CreateAttributeValueCompletion));
                    this.Node = node;
                }
            }
        }

        protected internal override void VisitDirective(Directive node)
        {
            this.currentDirective = node;
            base.VisitDirective(node);
        }

        protected internal override void VisitDirectiveName(DirectiveName node)
        {
            base.VisitDirectiveName(node);

            if (node.Span.Start <= this.position && this.position <= node.Span.End)
            {
                this.Completions = DirectiveDescriptor.GetBuiltInDirectives()
                    .Where(descriptor => !string.IsNullOrEmpty(descriptor.DisplayName)) // Skip custom directives
                    .Select(CreateDirectiveCompletion)
                    .ToList();
                this.Node = node;
            }
        }

        private static Completion CreateAttributeCompletion(AttributeDescriptor attribute)
        {
            return new Completion(attribute.DisplayName, attribute.DisplayName, attribute.Description, null, null);
        }

        private static Completion CreateAttributeValueCompletion(ValueDescriptor value)
        {
            Debug.Assert(value != null, "value");
            return new Completion(value.DisplayName, value.DisplayName, value.Description, null, null);
        }

        private static Completion CreateDirectiveCompletion(DirectiveDescriptor directive)
        {
            return new Completion(directive.DisplayName, directive.DisplayName, directive.Description, null, null);
        }
    }
}
