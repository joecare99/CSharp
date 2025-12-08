using Microsoft.VisualStudio.TestTools.UnitTesting;
using BaseGenClasses.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GenInterfaces.Interfaces.Genealogic;
using BaseGenClasses.Model;
using GenInterfaces.Data;

namespace BaseGenClasses.Helper.Tests
{
    [TestClass()]
    public class GenConverterTests
    {
        private GenConverter<GenDate, IGenDate> genConverter;

        [TestInitialize]
        public void Initialize()
        {
            genConverter = new();
        }

        [TestMethod()]
        [DataRow(0,false)]
        [DataRow(1,true)]
        public void CanConvertTest(int i,bool xExp)
        {
            Assert.AreEqual(xExp, genConverter.CanConvert(i switch {
                0 => typeof(object),
                1 => typeof(IGenDate)
            }));
        }

        [TestMethod()]
        [DataRow("{\"eGenType\":10,\"eDateModifier\":4,\"Date1\":\"1980-05-12T00:00:00\",\"eDateType2\":2,\"Date2\":\"1985-01-01T00:00:00\",\"DateText\":\"ca. 1980-1985\",\"ID\":0}", EDateModifier.About)]
        public void ReadTest(string json, EDateModifier about)
        {
            // Arrange
            var date = new GenDate(
                about,
                EDateType.Full,
                new DateTime(1980, 5, 12),
                EDateType.Year,
                new DateTime(1985, 1, 1),
                "ca. 1980-1985"
            );
            var options = new System.Text.Json.JsonSerializerOptions();
            options.Converters.Add(genConverter);

            var reader = new System.Text.Json.Utf8JsonReader(System.Text.Encoding.UTF8.GetBytes(json));

            // Act
            reader.Read(); // Move to StartObject
            var result = genConverter.Read(ref reader, typeof(IGenDate), options);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(date.eDateModifier, result.eDateModifier);
            Assert.AreEqual(date.eDateType1, result.eDateType1);
            Assert.AreEqual(date.Date1, result.Date1);
            Assert.AreEqual(date.eDateType2, result.eDateType2);
            Assert.AreEqual(date.Date2, result.Date2);
            Assert.AreEqual(date.DateText, result.DateText);
        }

        [TestMethod()]
        [DataRow("{\"eGenType\":10,\"eDateModifier\":4,\"Date1\":\"1980-05-12T00:00:00\",\"eDateType2\":2,\"Date2\":\"1985-01-01T00:00:00\",\"DateText\":\"ca. 1980-1985\"}", EDateModifier.About)]
        public void WriteTest(string sExp, EDateModifier about)
        {
            // Arrange
            var date = new GenDate(
                about,
                EDateType.Full,
                new DateTime(1980, 5, 12),
                EDateType.Year,
                new DateTime(1985, 1, 1),
                "ca. 1980-1985"
            );
            var options = new System.Text.Json.JsonSerializerOptions();

            // Act
            string json = System.Text.Json.JsonSerializer.Serialize<IGenDate>(date, options);

            // Assert
            Assert.IsFalse(string.IsNullOrEmpty(json), "JSON output should not be empty or null.");
             Assert.IsTrue(json.Contains($"\"eDateModifier\":{(int)about}"), "JSON should contain eDateModifier About.");
            Assert.AreEqual(sExp, json);
        }

        [TestMethod()]
        public void WriteNullTest()
        {
            // Arrange
            System.Text.Json.Utf8JsonWriter Null = null;
            IGenDate date = null;
            // Act
            genConverter.Write(Null, date, new System.Text.Json.JsonSerializerOptions());
            // Assert
            Assert.IsTrue(true, "Write should do nothing.");
        }
    }
}