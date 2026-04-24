using Microsoft.VisualStudio.TestTools.UnitTesting;
using MVVM.ViewModel;
using NSubstitute;
using MSQBrowser.Models;
using MSQBrowser.Models.Interfaces;
using MSQBrowser.ViewModels.Interfaces;
using System.Collections.Generic;

namespace MSQBrowser.ViewModels.Tests
{
    [TestClass()]
    public class TableViewViewModelTests : BaseTestViewModel
    {
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
        TableViewViewModel testModel;
        IDBViewViewModel testDBView;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.

        [TestInitialize]
        public void Init()
        {
            testDBView = Substitute.For<IDBViewViewModel>();
            testModel = new TableViewViewModel(testDBView);
            testModel.PropertyChanged += OnVMPropertyChanged;
            testModel.PropertyChanging += OnVMPropertyChanging;
        }

        [TestMethod()]
        public void SchemaViewViewModelTest()
        {
            Assert.IsNotNull(testModel);
            Assert.IsInstanceOfType(testModel, typeof(TableViewViewModel));
            Assert.IsInstanceOfType(testModel, typeof(ITableViewViewModel));
        }

        [TestMethod()]
        [DataRow("Test", true, EKind.Table, nameof(IDBViewViewModel.SelectedEntry), true)]
        [DataRow("Test1", false, EKind.Table, nameof(IDBViewViewModel.SelectedEntry))]
        [DataRow("Test2", true, EKind.Schema, nameof(IDBViewViewModel.SelectedEntry))]
        [DataRow("Test3", true, EKind.Table, nameof(IDBViewViewModel.ToString))]
        public void ParPropChangeTest(string sName, bool xAct, EKind kind, string sProp, bool xExp = false)
        {
            testDBView.SelectedEntry.Returns(xAct ? new DBMetaData("Test", kind, null!, null!) : null);
            testDBView.PropertyChanged += Raise.Event<System.ComponentModel.PropertyChangedEventHandler>(testDBView, new System.ComponentModel.PropertyChangedEventArgs(sProp));
            Assert.AreEqual(xExp ? "Test" : "", testModel.TableName);
            Assert.AreEqual(xExp ? @"PropChgn(MSQBrowser.ViewModels.TableViewViewModel,TableName)=
PropChgn(MSQBrowser.ViewModels.TableViewViewModel,TableData)=
PropChg(MSQBrowser.ViewModels.TableViewViewModel,TableData)=
PropChg(MSQBrowser.ViewModels.TableViewViewModel,TableName)=Test
" : "", DebugLog);
        }

        [DataTestMethod()]
        [DataRow(true, true, true, true)]
        [DataRow(true, true, false, true)]
        [DataRow(false, true, false, false)]
        [DataRow(true, false, false, false)]
        [DataRow(true, false, false, false)]
        public void TableNameTest(bool xAct, bool xAct2,bool xAct3, bool xExp)
        {
            var testModel = xAct ? this.testModel : new TableViewViewModel(null!);
            var tMod = Substitute.For<IDBModel>();
            Assert.AreEqual(xAct ? "" : "<TableName>", testModel.TableName);
            testDBView.dBModel.Returns(xAct2 ? tMod : null);
            tMod.QueryTable("Test5").Returns(xAct3?new List<DBMetaData>() { new DBMetaData("Test5", EKind.Table, null!, null!) } : null);
            testModel.TableName = "Test5";
            Assert.AreEqual("Test5", testModel.TableName);
            tMod.Received(xExp ? 1 : 0).QueryTable("Test5");
            tMod.Received(xExp ? 1 : 0).QueryTableData("Test5");
            tMod.Received( 0).QuerySchema("Test5");
            Assert.AreEqual(!xAct ? "" : @"PropChgn(MSQBrowser.ViewModels.TableViewViewModel,TableName)=
PropChgn(MSQBrowser.ViewModels.TableViewViewModel,TableData)=
PropChg(MSQBrowser.ViewModels.TableViewViewModel,TableData)=
PropChg(MSQBrowser.ViewModels.TableViewViewModel,TableName)=Test5
", DebugLog);
        }
    }
}
