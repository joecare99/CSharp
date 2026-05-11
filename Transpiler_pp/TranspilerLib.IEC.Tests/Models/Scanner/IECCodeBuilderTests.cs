using System;
using System.Collections.Generic;
using TranspilerLib.Interfaces.Code;
using TranspilerLib.Data;
using BaseLib.Helper;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using static TranspilerLib.Helper.TestHelper;
using TranspilerLib.IEC.Models.Ast;
using TranspilerLib.IEC.TestData;
using TranspilerLib.IEC.Models.Scanner;
using TranspilerLib.Models.Scanner;

#pragma warning disable IDE0130 // Der Namespace entspricht stimmt nicht der Ordnerstruktur.
namespace TranspilerLib.IEC.Models.Scanner.Tests;
#pragma warning restore IDE0130 // Der Namespace entspricht stimmt nicht der Ordnerstruktur.

[TestClass()]
public class IECCodeBuilderTests
{
    private const string TokensFilePath = ".\\Resources\\IECCodeBuilder\\Test{0}\\Tokens{0}.json";
    private const string BlocksFilePath = ".\\Resources\\IECCodeBuilder\\Test{0}\\Blocks{0}.json";
    private const string BlocksTextPath = ".\\Resources\\IECCodeBuilder\\Test{0}\\Blocks{0}.txt";

    public static IEnumerable<object?[]> OnIECTokenTestData => [
          ["0" , new List<TokenData>() { new("-", CodeBlockType.Operation, 1, 1), new("2.0", CodeBlockType.Number, 1, 3) }, GetBuildCommands("00")],
          ["1" , GetTokenlist("01"), GetBuildCommands("01") ],
          ["2" , new List<TokenData>() { new("1.0", CodeBlockType.Number, 0, 1), new("+", CodeBlockType.Operation, 0, 5), new("2.0", CodeBlockType.Number, 0, 7), new("*", CodeBlockType.Operation, 0, 5), new("3.0", CodeBlockType.Number, 0, 7) }, GetBuildCommands("02") ],
          new object?[] { "3", new List<TokenData>(), new[] { "///Declaration Unknown 0,0", "" } },
          ["4" , GetTokenlist("04"), GetBuildCommands("04") ],
          ["5" , GetTokenlist("05"), GetBuildCommands("05") ],
          new object?[] { "6", new List<TokenData>(), new[] { "///Declaration Unknown 0,0", "" } },
          new object?[] { "7", new List<TokenData>(), new[] { "///Declaration Unknown 0,0", "" } },
        ];

