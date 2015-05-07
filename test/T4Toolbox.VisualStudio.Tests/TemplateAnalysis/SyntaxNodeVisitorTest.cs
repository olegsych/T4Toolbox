// <copyright file="SyntaxNodeVisitorTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.TemplateAnalysis
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.VisualStudio.Text;
    using NSubstitute;

    [TestClass]
    public class SyntaxNodeVisitorTest
    {
        [TestMethod]
        public void SyntaxNodeVisitorIsInternalAndNotMeantForPublicConsumption()
        {
            Assert.IsFalse(typeof(SyntaxNodeVisitor).IsPublic);
        }

        [TestMethod]
        public void SyntaxNodeVisitorIsAbstractBecauseItOnlyAcceptsVisitsAndDoesNotDoAnything()
        {
            Assert.IsTrue(typeof(SyntaxNodeVisitor).IsAbstract);
        }

        [TestMethod]
        public void VisitCallsAcceptOfSyntaxNode()
        {
            var visitor = Substitute.For<SyntaxNodeVisitor>();
            var node = Substitute.For<SyntaxNode>();
            visitor.Visit(node);
            node.Received().Accept(visitor);
        }

        [TestMethod]
        public void VisitAssemblyDirectiveCallsVisitDirectiveToAllowProcessingAllDirectivesPolymorphically()
        {
            var visitor = Substitute.ForPartsOf<SyntaxNodeVisitor>();
            var directive = new AssemblyDirective(new DirectiveBlockStart(0), new DirectiveName(0, "assembly"), Enumerable.Empty<Attribute>(), new BlockEnd(0));
            visitor.VisitAssemblyDirective(directive);
            visitor.Received().VisitDirective(directive);
            Assert.AreEqual(typeof(Directive), typeof(AssemblyDirective).BaseType);
        }

        [TestMethod]
        public void VisitAttributeCallsVisitNonterminalNodeToAllowProcessingAllNonterminalNodesPolymorphically()
        {
            var visitor = Substitute.ForPartsOf<SyntaxNodeVisitor>();
            var attribute = new Attribute(new AttributeName(0, "attribute"), new Equals(0), new DoubleQuote(0), new AttributeValue(0, string.Empty), new DoubleQuote(0));
            visitor.VisitAttribute(attribute);
            visitor.Received().VisitNonterminalNode(attribute);
            Assert.AreEqual(typeof(NonterminalNode), typeof(Attribute).BaseType);
        }

        [TestMethod]
        public void VisitAttributeNameCallsVisitCaptureNodeToAllowProcessingAllCaptureNodesPolymorphically()
        {
            var visitor = Substitute.ForPartsOf<SyntaxNodeVisitor>();
            var attributeName = new AttributeName(0, "attribute");
            visitor.VisitAttributeName(attributeName);
            visitor.Received().VisitCaptureNode(attributeName);
            Assert.AreEqual(typeof(CaptureNode), typeof(AttributeName).BaseType);
        }

        [TestMethod]
        public void VisitAttributeValueCallsVisitCaptureNodeToAllowProcessingAllCaptureNodesPolymorphically()
        {
            var visitor = Substitute.ForPartsOf<SyntaxNodeVisitor>();
            var attributeValue = new AttributeValue(0, "value");
            visitor.VisitAttributeValue(attributeValue);
            visitor.Received().VisitCaptureNode(attributeValue);
            Assert.AreEqual(typeof(CaptureNode), typeof(AttributeValue).BaseType);
        }

        [TestMethod]
        public void VisitBlockEndCallsVisitSyntaxTokenToAllowProcessingAllSyntaxTokensPolymorphically()
        {
            var visitor = Substitute.ForPartsOf<SyntaxNodeVisitor>();
            var blockEnd = new BlockEnd(0);
            visitor.VisitBlockEnd(blockEnd);
            visitor.Received().VisitSyntaxToken(blockEnd);
            Assert.AreEqual(typeof(SyntaxToken), typeof(BlockEnd).BaseType);
        }

        [TestMethod]
        public void VisitCaptureNodeCallsVisitTerminalNodeToAllowProcessingAllTerminalNodesPolymorphically()
        {
            var visitor = Substitute.ForPartsOf<SyntaxNodeVisitor>();
            var captureNode = new AttributeName(0, "attribute");
            visitor.VisitCaptureNode(captureNode);
            visitor.Received().VisitTerminalNode(captureNode);
            Assert.AreEqual(typeof(TerminalNode), typeof(CaptureNode).BaseType);
        }

        [TestMethod]
        public void VisitClassBlockStartCallsVisitCodeBlockStartToAllowProcessingAllCodeBlockStartsPolymorphically()
        {
            var visitor = Substitute.ForPartsOf<SyntaxNodeVisitor>();
            var classBlockStart = new ClassBlockStart(0);
            visitor.VisitClassBlockStart(classBlockStart);
            visitor.Received().VisitCodeBlockStart(classBlockStart);
            Assert.AreEqual(typeof(CodeBlockStart), typeof(ClassBlockStart).BaseType);
        }

        [TestMethod]
        public void VisitCodeCallsVisitTerminalNodeToAllowProcessingAllTerminalNodesPolymorphically()
        {
            var visitor = Substitute.ForPartsOf<SyntaxNodeVisitor>();
            var code = new Code(default(Span));
            visitor.VisitCode(code);
            visitor.Received().VisitTerminalNode(code);
            Assert.AreEqual(typeof(TerminalNode), typeof(Code).BaseType);
        }

        [TestMethod]
        public void VisitCodeBlockCallsVisitNonterminalNodeToAllowProcessingAllNonterminalNodesPolymorphically()
        {
            var visitor = Substitute.ForPartsOf<SyntaxNodeVisitor>();
            var codeBlock = new CodeBlock(new StatementBlockStart(0), new BlockEnd(0));
            visitor.VisitCodeBlock(codeBlock);
            visitor.Received().VisitNonterminalNode(codeBlock);
            Assert.AreEqual(typeof(NonterminalNode), typeof(CodeBlock).BaseType);
        }

        [TestMethod]
        public void VisitCodeBlockStartCallsVisitSyntaxTokenToAllowProcessingAllSyntaxTokensPolymorphically()
        {
            var visitor = Substitute.ForPartsOf<SyntaxNodeVisitor>();
            var codeBlockStart = new StatementBlockStart(0);
            visitor.VisitCodeBlockStart(codeBlockStart);
            visitor.Received().VisitSyntaxToken(codeBlockStart);
            Assert.AreEqual(typeof(SyntaxToken), typeof(CodeBlockStart).BaseType);
        }

        [TestMethod]
        public void VisitCustomDirectiveCallsVisitDirectiveToAllowProcessingAllDirectivesPolymorphically()
        {
            var visitor = Substitute.ForPartsOf<SyntaxNodeVisitor>();
            var customDirective = new CustomDirective(new DirectiveBlockStart(0), new DirectiveName(0, "custom"), Enumerable.Empty<Attribute>(), new BlockEnd(0));
            visitor.VisitCustomDirective(customDirective);
            visitor.Received().VisitDirective(customDirective);
            Assert.AreEqual(typeof(Directive), typeof(CustomDirective).BaseType);
        }

        [TestMethod]
        public void VisitDirectiveCallsVisitNonterminalNodeToAllowProcessingAllNonterminalNodesPolymorphically()
        {
            var visitor = Substitute.ForPartsOf<SyntaxNodeVisitor>();
            var directive = new CustomDirective(new DirectiveBlockStart(0), new DirectiveName(0, "custom"), Enumerable.Empty<Attribute>(), new BlockEnd(0));
            visitor.VisitDirective(directive);
            visitor.Received().VisitNonterminalNode(directive);
            Assert.AreEqual(typeof(NonterminalNode), typeof(Directive).BaseType);
        }

        [TestMethod]
        public void VisitDirectiveBlockStartCallsVisitSyntaxTokenToAllowProcessingAllSyntaxTokensPolymorphically()
        {
            var visitor = Substitute.ForPartsOf<SyntaxNodeVisitor>();
            var directiveBlockStart = new DirectiveBlockStart(0);
            visitor.VisitDirectiveBlockStart(directiveBlockStart);
            visitor.Received().VisitSyntaxToken(directiveBlockStart);
            Assert.AreEqual(typeof(SyntaxToken), typeof(DirectiveBlockStart).BaseType);
        }

        [TestMethod]
        public void VisitDirectiveNameCallsVisitCaptureNodeToAllowProcessingAllCaptureNodesPolymorphically()
        {
            var visitor = Substitute.ForPartsOf<SyntaxNodeVisitor>();
            var directiveName = new DirectiveName(0, "directive");
            visitor.VisitDirectiveName(directiveName);
            visitor.Received().VisitCaptureNode(directiveName);
            Assert.AreEqual(typeof(CaptureNode), typeof(DirectiveName).BaseType);
        }

        [TestMethod]
        public void VisitDoubleQuoteCallsVisitSyntaxTokenToAllowProcessingAllSyntaxTokensPolymorphically()
        {
            var visitor = Substitute.ForPartsOf<SyntaxNodeVisitor>();
            var doubleQuote = new DoubleQuote(0);
            visitor.VisitDoubleQuote(doubleQuote);
            visitor.Received().VisitSyntaxToken(doubleQuote);
            Assert.AreEqual(typeof(SyntaxToken), typeof(DoubleQuote).BaseType);
        }

        [TestMethod]
        public void VisitEndOfFileCallsVisitSyntaxTokenToAllowProcessingAllSyntaxTokensPolymorphically()
        {
            var visitor = Substitute.ForPartsOf<SyntaxNodeVisitor>();
            var endOfFile = new EndOfFile(0);
            visitor.VisitEndOfFile(endOfFile);
            visitor.Received().VisitSyntaxToken(endOfFile);
            Assert.AreEqual(typeof(SyntaxToken), typeof(EndOfFile).BaseType);
        }

        [TestMethod]
        public void VisitEqualsCallsVisitSyntaxTokenToAllowProcessingAllSyntaxTokensPolymorphically()
        {
            var visitor = Substitute.ForPartsOf<SyntaxNodeVisitor>();
            var equals = new Equals(0);
            visitor.VisitEquals(equals);
            visitor.Received().VisitSyntaxToken(equals);
            Assert.AreEqual(typeof(SyntaxToken), typeof(Equals).BaseType);
        }

        [TestMethod]
        public void VisitExpressionBlockStartCallsVisitCodeBlockStartToAllowProcessingAllCodeBlockStartsPolymorphically()
        {
            var visitor = Substitute.ForPartsOf<SyntaxNodeVisitor>();
            var expressionBlockStart = new ExpressionBlockStart(0);
            visitor.VisitExpressionBlockStart(expressionBlockStart);
            visitor.Received().VisitCodeBlockStart(expressionBlockStart);
            Assert.AreEqual(typeof(CodeBlockStart), typeof(ExpressionBlockStart).BaseType);
        }

        [TestMethod]
        public void VisitImportDirectiveCallsVisitDirectiveToAllowProcessingAllDirectivesPolymorphically()
        {
            var visitor = Substitute.ForPartsOf<SyntaxNodeVisitor>();
            var importDirective = new ImportDirective(new DirectiveBlockStart(0), new DirectiveName(0, "import"), Enumerable.Empty<Attribute>(), new BlockEnd(0));
            visitor.VisitImportDirective(importDirective);
            visitor.Received().VisitDirective(importDirective);
            Assert.AreEqual(typeof(Directive), typeof(ImportDirective).BaseType);
        }

        [TestMethod]
        public void VisitIncludeDirectiveCallsVisitDirectiveToAllowProcessingAllDirectivesPolymorphically()
        {
            var visitor = Substitute.ForPartsOf<SyntaxNodeVisitor>();
            var includeDirective = new IncludeDirective(new DirectiveBlockStart(0), new DirectiveName(0, "include"), Enumerable.Empty<Attribute>(), new BlockEnd(0));
            visitor.VisitIncludeDirective(includeDirective);
            visitor.Received().VisitDirective(includeDirective);
            Assert.AreEqual(typeof(Directive), typeof(IncludeDirective).BaseType);
        }

        [TestMethod]
        public void VisitNonterminalNodeCallsVisitSyntaxNodeToAllowProcessingAllSyntaxNodesPolymorphically()
        {
            var visitor = Substitute.ForPartsOf<SyntaxNodeVisitor>();
            var nonterminalNode = new CodeBlock(new StatementBlockStart(0), new BlockEnd(0));
            visitor.VisitNonterminalNode(nonterminalNode);
            Assert.AreEqual(1, visitor.ReceivedCalls().Count(call => call.GetMethodInfo().Name == "VisitSyntaxNode" && call.GetArguments()[0] == nonterminalNode));
            Assert.AreEqual(typeof(SyntaxNode), typeof(NonterminalNode).BaseType);
        }

        [TestMethod]
        public void VisitNonterminalNodeVisitsChildNodesToAllowProcessingAllNodesRecursively()
        {
            var visitor = Substitute.ForPartsOf<SyntaxNodeVisitor>();
            var nonterminalNode = new CodeBlock(new StatementBlockStart(0), new BlockEnd(0));
            visitor.VisitNonterminalNode(nonterminalNode);
            Assert.AreEqual(3, visitor.ReceivedCalls().Count(call => call.GetMethodInfo().Name == "VisitSyntaxNode"));
        }

        [TestMethod]
        public void VisitOutputDirectiveCallsVisitDirectiveToAllowProcessingAllDirectivesPolymorphically()
        {
            var visitor = Substitute.ForPartsOf<SyntaxNodeVisitor>();
            var outputDirective = new OutputDirective(new DirectiveBlockStart(0), new DirectiveName(0, "output"), Enumerable.Empty<Attribute>(), new BlockEnd(0));
            visitor.VisitOutputDirective(outputDirective);
            visitor.Received().VisitDirective(outputDirective);            
            Assert.AreEqual(typeof(Directive), typeof(OutputDirective).BaseType);
        }

        [TestMethod]
        public void VisitParameterDirectiveCallsVisitDirectiveToAllowProcessingAllDirectivesPolymorphically()
        {
            var visitor = Substitute.ForPartsOf<SyntaxNodeVisitor>();
            var parameterDirective = new ParameterDirective(new DirectiveBlockStart(0), new DirectiveName(0, "parameter"), Enumerable.Empty<Attribute>(), new BlockEnd(0));
            visitor.VisitParameterDirective(parameterDirective);
            visitor.Received().VisitDirective(parameterDirective);            
            Assert.AreEqual(typeof(Directive), typeof(ParameterDirective).BaseType);
        }

        [TestMethod]
        public void VisitStatementBlockStartCallsVisitCodeBlockStartToAllowProcessingAllCodeBlockStartsPolymorphically()
        {
            var visitor = Substitute.ForPartsOf<SyntaxNodeVisitor>();
            var statementBlockStart = new StatementBlockStart(0);
            visitor.VisitStatementBlockStart(statementBlockStart);
            visitor.Received().VisitCodeBlockStart(statementBlockStart);
            Assert.AreEqual(typeof(CodeBlockStart), typeof(StatementBlockStart).BaseType);
        }

        [TestMethod]
        public void VisitSyntaxTokenCallsVisitTerminalNodeToAllowProcessingAllTerminalNodesPolymorphically()
        {
            var visitor = Substitute.ForPartsOf<SyntaxNodeVisitor>();
            var syntaxToken = new StatementBlockStart(0);
            visitor.VisitSyntaxToken(syntaxToken);
            visitor.Received().VisitTerminalNode(syntaxToken);
            Assert.AreEqual(typeof(TerminalNode), typeof(SyntaxToken).BaseType);
        }

        [TestMethod]
        public void VisitTemplateCallsVisitNonterminalNodeToAllowProcessingAllNonterminalNodesPolymorphically()
        {
            var visitor = Substitute.ForPartsOf<SyntaxNodeVisitor>();
            var template = new Template(new StatementBlockStart(0), new BlockEnd(0));
            visitor.VisitTemplate(template);
            visitor.Received().VisitNonterminalNode(template);
            Assert.AreEqual(typeof(NonterminalNode), typeof(Template).BaseType);
        }

        [TestMethod]
        public void VisitTemplateDirectiveCallsVisitDirectiveToAllowProcessingAllDirectivesPolymorphically()
        {
            var visitor = Substitute.ForPartsOf<SyntaxNodeVisitor>();
            var templateDirective = new TemplateDirective(new DirectiveBlockStart(0), new DirectiveName(0, "template"), Enumerable.Empty<Attribute>(), new BlockEnd(0));
            visitor.VisitTemplateDirective(templateDirective);
            visitor.Received().VisitDirective(templateDirective);
            Assert.AreEqual(typeof(Directive), typeof(TemplateDirective).BaseType);
        }

        [TestMethod]
        public void VisitTerminalNodeCallsVisitSyntaxNodeToAllowProcessingAllSyntaxNodesPolymorphically()
        {
            var visitor = Substitute.ForPartsOf<SyntaxNodeVisitor>();
            var terminalNode = new BlockEnd(0);
            visitor.VisitTerminalNode(terminalNode);
            Assert.AreEqual(1, visitor.ReceivedCalls().Count(call => call.GetMethodInfo().Name == "VisitSyntaxNode"));
            Assert.AreEqual(typeof(SyntaxNode), typeof(TerminalNode).BaseType);
        }
    }
}