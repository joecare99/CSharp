using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using WinAhnenCls.Model.HejMarr;
using WinAhnenCls.Model.HejPlace;
using WinAhnenCls.Model.HejSource;

namespace WinAhnenCls.Model.HejInd.Tests
{
    [TestClass()]
    public class CHejIndiDataTests2
    {
        public static readonly CHejIndiData[] cInd = new[] {
            new CHejIndiData() { },
            new CHejIndiData() { },
            new CHejIndiData() { },
            new CHejIndiData() { },
            new CHejIndiData() { },
            new CHejIndiData() { },
            new CHejIndiData() { },
        };

        public static readonly CHejSourceData[] cSource = new[] {
            new CHejSourceData() { },
            new CHejSourceData() { },
            new CHejSourceData() { },
            new CHejSourceData() { },
            new CHejSourceData() { },
            new CHejSourceData() { },
            new CHejSourceData() { },
        };

        public static readonly CHejMarriageData[] cMarr = new[] {
            new CHejMarriageData() { },
            new CHejMarriageData() { },
            new CHejMarriageData() { },
            new CHejMarriageData() { },
            new CHejMarriageData() { },
            new CHejMarriageData() { },
            new CHejMarriageData() { },
        };

        public static readonly CHejPlaceData[] cPlace = new[] {
            new CHejPlaceData() { },
            new CHejPlaceData() { },
            new CHejPlaceData() { },
            new CHejPlaceData() { },
            new CHejPlaceData() { },
            new CHejPlaceData() { },
            new CHejPlaceData() { },
        };

        private string FDataDir;
        private int FDataChCount;
        public int FUpdateCount;

        private void CreateTestData(bool Tested = false)
        {
            CHejGenealogy FHejClass = new CHejGenealogy();
            FHejClass.Clear();
            if (Tested)
            {
                FHejClass.OnStateChange += HejClassOnStateChange;
                FHejClass.OnUpdate += HejClassOnUpdate;
                FHejClass.OnDataChange += HejClassOnDataChange;
                Assert.AreEqual(0, FHejClass.MarriagesCount, "No Marriages");
                Assert.AreEqual(0, FHejClass.IndividualCount, "No Individuals");
            }

            FHejClass.Append();
            if (Tested)
            {
                Assert.AreEqual(1, FHejClass.IndividualCount, "1 Individuals");
                Assert.AreEqual(1, FUpdateCount, "1 Update");
                Assert.AreEqual(1, FHejClass.GetActID, "1 Update");
            }

            FHejClass.ActualInd = cInd[1];
            if (Tested)
                Assert.AreEqual(1, FDataChCount, "1 DataChange");

            FHejClass.AppendSpouse();
            if (Tested)
            {
                Assert.AreEqual(2, FHejClass.MarriagesCount, "2 Marriages");
                Assert.AreEqual(2, FHejClass.IndividualCount, "2 Individuals");
                Assert.AreEqual(2, FHejClass.GetActID, "ID:2");
                Assert.AreEqual(1, FHejClass.ActualInd.Marriages.Length, "length(ActInd.Marriage)");
                Assert.AreEqual(1, FHejClass.PeekInd(1).Marriages.Length, "length(peekInd[0].Marriage)");
                Assert.AreEqual(2, FUpdateCount, "2 Update");
            }

            FHejClass.ActualInd = cInd[2];
            if (Tested)
                Assert.AreEqual(2, FDataChCount, "2 DataChange");

            FHejClass.ActualMarriage = cMarr[0];
            if (Tested)
                Assert.AreEqual(3, FDataChCount, "3 DataChange");

            FHejClass.AppendParent(EHejIndDataFields.hind_idFather);
            if (Tested)
            {
                Assert.AreEqual(3, FHejClass.IndividualCount, "3 Individuals");
                Assert.AreEqual(1, FHejClass.ChildCount, "1 Child");
                Assert.AreEqual(3, FHejClass.GetActID, "ID:3");
                Assert.AreEqual(3, FUpdateCount, "3 Update");
                Assert.AreEqual(3, FHejClass.GetData(2, EHejIndDataFields.hind_idFather), "IDFather:3");
            }

            FHejClass.ActualInd = cInd[3];
            if (Tested)
                Assert.AreEqual(4, FDataChCount, "4 DataChange");

            FHejClass.GotoChild();
            if (Tested)
            {
                Assert.AreEqual(4, FUpdateCount, "4 Update");
                Assert.AreEqual(2, FHejClass.GetActID, "ID:2");
                Assert.AreEqual(3, FHejClass.ActualInd.idFather, "IDFather:3");
            }

            FHejClass.AppendParent(EHejIndDataFields.hind_idMother);
            if (Tested)
            {
                Assert.AreEqual(4, FHejClass.IndividualCount, "4 Individuals");
                Assert.AreEqual(1, FHejClass.ChildCount, "1 Child");
                Assert.AreEqual(4, FHejClass.GetActID, "ID:4");
                Assert.AreEqual(5, FUpdateCount, "5 Update");
                Assert.AreEqual(4, FHejClass.GetData(2, EHejIndDataFields.hind_idMother), "IDMother:4");
            }

            FHejClass.ActualInd = cInd[4];
            if (Tested)
            {
                Assert.AreEqual(5, FDataChCount, "5 DataChange");
            }

            FHejClass.Seek(2);
            if (Tested)
            {
                Assert.AreEqual(2, FHejClass.GetActID, "ID:1");
                Assert.AreEqual(6, FUpdateCount, "6 Update");
            }

            FHejClass.AppendAdoption(3);
            if (Tested)
            {
                Assert.AreEqual(2, FHejClass.GetActID, "ID:1");
                Assert.AreEqual(6, FUpdateCount, "6 Update");
            }

            FHejClass.SetPlace(cPlace[1]);
            FHejClass.SetPlace(cPlace[2]);
            FHejClass.SetPlace(cPlace[3]);
            FHejClass.SetPlace(cPlace[4]);
            FHejClass.SetPlace(cPlace[5]);
            FHejClass.SetPlace(cPlace[6]);
            FHejClass.SetPlace(cPlace[7]);
            if (Tested)
                Assert.AreEqual(7, FHejClass.PlaceCount, "7 Places");

            FHejClass.SetSource(cSource[1]);
            FHejClass.SetSource(cSource[2]);
            FHejClass.SetSource(cSource[3]);
            FHejClass.SetSource(cSource[4]);
            FHejClass.SetSource(cSource[5]);
            FHejClass.SetSource(cSource[6]);
            FHejClass.SetSource(cSource[7]);
            FHejClass.SetSource(cSource[8]);
            FHejClass.SetSource(cSource[9]);
            FHejClass.SetSource(cSource[10]);
            FHejClass.SetSource(cSource[11]);
            FHejClass.SetSource(cSource[12]);
            if (Tested)
                Assert.AreEqual(12, FHejClass.SourceCount, "12 Sources");
        }

