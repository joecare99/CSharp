using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using BaseLib.Interfaces;
using NSubstitute;

namespace BaseLib.Helper.Tests
{
    [TestClass()]
    public class ObjectHelperTests
    {
        [TestMethod()]
        [DataRow(null, -1)]
        [DataRow(1, 1)]
        [DataRow(int.MaxValue, int.MaxValue)]
        [DataRow(int.MinValue, int.MinValue)]
        [DataRow(uint.MaxValue-1, -2)]
        [DataRow(2u, 2)]
        [DataRow("3", 3)]
        [DataRow("4.5", -1)]
        [DataRow(5.1f, 5)]
        [DataRow(6L, 6)]
        [DataRow("DBNull", -1)]
        [DataRow("IHasValue", 0)]
        [DataRow("_", -1)]
        public void AsIntTest(object? obj,int iExp)
        {
            if (obj is string s)
                obj = obj switch
                {
                    "DBNull" => DBNull.Value,
                    "IHasValue" => Substitute.For<IHasValue>(),
                    "_" => new object(),
                    _ => obj
                };   
            // Act
            var result = ObjectHelper.AsInt(obj,-1);
            
            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(iExp, result);

        }

        [TestMethod()]
        [DataRow(null, -1)]
        [DataRow(1, 1)]
        [DataRow(long.MaxValue, long.MaxValue)]
        [DataRow(long.MinValue, long.MinValue)]
        [DataRow(ulong.MaxValue - 1, -2)]
        [DataRow(2u, 2)]
        [DataRow("3", 3)]
        [DataRow("4.5", -1)]
        [DataRow(5.1f, 5)]
        [DataRow(6L, 6)]
        [DataRow("DBNull", -1)]
        [DataRow("IHasValue", 0)]
        [DataRow("_", -1)]
        public void AsLongTest(object? obj, long lExp)
        {
            if (obj is string s)
                obj = obj switch
                {
                    "DBNull" => DBNull.Value,
                    "IHasValue" => Substitute.For<IHasValue>(),
                    "_" => new object(),
                    _ => obj
                };
            // Act
            var result = ObjectHelper.AsLong(obj, -1);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(lExp, result);
        }

        public enum TestEnum
        {
            Default = 0,
            Value1 = 1,
            Value2 = 2
        }

        [TestMethod()]
        [DataRow(null, TestEnum.Default)]
        [DataRow("Value1", TestEnum.Value1)]
        [DataRow("Value2", TestEnum.Value2)]
        [DataRow("InvalidValue", TestEnum.Default)]
        [DataRow(1, TestEnum.Value1)]
        [DataRow(2, TestEnum.Value2)]
        [DataRow(0, TestEnum.Default)]
        public void AsEnumTest(object? obj, TestEnum expected)
        {
            if (obj is string s)
                obj = obj switch
                {
                    "DBNull" => DBNull.Value,
                    "IHasValue" => Substitute.For<IHasValue>(),
                    "_" => new object(),
                    _ => obj
                };
            // Act
            var result = ObjectHelper.AsEnum<TestEnum>(obj);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expected, result);
        }



        [TestMethod()]
        [DataRow(null, "0001-01-01T00:00:00")]
        [DataRow(20231040, "0001-01-01T00:00:00")]
        [DataRow(20230010, "0001-01-01T00:00:00")]
        [DataRow(0, "0001-01-01T00:00:00")]
        [DataRow(20231010, "2023-10-10T00:00:00")]
        [DataRow(1000u, "1902-09-26T00:00:00")]
        [DataRow(1000, "1902-09-26T00:00:00")]
        [DataRow('c', "1999-01-01T00:00:00")]
        [DataRow("10-10-2000", "2000-10-10T00:00:00")]
        [DataRow("2023-10-10", "2023-10-10T00:00:00")]
        [DataRow("10/10/2023", "2023-10-10T00:00:00")]
        [DataRow("10-10-2023", "2023-10-10T00:00:00")]
        [DataRow("2023-10-10T10:10:10", "2023-10-10T10:10:10")]
        [DataRow("InvalidDate", "0001-01-01T00:00:00")]
        [DataRow(638000000000000000L, "2022-09-28T22:13:20")] 
        [DataRow("DBNull", "0001-01-01T00:00:00")]
        [DataRow("IHasValue", "0001-01-01T00:00:00")]
        [DataRow("_", "0001-01-01T00:00:00")]
        public void AsDateTest(object? obj, string expected)
        {
            if (obj is string s)
                obj = s switch
                {
                    "DBNull" => DBNull.Value,
                    "IHasValue" => Substitute.For<IHasValue>(),
                    "_" => new object(),
                    _ when s.Contains("2000") && DateTime.TryParse(s, out var dt) => dt,
                    _ => obj
                };
            // Act
            var result = ObjectHelper.AsDate(obj);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(DateTime.Parse(expected), result);
        }

