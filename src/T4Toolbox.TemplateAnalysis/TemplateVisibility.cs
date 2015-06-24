// <copyright file="TemplateVisibility.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.TemplateAnalysis
{
    using System.ComponentModel.DataAnnotations;

    internal enum TemplateVisibility
    {
        [Display(Name = "public", Description = "Changes visibility of generated template class to public. Public is the default.")]
        Public,

        [Display(Name = "internal", Description = "Changes visibility of generated template class to internal.")]
        Internal
    }
}
