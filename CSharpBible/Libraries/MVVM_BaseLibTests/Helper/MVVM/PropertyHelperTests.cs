using Microsoft.VisualStudio.TestTools.UnitTesting;
using BaseLib.Helper.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Drawing;
using MVVM.ViewModel;

namespace BaseLib.Helper.MVVM.Tests
{
    [TestClass()]
    public class PropertyHelperTests :BaseTestViewModel, IRaisePropChangedEvents
    {
        private string _testProperty="";
        private int _testProperty2;
        private Exception? exIntEx = null;
        private Exception? exCondEx = null;
        private bool? xResult=null;
        private bool xCondResult=false;

        public event PropertyChangedEventHandler? PropertyChanged;

        public string TestProperty { get => _testProperty; set => xResult= value.SetProperty(ref _testProperty , this); }
        public int TestProperty2 { get => _testProperty2; set => xResult = value.SetProperty(ref _testProperty2,IntAction, this); }
        public string TestProperty3 { get => _testProperty; set => xResult = value.CondSetProperty(ref _testProperty,StingCond,null, this); }
        public int TestProperty4 { get => _testProperty2; set => xResult = value.CondSetProperty(ref _testProperty2,IntCond, IntAction, this); }
        public string TestProperty5 { get => _testProperty; set => xResult = value.CondSetProperty(_testProperty, s=> _testProperty = s, StingCond, null, this); }
        public int TestProperty6 { get => _testProperty2; set => xResult = value.CondSetProperty(_testProperty2, i => _testProperty2 = i, IntCond, IntAction, this); }


        private bool StingCond(string arg1, string arg2, string arg3)
        {
            DoLog($"StringCond({arg1},{arg2},{arg3})={xCondResult}");
            if (exCondEx != null) throw exCondEx;
            return xCondResult;
        }
        private bool IntCond(string arg1, int arg2, int arg3)
        {
            DoLog($"IntCond({arg1},{arg2},{arg3})={xCondResult}");
            if (exCondEx != null) throw exCondEx;
            return xCondResult;
        }

        private void IntAction(string arg1, int arg2, int arg3)
        {
            DoLog($"IntAction({arg1},{arg2},{arg3})");
            if (exIntEx != null) throw exIntEx;
        }

        [TestInitialize()]
        public void Init()
        {
            exIntEx = null;
            xResult = null;
            _testProperty = "test1";
            _testProperty2 = 123;
            PropertyChanged += OnVMPropertyChanged;
            ClearLog();
        }

        [TestMethod()]
        public void SetUpTest()
        {
            Assert.AreEqual("test1",_testProperty);
            Assert.AreEqual(123, _testProperty2);
            Assert.AreEqual(null, exIntEx);
            Assert.AreEqual(null, xResult);
            Assert.AreEqual("", DebugLog);
        }

        [DataTestMethod()]
        [DataRow("",true, new[] {"", "PropChg(TestProperty)=\r\n" })]
        [DataRow("test1",false, new[] { "test1", "" })]
        [DataRow("test2",true, new[] { "test2", "PropChg(TestProperty)=test2\r\n" })]
        public void SetPropertyTest(string sVal, bool xExp, string[] asExp)
        {
            TestProperty = sVal;
            Assert.AreEqual(xExp, xResult);
            Assert.AreEqual(asExp[0], TestProperty);
            Assert.AreEqual(asExp[1],DebugLog);
        }

        [DataTestMethod()]
        [DataRow(0, false,true, new object[] { 0, "IntAction(TestProperty2,123,0)\r\nPropChg(TestProperty2)=0\r\n" })]
        [DataRow(123,false,false, new object[] { 123, "" })]
        [DataRow(124,true,true, new object[] { 124, "IntAction(TestProperty2,123,124)\r\nPropChg(TestProperty2)=124\r\n" })]
        public void SetPropertyTest1(int iVal,bool xExc, bool xExp, object[] asExp)
        {
            if (xExc) exIntEx = new ArgumentException("Test");
            TestProperty2 = iVal;
            Assert.AreEqual(xExp, xResult);
            Assert.AreEqual(asExp[0], TestProperty2);
            Assert.AreEqual(asExp[1], DebugLog);
        }

