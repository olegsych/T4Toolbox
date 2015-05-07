// <copyright file="OutputDirective.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.TemplateAnalysis
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Text;

    [DisplayName("output"), Description("Defines extension and encoding of the output file.")]
    internal sealed class OutputDirective : Directive
    {
        public OutputDirective(DirectiveBlockStart start, DirectiveName name, IEnumerable<Attribute> attributes, BlockEnd end)
            : base(start, name, attributes, end)
        {
        }

        [Required(ErrorMessage = "The Extension attribute is required")]
        [Description("Extension of the output file.")]
        public string Extension
        {
            get { return this.GetAttributeValue(); }
        }

        [KnownValues(typeof(OutputDirective), "GetKnownEncodingValues")]
        [Description("Encoding of the output file.")]
        public string Encoding
        {
            get { return this.GetAttributeValue(); }
        }

        protected internal override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitOutputDirective(this);
        }

        private static IEnumerable<ValueDescriptor> GetKnownEncodingValues()
        {
            var uniqueNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (EncodingInfo encoding in System.Text.Encoding.GetEncodings())
            {
                // Eliminate encodings that differ only by casing
                if (uniqueNames.Add(encoding.Name))
                {
                    yield return new ValueDescriptor(encoding.Name, encoding.DisplayName);
                }            
            }
        }
    }
}