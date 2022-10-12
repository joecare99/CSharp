using Microsoft.VisualStudio.TestTools.UnitTesting;
using JCAMS.Core.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Xml;

namespace JCAMS.Core.System.Tests
{
    [TestClass()]
    public class CStationTests
    {
        #region Properties
        CStation? TestStation;
        private string DebugResult = "";
        private readonly string cExpSetup= "NewStation: JCAMS.Core.System.CStation; 99\r\n";
        #endregion
        #region Methods
        private void onNewStation(object sender, long e)
        {
            DebugResult +=$"NewStation: {sender}; {e}{Environment.NewLine}";
        }

        #region Tests
        [TestInitialize()]
        public void Init()
        {
            CStation.OnNewStation += onNewStation;
            TestStation = new CStation(99, "TestStation");
        }


        [TestMethod()]
        public void SetupTest()
        {
            Assert.IsNotNull(TestStation);
            Assert.IsInstanceOfType(TestStation,typeof(CStation));
            Assert.AreEqual(cExpSetup,DebugResult);
        }

        [TestMethod()]
        public void ValDefTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void ValDefTest1()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void CStationTest()
        {
            Assert.Fail();
        }

        [DataTestMethod()]
        [DataRow( "AGV", 1,
            new object[] { new object[] { 1, "AGV_Val0", TypeCode.String}, new object[] { 2, "AGV_Val1", TypeCode.Int64 } },
            new object[] { new object[] { 1001L, "AGV01" }, new object[] { 1002L, "AGV02" } },new string[] {
            "<?xml version=\"1.0\"?>\r\n<CStation idStation=\"1\" Description=\"AGV\" ValueDefs.Count=\"2\" SubStations.Count=\"2\">\r\n  <CSystemValueDef Description=\"AGV_Val0\" idStation=\"1\" DataType=\"String\" />\r\n  <CSystemValueDef Description=\"AGV_Val1\" idStation=\"1\" DataType=\"Int64\" />\r\n  <CSubStation idSubStation=\"1001\" Description=\"AGV01\" idStation=\"1\" />\r\n  <CSubStation idSubStation=\"1002\" Description=\"AGV02\" idStation=\"1\" />\r\n</CStation>",
            "NewStation: JCAMS.Core.System.CStation; 99\r\nNewStation: JCAMS.Core.System.CStation; 1\r\n",

        })]
        [DataRow( "Loading", 2,
            new object[] { new object[] { 1001L, "Loading_Val0", TypeCode.Boolean }, 
                new object[] { 1002L, "Loading_Val2", TypeCode.String },
                new object[] { 1003L, "UnLoading_Val1", TypeCode.String },
                new object[] { 1004L, "UnLoading_Val2", TypeCode.String } }, 
            new object[] { new object[] { 1001L, "Feeder01" }, 
                new object[] { 1002L, "Feeder02" },
                new object[] { 1003L, "Feeder03" },
                new object[] { 1004L, "Feeder04" } }, new string[] {
            "<?xml version=\"1.0\"?>\r\n<CStation idStation=\"2\" Description=\"Loading\" ValueDefs.Count=\"4\" SubStations.Count=\"4\">\r\n  <CSystemValueDef Description=\"Loading_Val0\" idStation=\"2\" DataType=\"Boolean\" />\r\n  <CSystemValueDef Description=\"Loading_Val2\" idStation=\"2\" DataType=\"String\" />\r\n  <CSystemValueDef Description=\"UnLoading_Val1\" idStation=\"2\" DataType=\"String\" />\r\n  <CSystemValueDef Description=\"UnLoading_Val2\" idStation=\"2\" DataType=\"String\" />\r\n  <CSubStation idSubStation=\"1001\" Description=\"Feeder01\" idStation=\"2\" />\r\n  <CSubStation idSubStation=\"1002\" Description=\"Feeder02\" idStation=\"2\" />\r\n  <CSubStation idSubStation=\"1003\" Description=\"Feeder03\" idStation=\"2\" />\r\n  <CSubStation idSubStation=\"1004\" Description=\"Feeder04\" idStation=\"2\" />\r\n</CStation>",
            "NewStation: JCAMS.Core.System.CStation; 99\r\nNewStation: JCAMS.Core.System.CStation; 2\r\n"
        })]
        public void GetObjectDataTest( string aDescription, long aId, object[] vDeffs, object[] sStations, string[] ExpFullname)
        {
            var station = new CStation(aId, aDescription);
            foreach (object o in vDeffs)
                if (o is object[] a)
                    station.ValueDefs[a[1] as string] = new Values.CSystemValueDef(a[1] as string,Type.GetType($"System.{a[2] as TypeCode?}"),station);
            foreach (object o in sStations)
                if (o is object[] a)
                    station.SubStations[a[1] as string] = new CSubStation(station, (a[0] is long l ? l : a[0] is int ii ? (long)ii : -1), a[1] as string);
            var formatter = new XmlSerializer(typeof(CStation));
            string actual = "";
            using (var stream = new MemoryStream())
            {
                XmlTextWriter xmlTextWriter = new XmlTextWriter(stream, null);
                xmlTextWriter.Formatting = Formatting.Indented;
                xmlTextWriter.Indentation = 2;    
                formatter.Serialize(xmlTextWriter, station);
                stream.Seek(0L, SeekOrigin.Begin);
                var b = new byte[stream.Length];
                stream.Read(b, 0, b.Length);
                actual = Encoding.UTF8.GetString(b);
            }
            Assert.AreEqual(ExpFullname[0], actual);
            Assert.AreEqual(ExpFullname[1], DebugResult);
        }
        #endregion
        #endregion

    }
}