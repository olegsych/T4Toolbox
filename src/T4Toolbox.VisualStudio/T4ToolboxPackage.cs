// <copyright file="T4ToolboxPackage.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.TextTemplating.VSHost;
    using T4Toolbox.DirectiveProcessors;
    using VSLangProj;

    /// <summary>
    /// Visual Studio Package of the T4 Toolbox extension.
    /// </summary>
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration(AssemblyInfo.Product, AssemblyInfo.Description, AssemblyInfo.Version)]
    [Guid(T4ToolboxPackage.Id)]

    // Register the T4 Toolbox page in the Visual Studio Options dialog
    [ProvideOptionPage(typeof(T4ToolboxOptionsPage), T4ToolboxOptions.Category, T4ToolboxOptions.Page, 100, 101, false)]

    // Register the TransformationContext directive processor with the Visual Studio T4 host
    [ProvideDirectiveProcessor(typeof(TransformationContextProcessor), TransformationContextProcessor.Name, "T4 Toolbox Infrastructure")]

    // Register the ITransformationContextProvider service accessed by the TransformationContext
    [ProvideService(typeof(ITransformationContextProvider))]

    // Register TemplatedFileGenerator for C# and Visual Basic projects
    [ProvideCodeGenerator(typeof(TemplatedFileGenerator), TemplatedFileGenerator.Name, TemplatedFileGenerator.Description, false, ProjectSystem = ProvideCodeGeneratorAttribute.CSharpProjectGuid)]
    [ProvideCodeGenerator(typeof(TemplatedFileGenerator), TemplatedFileGenerator.Name, TemplatedFileGenerator.Description, false, ProjectSystem = ProvideCodeGeneratorAttribute.VisualBasicProjectGuid)]

    // Register ScriptFileGenerator for C# and Visual Basic projects
    [ProvideCodeGenerator(typeof(ScriptFileGenerator), ScriptFileGenerator.Name, ScriptFileGenerator.Description, false, ProjectSystem = ProvideCodeGeneratorAttribute.CSharpProjectGuid)]
    [ProvideCodeGenerator(typeof(ScriptFileGenerator), ScriptFileGenerator.Name, ScriptFileGenerator.Description, false, ProjectSystem = ProvideCodeGeneratorAttribute.VisualBasicProjectGuid)]

    // Auto-load the package for C# and Visual Basic projects to extend file properties 
    [ProvideAutoLoad(VSConstants.UICONTEXT.CSharpProject_string)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.VBProject_string)]

    public sealed class T4ToolboxPackage : Package, IDisposable
    {
        internal const string Id = "c88631b5-770c-453d-b90e-7136f127d57d";
        private readonly List<IDisposable> extenderProviders = new List<IDisposable>();

        /// <summary>
        /// Releases managed resources held by this package.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        /// Registers services implemented by this package.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
            TransformationContextProvider.Register(this);
            TemplateLocator.Register(this);
            this.extenderProviders.Add(new BrowseObjectExtenderProvider(this, PrjBrowseObjectCATID.prjCATIDCSharpFileBrowseObject));
            this.extenderProviders.Add(new BrowseObjectExtenderProvider(this, PrjBrowseObjectCATID.prjCATIDVBFileBrowseObject));
        }

        /// <summary>
        /// Unregisters the services provided by this package.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                this.extenderProviders.ForEach(provider => provider.Dispose());
            }
        }
    }
}
