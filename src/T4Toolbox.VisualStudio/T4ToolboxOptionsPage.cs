// <copyright file="T4ToolboxOptionsPage.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio
{
    using System;
    using System.Runtime.InteropServices;
    using Microsoft.VisualStudio.Shell;

    /// <summary>
    /// A property grid page for <see cref="T4ToolboxOptions"/> in the Visual Studio Options dialog.
    /// </summary>
    [Guid("7929ACCC-926C-4D90-865A-4BB7D2EFB6D9")]
    internal class T4ToolboxOptionsPage : DialogPage
    {
        private readonly T4ToolboxOptions localOptions = new T4ToolboxOptions();

        public override object AutomationObject
        {
            get { return this.localOptions; }
        }

        public override void LoadSettingsFromStorage()
        {
            this.localOptions.CopyFrom(this.GetGlobalOptions());
        }

        public override void SaveSettingsToStorage()
        {
            this.GetGlobalOptions().CopyFrom(this.localOptions);
        }

        protected override void OnApply(PageApplyEventArgs e)
        {
            base.OnApply(e);
            this.GetGlobalOptions().SaveSettingsToStorage();
        }

        protected virtual T4ToolboxOptions GetGlobalOptions()
        {
            return T4ToolboxOptions.Instance;
        }
    }
}