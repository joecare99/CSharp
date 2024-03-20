using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using BaseLib.Helper;

namespace TraceCsv2realCsv.Model.Tests
{
    [TestClass()]
    public class CsvModelTests
    {
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
        CsvModel testModel;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.

        [TestInitialize]
        public void Init()
        {
            testModel = new CsvModel();
        }

        [DataTestMethod]
        [DataRow(new string[] { "Name,Age,Height", "Alice,25,1.65", "Bob,30,1.80", "Charlie,35,1.75", "David,40,1.70" }, 4)]
        [DataRow(new string[] { "Name,Age,Height", "Alice,25,1.65", "Bob,30,1.80", "Charlie,35,1.75", "David,40,1.70", "Joe,45,1.73", "Morton,50,1.81" }, 6)]
        [DataRow(new string[] { "Name", "Alice", "Bob"}, 2)]
        public void ReadCsvTest(string[] lines, int iCount)
        {
            // Erstellen Sie eine CSV-Datei mit Testdaten.
            var csv = new StringBuilder();
            foreach (var line in lines)
                csv.AppendLine(line);

            // Konvertieren Sie die CSV-Datei in einen Stream.
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(csv.ToString()));

            // Lesen Sie die CSV-Datei und überprüfen Sie die Ergebnisse.           
            testModel.ReadCsv(stream);

            Assert.AreEqual(iCount, testModel.Rows.Count);
            Assert.AreEqual("Alice", testModel.Rows[0]["Name"]);
            if (testModel.Rows.Fields > 1)
            Assert.AreEqual(25, testModel.Rows[0]["Age"]);
            if (testModel.Rows.Fields > 2)
                Assert.AreEqual(1.65, testModel.Rows[0]["Height"]);
        }

        [DataTestMethod]
        [DataRow(new string[] { "Name", "Age", "Height" }, new object[] {
            new object[]{ "Alice",25,1.65d},
            new object[] { "Bob", 30, 1.80d },
            new object[] { "Charlie", 35, 1.75d },
            new object[] { "David", 40, 1.70d } }, new string[] { "\"Name\",\"Age\",\"Height\"\r\n\"Alice\",25,1.65\r\n\"Bob\",30,1.8\r\n\"Charlie\",35,1.75\r\n\"David\",40,1.7\r\n" })]
        [DataRow(new string[] {"ID", "Name", "Age", "Height" }, new object[] {
            new object[]{ 1,"Alice",25,1.65d},
            new object[] { 2, "Bob", 30, 1.80d },
            new object[] { 3, "Charlie", 35, 1.75d },
            new object[] { 4, "David", 40, 1.70d } }, new string[] { "\"ID\",\"Name\",\"Age\",\"Height\"\r\n1,\"Alice\",25,1.65\r\n2,\"Bob\",30,1.8\r\n3,\"Charlie\",35,1.75\r\n4,\"David\",40,1.7\r\n" })]
        public void TestWriteCSV(string[] asHNames, object[] aoData, string[] asExp)
        {
            // Erstellen Sie eine Liste von Zeilen.
            testModel.SetHeader(asHNames.ToList());
            List<object[]> laoData = new();
            foreach (var d in aoData)
                if (d is object[] aoD)
                    laoData.Add(aoD.ToList().ConvertAll((o)=>o is decimal d?(double)d:o).ToArray());
            testModel.AppendData(laoData.ToArray());
 
            // Schreiben Sie die Zeilen in eine CSV-Datei.
            var stream = new MemoryStream();
            testModel.WriteCSV(stream,',');

            // Konvertieren Sie den Stream in eine Zeichenfolge und überprüfen Sie die Ergebnisse.
            var stream2 = new MemoryStream(stream.ToArray()); 
            using var reader = new StreamReader(stream2);
            var result = reader.ReadToEnd();

            Assert.AreEqual(asExp[0], result);
        }


        [DataTestMethod()]
        [DataRow(new string[] { },new TypeCode[] { },new object[] { })]
        [DataRow(new string[] { "Alice" }, new TypeCode[] { TypeCode.String }, new object[] { "Alice" })]
        [DataRow(new string[] { "Bob","30" }, new TypeCode[] { TypeCode.String, TypeCode.Int32 }, new object[] { "Bob",30 })]
        [DataRow(new string[] { "Charlie", "35", "1.75" }, new TypeCode[] { TypeCode.String, TypeCode.Int32, TypeCode.Double }, new object[] { "Charlie", 35, 1.75 })]
        public void CastListTest(string[] asData, TypeCode[] types, object[] aoData)
        {
            // Arrange
            testModel.SetHeader(types.ToList().ConvertAll(t => (t.ToString(), t.ToType())));
            var loExp = aoData.ToList().ConvertAll(t=>t is decimal d? (double)d:t);
            // Act
            var rslt = testModel.CastList(asData.ToList());
            // Assert
            CollectionAssert.AreEqual(loExp,rslt);
        }

        [DataTestMethod]
        [DataRow("Alice", ",", '\"', new string[] { "Alice" })]
        [DataRow("Alice", "", '\"', new string[] { "Alice" })]
        [DataRow(",0,,", ",", '\"', new string[] { "","0","","" })]
        [DataRow(",,\"\",", ",", '\"', new string[] { "", "", "\"\"", "" })]
        [DataRow("\"Alice\"", ",", '\"', new string[] { "\"Alice\"" })]
        [DataRow("\"Alice,2,4.5", ",", '\"', new string[] { "\"Alice,2,4.5" })]
        [DataRow("Alice,25,\"1,65\"", ",", '\"', new string[] { "Alice", "25", "\"1,65\"" })]
        [DataRow("1; 2; 3", ";", '\"', new string[] { "1", " 2", " 3" })]
        [DataRow("1-_-2-_-3-_-", "-_-", '\"', new string[] { "1", "2", "3","" })]
        public void TestSplitCSVLine(string line, string separator, char quotation, string[] asExp)
        {

            var result = CsvModel.SplitCSVLine(line, separator, quotation);

            Assert.AreEqual(asExp.Length, result.Count);
            Assert.AreEqual(asExp[0], result[0], "0");
            if (asExp.Length>1)
            Assert.AreEqual(asExp[1], result[1], "1");
            if (asExp.Length > 2)
                Assert.AreEqual(asExp[2], result[2], "2");
        }

        [DataTestMethod]
        [DataRow(new string[] { "Alice", "Bob", "Charlie" }, TypeCode.String,DisplayName ="Strings")]
        [DataRow(new string[] { "1", "2", "3" }, TypeCode.Int32, DisplayName = "int")]
        [DataRow(new string[] { "1.4", "2", "3.8" }, TypeCode.Double, DisplayName = "floats")]
        [DataRow(new string[] { "1.4", "2", "Hello" }, TypeCode.String, DisplayName = "Wild => String")]
        [DataRow(new string[] { "\"1.4\"", "\"2\"", "\"Hello\"" }, TypeCode.String, DisplayName = "Quoted-Strings")]
        public void TestGetColumnType(string[] asVal, TypeCode tcExp)
        {
            var values = asVal.ToList();

            var result = CsvModel.GetColumnType(values);

            Assert.AreEqual(tcExp, result.TC());
        }
    }
}