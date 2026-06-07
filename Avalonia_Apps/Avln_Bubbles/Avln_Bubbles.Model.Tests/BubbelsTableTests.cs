using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Avln_Bubbles.Model;

namespace Avln_Bubbles.Model.Tests;

/// <summary>
/// Tests the logical bubble board operations.
/// </summary>
[TestClass]
public sealed class BubbleTableTests
{
    /// <summary>
    /// Verifies that connected cells are returned as a full union.
    /// </summary>
    [TestMethod]
    public void GetUnionReturnsCompleteConnectedRow()
    {
        var model = new BubbleTable(100, 100, 42);
        var data = new int[100];
        Array.Fill(data, 1);

        model.FillRow(data, 99, 100);
        model.FillCell(0, 98, 2);

        for (var i = 0; i < 100; i++)
        {
            Assert.AreEqual(100, model.GetUnion(i, 99).Count);
        }

        Assert.AreEqual(1, model.GetUnion(0, 98).Count);

        for (var i = 0; i < 100; i++)
        {
            for (var j = 0; j < 98; j++)
            {
                Assert.AreEqual(0, model.GetUnion(i, j).Count);
            }
        }
    }
}
