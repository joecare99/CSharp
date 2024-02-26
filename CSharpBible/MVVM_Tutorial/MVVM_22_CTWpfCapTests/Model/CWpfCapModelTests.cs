using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace MVVM_22_CTWpfCap.Model.Tests
{
    [TestClass()]
    public class CWpfCapModelTests
    {
        private CWpfCapModel testWpfCapModel=null!;

        public string DebugOut = "";

        [TestInitialize]
        public void Init()
        {
            var _rnd = new CRandom();
            _rnd.Seed(0);
            testWpfCapModel = new CWpfCapModel(_rnd);
            testWpfCapModel.SetTiles(new int[] { 1, 2, 3, 4,2,3,4,1,3,4,1,2,4,1,2,3 });
            testWpfCapModel.TileColorChanged += TestTileColorChanged;
            DebugOut = "";
        }

        private void TestTileColorChanged(object? sender, EventArgs e)
        {
            DebugOut += $"TileColChanged<{sender}>({e})\r\n";
        }

        [TestMethod()]
        public void SetUpTest()
        {
            Assert.IsNotNull(testWpfCapModel);
            Assert.IsInstanceOfType(testWpfCapModel, typeof(CWpfCapModel));
            Assert.IsInstanceOfType(testWpfCapModel, typeof(IWpfCapModel));
            Assert.AreEqual(4, testWpfCapModel.Width, "testWpfCapModel.Width");
            Assert.AreEqual(4, testWpfCapModel.Height, "testWpfCapModel.Width");
            Assert.AreEqual(false, testWpfCapModel.IsSorted, "testWpfCapModel.IsSorted");
            for (var i = 0; i < 16; i++)
            {
                Assert.AreEqual((i+i/4) % 4 +1, testWpfCapModel.TileColor(i / 4, i % 4), $"testWpfCapModel.TileColor({i})=={(i + i / 4) % 4 + 1}");
            }
            TestTileCount(4);
            Assert.AreEqual("", DebugOut, "DebugOut");
        }
        [TestMethod()]
        public void CWpfCapModelTest()
        {
            var _rnd = new CRandom();
            _rnd.Seed(1);
            var _wcm = new CWpfCapModel(_rnd);
            Assert.IsNotNull(_wcm);
            Assert.IsInstanceOfType(_wcm,typeof(CWpfCapModel));
            Assert.IsInstanceOfType(_wcm,typeof(IWpfCapModel));
            Assert.AreEqual(4,_wcm.Width, "_wcm.Width");
            Assert.AreEqual(4, _wcm.Height, "_wcm.Width");
            Assert.AreEqual(false, _wcm.IsSorted, "_wcm.IsSorted");
            for (var i = 0; i < 16; i++)
            {
                Assert.AreEqual(0, _wcm.TileColor(i/4,i%4), $"_wcm.TileColor({i})==0");
            }
            Assert.ThrowsException<AssertFailedException>(()=>TestTileCount(4, _wcm));
            Assert.AreEqual("", DebugOut, "DebugOut");
        }

        [TestMethod()]
        public void InitTest()
        {
            testWpfCapModel.Init();
            TestTileCount(4);
            Assert.AreEqual(true, testWpfCapModel.IsSorted, "testWpfCapModel.IsSorted");
            for (var i = 0; i < 16; i++)
            {
                Assert.AreEqual(i % 4+1, testWpfCapModel.TileColor(i / 4, i % 4), $"testWpfCapModel.TileColor({i})=={i % 4 +1}");
            }
            Assert.AreEqual("TileColChanged<MVVM_22_CTWpfCap.Model.CWpfCapModel>(System.EventArgs)\r\n", DebugOut, "DebugOut");
        }

        [TestMethod()]
        public void InitTest2()
        {
            testWpfCapModel.TileColorChanged -= TestTileColorChanged;
            testWpfCapModel.Init();
            TestTileCount(4);
            Assert.AreEqual(true, testWpfCapModel.IsSorted, "testWpfCapModel.IsSorted");
            for (var i = 0; i < 16; i++)
            {
                Assert.AreEqual(i % 4 + 1, testWpfCapModel.TileColor(i / 4, i % 4), $"testWpfCapModel.TileColor({i})=={i % 4 + 1}");
            }
            Assert.AreEqual("", DebugOut, "DebugOut");
        }


        [DataTestMethod()]
        [DataRow(-1)]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(3)]
        [DataRow(4)]
        public void MoveLeftTest(int row)
        {
            testWpfCapModel.MoveLeft(row);
            TestTileCount(4);
            for (var i = 0; i < 16; i++)
            {
                Assert.AreEqual(i%4!=row? (i + i / 4) % 4 + 1:(i+ i / 4+1) %4+1 , testWpfCapModel.TileColor(i / 4, i % 4), $"testWpfCapModel.TileColor({i})=={(i + i / 4) % 4 + 1}");
            }
            Assert.AreEqual("TileColChanged<MVVM_22_CTWpfCap.Model.CWpfCapModel>(System.EventArgs)\r\n", DebugOut, "DebugOut");
        }

        [DataTestMethod()]
        [DataRow(-1)]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(3)]
        [DataRow(4)]
        public void MoveLeftTest1(int row)
        {
            testWpfCapModel.TileColorChanged -= TestTileColorChanged;
            testWpfCapModel.MoveLeft(row);
            TestTileCount(4);
            for (var i = 0; i < 16; i++)
            {
                Assert.AreEqual(i % 4 != row ? (i + i / 4) % 4 + 1 : (i + i / 4 + 1) % 4 + 1, testWpfCapModel.TileColor(i / 4, i % 4), $"testWpfCapModel.TileColor({i})=={(i + i / 4) % 4 + 1}");
            }
            Assert.AreEqual("", DebugOut, "DebugOut");
        }

        [DataTestMethod()]
        [DataRow(-1)]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(3)]
        [DataRow(4)]
        public void MoveRightTest(int row)
        {
            testWpfCapModel.MoveRight(row);
            TestTileCount(4);
            for (var i = 0; i < 16; i++)
            {
                Assert.AreEqual(i % 4 != row ? (i + i / 4) % 4 + 1 : (i + i / 4 + 3) % 4 + 1, testWpfCapModel.TileColor(i / 4, i % 4), $"testWpfCapModel.TileColor({i})=={(i + i / 4) % 4 + 1}");
            }
            Assert.AreEqual("TileColChanged<MVVM_22_CTWpfCap.Model.CWpfCapModel>(System.EventArgs)\r\n", DebugOut, "DebugOut");
        }

        [DataTestMethod()]
        [DataRow(-1)]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(3)]
        [DataRow(4)]
        public void MoveRightTest1(int row)
        {
            testWpfCapModel.TileColorChanged -= TestTileColorChanged;
            testWpfCapModel.MoveRight(row);
            TestTileCount(4);
            for (var i = 0; i < 16; i++)
            {
                Assert.AreEqual(i % 4 != row ? (i + i / 4) % 4 + 1 : (i + i / 4 + 3) % 4 + 1, testWpfCapModel.TileColor(i / 4, i % 4), $"testWpfCapModel.TileColor({i})=={(i + i / 4) % 4 + 1}");
            }
            Assert.AreEqual("", DebugOut, "DebugOut");
        }

        [DataTestMethod()]
        [DataRow(-1)]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(3)]
        [DataRow(4)]
        public void MoveUpTest(int col)
        {
            testWpfCapModel.MoveUp(col);
            TestTileCount(4);
            for (var i = 0; i < 16; i++)
            {
                Assert.AreEqual(i / 4 != col ? (i + i / 4) % 4 + 1 : (i + i / 4 + 1) % 4 + 1, testWpfCapModel.TileColor(i / 4, i % 4), $"testWpfCapModel.TileColor({i})=={(i + i / 4) % 4 + 1}");
            }
            Assert.AreEqual("TileColChanged<MVVM_22_CTWpfCap.Model.CWpfCapModel>(System.EventArgs)\r\n", DebugOut, "DebugOut");
        }

        [DataTestMethod()]
        [DataRow(-1)]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(3)]
        [DataRow(4)]
        public void MoveUpTest1(int col)
        {
            testWpfCapModel.TileColorChanged -= TestTileColorChanged;
            testWpfCapModel.MoveUp(col);
            TestTileCount(4);
            for (var i = 0; i < 16; i++)
            {
                Assert.AreEqual(i / 4 != col ? (i + i / 4) % 4 + 1 : (i + i / 4 + 1) % 4 + 1, testWpfCapModel.TileColor(i / 4, i % 4), $"testWpfCapModel.TileColor({i})=={(i + i / 4) % 4 + 1}");
            }
            Assert.AreEqual("", DebugOut, "DebugOut");
        }

        [DataTestMethod()]
        [DataRow(-1)]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(3)]
        [DataRow(4)]
        public void MoveDownTest(int col)
        {
            testWpfCapModel.MoveDown(col);
            TestTileCount(4);
            for (var i = 0; i < 16; i++)
            {
                Assert.AreEqual(i / 4 != col ? (i + i / 4) % 4 + 1 : (i + i / 4 + 3) % 4 + 1, testWpfCapModel.TileColor(i / 4, i % 4), $"testWpfCapModel.TileColor({i})=={(i + i / 4) % 4 + 1}");
            }
            Assert.AreEqual("TileColChanged<MVVM_22_CTWpfCap.Model.CWpfCapModel>(System.EventArgs)\r\n", DebugOut, "DebugOut");
        }

        [DataTestMethod()]
        [DataRow(-1)]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(3)]
        [DataRow(4)]
        public void MoveDownTest1(int col)
        {
            testWpfCapModel.TileColorChanged -= TestTileColorChanged;
            testWpfCapModel.MoveDown(col);
            TestTileCount(4);
            for (var i = 0; i < 16; i++)
            {
                Assert.AreEqual(i / 4 != col ? (i + i / 4) % 4 + 1 : (i + i / 4 + 3) % 4 + 1, testWpfCapModel.TileColor(i / 4, i % 4), $"testWpfCapModel.TileColor({i})=={(i + i / 4) % 4 + 3}");
            }
            Assert.AreEqual("", DebugOut, "DebugOut");
        }


        [TestMethod()]
        public void ShuffleTest()
        {
            testWpfCapModel.Shuffle();
            TestTileCount(4);
            var _x = new int[] { 3, 2, 3, 2, 1, 2, 4, 1, 4, 3, 4, 4, 1, 2, 3, 1 };
            for (var i = 0; i < 16; i++)
                Assert.AreEqual(_x[i], testWpfCapModel.TileColor(i / 4, i % 4), $"testWpfCapModel.TileColor({i})=={_x[i]}");
            Assert.AreEqual("TileColChanged<MVVM_22_CTWpfCap.Model.CWpfCapModel>(System.EventArgs)\r\n", DebugOut, "DebugOut");
        }

        [TestMethod()]
        public void ShuffleTest2()
        {
            testWpfCapModel.TileColorChanged -= TestTileColorChanged;
            testWpfCapModel.Shuffle();
            TestTileCount(4);
            var _x = new int[] { 3, 2, 3, 2, 1, 2, 4, 1, 4, 3, 4, 4, 1, 2, 3, 1 };
            for (var i = 0; i < 16; i++)
                Assert.AreEqual(_x[i], testWpfCapModel.TileColor(i / 4, i % 4), $"testWpfCapModel.TileColor({i})=={_x[i]}");
            Assert.AreEqual("", DebugOut, "DebugOut");
        }

        [TestMethod()]
        public void TileColorTest()
        {
            for (var i = 0; i < 16; i++)
            {
                Assert.AreEqual((i + i / 4) % 4 + 1, testWpfCapModel.TileColor(i / 4, i % 4), $"testWpfCapModel.TileColor({i})=={(i + i / 4) % 4 + 1}");
            }
            testWpfCapModel.Init();
            for (var i = 0; i < 16; i++)
            {
                Assert.AreEqual(i  % 4 + 1, testWpfCapModel.TileColor(i / 4, i % 4), $"testWpfCapModel.TileColor({i})=={i  % 4 + 1}");
            }
            Assert.AreEqual("TileColChanged<MVVM_22_CTWpfCap.Model.CWpfCapModel>(System.EventArgs)\r\n", DebugOut, "DebugOut");
        }

        [TestMethod()]
        public void TileColorTest2()
        {
            for (var i = 0; i < 16; i++)
            {
                Assert.AreEqual((i + i / 4) % 4 + 1, testWpfCapModel.TileColor(i / 4, i % 4), $"testWpfCapModel.TileColor({i})=={(i + i / 4) % 4 + 1}");
            }
            Assert.AreEqual(0, testWpfCapModel.TileColor(-1, 0), ".TileColor(-1, 0)");
            Assert.AreEqual(0, testWpfCapModel.TileColor( 4, 0), ".TileColor( 4, 0)");
            Assert.AreEqual(0, testWpfCapModel.TileColor( 0,-1), ".TileColor( 0,-1)");
            Assert.AreEqual(0, testWpfCapModel.TileColor( 0, 4), ".TileColor( 0, 4)");
            Assert.AreEqual("", DebugOut, "DebugOut");
        }

        private void TestTileCount(int count,CWpfCapModel? wcm=null!)
        {
            var _wcm = wcm ?? testWpfCapModel;
            var _t = new int[count+1];
            for (var i = 0; i< 16; i++)
                _t[_wcm.TileColor(i / 4, i % 4)]++;
            Assert.AreEqual(0, _t[0],"Zero #0");
            for (var i = 1; i <= count; i++)
                Assert.AreEqual(16/count,_t[i], $"{16 / count} #{i}");
        }

    }
}
