// <copyright file="TemplateAnalysis.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.Editor
{
    using System;
    using System.Collections.Generic;
    using Microsoft.VisualStudio.Text;
    using T4Toolbox.TemplateAnalysis;

    internal class TemplateAnalysis : EventArgs
    {
        public TemplateAnalysis(ITextSnapshot textSnapshot, Template template, IReadOnlyList<TemplateError> errors)
        {
            this.TextSnapshot = textSnapshot;
            this.Template = template;
            this.Errors = errors;
        }

        public IReadOnlyList<TemplateError> Errors { get; private set; }

        public Template Template { get; private set; }

        public ITextSnapshot TextSnapshot { get; private set; }
    }
}