// <copyright file="TemplateContentType.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.Editor
{
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Utilities;

    internal static class TemplateContentType
    {
        internal const string Name = "TextTemplate";

        [Export, Name(Name), BaseDefinition("code")]
        internal static ContentTypeDefinition Definition { get; set; } // Used for metadata only

        [Export, FileExtension(".tt"), ContentType(Name)]
        internal static FileExtensionToContentTypeDefinition FileAssociation { get; set; } // Used for metadata only
    }
}