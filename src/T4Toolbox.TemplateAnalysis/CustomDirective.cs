// <copyright file="CustomDirective.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.TemplateAnalysis
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    [Description("Custom directive.")]
    internal sealed class CustomDirective : Directive
    {
        public CustomDirective(DirectiveBlockStart start, DirectiveName name, IEnumerable<Attribute> attributes, BlockEnd end)
            : base(start, name, attributes, end)
        {
        }

        [Required(ErrorMessage = "The Processor attribute is required")]
        [Description("Name of a custom directive processor.")]
        public string Processor
        {
            get { return this.GetAttributeValue(); }
        }

        protected internal override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitCustomDirective(this);
        }

        protected override IEnumerable<TemplateError> ValidateAttributes()
        {
            // Custom directives can contain any custom attributes they need
            return Enumerable.Empty<TemplateError>();
        }
    }
}