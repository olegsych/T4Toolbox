// <copyright file="TemplateLanguage.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.TemplateAnalysis
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    internal enum TemplateLanguage
    {
        [Display(Name = "C#", Description = "Visual C#. C# is the default template language.")]
        CSharp,

        [Display(Name = "VB", Description = "Visual Basic")]
        VisualBasic
    }
}
