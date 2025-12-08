// ***********************************************************************
// Assembly         : Avln_BaseLibTests
// Author           : Mir
// Created          : 01-17-2025
//
// Last Modified By : Mir
// Last Modified On : 01-17-2025
// ***********************************************************************
// <copyright file="TimerProxyTests.cs" company="JC-Soft">
//     Copyright © JC-Soft 2025
// </copyright>
// <summary></summary>
// ***********************************************************************
using Microsoft.VisualStudio.TestTools.UnitTesting;

/// <summary>
/// The Tests namespace.
/// </summary>
namespace BaseLib.Models.Tests
{
    /// <summary>
    /// Defines test class TimerProxyTests.
    /// </summary>
    [TestClass()]
    public class TimerProxyTests
    {
        /// <summary>
        /// Defines the test method TimerProxyTest.
        /// </summary>
        [TestMethod()]
        public void TimerProxyTest()
        {
            // Arrange
            var tp = new TimerProxy();

            // Act
            tp.Interval = 1000;

            // Assert
            Assert.IsNotNull(tp);
            Assert.IsInstanceOfType(tp, typeof(TimerProxy));
            Assert.AreEqual(1000, tp.Interval);
        }
    }
}