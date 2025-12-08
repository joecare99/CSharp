using BaseLib.Helper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MVVM_22_CTWpfCap.Model;
using MVVM_22_CTWpfCap.ViewModels.Factories;
using MVVM_22_CTWpfCap.ViewModels.Interfaces;
using NSubstitute;
using NSubstitute.Extensions;
using System;
using System.Collections.Specialized;
using System.ComponentModel;

namespace MVVM_22_CTWpfCap.ViewModels.Tests;

[TestClass()]
public class WpfCapViewModelTests 
{
    private IWpfCapModel _m = null!;
    private IRowDataFactory _r = null!;
    private IColDataFactory _c = null!;
    private WpfCapViewModel testWpfCapVM = null!;
    private WpfCapViewModel testWpfCapVM2 = null!;

    public string DebugOut = "";

    [TestInitialize]
    public void Init()
    {
        _m = Substitute.For<IWpfCapModel>();
        _r = Substitute.For<IRowDataFactory>();
        _c = Substitute.For<IColDataFactory>();
        _m.Height.Returns(4);
        _m.Width.Returns(4);
        _m.When(x => x.Init()).Do(x => DebugOut += "Init()\r\n");
        _m.When(x => x.Shuffle()).Do(x => DebugOut += "Shuffle()\r\n");
        _m.TileColor(Arg.Any<int>(), Arg.Any<int>()).Returns(x =>
        {
            var x1 = x.ArgAt<int>(0);
            var y1 = x.ArgAt<int>(1);
            DebugOut += $"TileColor({x1},{y1})={x1 % 8 + y1 * 8}\r\n";
            return x1 % 8 + y1 * 8;
        });
        IoC.GetReqSrv = t =>
        {
            if (t == typeof(IRowDataFactory)) return _r;
            if (t == typeof(IColDataFactory)) return _c;
            if (t == typeof(IWpfCapModel)) return _m;
            throw new NotImplementedException();
        };

        testWpfCapVM = new WpfCapViewModel();
        
        Assert.AreEqual("Init()\r\nShuffle()\r\n",DebugOut);
        testWpfCapVM.PropertyChanged += vmPropChanged;
        testWpfCapVM.Rows.CollectionChanged += vmColChanged;
        foreach (var row in testWpfCapVM.Rows)
        {
            row.When(x => x.OnPropertyChanged(Arg.Any<string>())).Do(x =>
            {
                var propName = x.ArgAt<string>(0);
                row.PropertyChanged += Raise.Event<PropertyChangedEventHandler>(row, new PropertyChangedEventArgs(propName));
            });
            row.ReturnsForAll("RowData");
            row.PropertyChanged += vmPropChanged;
        }
        testWpfCapVM2 = new WpfCapViewModel(_m,_r,_c);
        Assert.AreEqual("Init()\r\nShuffle()\r\nInit()\r\nShuffle()\r\n", DebugOut);
        DebugOut = "";
    }

