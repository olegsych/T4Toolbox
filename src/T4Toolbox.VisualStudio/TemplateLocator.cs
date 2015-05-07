// <copyright file="TemplateLocator.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio
{
    using System;
    using System.ComponentModel.Design;
    using System.Globalization;
    using System.IO;
    using Microsoft.VisualStudio.TextTemplating;
    using Microsoft.VisualStudio.TextTemplating.VSHost;

    /// <summary>
    /// Internal service for locating a template based on its file name.
    /// </summary>
    internal class TemplateLocator
    {
        protected TemplateLocator(IServiceProvider serviceProvider)
        {
            this.ServiceProvider = serviceProvider;
        }

        protected IServiceProvider ServiceProvider { get; private set; }

        /// <summary>
        /// Returns full path to the template file resolved using T4 include rules.
        /// </summary>
        public virtual bool LocateTemplate(string fullInputPath, ref string templatePath)
        {
            var textTemplating = (ITextTemplating)this.ServiceProvider.GetService(typeof(STextTemplating));

            // Use the built-in "include" resolution logic to find the template.
            string[] references;
            textTemplating.PreprocessTemplate(Path.ChangeExtension(fullInputPath, ".tt"), string.Empty, null, "DummyClass", string.Empty, out references);

            var engineHost = (ITextTemplatingEngineHost)textTemplating;

            string templateFileContent;
            string templateFullPath;
            if (engineHost.LoadIncludeText(templatePath, out templateFileContent, out templateFullPath))
            {
                templatePath = Path.GetFullPath(templateFullPath);
                return true;
            }

            return false;
        }

        internal static void Register(IServiceContainer serviceContainer)
        {
            serviceContainer.AddService(typeof(TemplateLocator), CreateService, true);
        }

        private static object CreateService(IServiceContainer container, Type serviceType)
        {
            if (serviceType == typeof(TemplateLocator))
            {
                return new TemplateLocator(container);                
            }

            return null;
        }
    }
}