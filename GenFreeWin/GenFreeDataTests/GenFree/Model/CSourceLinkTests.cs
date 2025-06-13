using BaseLib.Interfaces;
using GenFree.Data;
using GenFree.Model;
using GenFree.Interfaces.DB;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        [TestMethod()]
        public void CSourceLinkTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetIndex1FieldTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void SeekTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void ReadAllTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void ExistsTest()
        {
            Assert.Fail();
        }
    }
}