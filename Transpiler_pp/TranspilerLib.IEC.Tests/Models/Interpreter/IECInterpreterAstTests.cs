using System.Collections.Generic;
using TranspilerLib.Data;
using TranspilerLib.IEC.Models.Ast;
using TranspilerLib.IEC.Models.Scanner;
using TranspilerLib.Interfaces.Code;
using TranspilerLib.Models.Interpreter;
using TranspilerLib.Models.Scanner;

namespace TranspilerLib.IEC.Models.Interpreter.Tests;

[TestClass]
public class IECInterpreterAstTests
{
    private static ICodeBlock CreateProgramRoot(params IECCodeBlock[] blocks)
    {
        var root = new CodeBlock { Code = "ROOT", Type = CodeBlockType.Block };
        foreach (var block in blocks)
        {
            block.Parent = root;
        }

        return root;
    }

    private static IECCodeBlock CreateBeginBlock() => new() { Code = "BEGIN", Type = CodeBlockType.Label };

    [TestMethod]
    public void Interpret_UsesTypedLiteralAssignment()
    {
        var declaration = new IECCodeBlock { Code = "DECL", Type = CodeBlockType.Declaration };
        var begin = CreateBeginBlock();
        var assignment = new IECCodeBlock
        {
            Code = "Target := 5",
            Type = CodeBlockType.Assignment,
            AstNode = new IecAssignmentStatement(
                new IecIdentifierExpression("Target"),
                new IecLiteralExpression(5)),
        };
        var root = CreateProgramRoot(declaration, begin, assignment);
        var sut = new IECInterpreter(root);
        var parameters = new Dictionary<string, object> { ["Target"] = 0 };

        sut.Interpret(declaration, parameters);

        Assert.AreEqual(5, parameters["Target"]);
    }

    [TestMethod]
    public void Interpret_UsesTypedFunctionCallAssignment()
    {
        var declaration = new IECCodeBlock { Code = "DECL", Type = CodeBlockType.Declaration };
        var begin = CreateBeginBlock();
        var assignment = new IECCodeBlock
        {
            Code = "Target := rw_ABS(Input)",
            Type = CodeBlockType.Assignment,
            AstNode = new IecAssignmentStatement(
                new IecIdentifierExpression("Target"),
                new IecFunctionCallExpression("rw_ABS", [new IecIdentifierExpression("Input")]))
        };
        var root = CreateProgramRoot(declaration, begin, assignment);
        var sut = new IECInterpreter(root);
        var parameters = new Dictionary<string, object> { ["Target"] = 0, ["Input"] = -7 };

        sut.Interpret(declaration, parameters);

        Assert.AreEqual(7, parameters["Target"]);
    }

    [TestMethod]
    public void Interpret_UsesTypedUnaryAndBinaryExpressions()
    {
        var declaration = new IECCodeBlock { Code = "DECL", Type = CodeBlockType.Declaration };
        var begin = CreateBeginBlock();
        var assignment = new IECCodeBlock
        {
            Code = "Target := -(Left + Right)",
            Type = CodeBlockType.Assignment,
            AstNode = new IecAssignmentStatement(
                new IecIdentifierExpression("Target"),
                new IecUnaryExpression(
                    IecUnaryOperator.Negate,
                    new IecBinaryExpression(
                        new IecIdentifierExpression("Left"),
                        IecBinaryOperator.Add,
                        new IecIdentifierExpression("Right"))))
        };
        var root = CreateProgramRoot(declaration, begin, assignment);
        var sut = new IECInterpreter(root);
        var parameters = new Dictionary<string, object>
        {
            ["Target"] = 0,
            ["Left"] = 2,
            ["Right"] = 3,
        };

        sut.Interpret(declaration, parameters);

        Assert.AreEqual(-5, parameters["Target"]);
    }
}
