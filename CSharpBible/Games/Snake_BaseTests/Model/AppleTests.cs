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
    /// Defines test class AppleTests.
    /// </summary>
    [TestClass()]
    public class AppleTests
    {
        #region Properties
        private string ResultData="";

        #region TestResults
        private readonly string cExpResult0= "DataChange: Snake_Base.Model.Playfield2D`1[Snake_Base.Model.SnakeGameObject]\to:\tn:Snake_Base.Model.Apple\tp:Items\r\n";
        private readonly string cExpResult1= "DataChange: Snake_Base.Model.Playfield2D`1[Snake_Base.Model.SnakeGameObject]\to:\tn:Snake_Base.Model.Apple\tp:Items\r\n";
        private readonly string cExpResult2 = "DataChange: Snake_Base.Model.Playfield2D`1[Snake_Base.Model.SnakeGameObject]\to:\tn:Snake_Base.Model.Apple\tp:Items\r\n";
        private readonly string cExpResult3 ="";
        private readonly string cExpResult4 = "DataChange: Snake_Base.Model.Playfield2D`1[Snake_Base.Model.SnakeGameObject]\to:\tn:Snake_Base.Model.Apple\tp:Items\r\n";
        private readonly string cExpResultMove= "DataChange: Snake_Base.Model.Playfield2D`1[Snake_Base.Model.SnakeGameObject]\to:\tn:Snake_Base.Model.Apple\tp:Items\r\nDataChange: Snake_Base.Model.Apple\to:{X=1,Y=2}\tn:{X=2,Y=0}\tp:Place\r\n";
        #endregion

        /// <summary>
        /// The PLF
        /// </summary>
        internal Playfield2D<SnakeGameObject>? plf;
        #endregion
        /// <summary>
        /// Initializes this instance.
        /// </summary>
        [TestInitialize()]
        public void Init()
        {
            ResultData = "";
            plf = new Playfield2D<SnakeGameObject>(new Size(3, 3));
            plf.OnDataChanged += LogDataChanged;
        }

        /// <summary>
        /// Defines the test method AppleTest2.
        /// </summary>
        [TestMethod()]
        public void AppleTest2()
        {
            Point pp = Point.Empty;    
            var _testApple = new Apple(pp=new Point(1,2),plf);
            Assert.AreEqual(_testApple, plf[pp]);
            Assert.AreEqual(cExpResult2, ResultData);
        }

        /// <summary>
        /// Defines the test method AppleTest1.
        /// </summary>
        [TestMethod()]
        public void AppleTest1()
        {
            Point pp = Point.Empty;
            var _testApple = new Apple(pp = new Point(2, 1));
            Assert.AreEqual(null, plf[pp]);
            _testApple.Parent = plf;
            Assert.AreEqual(_testApple, plf[pp]);
            Assert.AreEqual(cExpResult1, ResultData);
        }

        /// <summary>
        /// Defines the test method AppleTest0.
        /// </summary>
        [TestMethod()]
        public void AppleTest0()
        {
            Point pp = new Point(2, 0);
            var _testApple = new Apple();
            _testApple.Place = pp; 
            Assert.AreEqual(null, plf[pp]);
            _testApple.Parent = plf;
            Assert.AreEqual(_testApple, plf[pp]);
            Assert.AreEqual(cExpResult0, ResultData);
        }

        /// <summary>
        /// Defines the test method AppleTest4.
        /// </summary>
        [TestMethod()]
        public void AppleTest4()
        {
            Apple.DefaultParent = plf;
            Point pp = Point.Empty;
            var _testApple = new Apple(pp = new Point(1, 2));
            Assert.AreEqual(_testApple, plf[pp]);
            Assert.AreEqual(cExpResult4, ResultData);
        }

        /// <summary>
        /// Defines the test method AppleMoveTest.
        /// </summary>
        [TestMethod()]
        public void AppleMoveTest()
        {
            Apple.DefaultParent = plf;
            Point pp = Point.Empty;
            var _testApple = new Apple(pp = new Point(1, 2));
            Assert.AreEqual(_testApple, plf[pp]);
            Point pp1 = new Point(2, 0);
            Assert.AreEqual(null, plf[pp1]);
            _testApple.Place = pp1;
            Assert.AreEqual(_testApple, plf[pp1]);
            Assert.AreEqual(null, plf[pp]);
            Assert.AreEqual(cExpResultMove, ResultData);
        }

        private void LogDataChanged(object? sender, (string prop, object? oldVal, object? newVal) e)
        {
            ResultData += $"DataChange: {sender}\to:{e.oldVal}\tn:{e.newVal}\tp:{e.prop}\r\n";
        }

    }
}