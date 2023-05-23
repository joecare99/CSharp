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
    public class NotificationObjectCTTests : NotificationObjectCT
    {
        public enum eValidReact
        {
            OK=0,
            NIO,
            GeneralException,
            ArgumentException,
        }

        private string _testString="";
        private int _testInt;
        private float _testFloat;
        private double _testDouble;

        private eValidReact valReact=eValidReact.OK;

        private string DebugResult ="";

        public string TestString { get => _testString;set=>SetProperty(ref _testString,value); }
           
        public string TestString0a { get => _testString; set => ExecPropSetter((v) => _testString = v, _testString, value); }
        public string TestString2a { get => _testString; set => ExecPropSetter((v) => _testString = v, _testString, value, StringAct); }
        public string TestString6a { get => _testString; set
                => ExecPropSetter((v)=>_testString=v, _testString, value, new string[] { nameof(TestString), nameof(TestString) },null, StringAct);}
        public string TestString7a { get => _testString; set
                => ExecPropSetter((v)=>_testString=v, _testString, value, new string[] { nameof(TestString), nameof(TestString) }, ValidateString, StringAct);}

        public int TestInt { get => _testInt; set => SetProperty(ref _testInt, value); }
        public float TestFloat { get => _testFloat; set => SetProperty(ref _testFloat, value); }
        public double TestDouble { get => _testDouble; set => SetProperty(ref _testDouble, value); }

        private void StringAct(string arg1, string arg2)
        {
            DebugResult += $"StrAct: {arg1}; {arg2}{Environment.NewLine}";
            var _=valReact switch
            {
                eValidReact.GeneralException => throw new Exception("A general exception occurred"),
                eValidReact.ArgumentException => throw new ArgumentException($"Argument ({arg2}) not valid!"),
                _ => (object?)null,
            };
        }

        private bool ValidateString(string arg1)
        {
            DebugResult += $"Validate: {arg1}, React:{valReact}{Environment.NewLine}";
            return valReact switch
            {
                eValidReact.OK => true,
                eValidReact.NIO => false,
                eValidReact.GeneralException => throw new Exception("A general exception occurred"),
                eValidReact.ArgumentException => throw new ArgumentException($"Argument ({arg1}) not valid!"),
                _ => false,
            };
        }

        public NotificationObjectCTTests()
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

        private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
            => DebugResult += $"OnPropChanged: o:{sender}, p:{e.PropertyName}:{sender?.GetType().GetProperty(e.PropertyName!)?.GetValue(sender)}{Environment.NewLine}";

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
            OnPropertyChanged("Test");
            OnPropertyChanged("Test2");
            Assert.AreEqual("", DebugResult);
        }

        [DataTestMethod]
        [TestProperty("Author","J.C.")]
        [DataRow("00 Empty",0,"",eValidReact.OK,"","")]
        [DataRow("01-Test", 0, "Test", eValidReact.OK, "Test", "OnPropChanged: o:MVVM_BaseLibTests.ViewModel.NotificationObjectTests, p:TestString:Test\r\n")]
               public void TestStringProp(string name,int iTs,string sVal, eValidReact eReact, string sExp,string sDebExp)
        {
            valReact = eReact;
            bool xCh = sVal != _testString;
            bool eRIsEx = eReact == eValidReact.GeneralException || eReact == eValidReact.ArgumentException;
            bool xITsHasVl = new[]{ 1,3,5,7}.Contains(iTs);
            Action<string> Setter = iTs switch
            {            
                _ => (s) => TestString = s
            };
            if (xCh && xITsHasVl && eReact == eValidReact.GeneralException)
                Assert.ThrowsException<Exception>(() => Setter(sVal), $"{name}.T{iTs}");
            else if (xCh && xITsHasVl && eReact == eValidReact.ArgumentException)
                Assert.ThrowsException<ArgumentException>(() => Setter(sVal), $"{name}.T{iTs}");
            else
                Setter(sVal);

            Assert.AreEqual(sExp, _testString, $"{name}.Result");
            Assert.AreEqual(sDebExp, DebugResult, $"{name}.DebRes");    
        }

        [DataTestMethod]
        [TestProperty("Author", "J.C.")]
        [DataRow("00 Empty", 0, "", eValidReact.OK, "", new string[] { "" })]
        [DataRow("01-Test", 0, "Test", eValidReact.OK, "Test", new string[] { "OnPropChanged: o:MVVM_BaseLibTests.ViewModel.NotificationObjectTests, p:TestString0a:Test\r\n" })]
        [DataRow("20-1 Empty", 2, "", eValidReact.OK, "", new string[] { "" })]
        [DataRow("20-2 Test", 2, "", eValidReact.NIO, "", new string[] { "" })]
        [DataRow("21-1 Test", 2, "Test", eValidReact.OK, "Test", new string[] { "OnPropChanged: o:MVVM_BaseLibTests.ViewModel.NotificationObjectTests, p:TestString2a:Test\r\nStrAct: ; Test\r\n" })]
        [DataRow("21-2 Test2", 2, "Test2", eValidReact.NIO, "Test2", new string[] { "OnPropChanged: o:MVVM_BaseLibTests.ViewModel.NotificationObjectTests, p:TestString2a:Test2\r\nStrAct: ; Test2\r\n" })]
        [DataRow("21-3 GEx", 2, "Test", eValidReact.GeneralException, "Test", new string[] { "OnPropChanged: o:MVVM_BaseLibTests.ViewModel.NotificationObjectTests, p:TestString2a:Test\r\nStrAct: ; Test\r\n" })]
        [DataRow("21-4 AEx", 2, "Test2", eValidReact.ArgumentException, "Test2", new string[] { "OnPropChanged: o:MVVM_BaseLibTests.ViewModel.NotificationObjectTests, p:TestString2a:Test2\r\nStrAct: ; Test2\r\n" })]
        [DataRow("60-1 Empty", 6, "", eValidReact.OK, "", new string[] { "" })]
        [DataRow("60-2 Test ", 6, "", eValidReact.NIO, "", new string[] { "" })]
        [DataRow("61-1 Test ", 6, "Test", eValidReact.OK, "Test", new string[] { "OnPropChanged: o:MVVM_BaseLibTests.ViewModel.NotificationObjectTests, p:TestString6a:Test\r\nOnPropChanged: o:MVVM_BaseLibTests.ViewModel.NotificationObjectTests, p:TestString:Test\r\nOnPropChanged: o:MVVM_BaseLibTests.ViewModel.NotificationObjectTests, p:TestString1:Test\r\nStrAct: ; Test\r\n" })]
        [DataRow("61-2 Test2", 6, "Test2", eValidReact.NIO, "Test2", new string[] { "OnPropChanged: o:MVVM_BaseLibTests.ViewModel.NotificationObjectTests, p:TestString6a:Test2\r\nOnPropChanged: o:MVVM_BaseLibTests.ViewModel.NotificationObjectTests, p:TestString:Test2\r\nOnPropChanged: o:MVVM_BaseLibTests.ViewModel.NotificationObjectTests, p:TestString1:Test2\r\nStrAct: ; Test2\r\n" })]
        [DataRow("61-3 GEx  ", 6, "Test", eValidReact.GeneralException, "Test", new string[] { "OnPropChanged: o:MVVM_BaseLibTests.ViewModel.NotificationObjectTests, p:TestString6a:Test\r\nOnPropChanged: o:MVVM_BaseLibTests.ViewModel.NotificationObjectTests, p:TestString:Test\r\nOnPropChanged: o:MVVM_BaseLibTests.ViewModel.NotificationObjectTests, p:TestString1:Test\r\nStrAct: ; Test\r\n" })]
        [DataRow("70-1 Empty", 7, "", eValidReact.OK, "", new string[] { "" })]
        [DataRow("70-2 Test ", 7, "", eValidReact.NIO, "", new string[] { "" })]
        [DataRow("70-2 Test ", 7, "", eValidReact.GeneralException, "", new string[] { "" })]
        [DataRow("71-1 Empty", 7, "Test", eValidReact.OK, "Test", new string[] { "Validate: Test, React:OK\r\nOnPropChanged: o:MVVM_BaseLibTests.ViewModel.NotificationObjectTests, p:TestString7a:Test\r\nOnPropChanged: o:MVVM_BaseLibTests.ViewModel.NotificationObjectTests, p:TestString:Test\r\nOnPropChanged: o:MVVM_BaseLibTests.ViewModel.NotificationObjectTests, p:TestString1:Test\r\nStrAct: ; Test\r\n" })]
        [DataRow("71-2 Test ", 7, "Test", eValidReact.NIO, "", new string[] { "Validate: Test, React:NIO\r\n" })]
        [DataRow("71-3 GEx  ", 7, "Test", eValidReact.GeneralException, "", new string[] { "Validate: Test, React:GeneralException\r\n" })]
        [DataRow("71-4 AEx  ", 7, "Test", eValidReact.ArgumentException, "",new string[] { "Validate: Test, React:ArgumentException\r\n" })]
        public void TestString2Prop(string name, int iTs, string sVal, eValidReact eReact, string sExp, string[] sDebExp)
        {
            valReact = eReact;
            bool xCh = sVal != _testString;
            bool eRIsEx = eReact == eValidReact.GeneralException || eReact == eValidReact.ArgumentException;
            bool xITsHasVl = new[] { 1, 3, 5, 7 }.Contains(iTs);
            Action<string> Setter = iTs switch
            {
                2 => (s) => TestString2a = s,
                6 => (s) => TestString6a = s,
                7 => (s) => TestString7a = s,
                _ => (s) => TestString0a = s
            };

            if (xCh && xITsHasVl && eReact == eValidReact.GeneralException)
                Assert.ThrowsException<Exception>(() => Setter(sVal), $"{name}.T{iTs}");
            else if (xCh && xITsHasVl && eReact == eValidReact.ArgumentException) 
                Assert.ThrowsException<ArgumentException>(() => Setter(sVal), $"{name}.T{iTs}");
            else
                Setter(sVal);

            Assert.AreEqual(sExp, _testString, $"{name}.Result");
            Assert.AreEqual(sDebExp[0], DebugResult, $"{name}.DebRes");
        }

    }
}
