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

[assembly: AssemblyProduct("T$Toolbox")]
[assembly: AssemblyDescription("T4Toolbox")]
//[assembly: AssemblyCompany("Oleg Sych")]
//[assembly: AssemblyCopyright("Copyright © Oleg Sych. All Rights Reserved.")]
//[assembly: AssemblyTrademark("")]
//[assembly: AssemblyCulture("")]
[assembly: AssemblyVersion("15.0.0.0")]
[assembly: AssemblyFileVersion(T4Toolbox.VisualStudio.AssemblyInfo.Version)]
[assembly: ComVisible(false)]
//[assembly: NeutralResourcesLanguage("en-US")]

// Allow all projects in this solution to access each-other's internals by default. 
// In many instances, we need this to enable testing as well as to access constants 
// in T4Toolbox.AssemblyInfo class. Revisit this decision when the number of assemblies
// in the project increases to the point where limiting access to internals within the 
// solution becomes beneficial.
//[assembly: InternalsVisibleTo("T4Toolbox.DirectiveProcessors" + T4Toolbox.AssemblyInfo.PublicKey)]
//[assembly: InternalsVisibleTo("T4Toolbox.TemplateAnalysis" + T4Toolbox.AssemblyInfo.PublicKey)]
//[assembly: InternalsVisibleTo("T4Toolbox.TemplateAnalysis.Tests" + T4Toolbox.AssemblyInfo.PublicKey)]
//[assembly: InternalsVisibleTo("T4Toolbox.Tests" + T4Toolbox.AssemblyInfo.PublicKey)]
//[assembly: InternalsVisibleTo("T4Toolbox.VisualStudio" + T4Toolbox.AssemblyInfo.PublicKey)]
//[assembly: InternalsVisibleTo("T4Toolbox.VisualStudio.Editor" + T4Toolbox.AssemblyInfo.PublicKey)]
//[assembly: InternalsVisibleTo("T4Toolbox.VisualStudio.Editor.Tests" + T4Toolbox.AssemblyInfo.PublicKey)]
//[assembly: InternalsVisibleTo("T4Toolbox.VisualStudio.IntegrationTests" + T4Toolbox.AssemblyInfo.PublicKey)]
//[assembly: InternalsVisibleTo("T4Toolbox.VisualStudio.Tests" + T4Toolbox.AssemblyInfo.PublicKey)]

#if SIGN_ASSEMBLY
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2, PublicKey=0024000004800000940000000602000000240000525341310004000001000100c547cac37abd99c8db225ef2f6c8a3602f3b3606cc9891605d02baa56104f4cfc0734aa39b93bf7852f7d9266654753cc297e7d2edfe0bac1cdcf9f717241550e0a7b191195b7667bb4f64bcb8e2121380fd1d9d46ad2d92d2d15605093924cceaf74c4861eff62abf69b9291ed0a340e113be11e6a7d3113e92484cf7045cc7")]
#else
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
#endif
