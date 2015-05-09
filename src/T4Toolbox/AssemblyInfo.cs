// <copyright file="AssemblyInfo.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

using System;
using System.Reflection;
using System.Runtime.CompilerServices;

[assembly: AssemblyTitle(T4Toolbox.AssemblyInfo.Name)]
[assembly: CLSCompliant(true)]

namespace T4Toolbox
{
    /// <summary>
    /// Defines constants for common assembly information.
    /// </summary>
    /// <remarks>
    /// This class is defined here instead of the CommonAssemblyInfo.cs because we need to use the
    /// <see cref="InternalsVisibleToAttribute"/> to enable access to internal members for testing.
    /// With internals visible to all projects in the solution, defining this class in every project
    /// generates compiler errors due to naming collisions.
    /// </remarks>
    internal abstract class AssemblyInfo
    {
        /// <summary>
        /// Name of the T4 Toolbox assembly.
        /// </summary>
        public const string Name = "T4Toolbox";

        /// <summary>
        /// Name of the product.
        /// </summary>
        public const string Product = "T4 Toolbox";

        /// <summary>
        /// Description of the product.
        /// </summary>
        public const string Description = "Extends code generation capabilities of Text Templates.";

        /// <summary>
        /// Version of the T4 Toolbox assembly.
        /// </summary>
        public const string Version = "14.0.0.0";
    }
}