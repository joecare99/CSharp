using BaseLib.Helper;
using CommunityToolkit.Mvvm.Input;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MVVM.View.Extension;
using System;
using System.Collections.Generic;
using System.Configuration;
using VBUnObfusicator.Models;
using VBUnObfusicator.Models.Tests;
using static VBUnObfusicator.Helper.TestHelper;

#pragma warning disable IDE0130 // Der Namespace entspricht stimmt nicht der Ordnerstruktur.
namespace VBUnObfusicator.ViewModels.Tests
#pragma warning restore IDE0130 // Der Namespace entspricht stimmt nicht der Ordnerstruktur.
{
    [TestClass]
    public class CodeUnObFusViewModelTests : TestBase, ICSCode
    {
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
        private CodeUnObFusViewModel _testViewModel;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
        private ICSCode.ICodeBlock _parseResult = new CSCode.CodeBlock() { Type = ICSCode.CodeBlockType.Unknown, Code = "<ParseResult>", Name = "ParseResult" };
        private string _toCodeResult = "<ToCodeResult>";
        private string _orginalCode = string.Empty;

        public string OriginalCode { get => _orginalCode; set => value.SetProperty(ref _orginalCode, (s, o, n) => DoLog($"SetProp({s},{o},{n})")); }

        [TestInitialize]
        public void TestInitialize()
        {
            IoC.GetReqSrv = (t) => t switch
            {
                Type tp when tp == typeof(ICSCode) => this,
                _ => throw new NotImplementedException()
            };
            _testViewModel = new();
            _testViewModel.PropertyChanging += OnVMPropertyChanging;
            _testViewModel.PropertyChanged += OnVMPropertyChanged;
            _testViewModel.ErrorsChanged += OnVMErrorsChanged;
            //   _testViewModel. += OnCanExChanged;
        }
        [TestMethod]
        public void SetUpTest()
        {
            Assert.IsNotNull(_testViewModel);
            Assert.IsInstanceOfType(_testViewModel, typeof(CodeUnObFusViewModel));
        }

        [DataTestMethod]
        [DataRow("Code", new[] { "PropChgn(VBUnObfusicator.ViewModels.CodeUnObFusViewModel,Code)=\r\nPropChg(VBUnObfusicator.ViewModels.CodeUnObFusViewModel,Code)=Code\r\n" })]
        [DataRow("Code2", new[] { "PropChgn(VBUnObfusicator.ViewModels.CodeUnObFusViewModel,Code)=\r\nPropChg(VBUnObfusicator.ViewModels.CodeUnObFusViewModel,Code)=Code2\r\n" })]
        public void CodeTest(string code, string[] asExp)
        {
            Assert.IsNotNull(_testViewModel.Code);
            Assert.AreEqual(string.Empty, _testViewModel.Code);
            _testViewModel.Code = code;
            Assert.AreEqual(code, _testViewModel.Code);
            Assert.AreEqual(asExp[0], DebugLog);
        }

        [DataTestMethod]
        [DataRow("Result", new[] { "PropChgn(VBUnObfusicator.ViewModels.CodeUnObFusViewModel,Result)=\r\nPropChg(VBUnObfusicator.ViewModels.CodeUnObFusViewModel,Result)=Result\r\n" })]
        [DataRow("Result2", new[] { "PropChgn(VBUnObfusicator.ViewModels.CodeUnObFusViewModel,Result)=\r\nPropChg(VBUnObfusicator.ViewModels.CodeUnObFusViewModel,Result)=Result2\r\n" })]
        public void ResultTest(string result, string[] asExp)
        {
            Assert.IsNotNull(_testViewModel.Result);
            Assert.AreEqual(string.Empty, _testViewModel.Result);
            _testViewModel.Result = result;
            Assert.AreEqual(result, _testViewModel.Result);
            Assert.AreEqual(asExp[0], DebugLog);
        }

        [DataTestMethod]
        [DataRow(false, new[] { "PropChgn(VBUnObfusicator.ViewModels.CodeUnObFusViewModel,Reorder)=True\r\nPropChg(VBUnObfusicator.ViewModels.CodeUnObFusViewModel,Reorder)=False\r\n" })]
        [DataRow(true, new[] { "" })]
        public void ReorderTest(bool reorder, string[] asExp)
        {
            Assert.IsNotNull(_testViewModel.Reorder);
            Assert.AreEqual(true, _testViewModel.Reorder);
            _testViewModel.Reorder = reorder;
            Assert.AreEqual(reorder, _testViewModel.Reorder);
            Assert.AreEqual(asExp[0], DebugLog);
        }
        [DataTestMethod]
        [DataRow(false, new[] { "PropChgn(VBUnObfusicator.ViewModels.CodeUnObFusViewModel,RemoveLbl)=True\r\nPropChg(VBUnObfusicator.ViewModels.CodeUnObFusViewModel,RemoveLbl)=False\r\n" })]
        [DataRow(true, new[] { "" })]
        public void RemoveLblTest(bool removeLbl, string[] asExp)
        {
            Assert.IsNotNull(_testViewModel.RemoveLbl);
            Assert.AreEqual(true, _testViewModel.RemoveLbl);
            _testViewModel.RemoveLbl = removeLbl;
            Assert.AreEqual(removeLbl, _testViewModel.RemoveLbl);
            Assert.AreEqual(asExp[0], DebugLog);
        }

        [DataTestMethod]
        [DataRow("0-Empty", new object[] { "", true, true }, new[] { "Please enter some code ...", @"PropChgn(VBUnObfusicator.ViewModels.CodeUnObFusViewModel,Result)=
PropChg(VBUnObfusicator.ViewModels.CodeUnObFusViewModel,Result)=Please enter some code ...
" })]
        [DataRow("1-Code1", new object[] { "Code1", true, false }, new[] { "<ToCodeResult>", @"PropChgn(VBUnObfusicator.ViewModels.CodeUnObFusViewModel,Code)=
PropChg(VBUnObfusicator.ViewModels.CodeUnObFusViewModel,Code)=Code1
PropChgn(VBUnObfusicator.ViewModels.CodeUnObFusViewModel,RemoveLbl)=True
PropChg(VBUnObfusicator.ViewModels.CodeUnObFusViewModel,RemoveLbl)=False
SetProp(OriginalCode,,Code1)
Parse(null)
ReorderLabels(///ParseResult Unknown 0,0
<ParseResult>)
ToCode(///ParseResult Unknown 0,0
<ParseResult>, indent: 4)
PropChgn(VBUnObfusicator.ViewModels.CodeUnObFusViewModel,Result)=
PropChg(VBUnObfusicator.ViewModels.CodeUnObFusViewModel,Result)=<ToCodeResult>
" })]
        [DataRow("2-Code2", new object[] { "Code2", false, true }, new[] { "<ToCodeResult>", @"PropChgn(VBUnObfusicator.ViewModels.CodeUnObFusViewModel,Code)=
PropChg(VBUnObfusicator.ViewModels.CodeUnObFusViewModel,Code)=Code2
PropChgn(VBUnObfusicator.ViewModels.CodeUnObFusViewModel,Reorder)=True
PropChg(VBUnObfusicator.ViewModels.CodeUnObFusViewModel,Reorder)=False
SetProp(OriginalCode,,Code2)
Parse(null)
RemoveSingleSourceLabels1(///ParseResult Unknown 0,0
<ParseResult>)
ToCode(///ParseResult Unknown 0,0
<ParseResult>, indent: 4)
PropChgn(VBUnObfusicator.ViewModels.CodeUnObFusViewModel,Result)=
PropChg(VBUnObfusicator.ViewModels.CodeUnObFusViewModel,Result)=<ToCodeResult>
" })]
        [DataRow("3-Code3", new object[] { "Code3", true, true }, new[] { "<ToCodeResult>", @"PropChgn(VBUnObfusicator.ViewModels.CodeUnObFusViewModel,Code)=
PropChg(VBUnObfusicator.ViewModels.CodeUnObFusViewModel,Code)=Code3
SetProp(OriginalCode,,Code3)
Parse(null)
ReorderLabels(///ParseResult Unknown 0,0
<ParseResult>)
RemoveSingleSourceLabels1(///ParseResult Unknown 0,0
<ParseResult>)
ToCode(///ParseResult Unknown 0,0
<ParseResult>, indent: 4)
PropChgn(VBUnObfusicator.ViewModels.CodeUnObFusViewModel,Result)=
PropChg(VBUnObfusicator.ViewModels.CodeUnObFusViewModel,Result)=<ToCodeResult>
" })]
        [DataRow("4-Code", new object[] { "Code", false, false }, new[] { "<ToCodeResult>", @"PropChgn(VBUnObfusicator.ViewModels.CodeUnObFusViewModel,Code)=
PropChg(VBUnObfusicator.ViewModels.CodeUnObFusViewModel,Code)=Code
PropChgn(VBUnObfusicator.ViewModels.CodeUnObFusViewModel,Reorder)=True
PropChg(VBUnObfusicator.ViewModels.CodeUnObFusViewModel,Reorder)=False
PropChgn(VBUnObfusicator.ViewModels.CodeUnObFusViewModel,RemoveLbl)=True
PropChg(VBUnObfusicator.ViewModels.CodeUnObFusViewModel,RemoveLbl)=False
SetProp(OriginalCode,,Code)
Parse(null)
ToCode(///ParseResult Unknown 0,0
<ParseResult>, indent: 4)
PropChgn(VBUnObfusicator.ViewModels.CodeUnObFusViewModel,Result)=
PropChg(VBUnObfusicator.ViewModels.CodeUnObFusViewModel,Result)=<ToCodeResult>
" })]
        public void ExecuteCommandTest(string _, object[] param, string[] asExp)
        {
            Assert.IsNotNull(_testViewModel.ExecuteCommand);
            Assert.IsInstanceOfType(_testViewModel.ExecuteCommand, typeof(IRelayCommand));
            Assert.IsTrue(_testViewModel.ExecuteCommand.CanExecute(null));
            _testViewModel.Code = param[0]?.ToString() ?? "";
            _testViewModel.Reorder = param[1] is not false and not null;
            _testViewModel.RemoveLbl = param[2] is not false and not null;
            _testViewModel.ExecuteCommand.Execute(null);
            Assert.AreEqual(asExp[0], _testViewModel.Result);
            AssertAreEqual(asExp[1], DebugLog);
        }

        public ICSCode.ICodeBlock Parse(IEnumerable<TokenData>? values = null)
        {
            DoLog($"Parse({values?.ToString() ?? "null"})");
            return _parseResult;
        }

        public void RemoveSingleSourceLabels1(ICSCode.ICodeBlock cStruct)
            => DoLog($"RemoveSingleSourceLabels1({cStruct?.ToString() ?? "null"})");

        public void ReorderLabels(ICSCode.ICodeBlock cStruct)
            => DoLog($"ReorderLabels({cStruct?.ToString() ?? "null"})");

        public string ToCode(ICSCode.ICodeBlock cStruct, int indent = 4)
        {
            DoLog($"ToCode({cStruct?.ToString() ?? "null"}, indent: {indent})");
            return _toCodeResult;
        }

        public void Tokenize(ICSCode.TokenDelegate? token)
            => throw new NotImplementedException();

        public IEnumerable<TokenData> Tokenize()
            => throw new NotImplementedException();
    }
}
