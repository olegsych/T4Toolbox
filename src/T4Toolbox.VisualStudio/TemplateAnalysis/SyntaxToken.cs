// <copyright file="SyntaxToken.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.TemplateAnalysis
{
    internal abstract class SyntaxToken : TerminalNode
    {
        private readonly int start;

        protected SyntaxToken(int start, Position position) : base(position)
        {
            this.start = start;
        }

        protected int Start
        {
            get { return this.start; }
        }
    }
}