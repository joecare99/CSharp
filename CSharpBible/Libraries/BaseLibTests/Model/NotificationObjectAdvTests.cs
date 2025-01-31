using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using BaseLib.Models;
using BaseLib.Interfaces;

namespace BaseLib.ViewModel.Tests
{
    /// <summary>
    /// Defines test class PropertyTests.
    /// </summary>
    [TestClass()]
    public class NotificationObjectAdvTests : NotificationObjectAdv
    {
        public enum eValidReact
        {
            OK=0,
            NIO,
            GeneralException,
            ArgumetException,
        }

        private string _testString;
        private int _testInt;
        private float _testFloat;
        private double _testDouble;

        private eValidReact valReact=eValidReact.OK;

        private string DebugResult ="";
        private TypeCode _testEnum;

        public string TestString { get => _testString;set=>SetProperty(ref _testString,value); }
        public string TestString1 { get => _testString; set => SetProperty(ref _testString, value,ValidateString); }
        public string TestString2 { get => _testString; set => SetProperty(ref _testString, value, StringAct); }
        public string TestString3 { get => _testString; set => SetProperty(ref _testString, value, ValidateString, StringAct); }
        public string TestString4 { get => _testString; set => SetProperty(ref _testString, value, new string[] {nameof(TestString),nameof(TestString2) }); }
        public string TestString5 { get => _testString; set 
                => SetProperty(ref _testString, value, new string[] { nameof(TestString), nameof(TestString1) },ValidateString); }
        public string TestString6 { get => _testString; set 
                => SetProperty(ref _testString, value, new string[] { nameof(TestString), nameof(TestString1) },StringAct); }
        public string TestString7 { get => _testString; set 
                => SetProperty(ref _testString, value, new string[] { nameof(TestString), nameof(TestString1) }, ValidateString, StringAct); }

        public int TestInt { get => _testInt; set => SetProperty(ref _testInt, value); }
        public float TestFloat { get => _testFloat; set => SetProperty(ref _testFloat, value); }
        public double TestDouble { get => _testDouble; set => SetProperty(ref _testDouble, value); }

        public TypeCode TestEnum { get => _testEnum; set => SetProperty(ref _testEnum, value); }

        private void StringAct(string arg1, string arg2)
        {
            DebugResult += $"StrAct: {arg1}; {arg2}{Environment.NewLine}";
            switch (valReact)
            {
                case eValidReact.OK:
                    return ;
                case eValidReact.NIO:
                    return ;
                case eValidReact.GeneralException:
                    throw new Exception("A general exception occured");
                case eValidReact.ArgumetException:
                    throw new ArgumentException($"Argument ({arg2}) not valid!");
                default: return;
            }
        }

        private bool ValidateString(string arg1)
        {
            DebugResult += $"Validate: {arg1}, React:{valReact}{Environment.NewLine}";
            switch (valReact)
            {
                case eValidReact.OK:
                    return true;
                case eValidReact.NIO:
                    return false;
                case eValidReact.GeneralException:
                    throw new Exception("A general exception occured");
                case eValidReact.ArgumetException:
                    throw new ArgumentException($"Argument ({arg1}) not valid!");
                default: return false;
            }
        }

        public NotificationObjectAdvTests()
        {
            PropertyChangedAdv += OnPropertyChanged;
        }

        private void Clear()
        {
            _testString = String.Empty;
            _testInt = 0;
            _testFloat = 0f;
            _testDouble = 0d;
            DebugResult = "";
        }

        private void OnPropertyChanged(object sender, PropertyChangedAdvEventArgs e)
            => DebugResult += $"OnPropChanged: o:{sender}, p:{e.PropertyName}:{sender?.GetType().GetProperty(e.PropertyName)?.GetValue(sender)}, o:{e.OldVal}, n:{e.NewVal}{Environment.NewLine}";

        [TestInitialize]
        public void Init()
        {
            Clear();
        }