        [TestMethod()]
        [DataRow(null, 0)]
        [DataRow(1, 1)]
        [DataRow(long.MaxValue, long.MaxValue)]
        [DataRow(long.MinValue, long.MinValue)]
        [DataRow(ulong.MaxValue , ulong.MaxValue)]
        [DataRow(2u, 2)]
        [DataRow("3", 3)]
        [DataRow("Dog", 0)]
        [DataRow("4.5", 4.5)]
        [DataRow(5.1f, 5.1f)]
        [DataRow(double.MinValue, double.MinValue)]
        [DataRow(double.MaxValue, double.MaxValue)]
        [DataRow(double.NaN, double.NaN)]
        [DataRow(6L, 6)]
        [DataRow('7', 48+7)]
        [DataRow("DBNull", 0)]
        [DataRow("IHasValue", 0)]
        [DataRow("_", 0)]
        public void AsDoubleTest(object? obj, double dExp)
        {
            if (obj is string s)
                obj = s switch
                {
                    "DBNull" => DBNull.Value,
                    "IHasValue" => Substitute.For<IHasValue>(),
                    "_" => new object(),
                    _ => obj
                };
            // Act
            var result = ObjectHelper.AsDouble(obj);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(dExp, result);
        }

        [TestMethod()]
        [DataRow(null, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(1, true)]
        [DataRow(0, false)]
        [DataRow("true", true)]
        [DataRow("false", false)]
        [DataRow("True", true)]
        [DataRow("False", false)]
        [DataRow("yes", false)]
        [DataRow("no", false)]
        [DataRow("1", true)]
        [DataRow("0", false)]  
        [DataRow('1', true)]
        [DataRow('T', true)]
        [DataRow('t', true)]
        [DataRow('0', false)]
        [DataRow("DBNull", false)]
        [DataRow("IHasValue", false)]
        [DataRow("_", false)]
        public void AsBoolTest(object? obj, bool bExp)
        {
            if (obj is string s)
                obj = obj switch
                {
                    "DBNull" => DBNull.Value,
                    "IHasValue" => Substitute.For<IHasValue>(),
                    "_" => new object(),
                    _ => obj
                };
            // Act
            var result = ObjectHelper.AsBool(obj);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(bExp, result);
        }


        [TestMethod()]
        [DataRow(null, "00000000-0000-0000-0000-000000000000")]
        [DataRow("d3c4a1b2-3f4e-5d6c-7a8b-9c0d1e2f3a4b", "d3c4a1b2-3f4e-5d6c-7a8b-9c0d1e2f3a4b")]
        [DataRow("invalid-guid", "00000000-0000-0000-0000-000000000000")]
        [DataRow("00000000-0000-0000-0000-000000000000", "00000000-0000-0000-0000-000000000000")]
        [DataRow("IHasValue", "00000000-0000-0000-0000-000000000000")]
        [DataRow("_", "00000000-0000-0000-0000-000000000000")]
        public void AsGUIDTest(object? obj, string expected)
        {
            if (obj is string s)
                obj = obj switch
                {
                    "DBNull" => DBNull.Value,
                    "IHasValue" => Substitute.For<IHasValue>(),
                    "_" => new object(),
                    _ => obj
                };

            // Act
            var result = ObjectHelper.AsGUID(obj);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(Guid.Parse(expected), result);
        }

        [TestMethod()]
        [DataRow(null, false)]
        [DataRow(1, true)]
        [DataRow("test", true)]
        [DataRow("", false)]
        [DataRow(0, false)]
        public void SetRetTest(object? input, bool expected)
        {
            // Arrange
            bool actionCalled = false;
            object? _o = null;
            Action<object?> action = (obj) => { actionCalled = true; _o = obj; };

            // Act
            var result = ObjectHelper.SetRet(input, action, expected);

            // Assert
            Assert.AreEqual(expected, result);
            Assert.AreEqual(input, _o);
            Assert.AreEqual(true, actionCalled);
        }
    }
}