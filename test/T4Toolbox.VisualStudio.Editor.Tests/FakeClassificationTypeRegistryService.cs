// <copyright file="FakeClassificationTypeRegistryService.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.Editor
{
    using System;
    using System.Collections.Generic;

    using Microsoft.VisualStudio.Text.Classification;

    internal class FakeClassificationTypeRegistryService : IClassificationTypeRegistryService
    {
        private readonly Dictionary<string, IClassificationType> registry = new Dictionary<string, IClassificationType>(StringComparer.OrdinalIgnoreCase);

        public IClassificationType GetClassificationType(string type)
        {
            IClassificationType classificationType;
            if (this.registry.TryGetValue(type, out classificationType))
            {
                return classificationType;
            }

            return null;
        }

        public IClassificationType CreateClassificationType(string type, IEnumerable<IClassificationType> baseTypes)
        {
            if (this.registry.ContainsKey(type))
            {
                throw new InvalidOperationException();
            }

            IClassificationType classificationType = new FakeClassificationType(type, baseTypes);
            this.registry.Add(type, classificationType);
            return classificationType;
        }

        public IClassificationType CreateTransientClassificationType(IEnumerable<IClassificationType> baseTypes)
        {
            throw new NotImplementedException();
        }

        public IClassificationType CreateTransientClassificationType(params IClassificationType[] baseTypes)
        {
            throw new NotImplementedException();
        }
    }
}