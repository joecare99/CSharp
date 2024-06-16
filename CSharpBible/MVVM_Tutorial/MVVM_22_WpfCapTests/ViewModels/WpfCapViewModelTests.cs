using Microsoft.VisualStudio.TestTools.UnitTesting;
using MVVM_22_WpfCap.Model;
using System;
using System.Collections.Specialized;
using System.ComponentModel;
using Telerik.JustMock;

namespace MVVM_22_WpfCap.ViewModel.Tests
{
    public class TestWpfCapModel : IWpfCapModel
    {
        public string DebugOut = "";
        public bool sorted;

        public bool IsSorted => sorted;

        public int Width => 4;

        public int Height => 4;

        public event EventHandler? TileColorChanged;

        public void Init()
        {
            DebugOut += $"Init()\r\n";
        }

        public void MoveDown(int column)
        {
            throw new NotImplementedException();
        }

        public void MoveLeft(int row)
        {
            throw new NotImplementedException();
        }

        public void MoveRight(int rpw)
        {
            throw new NotImplementedException();
        }

        public void MoveUp(int column)
        {
            throw new NotImplementedException();
        }

        public void Shuffle()
        {
            DebugOut += $"Shuffle()\r\n";
        }

        public int TileColor(int x, int y)
        {
            DebugOut += $"TileColor({x},{y})={x % 8 + y * 8}\r\n";
            return x%8+y*8;
        }

        internal void FireTileChange()
        {
            TileColorChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    [TestClass()]
    public class WpfCapViewModelTests
    {
        private TestWpfCapModel _m = null!;
        private WpfCapViewModel testWpfCapVM = null!;
        private WpfCapViewModel testWpfCapVM2 = null!;

        public string DebugOut = "";

        [TestInitialize]
        public void Init()
        {
            _m = new TestWpfCapModel();
            testWpfCapVM = new WpfCapViewModel(_m);
            Assert.AreEqual("Init()\r\nShuffle()\r\n", _m.DebugOut);
            testWpfCapVM.PropertyChanged += vmPropChanged;
            testWpfCapVM.Rows.CollectionChanged += vmColChanged;
            foreach (var row in testWpfCapVM.Rows)
                row.PropertyChanged += vmPropChanged;
            testWpfCapVM2 = new WpfCapViewModel(_m);
            Assert.AreEqual("Init()\r\nShuffle()\r\nInit()\r\nShuffle()\r\n", _m.DebugOut);
            DebugOut = "";
            _m.DebugOut = "";
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
            Assert.AreEqual("",_m.DebugOut);
            Assert.AreEqual(_m, testWpfCapVM.Model);
            Assert.AreEqual(_m, testWpfCapVM2.Model);
        }


        [TestMethod()]
        public void WpfCapViewModelTest()
        {
            var mdl = Mock.Create<IWpfCapModel>();
            var _testWpfCapVM = new WpfCapViewModel(mdl);
            Assert.IsNotNull(_testWpfCapVM);
            Mock.Assert(mdl);
            Mock.Assert(() => mdl.Init());
            Mock.Assert(() => mdl.Shuffle());
        }

        [TestMethod()]
        public void ModelTileChangeTest()
        {
            _m.FireTileChange();
            Assert.AreEqual("", _m.DebugOut);
            Assert.AreEqual("PropChange(MVVM_22_WpfCap.ViewModel.RowData,TileColor)\r\nPropChange(MVVM_22_WpfCap.ViewModel.RowData,TileColor)\r\nPropChange(MVVM_22_WpfCap.ViewModel.RowData,TileColor)\r\nPropChange(MVVM_22_WpfCap.ViewModel.RowData,TileColor)\r\n", DebugOut);
        }

        [DataTestMethod()]
        [DataRow(0, "TileColor(0,0)=0\r\nTileColor(1,0)=1\r\nTileColor(2,0)=2\r\nTileColor(3,0)=3\r\n")]
        [DataRow(1, "TileColor(0,1)=8\r\nTileColor(1,1)=9\r\nTileColor(2,1)=10\r\nTileColor(3,1)=11\r\n")]
        [DataRow(2, "TileColor(0,2)=16\r\nTileColor(1,2)=17\r\nTileColor(2,2)=18\r\nTileColor(3,2)=19\r\n")]
        [DataRow(3, "TileColor(0,3)=24\r\nTileColor(1,3)=25\r\nTileColor(2,3)=26\r\nTileColor(3,3)=27\r\n")]
        public void RowDataTest(int row,string sExp2)
        {
            var _rd = testWpfCapVM.Rows[row].TileColor;
            Assert.IsNotNull(_rd);
            Assert.AreEqual(4,_rd.Length);

            for (var i =0; i<_rd.Length;i++)
                Assert.AreEqual(i+row*8, _rd[i]);
            Assert.AreEqual("", DebugOut);
            Assert.AreEqual(sExp2, _m.DebugOut);
        }

        [DataTestMethod()]
        [DataRow(0, "")]
        [DataRow(1, "")]
        [DataRow(2, "")]
        [DataRow(3, "")]
        public void RowSelfTest(int row, string sExp2)
        {
            var _rd = testWpfCapVM.Rows[row];
            Assert.IsNotNull(_rd);
            Assert.AreEqual(_rd, _rd.This);

            Assert.AreEqual("", DebugOut);
            Assert.AreEqual(sExp2, _m.DebugOut);
        }

        [DataTestMethod()]
        [DataRow(0, 0, "TileColor(0,0)=0\r\n")]
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
            Assert.AreEqual(col *8 + row, _cd);
            Assert.AreEqual("", DebugOut);
            Assert.AreEqual(sExp2, _m.DebugOut);
        }

        [DataTestMethod()]
        [DataRow(0, "")]
        [DataRow(1, "")]
        [DataRow(2, "")]
        [DataRow(3, "")]
        public void ColSelfTest(int col, string sExp2)
        {
            var _cd = testWpfCapVM.Cols[col];
            Assert.IsNotNull(_cd);
            Assert.AreEqual(_cd, _cd.This);

            Assert.AreEqual("", DebugOut);
            Assert.AreEqual(sExp2, _m.DebugOut);
        }
    }
}