using BaseLib.Interfaces;
using GenFree.Data;
using GenFree.Interfaces.Data;
using GenFree.Interfaces.DB;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System.Collections.Generic;
using System.Linq;

namespace GenFree.Model.Tests
{
    [TestClass()]
    public class CSourceLinkTests
    {
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
        private CSourceLink testClass;
        private IRecordset testRS;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.

        [TestInitialize]
        public void Init()
        {
            testRS = Substitute.For<IRecordset>();
            testClass = new CSourceLink(() => testRS);
            testRS.NoMatch.Returns(true);
            (testRS.Fields[SourceLinkFields._1] as IHasValue).Value.Returns(1, 2, 5, 16);
            (testRS.Fields[SourceLinkFields._2] as IHasValue).Value.Returns(2,3,4);
            (testRS.Fields[SourceLinkFields._3] as IHasValue).Value.Returns(3,4,5);
            (testRS.Fields[SourceLinkFields._4] as IHasValue).Value.Returns(4,5,6);
            (testRS.Fields[SourceLinkFields.Art] as IHasValue).Value.Returns(7,8,9);
            (testRS.Fields[SourceLinkFields.LfNr] as IHasValue).Value.Returns(8, 9, 10);
            testRS.ClearReceivedCalls();
        }

        [TestMethod]
        [DataRow(1, 2, EEventArt.eA_Birth, 8)]
        [DataRow(2, 3, EEventArt.eA_Death, 9)]
        [DataRow(5, 4, EEventArt.eA_Burial, 10)]
        [DataRow(16, 4, EEventArt.eA_Unknown, 10)]
        public void CSourceLinkTest(int citKenn, int perFamNr, EEventArt art, int lfNr)
        {
            // Arrange
            var rs = testRS;
            var fields = testRS.Fields;

            (fields[SourceLinkFields._1] as IHasValue).Value.Returns(citKenn);
            (fields[SourceLinkFields._2] as IHasValue).Value.Returns(perFamNr);
            (fields[SourceLinkFields._3] as IHasValue).Value.Returns((int)art);
            (fields[SourceLinkFields.LfNr] as IHasValue).Value.Returns(lfNr);


            // Act
            var key = ((short)citKenn, perFamNr, art, (short)lfNr);
            bool xBreak;
            var result = testClass.Seek(key, out xBreak);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(xBreak);
            rs.Received().Seek(Arg.Any<string>(), (short)citKenn, perFamNr, (int)art, (short)lfNr);
        }

        [TestMethod]
        [DataRow(SourceLinkIndex.Tab, SourceLinkFields._1)]
        [DataRow(SourceLinkIndex.Tab21, SourceLinkFields._3)]
        [DataRow(SourceLinkIndex.Tab22, SourceLinkFields._2)]
        [DataRow(SourceLinkIndex.Tab23, SourceLinkFields._4)]
        public void GetIndex1FieldTest(SourceLinkIndex index, SourceLinkFields expectedField)
        {
            // Arrange
            var sut = testClass;

            // Act
            var result = sut.GetIndex1Field(index);

            // Assert
            Assert.AreEqual(expectedField, result);
        }

        [TestMethod]
        [DataRow(1, 2, EEventArt.eA_Birth, 8, true, false)]
        [DataRow(2, 3, EEventArt.eA_Death, 9, false, true)]
        [DataRow(5, 4, EEventArt.eA_Burial, 10, false, true)]
        [DataRow(16, 4, EEventArt.eA_Unknown, 10, true, false)]
        public void SeekTest(int citKenn, int perFamNr, EEventArt art, int lfNr, bool seekResult, bool noMatch)
        {
            // Arrange
            var rs = testRS;
            var fields = rs.Fields;

            (fields[SourceLinkFields._1] as IHasValue).Value.Returns((short)citKenn);
            (fields[SourceLinkFields._2] as IHasValue).Value.Returns(perFamNr);
            (fields[SourceLinkFields._3] as IHasValue).Value.Returns((int)art);
            (fields[SourceLinkFields.LfNr] as IHasValue).Value.Returns((short)lfNr);
            rs.NoMatch.Returns(noMatch);

            var sut = testClass;
            var key = ((short)citKenn, perFamNr, art, (short)lfNr);

            // Act
            bool xBreak;
            var result = sut.Seek(key, out xBreak);

            // Assert
            if (seekResult)
            {
                Assert.IsNotNull(result);
            }
            else
            {
                Assert.IsNull(result);
            }
                Assert.AreEqual(noMatch, xBreak);
            rs.Received().Seek(Arg.Any<string>(), (short)citKenn, perFamNr, (int)art, (short)lfNr);
        }

        [TestMethod]
        [DataRow(1, 2, EEventArt.eA_Birth, 1)]
        [DataRow(2, 3, EEventArt.eA_Death, 0)]
        [DataRow(5, 4, EEventArt.eA_Burial, 2)]
        [DataRow(16, 4, EEventArt.eA_Unknown, 0)]
        public void ReadAllTest(int persInArb, int eventArtInt, EEventArt expectedArt, int expectedCount)
        {
            // Arrange
            var rs = testRS;
            var sut = new CSourceLink(() => rs);

            // Simulierte Daten für das Recordset
            var dataList = new List<ISourceLinkData>();
            for (int i = 0; i < expectedCount; i++)
            {
                var data = Substitute.For<ISourceLinkData>();
                dataList.Add(data);
            }

            // Recordset simulieren: MoveNext/EOF
            int moveCount = 0;
            rs.EOF.Returns(ci => moveCount >= expectedCount);
            rs.When(x => x.MoveNext()).Do(_ => moveCount++);

            // Act
            var result = testClass.ReadAll(persInArb, (EEventArt)eventArtInt).ToList();

            // Assert
            Assert.AreEqual(expectedCount, result.Count);
            foreach (var item in result)
            {
                Assert.IsInstanceOfType(item, typeof(ISourceLinkData));
            }
        }

        [TestMethod]
        [DataRow(1, 2, EEventArt.eA_Birth, 8, true)]
        [DataRow(2, 3, EEventArt.eA_Death, 9, false)]
        [DataRow(5, 4, EEventArt.eA_Burial, 10, true)]
        [DataRow(16, 4, EEventArt.eA_Unknown, 0, false)]
        public void ExistsTest(int citKenn, int perFamNr, EEventArt art, int lfNr, bool expected)
        {
            // Arrange
            var rs = testRS;
            var fields = rs.Fields;

            (fields[SourceLinkFields._1] as IHasValue).Value.Returns(citKenn);
            (fields[SourceLinkFields._2] as IHasValue).Value.Returns(perFamNr);
            (fields[SourceLinkFields._3] as IHasValue).Value.Returns((int)art);
            (fields[SourceLinkFields.LfNr] as IHasValue).Value.Returns(lfNr);

            // Simuliere das Verhalten von Seek und NoMatch
            rs.When(x => x.Seek(Arg.Any<string>(), (short)citKenn, perFamNr, (int)art, (short)lfNr))
              .Do(_ => rs.NoMatch.Returns(!expected));

            // Act
            var result = testClass.Exists(citKenn, perFamNr, art, (short)lfNr);

            // Assert
            Assert.AreEqual(expected, result);
            rs.Received().Seek(Arg.Any<string>(), (short)citKenn, perFamNr, (int)art, (short)lfNr);
        }
    }
}