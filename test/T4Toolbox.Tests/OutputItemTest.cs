// <copyright file="OutputItemTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.Tests
{
    using System;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// A test class for <see cref="OutputItem"/>.
    /// </summary>
    [TestClass]
    public class OutputItemTest
    {
        #region CopyToOutputDirectory

        [TestMethod]
        public void CopyToOutputDirectoryIsDoNotCopyByDefault()
        {
            var output = new OutputItem();
            Assert.AreEqual(CopyToOutputDirectory.DoNotCopy, output.CopyToOutputDirectory);
        }

        [TestMethod]
        public void CopyToOutputDirectoryCanBeSet()
        {
            var output = new OutputItem();
            
            output.CopyToOutputDirectory = CopyToOutputDirectory.CopyAlways;
            Assert.AreEqual(CopyToOutputDirectory.CopyAlways, output.CopyToOutputDirectory);

            output.CopyToOutputDirectory = CopyToOutputDirectory.CopyIfNewer;
            Assert.AreEqual(CopyToOutputDirectory.CopyIfNewer, output.CopyToOutputDirectory);

            output.CopyToOutputDirectory = CopyToOutputDirectory.DoNotCopy;
            Assert.AreEqual(CopyToOutputDirectory.DoNotCopy, output.CopyToOutputDirectory);
        }

        [TestMethod]
        public void CopyToOutputDirectoryIsStoredAsMetadata()
        {
            var output = new OutputItem();
            
            output.CopyToOutputDirectory = CopyToOutputDirectory.DoNotCopy;
            Assert.AreEqual(string.Empty, output.Metadata[ItemMetadata.CopyToOutputDirectory]);

            output.CopyToOutputDirectory = CopyToOutputDirectory.CopyAlways;
            Assert.AreEqual("Always", output.Metadata[ItemMetadata.CopyToOutputDirectory]);

            output.CopyToOutputDirectory = CopyToOutputDirectory.CopyIfNewer;
            Assert.AreEqual("PreserveNewest", output.Metadata[ItemMetadata.CopyToOutputDirectory]);
        }

        #endregion

        #region CustomTool

        [TestMethod]
        public void CustomToolIsEmptyStringByDefault()
        {
            var output = new OutputItem();
            Assert.AreEqual(string.Empty, output.CustomTool);
        }

        [TestMethod]
        public void CustomToolCanBeSet()
        {
            var output = new OutputItem { CustomTool = "TextTemplatingFileGenerator" };
            Assert.AreEqual("TextTemplatingFileGenerator", output.CustomTool);
        }

        [TestMethod]
        public void CustomToolIsStoredAsMetadata()
        {
            var output = new OutputItem { CustomTool = "TextTemplatingFileGenerator" };
            Assert.AreEqual("TextTemplatingFileGenerator", output.Metadata[ItemMetadata.Generator]);
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void CustomToolThrowsArgumentNullExceptionWhenNewValueIsNull()
        {
            var output = new OutputItem();
            output.CustomTool = null;
        }

        #endregion

        #region CustomToolNamespace

        [TestMethod]
        public void CustomToolNamespaceIsEmptyStringByDefault()
        {
            var output = new OutputItem();
            Assert.AreEqual(string.Empty, output.CustomToolNamespace);
        }

        [TestMethod]
        public void CustomToolNamespaceCanBeSet()
        {
            var output = new OutputItem { CustomToolNamespace = "T4Toolbox" };
            Assert.AreEqual("T4Toolbox", output.CustomToolNamespace);
        }

        [TestMethod]
        public void CustomToolNamespaceIsStoredAsMetadata()
        {
            var output = new OutputItem { CustomToolNamespace = "T4Toolbox" };
            Assert.AreEqual("T4Toolbox", output.Metadata[ItemMetadata.CustomToolNamespace]);            
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void CustomToolNamespaceThrowsArgumentNullExceptionWhenNewValueIsNull()
        {
            var output = new OutputItem();
            output.CustomToolNamespace = null;
        }

        #endregion

        #region Directory

        [TestMethod]
        public void DirectoryIsEmptyStringByDefault()
        {
            var output = new OutputItem();
            Assert.AreEqual(string.Empty, output.Directory);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DirectoryThrowsArgumentNullExceptionWhenNewValueIsNull()
        {
            var output = new OutputItem();
            output.Directory = null;
        }

        #endregion

        #region Encoding

        [TestMethod]
        public void EncodingIsUtf8ByDefault()
        {
            var output = new OutputItem();
            Assert.AreEqual(Encoding.UTF8, output.Encoding);
        }

        [TestMethod]
        public void EncodingCanBeSet()
        {
            var output = new OutputItem { Encoding = Encoding.ASCII };
            Assert.AreEqual(Encoding.ASCII, output.Encoding);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void EncodingThrowsArgumentNullExceptionWhenNewValueIsNull()
        {
            var output = new OutputItem();
            output.Encoding = null;
        }

        #endregion

        #region File

        [TestMethod]
        public void FileIsEmptyStringByDefault()
        {
            var output = new OutputItem();
            Assert.AreEqual(string.Empty, output.File);
        }

        [TestMethod]
        public void FileCanBeSet()
        {
            var output = new OutputItem { File = "Test.cs" };
            Assert.AreEqual("Test.cs", output.File);
        }

        [TestMethod]
        public void FileUpdatesDirectoryPropertyWhenItIncludesDirectoryName()
        {
            var output = new OutputItem { File = @"Folder\Test.cs" };
            Assert.AreEqual("Test.cs", output.File);
            Assert.AreEqual("Folder", output.Directory);
        }

        [TestMethod]
        public void FilePreservesDirectoryPropertyWhenItDoesNotIncludeDirectoryName()
        {
            var output = new OutputItem { Directory = "Folder", File = "NewTest.cs" };
            Assert.AreEqual("Folder", output.Directory);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FileThrowsArgumentNullExceptionWhenNewValueIsNull()
        {
            var output = new OutputItem();
            output.File = null;
        }

        #endregion

        #region ItemType

        [TestMethod]
        public void ItemTypeIsEmptyStringByDefault()
        {
            var output = new OutputItem();
            Assert.AreEqual(string.Empty, output.ItemType);
        }

        [TestMethod]
        public void ItemTypeCanBeSet()
        {
            var output = new OutputItem { ItemType = ItemType.Compile };
            Assert.AreEqual(ItemType.Compile, output.ItemType);
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void ItemTypeThrowsArgumentNullExceptionWhenNewValueIsNull()
        {
            var output = new OutputItem();
            output.ItemType = null;
        }

        #endregion

        #region Metadata

        [TestMethod]
        public void MetadataIsEmptyByDefault()
        {
            var output = new OutputItem();
            Assert.AreEqual(0, output.Metadata.Count);
        }

        [TestMethod]
        public void MetadataIsNotCaseSensitive()
        {
            var output = new OutputItem();
            output.Metadata["TEST"] = "value";
            Assert.AreEqual("value", output.Metadata["test"]);
        }

        #endregion

        #region Path

        [TestMethod]
        public void PathReturnsEmptyStringByDefault()
        {
            var output = new OutputItem();
            Assert.AreEqual(string.Empty, output.Path);
        }

        [TestMethod]
        public void PathReturnsFileName()
        {
            var output = new OutputItem { File = "Test.cs" };
            Assert.AreEqual("Test.cs", output.Path);
        }

        [TestMethod]
        public void PathCombinesDirectoryAndFileName()
        {
            var output = new OutputItem { File = "Test.cs", Directory = "Folder" };
            Assert.AreEqual(@"Folder\Test.cs", output.Path);
        }

        [TestMethod]
        public void PathCombinesProjectDirectoryAndFileName()
        {
            var output = new OutputItem { File = "Test.cs", Project = @"Project\Test.proj" };
            Assert.AreEqual(@"Project\Test.cs", output.Path);           
        }

        [TestMethod]
        public void PathCombinesDirectoryAndProjectDirectory()
        {
            var output = new OutputItem { File = "Test.cs", Directory = "Folder", Project = @"Project\Test.proj" };
            Assert.AreEqual(@"Project\Folder\Test.cs", output.Path);
        }

        [TestMethod]
        public void PathReturnsIgnoresProjectWhenDirectoryIsRooted()
        {
            var output = new OutputItem { File = @"C:\Folder\Test.cs", Project = @"Project\Test.proj" };
            Assert.AreEqual(@"C:\Folder\Test.cs", output.Path);
        }

        #endregion

        #region Project

        [TestMethod]
        public void ProjectIsEmptyStringByDefault()
        {
            var output = new OutputItem();
            Assert.AreEqual(string.Empty, output.Project);
        }

        [TestMethod]
        public void ProjectCanBeSet()
        {
            var output = new OutputItem { Project = "Test.proj" };
            Assert.AreEqual("Test.proj", output.Project);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ProjectThrowsArgumentNullExceptionWhenNewValueIsNull()
        {
            var output = new OutputItem();
            output.Project = null;
        }

        #endregion

        #region References

        [TestMethod]
        public void ReferencesIsEmptyByDefault()
        {
            var output = new OutputItem();
            Assert.AreEqual(0, output.References.Count);
        }

        #endregion
    }
}
