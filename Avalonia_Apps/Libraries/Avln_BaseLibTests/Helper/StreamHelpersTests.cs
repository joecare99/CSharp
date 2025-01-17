using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using BaseLib.Interfaces;
using System.Buffers.Text;
using static BaseLib.Helper.TestHelper;

namespace BaseLib.Helper.Tests;

[TestClass()]
public class StreamHelpersTests
{
    public class PersTestClass : IPersistence
    {
        public int Value1 { get; set; } = 1;
        public int Value2 { get; set; } = 2;
        public PersTestClass() { }

        public IEnumerable<(string, Type)> PropTypes => [(nameof(Value1),typeof(int)), (nameof(Value2), typeof(int))];

        public IEnumerable<(string, object)> EnumerateProp()
        {
            yield return (nameof(Value1),Value1);
            yield return (nameof(Value2), Value2);
        }

        public bool ReadFromEnumerable(IEnumerable<(string, object)> enumerable)
        {
            foreach (var (name, value) in enumerable)
            {
                switch (name)
                {
                    case nameof(Value1):
                        Value1 = (int)value;
                        break;
                    case nameof(Value2):
                        Value2 = (int)value;
                        break;
                    default:
                        return false;
                }
            }
            return true;
        }
    }

    static IEnumerable<object[]> EnumToStreamTestData => [
         [(new List<(string, object)>() { ("Hello", 1), ("World", 2), ("Test", null!) }), "AQAAAAIAAAA="],
         [(new List<(string, object)>() { ("Point", new Point(2,4)), ("Bool", true), ("Byte", (Byte)0x34) }), "AgAAAAQAAAABNA=="],
         [(new List<(string, object)>() { ("IEInt", (new[] { 1, 2, 3, 4, 5 }).ToList()), ("IEP", new[] { new PersTestClass(),new PersTestClass() { Value1 = 3,Value2=9 } }), ("Test", null!) }), "BQABAAAAAgAAAAMAAAAEAAAABQAAAAIAAAABAAAAAgAAAAMAAAAJAAAA"],
    ];

    [TestMethod()]
    [DynamicData(nameof(EnumToStreamTestData))]
    public void EnumerateToStreamTest(IEnumerable<(string, object)> ieAct,string sExp)
    {
        // Arrange
        var memoryStream = new MemoryStream();

        // Act
        StreamHelpers.EnumerateToStream(memoryStream,ieAct);
        var result = Convert.ToBase64String( memoryStream.ToArray());

        // Assert
        Assert.AreEqual(sExp, result);
    }

    [TestMethod()]
    [DynamicData(nameof(EnumToStreamTestData))]

    public void StreamToEnumerableTest(IEnumerable<(string, object)> ieAct, string sExp)
    {
        // Arrange
        var input = Convert.FromBase64String( sExp);
        var types = ieAct.Select(d=>(d.Item1,d.Item2?.GetType() ?? typeof(object)));
        var expected = ieAct.Where(d=>d.Item2!=null).ToList();
        using var stream = new MemoryStream(input);

        // Act
        var result = StreamHelpers.StreamToEnumerable(stream,types).ToList();

        // Assert
        CollectionAssert.AreEqual(expected, result);
    }
}