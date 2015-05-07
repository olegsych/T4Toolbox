// <copyright file="ClassificationFormatDefinitions.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.Editor
{
    using System.ComponentModel.Composition;
    using System.Windows.Media;
    using Microsoft.VisualStudio.Text.Classification;
    using Microsoft.VisualStudio.Utilities;

    /// <summary>
    /// Provides metadata information for registering text template format definitions with the Visual Studio editor.
    /// </summary>
    internal static class ClassificationFormatDefinitions
    {
        [Export(typeof(EditorFormatDefinition))]
        [Name("Text Template Attribute Name")]
        [ClassificationType(ClassificationTypeNames = ClassificationTypeName.AttributeName)]
        [UserVisible(true)]
        [Order(Before = Priority.Default)]
        internal sealed class AttributeName : ClassificationFormatDefinition
        {
            internal AttributeName()
            {
                this.ForegroundColor = Colors.Red;
            }
        }

        [Export(typeof(EditorFormatDefinition))]
        [Name("Text Template Attribute Value")]
        [ClassificationType(ClassificationTypeNames = ClassificationTypeName.AttributeValue)]
        [UserVisible(true)]
        [Order(Before = Priority.Default)]
        internal sealed class AttributeValue : ClassificationFormatDefinition
        {
            internal AttributeValue()
            {
                this.ForegroundColor = Colors.Blue;
            }
        }

        [Export(typeof(EditorFormatDefinition))]
        [Name("Text Template Code Block")]
        [ClassificationType(ClassificationTypeNames = ClassificationTypeName.CodeBlock)]
        [UserVisible(true)]
        [Order(Before = Priority.Default)]
        internal sealed class CodeBlock : ClassificationFormatDefinition
        {
            internal CodeBlock()
            {
                this.BackgroundColor = Colors.Lavender;
            }
        }

        [Export(typeof(EditorFormatDefinition))]
        [Name("Text Template Delimiter")]
        [ClassificationType(ClassificationTypeNames = ClassificationTypeName.Delimiter)]
        [UserVisible(true)]
        [Order(Before = Priority.Default)]
        internal sealed class Delimiter : ClassificationFormatDefinition
        {
            internal Delimiter()
            {
                this.BackgroundColor = Colors.Yellow;
            }
        }

        [Export(typeof(EditorFormatDefinition))]
        [Name("Text Template Directive Name")]
        [ClassificationType(ClassificationTypeNames = ClassificationTypeName.DirectiveName)]
        [UserVisible(true)]
        [Order(Before = Priority.Default)]
        internal sealed class DirectiveName : ClassificationFormatDefinition
        {
            internal DirectiveName()
            {
                this.ForegroundColor = Colors.Maroon;
            }
        }
    }
}