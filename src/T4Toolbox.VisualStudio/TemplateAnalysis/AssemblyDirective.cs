// <copyright file="AssemblyDirective.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.TemplateAnalysis
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics;

    [DisplayName("assembly"), Description("Loads an assembly so that your template code can use its types. The effect is similar to adding an assembly reference in a Visual Studio project.")]
    internal sealed class AssemblyDirective : Directive
    {
        public AssemblyDirective(DirectiveBlockStart start, DirectiveName name, IEnumerable<Attribute> attributes, BlockEnd end)
            : base(start, name, attributes, end)
        {
            Debug.Assert(name.Text.Equals("assembly", StringComparison.OrdinalIgnoreCase), "name");
        }

        [DisplayName("Name"), Required(ErrorMessage = "The Name attribute is required"), Description("Name of an assembly in the GAC or absolute file path to the assembly.")]
        public string AssemblyName
        {
            get { return this.GetAttributeValue(); }
        }

        protected internal override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitAssemblyDirective(this);
        }
    }
}