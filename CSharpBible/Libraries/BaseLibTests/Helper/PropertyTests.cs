using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace BaseLib.Helper.Tests
{
    [TestClass()]
    public class PropertyTests
    {
        public enum eValidReact
        {
            OK = 0,
            NIO,
            GeneralException,
            ArgumetException,
        }

        private string _testString="";
        private int _testInt;
        private float _testFloat;
        private double _testDouble;
        private string DebugResult="";
        private TypeCode _testEnum;
        private eValidReact valReact;

        public string TestString { get => _testString; set => Property.SetProperty(ref _testString, value); }
        public string TestString1 { get => _testString; set => value.SetProperty(ref _testString); }
        public string TestString2 { get => _testString; set => Property.SetProperty(ref _testString, value, ValidateString); }
        public string TestString3 { get => _testString; set => value.SetProperty(ref _testString, ValidateString); }
        public string TestString4 { get => _testString; set => Property.SetProperty(ref _testString, value, StringAct); }
        public string TestString5 { get => _testString; set => value.SetProperty(ref _testString, StringAct); }
        
        public string TestString6
        {
             set
                => Property.SetProperty(ref _testString, value, ValidateString, StringAct);
        }

        public string TestString7 {  
            set => value.SetProperty(ref _testString, ValidateString, StringAct); }

        public int TestInt { get => _testInt; set => Property.SetProperty(ref _testInt, value); }
        public float TestFloat { get => _testFloat; set => Property.SetProperty(ref _testFloat, value); }
        public double TestDouble { get => _testDouble; set => Property.SetProperty(ref _testDouble, value); }

        public TypeCode TestEnum { get => _testEnum; set => Property.SetProperty(ref _testEnum, value); }

        private void StringAct(string arg1, string arg2,string arg3)
        {
            DebugResult += $"StrAct: {arg1}; {arg2}; {arg3}{Environment.NewLine}";
            switch (valReact)
            {
                case eValidReact.OK:
                    return;
                case eValidReact.GeneralException:
                    throw new Exception("A general exception occured");
                case eValidReact.ArgumetException:
                    throw new ArgumentException($"Argument ({arg1}) not valid!");
                default:
                    return;
  
            }
        }

        private bool ValidateString(string arg1)
        {
            DebugResult += $"Validate: {arg1}, React:{valReact}{Environment.NewLine}";
            switch (valReact)
            {
                case eValidReact.OK:
                    return true;
                case eValidReact.GeneralException:
                    throw new Exception("A general exception occured");
                case eValidReact.ArgumetException:
                    throw new ArgumentException($"Argument ({arg1}) not valid!");
                default:
                    return false;
           
            }            
        }

        [TestMethod()]
        [TestProperty("Author", "J.C.")]
        [TestCategory("SetData")]
        [DataRow("00 Empty", 0, "", eValidReact.OK, "", "")]
        [DataRow("01-Test", 0, "Test", eValidReact.OK, "Test", "")]
        [DataRow("20-1 Empty", 2, "", eValidReact.OK, "", "")]
        [DataRow("20-2 Test", 2, "", eValidReact.NIO, "", "")]
        [DataRow("20-2 Test", 2, "", eValidReact.GeneralException, "", "")]
        [DataRow("21-1 Empty", 2, "Test", eValidReact.OK, "Test", "Validate: Test, React:OK\r\n")]
        [DataRow("21-2 Test", 2, "Test", eValidReact.NIO, "", "Validate: Test, React:NIO\r\n")]
        [DataRow("21-3 GEx", 2, "Test", eValidReact.GeneralException, "", "Validate: Test, React:GeneralException\r\n")]
        [DataRow("21-4 AEx", 2, "Test", eValidReact.ArgumetException, "", "Validate: Test, React:ArgumetException\r\n")]
        [DataRow("30-1 Empty", 3, "", eValidReact.OK, "", "")]
        [DataRow("30-2 Test", 3, "", eValidReact.NIO, "", "")]
        [DataRow("31-1 Test", 3, "Test", eValidReact.OK, "Test", "StrAct: TestString4; ; Test\r\n")]
        [DataRow("31-2 Test2", 3, "Test2", eValidReact.NIO, "Test2", "StrAct: TestString4; ; Test2\r\n")]
        [DataRow("31-3 GEx", 3, "Test", eValidReact.GeneralException, "Test", "StrAct: TestString4; ; Test\r\n")]
        [DataRow("31-4 AEx", 3, "Test", eValidReact.ArgumetException, "Test", "StrAct: TestString4; ; Test\r\n")]
        [DataRow("40-1 Empty", 4, "", eValidReact.OK, "", "")]
        [DataRow("40-2 Test ", 4, "", eValidReact.NIO, "", "")]
        [DataRow("41-1 Test ", 4, "Test", eValidReact.OK, "Test", "Validate: Test, React:OK\r\nStrAct: TestString6; ; Test\r\n")]
        [DataRow("41-2 Test2", 4, "Test", eValidReact.NIO, "", "Validate: Test, React:NIO\r\n")]
        [DataRow("41-3 GEx", 4, "Test", eValidReact.GeneralException, "", "Validate: Test, React:GeneralException\r\n")]
        [DataRow("41-4 AEx", 4, "Test", eValidReact.ArgumetException, "", "Validate: Test, React:ArgumetException\r\n")]
        [DataRow("42-2 Test2", 4, "Test2", eValidReact.OK, "Test2", "Validate: Test2, React:OK\r\nStrAct: TestString6; ; Test2\r\n")]
        public void SetPropertyTest(string name, int iTs, string sVal, eValidReact eReact, string sExp, string sDebExp)
        {
            valReact = eReact;
            bool xCh = sVal != _testString;
            bool eRIsEx = eReact == eValidReact.GeneralException || eReact == eValidReact.ArgumetException;
            switch (iTs)
            {
                //case 1: TestString1 = sVal; break;
                case 2 when xCh && eReact == eValidReact.GeneralException: Assert.ThrowsException<Exception>(() => TestString2 = sVal, $"{name}.T2"); break;
                case 2 when xCh && eReact == eValidReact.ArgumetException: Assert.ThrowsException<ArgumentException>(() => TestString2 = sVal, $"{name}.T2"); break;
                case 2: TestString2 = sVal; break;
                case 3: TestString4 = sVal; break;
                case 4 when xCh && eReact == eValidReact.GeneralException: Assert.ThrowsException<Exception>(() => TestString6 = sVal, $"{name}.T4"); break;
                case 4 when xCh && eReact == eValidReact.ArgumetException: Assert.ThrowsException<ArgumentException>(() => TestString6 = sVal, $"{name}.T4"); break;
                case 4: TestString6 = sVal; break;
                default: TestString = sVal; break;
            }
            Assert.AreEqual(sExp, _testString, $"{name}.Result");
            Assert.AreEqual(sDebExp, DebugResult, $"{name}.DebRes");
        }

        [TestMethod()]
        [TestProperty("Author", "J.C.")]
        [TestCategory("SetData")]
        [DataRow("00 Empty", 0, "", eValidReact.OK, "", "")]
        [DataRow("01-Test", 0, "Test", eValidReact.OK, "Test", "")]
        [DataRow("20-1 Empty", 2, "", eValidReact.OK, "", "")]
        [DataRow("20-2 Test", 2, "", eValidReact.NIO, "", "")]
        [DataRow("20-2 Test", 2, "", eValidReact.GeneralException, "", "")]
        [DataRow("21-1 Empty", 2, "Test", eValidReact.OK, "Test", "Validate: Test, React:OK\r\n")]
        [DataRow("21-2 Test", 2, "Test", eValidReact.NIO, "", "Validate: Test, React:NIO\r\n")]
        [DataRow("21-3 GEx", 2, "Test", eValidReact.GeneralException, "", "Validate: Test, React:GeneralException\r\n")]
        [DataRow("21-4 AEx", 2, "Test", eValidReact.ArgumetException, "", "Validate: Test, React:ArgumetException\r\n")]
        [DataRow("30-1 Empty", 3, "", eValidReact.OK, "", "")]
        [DataRow("30-2 Test", 3, "", eValidReact.NIO, "", "")]
        [DataRow("31-1 Test", 3, "Test", eValidReact.OK, "Test", "StrAct: TestString5; ; Test\r\n")]
        [DataRow("31-2 Test2", 3, "Test2", eValidReact.NIO, "Test2", "StrAct: TestString5; ; Test2\r\n")]
        [DataRow("31-3 GEx", 3, "Test", eValidReact.GeneralException, "Test", "StrAct: TestString5; ; Test\r\n")]
        [DataRow("31-4 AEx", 3, "Test", eValidReact.ArgumetException, "Test", "StrAct: TestString5; ; Test\r\n")]
        [DataRow("40-1 Empty", 4, "", eValidReact.OK, "", "")]
        [DataRow("40-2 Test ", 4, "", eValidReact.NIO, "", "")]
        [DataRow("41-1 Test ", 4, "Test", eValidReact.OK, "Test", "Validate: Test, React:OK\r\nStrAct: TestString7; ; Test\r\n")]
        [DataRow("41-2 Test2", 4, "Test", eValidReact.NIO, "", "Validate: Test, React:NIO\r\n")]
        [DataRow("41-3 GEx", 4, "Test", eValidReact.GeneralException, "", "Validate: Test, React:GeneralException\r\n")]
        [DataRow("41-4 AEx", 4, "Test", eValidReact.ArgumetException, "", "Validate: Test, React:ArgumetException\r\n")]
        [DataRow("42-2 Test2", 4, "Test2", eValidReact.OK, "Test2", "Validate: Test2, React:OK\r\nStrAct: TestString7; ; Test2\r\n")]
        public void SetPropertyTest1(string name, int iTs, string sVal, eValidReact eReact, string sExp, string sDebExp)
        {
            valReact = eReact;
            bool xCh = sVal != _testString;
            bool eRIsEx = eReact == eValidReact.GeneralException || eReact == eValidReact.ArgumetException;
            switch (iTs)
            {
                //case 1: TestString1 = sVal; break;
                case 2 when xCh && eReact == eValidReact.GeneralException: Assert.ThrowsException<Exception>(() => TestString3 = sVal, $"{name}.T3"); break;
                case 2 when xCh && eReact == eValidReact.ArgumetException: Assert.ThrowsException<ArgumentException>(() => TestString3 = sVal, $"{name}.T3"); break;
                case 2: TestString3 = sVal; break;
                case 3: TestString5 = sVal; break;
                case 4 when xCh && eReact == eValidReact.GeneralException: Assert.ThrowsException<Exception>(() => TestString7 = sVal, $"{name}.T7"); break;
                case 4 when xCh && eReact == eValidReact.ArgumetException: Assert.ThrowsException<ArgumentException>(() => TestString7 = sVal, $"{name}.T7"); break;
                case 4: TestString7 = sVal; break;
                default: TestString1 = sVal; break;
            }
            Assert.AreEqual(sExp, _testString, $"{name}.Result");
            Assert.AreEqual(sDebExp, DebugResult, $"{name}.DebRes");
        }
    }
}