        [DataTestMethod()]
        [DataRow("", true, true, new[] { "", "StringCond(TestProperty3,test1,)=True\r\nPropChg(TestProperty3)=\r\n" })]
        [DataRow("", false, false, new[] { "test1", "StringCond(TestProperty3,test1,)=False\r\n" })]
        [DataRow("test1", true, false, new[] { "test1", "" })]
        [DataRow("test1", false, false, new[] { "test1", "" })]
        [DataRow("test2", true, true, new[] { "test2", "StringCond(TestProperty3,test1,test2)=True\r\nPropChg(TestProperty3)=test2\r\n" })]
        [DataRow("test2", false, false, new[] { "test1", "StringCond(TestProperty3,test1,test2)=False\r\n" })]
        public void CondSetPropertyTest(string sVal, bool xCond, bool xExp, string[] asExp)
        {
            xCondResult = xCond;
            TestProperty3 = sVal;
            Assert.AreEqual(xExp, xResult);
            Assert.AreEqual(asExp[0], TestProperty);
            Assert.AreEqual(asExp[1], DebugLog);
        }

        [DataTestMethod()]
        [DataRow(0, true, false, true, new object[] { 0, "IntCond(TestProperty4,123,0)=True\r\nIntAction(TestProperty4,123,0)\r\nPropChg(TestProperty4)=0\r\n" })]
        [DataRow(0, false, false, false, new object[] { 123, "IntCond(TestProperty4,123,0)=False\r\n" })]
        [DataRow(123, true, false, false, new object[] { 123, "" })]
        [DataRow(123, false, false, false, new object[] { 123, "" })]
        [DataRow(124, true, true, true, new object[] { 124, "IntCond(TestProperty4,123,124)=True\r\nIntAction(TestProperty4,123,124)\r\nPropChg(TestProperty4)=124\r\n" })]
        [DataRow(124, false, true, false, new object[] { 123, "IntCond(TestProperty4,123,124)=False\r\n" })]
        public void CondSetPropertyTest1(int iVal, bool xCond, bool xExc, bool xExp, object[] asExp)
        {
            xCondResult = xCond;
            if (xExc) exIntEx = new ArgumentException("Test");
            TestProperty4 = iVal;
            Assert.AreEqual(xExp, xResult);
            Assert.AreEqual(asExp[0], TestProperty2);
            Assert.AreEqual(asExp[1], DebugLog);
        }

        [DataTestMethod()]
        [DataRow("", true, true, new[] { "", "StringCond(TestProperty5,test1,)=True\r\nPropChg(TestProperty5)=\r\n" })]
        [DataRow("", false, false, new[] { "test1", "StringCond(TestProperty5,test1,)=False\r\n" })]
        [DataRow("test1", true, false, new[] { "test1", "" })]
        [DataRow("test1", false, false, new[] { "test1", "" })]
        [DataRow("test2", true, true, new[] { "test2", "StringCond(TestProperty5,test1,test2)=True\r\nPropChg(TestProperty5)=test2\r\n" })]
        [DataRow("test2", false, false, new[] { "test1", "StringCond(TestProperty5,test1,test2)=False\r\n" })]
        public void CondSetPropertyTest2(string sVal, bool xCond, bool xExp, string[] asExp)
        {
            xCondResult = xCond;
            TestProperty5 = sVal;
            Assert.AreEqual(xExp, xResult);
            Assert.AreEqual(asExp[0], TestProperty);
            Assert.AreEqual(asExp[1], DebugLog);
        }

        [DataTestMethod()]
        [DataRow(0, true, false, true, new object[] { 0, "IntCond(TestProperty6,123,0)=True\r\nIntAction(TestProperty6,123,0)\r\nPropChg(TestProperty6)=0\r\n" })]
        [DataRow(0, false, false, false, new object[] { 123, "IntCond(TestProperty6,123,0)=False\r\n" })]
        [DataRow(123, true, false, false, new object[] { 123, "" })]
        [DataRow(123, false, false, false, new object[] { 123, "" })]
        [DataRow(124, true, true, true, new object[] { 124, "IntCond(TestProperty6,123,124)=True\r\nIntAction(TestProperty6,123,124)\r\nPropChg(TestProperty6)=124\r\n" })]
        [DataRow(124, false, true, false, new object[] { 123, "IntCond(TestProperty6,123,124)=False\r\n" })]
        public void CondSetPropertyTest3(int iVal, bool xCond, bool xExc, bool xExp, object[] asExp)
        {
            xCondResult = xCond;
            if (xExc) exIntEx = new ArgumentException("Test");
            TestProperty6 = iVal;
            Assert.AreEqual(xExp, xResult);
            Assert.AreEqual(asExp[0], TestProperty2);
            Assert.AreEqual(asExp[1], DebugLog);
        }

        public void RaisePropertyChanged(string propertyName)
        {
            DoLog($"PropChg({propertyName})={this.GetProp(propertyName)}");
        }
    }
}