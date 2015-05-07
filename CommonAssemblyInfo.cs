// <copyright file="CommonAssemblyInfo.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif 

[assembly: AssemblyProduct(T4Toolbox.AssemblyInfo.Product)]
[assembly: AssemblyDescription(T4Toolbox.AssemblyInfo.Description)]
[assembly: AssemblyCompany("Oleg Sych")]
[assembly: AssemblyCopyright("Copyright © Oleg Sych. All Rights Reserved.")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: AssemblyVersion(T4Toolbox.AssemblyInfo.Version)]
[assembly: AssemblyFileVersion(T4Toolbox.AssemblyInfo.Version)]
[assembly: ComVisible(false)]
[assembly: NeutralResourcesLanguage("en-US")]

// Allow all projects in this solution to access each-other's internals by default. 
// In many instances, we need this to enable testing as well as to access constants 
// in T4Toolbox.AssemblyInfo class. Revisit this decision when the number of assemblies
// in the project increases to the point where limiting access to internals within the 
// solution becomes beneficial.
[assembly: InternalsVisibleTo("T4Toolbox.DirectiveProcessors")]
[assembly: InternalsVisibleTo("T4Toolbox.Tests")]
[assembly: InternalsVisibleTo("T4Toolbox.VisualStudio")]
[assembly: InternalsVisibleTo("T4Toolbox.VisualStudio.IntegrationTests")]
[assembly: InternalsVisibleTo("T4Toolbox.VisualStudio.Tests")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
