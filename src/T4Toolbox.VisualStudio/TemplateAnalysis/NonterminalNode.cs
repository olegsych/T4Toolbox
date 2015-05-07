// <copyright file="NonterminalNode.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.TemplateAnalysis
{
    using System.Collections.Generic;

    internal abstract class NonterminalNode : SyntaxNode
    {
        public override string ToString()
        {
            return this.Kind.ToString() + "(" + string.Join(", ", this.ChildNodes()) + ")";
        }

        public override IEnumerable<TemplateError> Validate()
        {
            foreach (SyntaxNode childNode in this.ChildNodes())
            {
                foreach (TemplateError error in childNode.Validate())
                {
                    yield return error;
                }
            }
        }
    }
}