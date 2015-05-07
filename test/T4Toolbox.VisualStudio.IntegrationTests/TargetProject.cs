// <copyright file="TargetProject.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.IntegrationTests
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Provides access to test data in TargetProject.xml file for use in data-driven tests.
    /// </summary>
    internal class TargetProject
    {
        public const string Provider = "Microsoft.VisualStudio.TestTools.DataSource.XML";
        public const string Connection = @"|DataDirectory|\TargetProject.xml";
        public const string Table = "TargetProject";

        public TargetProject(TestContext testContext)
        {
            if (testContext.DataRow == null)
            {
                throw new InvalidOperationException("Test method does not have a valid DataSourceAttribute.");
            }

            this.Language = (string)testContext.DataRow["Language"];
            this.Template = (string)testContext.DataRow["Template"];
            this.CodeFileExtension = (string)testContext.DataRow["CodeFileExtension"];
            this.DefaultTextTemplateLanguage = (string)testContext.DataRow["DefaultTextTemplateLanguage"];

            // To help distinguish test run results in Test Explorer, dump them to the console output
            Console.WriteLine(
                "Language = '{0}', Template = '{1}', Code File Extension = '{2}', Default Text Template Language = '{3}'.", 
                new object[] { this.Language, this.Template, this.CodeFileExtension, this.DefaultTextTemplateLanguage });
        }

        public string Language { get; private set; }

        public string Template { get; private set; }

        public string CodeFileExtension { get; private set; }

        /// <summary>
        /// Gets the default T4 language, as specified in the &lt;#@ template #&gt; directive of a text template.
        /// </summary>
        public string DefaultTextTemplateLanguage { get; private set; }
    }
}