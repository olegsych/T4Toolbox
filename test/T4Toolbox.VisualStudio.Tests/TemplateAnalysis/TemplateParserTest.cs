// <copyright file="TemplateParserTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.TemplateAnalysis
{
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.VisualStudio.Text;

    [TestClass]
    public class TemplateParserTest
    {
        [TestMethod]
        public void ParseReturnsTrueGivenEmptyTemplate()
        {
            AssertParseSuccess(string.Empty);
        }

        [TestMethod]
        public void ParseCreatesTemplateSyntaxNode()
        {
            TemplateParser parser = CreateParser(string.Empty);
            parser.Parse();
            Assert.AreEqual(new Template(), parser.Template);
        }

        [TestMethod]
        public void ParseReturnsTrueGivenMultipleCodeBlocks()
        {
            AssertParseSuccess(
                "<#+ private int meaningOfLife; #>" +
                "<# this.meaningOfLife = 42; #>" +
                "<#= this.meaningOfLife #>");
        }

        [TestMethod]
        public void ParseReportsErrorForMissingBlockStartInSingleEmptyCodeBlock()
        {
            AssertParseErrors("#>", new TemplateError("<#, <#=, <#+ or <#@ expected", new Span(0, 2), new Position(0, 0)));
        }

        [TestMethod]
        public void ParseReportsErrorsForMissingBlockStartsInMultipleEmptyCodeBlocks()
        {
            AssertParseErrors(
                "#>#>", 
                new TemplateError("<#, <#=, <#+ or <#@ expected", new Span(0, 2), new Position(0, 0)),
                new TemplateError("<#, <#=, <#+ or <#@ expected", new Span(2, 2), new Position(0, 2)));
        }

        [TestMethod]
        public void ParseReportsErrorForMissingBlockStartInNonemptyCodeBlock()
        {
            AssertParseErrors("text #>", new TemplateError("<#, <#=, <#+ or <#@ expected", new Span(5, 2), new Position(0, 5)));
        }

        #region Statement Blocks

        [TestMethod]
        public void ParseReturnsTrueGivenEmptyStatementBlock()
        {
            AssertParseSuccess("<##>");
        }

        [TestMethod]
        public void ParseCreatesCodeBlockSyntaxNodeForEmptyStatementBlock()
        {
            TemplateParser parser = CreateParser("<##>");
            parser.Parse();
            Assert.AreEqual(typeof(CodeBlock), parser.Template.ChildNodes().Single().GetType());
        }

        [TestMethod]
        public void ParseReturnsTrueGivenNonemptyStatementBlock()
        {
            AssertParseSuccess("<# Statement(); #>");
        }

        [TestMethod]
        public void ParseCreatesCodeBlockSyntaxNodeForNonemptyStatementBlock()
        {
            TemplateParser parser = CreateParser("<# Statement(); #>");
            parser.Parse();
            Assert.AreEqual(typeof(CodeBlock), parser.Template.ChildNodes().Single().GetType());
        }

        [TestMethod]
        public void ParseReportsErrorForUnclosedEmptyStatementBlockAtTheEndOfFile()
        {
            AssertParseErrors("<#", new TemplateError("code or #> expected", new Span(2, 0), new Position(0, 2)));
        }

        [TestMethod]
        public void ParseReportsErrorForUnclosedNonemptyStatementBlockAtTheEndOfFile()
        {
            AssertParseErrors("<# Statement();", new TemplateError("#> expected", new Span(15, 0), new Position(0, 15)));
        }

        [TestMethod]
        public void ParseReportsErrorForUnclosedEmptyStatementBlockFollowedByValidBlock()
        {
            AssertParseErrors("<#<##>", new TemplateError("code or #> expected", new Span(2, 2), new Position(0, 2)));
        }

        [TestMethod]
        public void ParseReportsErrorForUnclosedNonemptyStatementBlockFollowedByValidBlock()
        {
            AssertParseErrors("<# Statement(); <##>", new TemplateError("#> expected", new Span(16, 2), new Position(0, 16)));
        }

        #endregion

        #region Expression Blocks

        [TestMethod]
        public void ParseReturnsTrueGivenEmptyExpressionBlock()
        {
            AssertParseSuccess("<#=#>");
        }

        [TestMethod]
        public void ParseReturnsTrueGivenNonemptyExpressionBlock()
        {
            AssertParseSuccess("<#= expression #>");
        }

        [TestMethod]
        public void ParseReportsErrorForUnclosedEmptyExpressionBlockAtTheEndOfFile()
        {
            AssertParseErrors("<#=", new TemplateError("code or #> expected", new Span(3, 0), new Position(0, 3)));
        }

        [TestMethod]
        public void ParseReportsErrorForUnclosedNonemptyExpressionBlockAtTheEndOfFile()
        {
            AssertParseErrors("<#= expression", new TemplateError("#> expected", new Span(14, 0), new Position(0, 14)));
        }

        [TestMethod]
        public void ParseReportsErrorForUnclosedEmptyExpressionBlockFollowedByValidBlock()
        {
            AssertParseErrors("<#=<##>", new TemplateError("code or #> expected", new Span(3, 2), new Position(0, 3)));
        }

        [TestMethod]
        public void ParseReportsErrorForUnclosedNonemptyExpressionBlockFollowedByValidBlock()
        {
            AssertParseErrors("<#= expression <##>", new TemplateError("#> expected", new Span(15, 2), new Position(0, 15)));
        }

        #endregion

        #region Class Feature Blocks

        [TestMethod]
        public void ParseReturnsTrueGivenEmptyClassFeatureBlock()
        {
            AssertParseSuccess("<#+#>");
        }

        [TestMethod]
        public void ParseReturnsTrueGivenNonemptyClassFeatureBlock()
        {
            AssertParseSuccess("<#+ private int field; #>");
        }

        [TestMethod]
        public void ParseReportsErrorForUnclosedEmptyClassFeatureBlockAtTheEndOfFile()
        {
            AssertParseErrors("<#+", new TemplateError("code or #> expected", new Span(3, 0), new Position(0, 3)));
        }

        [TestMethod]
        public void ParseReportsErrorForUnclosedNonemptyClassFeatureBlockAtTheEndOfFile()
        {
            AssertParseErrors("<#+ private int field;", new TemplateError("#> expected", new Span(22, 0), new Position(0, 22)));
        }

        [TestMethod]
        public void ParseReportsErrorForUnclosedEmptyClassFeatureBlockFollowedByValidBlock()
        {
            AssertParseErrors("<#+<##>", new TemplateError("code or #> expected", new Span(3, 2), new Position(0, 3)));
        }

        [TestMethod]
        public void ParseReportsErrorForUnclosedNonemptyClassBlocksFollowedByValidBlock()
        {
            AssertParseErrors("<#+ private int field1; <##>", new TemplateError("#> expected", new Span(24, 2), new Position(0, 24)));
        }

        #endregion

        #region Directives

        [TestMethod]
        public void ParserReportsNoErrorsForDirectiveWithoutMandatoryAttributes()
        {
            AssertParseSuccess("<#@ template #>");
        }

        [TestMethod]
        public void ParseReportsNoErrorsForDirectiveWithSingleAttribute()
        {
            AssertParseSuccess("<#@ template language=\"C#\" #>");            
        }

        [TestMethod]
        public void ParseCreatesDirectiveSyntaxNodeForDirectiveWithSingleAttribute()
        {
            TemplateParser parser = CreateParser("<#@ template language=\"C#\" #>");
            parser.Parse();
            Assert.AreEqual(1, parser.Template.ChildNodes().Cast<Directive>().Single().Attributes.Count);
        }

        [TestMethod]
        public void ParseReportsNoErrorsForDirectiveWithMultipleAttributes()
        {
            AssertParseSuccess("<#@ template language=\"C#\" debug=\"True\" #>");
        }

        [TestMethod]
        public void ParseCreatesDirectiveSyntaxNodeForDirectiveWithMultipleAttributes()
        {
            TemplateParser parser = CreateParser("<#@ template language=\"C#\" debug=\"True\" #>");
            parser.Parse();
            Assert.AreEqual(2, parser.Template.ChildNodes().Cast<Directive>().Single().Attributes.Count);
        }

        [TestMethod]
        public void ParseReportsErrorsForUnclosedDirectiveAtTheEndOfFile()
        {
            AssertParseErrors("<#@ template", new TemplateError("#> or attribute name expected", new Span(12, 0), new Position(0, 12)));
        }

        [TestMethod]
        public void ParseReportsErrorsForMissingEqualsSignAtTheEndOfFile()
        {
            AssertParseErrors("<#@ template language", new TemplateError("= expected", new Span(21, 0), new Position(0, 21)));            
        }

        [TestMethod]
        public void ParseReportsErrorsForMissingDoubleQuoteAfterEqualsAtTheEndOfFile()
        {
            AssertParseErrors("<#@ template language=", new TemplateError("\" expected", new Span(22, 0), new Position(0, 22)));
        }

        [TestMethod]
        public void ParseReportsErrorsForMissingAttributeValueAfterDoubleQuoteAtTheEndOfFile()
        {
            AssertParseErrors("<#@ template language=\"", new TemplateError("attribute value expected", new Span(23, 0), new Position(0, 23)));                                    
        }

        [TestMethod]
        public void ParseReportsErrorsForUnclosedAttributeAtTheEndOfFile()
        {
            AssertParseErrors("<#@ template language=\"C#", new TemplateError("\" expected", new Span(25, 0), new Position(0, 25)));            
        }

        [TestMethod]
        public void ParseReportsErrorsForUnclosedAttributeAtTheEndOfDirective()
        {
            AssertParseErrors("<#@ template language=\"C# #>", new TemplateError("\" expected", new Span(26, 2), new Position(0, 26)));                        
        }

        [TestMethod]
        public void ParseReportsErrorsForUnclosedDirectiveFollowedByValidBlock()
        {
            AssertParseErrors("<#@ template language=\"C#\"<##>", new TemplateError("#> or attribute name expected", new Span(26, 2), new Position(0, 26)));
        }

        [TestMethod]
        public void ParserRecognizesAssemblyDirective()
        {
            TemplateParser parser = CreateParser("<#@ assembly name=\"Life.Universe.Everything\" #>");
            parser.Parse();
            SyntaxNode directive = parser.Template.ChildNodes().Single();
            Assert.AreEqual(typeof(AssemblyDirective), directive.GetType());
        }

        [TestMethod]
        public void ParserRecognizesCustomDirective()
        {
            TemplateParser parser = CreateParser("<#@ custom processor=\"CustomProcessor\" #>");
            parser.Parse();
            SyntaxNode directive = parser.Template.ChildNodes().Single();
            Assert.AreEqual(typeof(CustomDirective), directive.GetType());
        }

        [TestMethod]
        public void ParserRecognizesImportDirective()
        {
            TemplateParser parser = CreateParser("<#@ import namespace=\"Life.Universe.Everything\" #>");
            parser.Parse();
            SyntaxNode directive = parser.Template.ChildNodes().Single();
            Assert.AreEqual(typeof(ImportDirective), directive.GetType());
        }

        [TestMethod]
        public void ParseRecognizesIncludeDirective()
        {
            TemplateParser parser = CreateParser("<#@ include file=\"test.tt\" #>");
            parser.Parse();
            SyntaxNode directive = parser.Template.ChildNodes().Single();
            Assert.AreEqual(typeof(IncludeDirective), directive.GetType());
        }

        [TestMethod]
        public void ParseRecognizesOutputDirective()
        {
            TemplateParser parser = CreateParser("<#@ output extension=\".txt\" #>");
            parser.Parse();
            SyntaxNode directive = parser.Template.ChildNodes().Single();
            Assert.AreEqual(typeof(OutputDirective), directive.GetType());
        }
       
        [TestMethod]
        public void ParseRecognizesParameterDirective()
        {
            TemplateParser parser = CreateParser("<#@ parameter name=\"p1\" type=\"System.Int32\" #>");
            parser.Parse();
            SyntaxNode directive = parser.Template.ChildNodes().Single();
            Assert.AreEqual(typeof(ParameterDirective), directive.GetType());
        }

        [TestMethod]
        public void ParseRecognizesTemplateDirective()
        {
            TemplateParser parser = CreateParser("<#@ template language=\"C#\" #>");
            parser.Parse();
            SyntaxNode directive = parser.Template.ChildNodes().Single();
            Assert.AreEqual(typeof(TemplateDirective), directive.GetType());
        }
                
        #endregion

        private static TemplateParser CreateParser(string input)
        {
            var scanner = new TemplateScanner(input);
            return new TemplateParser(scanner);            
        }

        private static void AssertParseSuccess(string input)
        {
            TemplateParser parser = CreateParser(input);
            bool success = parser.Parse();
            string errorMessages = string.Join(";", parser.Errors.Select(error => error.Message));
            Assert.IsTrue(success, errorMessages);
            Assert.AreEqual(0, parser.Errors.Count, errorMessages);
        }

        private static void AssertParseErrors(string input, params TemplateError[] errors)
        {
            TemplateParser parser = CreateParser(input);
            parser.Parse();
            Assert.AreEqual(errors.Length, parser.Errors.Count);
            for (int i = 0; i < errors.Length; i++)
            {
                TemplateError expected = errors[i];
                TemplateError actual = parser.Errors[i];
                Assert.AreEqual(expected.Message, actual.Message);
                Assert.AreEqual(expected.Span, actual.Span);                
                Assert.AreEqual(expected.Position, actual.Position);
            }
        }
    }
}