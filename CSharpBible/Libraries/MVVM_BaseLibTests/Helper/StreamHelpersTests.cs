using Microsoft.VisualStudio.TestTools.UnitTesting;
using BaseLib.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace BaseLib.Helper.Tests
{
    [TestClass()]
    public class StreamHelpersTests
    {
        [TestMethod()]
        public void EnumerateToStreamTest()
        {
            // Arrange
            var memoryStream = new MemoryStream();
            var enumerable = new List<(string,object)> { ("Hello",1), ("World",2), ("Test",null!) };
            var expected = "\u0001\0\0\0\u0002\0\0\0";

            // Act
            StreamHelpers.EnumerateToStream(memoryStream,enumerable);
            memoryStream.Position = 0;
            using var reader = new StreamReader(memoryStream);
            var result = reader.ReadToEnd();

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod()]
        public void StreamToEnumerableTest()
        {
            // Arrange
            var input = "\u0001\0\0\0\u0002\0\0\0";
            var types = new List<(string, Type)> { ("Hello", typeof(int)), ("World", typeof(int)), ("Test", typeof(object)) };
            var expected = new List<(string, object)> { ("Hello", 1), ("World", 2) };
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(input));

            // Act
            var result = StreamHelpers.StreamToEnumerable(stream,types).ToList();

            // Assert
            CollectionAssert.AreEqual(expected, result);
        }
    }
}