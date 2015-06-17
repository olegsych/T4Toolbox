// <copyright file="TemplateScannerTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.TemplateAnalysis
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.VisualStudio.Text;
    using Xunit;

    public static class TemplateScannerTest
    {
        #region EndOfFile

        [Fact]
        public static void ScanIdentifiesEndOfFileInEmptyTemplate()
        {
            ScanIdentifiesEndOfFile(string.Empty);
        }

        [Fact]
        public static void ScanIdentifiesEndOfFileAfterText()
        {
            ScanIdentifiesEndOfFile("text");
        }

        [Fact]
        public static void ScanIdentifiesEndOfFileAfterCodeBlockStart()
        {
            ScanIdentifiesEndOfFile("<#");
        }

        [Fact]
        public static void ScanIdentifiesEndOfFileAfterCode()
        {
            ScanIdentifiesEndOfFile("<# code");
        }

        [Fact]
        public static void ScanIdentifiesEndOfFileAfterCodeBlock()
        {
            ScanIdentifiesEndOfFile("<# code #>");
        }

        [Fact]
        public static void ScanIdentifiesEndOfFileAfterDirectiveBlockStart()
        {
            ScanIdentifiesEndOfFile("<#@");
        }

        [Fact]
        public static void ScanIdentifiesEndOfFileAfterDirectiveName()
        {
            ScanIdentifiesEndOfFile("<#@ directive");
        }

        [Fact]
        public static void ScanIdentifiesEndOfFileAfterDirectiveAttributesStart()
        {
            ScanIdentifiesEndOfFile("<#@ template language=\"C#\"");
        }

        [Fact]
        public static void ScanIdentifiesEndOfFileAfterDirectiveAttributeValueStart()
        {
            ScanIdentifiesEndOfFile("<#@ template language=\"C#");
        }

        [Fact]
        public static void ScanIdentifiesEndOfFileAfterDirective()
        {
            ScanIdentifiesEndOfFile("<#@ template #>");
        }

        #endregion 

        #region BlockEnd

        [Fact]
        public static void ScanIdentifiesBlockEnd()
        {
            var node = (BlockEnd)Scan("#>").First();
            Assert.Equal(new Span(0, 2), node.Span);
            Assert.Equal(new Position(0, 0), node.Position);
        }

        [Fact]
        public static void ScanIdentifiesBlockEndWithoutCodeBlock()
        {
            var node = (BlockEnd)Scan("code #>").First();
            Assert.Equal(new Span(5, 2), node.Span);
            Assert.Equal(new Position(0, 5), node.Position);
        }

        [Fact]
        public static void ScanIdentifiesBlockEndAfterCodeBlock()
        {
            var node = (BlockEnd)Scan("<# statement(); #>").ElementAt(2);
            Assert.Equal(new Span(16, 2), node.Span);
            Assert.Equal(new Position(0, 16), node.Position);
        }

        [Fact]
        public static void ScanIdentifiesBlockEndAfterDirectiveBlockStart()
        {
            var node = (BlockEnd)Scan("<#@#>").ElementAt(1);
            Assert.Equal(new Span(3, 2), node.Span);
            Assert.Equal(new Position(0, 3), node.Position);
        }

        [Fact]
        public static void ScanIdentifiesBlockEndAfterDirectiveName()
        {
            var node = (BlockEnd)Scan("<#@ directive #>").ElementAt(2);
            Assert.Equal(new Span(14, 2), node.Span);
            Assert.Equal(new Position(0, 14), node.Position);
        }

        [Fact]
        public static void ScanIdentifiesBlockEndAfterAttributeValue()
        {
            var node = (BlockEnd)Scan("<#@ directive attribute=\"value #>").ElementAt(6);
            Assert.Equal(new Span(31, 2), node.Span);
            Assert.Equal(new Position(0, 31), node.Position);
        }

        #endregion

        #region StatementBlockStart

        [Fact]
        public static void ScanIdentifiesStatementBlockStart()
        {
            var node = (StatementBlockStart)Scan("<#").First();
            Assert.Equal(new Span(0, 2), node.Span);
            Assert.Equal(new Position(0, 0), node.Position);
        }

        [Fact]
        public static void ScanIdentifiesStatementBlockStartAfterUnclosedCodeBlock()
        {
            var node = (StatementBlockStart)Scan("<#<#").ElementAt(1);
            Assert.Equal(new Span(2, 2), node.Span);
            Assert.Equal(new Position(0, 2), node.Position);
        }

        [Fact]
        public static void ScanIdentifiesStatementBlockStartAfterUnclosedDirective()
        {
            var node = (StatementBlockStart)Scan("<#@<#").ElementAt(1);
            Assert.Equal(new Span(3, 2), node.Span);
            Assert.Equal(new Position(0, 3), node.Position);
        }

        [Fact]
        public static void ScanIdentifiesStatementBlockStartAfterBlockEnd()
        {
            var node = (StatementBlockStart)Scan("#><#").ElementAt(1);
            Assert.Equal(new Span(2, 2), node.Span);
            Assert.Equal(new Position(0, 2), node.Position);
        }

        #endregion

        #region Code

        [Fact]
        public static void ScanIdentifiesCodeAfterStatementBlockStart()
        {
            var node = (Code)Scan("<# statement();").ElementAt(1);
            Assert.Equal(new Span(2, 13), node.Span);
            Assert.Equal(new Position(0, 2), node.Position);
        }

        [Fact]
        public static void ScanIdentifiesCodeAfterExpressionBlockStart()
        {
            var node = (Code)Scan("<#= expression").ElementAt(1);
            Assert.Equal(new Span(3, 11), node.Span);
            Assert.Equal(new Position(0, 3), node.Position);
        }

        [Fact]
        public static void ScanIdentifiesCodeInClassBlock()
        {
            var node = (Code)Scan("<#+ private int classFeature;").ElementAt(1);
            Assert.Equal(new Span(3, 26), node.Span);
            Assert.Equal(new Position(0, 3), node.Position);
        }

        [Fact]
        public static void ScanIdentifiesCodeFollowedByBlockEnd()
        {
            var code = (Code)Scan("<# code();#>").ElementAt(1);
            Assert.Equal(new Span(2, 8), code.Span);
            Assert.Equal(new Position(0, 2), code.Position);
        }

        [Fact]
        public static void ScanIdentifiesCodeFollowedByBlockStart()
        {
            var code = (Code)Scan("<# code();<#").ElementAt(1);
            Assert.Equal(new Span(2, 8), code.Span);
            Assert.Equal(new Position(0, 2), code.Position);
        }

        [Fact]
        public static void ScanIdentifiesCodeFollowedByEndOfFile()
        {
            var code = (Code)Scan("<# code();").ElementAt(1);
            Assert.Equal(new Span(2, 8), code.Span);
            Assert.Equal(new Position(0, 2), code.Position);
        }

        [Fact]
        public static void ScanSkipsEmptyCode()
        {
            SyntaxNode[] code = Scan("<##>").ToArray();
            Assert.Equal(typeof(StatementBlockStart), code[0].GetType());
            Assert.Equal(typeof(BlockEnd), code[1].GetType());
        }

        [Fact]
        public static void ScanIdentifiesMultilineCode()
        {
            const string Input = "<# line1(); \r\n line2(); #>";
            var code = (Code)Scan(Input).ElementAt(1);
            Assert.Equal(Span.FromBounds("<#".Length, Input.LastIndexOf("#>", StringComparison.Ordinal)), code.Span);
            Assert.Equal(new Position(0, "<#".Length), code.Position);
        }

        #endregion

        #region ExpressionBlockStart

        [Fact]
        public static void ScanIdentifiesExpressionBlockStart()
        {
            var node = (ExpressionBlockStart)Scan("<#=").First();
            Assert.Equal(new Span(0, 3), node.Span);
            Assert.Equal(new Position(0, 0), node.Position);
        }

        [Fact]
        public static void ScanIdentifiesExpressionBlockStartAfterUnclosedDirective()
        {
            var node = (ExpressionBlockStart)Scan("<#@<#=").ElementAt(1);
            Assert.Equal(new Span(3, 3), node.Span);
            Assert.Equal(new Position(0, 3), node.Position);
        }

        [Fact]
        public static void ScanIdentifiesExpressionBlockStartAfterBlockEnd()
        {
            var node = (ExpressionBlockStart)Scan("#><#=").ElementAt(1);
            Assert.Equal(new Span(2, 3), node.Span);
            Assert.Equal(new Position(0, 2), node.Position);
        }

        #endregion

        #region Class Block Tests

        [Fact]
        public static void ScanIdentifiesClassBlockStart()
        {
            var node = (ClassBlockStart)Scan("<#+").First();
            Assert.Equal(new Span(0, 3), node.Span);
            Assert.Equal(new Position(0, 0), node.Position);
        }

        [Fact]
        public static void ScanIdentifiesClassBlockStartAfterUnclosedDirective()
        {
            var node = (ClassBlockStart)Scan("<#@<#+").ElementAt(1);
            Assert.Equal(new Span(3, 3), node.Span);
            Assert.Equal(new Position(0, 3), node.Position);
        }

        [Fact]
        public static void ScanIdentifiesClassBlockStartAfterBlockEnd()
        {
            var node = (ClassBlockStart)Scan("#><#+").ElementAt(1);
            Assert.Equal(new Span(2, 3), node.Span);
            Assert.Equal(new Position(0, 2), node.Position);
        }

        #endregion

        #region DirectiveBlockStart

        [Fact]
        public static void ScanIdentifiesDirectiveBlockStart()
        {
            var node = (DirectiveBlockStart)Scan("<#@").First();
            Assert.Equal(new Span(0, 3), node.Span);
            Assert.Equal(new Position(0, 0), node.Position);
        }

        [Fact]
        public static void ScanIdentifiesDirectiveBlockStartAfterUnclosedDirective()
        {
            var node = (DirectiveBlockStart)Scan("<#@<#@").ElementAt(1);
            Assert.Equal(new Span(3, 3), node.Span);
            Assert.Equal(new Position(0, 3), node.Position);
        }

        #endregion

        #region DirectiveName

        [Fact]
        public static void ScanIdentifiesDirectiveName()
        {
            var node = (DirectiveName)Scan("<#@ directive").ElementAt(1);
            Assert.Equal(new Span(4, 9), node.Span);
            Assert.Equal(new Position(0, 4), node.Position);
        }

        [Fact]
        public static void ScanIdentifiesDirectiveNameWithoutLeadingSpaces() // For consistency with T4 parsing logic
        {
            var node = (DirectiveName)Scan("<#@directive").ElementAt(1);
            Assert.Equal(new Span(3, 9), node.Span);
            Assert.Equal(new Position(0, 3), node.Position);
        }

        #endregion

        #region AttributeName

        [Fact]
        public static void ScanIdentifiesAttributeNameFollowedByEquals()
        {
            var node = (AttributeName)Scan("<#@ attribute =").ElementAt(1);
            Assert.Equal(new Span(4, 9), node.Span);
            Assert.Equal(new Position(0, 4), node.Position);
        }

        [Fact]
        public static void ScanIdentifiesAttributeNameWithoutValue() // to colorize partially valid directives
        {
            var node = (AttributeName)Scan("<#@ directive attribute").ElementAt(2);
            Assert.Equal(new Span(14, 9), node.Span);
            Assert.Equal(new Position(0, 14), node.Position);
        }

        #endregion

        #region DoubleQuote

        [Fact]
        public static void ScanIdentifiesDoubleQuoteAfterDirectiveBlockStart()
        {
            var node = (DoubleQuote)Scan("<#@ \"").ElementAt(1);
            Assert.Equal(new Span(4, 1), node.Span);
            Assert.Equal(new Position(0, 4), node.Position);
        }

        [Fact]
        public static void ScanIdentifiesDoubleQuoteAfterDirectiveName()
        {
            var node = (DoubleQuote)Scan("<#@ directive \"").ElementAt(2);
            Assert.Equal(new Span(14, 1), node.Span);
            Assert.Equal(new Position(0, 14), node.Position);
        }

        [Fact]
        public static void ScanIdentifiesDoubleQuoteAfterAttributeName()
        {
            var node = (DoubleQuote)Scan("<#@ directive attribute = \"").ElementAt(4);
            Assert.Equal(new Span(26, 1), node.Span);
            Assert.Equal(new Position(0, 26), node.Position);
        }

        [Fact]
        public static void ScanIdentifiesDoubleQuoteWithoutLeadingSpaces()
        {
            var node = (DoubleQuote)Scan("<#@ directive attribute =\"").ElementAt(4);
            Assert.Equal(new Span(25, 1), node.Span);
            Assert.Equal(new Position(0, 25), node.Position);
        }

        [Fact]
        public static void ScanIdentifiesDoubleQuoteAfterAttributeValue()
        {
            var node = (DoubleQuote)Scan("<#@ directive attribute = \"value\"").ElementAt(6);
            Assert.Equal(new Span(32, 1), node.Span);
            Assert.Equal(new Position(0, 32), node.Position);
        }

        [Fact]
        public static void ScanIdentifiesDoubleQuoteAfterAttribute()
        {
            var node = (DoubleQuote)Scan("<#@ \"value1\" \"").ElementAt(4);
            Assert.Equal(new Span(13, 1), node.Span);
            Assert.Equal(new Position(0, 13), node.Position);
        }

        #endregion

        #region Equals

        [Fact]
        public static void ScanIdentifiesEqualsAfterDirectiveBlockStart()
        {
            var node = (Equals)Scan("<#@=").ElementAt(1);
            Assert.Equal(new Span(3, 1), node.Span);
            Assert.Equal(new Position(0, 3), node.Position);
        }

        [Fact]
        public static void ScanIdentifiesEqualsAfterDirectiveName()
        {
            var node = (Equals)Scan("<#@ directive =").ElementAt(2);
            Assert.Equal(new Span(14, 1), node.Span);
            Assert.Equal(new Position(0, 14), node.Position);
        }

        [Fact]
        public static void ScanIdentifiesEqualsAfterAttributeName()
        {
            var node = (Equals)Scan("<#@ directive attribute =").ElementAt(3);
            Assert.Equal(new Span(24, 1), node.Span);
            Assert.Equal(new Position(0, 24), node.Position);
        }

        [Fact]
        public static void ScanIdentifiesEqualsWithoutLeadingSpaces()
        {
            var node = (Equals)Scan("<#@ directive attribute=").ElementAt(3);
            Assert.Equal(new Span(23, 1), node.Span);
            Assert.Equal(new Position(0, 23), node.Position);
        }

        #endregion

        #region AttributeValue

        [Fact]
        public static void ScanIdentifiesAttributeValueFollowedByDoubleQuote()
        {
            var node = (AttributeValue)Scan("<#@ \"value\"").ElementAt(2);
            Assert.Equal(new Span(5, 5), node.Span);
            Assert.Equal(new Position(0, 5), node.Position);
        }

        [Fact]
        public static void ScanIdentifiesAttributeValueFollowedByEndOfFile()
        {
            var node = (AttributeValue)Scan("<#@ \"value").ElementAt(2);
            Assert.Equal(new Span(5, 5), node.Span);
            Assert.Equal(new Position(0, 5), node.Position);
        }

        [Fact]
        public static void ScanIdentifiesAttributeValueFollowedByBlockEnd()
        {
            var node = (AttributeValue)Scan("<#@ \"value#>").ElementAt(2);
            Assert.Equal(new Span(5, 5), node.Span);
            Assert.Equal(new Position(0, 5), node.Position);
        }

        [Fact]
        public static void ScanIdentifiesAttributeValueFollowedByBlockStart()
        {
            var node = (AttributeValue)Scan("<#@ \"value<#").ElementAt(2);
            Assert.Equal(new Span(5, 5), node.Span);
            Assert.Equal(new Position(0, 5), node.Position);
        }

        [Fact]
        public static void ScanIdentifiesMultilineAttributeValue()
        {
            const string Input = "<#@ \"line1 \r\n line2\" #>";
            var node = (AttributeValue)Scan(Input).ElementAt(2);
            Assert.Equal(
                Span.FromBounds(
                    Input.IndexOf("\"", System.StringComparison.Ordinal) + 1, 
                    Input.LastIndexOf("\"", System.StringComparison.Ordinal)), 
                node.Span);
            Assert.Equal(new Position(0, Input.IndexOf("\"", System.StringComparison.Ordinal) + 1), node.Position);
        }

        #endregion

        private static IEnumerable<SyntaxNode> Scan(string input)
        {
            var scanner = new TemplateScanner(input);
            while (scanner.yylex() != (int)SyntaxKind.EOF)
            {
                yield return scanner.yylval;
            }

            yield return scanner.yylval; // EndOfFile 
        }

        private static void ScanIdentifiesEndOfFile(string input)
        {
            var node = (EndOfFile)Scan(input).Last();
            Assert.Equal(new Span(input.Length, 0), node.Span);
            Assert.Equal(new Position(0, input.Length), node.Position); 
        }
    }
}