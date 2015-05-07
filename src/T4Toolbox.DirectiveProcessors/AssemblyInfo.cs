// <copyright file="AssemblyInfo.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

using System;

[assembly: CLSCompliant(false)]

namespace T4Toolbox.DirectiveProcessors
{
    /// <summary>
    /// Defines constants describing the T4Toolbox.VisualStudio assembly.
    /// </summary>
    internal abstract class AssemblyInfo : T4Toolbox.AssemblyInfo
    {
        /// <summary>
        /// Gets the name of the assembly.
        /// </summary>
        public new const string Name = "T4Toolbox.DirectiveProcessors";
    }
}