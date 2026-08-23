using System;
using System.Collections.Generic;
using System.Drawing;
using ColorVis.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConsoleApp1.Tests;

[TestClass]
public class MathHelpersTests
{
    [TestMethod]
    public void DegToRad_Converts180DegreesToPi()
    {
        var result = MathHelpers.DegToRad(180f);

        Assert.AreEqual((float)Math.PI, result, 1e-5f);
    }

    [TestMethod]
    public void RgbToHsl_RedAndWhite_ReturnExpectedValues()
    {
        MathHelpers.RgbToHsl(Color.Red, out var hRed, out var sRed, out var lRed);
        Assert.AreEqual(0f, hRed, 1e-4f);
        Assert.AreEqual(1f, sRed, 1e-4f);
        Assert.AreEqual(0.5f, lRed, 1e-4f);

        MathHelpers.RgbToHsl(Color.White, out var hWhite, out var sWhite, out var lWhite);
        Assert.AreEqual(0f, hWhite, 1e-4f);
        Assert.AreEqual(0f, sWhite, 1e-4f);
        Assert.AreEqual(1f, lWhite, 1e-4f);
    }

    [TestMethod]
    public void Transform_RotatesVectorAroundYAxis()
    {
        var rotation = MathHelpers.MatrixRotateYawPitch(MathHelpers.DegToRad(90f), 0f);
        var result = MathHelpers.Transform(rotation, new Vector3(1f, 0f, 0f));

        Assert.AreEqual(0f, result.X, 1e-5f);
        Assert.AreEqual(0f, result.Y, 1e-5f);
        Assert.AreEqual(-1f, result.Z, 1e-5f);
    }

    [TestMethod]
    public void Dot_CalculatesTheThreeDimensionalScalarProduct()
    {
        Assert.AreEqual(32f, MathHelpers.Dot(new Vector3(1, 2, 3), new Vector3(4, 5, 6)), 1e-5f);
    }

    [TestMethod]
    public void Multiply_ComposesMatrices()
    {
        var left = new Matrix3x3(1, 2, 3, 0, 1, 4, 5, 6, 0);
        var right = new Matrix3x3(-2, 1, 0, 3, 0, 0, 4, 5, 1);

        Matrix3x3 result = MathHelpers.Multiply(left, right);

        Assert.AreEqual(16f, result.M11);
        Assert.AreEqual(16f, result.M12);
        Assert.AreEqual(3f, result.M13);
        Assert.AreEqual(19f, result.M21);
        Assert.AreEqual(20f, result.M22);
        Assert.AreEqual(4f, result.M23);
        Assert.AreEqual(8f, result.M31);
        Assert.AreEqual(5f, result.M32);
        Assert.AreEqual(0f, result.M33);
    }

    [TestMethod]
    public void RgbToHsl_GrayColor_HasZeroSaturation()
    {
        MathHelpers.RgbToHsl(Color.FromArgb(128, 128, 128), out float h, out float s, out float l);

        Assert.AreEqual(0f, h, 1e-5f);
        Assert.AreEqual(0f, s, 1e-5f);
        Assert.AreEqual(128f / 255f, l, 1e-5f);
    }

    [TestMethod]
    public void RgbNode_Pos3_MapsLightnessToVerticalCoordinate()
    {
        var node = new RgbNode { H = 0f, S = 0f, L = 0.25f };

        Vector3 position = node.Pos3;

        Assert.AreEqual(0.5f, position.X, 1e-5f);
        Assert.AreEqual(0.75f, position.Y, 1e-5f);
        Assert.AreEqual(0.5f, position.Z, 1e-5f);
    }
}

[TestClass]
public class ProgramTests
{
    [TestMethod]
    public void CreateNodes_GeneratesTheExpectedGrid()
    {
        var nodes = Program.CreateNodes(2);

        Assert.AreEqual(8, nodes.Count);
        Assert.AreEqual(0, nodes[0].R);
        Assert.AreEqual(0, nodes[0].G);
        Assert.AreEqual(0, nodes[0].B);
        Assert.AreEqual(Color.FromArgb(64, 64, 64), nodes[0].ColorRgb);
    }

    [TestMethod]
    public void BuildNeighbors_ConnectsOnlyManhattanDistanceOne()
    {
        var nodes = Program.CreateNodes(2);

        Program.BuildNeighbors(nodes);

        CollectionAssert.AreEquivalent(new[] { 1, 2, 4 }, nodes[0].Neighbors);
        CollectionAssert.AreEquivalent(new[] { 0, 3, 5 }, nodes[1].Neighbors);
        CollectionAssert.AreEquivalent(new[] { 6, 5, 3 }, nodes[7].Neighbors);
    }
}
