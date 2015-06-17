// <copyright file="SyntaxNodeVisitorTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.TemplateAnalysis
{
    using System.Linq;
    using Microsoft.VisualStudio.Text;
    using NSubstitute;
    using Xunit;

    public static class SyntaxNodeVisitorTest
    {
        [Fact]
        public static void SyntaxNodeVisitorIsInternalAndNotMeantForPublicConsumption()
        {
            Assert.False(typeof(SyntaxNodeVisitor).IsPublic);
        }

        [Fact]
        public static void SyntaxNodeVisitorIsAbstractBecauseItOnlyAcceptsVisitsAndDoesNotDoAnything()
        {
            Assert.True(typeof(SyntaxNodeVisitor).IsAbstract);
        }

        [Fact]
        public static void VisitCallsAcceptOfSyntaxNode()
        {
            var visitor = Substitute.For<SyntaxNodeVisitor>();
            var node = Substitute.For<SyntaxNode>();
            visitor.Visit(node);
            node.Received().Accept(visitor);
        }

        [Fact]
        public static void VisitAssemblyDirectiveCallsVisitDirectiveToAllowProcessingAllDirectivesPolymorphically()
        {
            var visitor = Substitute.ForPartsOf<SyntaxNodeVisitor>();
            var directive = new AssemblyDirective(new DirectiveBlockStart(0), new DirectiveName(0, "assembly"), Enumerable.Empty<Attribute>(), new BlockEnd(0));
            visitor.VisitAssemblyDirective(directive);
            visitor.Received().VisitDirective(directive);
            Assert.Equal(typeof(Directive), typeof(AssemblyDirective).BaseType);
        }

        [Fact]
        public static void VisitAttributeCallsVisitNonterminalNodeToAllowProcessingAllNonterminalNodesPolymorphically()
        {
            var visitor = Substitute.ForPartsOf<SyntaxNodeVisitor>();
            var attribute = new Attribute(new AttributeName(0, "attribute"), new Equals(0), new DoubleQuote(0), new AttributeValue(0, string.Empty), new DoubleQuote(0));
            visitor.VisitAttribute(attribute);
            visitor.Received().VisitNonterminalNode(attribute);
            Assert.Equal(typeof(NonterminalNode), typeof(Attribute).BaseType);
        }

        [Fact]
        public static void VisitAttributeNameCallsVisitCaptureNodeToAllowProcessingAllCaptureNodesPolymorphically()
        {
            var visitor = Substitute.ForPartsOf<SyntaxNodeVisitor>();
            var attributeName = new AttributeName(0, "attribute");
            visitor.VisitAttributeName(attributeName);
            visitor.Received().VisitCaptureNode(attributeName);
            Assert.Equal(typeof(CaptureNode), typeof(AttributeName).BaseType);
        }

        [Fact]
        public static void VisitAttributeValueCallsVisitCaptureNodeToAllowProcessingAllCaptureNodesPolymorphically()
        {
            var visitor = Substitute.ForPartsOf<SyntaxNodeVisitor>();
            var attributeValue = new AttributeValue(0, "value");
            visitor.VisitAttributeValue(attributeValue);
            visitor.Received().VisitCaptureNode(attributeValue);
            Assert.Equal(typeof(CaptureNode), typeof(AttributeValue).BaseType);
        }

        [Fact]
        public static void VisitBlockEndCallsVisitSyntaxTokenToAllowProcessingAllSyntaxTokensPolymorphically()
        {
            var visitor = Substitute.ForPartsOf<SyntaxNodeVisitor>();
            var blockEnd = new BlockEnd(0);
            visitor.VisitBlockEnd(blockEnd);
            visitor.Received().VisitSyntaxToken(blockEnd);
            Assert.Equal(typeof(SyntaxToken), typeof(BlockEnd).BaseType);
        }

        [Fact]
        public static void VisitCaptureNodeCallsVisitTerminalNodeToAllowProcessingAllTerminalNodesPolymorphically()
        {
            var visitor = Substitute.ForPartsOf<SyntaxNodeVisitor>();
            var captureNode = new AttributeName(0, "attribute");
            visitor.VisitCaptureNode(captureNode);
            visitor.Received().VisitTerminalNode(captureNode);
            Assert.Equal(typeof(TerminalNode), typeof(CaptureNode).BaseType);
        }

        [Fact]
        public static void VisitClassBlockStartCallsVisitCodeBlockStartToAllowProcessingAllCodeBlockStartsPolymorphically()
        {
            var visitor = Substitute.ForPartsOf<SyntaxNodeVisitor>();
            var classBlockStart = new ClassBlockStart(0);
            visitor.VisitClassBlockStart(classBlockStart);
            visitor.Received().VisitCodeBlockStart(classBlockStart);
            Assert.Equal(typeof(CodeBlockStart), typeof(ClassBlockStart).BaseType);
        }

        [Fact]
        public static void VisitCodeCallsVisitTerminalNodeToAllowProcessingAllTerminalNodesPolymorphically()
        {
            var visitor = Substitute.ForPartsOf<SyntaxNodeVisitor>();
            var code = new Code(default(Span));
            visitor.VisitCode(code);
            visitor.Received().VisitTerminalNode(code);
            Assert.Equal(typeof(TerminalNode), typeof(Code).BaseType);
        }

        [Fact]
        public static void VisitCodeBlockCallsVisitNonterminalNodeToAllowProcessingAllNonterminalNodesPolymorphically()
        {
            var visitor = Substitute.ForPartsOf<SyntaxNodeVisitor>();
            var codeBlock = new CodeBlock(new StatementBlockStart(0), new BlockEnd(0));
            visitor.VisitCodeBlock(codeBlock);
            visitor.Received().VisitNonterminalNode(codeBlock);
            Assert.Equal(typeof(NonterminalNode), typeof(CodeBlock).BaseType);
        }

        [Fact]
        public static void VisitCodeBlockStartCallsVisitSyntaxTokenToAllowProcessingAllSyntaxTokensPolymorphically()
        {
            var visitor = Substitute.ForPartsOf<SyntaxNodeVisitor>();
            var codeBlockStart = new StatementBlockStart(0);
            visitor.VisitCodeBlockStart(codeBlockStart);
            visitor.Received().VisitSyntaxToken(codeBlockStart);
            Assert.Equal(typeof(SyntaxToken), typeof(CodeBlockStart).BaseType);
        }

        [Fact]
        public static void VisitCustomDirectiveCallsVisitDirectiveToAllowProcessingAllDirectivesPolymorphically()
        {
            var visitor = Substitute.ForPartsOf<SyntaxNodeVisitor>();
            var customDirective = new CustomDirective(new DirectiveBlockStart(0), new DirectiveName(0, "custom"), Enumerable.Empty<Attribute>(), new BlockEnd(0));
            visitor.VisitCustomDirective(customDirective);
            visitor.Received().VisitDirective(customDirective);
            Assert.Equal(typeof(Directive), typeof(CustomDirective).BaseType);
        }

        [Fact]
        public static void VisitDirectiveCallsVisitNonterminalNodeToAllowProcessingAllNonterminalNodesPolymorphically()
        {
            var visitor = Substitute.ForPartsOf<SyntaxNodeVisitor>();
            var directive = new CustomDirective(new DirectiveBlockStart(0), new DirectiveName(0, "custom"), Enumerable.Empty<Attribute>(), new BlockEnd(0));
            visitor.VisitDirective(directive);
            visitor.Received().VisitNonterminalNode(directive);
            Assert.Equal(typeof(NonterminalNode), typeof(Directive).BaseType);
        }

        [Fact]
        public static void VisitDirectiveBlockStartCallsVisitSyntaxTokenToAllowProcessingAllSyntaxTokensPolymorphically()
        {
            var visitor = Substitute.ForPartsOf<SyntaxNodeVisitor>();
            var directiveBlockStart = new DirectiveBlockStart(0);
            visitor.VisitDirectiveBlockStart(directiveBlockStart);
            visitor.Received().VisitSyntaxToken(directiveBlockStart);
            Assert.Equal(typeof(SyntaxToken), typeof(DirectiveBlockStart).BaseType);
        }

        [Fact]
        public static void VisitDirectiveNameCallsVisitCaptureNodeToAllowProcessingAllCaptureNodesPolymorphically()
        {
            var visitor = Substitute.ForPartsOf<SyntaxNodeVisitor>();
            var directiveName = new DirectiveName(0, "directive");
            visitor.VisitDirectiveName(directiveName);
            visitor.Received().VisitCaptureNode(directiveName);
            Assert.Equal(typeof(CaptureNode), typeof(DirectiveName).BaseType);
        }

        [Fact]
        public static void VisitDoubleQuoteCallsVisitSyntaxTokenToAllowProcessingAllSyntaxTokensPolymorphically()
        {
            var visitor = Substitute.ForPartsOf<SyntaxNodeVisitor>();
            var doubleQuote = new DoubleQuote(0);
            visitor.VisitDoubleQuote(doubleQuote);
            visitor.Received().VisitSyntaxToken(doubleQuote);
            Assert.Equal(typeof(SyntaxToken), typeof(DoubleQuote).BaseType);
        }

        [Fact]
        public static void VisitEndOfFileCallsVisitSyntaxTokenToAllowProcessingAllSyntaxTokensPolymorphically()
        {
            var visitor = Substitute.ForPartsOf<SyntaxNodeVisitor>();
            var endOfFile = new EndOfFile(0);
            visitor.VisitEndOfFile(endOfFile);
            visitor.Received().VisitSyntaxToken(endOfFile);
            Assert.Equal(typeof(SyntaxToken), typeof(EndOfFile).BaseType);
        }

        [Fact]
        public static void VisitEqualsCallsVisitSyntaxTokenToAllowProcessingAllSyntaxTokensPolymorphically()
        {
            var visitor = Substitute.ForPartsOf<SyntaxNodeVisitor>();
            var equals = new Equals(0);
            visitor.VisitEquals(equals);
            visitor.Received().VisitSyntaxToken(equals);
            Assert.Equal(typeof(SyntaxToken), typeof(Equals).BaseType);
        }

        [Fact]
        public static void VisitExpressionBlockStartCallsVisitCodeBlockStartToAllowProcessingAllCodeBlockStartsPolymorphically()
        {
            var visitor = Substitute.ForPartsOf<SyntaxNodeVisitor>();
            var expressionBlockStart = new ExpressionBlockStart(0);
            visitor.VisitExpressionBlockStart(expressionBlockStart);
            visitor.Received().VisitCodeBlockStart(expressionBlockStart);
            Assert.Equal(typeof(CodeBlockStart), typeof(ExpressionBlockStart).BaseType);
        }

        [Fact]
        public static void VisitImportDirectiveCallsVisitDirectiveToAllowProcessingAllDirectivesPolymorphically()
        {
            var visitor = Substitute.ForPartsOf<SyntaxNodeVisitor>();
            var importDirective = new ImportDirective(new DirectiveBlockStart(0), new DirectiveName(0, "import"), Enumerable.Empty<Attribute>(), new BlockEnd(0));
            visitor.VisitImportDirective(importDirective);
            visitor.Received().VisitDirective(importDirective);
            Assert.Equal(typeof(Directive), typeof(ImportDirective).BaseType);
        }

        [Fact]
        public static void VisitIncludeDirectiveCallsVisitDirectiveToAllowProcessingAllDirectivesPolymorphically()
        {
            var visitor = Substitute.ForPartsOf<SyntaxNodeVisitor>();
            var includeDirective = new IncludeDirective(new DirectiveBlockStart(0), new DirectiveName(0, "include"), Enumerable.Empty<Attribute>(), new BlockEnd(0));
            visitor.VisitIncludeDirective(includeDirective);
            visitor.Received().VisitDirective(includeDirective);
            Assert.Equal(typeof(Directive), typeof(IncludeDirective).BaseType);
        }

        [Fact]
        public static void VisitNonterminalNodeCallsVisitSyntaxNodeToAllowProcessingAllSyntaxNodesPolymorphically()
        {
            var visitor = Substitute.ForPartsOf<SyntaxNodeVisitor>();
            var nonterminalNode = new CodeBlock(new StatementBlockStart(0), new BlockEnd(0));
            visitor.VisitNonterminalNode(nonterminalNode);
            Assert.Equal(1, visitor.ReceivedCalls().Count(call => call.GetMethodInfo().Name == "VisitSyntaxNode" && call.GetArguments()[0] == nonterminalNode));
            Assert.Equal(typeof(SyntaxNode), typeof(NonterminalNode).BaseType);
        }

        [Fact]
        public static void VisitNonterminalNodeVisitsChildNodesToAllowProcessingAllNodesRecursively()
        {
            var visitor = Substitute.ForPartsOf<SyntaxNodeVisitor>();
            var nonterminalNode = new CodeBlock(new StatementBlockStart(0), new BlockEnd(0));
            visitor.VisitNonterminalNode(nonterminalNode);
            Assert.Equal(3, visitor.ReceivedCalls().Count(call => call.GetMethodInfo().Name == "VisitSyntaxNode"));
        }

        [Fact]
        public static void VisitOutputDirectiveCallsVisitDirectiveToAllowProcessingAllDirectivesPolymorphically()
        {
            var visitor = Substitute.ForPartsOf<SyntaxNodeVisitor>();
            var outputDirective = new OutputDirective(new DirectiveBlockStart(0), new DirectiveName(0, "output"), Enumerable.Empty<Attribute>(), new BlockEnd(0));
            visitor.VisitOutputDirective(outputDirective);
            visitor.Received().VisitDirective(outputDirective);            
            Assert.Equal(typeof(Directive), typeof(OutputDirective).BaseType);
        }

        [Fact]
        public static void VisitParameterDirectiveCallsVisitDirectiveToAllowProcessingAllDirectivesPolymorphically()
        {
            var visitor = Substitute.ForPartsOf<SyntaxNodeVisitor>();
            var parameterDirective = new ParameterDirective(new DirectiveBlockStart(0), new DirectiveName(0, "parameter"), Enumerable.Empty<Attribute>(), new BlockEnd(0));
            visitor.VisitParameterDirective(parameterDirective);
            visitor.Received().VisitDirective(parameterDirective);            
            Assert.Equal(typeof(Directive), typeof(ParameterDirective).BaseType);
        }

        [Fact]
        public static void VisitStatementBlockStartCallsVisitCodeBlockStartToAllowProcessingAllCodeBlockStartsPolymorphically()
        {
            var visitor = Substitute.ForPartsOf<SyntaxNodeVisitor>();
            var statementBlockStart = new StatementBlockStart(0);
            visitor.VisitStatementBlockStart(statementBlockStart);
            visitor.Received().VisitCodeBlockStart(statementBlockStart);
            Assert.Equal(typeof(CodeBlockStart), typeof(StatementBlockStart).BaseType);
        }

        [Fact]
        public static void VisitSyntaxTokenCallsVisitTerminalNodeToAllowProcessingAllTerminalNodesPolymorphically()
        {
            var visitor = Substitute.ForPartsOf<SyntaxNodeVisitor>();
            var syntaxToken = new StatementBlockStart(0);
            visitor.VisitSyntaxToken(syntaxToken);
            visitor.Received().VisitTerminalNode(syntaxToken);
            Assert.Equal(typeof(TerminalNode), typeof(SyntaxToken).BaseType);
        }

        [Fact]
        public static void VisitTemplateCallsVisitNonterminalNodeToAllowProcessingAllNonterminalNodesPolymorphically()
        {
            var visitor = Substitute.ForPartsOf<SyntaxNodeVisitor>();
            var template = new Template(new StatementBlockStart(0), new BlockEnd(0));
            visitor.VisitTemplate(template);
            visitor.Received().VisitNonterminalNode(template);
            Assert.Equal(typeof(NonterminalNode), typeof(Template).BaseType);
        }

        [Fact]
        public static void VisitTemplateDirectiveCallsVisitDirectiveToAllowProcessingAllDirectivesPolymorphically()
        {
            var visitor = Substitute.ForPartsOf<SyntaxNodeVisitor>();
            var templateDirective = new TemplateDirective(new DirectiveBlockStart(0), new DirectiveName(0, "template"), Enumerable.Empty<Attribute>(), new BlockEnd(0));
            visitor.VisitTemplateDirective(templateDirective);
            visitor.Received().VisitDirective(templateDirective);
            Assert.Equal(typeof(Directive), typeof(TemplateDirective).BaseType);
        }

        [Fact]
        public static void VisitTerminalNodeCallsVisitSyntaxNodeToAllowProcessingAllSyntaxNodesPolymorphically()
        {
            var visitor = Substitute.ForPartsOf<SyntaxNodeVisitor>();
            var terminalNode = new BlockEnd(0);
            visitor.VisitTerminalNode(terminalNode);
            Assert.Equal(1, visitor.ReceivedCalls().Count(call => call.GetMethodInfo().Name == "VisitSyntaxNode"));
            Assert.Equal(typeof(SyntaxNode), typeof(TerminalNode).BaseType);
        }
    }
}