        private void HejClassOnUpdate(object? sender, EventArgs e)
        {
            FUpdateCount++;
        }

        private void HejClassOnStateChange(object? sender, EventArgs e)
        {

        }

        private void HejClassOnDataChange(object? sender, EventArgs e)
        {
            FDataChCount++;
        }

        public CHejGenealogy FHejClass { get; set; }

        [TestMethod()]
        public void CHejIndiDataTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void CHejIndiDataTest1()
        {
            FHejClass = new();
            Stream lStr;
            MemoryStream lStl;

            Assert.IsTrue(File.Exists(Path.Combine(FDataDir, "Care.hej")), "Datei existiert");
            using (lStr = new FileStream(Path.Combine(FDataDir, "Care.hej"), FileMode.Open))
                try
                {
                    Assert.AreEqual(1710, lStr.Length, "StreamSize is 1710");
                    FHejClass.ReadFromStream(lStr);
                    Assert.AreEqual(1710, lStr.Position, "Streamposition is 1710");
                }
                finally
                { }

            Assert.AreEqual(4, FHejClass.IndividualCount, "4 Individuals");
            Assert.AreEqual(2, FHejClass.MarriagesCount, "2 Marriages");

            FHejClass.First();
            Assert.AreEqual(1, FHejClass.GetActID, "ID:1");
            Assert.AreEqual(1, FHejClass.SpouseCount, "C(S):1");
            Assert.AreEqual(0, FHejClass.ChildCount, "C(C):0");
            FHejClass.Next();
            Assert.AreEqual(2, FHejClass.GetActID, "ID:2");
            Assert.AreEqual(1, FHejClass.SpouseCount, "C(S):1");
            Assert.AreEqual(0, FHejClass.ChildCount, "C(C):0");
            FHejClass.Next();
            Assert.AreEqual(3, FHejClass.GetActID, "ID:3");
            Assert.AreEqual(0, FHejClass.SpouseCount, "C(S):0");
            Assert.AreEqual(1, FHejClass.ChildCount, "C(C):1");
            FHejClass.Next();
            Assert.AreEqual(4, FHejClass.GetActID, "ID:4");
            Assert.AreEqual(0, FHejClass.SpouseCount, "C(S):0");
            Assert.AreEqual(1, FHejClass.ChildCount, "C(C):1");

            FHejClass.Clear();
            Assert.AreEqual(0, FHejClass.MarriagesCount, "No Marriages");

            Assert.IsTrue(File.Exists(Path.Combine(FDataDir, "BigData5.hej")), "Datei existiert");
            using (lStr = new FileStream(Path.Combine(FDataDir, "BigData5.hej"), FileMode.Open))
            using (lStl = new MemoryStream())
                try
                {
                    Assert.AreEqual(0, lStl.Length, "StreamSize is 11760592");
                    lStr.CopyTo(lStl);
                    Assert.AreEqual(11760592, lStl.Length, "StreamSize is 11760592");
                    lStl.Position = 0;
                    FHejClass.ReadFromStream(lStl);
                    Assert.AreEqual(49302, FHejClass.IndividualCount, "49302 Individuals");
                    Assert.AreEqual(30643, FHejClass.MarriagesCount, "30643 Marriages");
                    Assert.AreEqual(11760592, lStl.Position, "StreamPosition is 11760592");
                }
                finally
                {
                };

        }
    }
}
