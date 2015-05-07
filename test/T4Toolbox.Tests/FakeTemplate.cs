// <copyright file="FakeTemplate.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.Tests
{
    using System;

    internal class FakeTemplate : Template
    {
        public Action Initialized { get; set; }

        public Action TransformedText { get; set; }

        public Action Validated { get; set; }

        public override void Initialize()
        {
            base.Initialize();

            if (this.Initialized != null)
            {
                this.Initialized();
            }
        }

        public override string TransformText()
        {
            if (this.TransformedText != null)
            {
                this.TransformedText();
            }

            return this.GenerationEnvironment.ToString();
        }

        protected override void Validate()
        {
            base.Validate();

            if (this.Validated != null)
            {
                this.Validated();
            }
        }
    }
}
