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

#if SIGN_ASSEMBLY
        public const string PublicKey = ", PublicKey=00240000048000009400000006020000002400005253413100040000010001005d511b529d32a2542d3c9c5824631a08e8dc6ea3e5a6175989d27df82bb803bd869f9ada9fbc11ce7a99208b7228c6b9b2ce9440bdc61062bf27c01d2633cee0bd0076dc2ca87c5f88a79316cf8b4a9db4d79f8d019b2b289b3a9fedccb610d53d6e3b6d9dfe05d8aee8e5ccb9f5a709ed0c93c88824c552787bded111d02bca";
        public const string PublicKeyToken = "dc4a538672a7b38f";
#else
        public const string PublicKey = "";
        public const string PublicKeyToken = "";
#endif

        /// <summary>
        /// Version of the T4 Toolbox assembly.
        /// </summary>
        public const string Version = "12.0.0.0";
    }
}