// <copyright file="IncludeDirective.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.TemplateAnalysis
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics;

    [DisplayName("include"), Description("Includes text from another file in the current template.")]
    internal sealed class IncludeDirective : Directive
    {
        public IncludeDirective(DirectiveBlockStart start, DirectiveName name, IEnumerable<Attribute> attributes, BlockEnd end)
            : base(start, name, attributes, end)
        {
            Debug.Assert(name.Text.Equals("include", StringComparison.OrdinalIgnoreCase), "name");
        }

        [Required(ErrorMessage = "The File attribute is required")]
        [Description("Absolute or relative path to the included file.")]
        public string File
        {
            get { return this.GetAttributeValue(); }
        }

        protected internal override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitIncludeDirective(this);
        }
    }
}