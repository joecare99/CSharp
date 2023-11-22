using Microsoft.VisualStudio.TestTools.UnitTesting;

/* Nicht gemergte Änderung aus Projekt "WinAhnenClsTests (net481-windows)"
Vor:
using WinAhnenCls.Model.HejInd;
using System;
using System.Collections.Generic;
Nach:
using System;
using System.Collections.Generic;
using System.IO;
*/

/* Nicht gemergte Änderung aus Projekt "WinAhnenClsTests (net6.0)"
Vor:
using WinAhnenCls.Model.HejInd;
using System;
using System.Collections.Generic;
Nach:
using System;
using System.Collections.Generic;
using System.IO;
*/
using System
/* Nicht gemergte Änderung aus Projekt "WinAhnenClsTests (net481-windows)"
Vor:
using System.Text;
using System.Threading.Tasks;
Nach:
using System.Security.Cryptography;
using System.Text;
*/

/* Nicht gemergte Änderung aus Projekt "WinAhnenClsTests (net6.0)"
Vor:
using System.Text;
using System.Threading.Tasks;
Nach:
using System.Security.Cryptography;
using System.Text;
*/
;
using System.IO;
/* Nicht gemergte Änderung aus Projekt "WinAhnenClsTests (net481-windows)"
Vor:
using WinAhnenCls.Model.HejSource;
using WinAhnenCls.Model.HejPlace;
using WinAhnenCls.Model.HejMarr;
using System.Security.Cryptography;
Nach:
using WinAhnenCls.Model.HejInd;
using WinAhnenCls.Model.HejMarr;
using WinAhnenCls.Model.HejPlace;
using WinAhnenCls.Model.HejSource;
*/

/* Nicht gemergte Änderung aus Projekt "WinAhnenClsTests (net6.0)"
Vor:
using WinAhnenCls.Model.HejSource;
using WinAhnenCls.Model.HejPlace;
using WinAhnenCls.Model.HejMarr;
using System.Security.Cryptography;
Nach:
using WinAhnenCls.Model.HejInd;
using WinAhnenCls.Model.HejMarr;
using WinAhnenCls.Model.HejPlace;
using WinAhnenCls.Model.HejSource;
*/


namespace WinAhnenCls.Model.HejInd.Tests
{
    [TestClass()]
    public class CHejIndiDataTests
    {
        //private CHejInds FClsHejIndividuals;
        private CHejIndiData FHejIndData;
        private string FDataDir;
        private Random rnd;
        const string DefDataDir = "Data";
        /*
        private void CreateTestData(bool Tested = false)
        {
            int
              i, j;

            FClsHejIndividuals.Clear();
            if (Tested)
                Assert.AreEqual(0, FClsHejIndividuals.Count, "No Individuals");
            if (Tested)
                Assert.AreEqual(0, FClsHejIndividuals.Count, "No Individuals");

            FClsHejIndividuals.Append(self);

            if (Tested)
                Assert.AreEqual(1, FClsHejIndividuals.Count, "1 Individual");
            FClsHejIndividuals.ActualInd = cInd[1];
            if (Tested)
                AssertAreEqual(cind[1], FClsHejIndividuals.ActualInd, "Individual[1] match");
            if (Tested)
                AssertAreEqual(cind[1], FClsHejIndividuals.PeekInd[1], "Individual[1] match 2");

            for i := 2 to high(cInd) do
                    begin


                 FClsHejIndividuals.Append(self);

            if (Tested)
                Assert.AreEqual(i, FClsHejIndividuals.Count, inttostr(i) + " Individuals");
            FClsHejIndividuals.ActualInd := cInd[i];

            if (Tested)
                AssertAreEqual(cind[i], FClsHejIndividuals.ActualInd, "Individual[" + inttostr(i) + "] match");
            if (Tested)
                AssertAreEqual(cind[i], FClsHejIndividuals.PeekInd[i], "Individual[" + inttostr(i) + "] match 2");

            if cInd[i].idFather < i then
          FClsHejIndividuals.AppendLinkChild(cInd[i].idFather, i);
            if cInd[i].idMother < i then
          FClsHejIndividuals.AppendLinkChild(cInd[i].idMother, i);

            for j := 1 to FClsHejIndividuals.Count - 1 do
                    if (cInd[j].idFather = i) or(cInd[j].idMother = i) then
             FClsHejIndividuals.AppendLinkChild(i, j);
            end;

            // Append some Marriages
            FClsHejIndividuals.AppendMarriage(1, 0);
            FClsHejIndividuals.AppendMarriage(2, 1);
            for i := 1 to high(cMarr) do
                    FClsHejIndividuals.AppendMarriage(cMarr[i].idPerson, cMarr[i].id);

// Delete an unwanted record
for (i = 1, i < cInd.Length; i++)
                if (cInd[i].id == 0)
                {
                    FClsHejIndividuals.Seek(i);
                    FClsHejIndividuals.Delete(self);
                }
            FClsHejIndividuals.First;
        }
        */
        private void AssertAreEqual(CHejIndiData expected, CHejIndiData actual, string msg, bool ChkID)
        {
            //            EHejIndDataFields lFld;

            foreach (EHejIndDataFields lFld in Enum.GetValues(typeof(EHejIndDataFields)))
                if (lFld > EHejIndDataFields.hind_idMother)
                    Assert.AreEqual(expected.Data[lFld].ToString(), actual.Data[lFld].ToString(), $"{msg} fld:{lFld}");
            if (ChkID)
            {
                Assert.AreEqual((int)expected.Data[EHejIndDataFields.hind_idMother], actual.Data[EHejIndDataFields.hind_idMother], $"{msg} fld:{EHejIndDataFields.hind_idMother}");
                Assert.AreEqual((int)expected.Data[EHejIndDataFields.hind_idFather], actual.Data[EHejIndDataFields.hind_idFather], $"{msg} fld:{EHejIndDataFields.hind_idFather}");
                Assert.AreEqual((int)expected.Data[EHejIndDataFields.hind_ID], actual.Data[EHejIndDataFields.hind_ID], $"{msg} fld:{EHejIndDataFields.hind_ID}");
            }

        }
        [TestInitialize]
        public void Init()
        {
            FHejIndData = new();
            FDataDir = DefDataDir;
            for (var i = 0; i <= 2; i++)
                if (!Directory.Exists(FDataDir))
                    FDataDir = Path.Combine("..", FDataDir);
                else
                    break;
            FDataDir = Path.Combine(FDataDir, "HejTest");
        }

