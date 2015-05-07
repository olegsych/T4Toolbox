// <copyright file="ClassificationTypeDefinitions.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.Editor
{
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Text.Classification;
    using Microsoft.VisualStudio.Utilities;

    /// <summary>
    /// Provides metadata information for registering text template classification types with the Visual Studio editor.
    /// </summary>
    internal static class ClassificationTypeDefinitions
    {
        [Export, Name(ClassificationTypeName.AttributeName)]
        internal static ClassificationTypeDefinition AttributeName { get; set; }

        [Export, Name(ClassificationTypeName.AttributeValue)]
        internal static ClassificationTypeDefinition AttributeValue { get; set; }

        [Export, Name(ClassificationTypeName.CodeBlock)]
        internal static ClassificationTypeDefinition CodeBlock { get; set; } 

        [Export, Name(ClassificationTypeName.Delimiter)]
        internal static ClassificationTypeDefinition Delimiter { get; set; }

        [Export, Name(ClassificationTypeName.DirectiveName)]
        internal static ClassificationTypeDefinition DirectiveName { get; set; }
    }
}