    private static string[]? GetBuildCommands(string sFNStumb)
    {
        var sFile = BlocksTextPath.Format(sFNStumb.PadLeft(2, '0'));
        if (File.Exists(sFile))
            try
            {
                return File.ReadAllText(sFile).Split(Environment.NewLine);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        else
        {
            return null;
        }
    }

    private static object? GetTokenlist(string sFNStumb)
    {
        var sFile = TokensFilePath.Format(sFNStumb.PadLeft(2, '0'));
        if (File.Exists(sFile))
        {
            return IECTestDataClass.ReadFileObject(sFile);
        }
        else
        {
            return null;
        }
    }

#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Fügen Sie ggf. den „erforderlichen“ Modifizierer hinzu, oder deklarieren Sie den Modifizierer als NULL-Werte zulassend.
    private IECCodeBuilder testClass;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Fügen Sie ggf. den „erforderlichen“ Modifizierer hinzu, oder deklarieren Sie den Modifizierer als NULL-Werte zulassend.

    private static string NormalizeCodeSequence(string code)
    {
        var sb = new StringBuilder(code.Length);
        foreach (var ch in code)
        {
            if (ch != ' ' && ch != '\t' && ch != '\r' && ch != '\n')
            {
                sb.Append(ch);
            }
        }

        return sb.ToString();
    }

    private ICodeBlock BuildCodeBlockTree(IEnumerable<TokenData> tokens)
    {
        ICodeBlock root = new CodeBlock() { Name = "Declaration", Code = "", Parent = null, SourcePos = -1 };
        var data = testClass.NewData(root);
        foreach (var token in tokens)
        {
            testClass.OnToken(token, data);
        }

        return root;
    }

    private static ICodeBlock? FindDeclarationByLeftCode(ICodeBlock block, string leftCode)
    {
        if (block.Type == CodeBlockType.Declaration
            && block.SubBlocks.Count > 0
            && string.Equals(block.SubBlocks[0].Code, leftCode, StringComparison.Ordinal))
        {
            return block;
        }

        foreach (var subBlock in block.SubBlocks)
        {
            var found = FindDeclarationByLeftCode(subBlock, leftCode);
            if (found != null)
            {
                return found;
            }
        }

        return null;
    }

    [TestInitialize]
    public void Init() { 
        testClass = new IECCodeBuilder();
    }

    [TestMethod()]
    public void IECCodeBuilderTest()
    {
        Assert.IsNotNull(testClass);
        Assert.IsInstanceOfType(testClass, typeof(ICodeBuilder));
        Assert.IsInstanceOfType(testClass, typeof(IECCodeBuilder));
        var data = GetTokenlist("01");
        Assert.IsNotNull(data);
        data = GetTokenlist("2");
    }

    [TestMethod()]
    [DynamicData(nameof(OnIECTokenTestData))]
    public void OnTokenTest(string sFNStumb, List<TokenData> tokens, string[] codes)
    {
        // Arrange
        ICodeBlock codeBlock = new CodeBlock() { Name = "Declaration", Code = "", Parent = null,SourcePos=-1 };
        var data = testClass.NewData(codeBlock);

        // Act
        if (tokens != null)
        foreach (var token in tokens)
        {
            testClass.OnToken(token, data);
        }

        var sFile = BlocksFilePath.Format(sFNStumb.PadLeft(2, '0'))+"_";
        var directoryPath = Path.GetDirectoryName(sFile);
        Assert.IsNotNull(directoryPath);
        Directory.CreateDirectory(directoryPath);
        using var ms = new FileStream(sFile, FileMode.Create);
        new DataContractJsonSerializer(
            type: typeof(List<ICodeBlock>),
            knownTypes: [typeof(IECCodeBlock)])
        .WriteObject(ms, codeBlock.SubBlocks);
        ms.Close();

        sFile = BlocksTextPath.Format(sFNStumb.PadLeft(2, '0')) + "_";
        using var fs2 = new FileStream(sFile, FileMode.Create);
        using var sw2 = new StreamWriter(fs2);
        sw2.Write(codeBlock.ToString());
        sw2.Close();

        // Assert
        Debug.WriteLine("CodeBlocks:");
        Debug.WriteLine(codeBlock.ToString());

        Debug.WriteLine("Code:");
        Debug.WriteLine(codeBlock.ToCode(2));            
        AssertAreEqual(string.Join(Environment.NewLine, codes??[""]), codeBlock.ToString());
    }

    [TestMethod]
    public void OnToken_Assignment_AttachesTypedLiteralAssignment()
    {
        var container = new CodeBlock() { Name = "Container", Code = "", Parent = null, SourcePos = -1 };
        ICodeBlock root = new CodeBlock() { Name = "Declaration", Code = "", Parent = container, SourcePos = -1 };
        var data = testClass.NewData(root);
        var tokens = new List<TokenData>
        {
            new("Target", CodeBlockType.Variable, 1, 1),
            new(":=", CodeBlockType.Operation, 1, 8),
            new("5", CodeBlockType.Number, 2, 11),
        };

        foreach (var token in tokens)
        {
            testClass.OnToken(token, data);
        }

        Assert.AreEqual(1, root.SubBlocks.Count);
        Assert.IsInstanceOfType<IECCodeBlock>(root.SubBlocks[0]);
        var assignment = (IECCodeBlock)root.SubBlocks[0];
        Assert.AreEqual(CodeBlockType.Assignment, assignment.Type);
        Assert.IsInstanceOfType<IecAssignmentStatement>(assignment.AstNode);

        var typedAssignment = (IecAssignmentStatement)assignment.AstNode!;
        Assert.AreEqual("Target", typedAssignment.Target.Identifier);
        Assert.IsInstanceOfType<IecLiteralExpression>(typedAssignment.Value);
        Assert.AreEqual(5, ((IecLiteralExpression)typedAssignment.Value).Value);
    }

    [TestMethod]
    public void OnToken_Assignment_AttachesTypedFunctionCallAssignment()
    {
        var container = new CodeBlock() { Name = "Container", Code = "", Parent = null, SourcePos = -1 };
        ICodeBlock root = new CodeBlock() { Name = "Declaration", Code = "", Parent = container, SourcePos = -1 };
        var data = testClass.NewData(root);
        var tokens = new List<TokenData>
        {
            new("Target", CodeBlockType.Variable, 1, 1),
            new(":=", CodeBlockType.Operation, 1, 8),
            new("rw_ABS", CodeBlockType.Function, 2, 11),
            new("(", CodeBlockType.Bracket, 2, 17),
            new("Input", CodeBlockType.Variable, 3, 18),
            new(")", CodeBlockType.Bracket, 2, 23),
        };

        foreach (var token in tokens)
        {
            testClass.OnToken(token, data);
        }

        Assert.AreEqual(1, root.SubBlocks.Count);
        Assert.IsInstanceOfType<IECCodeBlock>(root.SubBlocks[0]);
        var assignment = (IECCodeBlock)root.SubBlocks[0];
        Assert.IsInstanceOfType<IecAssignmentStatement>(assignment.AstNode);

        var typedAssignment = (IecAssignmentStatement)assignment.AstNode!;
        Assert.IsInstanceOfType<IecFunctionCallExpression>(typedAssignment.Value);

        var functionCall = (IecFunctionCallExpression)typedAssignment.Value;
        Assert.AreEqual("rw_ABS", functionCall.FunctionName);
        Assert.AreEqual(1, functionCall.Arguments.Count);
        Assert.IsInstanceOfType<IecIdentifierExpression>(functionCall.Arguments[0]);
        Assert.AreEqual("Input", ((IecIdentifierExpression)functionCall.Arguments[0]).Identifier);
    }

    [TestMethod]
    public void OnToken_Assignment_EmitsExpectedLiteralCode()
    {
        var container = new CodeBlock() { Name = "Container", Code = "", Parent = null, SourcePos = -1 };
        ICodeBlock root = new CodeBlock() { Name = "Declaration", Code = "", Parent = container, SourcePos = -1 };
        var data = testClass.NewData(root);
        var tokens = new List<TokenData>
        {
            new("Target", CodeBlockType.Variable, 1, 1),
            new(":=", CodeBlockType.Operation, 1, 8),
            new("5", CodeBlockType.Number, 2, 11),
        };

        foreach (var token in tokens)
        {
            testClass.OnToken(token, data);
        }

        Assert.AreEqual("Target:=5", NormalizeCodeSequence(root.ToCode(2)));
    }

    [TestMethod]
    public void OnToken_Assignment_EmitsExpectedFunctionCallCode()
    {
        var container = new CodeBlock() { Name = "Container", Code = "", Parent = null, SourcePos = -1 };
        ICodeBlock root = new CodeBlock() { Name = "Declaration", Code = "", Parent = container, SourcePos = -1 };
        var data = testClass.NewData(root);
        var tokens = new List<TokenData>
        {
            new("Target", CodeBlockType.Variable, 1, 1),
            new(":=", CodeBlockType.Operation, 1, 8),
            new("rw_ABS", CodeBlockType.Function, 2, 11),
            new("(", CodeBlockType.Bracket, 2, 17),
            new("Input", CodeBlockType.Variable, 3, 18),
            new(")", CodeBlockType.Bracket, 2, 23),
        };

        foreach (var token in tokens)
        {
            testClass.OnToken(token, data);
        }

        Assert.AreEqual("Target:=rw_ABS(Input)", NormalizeCodeSequence(root.ToCode(2)));
    }

    [TestMethod]
    public void OnToken_Declaration_ColonOwnsIdentifierAndDeclaredType()
    {
        var root = BuildCodeBlockTree((List<TokenData>)IECTestDataClass.TestDataList0());

        var declaration = FindDeclarationByLeftCode(root, "v");

        Assert.IsNotNull(declaration, root.ToString());
        Assert.AreEqual(2, declaration.SubBlocks.Count);
        Assert.AreEqual("v", declaration.SubBlocks[0].Code);
        Assert.AreEqual("udtVectorData", declaration.SubBlocks[1].Code);
    }

    [TestMethod]
    public void OnToken_Declaration_ColonOwnsArrayTypeStructure()
    {
        var root = BuildCodeBlockTree((List<TokenData>)IECTestDataClass.TestDataList0());

        var declaration = FindDeclarationByLeftCode(root, "Koordinate");

        Assert.IsNotNull(declaration, root.ToString());
        Assert.AreEqual("Koordinate", declaration.SubBlocks[0].Code);
        Assert.AreEqual("ARRAY[", declaration.SubBlocks[1].Code);
        Assert.AreEqual(CodeBlockType.Function, declaration.SubBlocks[1].Type);
        var rendered = NormalizeCodeSequence(declaration.ToCode(2));
        Assert.IsTrue(rendered.Contains("Koordinate"), rendered);
        Assert.IsTrue(rendered.Contains("ARRAY["), rendered);
        Assert.IsTrue(rendered.Contains("0..1"), rendered);
        Assert.IsTrue(rendered.Contains("OF"), rendered);
        Assert.IsTrue(rendered.Contains("LREAL"), rendered);
    }
}