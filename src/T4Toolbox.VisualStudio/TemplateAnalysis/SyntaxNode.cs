// <copyright file="SyntaxNode.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.TemplateAnalysis
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using Microsoft.VisualStudio.Text;

    internal abstract class SyntaxNode : IEquatable<SyntaxNode>
    {
        private static readonly ConcurrentDictionary<Type, DescriptionAttribute> DescriptionAttributes = new ConcurrentDictionary<Type, DescriptionAttribute>();

        public abstract SyntaxKind Kind { get; }

        public abstract Position Position { get; }

        public abstract Span Span { get; }

        public abstract IEnumerable<SyntaxNode> ChildNodes();

        public abstract IEnumerable<TemplateError> Validate();

        public string GetText(string template)
        {
            Span span = this.Span;
            return template.Substring(span.Start, span.Length);
        }

        public bool Equals(SyntaxNode other)
        {
            return other != null
                && this.Kind == other.Kind 
                && this.Span == other.Span 
                && this.Position.Equals(other.Position)
                && this.ChildNodes().SequenceEqual(other.ChildNodes());
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as SyntaxNode);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(HashCode.Combine(this.Kind.GetHashCode(), this.Span.GetHashCode()), this.Position.GetHashCode());
        }

        public virtual bool TryGetDescription(int position, out string description, out Span applicableTo)
        {
            if (this.Span.Contains(position))
            {
                foreach (SyntaxNode childNode in this.ChildNodes())
                {
                    if (childNode.TryGetDescription(position, out description, out applicableTo))
                    {
                        return true;
                    }
                }

                DescriptionAttribute attribute = DescriptionAttributes.GetOrAdd(this.GetType(), GetDescriptionAttribute);
                if (attribute != null)
                {
                    description = attribute.Description;
                    applicableTo = this.Span;
                    return true;
                }
            }

            description = string.Empty;
            applicableTo = default(Span);
            return false;
        }

        protected internal abstract void Accept(SyntaxNodeVisitor visitor);

        private static DescriptionAttribute GetDescriptionAttribute(Type nodeType)
        {
            return TypeDescriptor.GetAttributes(nodeType).OfType<DescriptionAttribute>().FirstOrDefault();
        }
    }
}