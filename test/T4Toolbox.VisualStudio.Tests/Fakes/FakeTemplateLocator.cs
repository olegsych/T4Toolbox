// <copyright file="FakeTemplateLocator.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.Tests.Fakes
{
    using System.IO;

    internal class FakeTemplateLocator : TemplateLocator
    {
        public FakeTemplateLocator() : base(null)
        {            
        }

        public override bool LocateTemplate(string fullInputPath, ref string templatePath)
        {
            templatePath = Path.GetFullPath(templatePath);
            return File.Exists(templatePath);
        }
    }
}