using Microsoft.VisualStudio.TestTools.UnitTesting;
using BaseLib.Helper.MVVM;
using System;
using System.ComponentModel;
using static BaseLib.Helper.TestHelper;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace BaseLib.Helper.MVVM.Tests
{
    [TestClass()]
    public class ValidationHelperTests
    {
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
        ValidationHelper _helper;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
        private string DebugLog="";

        [TestInitialize]
        public void Init()
        {
            _helper = new();
            _helper.ErrorsChanged += OnErrorsChanged;
            DebugLog = "";
        }

        private void OnErrorsChanged(object? sender, DataErrorsChangedEventArgs e)
        {
            DoLog($"ErrorsChg({sender},{e.PropertyName})");
        }

        private void DoLog(string v)
        {
            DebugLog+=$"{v}{Environment.NewLine}";
        }

        [TestMethod()]
        public void SetupTest()
        {
            Assert.IsNotNull(_helper);
            Assert.IsInstanceOfType(_helper, typeof(ValidationHelper));
            Assert.IsInstanceOfType(_helper, typeof(INotifyDataErrorInfo));
            Assert.AreEqual(false, _helper.HasErrors);
            Assert.AreEqual("", DebugLog);
        }

        [TestMethod()]
        public void SetupTest2()
        {
            CreateTestData(_helper);
            Assert.AreEqual(true, _helper.HasErrors);
            Assert.AreEqual("", DebugLog);
        }

        private void CreateTestData(ValidationHelper helper)
        {
            helper.AddError("SomeProp", "Not a valid property");
            helper.AddError("SomeProp1", "Not a valid property (1)");
            helper.AddError("SomeProp2", "Not a valid property (2)");
            helper.AddError("SomeProp3", "Not a valid property (3)");
            Assert.AreEqual(@"ErrorsChg(BaseLib.Helper.MVVM.ValidationHelper,SomeProp)
ErrorsChg(BaseLib.Helper.MVVM.ValidationHelper,SomeProp1)
ErrorsChg(BaseLib.Helper.MVVM.ValidationHelper,SomeProp2)
ErrorsChg(BaseLib.Helper.MVVM.ValidationHelper,SomeProp3)
", DebugLog);
            DebugLog = "";

        }

        [TestMethod()]
        public void GetErrorsTest()
        {
            AssertAreEqual(Array.Empty<string>(), _helper.GetErrors("SomeProp"));
            CreateTestData(_helper);
            AssertAreEqual(new List<string>() { "Not a valid property" }, (_helper.GetErrors("SomeProp") as List<ValidationResult>)?.ConvertAll(o=>o.ErrorMessage??"")!);
        }

        [DataTestMethod()]
        [DataRow(new string []{}, new[] { "" },DisplayName ="Empty")]
        [DataRow(new[] { "SomeProp", "Not a valid property" }, new[] { "ErrorsChg(BaseLib.Helper.MVVM.ValidationHelper,SomeProp)\r\n",
        "SomeProp","Not a valid property" }, DisplayName = "1 SomeProp")]
        [DataRow(new[] { "SomeProp", "Not a valid property","SomeProp2", "Not a valid property (2)" }, new[] { "ErrorsChg(BaseLib.Helper.MVVM.ValidationHelper,SomeProp)\r\nErrorsChg(BaseLib.Helper.MVVM.ValidationHelper,SomeProp2)\r\n",
        "SomeProp","Not a valid property","SomeProp2", "Not a valid property (2)" }, DisplayName = "2 SomeProp,SomeProp2")]
        [DataRow(new[] { "SomeProp", "Not a valid property", "SomeProp", "Something else" }, new[] { "ErrorsChg(BaseLib.Helper.MVVM.ValidationHelper,SomeProp)\r\nErrorsChg(BaseLib.Helper.MVVM.ValidationHelper,SomeProp)\r\n",
        "SomeProp","Not a valid property","SomeProp2", null! }, DisplayName = "3 SomeProp,SomeProp")]
        public void AddErrorTest(string[] Props, string[] asExp)
        {
            for (int i=0;i< Props.Length ;i+=2)
                Assert.AreEqual(null, _helper[Props[i]]);
            for (int i = 0; i < Props.Length; i += 2)
                _helper.AddError(Props[i], Props[i+1]);
            for (int i = 1; i < asExp.Length; i += 2)
                Assert.AreEqual(asExp[i+1], _helper[asExp[i]], asExp[i]);
            Assert.AreEqual(asExp[0], DebugLog);
        }

        [TestMethod()]
        public void AddError2Test()
        {
            _helper.ErrorsChanged -= OnErrorsChanged;
            _helper.AddError("SomeProp", "Not a valid property");
            Assert.AreEqual("Not a valid property", _helper["SomeProp"]);
            Assert.AreEqual("", DebugLog);
        }
        [TestMethod()]
        public void ClearErrorsTest()
        {
            CreateTestData(_helper);
            _helper.ClearErrors("SomeProp");
            Assert.AreEqual(true, _helper.HasErrors);
            _helper.ClearErrors("SomeProp1");
            Assert.AreEqual(true, _helper.HasErrors);
            _helper.ClearErrors("SomeProp2");
            Assert.AreEqual(true, _helper.HasErrors);
            _helper.ClearErrors("SomeProp2");
            Assert.AreEqual(true, _helper.HasErrors);
            _helper.ClearErrors("SomeProp3");
            Assert.AreEqual(false, _helper.HasErrors);
            Assert.AreEqual(@"ErrorsChg(BaseLib.Helper.MVVM.ValidationHelper,SomeProp)
ErrorsChg(BaseLib.Helper.MVVM.ValidationHelper,SomeProp1)
ErrorsChg(BaseLib.Helper.MVVM.ValidationHelper,SomeProp2)
ErrorsChg(BaseLib.Helper.MVVM.ValidationHelper,SomeProp2)
ErrorsChg(BaseLib.Helper.MVVM.ValidationHelper,SomeProp3)
", DebugLog);
        }

        [TestMethod()]
        public void ClearErrors2Test()
        {
            CreateTestData(_helper);
            _helper.ErrorsChanged -= OnErrorsChanged;
            _helper.ClearErrors("SomeProp");
            Assert.AreEqual(true, _helper.HasErrors);
            Assert.AreEqual(@"", DebugLog);
        }
    }
}