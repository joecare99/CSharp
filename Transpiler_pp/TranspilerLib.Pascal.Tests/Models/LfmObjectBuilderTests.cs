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
                    dir,
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
        Assert.HasCount(1, result.Properties);
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
        Assert.HasCount(1, result.Properties);
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
        Assert.HasCount(2, result.Properties);
        Assert.IsTrue((bool?)result.Properties[0].Value);
        Assert.IsFalse((bool?)result.Properties[1].Value);
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
        Assert.HasCount(1, result.Properties);
        Assert.HasCount(1, result.Children);
        
        var child = result.Children[0];
        Assert.AreEqual("Button1", child.Name);
        Assert.AreEqual("TButton", child.TypeName);
        Assert.HasCount(1, child.Properties);
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
        Assert.HasCount(1, result.Properties);
        Assert.AreEqual(LfmPropertyType.Set, result.Properties[0].PropertyType);
        
        var setItems = result.Properties[0].Value as List<string>;
        Assert.IsNotNull(setItems);
        Assert.HasCount(2, setItems);
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
        Assert.HasCount(1, result.Properties);
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
        Assert.HasCount(1, result.Properties);
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
        Assert.HasCount(1, result.Properties);
        Assert.AreEqual("Font.Name", result.Properties[0].Name);
        Assert.AreEqual("Arial", result.Properties[0].Value);
    }

    [TestMethod]
    public void Build_ObjectWithItemListProperty_ParsesItemListCorrectly()
    {
        // Arrange
        const string input = @"object StatusBar1: TStatusBar
  Panels = <    
    item
      Width = 50
    end    
    item
      Width = 80
    end>
end";
        _tokenizer.SetInput(input);
        var tokens = _tokenizer.Tokenize();

        // Act
        var result = _builder.Build(tokens);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("StatusBar1", result.Name);
        Assert.AreEqual("TStatusBar", result.TypeName);
        Assert.HasCount(1, result.Properties);
        
        var panelsProperty = result.Properties[0];
        Assert.AreEqual("Panels", panelsProperty.Name);
        Assert.AreEqual(LfmPropertyType.ItemList, panelsProperty.PropertyType);
        
        var items = panelsProperty.Value as List<LfmItem>;
        Assert.IsNotNull(items);
        Assert.HasCount(2, items);
        
        // Check first item
        Assert.HasCount(1, items[0].Properties);
        Assert.AreEqual("Width", items[0].Properties[0].Name);
        Assert.AreEqual(50, items[0].Properties[0].Value);
        
        // Check second item
        Assert.HasCount(1, items[1].Properties);
        Assert.AreEqual("Width", items[1].Properties[0].Name);
        Assert.AreEqual(80, items[1].Properties[0].Value);
    }

    [TestMethod]
    [DynamicData(nameof(TestBuildList))]
    public void Build_FromTestResources_BuildsSuccessfully(string testName, string dir)
    {
        var source = File.ReadAllText(Path.Combine(dir, testName + "_Source.lfm"));

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
        var json = JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = false });
        var outputPath = Path.Combine(dir, testName + "_ObjectTree.json");
        File.WriteAllText(outputPath, json);
    }
}
