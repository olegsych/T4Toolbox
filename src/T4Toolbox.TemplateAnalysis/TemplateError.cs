// <copyright file="TemplateError.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.TemplateAnalysis
{
    using System.Diagnostics;
    using Microsoft.VisualStudio.Text;

    internal class TemplateError
    {
        public TemplateError(string message, Span span, Position position)
        {
            Debug.Assert(message != null, "message");
    
            this.Message = message;
            this.Position = position;
            this.Span = span;
        }

        public string Message { get; private set; }

        public Position Position { get; private set; }

        public Span Span { get; private set; }
    }
}