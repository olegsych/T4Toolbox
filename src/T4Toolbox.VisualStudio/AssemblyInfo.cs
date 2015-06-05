// <copyright file="AssemblyInfo.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

using System;
using System.Reflection;
using Microsoft.VisualStudio.Shell;

[assembly: AssemblyTitle(T4Toolbox.VisualStudio.AssemblyInfo.Name)]
[assembly: CLSCompliant(false)]

// Help Visual Studio resolve references to the T4Toolbox assembly.
[assembly: ProvideCodeBase(
    AssemblyName = T4Toolbox.AssemblyInfo.Name,
    PublicKeyToken = T4Toolbox.AssemblyInfo.PublicKeyToken,
    Version = T4Toolbox.AssemblyInfo.Version,
    CodeBase = @"$PackageFolder$\" + T4Toolbox.AssemblyInfo.Name + ".dll")]

// This is currently required because MPF enumerates attributes applied to T4ToolboxPackage class when instantiating 
// the T4ToolboxOptionsPage. This can be eliminated if I can figure out how to register directive processors via MEF instead 
// of ProvideDirectiveProcessorAttribute.
[assembly: ProvideCodeBase(
    AssemblyName = T4Toolbox.DirectiveProcessors.AssemblyInfo.Name,
    PublicKeyToken = T4Toolbox.DirectiveProcessors.AssemblyInfo.PublicKeyToken,
    Version = T4Toolbox.DirectiveProcessors.AssemblyInfo.Version,
    CodeBase = @"$PackageFolder$\" + T4Toolbox.DirectiveProcessors.AssemblyInfo.Name + ".dll")]

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
    internal abstract class AssemblyInfo : T4Toolbox.AssemblyInfo
    {
        /// <summary>
        /// Gets the name of the assembly.
        /// </summary>
        public new const string Name = "T4Toolbox.VisualStudio";
    }
}