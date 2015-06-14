// <copyright file="TemplateParser.lex" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

%namespace T4Toolbox.TemplateAnalysis
%using Microsoft.VisualStudio.Text;
%scannertype TemplateScanner
%visibility internal
%tokentype SyntaxKind

%option unicode, minimize, nopersistbuffer, nofiles

// Regular expressions

BlockStart      <#
BlockEnd        #>
Name            [a-zA-Z_][a-zA-Z0-9_]*

// States

%x CodeBlock      // Scanner is inside of a Statement, Expression or Class Feature block 
%x Directive      // Scanner is inside of a Directive
%x Attributes     // Scanner is inside of a Directive, after the name of the directive 
%x AttributeValue // Scanner is inside of an attribute value

%% // Rules (Scan/switch)

%{
    int eofPos;
%}

<<EOF>> {
    // Create an EOF token for the parser error handling logic
    eofPos = Math.Max(0, this.tokPos - 1); // why tokPos - 1?
    this.yylval = new EndOfFile(eofPos, this.endOfFilePosition);
    return (int)this.yylval.Kind;
}

{BlockStart} { 
    this.yylval = new StatementBlockStart(this.tokPos, this.CurrentPosition);
    this.BeginToken(CodeBlock);
    return (int)this.yylval.Kind;
}

{BlockStart}= { 
    this.yylval = new ExpressionBlockStart(this.tokPos, this.CurrentPosition);
    this.BeginToken(CodeBlock);
    return (int)this.yylval.Kind;
}

{BlockStart}\+ { 
    this.yylval = new ClassBlockStart(this.tokPos, this.CurrentPosition);
    this.BeginToken(CodeBlock);
    return (int)this.yylval.Kind;
}

{BlockStart}@ { 
    this.yylval = new DirectiveBlockStart(this.tokPos, this.CurrentPosition);
    this.BEGIN(Directive);
    return (int)this.yylval.Kind;
}

{BlockEnd} { 
    this.yylval = new BlockEnd(this.tokPos, this.CurrentPosition);
    return (int)this.yylval.Kind;
}

<Directive> {
    {Name} {
        this.yylval = new DirectiveName(this.tokPos, this.yytext, this.CurrentPosition);
        this.BEGIN(Attributes);
        return (int)this.yylval.Kind;
    }

    ({Name}[ \t\n\r]*=|=|\") {
        this.BEGIN(Attributes);
        this.DiscardToken();
    }

    ({BlockStart}|{BlockEnd}) {
        this.BEGIN(INITIAL); 
        this.DiscardToken();
    }

    <<EOF>> {
        this.yylval = new EndOfFile(this.tokPos - 1, this.endOfFilePosition);
        return (int)this.yylval.Kind;
    }
}

<Attributes> {
    {Name} {
        this.yylval = new AttributeName(this.tokPos, this.yytext, this.CurrentPosition);
        return (int)this.yylval.Kind;
    }

    = {
        this.yylval = new Equals(this.tokPos, this.CurrentPosition);
        return (int)this.yylval.Kind;
    }

    \" {
        this.yylval = new DoubleQuote(this.tokPos, this.CurrentPosition);
        this.BeginToken(AttributeValue);
        return (int)this.yylval.Kind;
    }

    ({BlockStart}|{BlockEnd}) {
        this.BEGIN(INITIAL); 
        this.DiscardToken();
    }

    <<EOF>> {
        this.yylval = new EndOfFile(this.tokPos - 1, this.endOfFilePosition);
        return (int)this.yylval.Kind;
    }
}

<AttributeValue> {
    \" {       
        if (this.tokPos > this.tokenStart)
        {
            this.yylval = new AttributeValue(this.tokenStart, this.buffer.GetString(this.tokenStart, this.tokPos), this.tokenPosition);
            this.tokenStart = this.tokPos; // discard the attribute value token 
            this.DiscardToken();
            return (int)this.yylval.Kind;
        }

        this.yylval = new DoubleQuote(this.tokPos, this.CurrentPosition);
        this.BEGIN(Attributes);
        return (int)this.yylval.Kind;
    }

    ({BlockStart}|{BlockEnd}) {
        this.BEGIN(INITIAL);
        this.DiscardToken();

        if (this.tokPos > this.tokenStart)
        {
            this.yylval = new AttributeValue(this.tokenStart, this.buffer.GetString(this.tokenStart, this.tokPos), this.tokenPosition);
            return (int)this.yylval.Kind;
        }
    }

    <<EOF>> {
        eofPos = this.tokPos - 1;

        if (eofPos > this.tokenStart)
        {
            this.yylval = new AttributeValue(this.tokenStart, this.buffer.GetString(this.tokenStart, eofPos), this.tokenPosition);
            this.tokenStart = this.tokPos; // discard the attribute value token
            this.yyless(0); // discard the end of file token
            return (int)this.yylval.Kind;
        }

        this.yylval = new EndOfFile(eofPos, this.endOfFilePosition);
        return (int)this.yylval.Kind;
    }
}

<CodeBlock> {
    ({BlockStart}|{BlockEnd}) {
        this.BEGIN(INITIAL); 
        this.DiscardToken();

        if (this.tokPos > this.tokenStart)
        {
            this.yylval = new Code(Span.FromBounds(this.tokenStart, this.tokPos), this.tokenPosition);
            return (int)this.yylval.Kind;
        }
    }

    <<EOF>> {
        eofPos = this.tokPos - 1;

        if (eofPos > this.tokenStart)
        {
            this.yylval = new Code(Span.FromBounds(this.tokenStart, eofPos), this.tokenPosition);
            this.tokenStart = this.tokPos; // discard the code token 
            this.yyless(0); // discard the end of file token
            return (int)this.yylval.Kind;
        }

        this.yylval = new EndOfFile(eofPos, this.endOfFilePosition);
        return (int)this.yylval.Kind;
    }
}
