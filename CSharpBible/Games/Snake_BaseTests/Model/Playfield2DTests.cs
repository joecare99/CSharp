﻿// ***********************************************************************
// Assembly         : Snake_BaseTests
// Author           : Mir
// Created          : 08-24-2022
//
// Last Modified By : Mir
// Last Modified On : 09-09-2022
// ***********************************************************************
// <copyright file="Playfield2DTests.cs" company="JC-Soft">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using BaseLib.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Drawing;
using System.Reflection;

/// <summary>
/// The Tests namespace.
/// </summary>
/// <autogeneratedoc />
namespace Snake_Base.Model.Tests
{
    /// <summary>
    /// Defines test class Playfield2DTests.
    /// </summary>
    [TestClass()]
    public class Playfield2DTests
    {
        /// <summary>
        /// The result data
        /// </summary>
        /// <autogeneratedoc />
        private string ResultData = "";
        private Apple oApple;
        private SnakeHead oSHead;
        private SnakeBodyPart oSBody1;
        private SnakeBodyPart oSBody2;
        private SnakeTail oSTail;
        private TestItem oTest1;
        private TestItem oTest2;

        /// <summary>
        /// Gets the test playfield.
        /// </summary>
        /// <value>The test playfield.</value>
        internal Playfield2D<SnakeGameObject>? testPlayfield { get; private set; }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        [TestInitialize]
        public void Init()
        {
            testPlayfield = new Playfield2D<SnakeGameObject>(new Size(4, 3));
            testPlayfield.OnDataChanged += TPF_OnDataChanged; 
            TestItem.logOperation += LogOperation;
            ResultData = "";
        }

        /// <summary>Data changed on the Test-Playfield.</summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The eventdata e.</param>
        /// <autogeneratedoc />
        private void TPF_OnDataChanged(object? sender, (string prop, object? oldVal, object? newVal) e)
        {
            ResultData += $"OnDataChanged: {sender}\to:{e.oldVal}\tn:{e.newVal}\tp:{e.prop}\r\n";
        }

        /// <summary>
        /// Logs the operation.
        /// </summary>
        /// <param name="sOperation">The s operation.</param>
        /// <param name="sender">The sender.</param>
        /// <param name="oldVal">The old value.</param>
        /// <param name="newVal">The new value.</param>
        /// <param name="sProp">The s property.</param>
        /// <autogeneratedoc />
        private void LogOperation(string sOperation, TestItem sender, object? oldVal, object? newVal, string sProp)
        {
            ResultData += $"{sOperation}: {sender}\to:{oldVal}\tn:{newVal}\tp:{sProp}\r\n";
        }

