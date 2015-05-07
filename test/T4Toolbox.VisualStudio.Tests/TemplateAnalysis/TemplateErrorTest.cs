// <copyright file="TemplateErrorTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.TemplateAnalysis
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.VisualStudio.Text;

    [TestClass]
    public class TemplateErrorTest
    {
        [TestMethod]
        public void MessageReturnsValueSpecifiedInConstructor()
        {
            var target = new TemplateError("42", default(Span), default(Position));
            Assert.AreEqual("42", target.Message);
        }

        [TestMethod]
        public void PositionReturnsValueSpecifiedInConstructor()
        {
            var target = new TemplateError(string.Empty, default(Span), new Position(4, 2));
            Assert.AreEqual(new Position(4, 2), target.Position);
        }

        [TestMethod]
        public void SpanReturnsValueSpecifiedInConstructor()
        {
            var target = new TemplateError(string.Empty, new Span(4, 2), default(Position));
            Assert.AreEqual(new Span(4, 2), target.Span);
        }
    }
}