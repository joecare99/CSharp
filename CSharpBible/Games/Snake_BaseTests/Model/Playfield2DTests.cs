using Microsoft.VisualStudio.TestTools.UnitTesting;
using Snake_Base.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snake_Base.Model.Tests
{
    /// <summary>
    /// Defines test class Playfield2DTests.
    /// </summary>
    [TestClass()]
    public class Playfield2DTests
    {
        private string ResultData="";

        /// <summary>
        /// Gets the test playfield.
        /// </summary>
        /// <value>The test playfield.</value>
        internal Playfield2D<TestItem>? testPlayfield { get; private set; }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        [TestInitialize]
        public void Init()
        {
            testPlayfield = new Playfield2D<TestItem>(new Size(4, 3));
            TestItem.logOperation += LogOperation;
            ResultData = "";
        }

        private void LogOperation(string sOperation, TestItem sender, object? oldVal, object? newVal, string sProp)
        {
            ResultData += $"{sOperation}: {sender}\to:{oldVal}\tn:{newVal}\tp:{sProp}\r\n";
        }

        /// <summary>
        /// Defines the test method Playfield2DTest.
        /// </summary>
        [TestMethod()]
        public void Playfield2DTest()
        {
            Assert.Fail();
        }

        /// <summary>
        /// Defines the test method AddItemTest.
        /// </summary>
        [TestMethod()]
        public void AddItemTest()
        {
            var _testItem = new TestItem();
            
        }

        /// <summary>
        /// Defines the test method RemoveItemTest.
        /// </summary>
        [TestMethod()]
        public void RemoveItemTest()
        {
            Assert.Fail();
        }

        /// <summary>
        /// Defines the test method GetItemsTest.
        /// </summary>
        [TestMethod()]
        public void GetItemsTest()
        {
            Assert.Fail();
        }

        private void LogDataChanged(object? sender, (string prop, object? oldVal, object? newVal) e)
        {
            ResultData += $"DataChange: {sender}\to:{e.oldVal}\tn:{e.newVal}\tc:{e.prop}\r\n";
        }

    }
}