        private void CreateTestData()
        {
            oApple=new Apple(new Point(2, 2), testPlayfield);
            oSHead = new SnakeHead(new Point(2, 1),null ,testPlayfield);
            oSBody1 = new SnakeBodyPart(new Point(1, 1), null, testPlayfield);
            oSBody2 = new SnakeBodyPart(new Point(1, 2), null, testPlayfield);
            oSTail = new SnakeTail(new Point(0, 2), null, testPlayfield);
            (oTest1 =new TestItem() { _place = new Point(3, 2) }).SetParent(testPlayfield);
            (oTest2 =new TestItem() { _place = new Point(3, 1) }).SetParent(testPlayfield);            
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
        [DataTestMethod()]
        [TestProperty("Author", "J.C.")]
        [TestCategory("AddData")]
        [DataRow("Nothing", new int[] { 0, 0 }, "Snake_Base.Model.Tests.TestItem", new string[] {
            "New Parent: TestItem(,{X=0,Y=0})\to:\tn:Snake_Base.Model.Playfield2D`1[Snake_Base.Model.SnakeGameObject]\tp:AddItemTest1\r\n" })]
        [DataRow("Apple", new int[] { 0, 1 }, "Snake_Base.Model.Apple", new string[] {
            "OnDataChanged: Snake_Base.Model.Playfield2D`1[Snake_Base.Model.SnakeGameObject]\to:\tn:Snake_Base.Model.Apple\tp:Items\r\n",
            ""})]
        [DataRow("Snake",  new int[] { 1, 0 }, "Snake_Base.Model.SnakeBodyPart", new string[] { 
            "OnDataChanged: Snake_Base.Model.Playfield2D`1[Snake_Base.Model.SnakeGameObject]\to:\tn:Snake_Base.Model.SnakeBodyPart\tp:Items\r\n",
            "" })]
        public void AddItemTest1(string sName,int[] p, string tind, string[] sExp)
        {
            Type? t = Assembly.GetAssembly(typeof(Playfield2D))?.GetType(tind);  
            t ??= Assembly.GetAssembly(typeof(TestItem))?.GetType(tind);
            var _testItem = Activator.CreateInstance(t);
            (_testItem as IPlacedObject)?.SetPlace(new Point(p[0], p[1]));
            if (_testItem is IParentedObject i) i.SetParent(testPlayfield);
            else
                (_testItem as SnakeGameObject)?.SetParent(testPlayfield);
            Assert.AreEqual(sExp[0], ResultData);
        }

        /// <summary>
        /// Defines the test method AddItemTest.
        /// </summary>
        [DataTestMethod()]
        [TestProperty("Author", "J.C.")]
        [TestCategory("AddData")]
        [DataRow("Nothing", new int[] { 0, 0 }, "Snake_Base.Model.Tests.TestItem", new string[] { 
            "New Parent: TestItem(,{X=0,Y=0})\to:\tn:Snake_Base.Model.Playfield2D`1[Snake_Base.Model.SnakeGameObject]\tp:AddItemTest2\r\n" })]
        [DataRow("Apple", new int[] { 0, 1 }, "Snake_Base.Model.Apple", new string[] { 
            "OnDataChanged: Snake_Base.Model.Playfield2D`1[Snake_Base.Model.SnakeGameObject]\to:\tn:Snake_Base.Model.Apple\tp:Items\r\nOnDataChanged: Snake_Base.Model.Apple\to:{X=0,Y=0}\tn:{X=0,Y=1}\tp:Place\r\n" })]
        [DataRow("Snake", new int[] { 1, 0 }, "Snake_Base.Model.SnakeBodyPart", new string[] {
            "OnDataChanged: Snake_Base.Model.Playfield2D`1[Snake_Base.Model.SnakeGameObject]\to:\tn:Snake_Base.Model.SnakeBodyPart\tp:Items\r\nOnDataChanged: Snake_Base.Model.SnakeBodyPart\to:{X=0,Y=0}\tn:{X=1,Y=0}\tp:Place\r\n" })]
        public void AddItemTest2(string sName, int[] p, string tind, string[] sExp)
        {
            Type? t = Assembly.GetAssembly(typeof(Playfield2D))?.GetType(tind);
            t ??= Assembly.GetAssembly(typeof(TestItem))?.GetType(tind);
            var _testItem = Activator.CreateInstance(t);
            if (_testItem is IParentedObject i) i.SetParent(testPlayfield);
            else
                (_testItem as SnakeGameObject)?.SetParent(testPlayfield); 
            (_testItem as IPlacedObject)?.SetPlace(new Point(p[0], p[1]));
            Assert.AreEqual(sExp[0], ResultData);
        }

        /// <summary>
        /// Defines the test method RemoveItemTest.
        /// </summary>
        [DataTestMethod()]
        [TestProperty("Author", "J.C.")]
        [TestCategory("RemoveData")]
        public void RemoveItemTest()
        {
            CreateTestData();
            Assert.AreEqual(true,testPlayfield!.RemoveItem(oApple));
            Assert.AreEqual(null, testPlayfield[new Point(2,2)]);
            Assert.AreEqual(false, testPlayfield.RemoveItem(oApple));
            
        }

        /// <summary>
        /// Defines the test method GetItemsTest.
        /// </summary>
        [DataTestMethod()]
        [TestProperty("Author", "J.C.")]
        [TestCategory("GetItem")]
        [DataRow("00-Nothing", new int[] { 0, 0 }, "" )]
        [DataRow("01- ", new int[] { 0, 1 }, "")]
        [DataRow("02-Tail", new int[] { 0, 2 }, "Snake_Base.Model.SnakeTail")]
        [DataRow("10- ", new int[] { 1, 0 }, "")]
        [DataRow("11-Body", new int[] { 1, 1 }, "Snake_Base.Model.SnakeBodyPart")]
        [DataRow("12-Body", new int[] { 1, 2 }, "Snake_Base.Model.SnakeBodyPart")]
        [DataRow("20-Apple4", new int[] { 2, 0 }, "")]
        [DataRow("21-Head", new int[] { 2, 1 }, "Snake_Base.Model.SnakeHead")]
        [DataRow("22-Apple", new int[] { 2, 2 }, "Snake_Base.Model.Apple")]
        [DataRow("30- ", new int[] { 3, 0 }, "")]
        [DataRow("31- ", new int[] { 3, 1 }, "")]//??
        [DataRow("32- ", new int[] { 3, 2 }, "")]//??
        public void GetItemsTest(string name, int[] p, string sExpClass)
        {
            CreateTestData();
            Assert.AreEqual(sExpClass, testPlayfield[new Point(p[0], p[1])]?.ToString()??"");
        }

        /// <summary>
        /// Logs the data changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <autogeneratedoc />
        private void LogDataChanged(object? sender, (string prop, object? oldVal, object? newVal) e)
        {
            ResultData += $"DataChange: {sender}\to:{e.oldVal}\tn:{e.newVal}\tc:{e.prop}\r\n";
        }

    }
}