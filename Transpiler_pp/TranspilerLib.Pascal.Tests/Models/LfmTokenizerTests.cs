using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TranspilerLib.Pascal.Models.Tests;

[TestClass]
public class LfmTokenizerTests
{
    #pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Fügen Sie ggf. den „erforderlichen“ Modifizierer hinzu, oder deklarieren Sie den Modifizierer als NULL-Werte zulassend.
    private LfmTokenizer testClass;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Fügen Sie ggf. den „erforderlichen“ Modifizierer hinzu, oder deklarieren Sie den Modifizierer als NULL-Werte zulassend.

    public static IEnumerable<object[]> TestTokenizeList
    {
        get
        {
            foreach (var dir in Directory.EnumerateDirectories(".\\Resources"))
            {
                if (!File.Exists(Path.Combine(dir, Path.GetFileName(dir) + "_Source.lfm")))
                    continue;

                var value = new object[]
                {
                    Path.GetFileName(dir)!,
                    dir ,
                };
                yield return value;
            }
        }
    }

    [TestInitialize]
    public void Init()
    {
        testClass = new LfmTokenizer();
    }

    [TestMethod]
    public void Setup_tests()
    {
        // Assert
        Assert.IsNotNull(testClass);
        Assert.IsInstanceOfType(testClass, typeof(LfmTokenizer));
    }

    [TestMethod]
    [DynamicData(nameof(TestTokenizeList))]
    public void Tokenize_tests(string sTestName, string dir)
    {
        var sSource = File.ReadAllText(Path.Combine(dir, Path.GetFileName(dir) + "_Source.lfm"));
        var sExpectedTokens = File.Exists(Path.Combine(dir, Path.GetFileName(dir) + "_Expected.json")) ? File.ReadAllText(Path.Combine(dir, Path.GetFileName(dir) + "_Expected.json")):"";
        // Arrange
        testClass.SetInput(sSource);
        // Act
        var tokens = testClass.Tokenize();
        // Assert
        var sActualTokens = JsonSerializer.Serialize(tokens);
        try
        {
            Assert.AreEqual(sExpectedTokens, sActualTokens, $"Testcase: {sTestName}");
        }
        catch (AssertFailedException)
        {
            if (File.Exists(Path.Combine(".", "Resources", sTestName, sTestName + "_actual.json")))
                File.Delete(Path.Combine(".", "Resources", sTestName, sTestName + "_actual.json"));
            File.WriteAllText(Path.Combine(".", "Resources", sTestName, sTestName + "_actual.json"), sActualTokens);
            throw;
        }

    }
}
