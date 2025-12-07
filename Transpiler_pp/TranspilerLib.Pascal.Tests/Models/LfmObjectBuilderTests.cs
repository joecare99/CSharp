using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace TranspilerLib.Pascal.Models.Tests;

[TestClass]
public class LfmObjectBuilderTests
{
#pragma warning disable CS8618
    private LfmObjectBuilder _builder;
    private LfmTokenizer _tokenizer;
#pragma warning restore CS8618

    public static IEnumerable<object[]> TestBuildList
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
                    File.ReadAllText(Path.Combine(dir, Path.GetFileName(dir) + "_Source.lfm"))
                };
                yield return value;
            }
        }
    }

    [TestInitialize]
    public void Init()
    {
        _builder = new LfmObjectBuilder();
        _tokenizer = new LfmTokenizer();
    }

    [TestMethod]
    public void Setup_tests()
    {
        Assert.IsNotNull(_builder);
        Assert.IsInstanceOfType(_builder, typeof(LfmObjectBuilder));
    }

    [TestMethod]
    public void Build_SimpleObject_ReturnsLfmObject()
    {
        // Arrange
        const string input = @"object Form1: TForm1
  Left = 100
end";
        _tokenizer.SetInput(input);
        var tokens = _tokenizer.Tokenize();

        // Act
        var result = _builder.Build(tokens);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("Form1", result.Name);
        Assert.AreEqual("TForm1", result.TypeName);
        Assert.IsFalse(result.IsInherited);
        Assert.AreEqual(1, result.Properties.Count);
        Assert.AreEqual("Left", result.Properties[0].Name);
        Assert.AreEqual(100, result.Properties[0].Value);
    }

    [TestMethod]
    public void Build_InheritedObject_ReturnsLfmObjectWithIsInheritedTrue()
    {
        // Arrange
        const string input = @"inherited Form1: TForm1
  Caption = 'Test'
end";
        _tokenizer.SetInput(input);
        var tokens = _tokenizer.Tokenize();

        // Act
        var result = _builder.Build(tokens);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsTrue(result.IsInherited);
        Assert.AreEqual("Form1", result.Name);
        Assert.AreEqual("TForm1", result.TypeName);
    }

    [TestMethod]
    public void Build_ObjectWithStringProperty_ParsesStringCorrectly()
    {
        // Arrange
        const string input = @"object Form1: TForm1
  Caption = 'Hello World'
end";
        _tokenizer.SetInput(input);
        var tokens = _tokenizer.Tokenize();

        // Act
        var result = _builder.Build(tokens);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Properties.Count);
        Assert.AreEqual("Caption", result.Properties[0].Name);
        Assert.AreEqual("Hello World", result.Properties[0].Value);
    }

    [TestMethod]
    public void Build_ObjectWithBooleanProperty_ParsesBooleanCorrectly()
    {
        // Arrange
        const string input = @"object Form1: TForm1
  Enabled = True
  Visible = False
end";
        _tokenizer.SetInput(input);
        var tokens = _tokenizer.Tokenize();

        // Act
        var result = _builder.Build(tokens);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Properties.Count);
        Assert.AreEqual(true, result.Properties[0].Value);
        Assert.AreEqual(false, result.Properties[1].Value);
    }

    [TestMethod]
    public void Build_ObjectWithNestedObject_ParsesChildrenCorrectly()
    {
        // Arrange
        const string input = @"object Form1: TForm1
  Left = 100
  object Button1: TButton
    Caption = 'Click'
  end
end";
        _tokenizer.SetInput(input);
        var tokens = _tokenizer.Tokenize();

        // Act
        var result = _builder.Build(tokens);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("Form1", result.Name);
        Assert.AreEqual(1, result.Properties.Count);
        Assert.AreEqual(1, result.Children.Count);
        
        var child = result.Children[0];
        Assert.AreEqual("Button1", child.Name);
        Assert.AreEqual("TButton", child.TypeName);
        Assert.AreEqual(1, child.Properties.Count);
        Assert.AreEqual("Caption", child.Properties[0].Name);
        Assert.AreEqual("Click", child.Properties[0].Value);
    }

    [TestMethod]
    public void Build_ObjectWithSetProperty_ParsesSetCorrectly()
    {
        // Arrange
        const string input = @"object Form1: TForm1
  BorderIcons = [biMinimize, biMaximize]
end";
        _tokenizer.SetInput(input);
        var tokens = _tokenizer.Tokenize();

        // Act
        var result = _builder.Build(tokens);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Properties.Count);
        Assert.AreEqual(LfmPropertyType.Set, result.Properties[0].PropertyType);
        
        var setItems = result.Properties[0].Value as List<string>;
        Assert.IsNotNull(setItems);
        Assert.AreEqual(2, setItems.Count);
        Assert.AreEqual("biMinimize", setItems[0]);
        Assert.AreEqual("biMaximize", setItems[1]);
    }

    [TestMethod]
    public void Build_ObjectWithNegativeNumber_ParsesNegativeCorrectly()
    {
        // Arrange
        const string input = @"object Form1: TForm1
  Min = -1000
end";
        _tokenizer.SetInput(input);
        var tokens = _tokenizer.Tokenize();

        // Act
        var result = _builder.Build(tokens);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Properties.Count);
        Assert.AreEqual("Min", result.Properties[0].Name);
        Assert.AreEqual(-1000, result.Properties[0].Value);
    }

    [TestMethod]
    public void Build_ObjectWithIdentifierProperty_ParsesIdentifierCorrectly()
    {
        // Arrange
        const string input = @"object Form1: TForm1
  BorderStyle = bsToolWindow
end";
        _tokenizer.SetInput(input);
        var tokens = _tokenizer.Tokenize();

        // Act
        var result = _builder.Build(tokens);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Properties.Count);
        Assert.AreEqual("BorderStyle", result.Properties[0].Name);
        Assert.AreEqual("bsToolWindow", result.Properties[0].Value);
    }

    [TestMethod]
    public void Build_ObjectWithQualifiedPropertyName_ParsesQualifiedNameCorrectly()
    {
        // Arrange
        const string input = @"object Form1: TForm1
  Font.Name = 'Arial'
end";
        _tokenizer.SetInput(input);
        var tokens = _tokenizer.Tokenize();

        // Act
        var result = _builder.Build(tokens);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Properties.Count);
        Assert.AreEqual("Font.Name", result.Properties[0].Name);
        Assert.AreEqual("Arial", result.Properties[0].Value);
    }

    [TestMethod]
    [DynamicData(nameof(TestBuildList))]
    public void Build_FromTestResources_BuildsSuccessfully(string testName, string source)
    {
        // Arrange
        _tokenizer.SetInput(source);
        var tokens = _tokenizer.Tokenize();

        // Act
        var result = _builder.Build(tokens);

        // Assert
        Assert.IsNotNull(result, $"Failed to build object tree for {testName}");
        Assert.IsFalse(string.IsNullOrEmpty(result.Name), $"Object name should not be empty for {testName}");
        Assert.IsFalse(string.IsNullOrEmpty(result.TypeName), $"Object type should not be empty for {testName}");

        // Output for debugging
        var json = JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
        var outputPath = Path.Combine(".", "Resources", testName, testName + "_ObjectTree.json");
        File.WriteAllText(outputPath, json);
    }
}