        [DataTestMethod]
        [TestProperty("Author","J.C.")]
        [TestCategory("SetData")]
        [DataRow("00 Empty",0,"",eValidReact.OK,"","")]
        [DataRow("01-Test", 0, "Test", eValidReact.OK, "Test", "OnPropChanged: o:BaseLib.ViewModel.Tests.NotificationObjectAdvTests, p:TestString:Test, o:, n:Test\r\n")]
        [DataRow("10-1 Empty", 1, "", eValidReact.OK, "", "")]
        [DataRow("10-2 Test" , 1, "", eValidReact.NIO, "", "")]
        [DataRow("10-2 Test" , 1, "", eValidReact.GeneralException, "", "")]
        [DataRow("11-1 Empty", 1, "Test", eValidReact.OK, "Test", "Validate: Test, React:OK\r\nOnPropChanged: o:BaseLib.ViewModel.Tests.NotificationObjectAdvTests, p:TestString1:Test, o:, n:Test\r\n")]
        [DataRow("11-2 Test" , 1, "Test", eValidReact.NIO, "", "Validate: Test, React:NIO\r\n")]
        [DataRow("11-3 GEx"  , 1, "Test", eValidReact.GeneralException, "", "Validate: Test, React:GeneralException\r\n")]
        [DataRow("11-4 AEx"  , 1, "Test", eValidReact.ArgumetException, "", "Validate: Test, React:ArgumetException\r\n")]
        [DataRow("12-2 Test2", 1, "Test2", eValidReact.OK, "Test2", "Validate: Test2, React:OK\r\nOnPropChanged: o:BaseLib.ViewModel.Tests.NotificationObjectAdvTests, p:TestString1:Test2, o:, n:Test2\r\n")]
        [DataRow("20-1 Empty", 2, "", eValidReact.OK, "", "")]
        [DataRow("20-2 Test", 2, "", eValidReact.NIO, "", "")]
        [DataRow("21-1 Test", 2, "Test", eValidReact.OK, "Test", "OnPropChanged: o:BaseLib.ViewModel.Tests.NotificationObjectAdvTests, p:TestString2:Test, o:, n:Test\r\nStrAct: ; Test\r\n")]
        [DataRow("21-2 Test2", 2, "Test2", eValidReact.NIO, "Test2", "OnPropChanged: o:BaseLib.ViewModel.Tests.NotificationObjectAdvTests, p:TestString2:Test2, o:, n:Test2\r\nStrAct: ; Test2\r\n")]
        [DataRow("21-3 GEx", 2, "Test", eValidReact.GeneralException, "Test", "OnPropChanged: o:BaseLib.ViewModel.Tests.NotificationObjectAdvTests, p:TestString2:Test, o:, n:Test\r\nStrAct: ; Test\r\n")]
        [DataRow("21-4 AEx", 2, "Test", eValidReact.ArgumetException, "Test", "OnPropChanged: o:BaseLib.ViewModel.Tests.NotificationObjectAdvTests, p:TestString2:Test, o:, n:Test\r\nStrAct: ; Test\r\n")]
        [DataRow("30-1 Empty", 3, "", eValidReact.OK, "", "")]
        [DataRow("30-2 Test", 3, "", eValidReact.NIO, "", "")]
        [DataRow("30-2 Test", 3, "", eValidReact.GeneralException, "", "")]
        [DataRow("31-1 Empty", 3, "Test", eValidReact.OK, "Test", "Validate: Test, React:OK\r\nOnPropChanged: o:BaseLib.ViewModel.Tests.NotificationObjectAdvTests, p:TestString3:Test, o:, n:Test\r\nStrAct: ; Test\r\n")]
        [DataRow("31-2 Test", 3, "Test", eValidReact.NIO, "", "Validate: Test, React:NIO\r\n")]
        [DataRow("31-3 GEx", 3, "Test", eValidReact.GeneralException, "", "Validate: Test, React:GeneralException\r\n")]
        [DataRow("31-4 AEx", 3, "Test", eValidReact.ArgumetException, "", "Validate: Test, React:ArgumetException\r\n")]
        [DataRow("40-1 Empty", 4, "", eValidReact.OK, "", "")]
        [DataRow("40-2 Test " , 4, "", eValidReact.NIO, "", "")]
        [DataRow("41-1 Test " , 4, "Test", eValidReact.OK, "Test", "OnPropChanged: o:BaseLib.ViewModel.Tests.NotificationObjectAdvTests, p:TestString4:Test, o:, n:Test\r\nOnPropChanged: o:BaseLib.ViewModel.Tests.NotificationObjectAdvTests, p:TestString:Test, o:, n:\r\nOnPropChanged: o:BaseLib.ViewModel.Tests.NotificationObjectAdvTests, p:TestString2:Test, o:, n:\r\n")]
        [DataRow("41-2 Test2", 4, "Test2", eValidReact.NIO, "Test2", "OnPropChanged: o:BaseLib.ViewModel.Tests.NotificationObjectAdvTests, p:TestString4:Test2, o:, n:Test2\r\nOnPropChanged: o:BaseLib.ViewModel.Tests.NotificationObjectAdvTests, p:TestString:Test2, o:, n:\r\nOnPropChanged: o:BaseLib.ViewModel.Tests.NotificationObjectAdvTests, p:TestString2:Test2, o:, n:\r\n")]
        [DataRow("50-1 Empty", 5, "", eValidReact.OK, "", "")]
        [DataRow("50-2 Test ", 5, "", eValidReact.NIO, "", "")]
        [DataRow("50-2 Test ", 5, "", eValidReact.GeneralException, "", "")]
        [DataRow("51-1 Empty", 5, "Test", eValidReact.OK, "Test", "Validate: Test, React:OK\r\nOnPropChanged: o:BaseLib.ViewModel.Tests.NotificationObjectAdvTests, p:TestString5:Test, o:, n:Test\r\nOnPropChanged: o:BaseLib.ViewModel.Tests.NotificationObjectAdvTests, p:TestString:Test, o:, n:\r\nOnPropChanged: o:BaseLib.ViewModel.Tests.NotificationObjectAdvTests, p:TestString1:Test, o:, n:\r\n")]
        [DataRow("51-2 Test ", 5, "Test", eValidReact.NIO, "", "Validate: Test, React:NIO\r\n")]
        [DataRow("51-3 GEx  ", 5, "Test", eValidReact.GeneralException, "", "Validate: Test, React:GeneralException\r\n")]
        [DataRow("51-4 AEx  ", 5, "Test", eValidReact.ArgumetException, "", "Validate: Test, React:ArgumetException\r\n")]
        [DataRow("60-1 Empty", 6, "", eValidReact.OK, "", "")]
        [DataRow("60-2 Test ", 6, "", eValidReact.NIO, "", "")]
        [DataRow("61-1 Test ", 6, "Test", eValidReact.OK, "Test", "OnPropChanged: o:BaseLib.ViewModel.Tests.NotificationObjectAdvTests, p:TestString6:Test, o:, n:Test\r\nOnPropChanged: o:BaseLib.ViewModel.Tests.NotificationObjectAdvTests, p:TestString:Test, o:, n:\r\nOnPropChanged: o:BaseLib.ViewModel.Tests.NotificationObjectAdvTests, p:TestString1:Test, o:, n:\r\nStrAct: ; Test\r\n")]
        [DataRow("61-2 Test2", 6, "Test2", eValidReact.NIO, "Test2", "OnPropChanged: o:BaseLib.ViewModel.Tests.NotificationObjectAdvTests, p:TestString6:Test2, o:, n:Test2\r\nOnPropChanged: o:BaseLib.ViewModel.Tests.NotificationObjectAdvTests, p:TestString:Test2, o:, n:\r\nOnPropChanged: o:BaseLib.ViewModel.Tests.NotificationObjectAdvTests, p:TestString1:Test2, o:, n:\r\nStrAct: ; Test2\r\n")]
        [DataRow("70-1 Empty", 7, "", eValidReact.OK, "", "")]
        [DataRow("70-2 Test ", 7, "", eValidReact.NIO, "", "")]
        [DataRow("70-2 Test ", 7, "", eValidReact.GeneralException, "", "")]
        [DataRow("71-1 Empty", 7, "Test", eValidReact.OK, "Test", "Validate: Test, React:OK\r\nOnPropChanged: o:BaseLib.ViewModel.Tests.NotificationObjectAdvTests, p:TestString7:Test, o:, n:Test\r\nOnPropChanged: o:BaseLib.ViewModel.Tests.NotificationObjectAdvTests, p:TestString:Test, o:, n:\r\nOnPropChanged: o:BaseLib.ViewModel.Tests.NotificationObjectAdvTests, p:TestString1:Test, o:, n:\r\nStrAct: ; Test\r\n")]
        [DataRow("71-2 Test ", 7, "Test", eValidReact.NIO, "", "Validate: Test, React:NIO\r\n")]
        [DataRow("71-3 GEx  ", 7, "Test", eValidReact.GeneralException, "", "Validate: Test, React:GeneralException\r\n")]
        [DataRow("71-4 AEx  ", 7, "Test", eValidReact.ArgumetException, "", "Validate: Test, React:ArgumetException\r\n")]
        public void TestStringProp(string name,int iTs,string sVal, eValidReact eReact, string sExp,string sDebExp)
        {
            valReact = eReact;
            bool xCh = sVal != _testString;
            bool eRIsEx = eReact == eValidReact.GeneralException || eReact == eValidReact.ArgumetException;
            switch (iTs)
            {
                //case 1: TestString1 = sVal; break;
                case 1 when xCh && eReact == eValidReact.GeneralException: Assert.ThrowsException<Exception>(()=>TestString1 = sVal,$"{name}.T1"); break;
                case 1 when xCh && eReact == eValidReact.ArgumetException: Assert.ThrowsException<ArgumentException>(() => TestString1 = sVal, $"{name}.T1"); break;
                case 1: TestString1 = sVal; break;
                case 2: TestString2 = sVal; break;
                case 3 when xCh && eReact == eValidReact.GeneralException: Assert.ThrowsException<Exception>(() => TestString3 = sVal, $"{name}.T3"); break;
                case 3 when xCh && eReact == eValidReact.ArgumetException: Assert.ThrowsException<ArgumentException>(() => TestString3 = sVal, $"{name}.T3"); break;
                case 3: TestString3 = sVal; break;
                case 4: TestString4 = sVal; break;
                case 5 when xCh && eReact == eValidReact.GeneralException: Assert.ThrowsException<Exception>(() => TestString5 = sVal, $"{name}.T5"); break;
                case 5 when xCh && eReact == eValidReact.ArgumetException: Assert.ThrowsException<ArgumentException>(() => TestString5 = sVal, $"{name}.T5"); break;
                case 5: TestString5 = sVal; break;
                case 6: TestString6 = sVal; break;
                case 7 when xCh && eReact == eValidReact.GeneralException: Assert.ThrowsException<Exception>(() => TestString7 = sVal, $"{name}.T7"); break;
                case 7 when xCh && eReact == eValidReact.ArgumetException: Assert.ThrowsException<ArgumentException>(() => TestString7 = sVal, $"{name}.T7"); break;
                case 7: TestString7 = sVal; break;
                default: TestString = sVal; break;
            }
            Assert.AreEqual(sExp, _testString, $"{name}.Result");
            Assert.AreEqual(sDebExp, DebugResult, $"{name}.DebRes");    
        }

