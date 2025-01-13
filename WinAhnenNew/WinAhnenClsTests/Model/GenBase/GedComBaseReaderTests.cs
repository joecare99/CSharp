using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.IO;
using System.Text.Json;

namespace WinAhnenCls.Model.GenBase.Tests
{
    [TestClass()]
    public class GedComBaseReaderTests
    {

        [TestMethod()]
        public void GedComBaseReaderTest()
        {
            using var ms = new MemoryStream();
            using var gbr = new GedComBaseReader(ms);
            Assert.IsNotNull(gbr);
            Assert.IsInstanceOfType(gbr, typeof(GedComBaseReader));
            Assert.IsInstanceOfType(gbr, typeof(TextReader));
            Assert.IsInstanceOfType(gbr, typeof(IDisposable));
        }

        [TestMethod()]
        public void CloseTest()
        {
            Assert.Fail();
        }

        [DataTestMethod()]
        [DataRow("Resources\\Muster_GEDCOM_UTF-8.ged", new object[] { new object[] { 0, null!, "HEAD" }, new object[] { 1, null!, "CHAR", "UTF-8" } })]
        [DataRow("Resources\\Care_exp.ged", new object[] { new object[] { 0, null!, "HEAD" }, new object[] { 1, null!, "CHAR", "UTF-8" } })]
        public void ReadLineTest(string sFile, object[] aoExp)
        {
            if (File.Exists(sFile))
            {
                using var fs = new FileStream(sFile, FileMode.Open, FileAccess.Read);
                using var gbr = new GedComBaseReader(fs);
                foreach (var v in aoExp)
                {
                    var sLine = gbr.ReadLine();
                    AssertAreEqual(v, sLine);
                }
            }
            else if (!string.IsNullOrEmpty(sFile))
            {
                Assert.Fail($"File {sFile} not found");
            }
        }

        [DataTestMethod()]
        [DataRow("Resources\\Muster_GEDCOM_UTF-8.ged")]
        [DataRow("Resources\\Care_exp.ged")]
        public void GetGedComLinesTest(string sFile)
        {
            if (File.Exists(sFile))
            {
                if (File.Exists(Path.ChangeExtension(sFile, "0.json")))
                {
                    using var fs = new FileStream(sFile, FileMode.Open, FileAccess.Read);
                    using var gbr = new GedComBaseReader(fs);
                    using var fsIn = new FileStream(Path.ChangeExtension(sFile, "0.json"), FileMode.Open, FileAccess.Read);
                    var aoExp = JsonSerializer.Deserialize<object[][]>(fsIn);
                    var sLines = gbr.GetGedComLines().GetEnumerator();
                    var i = 0;
                    sLines.MoveNext();
                    foreach (var v in aoExp)
                    {
                        var sLine = sLines.Current;
                        AssertAreEqual(v.Select(o => ((JsonElement?)o)?.ValueKind switch
                        {
                            JsonValueKind.Number => ((JsonElement)o).GetInt32(),
                            JsonValueKind.String => ((JsonElement)o).GetString(),
                            _ => (object?)null
                        }).ToArray(), sLine, $"{i++}");
                        sLines.MoveNext();
                    }
                }
                else
                {
                    {
                        using var fs = new FileStream(sFile, FileMode.Open, FileAccess.Read);
                        using var gbr = new GedComBaseReader(fs);
                        using var fsOut = new FileStream(Path.ChangeExtension(sFile, "0.json"), FileMode.Create, FileAccess.Write);
                        JsonSerializer.Serialize<Object[]>(fsOut, gbr.GetGedComLines().Select(o => o.ToOArray()).ToArray());
                    }
                    Assert.Inconclusive("No json file found. Created one.");
                }
            }
            else if (!string.IsNullOrEmpty(sFile))
            {
                Assert.Fail($"File {sFile} not found");
            }
        }


        private void AssertAreEqual(object v, GedComBaseReader.SGedComLine sLine, string msg = "")
        {
            if (v is not object[] _ao || _ao.Length < 3)
                throw new ArgumentException($"{msg}.{nameof(v)}");
            Assert.AreEqual(_ao[0], sLine.iLevel, $"{msg}.{nameof(sLine.iLevel)}");
            Assert.AreEqual(_ao[1], sLine.sID, $"{msg}.{nameof(sLine.sID)}");
            Assert.AreEqual(_ao[2], sLine.sTag, $"{msg}.{nameof(sLine.sTag)}");
            if (_ao.Length >= 4) Assert.AreEqual(_ao[3], sLine.sData, $"{msg}.{nameof(sLine.sData)}"); else Assert.IsNull(sLine.sData, $"{msg}.{nameof(sLine.sData)}");
            if (_ao.Length >= 5) Assert.AreEqual(_ao[4], sLine.sXRef, $"{msg}.{nameof(sLine.sXRef)}"); else Assert.IsNull(sLine.sXRef, $"{msg}.{nameof(sLine.sXRef)}");
            if (_ao.Length >= 6) Assert.AreEqual(_ao[5], sLine.sRest, $"{msg}.{nameof(sLine.sRest)}"); else Assert.IsNull(sLine.sRest, $"{msg}.{nameof(sLine.sRest)}");
        }
    }
}