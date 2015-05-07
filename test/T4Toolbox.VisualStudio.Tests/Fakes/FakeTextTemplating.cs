// <copyright file="FakeTextTemplating.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.Tests.Fakes
{
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.VisualStudio.TextTemplating;
    using Microsoft.VisualStudio.TextTemplating.VSHost;

    internal class FakeTextTemplating : STextTemplating, ITextTemplating, ITextTemplatingEngineHost, ITextTemplatingComponents
    {
        public FakeTextTemplating()
        {
            this.Errors = new CompilerErrorCollection();

            var callback = new TextTemplatingCallback();
            callback.Initialize();
            this.Callback = callback;
        }

        public CompilerErrorCollection Errors { get; private set; }

        #region ITextTemplating

        string ITextTemplating.ProcessTemplate(string inputFile, string content, ITextTemplatingCallback callback, object hierarchy)
        {
            throw new NotImplementedException();
        }

        string ITextTemplating.PreprocessTemplate(string inputFile, string content, ITextTemplatingCallback callback, string className, string classNamespace, out string[] references)
        {
            throw new NotImplementedException();
        }

        void ITextTemplating.BeginErrorSession()
        {
            throw new NotImplementedException();
        }

        bool ITextTemplating.EndErrorSession()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region ITextTemplatingEngineHost

        bool ITextTemplatingEngineHost.LoadIncludeText(string requestFileName, out string content, out string location)
        {
            throw new NotImplementedException();
        }

        string ITextTemplatingEngineHost.ResolveAssemblyReference(string assemblyReference)
        {
            throw new NotImplementedException();
        }

        Type ITextTemplatingEngineHost.ResolveDirectiveProcessor(string processorName)
        {
            throw new NotImplementedException();
        }

        string ITextTemplatingEngineHost.ResolvePath(string path)
        {
            throw new NotImplementedException();
        }

        string ITextTemplatingEngineHost.ResolveParameterValue(string directiveId, string processorName, string parameterName)
        {
            throw new NotImplementedException();
        }

        AppDomain ITextTemplatingEngineHost.ProvideTemplatingAppDomain(string content)
        {
            throw new NotImplementedException();
        }

        void ITextTemplatingEngineHost.LogErrors(CompilerErrorCollection errors)
        {
            this.Errors.AddRange(errors);
        }

        void ITextTemplatingEngineHost.SetFileExtension(string extension)
        {
            throw new NotImplementedException();
        }

        void ITextTemplatingEngineHost.SetOutputEncoding(Encoding encoding, bool fromOutputDirective)
        {
            throw new NotImplementedException();
        }

        object ITextTemplatingEngineHost.GetHostOption(string optionName)
        {
            throw new NotImplementedException();
        }

        IList<string> ITextTemplatingEngineHost.StandardAssemblyReferences
        {
            get { throw new NotImplementedException(); }
        }

        IList<string> ITextTemplatingEngineHost.StandardImports
        {
            get { throw new NotImplementedException(); }
        }

        string ITextTemplatingEngineHost.TemplateFile
        {
            get { return this.InputFile; }
        }

        #endregion

        #region ITextTemplatingComponents

        ITextTemplatingEngineHost ITextTemplatingComponents.Host
        {
            get { return this; }
        }

        ITextTemplatingEngine ITextTemplatingComponents.Engine
        {
            get { throw new NotImplementedException(); }
        }

        public string InputFile { get; set; }

        public ITextTemplatingCallback Callback { get; set; }

        public object Hierarchy { get; set; }

        #endregion
    }
}