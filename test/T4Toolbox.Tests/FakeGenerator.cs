// <copyright file="FakeGenerator.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.Tests
{
    using System;

    internal class FakeGenerator : Generator
    {
        public Action RanCore { get; set; }

        public Action Validated { get; set; }

        protected override void RunCore()
        {
            if (this.RanCore != null)
            {
                this.RanCore();
            }
        }

        protected override void Validate()
        {
            if (this.Validated != null)
            {
                this.Validated();
            }
        }
    }
}
