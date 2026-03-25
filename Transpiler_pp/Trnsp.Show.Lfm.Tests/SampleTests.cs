using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace MSTestv4.Tests;

public interface IMathService
{
    int Add(int a, int b);
}

[TestClass]
public class SampleTests
{
    [TestMethod]
    [DataRow(1, 2, 3)]
    [DataRow(-1, 1, 0)]
    [DataRow(10, 5, 15)]
    public void Add_UsesService(int a, int b, int expected)
    {
        var mathService = Substitute.For<IMathService>();
        mathService.Add(a, b).Returns(expected);

        var result = mathService.Add(a, b);

        Assert.AreEqual(expected, result);
        mathService.Received(1).Add(a, b);
    }
}
