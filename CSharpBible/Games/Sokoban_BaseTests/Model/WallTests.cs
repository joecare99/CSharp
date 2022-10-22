using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sokoban_Base.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sokoban_Base.Model.Tests
{
    /// <summary>
    /// Defines test class WallTests.
    /// </summary>
    [TestClass()]
    public class WallTests
    {
        /// <summary>
        /// Defines the test method WallTest.
        /// </summary>
        [TestMethod()]
        public void WallTest()
        {
            var w = new Wall(new Point(2, 3), null);
            Assert.AreEqual(new Point(2, 3), w.Position);
            Assert.IsNull(w.Item);
            w.Item = null;
            Assert.ThrowsException<NotImplementedException>(()=>w.Item = new Stone(null));
        }
    }
}