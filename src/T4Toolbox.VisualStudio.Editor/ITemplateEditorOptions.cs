// <copyright file="ITemplateEditorOptions.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.Editor
{
    internal interface ITemplateEditorOptions
    {
        bool CompletionListsEnabled { get; }

        bool ErrorReportingEnabled { get; }

        bool ErrorUnderliningEnabled { get; }

        bool QuickInfoTooltipsEnabled { get; }

        bool SyntaxColorizationEnabled { get; }

        bool TemplateOutliningEnabled { get; }
    }
}
