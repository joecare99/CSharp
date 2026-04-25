using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using GenFree.Interfaces.DB;
using NSubstitute;
using GenFree.Interfaces.Sys;
using System.IO;
using BaseLib.Interfaces;
using System;

namespace GenFree.Data.Tests
{
    [TestClass()]
    public class DataModulTests
    {
        [TestMethod()]
        [DataRow(true, "test1")]
        [DataRow(false, "test2")]
        public void OpenNBDatabaseTest(bool xNB, string sName)
        {
            // Arrange
            DataModul.DAODBEngine_definst = Substitute.For<IDBEngine>();
            var oldNB = Substitute.For<IDatabase>();
            if (xNB)
                DataModul.NB = oldNB;
            else
                DataModul.NB = null;

            // Act
            DataModul.OpenNBDatabase(sName);

            // Assert
            if (xNB)
            {
                oldNB.Received(1).Close();
            }
            else
            {
                oldNB.Received(0).Close();
            }
            DataModul.DAODBEngine_definst.Received(1).OpenDatabase(sName, Arg.Any<bool>(), Arg.Any<bool>(), Arg.Any<string>());
            Assert.IsNotNull(DataModul.NB);

            DataModul.NB.Received(4).OpenRecordset(Arg.Any<Enum>(), RecordsetTypeEnum.dbOpenTable);
            DataModul.NB.Received(1).OpenRecordset(dbTables.Ahnen1, RecordsetTypeEnum.dbOpenTable);
            DataModul.NB.Received(1).OpenRecordset(dbTables.Ahnen2, RecordsetTypeEnum.dbOpenTable);
            DataModul.NB.Received(1).OpenRecordset(dbTables.Frauen1, RecordsetTypeEnum.dbOpenTable);
            DataModul.NB.Received(1).OpenRecordset(dbTables.Frauen2, RecordsetTypeEnum.dbOpenTable);

        }

        [TestMethod()]
        [DataRow(true, "test1")]
        [DataRow(false, "test2")]
        public void CreateNewNBDatabaseTest(bool xNB, string sName)
        {
            // Arrange
            DataModul.DAODBEngine_definst = Substitute.For<IDBEngine>();
            var oldNB = Substitute.For<IDatabase>();
            var persistence = Substitute.For<IGenPersistence>();
            persistence.CreateTempFilefromInit(Arg.Any<string>()).Returns(sName);
            if (xNB)
                DataModul.NB = oldNB;
            else
                DataModul.NB = null;

            // Act
            DataModul.CreateNewNBDatabase(persistence);

            // Assert
            if (xNB)
            {
                oldNB.Received(2).Close();
            }
            else
            {
                oldNB.Received(0).Close();
            }
            persistence.Received(1).CreateTempFilefromInit("GedAus.mdb");
            DataModul.DAODBEngine_definst.Received(1).OpenDatabase(sName, Arg.Any<bool>(), Arg.Any<bool>(), Arg.Any<string>());

        }

        [TestMethod()]
        [DataRow(true, "test1", 0)]
        [DataRow(false, "test2", 3)]
        [DataRow(true, "\\t1\\test1", 0)]
        [DataRow(false, "\\t2\\test2", 3)]
        public void PeekMandantTest(bool xRO, string sName, int iExp)
        {
            //Arrange
            DataModul.DAODBEngine_definst = Substitute.For<IDBEngine>();
            List<(string, FileAttributes)> lst = new();

            // Act
            var (i1, i2) = DataModul.PeekMandant(sName, xRO, (s, f) => lst.Add((s, f)));
            // Assert
            Assert.AreEqual(iExp, lst.Count);
            DataModul.DAODBEngine_definst.Received(1).OpenDatabase(sName, v2: false, v3: xRO, Arg.Any<string>());
            Assert.IsNotNull(DataModul.MandDB);
            DataModul.MandDB.Received(2).OpenRecordset(Arg.Any<Enum>(), RecordsetTypeEnum.dbOpenTable);
            DataModul.MandDB.Received(1).OpenRecordset(dbTables.Personen, RecordsetTypeEnum.dbOpenTable);
            DataModul.MandDB.Received(1).OpenRecordset(dbTables.Familie, RecordsetTypeEnum.dbOpenTable);
            Assert.IsNotNull(DataModul.DB_PersonTable);
            DataModul.DB_PersonTable.Received(1).MoveLast();
            Assert.IsNotNull(DataModul.DB_FamilyTable);
            DataModul.DB_PersonTable.Received(1).MoveLast();
        }

