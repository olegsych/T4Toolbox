// <copyright file="ImportDirective.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.TemplateAnalysis
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics;

    [DisplayName("import"), Description("Allows the use of types in a namespace without providing a fully-qualified name. It is the equivalent of using in C# or imports in Visual Basic.")]
    internal sealed class ImportDirective : Directive
    {
        public ImportDirective(DirectiveBlockStart start, DirectiveName name, IEnumerable<Attribute> attributes, BlockEnd end)
            : base(start, name, attributes, end)
        {
            Debug.Assert(name.Text.Equals("import", StringComparison.OrdinalIgnoreCase), "name");
        }

        [Required(ErrorMessage = "The Namespace attribute is required")]
        [Description("The fully-qualified name of the namespace being imported. Can be a string of namespaces nested to any level.")]
        public string Namespace
        {
            get { return this.GetAttributeValue(); }
        }

        protected internal override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitImportDirective(this);
        }
    }
}