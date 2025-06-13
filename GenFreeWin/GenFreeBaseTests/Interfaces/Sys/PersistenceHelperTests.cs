using Microsoft.VisualStudio.TestTools.UnitTesting;
using GenFree.Interfaces.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSubstitute;

namespace GenFree.Interfaces.Sys.Tests;

[TestClass()]
public class PersistenceHelperTests
{
    private IGenPersistence iPersistence;

    [TestInitialize]
    public void Init()
    {
        iPersistence = Substitute.For<IGenPersistence>();
    }
    // Beispiel-Enum für den Test
    public enum TestEnum
    {
        ValueA,
        ValueB
    }
    [TestMethod()]
    [DataRow("Test1", TestEnum.ValueA)]
    [DataRow("Test2", TestEnum.ValueB)]
    public void ReadEnumInitTest(string sTest, TestEnum eExp)
    {
        // Arrange
        iPersistence.ReadIntInit(Arg.Any<string>()).Returns((int)eExp);

        // Act
        var result = iPersistence.ReadEnumInit<TestEnum>(sTest);

        // Assert
        Assert.AreEqual(eExp, result);
        iPersistence.Received(1).ReadIntInit(sTest);
    }


    [TestMethod()]
    [DataRow("Test1", new int[] { 0, 1 }, new TestEnum[] { TestEnum.ValueA, TestEnum.ValueB })]
    [DataRow("Test2", new int[] { 1, 0 }, new TestEnum[] { TestEnum.ValueB, TestEnum.ValueA })]
    public void ReadEnumsMandTest(string sTest, int[] intValues, TestEnum[] expectedEnums)
    {
        // Arrange
        iPersistence.ReadIntsMand(sTest, intValues.Length).Returns(intValues);

        // Act
        var result = new TestEnum[intValues.Length];
        iPersistence.ReadEnumsMand(sTest, result);

        // Assert
        CollectionAssert.AreEqual(expectedEnums, result);
        iPersistence.Received(1).ReadIntsMand(sTest, intValues.Length);
    }

    [TestMethod()]
    [DataRow("Test1", new int[] { 0, 1, 0 }, new TestEnum[] { TestEnum.ValueA, TestEnum.ValueB, TestEnum.ValueA })]
    [DataRow("Test2", new int[] { 1, 1, 0 }, new TestEnum[] { TestEnum.ValueB, TestEnum.ValueB, TestEnum.ValueA })]
    [DataRow("Test3", new int[] { 0 }, new TestEnum[] { TestEnum.ValueA })]
    public void ReadEnumsMandTest1(string sTest, int[] intValues, TestEnum[] expectedEnums)
    {
        // Arrange
        iPersistence.ReadIntsMand(sTest, intValues.Length).Returns(intValues);

        // Act
        IList<TestEnum> result = [];
        for (int i = 0; i < intValues.Length; i++)
        {
            result.Add((TestEnum)2);
        }
        iPersistence.ReadEnumsMand(sTest, result);

        // Assert
        CollectionAssert.AreEqual(expectedEnums, result.ToArray());
        iPersistence.Received(1).ReadIntsMand(sTest, intValues.Length);
    }

    [TestMethod()]
    [DataRow("Test1", new TestEnum[] { TestEnum.ValueA, TestEnum.ValueB }, new int[] { 0, 1 })]
    [DataRow("Test2", new TestEnum[] { TestEnum.ValueB, TestEnum.ValueA }, new int[] { 1, 0 })]
    [DataRow("Test3", new TestEnum[] { TestEnum.ValueA }, new int[] { 0 })]
    public void PutEnumsMandTest(string sTest, TestEnum[] enumsToWrite, int[] expectedInts)
    {
        // Arrange

        // Act
        iPersistence.PutEnumsMand(sTest, enumsToWrite);

        // Assert
        iPersistence.Received(1).PutIntsMand(sTest, Arg.Is<int[]>(arr => arr.SequenceEqual(expectedInts)));
    }

    [TestMethod()]
    [DataRow("Test1", new int[] { 0, 1 }, new TestEnum[] { TestEnum.ValueA, TestEnum.ValueB })]
    [DataRow("Test2", new int[] { 1, 0 }, new TestEnum[] { TestEnum.ValueB, TestEnum.ValueA })]
    [DataRow("Test3", new int[] { 0 }, new TestEnum[] { TestEnum.ValueA })]
    public void ReadEnumsInitTest(string sTest, int[] intValues, TestEnum[] expectedEnums)
    {
        // Arrange
        iPersistence.ReadIntsInit(sTest).Returns(intValues);

        // Act
        var result = iPersistence.ReadEnumsInit<TestEnum>(sTest);

        // Assert
        CollectionAssert.AreEqual(expectedEnums, result.ToArray());
        iPersistence.Received(1).ReadIntsInit(sTest, -1);
    }

    [TestMethod()]
    [DataRow("Test1", new TestEnum[] { TestEnum.ValueB, TestEnum.ValueA }, 2)]
    [DataRow("Test2", new TestEnum[] { TestEnum.ValueA }, 1)]
    [DataRow("Test3", new TestEnum[] { }, 0)]
    public void ReadSuchDatMandTest(string sTest, TestEnum[] expected, int count)
    {
        // Arrange
        iPersistence.ReadIntsMand(sTest, expected.Length).Returns(expected.Select(e => (int)e).ToArray());

        // Act
        IList<TestEnum> result = [];
        for (int i = 0; i < expected.Length; i++)
        {
            result.Add((TestEnum)2);
        }
        iPersistence.ReadSuchDatMand(sTest, result);

        // Assert
        CollectionAssert.AreEqual(expected, result.ToArray());
        iPersistence.Received(1).ReadIntsMand(sTest, expected.Length);
    }

    [TestMethod()]
    [DataRow("Test1", new int[] { 0, 1, 0 }, new TestEnum[] { TestEnum.ValueA, TestEnum.ValueB, TestEnum.ValueA })]
    [DataRow("Test2", new int[] { 1, 1, 0 }, new TestEnum[] { TestEnum.ValueB, TestEnum.ValueB, TestEnum.ValueA })]
    [DataRow("Test3", new int[] { 0 }, new TestEnum[] { TestEnum.ValueA })]
    public void ReadEnumsInitTest1(string sTest, int[] intValues, TestEnum[] expectedEnums)
    {
        // Arrange
        iPersistence.ReadIntsInit(sTest, -1).Returns(intValues);

        // Act
        var result = iPersistence.ReadEnumsInit<TestEnum>(sTest);

        // Assert
        CollectionAssert.AreEqual(expectedEnums, result.ToArray());
        iPersistence.Received(1).ReadIntsInit(sTest, -1);
    }

    [TestMethod()]
    [DataRow("Test1", TestEnum.ValueA)]
    [DataRow("Test2", TestEnum.ValueB)]
    public void ReadEnumInit1Test(string sTest, TestEnum eExp)
    {
        // Arrange
        iPersistence.ReadIntInit(Arg.Any<string>()).Returns((int)eExp);

        // Act
        iPersistence.ReadEnumInit<TestEnum>(sTest, out var result);

        // Assert
        Assert.AreEqual(eExp, result);
        iPersistence.Received(1).ReadIntInit(sTest);
    }
}