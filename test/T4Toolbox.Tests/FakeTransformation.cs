// <copyright file="FakeTransformation.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.Tests
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Text;
    using Microsoft.VisualStudio.TextTemplating;

    internal class FakeTransformation : TextTransformation
    {
        [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "Session is initialized by default")]
        public FakeTransformation()
        {
            this.Host = new FakeTextTemplatingService();  
            this.Session = new Dictionary<string, object>();
        }

        public FakeTextTemplatingService Host { get; set; }

        public new StringBuilder GenerationEnvironment
        {
            get { return base.GenerationEnvironment; }
        }

        public override string TransformText()
        {
            return this.GenerationEnvironment.ToString();
        }
    }
}