        [DataTestMethod]
        [TestProperty("Author", "J.C.")]
        [TestCategory("SetData")]
        [DataRow("00 Empty", 0, "", eValidReact.OK, "", "")]
        [DataRow("01-Test", 0, "Test", eValidReact.OK, "Test", "")]
        [DataRow("10-1 Empty", 1, "", eValidReact.OK, "", "")]
        [DataRow("10-2 Test", 1, "", eValidReact.NIO, "", "")]
        [DataRow("10-2 Test", 1, "", eValidReact.GeneralException, "", "")]
        [DataRow("11-1 Empty", 1, "Test", eValidReact.OK, "Test", "Validate: Test, React:OK\r\n")]
        [DataRow("11-2 Test", 1, "Test", eValidReact.NIO, "", "Validate: Test, React:NIO\r\n")]
        [DataRow("11-3 GEx", 1, "Test", eValidReact.GeneralException, "", "Validate: Test, React:GeneralException\r\n")]
        [DataRow("11-4 AEx", 1, "Test", eValidReact.ArgumetException, "", "Validate: Test, React:ArgumetException\r\n")]
        [DataRow("12-2 Test2", 1, "Test2", eValidReact.OK, "Test2", "Validate: Test2, React:OK\r\n")]
        [DataRow("20-1 Empty", 2, "", eValidReact.OK, "", "")]
        [DataRow("20-2 Test", 2, "", eValidReact.NIO, "", "")]
        [DataRow("21-1 Test", 2, "Test", eValidReact.OK, "Test", "StrAct: ; Test\r\n")]
        [DataRow("21-2 Test2", 2, "Test2", eValidReact.NIO, "Test2", "StrAct: ; Test2\r\n")]
        [DataRow("21-3 GEx", 2, "Test", eValidReact.GeneralException, "Test", "StrAct: ; Test\r\n")]
        [DataRow("21-4 AEx", 2, "Test", eValidReact.ArgumetException, "Test", "StrAct: ; Test\r\n")]
        [DataRow("30-1 Empty", 3, "", eValidReact.OK, "", "")]
        [DataRow("30-2 Test", 3, "", eValidReact.NIO, "", "")]
        [DataRow("30-2 Test", 3, "", eValidReact.GeneralException, "", "")]
        [DataRow("31-1 Empty", 3, "Test", eValidReact.OK, "Test", "Validate: Test, React:OK\r\nStrAct: ; Test\r\n")]
        [DataRow("31-2 Test", 3, "Test", eValidReact.NIO, "", "Validate: Test, React:NIO\r\n")]
        [DataRow("31-3 GEx", 3, "Test", eValidReact.GeneralException, "", "Validate: Test, React:GeneralException\r\n")]
        [DataRow("31-4 AEx", 3, "Test", eValidReact.ArgumetException, "", "Validate: Test, React:ArgumetException\r\n")]
        [DataRow("40-1 Empty", 4, "", eValidReact.OK, "", "")]
        [DataRow("40-2 Test ", 4, "", eValidReact.NIO, "", "")]
        [DataRow("41-1 Test ", 4, "Test", eValidReact.OK, "Test", "")]
        [DataRow("41-2 Test2", 4, "Test2", eValidReact.NIO, "Test2", "")]
        [DataRow("50-1 Empty", 5, "", eValidReact.OK, "", "")]
        [DataRow("50-2 Test ", 5, "", eValidReact.NIO, "", "")]
        [DataRow("50-2 Test ", 5, "", eValidReact.GeneralException, "", "")]
        [DataRow("51-1 Empty", 5, "Test", eValidReact.OK, "Test", "Validate: Test, React:OK\r\n")]
        [DataRow("51-2 Test ", 5, "Test", eValidReact.NIO, "", "Validate: Test, React:NIO\r\n")]
        [DataRow("51-3 GEx  ", 5, "Test", eValidReact.GeneralException, "", "Validate: Test, React:GeneralException\r\n")]
        [DataRow("51-4 AEx  ", 5, "Test", eValidReact.ArgumetException, "", "Validate: Test, React:ArgumetException\r\n")]
        [DataRow("60-1 Empty", 6, "", eValidReact.OK, "", "")]
        [DataRow("60-2 Test ", 6, "", eValidReact.NIO, "", "")]
        [DataRow("61-1 Test ", 6, "Test", eValidReact.OK, "Test", "StrAct: ; Test\r\n")]
        [DataRow("61-2 Test2", 6, "Test2", eValidReact.NIO, "Test2", "StrAct: ; Test2\r\n")]
        [DataRow("70-1 Empty", 7, "", eValidReact.OK, "", "")]
        [DataRow("70-2 Test ", 7, "", eValidReact.NIO, "", "")]
        [DataRow("70-2 Test ", 7, "", eValidReact.GeneralException, "", "")]
        [DataRow("71-1 Empty", 7, "Test", eValidReact.OK, "Test", "Validate: Test, React:OK\r\nStrAct: ; Test\r\n")]
        [DataRow("71-2 Test ", 7, "Test", eValidReact.NIO, "", "Validate: Test, React:NIO\r\n")]
        [DataRow("71-3 GEx  ", 7, "Test", eValidReact.GeneralException, "", "Validate: Test, React:GeneralException\r\n")]
        [DataRow("71-4 AEx  ", 7, "Test", eValidReact.ArgumetException, "", "Validate: Test, React:ArgumetException\r\n")]
        public void TestStringProp2(string name, int iTs, string sVal, eValidReact eReact, string sExp, string sDebExp)
        {
            PropertyChangedAdv -= OnPropertyChanged;
            valReact = eReact;
            bool xCh = sVal != _testString;
            bool eRIsEx = eReact == eValidReact.GeneralException || eReact == eValidReact.ArgumetException;
            switch (iTs)
            {
                //case 1: TestString1 = sVal; break;
                case 1 when xCh && eReact == eValidReact.GeneralException: Assert.ThrowsException<Exception>(() => TestString1 = sVal, $"{name}.T1"); break;
                case 1 when xCh && eReact == eValidReact.ArgumetException: Assert.ThrowsException<ArgumentException>(() => TestString1 = sVal, $"{name}.T1"); break;
                case 1: TestString1 = sVal; break;
                case 2: TestString2 = sVal; break;
                case 3 when xCh && eReact == eValidReact.GeneralException: Assert.ThrowsException<Exception>(() => TestString3 = sVal, $"{name}.T3"); break;
                case 3 when xCh && eReact == eValidReact.ArgumetException: Assert.ThrowsException<ArgumentException>(() => TestString3 = sVal, $"{name}.T3"); break;
                case 3: TestString3 = sVal; break;
                case 4: TestString4 = sVal; break;
                case 5 when xCh && eReact == eValidReact.GeneralException: Assert.ThrowsException<Exception>(() => TestString5 = sVal, $"{name}.T5"); break;
                case 5 when xCh && eReact == eValidReact.ArgumetException: Assert.ThrowsException<ArgumentException>(() => TestString5 = sVal, $"{name}.T5"); break;
                case 5: TestString5 = sVal; break;
                case 6: TestString6 = sVal; break;
                case 7 when xCh && eReact == eValidReact.GeneralException: Assert.ThrowsException<Exception>(() => TestString7 = sVal, $"{name}.T7"); break;
                case 7 when xCh && eReact == eValidReact.ArgumetException: Assert.ThrowsException<ArgumentException>(() => TestString7 = sVal, $"{name}.T7"); break;
                case 7: TestString7 = sVal; break;
                default: TestString = sVal; break;
            }
            Assert.AreEqual(sExp, _testString, $"{name}.Result");
            Assert.AreEqual(sDebExp, DebugResult, $"{name}.DebRes");
        }

    }
}
