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
        public string TestString2 { get => _testString; set => SetProperty(ref _testString, value,ValidateString); }
        public string TestString3 { get => _testString; set => SetProperty(ref _testString, value, StringAct); }
        public string TestString4 { get => _testString; set => SetProperty(ref _testString, value, new string[] {nameof(TestString),nameof(TestString3) }); }
        public string TestString5 { get => _testString; set 
                => SetProperty(ref _testString, value, new string[] { nameof(TestString), nameof(TestString2) },ValidateString); }
        public string TestString6 { get => _testString; set 
                => SetProperty(ref _testString, value, new string[] { nameof(TestString), nameof(TestString2) },StringAct); }
        public string TestString7 { get => _testString; set 
                => SetProperty(ref _testString, value, new string[] { nameof(TestString), nameof(TestString2) }, ValidateString, StringAct); }

        public int TestInt { get => _testInt; set => SetProperty(ref _testInt, value); }
        public float TestFloat { get => _testFloat; set => SetProperty(ref _testFloat, value); }
        public double TestDouble { get => _testDouble; set => SetProperty(ref _testDouble, value); }

        private void StringAct(string arg1, string arg2)
        {
            DebugResult += $"StrAct: {arg1}; {arg2}{Environment.NewLine}";
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

        public NotificationObjectTests()
        {
            PropertyChanged += OnPropertyChanged;
        }

        private void Clear()
        {
            _testString = String.Empty;
            _testInt = 0;
            _testFloat = 0f;
            _testDouble = 0d;
            DebugResult = "";
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
            => DebugResult += $"OnPropChanged: o:{sender}, p:{e.PropertyName}:{sender?.GetType().GetProperty(e.PropertyName)?.GetValue(sender)}{Environment.NewLine}";

        [TestInitialize]
        public void Init()
        {
            Clear();
        }

        [DataTestMethod]
        [TestProperty("Author","J.C.")]
        [DataRow("Empty",0,"",eValidReact.OK,"","")]
        public void TestStringProp(string name,int iTs,string sVal, eValidReact eReact, string sExp,string sDebExp)
        {
            valReact = eReact;
            bool xCh = sVal != _testString;
            bool eRIsEx = eReact == eValidReact.GeneralException || eReact == eValidReact.ArgumetException;
            switch (iTs)
            {
                //case 1: TestString1 = sVal; break;
                case 2 when xCh && eReact == eValidReact.GeneralException: Assert.ThrowsException<Exception>(()=>TestString2 = sVal,$"{name}.T2"); break;
                case 2 when xCh && eReact == eValidReact.ArgumetException: Assert.ThrowsException<ArgumentException>(() => TestString2 = sVal, $"{name}.T2"); break;
                case 2: TestString2 = sVal; break;
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
