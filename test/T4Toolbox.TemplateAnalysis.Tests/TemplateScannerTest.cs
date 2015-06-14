// <copyright file="TemplateScannerTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.TemplateAnalysis
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.VisualStudio.Text;

    [TestClass]
    public class TemplateScannerTest
    {
        #region EndOfFile

        [TestMethod]
        public void ScanIdentifiesEndOfFileInEmptyTemplate()
        {
            ScanIdentifiesEndOfFile(string.Empty);
        }

        [TestMethod]
        public void ScanIdentifiesEndOfFileAfterText()
        {
            ScanIdentifiesEndOfFile("text");
        }

        [TestMethod]
        public void ScanIdentifiesEndOfFileAfterCodeBlockStart()
        {
            ScanIdentifiesEndOfFile("<#");
        }

        [TestMethod]
        public void ScanIdentifiesEndOfFileAfterCode()
        {
            ScanIdentifiesEndOfFile("<# code");
        }

        [TestMethod]
        public void ScanIdentifiesEndOfFileAfterCodeBlock()
        {
            ScanIdentifiesEndOfFile("<# code #>");
        }

        [TestMethod]
        public void ScanIdentifiesEndOfFileAfterDirectiveBlockStart()
        {
            ScanIdentifiesEndOfFile("<#@");
        }

        [TestMethod]
        public void ScanIdentifiesEndOfFileAfterDirectiveName()
        {
            ScanIdentifiesEndOfFile("<#@ directive");
        }

        [TestMethod]
        public void ScanIdentifiesEndOfFileAfterDirectiveAttributesStart()
        {
            ScanIdentifiesEndOfFile("<#@ template language=\"C#\"");
        }

        [TestMethod]
        public void ScanIdentifiesEndOfFileAfterDirectiveAttributeValueStart()
        {
            ScanIdentifiesEndOfFile("<#@ template language=\"C#");
        }

        [TestMethod]
        public void ScanIdentifiesEndOfFileAfterDirective()
        {
            ScanIdentifiesEndOfFile("<#@ template #>");
        }

        #endregion 

        #region BlockEnd

        [TestMethod]
        public void ScanIdentifiesBlockEnd()
        {
            var node = (BlockEnd)Scan("#>").First();
            Assert.AreEqual(new Span(0, 2), node.Span);
            Assert.AreEqual(new Position(0, 0), node.Position);
        }

        [TestMethod]
        public void ScanIdentifiesBlockEndWithoutCodeBlock()
        {
            var node = (BlockEnd)Scan("code #>").First();
            Assert.AreEqual(new Span(5, 2), node.Span);
            Assert.AreEqual(new Position(0, 5), node.Position);
        }

        [TestMethod]
        public void ScanIdentifiesBlockEndAfterCodeBlock()
        {
            var node = (BlockEnd)Scan("<# statement(); #>").ElementAt(2);
            Assert.AreEqual(new Span(16, 2), node.Span);
            Assert.AreEqual(new Position(0, 16), node.Position);
        }

        [TestMethod]
        public void ScanIdentifiesBlockEndAfterDirectiveBlockStart()
        {
            var node = (BlockEnd)Scan("<#@#>").ElementAt(1);
            Assert.AreEqual(new Span(3, 2), node.Span);
            Assert.AreEqual(new Position(0, 3), node.Position);
        }

        [TestMethod]
        public void ScanIdentifiesBlockEndAfterDirectiveName()
        {
            var node = (BlockEnd)Scan("<#@ directive #>").ElementAt(2);
            Assert.AreEqual(new Span(14, 2), node.Span);
            Assert.AreEqual(new Position(0, 14), node.Position);
        }

        [TestMethod]
        public void ScanIdentifiesBlockEndAfterAttributeValue()
        {
            var node = (BlockEnd)Scan("<#@ directive attribute=\"value #>").ElementAt(6);
            Assert.AreEqual(new Span(31, 2), node.Span);
            Assert.AreEqual(new Position(0, 31), node.Position);
        }

        #endregion

        #region StatementBlockStart

        [TestMethod]
        public void ScanIdentifiesStatementBlockStart()
        {
            var node = (StatementBlockStart)Scan("<#").First();
            Assert.AreEqual(new Span(0, 2), node.Span);
            Assert.AreEqual(new Position(0, 0), node.Position);
        }

        [TestMethod]
        public void ScanIdentifiesStatementBlockStartAfterUnclosedCodeBlock()
        {
            var node = (StatementBlockStart)Scan("<#<#").ElementAt(1);
            Assert.AreEqual(new Span(2, 2), node.Span);
            Assert.AreEqual(new Position(0, 2), node.Position);
        }

        [TestMethod]
        public void ScanIdentifiesStatementBlockStartAfterUnclosedDirective()
        {
            var node = (StatementBlockStart)Scan("<#@<#").ElementAt(1);
            Assert.AreEqual(new Span(3, 2), node.Span);
            Assert.AreEqual(new Position(0, 3), node.Position);
        }

        [TestMethod]
        public void ScanIdentifiesStatementBlockStartAfterBlockEnd()
        {
            var node = (StatementBlockStart)Scan("#><#").ElementAt(1);
            Assert.AreEqual(new Span(2, 2), node.Span);
            Assert.AreEqual(new Position(0, 2), node.Position);
        }

        #endregion

        #region Code

        [TestMethod]
        public void ScanIdentifiesCodeAfterStatementBlockStart()
        {
            var node = (Code)Scan("<# statement();").ElementAt(1);
            Assert.AreEqual(new Span(2, 13), node.Span);
            Assert.AreEqual(new Position(0, 2), node.Position);
        }

        [TestMethod]
        public void ScanIdentifiesCodeAfterExpressionBlockStart()
        {
            var node = (Code)Scan("<#= expression").ElementAt(1);
            Assert.AreEqual(new Span(3, 11), node.Span);
            Assert.AreEqual(new Position(0, 3), node.Position);
        }

        [TestMethod]
        public void ScanIdentifiesCodeInClassBlock()
        {
            var node = (Code)Scan("<#+ private int classFeature;").ElementAt(1);
            Assert.AreEqual(new Span(3, 26), node.Span);
            Assert.AreEqual(new Position(0, 3), node.Position);
        }

        [TestMethod]
        public void ScanIdentifiesCodeFollowedByBlockEnd()
        {
            var code = (Code)Scan("<# code();#>").ElementAt(1);
            Assert.AreEqual(new Span(2, 8), code.Span);
            Assert.AreEqual(new Position(0, 2), code.Position);
        }

        [TestMethod]
        public void ScanIdentifiesCodeFollowedByBlockStart()
        {
            var code = (Code)Scan("<# code();<#").ElementAt(1);
            Assert.AreEqual(new Span(2, 8), code.Span);
            Assert.AreEqual(new Position(0, 2), code.Position);
        }

        [TestMethod]
        public void ScanIdentifiesCodeFollowedByEndOfFile()
        {
            var code = (Code)Scan("<# code();").ElementAt(1);
            Assert.AreEqual(new Span(2, 8), code.Span);
            Assert.AreEqual(new Position(0, 2), code.Position);
        }

        [TestMethod]
        public void ScanSkipsEmptyCode()
        {
            SyntaxNode[] code = Scan("<##>").ToArray();
            Assert.AreEqual(typeof(StatementBlockStart), code[0].GetType());
            Assert.AreEqual(typeof(BlockEnd), code[1].GetType());
        }

        [TestMethod]
        public void ScanIdentifiesMultilineCode()
        {
            const string Input = "<# line1(); \r\n line2(); #>";
            var code = (Code)Scan(Input).ElementAt(1);
            Assert.AreEqual(Span.FromBounds("<#".Length, Input.LastIndexOf("#>", StringComparison.Ordinal)), code.Span);
            Assert.AreEqual(new Position(0, "<#".Length), code.Position);
        }

        #endregion

        #region ExpressionBlockStart

        [TestMethod]
        public void ScanIdentifiesExpressionBlockStart()
        {
            var node = (ExpressionBlockStart)Scan("<#=").First();
            Assert.AreEqual(new Span(0, 3), node.Span);
            Assert.AreEqual(new Position(0, 0), node.Position);
        }

        [TestMethod]
        public void ScanIdentifiesExpressionBlockStartAfterUnclosedDirective()
        {
            var node = (ExpressionBlockStart)Scan("<#@<#=").ElementAt(1);
            Assert.AreEqual(new Span(3, 3), node.Span);
            Assert.AreEqual(new Position(0, 3), node.Position);
        }

        [TestMethod]
        public void ScanIdentifiesExpressionBlockStartAfterBlockEnd()
        {
            var node = (ExpressionBlockStart)Scan("#><#=").ElementAt(1);
            Assert.AreEqual(new Span(2, 3), node.Span);
            Assert.AreEqual(new Position(0, 2), node.Position);
        }

        #endregion

        #region Class Block Tests

        [TestMethod]
        public void ScanIdentifiesClassBlockStart()
        {
            var node = (ClassBlockStart)Scan("<#+").First();
            Assert.AreEqual(new Span(0, 3), node.Span);
            Assert.AreEqual(new Position(0, 0), node.Position);
        }

        [TestMethod]
        public void ScanIdentifiesClassBlockStartAfterUnclosedDirective()
        {
            var node = (ClassBlockStart)Scan("<#@<#+").ElementAt(1);
            Assert.AreEqual(new Span(3, 3), node.Span);
            Assert.AreEqual(new Position(0, 3), node.Position);
        }

        [TestMethod]
        public void ScanIdentifiesClassBlockStartAfterBlockEnd()
        {
            var node = (ClassBlockStart)Scan("#><#+").ElementAt(1);
            Assert.AreEqual(new Span(2, 3), node.Span);
            Assert.AreEqual(new Position(0, 2), node.Position);
        }

        #endregion

        #region DirectiveBlockStart

        [TestMethod]
        public void ScanIdentifiesDirectiveBlockStart()
        {
            var node = (DirectiveBlockStart)Scan("<#@").First();
            Assert.AreEqual(new Span(0, 3), node.Span);
            Assert.AreEqual(new Position(0, 0), node.Position);
        }

        [TestMethod]
        public void ScanIdentifiesDirectiveBlockStartAfterUnclosedDirective()
        {
            var node = (DirectiveBlockStart)Scan("<#@<#@").ElementAt(1);
            Assert.AreEqual(new Span(3, 3), node.Span);
            Assert.AreEqual(new Position(0, 3), node.Position);
        }

        #endregion

        #region DirectiveName

        [TestMethod]
        public void ScanIdentifiesDirectiveName()
        {
            var node = (DirectiveName)Scan("<#@ directive").ElementAt(1);
            Assert.AreEqual(new Span(4, 9), node.Span);
            Assert.AreEqual(new Position(0, 4), node.Position);
        }

        [TestMethod]
        public void ScanIdentifiesDirectiveNameWithoutLeadingSpaces() // For consistency with T4 parsing logic
        {
            var node = (DirectiveName)Scan("<#@directive").ElementAt(1);
            Assert.AreEqual(new Span(3, 9), node.Span);
            Assert.AreEqual(new Position(0, 3), node.Position);
        }

        #endregion

        #region AttributeName

        [TestMethod]
        public void ScanIdentifiesAttributeNameFollowedByEquals()
        {
            var node = (AttributeName)Scan("<#@ attribute =").ElementAt(1);
            Assert.AreEqual(new Span(4, 9), node.Span);
            Assert.AreEqual(new Position(0, 4), node.Position);
        }

        [TestMethod]
        public void ScanIdentifiesAttributeNameWithoutValue() // to colorize partially valid directives
        {
            var node = (AttributeName)Scan("<#@ directive attribute").ElementAt(2);
            Assert.AreEqual(new Span(14, 9), node.Span);
            Assert.AreEqual(new Position(0, 14), node.Position);
        }

        #endregion

        #region DoubleQuote

        [TestMethod]
        public void ScanIdentifiesDoubleQuoteAfterDirectiveBlockStart()
        {
            var node = (DoubleQuote)Scan("<#@ \"").ElementAt(1);
            Assert.AreEqual(new Span(4, 1), node.Span);
            Assert.AreEqual(new Position(0, 4), node.Position);
        }

        [TestMethod]
        public void ScanIdentifiesDoubleQuoteAfterDirectiveName()
        {
            var node = (DoubleQuote)Scan("<#@ directive \"").ElementAt(2);
            Assert.AreEqual(new Span(14, 1), node.Span);
            Assert.AreEqual(new Position(0, 14), node.Position);
        }

        [TestMethod]
        public void ScanIdentifiesDoubleQuoteAfterAttributeName()
        {
            var node = (DoubleQuote)Scan("<#@ directive attribute = \"").ElementAt(4);
            Assert.AreEqual(new Span(26, 1), node.Span);
            Assert.AreEqual(new Position(0, 26), node.Position);
        }

        [TestMethod]
        public void ScanIdentifiesDoubleQuoteWithoutLeadingSpaces()
        {
            var node = (DoubleQuote)Scan("<#@ directive attribute =\"").ElementAt(4);
            Assert.AreEqual(new Span(25, 1), node.Span);
            Assert.AreEqual(new Position(0, 25), node.Position);
        }

        [TestMethod]
        public void ScanIdentifiesDoubleQuoteAfterAttributeValue()
        {
            var node = (DoubleQuote)Scan("<#@ directive attribute = \"value\"").ElementAt(6);
            Assert.AreEqual(new Span(32, 1), node.Span);
            Assert.AreEqual(new Position(0, 32), node.Position);
        }

        [TestMethod]
        public void ScanIdentifiesDoubleQuoteAfterAttribute()
        {
            var node = (DoubleQuote)Scan("<#@ \"value1\" \"").ElementAt(4);
            Assert.AreEqual(new Span(13, 1), node.Span);
            Assert.AreEqual(new Position(0, 13), node.Position);
        }

        #endregion

        #region Equals

        [TestMethod]
        public void ScanIdentifiesEqualsAfterDirectiveBlockStart()
        {
            var node = (Equals)Scan("<#@=").ElementAt(1);
            Assert.AreEqual(new Span(3, 1), node.Span);
            Assert.AreEqual(new Position(0, 3), node.Position);
        }

        [TestMethod]
        public void ScanIdentifiesEqualsAfterDirectiveName()
        {
            var node = (Equals)Scan("<#@ directive =").ElementAt(2);
            Assert.AreEqual(new Span(14, 1), node.Span);
            Assert.AreEqual(new Position(0, 14), node.Position);
        }

        [TestMethod]
        public void ScanIdentifiesEqualsAfterAttributeName()
        {
            var node = (Equals)Scan("<#@ directive attribute =").ElementAt(3);
            Assert.AreEqual(new Span(24, 1), node.Span);
            Assert.AreEqual(new Position(0, 24), node.Position);
        }

        [TestMethod]
        public void ScanIdentifiesEqualsWithoutLeadingSpaces()
        {
            var node = (Equals)Scan("<#@ directive attribute=").ElementAt(3);
            Assert.AreEqual(new Span(23, 1), node.Span);
            Assert.AreEqual(new Position(0, 23), node.Position);
        }

        #endregion

        #region AttributeValue

        [TestMethod]
        public void ScanIdentifiesAttributeValueFollowedByDoubleQuote()
        {
            var node = (AttributeValue)Scan("<#@ \"value\"").ElementAt(2);
            Assert.AreEqual(new Span(5, 5), node.Span);
            Assert.AreEqual(new Position(0, 5), node.Position);
        }

        [TestMethod]
        public void ScanIdentifiesAttributeValueFollowedByEndOfFile()
        {
            var node = (AttributeValue)Scan("<#@ \"value").ElementAt(2);
            Assert.AreEqual(new Span(5, 5), node.Span);
            Assert.AreEqual(new Position(0, 5), node.Position);
        }

        [TestMethod]
        public void ScanIdentifiesAttributeValueFollowedByBlockEnd()
        {
            var node = (AttributeValue)Scan("<#@ \"value#>").ElementAt(2);
            Assert.AreEqual(new Span(5, 5), node.Span);
            Assert.AreEqual(new Position(0, 5), node.Position);
        }

        [TestMethod]
        public void ScanIdentifiesAttributeValueFollowedByBlockStart()
        {
            var node = (AttributeValue)Scan("<#@ \"value<#").ElementAt(2);
            Assert.AreEqual(new Span(5, 5), node.Span);
            Assert.AreEqual(new Position(0, 5), node.Position);
        }

        [TestMethod]
        public void ScanIdentifiesMultilineAttributeValue()
        {
            const string Input = "<#@ \"line1 \r\n line2\" #>";
            var node = (AttributeValue)Scan(Input).ElementAt(2);
            Assert.AreEqual(
                Span.FromBounds(
                    Input.IndexOf("\"", System.StringComparison.Ordinal) + 1, 
                    Input.LastIndexOf("\"", System.StringComparison.Ordinal)), 
                node.Span);
            Assert.AreEqual(new Position(0, Input.IndexOf("\"", System.StringComparison.Ordinal) + 1), node.Position);
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
            Assert.AreEqual(new Span(input.Length, 0), node.Span);
            Assert.AreEqual(new Position(0, input.Length), node.Position); 
        }
    }
}