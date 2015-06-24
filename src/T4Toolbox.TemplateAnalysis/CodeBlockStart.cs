// <copyright file="CodeBlockStart.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.TemplateAnalysis
{
    internal abstract class CodeBlockStart : SyntaxToken
    {
        protected CodeBlockStart(int start, Position position) : base(start, position)
        {
        }
    }
}