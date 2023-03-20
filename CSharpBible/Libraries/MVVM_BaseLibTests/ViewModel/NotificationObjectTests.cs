using Microsoft.VisualStudio.TestTools.UnitTesting;
using MVVM.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVM_BaseLibTests.ViewModel
{
    /// <summary>
    /// Defines test class PropertyTests.
    /// </summary>
    [TestClass()]
    public class NotificationObjectTests : NotificationObject
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

        public string TestString { get => _testString;set=>SetProperty(ref _testString,value); }
        public string TestString1 { get => _testString; set => SetProperty(ref _testString, value, ValidateString); }
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

        private void StringAct(string arg1, string arg2)
        {
            DebugResult += $"StrAct: {arg1}; {arg2}{Environment.NewLine}";
            var _=valReact switch
            {
                eValidReact.GeneralException => throw new Exception("A general exception occured"),
                eValidReact.ArgumetException => throw new ArgumentException($"Argument ({arg2}) not valid!"),
                _ => (object)null,
            };
        }

        private bool ValidateString(string arg1)
        {
            DebugResult += $"Validate: {arg1}, React:{valReact}{Environment.NewLine}";
            return valReact switch
            {
                eValidReact.OK => true,
                eValidReact.NIO => false,
                eValidReact.GeneralException => throw new Exception("A general exception occured"),
                eValidReact.ArgumetException => throw new ArgumentException($"Argument ({arg1}) not valid!"),
                _ => false,
            };
        }

        public NotificationObjectTests()
        {
        }

        private void Clear()
        {
            _testString = String.Empty;
            _testInt = 0;
            _testFloat = 0f;
            _testDouble = 0d;
            DebugResult = "";
            PropertyChanged -= OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
            => DebugResult += $"OnPropChanged: o:{sender}, p:{e.PropertyName}:{sender?.GetType().GetProperty(e.PropertyName)?.GetValue(sender)}{Environment.NewLine}";

        [TestInitialize]
        public void Init()
        {
            Clear();
            PropertyChanged += OnPropertyChanged;
        }

        [TestMethod()]
        public void TestRaise()
        {
            PropertyChanged -= OnPropertyChanged;
            RaisePropertyChanged("Test");
            RaisePropertyChanged("Test","Test2");
            Assert.AreEqual("", DebugResult);
        }

        [DataTestMethod]
        [TestProperty("Author","J.C.")]
        [DataRow("00 Empty",0,"",eValidReact.OK,"","")]
        [DataRow("01-Test", 0, "Test", eValidReact.OK, "Test", "OnPropChanged: o:MVVM_BaseLibTests.ViewModel.NotificationObjectTests, p:TestString:Test\r\n")]
        [DataRow("10-1 Empty", 1, "", eValidReact.OK, "", "")]
        [DataRow("10-2 Test" , 1, "", eValidReact.NIO, "", "")]
        [DataRow("10-2 Test" , 1, "", eValidReact.GeneralException, "", "")]
        [DataRow("11-1 Empty", 1, "Test", eValidReact.OK, "Test", "Validate: Test, React:OK\r\nOnPropChanged: o:MVVM_BaseLibTests.ViewModel.NotificationObjectTests, p:TestString1:Test\r\n")]
        [DataRow("11-2 Test" , 1, "Test", eValidReact.NIO, "", "Validate: Test, React:NIO\r\n")]
        [DataRow("11-3 GEx"  , 1, "Test", eValidReact.GeneralException, "", "Validate: Test, React:GeneralException\r\n")]
        [DataRow("11-4 AEx"  , 1, "Test", eValidReact.ArgumetException, "", "Validate: Test, React:ArgumetException\r\n")]
        [DataRow("20-1 Empty", 2, "", eValidReact.OK, "", "")]
        [DataRow("20-2 Test", 2, "", eValidReact.NIO, "", "")]
        [DataRow("21-1 Test", 2, "Test", eValidReact.OK, "Test", "OnPropChanged: o:MVVM_BaseLibTests.ViewModel.NotificationObjectTests, p:TestString2:Test\r\nStrAct: ; Test\r\n")]
        [DataRow("21-2 Test2", 2, "Test2", eValidReact.NIO, "Test2", "OnPropChanged: o:MVVM_BaseLibTests.ViewModel.NotificationObjectTests, p:TestString2:Test2\r\nStrAct: ; Test2\r\n")]
		[DataRow("21-3 GEx", 2, "Test", eValidReact.GeneralException, "Test", "OnPropChanged: o:MVVM_BaseLibTests.ViewModel.NotificationObjectTests, p:TestString2:Test\r\nStrAct: ; Test\r\n")]
		[DataRow("21-4 AEx", 2, "Test2", eValidReact.ArgumetException, "Test2", "OnPropChanged: o:MVVM_BaseLibTests.ViewModel.NotificationObjectTests, p:TestString2:Test2\r\nStrAct: ; Test2\r\n")]
		[DataRow("30-1 Empty", 3, "", eValidReact.OK, "", "")]
		[DataRow("30-2 Test", 3, "", eValidReact.NIO, "", "")]
		[DataRow("30-2 Test", 3, "", eValidReact.GeneralException, "", "")]
		[DataRow("31-1 Empty", 3, "Test", eValidReact.OK, "Test", "Validate: Test, React:OK\r\nOnPropChanged: o:MVVM_BaseLibTests.ViewModel.NotificationObjectTests, p:TestString3:Test\r\nStrAct: ; Test\r\n")]
		[DataRow("31-2 Test", 3, "Test", eValidReact.NIO, "", "Validate: Test, React:NIO\r\n")]
		[DataRow("31-3 GEx", 3, "Test", eValidReact.GeneralException, "", "Validate: Test, React:GeneralException\r\n")]
		[DataRow("31-4 AEx", 3, "Test", eValidReact.ArgumetException, "", "Validate: Test, React:ArgumetException\r\n")]
		[DataRow("32-1 Test2", 3, "Test2", eValidReact.OK, "Test2", "Validate: Test2, React:OK\r\nOnPropChanged: o:MVVM_BaseLibTests.ViewModel.NotificationObjectTests, p:TestString3:Test2\r\nStrAct: ; Test2\r\n")]
		[DataRow("40-1 Empty", 4, "", eValidReact.OK, "", "")]
        [DataRow("40-2 Test " , 4, "", eValidReact.NIO, "", "")]
        [DataRow("41-1 Test " , 4, "Test", eValidReact.OK, "Test", "OnPropChanged: o:MVVM_BaseLibTests.ViewModel.NotificationObjectTests, p:TestString4:Test\r\nOnPropChanged: o:MVVM_BaseLibTests.ViewModel.NotificationObjectTests, p:TestString:Test\r\nOnPropChanged: o:MVVM_BaseLibTests.ViewModel.NotificationObjectTests, p:TestString2:Test\r\n")]
        [DataRow("41-2 Test2", 4, "Test2", eValidReact.NIO, "Test2", "OnPropChanged: o:MVVM_BaseLibTests.ViewModel.NotificationObjectTests, p:TestString4:Test2\r\nOnPropChanged: o:MVVM_BaseLibTests.ViewModel.NotificationObjectTests, p:TestString:Test2\r\nOnPropChanged: o:MVVM_BaseLibTests.ViewModel.NotificationObjectTests, p:TestString2:Test2\r\n")]
        [DataRow("50-1 Empty", 5, "", eValidReact.OK, "", "")]
        [DataRow("50-2 Test ", 5, "", eValidReact.NIO, "", "")]
        [DataRow("50-2 Test ", 5, "", eValidReact.GeneralException, "", "")]
        [DataRow("51-1 Empty", 5, "Test", eValidReact.OK, "Test", "Validate: Test, React:OK\r\nOnPropChanged: o:MVVM_BaseLibTests.ViewModel.NotificationObjectTests, p:TestString5:Test\r\nOnPropChanged: o:MVVM_BaseLibTests.ViewModel.NotificationObjectTests, p:TestString:Test\r\nOnPropChanged: o:MVVM_BaseLibTests.ViewModel.NotificationObjectTests, p:TestString1:Test\r\n")]
        [DataRow("51-2 Test ", 5, "Test", eValidReact.NIO, "", "Validate: Test, React:NIO\r\n")]
        [DataRow("51-3 GEx  ", 5, "Test", eValidReact.GeneralException, "", "Validate: Test, React:GeneralException\r\n")]
        [DataRow("51-4 AEx  ", 5, "Test", eValidReact.ArgumetException, "", "Validate: Test, React:ArgumetException\r\n")]
        [DataRow("60-1 Empty", 6, "", eValidReact.OK, "", "")]
        [DataRow("60-2 Test ", 6, "", eValidReact.NIO, "", "")]
        [DataRow("61-1 Test ", 6, "Test", eValidReact.OK, "Test", "OnPropChanged: o:MVVM_BaseLibTests.ViewModel.NotificationObjectTests, p:TestString6:Test\r\nOnPropChanged: o:MVVM_BaseLibTests.ViewModel.NotificationObjectTests, p:TestString:Test\r\nOnPropChanged: o:MVVM_BaseLibTests.ViewModel.NotificationObjectTests, p:TestString1:Test\r\nStrAct: ; Test\r\n")]
        [DataRow("61-2 Test2", 6, "Test2", eValidReact.NIO, "Test2", "OnPropChanged: o:MVVM_BaseLibTests.ViewModel.NotificationObjectTests, p:TestString6:Test2\r\nOnPropChanged: o:MVVM_BaseLibTests.ViewModel.NotificationObjectTests, p:TestString:Test2\r\nOnPropChanged: o:MVVM_BaseLibTests.ViewModel.NotificationObjectTests, p:TestString1:Test2\r\nStrAct: ; Test2\r\n")]
        [DataRow("70-1 Empty", 7, "", eValidReact.OK, "", "")]
        [DataRow("70-2 Test ", 7, "", eValidReact.NIO, "", "")]
        [DataRow("70-2 Test ", 7, "", eValidReact.GeneralException, "", "")]
        [DataRow("71-1 Empty", 7, "Test", eValidReact.OK, "Test", "Validate: Test, React:OK\r\nOnPropChanged: o:MVVM_BaseLibTests.ViewModel.NotificationObjectTests, p:TestString7:Test\r\nOnPropChanged: o:MVVM_BaseLibTests.ViewModel.NotificationObjectTests, p:TestString:Test\r\nOnPropChanged: o:MVVM_BaseLibTests.ViewModel.NotificationObjectTests, p:TestString1:Test\r\nStrAct: ; Test\r\n")]
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

    }
}
