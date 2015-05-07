// <copyright file="TemplateTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.Tests
{
    using System;
    using System.CodeDom.Compiler;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.VisualStudio.TextTemplating;

    [TestClass]
    public class TemplateTest : IDisposable
    {
        private const string TestFile = "Test.txt";

        private const string TestMessage = "Test Message";

        private const string TestOutput = "Test Template Output";

        private FakeTransformation transformation;

        public TemplateTest()
        {
            this.transformation = new FakeTransformation();
            TransformationContext.Initialize(this.transformation, this.transformation.GenerationEnvironment);
        }

        public void Dispose()
        {
            if (this.transformation != null)
            {
                TransformationContext.Cleanup();
                this.transformation.Dispose();
                this.transformation = null;
            }
        }

        #region Context

        [TestMethod]
        public void ContextReturnsTransformationContextByDefault()
        {
            using (var template = new FakeTemplate())
            {
                Assert.AreSame(TransformationContext.Current, template.Context);
            }
        }

        [TestMethod]
        public void ContextCanBeSet()
        {
            using (var transformation = new FakeTransformation())
            using (var context = new TransformationContext(transformation, transformation.GenerationEnvironment))
            using (var template = new FakeTemplate())
            {
                template.Context = context;
                Assert.AreSame(context, template.Context);
            }
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void ContextThrowsArgumentNullExceptionWhenNewValueIsNull() // Because allowing it would magically change property value to TransformationContext.Current.
        {
            using (var template = new FakeTemplate())
            {
                template.Context = null;
            }
        }

        #endregion

        #region Enabled

        [TestMethod]
        public void EnabledIsTrueByDefault()
        {
            using (var template = new FakeTemplate())
            {
                Assert.IsTrue(template.Enabled);
            }
        }

        [TestMethod]
        public void EnabledCanBeSet()
        {
            using (var template = new FakeTemplate())
            {
                template.Enabled = false;
                Assert.AreEqual(false, template.Enabled);
            }
        }

        #endregion

        #region Error(string)

        [TestMethod]
        public void ErrorAddsNewErrorToErrorsCollection()
        {
            using (var template = new FakeTemplate())
            {
                template.Error(TestMessage);
                Assert.AreEqual(1, template.Errors.Count);
                Assert.AreEqual(TestMessage, template.Errors[0].ErrorText);
                Assert.AreEqual(false, template.Errors[0].IsWarning);                
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ErrorThrowsArgumentNullExceptionWhenMessageIsNull()
        {
            using (var template = new FakeTemplate())
            {
                template.Error(null);
            }
        }

        #endregion

        #region Error(string, params object[])

        [TestMethod]
        public void ErrorFormatAddsNewErrorToErrorsCollection()
        {
            using (var template = new FakeTemplate())
            {
                template.Error("{0}", TestMessage);
                Assert.AreEqual(1, template.Errors.Count);
                Assert.AreEqual(TestMessage, template.Errors[0].ErrorText);
                Assert.AreEqual(false, template.Errors[0].IsWarning);                
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ErrorFormatThrowsNullArgumentExceptionWhenFormatIsNull()
        {
            using (var template = new FakeTemplate())
            {
                template.Error(null, null);                
            }
        }

        #endregion

        #region Errors

        [TestMethod]
        public void ErrorsIsNotNull()
        {
            using (var template = new FakeTemplate())
            {
                Assert.IsNotNull(template.Errors);                
            }
        }

        #endregion

        #region Render

        [TestMethod]
        public void RenderDoesNotTransformTemplateWhenEnabledIsFalse()
        {
            using (var template = new FakeTemplate())
            {
                template.Enabled = false;
                bool transformed = false;
                template.TransformedText = () => transformed = true;
                template.Render();
                Assert.IsFalse(transformed);
            }
        }

        [TestMethod]
        public void RenderRaisesRenderingEvent()
        {
            using (var template = new FakeTemplate())
            {
                bool eventRaised = false;
                template.Rendering += delegate { eventRaised = true; };
                template.Render();
                Assert.AreEqual(true, eventRaised);                
            }
        }

        [TestMethod]
        public void RenderTransformsTemplateWhenEnabledIsSetByRenderingEventHandler()
        {
            using (var template = new FakeTemplate())
            {
                template.Enabled = false;
                bool transformed = false;
                template.Rendering += delegate { template.Enabled = true; };
                template.TransformedText = () => transformed = true;
                template.Render();
                Assert.IsTrue(transformed);
            }            
        }

        [TestMethod]
        public void RenderReportsTransformationExceptionsAsErrors()
        {
            using (var template = new FakeTemplate())
            {
                template.Validated = () => { throw new TransformationException(TestMessage); };
                template.Render();
                AssertSingleError(template.Errors, TestMessage);
            }
        }

        [TestMethod]
        public void RenderReportsTemplateValidationErrorsToTransformation()
        {
            using (var template = new FakeTemplate())
            {
                template.Validated = () => template.Error(TestMessage);
                template.Render();
                AssertSingleError(this.transformation.Errors, TestMessage);
            }
        }

        [TestMethod]
        public void RenderReportsInputFileInErrorsOfInputFileBasedTransformation()
        {
            this.transformation.Session[TransformationContext.InputFileNameKey] = "Input.cs";

            using (var template = new FakeTemplate())
            {
                template.Validated = () => template.Error(TestMessage);
                template.Render();
            }

            var error = this.transformation.Errors.Cast<CompilerError>().Single();
            Assert.AreEqual("Input.cs", error.FileName);
        }

        [TestMethod]
        public void RenderReportsTemplateFileInErrorsOfTemplateFileBasedTransformation()
        {
            this.transformation.Host.TemplateFile = "Template.tt";

            using (var template = new FakeTemplate())
            {
                template.Validated = () => template.Error(TestMessage);
                template.Render();
            }

            var error = this.transformation.Errors.Cast<CompilerError>().Single();
            Assert.AreEqual("Template.tt", error.FileName);            
        }

        #endregion 

        #region RenderToFile

        [TestMethod]
        public void RenderToFileSetsOutputFile()
        {
            using (var template = new FakeTemplate())
            {
                template.RenderToFile(TestFile);
                Assert.AreEqual(TestFile, template.Output.File);
            }
        }

        [TestMethod]
        public void RenderToFileRendersTheTemplate()
        {
            OutputFile[] outputFiles = null;
            this.transformation.Host.UpdatedOutputFiles = (input, outputs) => outputFiles = outputs;

            using (var template = new FakeTemplate())
            {
                template.TransformedText = () => template.Write(TestOutput);
                template.RenderToFile(TestFile);
            }

            this.Dispose(); // Force the end of transformation

            OutputFile outputFile = outputFiles.Single(output => output.File == TestFile);
            Assert.AreEqual(TestOutput, outputFile.Content.ToString());
        }

        #endregion

        #region Session

        [TestMethod]
        public void SessionReturnsTransformationSession()
        {
            using (var template = new FakeTemplate())
            {
                Assert.AreSame(this.transformation.Session, template.Session);
            }
        }

        #endregion

        #region Transform

        [TestMethod]
        public void TransformRunsCodeGeneratedByDirectiveProcessors()
        {
            using (var template = new FakeTemplate())
            {
                bool initialized = false;
                template.Initialized = () => initialized = true;
                template.Transform();
                Assert.IsTrue(initialized);
            }                        
        }

        [TestMethod]
        public void TransformValidatesTemplate()
        {
            using (var template = new FakeTemplate())
            {
                template.Validated = () => template.Error(TestMessage);
                template.Transform();
                Assert.AreEqual(1, template.Errors.Count);
            }            
        }

        [TestMethod, ExpectedException(typeof(TransformationException))]
        public void TransformDoesNotCatchTransformationException()
        {
            using (var template = new FakeTemplate())
            {
                template.Validated = () => { throw new TransformationException(); };
                template.Transform();
            }
        }

        [TestMethod]
        public void TransformDoesNotGenerateOutputWhenValidateReportsErrors()
        {
            using (var template = new FakeTemplate())
            {
                template.TransformedText = () => template.WriteLine(TestOutput);
                template.Validated = () => template.Error(TestMessage);
                Assert.AreEqual(string.Empty, template.Transform());
            }
        }

        [TestMethod]
        public void TransformGeneratesOutputWhenValidateReportsWarnings()
        {
            using (var template = new FakeTemplate())
            {
                template.TransformedText = () => template.Write(TestOutput);
                template.Validated = () => template.Warning(TestMessage);
                Assert.AreEqual(TestOutput, template.Transform());
            }
        }

        [TestMethod]
        public void TransformDoesNotValidateOutputProperties()
        {
            using (var template = new FakeTemplate())
            {
                template.Output.Project = "Test.proj";
                template.Transform();
                Assert.AreEqual(0, template.Errors.Count);
            }
        }

        [TestMethod]
        public void TransformClearsPreviousOutputToAllowGeneratingMultipleOutputsFromSingleTemplate()
        {
            using (var template = new FakeTemplate())
            {
                template.TransformedText = () => template.Write("First Output");
                template.Transform();

                template.TransformedText = () => template.Write(TestOutput);
                Assert.AreEqual(TestOutput, template.Transform());
            }
        }
        
        [TestMethod]
        public void TransformClearsPreviousErrorsToAllowTransformingSameTemplateMultipleTimes()
        {
            using (var template = new FakeTemplate())
            {
                template.Validated = () => template.Error(TestMessage);
                template.Transform();

                template.Validated = null;
                template.TransformedText = () => template.Write(TestOutput);
                Assert.AreEqual(TestOutput, template.Transform());
            }            
        }

        #endregion

        #region Warning(string)

        [TestMethod]
        public void WarningAddsNewWarningToErrorsCollection()
        {
            using (var template = new FakeTemplate())
            {
                template.Warning(TestMessage);
                Assert.AreEqual(1, template.Errors.Count);
                Assert.AreEqual(TestMessage, template.Errors[0].ErrorText);
                Assert.AreEqual(true, template.Errors[0].IsWarning);                
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void WarningThrowsArgumentNullExceptionWhenMessageIsNull()
        {
            using (var template = new FakeTemplate())
            {
                template.Warning(null);                
            }
        }

        #endregion

        #region Warning(string, params object[])

        [TestMethod]
        public void WarningFormatAddsNewWarningToErrorsCollection()
        {
            using (var template = new FakeTemplate())
            {
                template.Warning("{0}", TestMessage);
                Assert.AreEqual(1, template.Errors.Count);
                Assert.AreEqual(TestMessage, template.Errors[0].ErrorText);
                Assert.AreEqual(true, template.Errors[0].IsWarning);                
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void WarningFormatThrowsArgumentNullExceptionWhenFormatIsNull()
        {
            using (var template = new FakeTemplate())
            {
                template.Warning(null, null);                
            }
        }

        #endregion

        private static void AssertSingleError(CompilerErrorCollection errors, params string[] keywords)
        {
            var error = errors.Cast<CompilerError>().Single();
            foreach (string keyword in keywords)
            {
                StringAssert.Contains(error.ErrorText, keyword);
            }
        }
    }
}
