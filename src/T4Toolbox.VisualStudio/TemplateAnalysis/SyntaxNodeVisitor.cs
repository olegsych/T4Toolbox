// <copyright file="SyntaxNodeVisitor.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.TemplateAnalysis
{
    using System.Diagnostics;

    internal abstract class SyntaxNodeVisitor
    {
        public void Visit(SyntaxNode node)
        {
            Debug.Assert(node != null, "null");
            node.Accept(this);
        }

        protected internal virtual void VisitAssemblyDirective(AssemblyDirective node)
        {
            this.VisitDirective(node);
        }
        
        protected internal virtual void VisitAttribute(Attribute node)
        {
            this.VisitNonterminalNode(node);
        }

        protected internal virtual void VisitAttributeName(AttributeName node)
        {
            this.VisitCaptureNode(node);
        }

        protected internal virtual void VisitAttributeValue(AttributeValue node)
        {
            this.VisitCaptureNode(node);
        }

        protected internal virtual void VisitBlockEnd(BlockEnd node)
        {            
            this.VisitSyntaxToken(node);
        }

        protected internal virtual void VisitCaptureNode(CaptureNode node)
        {
            this.VisitTerminalNode(node);
        }

        protected internal virtual void VisitClassBlockStart(ClassBlockStart node)
        {            
            this.VisitCodeBlockStart(node);
        }

        protected internal virtual void VisitCode(Code node)
        {            
            this.VisitTerminalNode(node);
        }

        protected internal virtual void VisitCodeBlock(CodeBlock node)
        {
            this.VisitNonterminalNode(node);
        }

        protected internal virtual void VisitCustomDirective(CustomDirective node)
        {
            this.VisitDirective(node);
        }

        protected internal virtual void VisitCodeBlockStart(CodeBlockStart node)
        {
            this.VisitSyntaxToken(node);
        }

        protected internal virtual void VisitDirective(Directive node)
        {
            this.VisitNonterminalNode(node);
        }

        protected internal virtual void VisitDirectiveBlockStart(DirectiveBlockStart node)
        {            
            this.VisitSyntaxToken(node);
        }

        protected internal virtual void VisitDirectiveName(DirectiveName node)
        {
            this.VisitCaptureNode(node);
        }

        protected internal virtual void VisitDoubleQuote(DoubleQuote node)
        {
            this.VisitSyntaxToken(node);
        }

        protected internal virtual void VisitEndOfFile(EndOfFile node)
        {
            this.VisitSyntaxToken(node);
        }

        protected internal virtual void VisitEquals(Equals node)
        {
            this.VisitSyntaxToken(node);
        }

        protected internal virtual void VisitExpressionBlockStart(ExpressionBlockStart node)
        {            
            this.VisitCodeBlockStart(node);
        }

        protected internal virtual void VisitImportDirective(ImportDirective node)
        {
            this.VisitDirective(node);
        }

        protected internal virtual void VisitIncludeDirective(IncludeDirective node)
        {
            this.VisitDirective(node);
        }

        protected internal virtual void VisitNonterminalNode(NonterminalNode node)
        {            
            this.VisitSyntaxNode(node);

            foreach (SyntaxNode childNode in node.ChildNodes())
            {
                childNode.Accept(this);
            }
        }

        protected internal virtual void VisitOutputDirective(OutputDirective node)
        {
            this.VisitDirective(node);
        }

        protected internal virtual void VisitParameterDirective(ParameterDirective node)
        {
            this.VisitDirective(node);
        }

        protected internal virtual void VisitStatementBlockStart(StatementBlockStart node)
        {            
            this.VisitCodeBlockStart(node);
        }

        protected internal virtual void VisitSyntaxToken(SyntaxToken node)
        {            
            this.VisitTerminalNode(node);
        }

        protected internal virtual void VisitTemplate(Template node)
        {            
            this.VisitNonterminalNode(node);
        }

        protected internal virtual void VisitTemplateDirective(TemplateDirective node)
        {            
            this.VisitDirective(node);
        }

        protected internal virtual void VisitTerminalNode(TerminalNode node)
        {           
            this.VisitSyntaxNode(node);
        }

        protected virtual void VisitSyntaxNode(SyntaxNode node)
        {
            Debug.Assert(node != null, "node");
        }
    }
}