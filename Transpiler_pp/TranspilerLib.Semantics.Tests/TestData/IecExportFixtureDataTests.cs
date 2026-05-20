using System;
using System.IO;
using TranspilerLib.IEC.TestData;

namespace TranspilerLib.IEC.TestData.Tests;

/// <summary>
/// Tests export fixture loading helpers for both valid and invalid XML shapes.
/// </summary>
[TestClass]
public class IecExportFixtureDataTests
{
    [TestMethod]
    public void LoadFromPath_Throws_WhenExpectedNodesAreMissing()
    {
        var path = Path.GetTempFileName();
        try
        {
            File.WriteAllText(path, "<root><Single Name=\"Interface\"></Single></root>");

            var exception = CaptureInvalidDataException(() => IecExportFixtureData.LoadFromPath(path));

            StringAssert.Contains(exception.Message, path);
        }
        finally
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }

    private static InvalidDataException CaptureInvalidDataException(Action action)
    {
        try
        {
            action();
        }
        catch (InvalidDataException exception)
        {
            return exception;
        }

        Assert.Fail("Expected InvalidDataException.");
        throw new InvalidOperationException("Unreachable.");
    }
}
