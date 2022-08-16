// <copyright file="AssemblyInfo.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

using System;
using System.Reflection;
using Microsoft.VisualStudio.Shell;

[assembly: AssemblyTitle(T4Toolbox.VisualStudio.AssemblyInfo.Name)]
[assembly: CLSCompliant(false)]

//// Help Visual Studio resolve references to the T4Toolbox assembly.
//[assembly: ProvideCodeBase(
//    AssemblyName = T4Toolbox.AssemblyInfo.Name,
//    PublicKeyToken = T4Toolbox.AssemblyInfo.PublicKeyToken,
//    Version = T4Toolbox.AssemblyInfo.Version,
//    CodeBase = @"$PackageFolder$\" + T4Toolbox.AssemblyInfo.Name + ".dll")]

// This is currently required because MPF enumerates attributes applied to T4ToolboxPackage class when instantiating 
// the T4ToolboxOptionsPage. This can be eliminated if I can figure out how to register directive processors via MEF instead 
// of ProvideDirectiveProcessorAttribute.
//[assembly: ProvideCodeBase(
//    AssemblyName = T4Toolbox.DirectiveProcessors.AssemblyInfo.Name,
//    PublicKeyToken = T4Toolbox.DirectiveProcessors.AssemblyInfo.PublicKeyToken,
//    Version = T4Toolbox.DirectiveProcessors.AssemblyInfo.Version,
//    CodeBase = @"$PackageFolder$\" + T4Toolbox.DirectiveProcessors.AssemblyInfo.Name + ".dll")]

// Help Visual Studio resolve references to the T4Toolbox.VisualStudio assembly.
[assembly: ProvideCodeBase(
    AssemblyName = T4Toolbox.VisualStudio.AssemblyInfo.Name,
    PublicKeyToken = T4Toolbox.VisualStudio.AssemblyInfo.PublicKeyToken,
    Version = T4Toolbox.VisualStudio.AssemblyInfo.Version,
    CodeBase = @"$PackageFolder$\" + T4Toolbox.VisualStudio.AssemblyInfo.Name + ".dll")]

namespace T4Toolbox.VisualStudio
{
    /// <summary>
    /// Defines constants describing the T4Toolbox.VisualStudio assembly.
    /// </summary>
    internal abstract class AssemblyInfo : AssemblyInfoT4Toolbox
    {
        /// <summary>
        /// Gets the name of the assembly.
        /// </summary>
        public new const string Name = "T4Toolbox.VisualStudio";
    }

    internal abstract class AssemblyInfoT4Toolbox
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
        public const string Version = "15.0.0.0";
    }
}