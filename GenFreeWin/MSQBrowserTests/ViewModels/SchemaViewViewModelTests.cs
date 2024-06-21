using Microsoft.VisualStudio.TestTools.UnitTesting;
using MVVM.ViewModel;
using NSubstitute;
using MSQBrowser.Models;
using MSQBrowser.Models.Interfaces;
using MSQBrowser.ViewModels.Interfaces;

namespace MSQBrowser.ViewModels.Tests
{

    [TestClass()]
    public class SchemaViewViewModelTests : BaseTestViewModel
    {
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
        SchemaViewViewModel testModel;
        IDBViewViewModel testDBView;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.

        [TestInitialize]
        public void Init()
        {
            testDBView = Substitute.For<IDBViewViewModel>();
            testModel = new SchemaViewViewModel(testDBView);
            testModel.PropertyChanged += OnVMPropertyChanged;
            testModel.PropertyChanging += OnVMPropertyChanging;
        }

        [TestMethod()]
        public void SchemaViewViewModelTest()
        {
            Assert.IsNotNull(testModel);
            Assert.IsInstanceOfType(testModel, typeof(SchemaViewViewModel));
            Assert.IsInstanceOfType(testModel, typeof(ISchemaViewViewModel));
        }

        [TestMethod()]
        [DataRow("Test",true,EKind.Schema,nameof(IDBViewViewModel.SelectedEntry),true)]
        [DataRow("Test1",false,EKind.Schema,nameof(IDBViewViewModel.SelectedEntry))]
        [DataRow("Test2",true,EKind.Table,nameof(IDBViewViewModel.SelectedEntry))]
        [DataRow("Test3",true,EKind.Schema,nameof(IDBViewViewModel.ToString))]
        public void ParPropChangeTest(string sName, bool xAct,EKind kind,string sProp,bool xExp=false)
        {
            testDBView.SelectedEntry.Returns(xAct ? new DBMetaData("Test",kind,null!,null!):null);
            testDBView.PropertyChanged += Raise.Event<System.ComponentModel.PropertyChangedEventHandler>(testDBView, new System.ComponentModel.PropertyChangedEventArgs(sProp));
            Assert.AreEqual(xExp?"Test":"", testModel.TableName);
            Assert.AreEqual(xExp? @"PropChgn(MSQBrowser.ViewModels.SchemaViewViewModel,TableName)=
PropChgn(MSQBrowser.ViewModels.SchemaViewViewModel,TableData)=
PropChg(MSQBrowser.ViewModels.SchemaViewViewModel,TableData)=
PropChg(MSQBrowser.ViewModels.SchemaViewViewModel,TableName)=Test
" : "", DebugLog);
        }

        [DataTestMethod()]
        [DataRow(true,true,true)]
        [DataRow(false,true,false)]
        [DataRow(true,false,false)]
        public void TableNameTest(bool xAct,bool xAct2,bool xExp)
        {
            var testModel =xAct? this.testModel :new SchemaViewViewModel(null!);
            var tMod = Substitute.For<IDBModel>();
            Assert.AreEqual(xAct?"":"<TableName>", testModel.TableName);
            testDBView.dBModel.Returns(xAct2 ?tMod:null );
            testModel.TableName = "Test5";
            Assert.AreEqual("Test5", testModel.TableName);
            tMod.Received(xExp?1:0).QuerySchema("Test5");
            Assert.AreEqual(!xAct?"": @"PropChgn(MSQBrowser.ViewModels.SchemaViewViewModel,TableName)=
PropChgn(MSQBrowser.ViewModels.SchemaViewViewModel,TableData)=
PropChg(MSQBrowser.ViewModels.SchemaViewViewModel,TableData)=
PropChg(MSQBrowser.ViewModels.SchemaViewViewModel,TableName)=Test5
", DebugLog);
        }
    }
}
