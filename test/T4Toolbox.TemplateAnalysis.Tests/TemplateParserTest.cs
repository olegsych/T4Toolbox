// <copyright file="TemplateParserTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.TemplateAnalysis
{
    using System.Linq;
    using Microsoft.VisualStudio.Text;
    using Xunit;

    public static class TemplateParserTest
    {
        [Fact]
        public static void ParseReturnsTrueGivenEmptyTemplate()
        {
            AssertParseSuccess(string.Empty);
        }

        [Fact]
        public static void ParseCreatesTemplateSyntaxNode()
        {
            TemplateParser parser = CreateParser(string.Empty);
            parser.Parse();
            Assert.Equal(new Template(), parser.Template);
        }

        [Fact]
        public static void ParseReturnsTrueGivenMultipleCodeBlocks()
        {
            AssertParseSuccess(
                "<#+ private int meaningOfLife; #>" +
                "<# this.meaningOfLife = 42; #>" +
                "<#= this.meaningOfLife #>");
        }

        [Fact]
        public static void ParseReportsErrorForMissingBlockStartInSingleEmptyCodeBlock()
        {
            AssertParseErrors("#>", new TemplateError("<#, <#=, <#+ or <#@ expected", new Span(0, 2), new Position(0, 0)));
        }

        [Fact]
        public static void ParseReportsErrorsForMissingBlockStartsInMultipleEmptyCodeBlocks()
        {
            AssertParseErrors(
                "#>#>", 
                new TemplateError("<#, <#=, <#+ or <#@ expected", new Span(0, 2), new Position(0, 0)),
                new TemplateError("<#, <#=, <#+ or <#@ expected", new Span(2, 2), new Position(0, 2)));
        }

        [Fact]
        public static void ParseReportsErrorForMissingBlockStartInNonemptyCodeBlock()
        {
            AssertParseErrors("text #>", new TemplateError("<#, <#=, <#+ or <#@ expected", new Span(5, 2), new Position(0, 5)));
        }

        #region Statement Blocks

        [Fact]
        public static void ParseReturnsTrueGivenEmptyStatementBlock()
        {
            AssertParseSuccess("<##>");
        }

        [Fact]
        public static void ParseCreatesCodeBlockSyntaxNodeForEmptyStatementBlock()
        {
            TemplateParser parser = CreateParser("<##>");
            parser.Parse();
            Assert.Equal(typeof(CodeBlock), parser.Template.ChildNodes().Single().GetType());
        }

        [Fact]
        public static void ParseReturnsTrueGivenNonemptyStatementBlock()
        {
            AssertParseSuccess("<# Statement(); #>");
        }

        [Fact]
        public static void ParseCreatesCodeBlockSyntaxNodeForNonemptyStatementBlock()
        {
            TemplateParser parser = CreateParser("<# Statement(); #>");
            parser.Parse();
            Assert.Equal(typeof(CodeBlock), parser.Template.ChildNodes().Single().GetType());
        }

        [Fact]
        public static void ParseReportsErrorForUnclosedEmptyStatementBlockAtTheEndOfFile()
        {
            AssertParseErrors("<#", new TemplateError("code or #> expected", new Span(2, 0), new Position(0, 2)));
        }

        [Fact]
        public static void ParseReportsErrorForUnclosedNonemptyStatementBlockAtTheEndOfFile()
        {
            AssertParseErrors("<# Statement();", new TemplateError("#> expected", new Span(15, 0), new Position(0, 15)));
        }

        [Fact]
        public static void ParseReportsErrorForUnclosedEmptyStatementBlockFollowedByValidBlock()
        {
            AssertParseErrors("<#<##>", new TemplateError("code or #> expected", new Span(2, 2), new Position(0, 2)));
        }

        [Fact]
        public static void ParseReportsErrorForUnclosedNonemptyStatementBlockFollowedByValidBlock()
        {
            AssertParseErrors("<# Statement(); <##>", new TemplateError("#> expected", new Span(16, 2), new Position(0, 16)));
        }

        #endregion

        #region Expression Blocks

        [Fact]
        public static void ParseReturnsTrueGivenEmptyExpressionBlock()
        {
            AssertParseSuccess("<#=#>");
        }

        [Fact]
        public static void ParseReturnsTrueGivenNonemptyExpressionBlock()
        {
            AssertParseSuccess("<#= expression #>");
        }

        [Fact]
        public static void ParseReportsErrorForUnclosedEmptyExpressionBlockAtTheEndOfFile()
        {
            AssertParseErrors("<#=", new TemplateError("code or #> expected", new Span(3, 0), new Position(0, 3)));
        }

        [Fact]
        public static void ParseReportsErrorForUnclosedNonemptyExpressionBlockAtTheEndOfFile()
        {
            AssertParseErrors("<#= expression", new TemplateError("#> expected", new Span(14, 0), new Position(0, 14)));
        }

        [Fact]
        public static void ParseReportsErrorForUnclosedEmptyExpressionBlockFollowedByValidBlock()
        {
            AssertParseErrors("<#=<##>", new TemplateError("code or #> expected", new Span(3, 2), new Position(0, 3)));
        }

        [Fact]
        public static void ParseReportsErrorForUnclosedNonemptyExpressionBlockFollowedByValidBlock()
        {
            AssertParseErrors("<#= expression <##>", new TemplateError("#> expected", new Span(15, 2), new Position(0, 15)));
        }

        #endregion

        #region Class Feature Blocks

        [Fact]
        public static void ParseReturnsTrueGivenEmptyClassFeatureBlock()
        {
            AssertParseSuccess("<#+#>");
        }

        [Fact]
        public static void ParseReturnsTrueGivenNonemptyClassFeatureBlock()
        {
            AssertParseSuccess("<#+ private int field; #>");
        }

        [Fact]
        public static void ParseReportsErrorForUnclosedEmptyClassFeatureBlockAtTheEndOfFile()
        {
            AssertParseErrors("<#+", new TemplateError("code or #> expected", new Span(3, 0), new Position(0, 3)));
        }

        [Fact]
        public static void ParseReportsErrorForUnclosedNonemptyClassFeatureBlockAtTheEndOfFile()
        {
            AssertParseErrors("<#+ private int field;", new TemplateError("#> expected", new Span(22, 0), new Position(0, 22)));
        }

        [Fact]
        public static void ParseReportsErrorForUnclosedEmptyClassFeatureBlockFollowedByValidBlock()
        {
            AssertParseErrors("<#+<##>", new TemplateError("code or #> expected", new Span(3, 2), new Position(0, 3)));
        }

        [Fact]
        public static void ParseReportsErrorForUnclosedNonemptyClassBlocksFollowedByValidBlock()
        {
            AssertParseErrors("<#+ private int field1; <##>", new TemplateError("#> expected", new Span(24, 2), new Position(0, 24)));
        }

        #endregion

        #region Directives

        [Fact]
        public static void ParserReportsNoErrorsForDirectiveWithoutMandatoryAttributes()
        {
            AssertParseSuccess("<#@ template #>");
        }

        [Fact]
        public static void ParseReportsNoErrorsForDirectiveWithSingleAttribute()
        {
            AssertParseSuccess("<#@ template language=\"C#\" #>");            
        }

        [Fact]
        public static void ParseCreatesDirectiveSyntaxNodeForDirectiveWithSingleAttribute()
        {
            TemplateParser parser = CreateParser("<#@ template language=\"C#\" #>");
            parser.Parse();
            Assert.Equal(1, parser.Template.ChildNodes().Cast<Directive>().Single().Attributes.Count);
        }

        [Fact]
        public static void ParseReportsNoErrorsForDirectiveWithMultipleAttributes()
        {
            AssertParseSuccess("<#@ template language=\"C#\" debug=\"True\" #>");
        }

        [Fact]
        public static void ParseCreatesDirectiveSyntaxNodeForDirectiveWithMultipleAttributes()
        {
            TemplateParser parser = CreateParser("<#@ template language=\"C#\" debug=\"True\" #>");
            parser.Parse();
            Assert.Equal(2, parser.Template.ChildNodes().Cast<Directive>().Single().Attributes.Count);
        }

        [Fact]
        public static void ParseReportsErrorsForUnclosedDirectiveAtTheEndOfFile()
        {
            AssertParseErrors("<#@ template", new TemplateError("#> or attribute name expected", new Span(12, 0), new Position(0, 12)));
        }

        [Fact]
        public static void ParseReportsErrorsForMissingEqualsSignAtTheEndOfFile()
        {
            AssertParseErrors("<#@ template language", new TemplateError("= expected", new Span(21, 0), new Position(0, 21)));            
        }

        [Fact]
        public static void ParseReportsErrorsForMissingDoubleQuoteAfterEqualsAtTheEndOfFile()
        {
            AssertParseErrors("<#@ template language=", new TemplateError("\" expected", new Span(22, 0), new Position(0, 22)));
        }

        [Fact]
        public static void ParseReportsErrorsForMissingAttributeValueAfterDoubleQuoteAtTheEndOfFile()
        {
            AssertParseErrors("<#@ template language=\"", new TemplateError("attribute value expected", new Span(23, 0), new Position(0, 23)));                                    
        }

        [Fact]
        public static void ParseReportsErrorsForUnclosedAttributeAtTheEndOfFile()
        {
            AssertParseErrors("<#@ template language=\"C#", new TemplateError("\" expected", new Span(25, 0), new Position(0, 25)));            
        }

        [Fact]
        public static void ParseReportsErrorsForUnclosedAttributeAtTheEndOfDirective()
        {
            AssertParseErrors("<#@ template language=\"C# #>", new TemplateError("\" expected", new Span(26, 2), new Position(0, 26)));                        
        }

        [Fact]
        public static void ParseReportsErrorsForUnclosedDirectiveFollowedByValidBlock()
        {
            AssertParseErrors("<#@ template language=\"C#\"<##>", new TemplateError("#> or attribute name expected", new Span(26, 2), new Position(0, 26)));
        }

        [Fact]
        public static void ParserRecognizesAssemblyDirective()
        {
            TemplateParser parser = CreateParser("<#@ assembly name=\"Life.Universe.Everything\" #>");
            parser.Parse();
            SyntaxNode directive = parser.Template.ChildNodes().Single();
            Assert.Equal(typeof(AssemblyDirective), directive.GetType());
        }

        [Fact]
        public static void ParserRecognizesCustomDirective()
        {
            TemplateParser parser = CreateParser("<#@ custom processor=\"CustomProcessor\" #>");
            parser.Parse();
            SyntaxNode directive = parser.Template.ChildNodes().Single();
            Assert.Equal(typeof(CustomDirective), directive.GetType());
        }

        [Fact]
        public static void ParserRecognizesImportDirective()
        {
            TemplateParser parser = CreateParser("<#@ import namespace=\"Life.Universe.Everything\" #>");
            parser.Parse();
            SyntaxNode directive = parser.Template.ChildNodes().Single();
            Assert.Equal(typeof(ImportDirective), directive.GetType());
        }

        [Fact]
        public static void ParseRecognizesIncludeDirective()
        {
            TemplateParser parser = CreateParser("<#@ include file=\"test.tt\" #>");
            parser.Parse();
            SyntaxNode directive = parser.Template.ChildNodes().Single();
            Assert.Equal(typeof(IncludeDirective), directive.GetType());
        }

        [Fact]
        public static void ParseRecognizesOutputDirective()
        {
            TemplateParser parser = CreateParser("<#@ output extension=\".txt\" #>");
            parser.Parse();
            SyntaxNode directive = parser.Template.ChildNodes().Single();
            Assert.Equal(typeof(OutputDirective), directive.GetType());
        }
       
        [Fact]
        public static void ParseRecognizesParameterDirective()
        {
            TemplateParser parser = CreateParser("<#@ parameter name=\"p1\" type=\"System.Int32\" #>");
            parser.Parse();
            SyntaxNode directive = parser.Template.ChildNodes().Single();
            Assert.Equal(typeof(ParameterDirective), directive.GetType());
        }

        [Fact]
        public static void ParseRecognizesTemplateDirective()
        {
            TemplateParser parser = CreateParser("<#@ template language=\"C#\" #>");
            parser.Parse();
            SyntaxNode directive = parser.Template.ChildNodes().Single();
            Assert.Equal(typeof(TemplateDirective), directive.GetType());
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
            Assert.True(success, errorMessages);
            Assert.True(parser.Errors.Count == 0, errorMessages);
        }

        private static void AssertParseErrors(string input, params TemplateError[] errors)
        {
            TemplateParser parser = CreateParser(input);
            parser.Parse();
            Assert.Equal(errors.Length, parser.Errors.Count);
            for (int i = 0; i < errors.Length; i++)
            {
                TemplateError expected = errors[i];
                TemplateError actual = parser.Errors[i];
                Assert.Equal(expected.Message, actual.Message);
                Assert.Equal(expected.Span, actual.Span);                
                Assert.Equal(expected.Position, actual.Position);
            }
        }
    }
}