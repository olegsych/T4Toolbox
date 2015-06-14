// <copyright file="PositionTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.TemplateAnalysis
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class PositionTest
    {
        [TestMethod]
        public void PositionIsValueType() // to be lightweight
        {
            Assert.IsTrue(typeof(Position).IsValueType);
        }

        [TestMethod]
        public void PositionIsInternal() // for internal consumption only
        {
            Assert.IsTrue(typeof(Position).IsNotPublic);
        }

        [TestMethod]
        public void LineReturnsValueSpecifiedInConstructor()
        {
            var target = new Position(42, 0);
            Assert.AreEqual(42, target.Line);
        }

        [TestMethod]
        public void ColumnReturnsValueSpecifiedInConstructor()
        {
            var target = new Position(0, 42);
            Assert.AreEqual(42, target.Column);
        }

        [TestMethod]
        public void EqualsReturnsTrueWhenLineAndColumnAreSame()
        {
            var a = new Position(4, 2);
            var b = new Position(4, 2);
            Assert.AreEqual(a, b);
        }

        [TestMethod]
        public void GetHashCodeReturnsValueCalculatedFromLineAndColumn()
        {
            var a = new Position(4, 2);
            var b = new Position(4, 2);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [TestMethod]
        public void ToStringReturnsLineAndColumnInParenthesis() // consistently with MSBuild messages
        {
            Assert.AreEqual("(4,2)", new Position(4, 2).ToString());
        }
    }
}