// <copyright file="TemplateClassificationTagger.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.Editor
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Windows;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Classification;
    using Microsoft.VisualStudio.Text.Tagging;
    using T4Toolbox.TemplateAnalysis;

    internal sealed class TemplateClassificationTagger : SimpleTagger<ClassificationTag>
    {
        private readonly ITextBuffer buffer;
        private readonly IClassificationType attributeNameClassification;
        private readonly IClassificationType attributeValueClassification;
        private readonly IClassificationType codeBlockClassificaiton;
        private readonly IClassificationType delimiterClassification;
        private readonly IClassificationType directiveNameClassificaiton;

        internal TemplateClassificationTagger(ITextBuffer buffer, IClassificationTypeRegistryService classificationTypeRegistry)
            : base(buffer)
        {
            Debug.Assert(classificationTypeRegistry != null, "classificationTypeRegistry");

            this.buffer = buffer;
            WeakEventManager<ITextBuffer, TextContentChangedEventArgs>.AddHandler(this.buffer, "Changed", this.BufferChanged);

            this.attributeNameClassification = classificationTypeRegistry.GetClassificationType(ClassificationTypeName.AttributeName);
            this.attributeValueClassification = classificationTypeRegistry.GetClassificationType(ClassificationTypeName.AttributeValue);
            this.delimiterClassification = classificationTypeRegistry.GetClassificationType(ClassificationTypeName.Delimiter);
            this.directiveNameClassificaiton = classificationTypeRegistry.GetClassificationType(ClassificationTypeName.DirectiveName);
            this.codeBlockClassificaiton = classificationTypeRegistry.GetClassificationType(ClassificationTypeName.CodeBlock);

            this.UpdateTagSpans();
        }

        private void BufferChanged(object sender, TextContentChangedEventArgs e)
        {
            this.UpdateTagSpans();
        }

        private void CreateTagSpans(ITextSnapshot snapshot)
        {
            var scanner = new TemplateScanner(snapshot.GetText());
            while (scanner.yylex() != (int)SyntaxKind.EOF)
            {
                SyntaxNode token = scanner.yylval;
                IClassificationType classificationType;
                switch (token.Kind)
                {
                    case SyntaxKind.BlockEnd:
                    case SyntaxKind.ClassBlockStart:
                    case SyntaxKind.DirectiveBlockStart:
                    case SyntaxKind.ExpressionBlockStart:
                    case SyntaxKind.StatementBlockStart:
                        classificationType = this.delimiterClassification;
                        break;

                    case SyntaxKind.DirectiveName:
                        classificationType = this.directiveNameClassificaiton;
                        break;

                    case SyntaxKind.AttributeName:
                        classificationType = this.attributeNameClassification;
                        break;

                    case SyntaxKind.AttributeValue:
                        classificationType = this.attributeValueClassification;
                        break;

                    case SyntaxKind.Code:
                        classificationType = this.codeBlockClassificaiton;
                        break;

                    case SyntaxKind.Equals:
                    case SyntaxKind.DoubleQuote:
                        // Ignore
                        continue;

                    default:
                        throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "Unexpected SyntaxKind value {0}.", token.Kind));
                }

                this.CreateTagSpan(snapshot.CreateTrackingSpan(token.Span, SpanTrackingMode.EdgeNegative), new ClassificationTag(classificationType));
            }
        }

        private void UpdateTagSpans()
        {
            using (this.Update())
            {
                this.RemoveTagSpans(trackingTagSpan => true); // remove all tag spans
                this.CreateTagSpans(this.buffer.CurrentSnapshot);
            }
        }
    }
}