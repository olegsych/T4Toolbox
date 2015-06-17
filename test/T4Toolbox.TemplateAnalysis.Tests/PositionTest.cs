// <copyright file="PositionTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.TemplateAnalysis
{
    using Xunit;

    public static class PositionTest
    {
        [Fact]
        public static void PositionIsValueType() // to be lightweight
        {
            Assert.True(typeof(Position).IsValueType);
        }

        [Fact]
        public static void PositionIsInternal() // for internal consumption only
        {
            Assert.True(typeof(Position).IsNotPublic);
        }

        [Fact]
        public static void LineReturnsValueSpecifiedInConstructor()
        {
            var target = new Position(42, 0);
            Assert.Equal(42, target.Line);
        }

        [Fact]
        public static void ColumnReturnsValueSpecifiedInConstructor()
        {
            var target = new Position(0, 42);
            Assert.Equal(42, target.Column);
        }

        [Fact]
        public static void EqualsReturnsTrueWhenLineAndColumnAreSame()
        {
            var a = new Position(4, 2);
            var b = new Position(4, 2);
            Assert.Equal(a, b);
        }

        [Fact]
        public static void GetHashCodeReturnsValueCalculatedFromLineAndColumn()
        {
            var a = new Position(4, 2);
            var b = new Position(4, 2);
            Assert.Equal(a.GetHashCode(), b.GetHashCode());
        }

        [Fact]
        public static void ToStringReturnsLineAndColumnInParenthesis() // consistently with MSBuild messages
        {
            Assert.Equal("(4,2)", new Position(4, 2).ToString());
        }
    }
}