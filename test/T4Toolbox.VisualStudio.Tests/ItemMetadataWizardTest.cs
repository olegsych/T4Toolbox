// <copyright file="ItemMetadataWizardTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.Tests
{
    using System.Collections.Generic;
    using Microsoft.VisualStudio.TemplateWizard;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using T4Toolbox.VisualStudio.Tests.Fakes;

    [TestClass]
    public class ItemMetadataWizardTest
    {
        [TestMethod]
        public void ProjectItemFinishedGeneratingSetsMetadata()
        {
            using (var dte = new FakeDte())
            {
                var solution = new FakeSolution(dte);
                var project = new FakeProject(solution);
                var projectItem = new FakeProjectItem(project);

                var replacements = new Dictionary<string, string>();
                replacements["$wizarddata$"] = @"
                    <Properties>
                        <Generator>TextTemplatingFilePreprocessor</Generator>
                    </Properties>";

                var wizard = new ItemMetadataWizard();
                wizard.RunStarted(null, replacements, default(WizardRunKind), null);
                wizard.ProjectItemFinishedGenerating(projectItem);

                // Wizard should clear the value
                Assert.AreEqual("TextTemplatingFilePreprocessor", projectItem.GetItemAttribute("Generator"));
            }
        }

        [TestMethod]
        public void ProjectItemFinishedGeneratingRemovesEmptyMetadata()
        {
            using (var dte = new FakeDte())
            {
                var solution = new FakeSolution(dte);
                var project = new FakeProject(solution);
                var projectItem = new FakeProjectItem(project);
                projectItem.SetItemAttribute("Generator", "TextTemplatingFileGenerator");

                var replacements = new Dictionary<string, string>();
                replacements["$wizarddata$"] = @"
                    <Properties>
                        <Generator/>
                    </Properties>";

                var wizard = new ItemMetadataWizard();
                wizard.RunStarted(null, replacements, default(WizardRunKind), null);
                wizard.ProjectItemFinishedGenerating(projectItem);

                // Wizard should clear the value
                Assert.AreEqual(string.Empty, projectItem.GetItemAttribute("Generator"));
            }
        }
    }
}
