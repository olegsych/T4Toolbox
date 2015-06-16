// <copyright file="SubstituteExporter.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.Editor
{
    using System.ComponentModel.Composition;
    using NSubstitute;

    internal class SubstituteExporter<T> where T: class
    {
        [Export]
        public T Instance = Substitute.For<T>(); 
    }
}
