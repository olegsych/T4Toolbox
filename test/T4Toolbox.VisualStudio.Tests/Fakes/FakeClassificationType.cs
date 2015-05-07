// <copyright file="FakeClassificationType.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.Tests.Fakes
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Microsoft.VisualStudio.Text.Classification;

    internal class FakeClassificationType : IClassificationType
    {
        public FakeClassificationType(string classification, IEnumerable<IClassificationType> baseTypes)
        {
            Debug.Assert(classification != null, "classification");
            Debug.Assert(baseTypes != null, "baseTypes");

            this.Classification = classification;
            this.BaseTypes = baseTypes;
        }

        public bool IsOfType(string type)
        {
            throw new NotImplementedException();
        }

        public string Classification { get; private set; }

        public IEnumerable<IClassificationType> BaseTypes { get; private set; }
    }
}