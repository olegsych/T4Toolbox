// <copyright file="TemplateParser.y" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

%namespace T4Toolbox.VisualStudio.TemplateAnalysis
%using System.Linq;

%parsertype TemplateParser
%visibility internal
%partial 

%YYSTYPE SyntaxNode

%tokentype SyntaxKind

%token Attribute
%token AttributeName
%token AttributeValue
%token BlockEnd
%token ClassBlockStart
%token Code
%token CodeBlock
%token Directive
%token DirectiveBlockStart
%token DirectiveName
%token DoubleQuote
%token Equals
%token ExpressionBlockStart
%token StatementBlockStart
%token Template
%token EOF

%%

TemplateBody 
    : Blocks { 
        this.Template = new Template($1.ChildNodes().ToArray());
    }
    ;

Blocks
    : /* empty */ {
        $$ = new NodeBuilder();
    }
    | Blocks Block {
        var nodeBuilder = (NodeBuilder)$1;
        nodeBuilder.AddChildNode($2);
        $$ = nodeBuilder;
    }
    ;

Block
    : CodeBlockBody
    | DirectiveBody
    ;

CodeBlockBody 
    : CodeBlockStart Code BlockEnd {
        $$ = new CodeBlock((CodeBlockStart)$1, (Code)$2, (BlockEnd)$3);
    }
    | CodeBlockStart BlockEnd {
        $$ = new CodeBlock((CodeBlockStart)$1, (BlockEnd)$2);
    }
    | error BlockEnd { 
        // Recover from error to continue further parsing
        this.yyerrok(); 
        // Ignore the BlockEnd to avoid endless loop
        this.yyclearin(); 
    }
    | error { 
        // Recover from error to continue further parsing
        this.yyerrok(); 
    }
    ;

CodeBlockStart
    : StatementBlockStart
    | ExpressionBlockStart
    | ClassBlockStart
    ;

DirectiveBody
    : DirectiveBlockStart DirectiveName Attributes BlockEnd {
        $$ = Directive.Create((DirectiveBlockStart)$1, (DirectiveName)$2, $3.ChildNodes().Cast<Attribute>(), (BlockEnd)$4);
    }
    | DirectiveBlockStart DirectiveName BlockEnd {
        $$ = Directive.Create((DirectiveBlockStart)$1, (DirectiveName)$2, Enumerable.Empty<Attribute>(), (BlockEnd)$3);
    }
    ;

Attributes
    : AttributeBody {
        var nodeBuilder = new NodeBuilder();
        nodeBuilder.AddChildNode($1);
        $$ = nodeBuilder;
    }
    | Attributes AttributeBody {
        var nodeBuilder = (NodeBuilder)$1;
        nodeBuilder.AddChildNode($2);
        $$ = nodeBuilder;
    }
    ;

AttributeBody
    : AttributeName Equals DoubleQuote AttributeValue DoubleQuote {
        $$ = new Attribute((AttributeName)$1, (Equals)$2, (DoubleQuote)$3, (AttributeValue)$4, (DoubleQuote)$5);
    }
    ;