using Microsoft.VisualStudio.TestTools.UnitTesting;
using MVVM_22_CTWpfCap.Model;
using MVVM_22_CTWpfCap.ViewModels.Factories;
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
        testWpfCapVM = new WpfCapViewModel(_m,_r,_c);

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
    [DataRow(0, 1, "TileColor(1,0)=1\r\n")]
    [DataRow(0, 2, "TileColor(2,0)=2\r\n")]
    [DataRow(0, 3, "TileColor(3,0)=3\r\n")]
    [DataRow(0, 4, "TileColor(4,0)=4\r\n")]
    [DataRow(1, 0, "TileColor(0,1)=8\r\n")]
    [DataRow(1, 1, "TileColor(1,1)=9\r\n")]
    [DataRow(1, 2, "TileColor(2,1)=10\r\n")]
    [DataRow(1, 3, "TileColor(3,1)=11\r\n")]
    [DataRow(1, 4, "TileColor(4,1)=12\r\n")]
    [DataRow(2, 0, "TileColor(0,2)=16\r\n")]
    [DataRow(2, 1, "TileColor(1,2)=17\r\n")]
    [DataRow(2, 2, "TileColor(2,2)=18\r\n")]
    [DataRow(2, 3, "TileColor(3,2)=19\r\n")]
    [DataRow(2, 4, "TileColor(4,2)=20\r\n")]
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
}
