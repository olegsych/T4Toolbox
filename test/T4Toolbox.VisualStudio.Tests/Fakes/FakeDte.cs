// <copyright file="FakeDte.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.Tests.Fakes
{
    using System;
    using System.IO;

    using EnvDTE;

    using Microsoft.VsSDK.UnitTestLibrary;

    internal class FakeDte : OleServiceProvider, DTE
    {
        private DirectoryInfo testDirectory;

        public FakeDte()
        {
            this.ObjectExtenders = new FakeObjectExtenders(this);
            this.AddService(typeof(DTE), this, false);
        }

        public FakeObjectExtenders ObjectExtenders { get; private set; }

        public FakeSolution Solution { get; set; }

        #region EnvDTE.DTE

        Document _DTE.ActiveDocument
        {
            get { throw new NotImplementedException(); }
        }

        object _DTE.ActiveSolutionProjects
        {
            get { throw new NotImplementedException(); }
        }

        Window _DTE.ActiveWindow
        {
            get { throw new NotImplementedException(); }
        }

        AddIns _DTE.AddIns
        {
            get { throw new NotImplementedException(); }
        }

        DTE _DTE.Application
        {
            get { throw new NotImplementedException(); }
        }

        object _DTE.CommandBars
        {
            get { throw new NotImplementedException(); }
        }

        string _DTE.CommandLineArguments
        {
            get { throw new NotImplementedException(); }
        }

        Commands _DTE.Commands
        {
            get { throw new NotImplementedException(); }
        }

        ContextAttributes _DTE.ContextAttributes
        {
            get { throw new NotImplementedException(); }
        }

        DTE _DTE.DTE
        {
            get { throw new NotImplementedException(); }
        }

        Debugger _DTE.Debugger
        {
            get { throw new NotImplementedException(); }
        }

        vsDisplay _DTE.DisplayMode
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        Documents _DTE.Documents
        {
            get { throw new NotImplementedException(); }
        }

        string _DTE.Edition
        {
            get { throw new NotImplementedException(); }
        }

        Events _DTE.Events
        {
            get { throw new NotImplementedException(); }
        }

        void _DTE.ExecuteCommand(string commandName, string commandArgs)
        {
            throw new NotImplementedException();
        }

        string _DTE.FileName
        {
            get { throw new NotImplementedException(); }
        }

        Find _DTE.Find
        {
            get { throw new NotImplementedException(); }
        }

        string _DTE.FullName
        {
            get { throw new NotImplementedException(); }
        }

        object _DTE.GetObject(string name)
        {
            throw new NotImplementedException();
        }

        Globals _DTE.Globals
        {
            get { throw new NotImplementedException(); }
        }

        ItemOperations _DTE.ItemOperations
        {
            get { throw new NotImplementedException(); }
        }

        wizardResult _DTE.LaunchWizard(string file, ref object[] contextParams)
        {
            throw new NotImplementedException();
        }

        int _DTE.LocaleID
        {
            get { throw new NotImplementedException(); }
        }

        Macros _DTE.Macros
        {
            get { throw new NotImplementedException(); }
        }

        DTE _DTE.MacrosIDE
        {
            get { throw new NotImplementedException(); }
        }

        Window _DTE.MainWindow
        {
            get { throw new NotImplementedException(); }
        }

        vsIDEMode _DTE.Mode
        {
            get { throw new NotImplementedException(); }
        }

        string _DTE.Name
        {
            get { throw new NotImplementedException(); }
        }

        ObjectExtenders _DTE.ObjectExtenders
        {
            get { return this.ObjectExtenders; }
        }

        Window _DTE.OpenFile(string viewKind, string fileName)
        {
            throw new NotImplementedException();
        }

        void _DTE.Quit()
        {
            throw new NotImplementedException();
        }

        string _DTE.RegistryRoot
        {
            get { throw new NotImplementedException(); }
        }

        string _DTE.SatelliteDllPath(string path, string mame)
        {
            throw new NotImplementedException();
        }

        SelectedItems _DTE.SelectedItems
        {
            get { throw new NotImplementedException(); }
        }

        Solution _DTE.Solution
        {
            get { return this.Solution; }
        }

        SourceControl _DTE.SourceControl
        {
            get { throw new NotImplementedException(); }
        }

        StatusBar _DTE.StatusBar
        {
            get { throw new NotImplementedException(); }
        }

        bool _DTE.SuppressUI
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        UndoContext _DTE.UndoContext
        {
            get { throw new NotImplementedException(); }
        }

        bool _DTE.UserControl
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        string _DTE.Version
        {
            get { throw new NotImplementedException(); }
        }

        WindowConfigurations _DTE.WindowConfigurations
        {
            get { throw new NotImplementedException(); }
        }

        Windows _DTE.Windows
        {
            get { throw new NotImplementedException(); }
        }

        bool _DTE.get_IsOpenFile(string viewKind, string fileName)
        {
            throw new NotImplementedException();
        }

        Properties _DTE.get_Properties(string category, string page)
        {
            throw new NotImplementedException();
        }

        #endregion

        internal DirectoryInfo TestDirectory
        {
            get { return this.testDirectory ?? (this.testDirectory = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), Path.GetRandomFileName()))); }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                if (this.testDirectory != null)
                {
                    this.testDirectory.Delete(true);
                }
            }
        }
    }
}
