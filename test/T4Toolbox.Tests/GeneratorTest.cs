// <copyright file="GeneratorTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.Tests
{
    using System;
    using System.CodeDom.Compiler;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This is a test class for <see cref="Generator"/> and is intended to contain all of its unit tests.
    /// </summary>
    [TestClass]
    public class GeneratorTest : IDisposable
    {
        private const string TestMessage = "Test Message";

        private readonly FakeGenerator generator = new FakeGenerator();

        private readonly FakeTransformation transformation;

        public GeneratorTest()
        {
            this.transformation = new FakeTransformation();
            TransformationContext.Initialize(this.transformation, this.transformation.GenerationEnvironment);
        }

        public void Dispose()
        {
            TransformationContext.Cleanup();
            this.transformation.Dispose();
        }

        #region Context

        [TestMethod]
        public void ContextReturnsTransformationContextByDefault()
        {
            Assert.AreSame(TransformationContext.Current, this.generator.Context);
        }

        [TestMethod]
        public void ContextCanBeSet()
        {
            using (var transformation = new FakeTransformation())
            using (var context = new TransformationContext(transformation, transformation.GenerationEnvironment))
            {
                this.generator.Context = context;
                Assert.AreSame(context, this.generator.Context);
            }
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void ContextThrowsArgumentNullExceptionWhenNewValueIsNull() // Because allowing it would magically change property value to TransformationContext.Current.
        {
            this.generator.Context = null;
        }

        #endregion

        #region Error(string)

        [TestMethod]
        public void ErrorAddsNewErrorToErrorsCollection()
        {
            this.generator.Error(TestMessage);
            Assert.AreEqual(1, this.generator.Errors.Count);
            Assert.AreEqual(TestMessage, this.generator.Errors[0].ErrorText);
            Assert.AreEqual(false, this.generator.Errors[0].IsWarning);
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void ErrorThrowsArgumentNullExceptionWhenMessageIsNull()
        {
            this.generator.Error(null);
        }

        #endregion

        #region Error(string, params object[])

        [TestMethod]
        public void ErrorFormatAddsNewErrorToErrorsCollection()
        {
            this.generator.Error("{0}", TestMessage);
            Assert.AreEqual(1, this.generator.Errors.Count);
            Assert.AreEqual(TestMessage, this.generator.Errors[0].ErrorText);
            Assert.AreEqual(false, this.generator.Errors[0].IsWarning);
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void ErrorFormatThrowsArgumentNullExceptionWhenFormatIsNull()
        {
            this.generator.Error(null, null);
        }

        #endregion

        #region Errors

        [TestMethod]
        public void ErrorsIsNotNull()
        {
            Assert.IsNotNull(this.generator.Errors);
        }

        #endregion

        #region Run

        [TestMethod]
        public void RunValidatesGenerator()
        {
            bool validated = false;
            this.generator.Validated = () => validated = true;
            this.generator.Run();
            Assert.IsTrue(validated);
        }

        [TestMethod]
        public void RunSkipsRunCoreIfValidateGeneratesErrors()
        {
            this.generator.Validated = () => this.generator.Error("Test Error");

            bool runCoreExecuted = false;
            this.generator.RanCore = () => runCoreExecuted = true;

            this.generator.Run();
            Assert.IsFalse(runCoreExecuted);
        }

        [TestMethod]
        public void RunExecutesRunCoreIfValidateGeneratesWarnings()
        {
            this.generator.Validated = () => this.generator.Warning(TestMessage);

            bool runCoreExecuted = false;
            this.generator.RanCore = () => runCoreExecuted = true;

            this.generator.Run();
            Assert.IsTrue(runCoreExecuted);
        }

        [TestMethod]
        public void RunReportsTransformationExceptionsAsError()
        {
            const string ErrorMessage = "Test Error";
            this.generator.Validated = () => { throw new TransformationException(ErrorMessage); };

            this.generator.Run();
            Assert.AreEqual(1, this.generator.Errors.Count);
            Assert.AreEqual(ErrorMessage, this.generator.Errors[0].ErrorText);
        }

        [TestMethod]
        public void RunReportsErrorsToTransformation()
        {
            this.generator.Validated = () => this.generator.Warning(TestMessage);
            this.generator.Run();
            Assert.AreEqual(1, this.transformation.Errors.Count);
            Assert.AreEqual(TestMessage, this.transformation.Errors[0].ErrorText);
        }

        [TestMethod]
        public void RunClearsPreviousErrorsToAvoidReportingThemToTransformationMoreThanOnce()
        {
            this.generator.Error(TestMessage);
            this.generator.Run();
            Assert.AreEqual(0, this.generator.Errors.Count);
        }

        [TestMethod]
        public void RunReportsInputFileInErrorsOfInputFileBasedTransformation()
        {
            this.transformation.Session[TransformationContext.InputFileNameKey] = "Input.cs";
            this.generator.Validated = () => this.generator.Error(TestMessage);
            this.generator.Run();

            var error = this.transformation.Errors.Cast<CompilerError>().Single();
            Assert.AreEqual("Input.cs", error.FileName);
        }

        [TestMethod]
        public void RunReportsTemplateFileInErrorsOfTemplateFileBasedTransformation()
        {
            this.transformation.Host.TemplateFile = "Template.tt";
            this.generator.Validated = () => this.generator.Error(TestMessage);
            this.generator.Run();

            var error = this.transformation.Errors.Cast<CompilerError>().Single();
            Assert.AreEqual("Template.tt", error.FileName);
        }

        #endregion

        #region Warning(string)

        [TestMethod]
        public void WarningAddsWarningToErrorsCollection()
        {
            const string WarningMessage = TestMessage;
            this.generator.Warning(WarningMessage);
            Assert.AreEqual(1, this.generator.Errors.Count);
            Assert.AreEqual(WarningMessage, this.generator.Errors[0].ErrorText);
            Assert.AreEqual(true, this.generator.Errors[0].IsWarning);
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void WarningThrowsArgumentNullExceptionWhenMessageIsNull()
        {
            this.generator.Warning(null);
        }

        #endregion

        #region Warning(string, params object[])

        [TestMethod]
        public void WarningFormatAddsWarningToErrorsCollection()
        {
            this.generator.Warning("{0}", TestMessage);
            Assert.AreEqual(1, this.generator.Errors.Count);
            Assert.AreEqual(TestMessage, this.generator.Errors[0].ErrorText);
            Assert.AreEqual(true, this.generator.Errors[0].IsWarning);
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void WarningFormatThrowsArgumentNullExceptionWhenFormatIsNull()
        {
            this.generator.Warning(null, null);
        }

        #endregion
    }
}
