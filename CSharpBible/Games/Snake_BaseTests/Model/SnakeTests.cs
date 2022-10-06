﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
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
    /// Defines test class SnakeTests.
    /// </summary>
    [TestClass()]
    public class SnakeTests
    {
        #region Properties
        #region private Properties
        private Playfield2D<SnakeGameObject>? playfield;
        private string ResultData="";
        #endregion

        #region Expected test-data
        private readonly string cExpSnakeTest = "DataChange: Snake_Base.Model.Playfield2D`1[Snake_Base.Model.SnakeGameObject]\to:\tn:Snake_Base.Model.SnakeTail\tp:Items\r\nDataChange: Snake_Base.Model.Playfield2D`1[Snake_Base.Model.SnakeGameObject]\to:\tn:Snake_Base.Model.SnakeHead\tp:Items\r\n";
        #endregion
        #endregion
        #region Methods
        /// <summary>
        /// Initializes this instance.
        /// </summary>
        [TestInitialize]
        public void Init()
        {
            playfield = new Playfield2D<SnakeGameObject>(new Size(4,3));
            playfield.OnDataChanged += LogDataChanged;
            SnakeGameObject.DefaultParent = playfield;
            ResultData = "";
        }

        /// <summary>
        /// Defines the test method TestSetup.
        /// </summary>
        [TestMethod()]
        public void TestSetup()
        {
            Assert.IsNotNull(playfield);
        }

        /// <summary>
        /// Defines the test method SnakeTest.
        /// </summary>
        [TestMethod()]
        public void SnakeTest()
        {
            var testSnake = new Snake(new Point(2, 1));
            Assert.IsNotNull(testSnake);
            Assert.IsNotNull(playfield![new Point(2,1)]);
            Assert.IsTrue(testSnake.alive);
            Assert.AreEqual(1, playfield!.Items.Count());
            Assert.IsInstanceOfType(playfield![new Point(2, 1)], typeof(SnakeHead));
            Assert.AreEqual(cExpSnakeTest, ResultData);
        }

        /// <summary>
        /// Snakes the move test.
        /// </summary>
        /// <param name="TestName">Name of the test.</param>
        /// <param name="start">The start.</param>
        /// <param name="dir">The dir.</param>
        /// <param name="iExpCount">The i exp count.</param>
        /// <param name="xExpAlive">if set to <c>true</c> [x exp alive].</param>
        /// <param name="expPos">The exp position.</param>
        /// <param name="sExpSnakeTest">The s exp snake test.</param>
        [DataTestMethod()]
        [DataRow("1-E",new int[] { 2, 1 },new Direction[] {Direction.East }, 2,true,new int[] {3,1},new string[] { "DataChange: Snake_Base.Model.Playfield2D`1[Snake_Base.Model.SnakeGameObject]\to:\tn:Snake_Base.Model.SnakeTail\tp:Items\r\nDataChange: Snake_Base.Model.Playfield2D`1[Snake_Base.Model.SnakeGameObject]\to:\tn:Snake_Base.Model.SnakeHead\tp:Items\r\nDataChange: Snake_Base.Model.SnakeHead\to:{X=2,Y=1}\tn:{X=3,Y=1}\tp:Place\r\nDataChange: Snake_Base.Model.Playfield2D`1[Snake_Base.Model.SnakeGameObject]\to:\tn:Snake_Base.Model.SnakeBodyPart\tp:Items\r\n" })]
        [DataRow("2-E", new int[] { 3, 1 }, new Direction[] { Direction.East }, 1, false, new int[] { 3, 1 }, new string[] { "DataChange: Snake_Base.Model.Playfield2D`1[Snake_Base.Model.SnakeGameObject]\to:\tn:Snake_Base.Model.SnakeTail\tp:Items\r\nDataChange: Snake_Base.Model.Playfield2D`1[Snake_Base.Model.SnakeGameObject]\to:\tn:Snake_Base.Model.SnakeHead\tp:Items\r\n" })]
        [DataRow("3-2E", new int[] { 2, 1 }, new Direction[] { Direction.East, Direction.North }, 2, true, new int[] { 3, 0 }, new string[] { "DataChange: Snake_Base.Model.Playfield2D`1[Snake_Base.Model.SnakeGameObject]\to:\tn:Snake_Base.Model.SnakeTail\tp:Items\r\nDataChange: Snake_Base.Model.Playfield2D`1[Snake_Base.Model.SnakeGameObject]\to:\tn:Snake_Base.Model.SnakeHead\tp:Items\r\nDataChange: Snake_Base.Model.SnakeHead\to:{X=2,Y=1}\tn:{X=3,Y=1}\tp:Place\r\nDataChange: Snake_Base.Model.Playfield2D`1[Snake_Base.Model.SnakeGameObject]\to:\tn:Snake_Base.Model.SnakeBodyPart\tp:Items\r\nDataChange: Snake_Base.Model.SnakeHead\to:{X=3,Y=1}\tn:{X=3,Y=0}\tp:Place\r\nDataChange: Snake_Base.Model.SnakeBodyPart\to:{X=2,Y=1}\tn:{X=3,Y=1}\tp:Place\r\n" })]
        [DataRow("4-2E+A", new int[] { 2, 1 ,3,1,3,0}, new Direction[] { Direction.East, Direction.North,Direction.West }, 4, true, new int[] { 2, 0 }, new string[] { "DataChange: Snake_Base.Model.Playfield2D`1[Snake_Base.Model.SnakeGameObject]\to:\tn:Snake_Base.Model.SnakeTail\tp:Items\r\nDataChange: Snake_Base.Model.Playfield2D`1[Snake_Base.Model.SnakeGameObject]\to:\tn:Snake_Base.Model.SnakeHead\tp:Items\r\nDataChange: Snake_Base.Model.Playfield2D`1[Snake_Base.Model.SnakeGameObject]\to:\tn:Snake_Base.Model.Apple\tp:Items\r\nDataChange: Snake_Base.Model.Playfield2D`1[Snake_Base.Model.SnakeGameObject]\to:\tn:Snake_Base.Model.Apple\tp:Items\r\nDataChange: Snake_Base.Model.SnakeHead\to:{X=2,Y=1}\tn:{X=3,Y=1}\tp:Place\r\nDataChange: Snake_Base.Model.Playfield2D`1[Snake_Base.Model.SnakeGameObject]\to:\tn:Snake_Base.Model.SnakeBodyPart\tp:Items\r\nDataChange: Snake_Base.Model.SnakeHead\to:{X=3,Y=1}\tn:{X=3,Y=0}\tp:Place\r\nDataChange: Snake_Base.Model.SnakeBodyPart\to:{X=2,Y=1}\tn:{X=3,Y=1}\tp:Place\r\nDataChange: Snake_Base.Model.Playfield2D`1[Snake_Base.Model.SnakeGameObject]\to:\tn:Snake_Base.Model.SnakeBodyPart\tp:Items\r\nDataChange: Snake_Base.Model.SnakeHead\to:{X=3,Y=0}\tn:{X=2,Y=0}\tp:Place\r\nDataChange: Snake_Base.Model.SnakeBodyPart\to:{X=3,Y=1}\tn:{X=3,Y=0}\tp:Place\r\nDataChange: Snake_Base.Model.SnakeBodyPart\to:{X=2,Y=1}\tn:{X=3,Y=1}\tp:Place\r\nDataChange: Snake_Base.Model.Playfield2D`1[Snake_Base.Model.SnakeGameObject]\to:\tn:Snake_Base.Model.SnakeBodyPart\tp:Items\r\n" })]
        [DataRow("5-2E+A", new int[] { 2, 1, 3, 1, 3, 0 }, new Direction[] { Direction.East, Direction.North, Direction.West,Direction.South }, 4, false, new int[] { 2, 0 }, new string[] { "DataChange: Snake_Base.Model.Playfield2D`1[Snake_Base.Model.SnakeGameObject]\to:\tn:Snake_Base.Model.SnakeTail\tp:Items\r\nDataChange: Snake_Base.Model.Playfield2D`1[Snake_Base.Model.SnakeGameObject]\to:\tn:Snake_Base.Model.SnakeHead\tp:Items\r\nDataChange: Snake_Base.Model.Playfield2D`1[Snake_Base.Model.SnakeGameObject]\to:\tn:Snake_Base.Model.Apple\tp:Items\r\nDataChange: Snake_Base.Model.Playfield2D`1[Snake_Base.Model.SnakeGameObject]\to:\tn:Snake_Base.Model.Apple\tp:Items\r\nDataChange: Snake_Base.Model.SnakeHead\to:{X=2,Y=1}\tn:{X=3,Y=1}\tp:Place\r\nDataChange: Snake_Base.Model.Playfield2D`1[Snake_Base.Model.SnakeGameObject]\to:\tn:Snake_Base.Model.SnakeBodyPart\tp:Items\r\nDataChange: Snake_Base.Model.SnakeHead\to:{X=3,Y=1}\tn:{X=3,Y=0}\tp:Place\r\nDataChange: Snake_Base.Model.SnakeBodyPart\to:{X=2,Y=1}\tn:{X=3,Y=1}\tp:Place\r\nDataChange: Snake_Base.Model.Playfield2D`1[Snake_Base.Model.SnakeGameObject]\to:\tn:Snake_Base.Model.SnakeBodyPart\tp:Items\r\nDataChange: Snake_Base.Model.SnakeHead\to:{X=3,Y=0}\tn:{X=2,Y=0}\tp:Place\r\nDataChange: Snake_Base.Model.SnakeBodyPart\to:{X=3,Y=1}\tn:{X=3,Y=0}\tp:Place\r\nDataChange: Snake_Base.Model.SnakeBodyPart\to:{X=2,Y=1}\tn:{X=3,Y=1}\tp:Place\r\nDataChange: Snake_Base.Model.Playfield2D`1[Snake_Base.Model.SnakeGameObject]\to:\tn:Snake_Base.Model.SnakeBodyPart\tp:Items\r\n" })]
        public void SnakeMoveTest(string TestName, int[] start,Direction[] dir,int iExpCount,bool xExpAlive ,int[] expPos, string[] sExpSnakeTest)
        {
            var testSnake = new Snake(new Point(start[0], start[1]));
            Apple? testApple;
            if (start.Length > 3)
                testApple = new Apple(new Point(start[2], start[3]));
            if (start.Length > 5)
                _ = new Apple(new Point(start[4], start[5]));
            Assert.IsTrue(testSnake.alive); 
            for (var i=0;i<dir.Length && testSnake.alive; i++)
                testSnake.SnakeMove(dir[i]);

            Assert.AreEqual(new Point(expPos[0], expPos[1]),testSnake.HeadPos);
            //            Assert.AreEqual
            Assert.AreEqual(iExpCount, playfield!.Items.Count());
            Assert.AreEqual(xExpAlive, testSnake.alive);

            Assert.AreEqual(sExpSnakeTest[0], ResultData);
        }

        #region private Methods
        private void LogDataChanged(object? sender, (string prop, object? oldVal, object? newVal) e)
        {
            ResultData += $"DataChange: {sender}\to:{e.oldVal}\tn:{e.newVal}\tp:{e.prop}\r\n";
        }
        #endregion
        #endregion
    }
}