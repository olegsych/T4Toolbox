// <copyright file="ParameterDirective.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.TemplateAnalysis
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics;

    [DisplayName("parameter"), Description("Declares a property in template code initialized with a value passed in from the external context.")]
    internal sealed class ParameterDirective : Directive
    {
        public ParameterDirective(DirectiveBlockStart start, DirectiveName name, IEnumerable<Attribute> attributes, BlockEnd end)
            : base(start, name, attributes, end)
        {
            Debug.Assert(name.Text.Equals("parameter", StringComparison.OrdinalIgnoreCase), "name");
        }

        [DisplayName("Name"), Required(ErrorMessage = "The Name attribute is required")]
        [Description("Name of the property.")]
        public string ParameterName
        {
            get { return this.GetAttributeValue(); }
        }

        [DisplayName("Type"), Required(ErrorMessage = "The Type attribute is required")]
        [Description("Fully-qualified name of the property type.")]
        public string ParameterType
        {
            get { return this.GetAttributeValue(); }
        }

        protected internal override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitParameterDirective(this);
        }
    }
}