// <copyright file="TransformationContextProviderTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.Tests
{
    using System;
    using System.ComponentModel.Design;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class TransformationContextProviderTest
    {
        [TestMethod]
        public void RegisterAddsServiceToContainer()
        {
            Assert.IsNotNull(CreateTransformationContextProvider());
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void UpdateOutputFilesThrowsArgumentNullExceptionWhenInputFileIsNull()
        {
            ITransformationContextProvider target = CreateTransformationContextProvider();
            target.UpdateOutputFiles(null, new OutputFile[0]);
        }

        [TestMethod, ExpectedException(typeof(ArgumentException))]
        public void UpdateOutputFilesThrowsArgumentExceptionWhenInputFileIsNotRooted()
        {
            ITransformationContextProvider target = CreateTransformationContextProvider();
            target.UpdateOutputFiles("input.tt", new OutputFile[0]);            
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void UpdateOutputFilesThrowsArgumentNullExceptionWhenOutputFilesIsNull()
        {
            ITransformationContextProvider target = CreateTransformationContextProvider();
            target.UpdateOutputFiles(@"C:\input.tt", null);
        }

        [TestMethod, ExpectedException(typeof(ArgumentException))]
        public void UpdateOutputFilesThrowsArgumentExceptionWhenAnyOutputFileIsNull()
        {
            ITransformationContextProvider target = CreateTransformationContextProvider();
            target.UpdateOutputFiles(@"C:\input.tt", new OutputFile[1]);            
        }

        private static ITransformationContextProvider CreateTransformationContextProvider()
        {
            using (var container = new ServiceContainer())
            {
                TransformationContextProvider.Register(container);
                return (ITransformationContextProvider)container.GetService(typeof(ITransformationContextProvider));                
            }
        }
    }
}