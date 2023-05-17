using Microsoft.VisualStudio.TestTools.UnitTesting;
using BaseLib.Helper.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace BaseLib.Helper.MVVM.Tests
{
    [TestClass()]
    public class PropertyHelperTests : IRaisePropChangedEvents
    {
        private string DebugLog = "";
        private string _testProperty;
        private int _testProperty2;
        private Exception? exIntEx=null;
        private bool? xResult=null;

        public event PropertyChangedEventHandler PropertyChanged;

        public string TestProperty { get => _testProperty; set => xResult= value.SetProperty(ref _testProperty , this); }
        public int TestProperty2 { get => _testProperty2; set => xResult = value.SetProperty(ref _testProperty2,IntAction, this); }

        private void IntAction(string arg1, int arg2, int arg3)
        {
            DoLog($"IntAction({arg1},{arg2},{arg3})");
            if (exIntEx != null) throw exIntEx;
        }

        [TestInitialize()]
        public void Init()
        {
            DebugLog = "";
            exIntEx = null;
            xResult = null;
            _testProperty = "test1";
            _testProperty2 = 123;
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

        [TestMethod()]
        public void CondSetPropertyTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void CondSetPropertyTest1()
        {
            Assert.Fail();
        }

        public void RaisePropertyChanged(string propertyName)
        {
            DoLog($"PropChg({propertyName})={this.GetProp(propertyName)}");
        }

        private void DoLog(string v)
        {
            DebugLog += $"{v}{Environment.NewLine}";
        }
    }
}