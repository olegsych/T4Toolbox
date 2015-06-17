// <copyright file="TemplateErrorTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.TemplateAnalysis
{
    using Microsoft.VisualStudio.Text;
    using Xunit;

    public static class TemplateErrorTest
    {
        [Fact]
        public static void MessageReturnsValueSpecifiedInConstructor()
        {
            var target = new TemplateError("42", default(Span), default(Position));
            Assert.Equal("42", target.Message);
        }

        [Fact]
        public static void PositionReturnsValueSpecifiedInConstructor()
        {
            var target = new TemplateError(string.Empty, default(Span), new Position(4, 2));
            Assert.Equal(new Position(4, 2), target.Position);
        }

        [Fact]
        public static void SpanReturnsValueSpecifiedInConstructor()
        {
            var target = new TemplateError(string.Empty, new Span(4, 2), default(Position));
            Assert.Equal(new Span(4, 2), target.Span);
        }
    }
}