        [TestMethod()]
        [DataRow(true)]
        [DataRow(false)]
        public void DataOpenROTest(bool xEx)
        {
            // Arrange
            DataModul.DAODBEngine_definst = Substitute.For<IDBEngine>();
            IDatabase? _om = Substitute.For<IDatabase>(),
                _ot = Substitute.For<IDatabase>(),
                _odo = Substitute.For<IDatabase>(),
                _ods = Substitute.For<IDatabase>();
            if (xEx)
            {
                DataModul.MandDB = _om;
                DataModul.TempDB = _ot;
                DataModul.DOSB = _odo;
                DataModul.DSB = _ods;
            }
            else
            {
                DataModul.MandDB = null;
                DataModul.TempDB = null;
                DataModul.DOSB = null;
                DataModul.DSB = null;
            }
            string sName = "test.mdb";

            // Act
            DataModul.DataOpenRO(sName);

            // Assert
            if (xEx)
            {
                _om.Received(1).Close();
                _ot.Received(1).Close();
                _odo.Received(1).Close();
                _ods.Received(1).Close();
            }
            else
            {
                _om.Received(0).Close();
                _ot.Received(0).Close();
                _odo.Received(0).Close();
                _ods.Received(0).Close();
            }
            DataModul.DAODBEngine_definst.Received(4).OpenDatabase(Arg.Any<string>(), v2: false, v3: true, v4: Arg.Any<string>());
            Assert.IsNotNull(DataModul.MandDB);
            DataModul.MandDB.Received(0).OpenRecordset(Arg.Any<Enum>(), RecordsetTypeEnum.dbOpenTable);
            Assert.IsNotNull(DataModul.TempDB);
            DataModul.TempDB.Received(4).OpenRecordset(Arg.Any<Enum>(), RecordsetTypeEnum.dbOpenTable);
            Assert.IsNotNull(DataModul.DOSB);
            DataModul.DOSB.Received(1).OpenRecordset(Arg.Any<Enum>(), RecordsetTypeEnum.dbOpenTable);
            Assert.IsNotNull(DataModul.DSB);
            DataModul.DSB.Received(1).OpenRecordset(Arg.Any<Enum>(), RecordsetTypeEnum.dbOpenTable);
        }

        [TestMethod()]
        [DataRow("test1", "test2")]
        [DataRow("test3", "test4")]
        public void Person_Clear_EntryRawTest(string sX, string sC)
        {
            // Arrange
            DataModul.DB_PersonTable = Substitute.For<IRecordset>();

            // Act
            DataModul.Person_Clear_EntryRaw(sX, sC);

            // Assert
            DataModul.DB_PersonTable.Received(1).Edit();
            DataModul.DB_PersonTable.Received(1).Update();
            _ = DataModul.DB_PersonTable.Received(7).Fields;
            DataModul.DB_PersonTable.Fields[PersonFields.AnlDatum].Received(1).Value = 0;
            DataModul.DB_PersonTable.Fields[PersonFields.EditDat].Received(1).Value = 0;
            DataModul.DB_PersonTable.Fields[PersonFields.Konv].Received(1).Value = " ";
            DataModul.DB_PersonTable.Fields[PersonFields.religi].Received(1).Value = " ";
            DataModul.DB_PersonTable.Fields[PersonFields.Sex].Received(1).Value = sX;
            DataModul.DB_PersonTable.Fields[PersonFields.Bem1].Received(1).Value = " ";
            DataModul.DB_PersonTable.Fields[PersonFields.Pruefen].Received(1).Value = sC;
        }

        [TestMethod()]
        [DataRow(true, 1, PersonIndex.Such3, "Mustermann", 0)]
        public void Person_DoSearchTest(bool xRev,int iPers, PersonIndex eIdx,string sSuch,int iExp)
        {
            // Arrange
            var personTable = Substitute.For<IRecordset>();
            var searchResult = Substitute.For<IRecordset>();
            DataModul.DB_PersonTable = personTable;
            string searchField = nameof(PersonFields.Such1);
            string searchValue = "Mustermann";
            (personTable.Fields[searchField] as IHasValue).Value.Returns(searchValue);

            // Simuliertes Suchverhalten: Treffer, wenn Nachname "Mustermann"
            personTable.Seek(Arg.Is<string>(s => s == searchField), Arg.Any<object>());

            // Act
            int found = DataModul.Person_DoSearch(PersonIndex.Such3, searchValue,iPers,xRev);

            // Assert
            personTable.Received(0).FindFirst($"{searchField} = '{searchValue}'");
            personTable.ReceivedWithAnyArgs(1).Seek(Arg.Any<string>());
            Assert.AreEqual(iExp,found);
        }

        [TestMethod()]
        public void NB_SperrPers_ExistsTest()
        {
            
        }
    }
}