        [TestMethod()]
        public void TestSetUp()
        {
            Assert.IsTrue(Directory.Exists(FDataDir), $"Data-Directory {FDataDir} exists");
            foreach (EHejIndDataFields i in Enum.GetValues(typeof(EHejIndDataFields)))
            {
                if ((int)i <= (int)EHejIndDataFields.hind_idMother)
                    Assert.AreEqual(0, FHejIndData.Data[i], $"Data[{i}] is 0");
                else
                    Assert.AreEqual("", FHejIndData.Data[i], $"Data[{i}] is \"\"");
            }
        }

        [TestMethod()]
        public void CHejIndiDataTest()
        {
        }

        [DataTestMethod()]
        [DataRow(15, 19)]
        [DataRow(1, 2)]
        public void TestID(int iVal1, int iVal2)
        {
            // Test positive
            FHejIndData.ID = iVal1;
            Assert.AreEqual(iVal1, FHejIndData.ID, $"ID is {iVal1}");
            Assert.AreEqual(iVal1, FHejIndData.Data[EHejIndDataFields.hind_ID], $"Data[ID] is {iVal1}");
            FHejIndData.Data[EHejIndDataFields.hind_ID] = iVal2;
            Assert.AreEqual(iVal2, FHejIndData.ID, $"ID is {iVal2}");
            Assert.AreEqual(iVal2, FHejIndData.Data[EHejIndDataFields.hind_ID], $"Data[ID] is {iVal2}");
        }

        [TestMethod()]
        public void TestID2()
        {
            // Test-Schleife
            for (var i = 2; i < 10000; i++)
            {
                int lTestValue = GetRandom(int.MaxValue);
                FHejIndData.ID = lTestValue;
                Assert.AreEqual(lTestValue, FHejIndData.ID, $"ID is {lTestValue} ({i})");
                Assert.AreEqual(lTestValue, FHejIndData.Data[EHejIndDataFields.hind_ID], $"Data[ID] is {lTestValue} ({i})");
                lTestValue = GetRandom(int.MaxValue);
                FHejIndData.Data[EHejIndDataFields.hind_ID] = lTestValue;
                Assert.AreEqual(lTestValue, FHejIndData.ID, $"ID is {lTestValue} ({i})");
                Assert.AreEqual(lTestValue, FHejIndData.Data[EHejIndDataFields.hind_ID], $"Data[ID] is  is {lTestValue} ({i})");
            }
        }
        [DataTestMethod()]
        [DataRow(15, 19)]
        [DataRow(1, 2)]
        public void TestID3(int iVal1, int iVal2)
        {
            // Test negative
            FHejIndData.ID = iVal1;
            FHejIndData.idFather = iVal2;
            Assert.AreEqual(iVal1, FHejIndData.ID, $"ID is {iVal1}");
            FHejIndData.Data[EHejIndDataFields.hind_ID] = iVal2;
            FHejIndData.idMother = iVal1;
            Assert.AreEqual(iVal2, FHejIndData.ID, $"ID is {iVal2}");
        }

        [TestMethod()]
        public void TestID4()
        {
            // Test-Schleife
            for (var i = 2; i < 10000; i++)
            {
                int lTestValue = GetRandom(int.MaxValue);
                int lTestValue2 = GetRandom(int.MaxValue);
                if (lTestValue == lTestValue2)
                    lTestValue2++;
                Assert.AreNotEqual(lTestValue, lTestValue2, "Ungültiger Test Matching TestValues");
                FHejIndData.ID = lTestValue;
                int lSecVal = GetRandom(Enum.GetValues(typeof(EHejIndDataFields)).Length);
                if (lSecVal <= (int)EHejIndDataFields.hind_ID)
                    lSecVal--;
                FHejIndData.Data[(EHejIndDataFields)lSecVal] = lTestValue2;
                Assert.AreEqual(lTestValue, FHejIndData.ID, $"ID is still {lTestValue} ({i})");
                Assert.AreEqual(lTestValue, FHejIndData.Data[EHejIndDataFields.hind_ID], $"Data[ID] is still {lTestValue} ({i})");
            }
        }

        private int GetRandom(int maxValue)
        {
            rnd ??= new Random();
            return rnd.Next(maxValue);
        }
    }
}