    private void vmColChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        DebugOut += $"PropChange({sender},{e.OldStartingIndex},{e.NewStartingIndex})\r\n";
    }

    private void vmPropChanged(object? sender, PropertyChangedEventArgs e)
    {
        DebugOut += $"PropChange({sender},{e.PropertyName})\r\n";
    }

    [TestMethod()]
    public void SetUpTest()
    {
        Assert.IsNotNull(testWpfCapVM);
        Assert.IsNotNull(testWpfCapVM2);
        Assert.AreEqual("", DebugOut);
        Assert.AreEqual(_m, testWpfCapVM.Model);
        Assert.AreEqual(_m, testWpfCapVM2.Model);
    }


    [TestMethod()]
    public void WpfCapViewModelTest()
    {
        var mdl = Substitute.For<IWpfCapModel>();
        var _testWpfCapVM = new WpfCapViewModel(mdl,_r,_c);
        Assert.IsNotNull(_testWpfCapVM);
        mdl.Received().Init();
        mdl.Received().Shuffle();
    }

    [TestMethod()]
    public void ModelTileChangeTest()
    {
        _m.TileColorChanged+=Raise.Event<EventHandler>();
        Assert.AreEqual(String.Format("PropChange({0},TileColor)\r\nPropChange({1},TileColor)\r\nPropChange({2},TileColor)\r\nPropChange({3},TileColor)\r\n", 
            testWpfCapVM.Rows[0].ToString(), 
            testWpfCapVM.Rows[1].ToString(),
            testWpfCapVM.Rows[2].ToString(),
            testWpfCapVM.Rows[3].ToString()), DebugOut);
    }

    [TestMethod()]
    [DataRow(0, "")]
    [DataRow(1, "")]
    [DataRow(2, "")]
    [DataRow(3, "")]
    public void RowDataTest(int row,string sExp2)
    {
        var _rd = testWpfCapVM.Rows[row].TileColor;
        Assert.IsNotNull(_rd);
        Assert.AreEqual(sExp2, DebugOut);
    }

    [TestMethod()]
    [DataRow(0, "")]
    [DataRow(1, "")]
    [DataRow(2, "")]
    [DataRow(3, "")]
    public void RowSelfTest(int row, string sExp2)
    {
        var _rd = testWpfCapVM.Rows[row];
        Assert.IsNotNull(_rd);

        Assert.AreEqual(sExp2, DebugOut);
    }

    [TestMethod()]
    [DataRow(0, 0, "")]
    [DataRow(0, 1, "")]
    [DataRow(0, 2, "")]
    [DataRow(0, 3, "")]
    [DataRow(0, 4, "")]
    [DataRow(1, 0, "")]
    [DataRow(1, 1, "")]
    [DataRow(1, 2, "")]
    [DataRow(1, 3, "")]
    [DataRow(1, 4, "")]
    [DataRow(2, 0, "")]
    [DataRow(2, 1, "")]
    [DataRow(2, 2, "")]
    [DataRow(2, 3, "")]
    [DataRow(2, 4, "")]
    public void ColDataTest(int col,int row, string sExp2)
    {
        var _cd = testWpfCapVM.Cols[col][row];
        Assert.IsNotNull(_cd);
        Assert.AreEqual(sExp2, DebugOut);
    }

    [TestMethod()]
    [DataRow(0, "")]
    [DataRow(1, "")]
    [DataRow(2, "")]
    [DataRow(3, "")]
    public void ColSelfTest(int col, string sExp2)
    {
        var _cd = testWpfCapVM.Cols[col];
        Assert.IsNotNull(_cd);

        Assert.AreEqual(sExp2, DebugOut);
    }

    [TestMethod()]
    public void ShuffleCommandTest()
    {
        testWpfCapVM.ShuffleCommand.Execute(null);
        _m.Received().Shuffle();
    }

    [TestMethod()]
    [DataRow(1)]
    [DataRow(3)]
    public void MoveUpCommandTest(int iAct)
    {
        var r = Substitute.For<IColData>();
        r.ColId.Returns(iAct);
        testWpfCapVM.MoveUpCommand.Execute(r);
        _m.Received(1).MoveUp(iAct-1);
    }

    [TestMethod()]
    [DataRow(1)]
    [DataRow(3)]
    public void MoveDownCommandTest(int iAct)
    {
        var r = Substitute.For<IColData>();
        r.ColId.Returns(iAct);
        testWpfCapVM.MoveDownCommand.Execute(r);
        _m.Received(1).MoveDown(iAct-1);
    }

    [TestMethod()]
    [DataRow(1)]
    [DataRow(3)]
    public void MoveLeftCommandTest(int iAct)
    {
        var r = Substitute.For<IRowData>();
        r.RowId.Returns(iAct);
        testWpfCapVM.MoveLeftCommand.Execute(r);
        _m.Received(1).MoveLeft(iAct-1);
    }

    [TestMethod()]
    [DataRow(1)]
    [DataRow(3)]
    public void MoveRightCommandTest(int iAct)
    {
        var r = Substitute.For<IRowData>();
        r.RowId.Returns(iAct);
        testWpfCapVM.MoveRightCommand.Execute(r);
        _m.Received(1).MoveRight(iAct-1);
    }
}
