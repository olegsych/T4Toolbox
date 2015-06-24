// <copyright file="TemplateDirective.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.TemplateAnalysis
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Linq;

    [DisplayName("template"), Description("Specifies how the template should be processed.")]
    internal sealed class TemplateDirective : Directive
    {
        public TemplateDirective(DirectiveBlockStart start, DirectiveName name, IEnumerable<Attribute> attributes, BlockEnd end)
            : base(start, name, attributes, end)
        {
        }

        private enum HostSpecificValue
        {
            [Display(Name = "false", Description = "No Host property will be generated in the intermediate code. False is the default.")]
            False,

            [Display(Name = "true", Description = 
                "A property named Host will be generated in the intermediate code. The property is a reference to the host of the " +
                "transformation engine, and is declared as Microsoft.VisualStudio.TextTemplating.ITextTemplatingEngineHost. " + 
                "If you are using Visual Studio, you can cast this.Host to IServiceProvider to access Visual Studio features.")]
            True,

            [Display(Name = "trueFromBase", Description = 
                "If you use the inherits and hostspecific attributes together, specify host=\"trueFromBase\" in the derived class " +
                "and host=\"true\" in the base class. This avoids a double definition of the Host property in the intermediate code.")]
            TrueFromBase
        }

        [KnownValues(typeof(TemplateDirective), "GetKnownCultureValues")]
        [Description("Culture used when an expression block is converted to text.")]
        public string Culture
        {
            get { return this.GetAttributeValue(); }
        }

        [Description("C# or Visual Basic compiler options applied when template has been converted to code and compiled.")]
        public string CompilerOptions
        {
            get { return this.GetAttributeValue(); }
        }

        [KnownValues(typeof(TemplateDirective), "GetKnownDebugValues")]
        [Description("When set to true, enables debugging of the template code.")]
        public string Debug
        {
            get { return this.GetAttributeValue(); }
        }

        [KnownValues(typeof(HostSpecificValue))]
        [Description("When set to true, a property named Host is added to the template code. The property is a reference to the host of the transformation engine, and is declared as Microsoft.VisualStudio.TextTemplating.ITextTemplatingEngineHost.")]
        public string HostSpecific
        {
            get { return this.GetAttributeValue(); }
        }

        [Description("Specifies base class of the template.")]
        public string Inherits
        {
            get { return this.GetAttributeValue(); }
        }

        [KnownValues(typeof(TemplateLanguage))]
        [Description("Specifies the language (Visual Basic or Visual C#) used in code blocks of the template.")]
        public string Language
        {
            get { return this.GetAttributeValue(); }
        }

        [KnownValues(typeof(TemplateDirective), "GetKnownLinePragmasValues")]
        [Description("Specifies whether generated template code will include information about line numbers from the original template file.")]
        public string LinePragmas
        {
            get { return this.GetAttributeValue(); }
        }

        [KnownValues(typeof(TemplateVisibility))]
        [Description("In a runtime template, specifies visibility of the generated template class.")]
        public string Visibility
        {
            get { return this.GetAttributeValue(); }
        }

        protected internal override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitTemplateDirective(this);
        }

        private static IEnumerable<ValueDescriptor> GetKnownCultureValues()
        {
            return CultureInfo.GetCultures(CultureTypes.AllCultures)
                .Where(culture => !string.IsNullOrWhiteSpace(culture.Name)) // Skip the invariant culture (it doesn't have a name and wouldn't be a valid value with the current TemplateParser).
                .Select(culture => new ValueDescriptor(culture.Name, culture.DisplayName));
        }

        private static IEnumerable<ValueDescriptor> GetKnownDebugValues()
        {
            yield return new ValueDescriptor("false", "No debugging information will be generated for the intermediate code. False is the default.");
            yield return new ValueDescriptor("true", "The intermediate code file will contain information that enables the debugger to identify more accurately the position in your template where a break or exception occurred.");
        }

        private static IEnumerable<ValueDescriptor> GetKnownLinePragmasValues()
        {
            yield return new ValueDescriptor("false", "Removes the tags that identify template line numbers within the intermediate code generated by T4. The compiler will report errors by using line numbers of the intermediate code.");
            yield return new ValueDescriptor("true", "Adds the tags that identify template line numbers within the intermediate code generated by T4. The compiler will report errors by using line numbers of the template code. True is the default.");
        }
    }
}