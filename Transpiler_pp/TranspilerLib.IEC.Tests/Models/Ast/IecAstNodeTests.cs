using TranspilerLib.Data;
using TranspilerLib.IEC.Models.Ast;
using TranspilerLib.Models.Scanner;

namespace TranspilerLib.IEC.Models.Ast.Tests;

[TestClass]
public class IecAstNodeTests
{
    [TestMethod]
    public void CompilationUnit_StoresDeclarationsAndStatements()
    {
        var declaration = new IecVariableDeclaration("Value", "INT", sourcePos: 3);
        var statement = new IecAssignmentStatement(
            new IecIdentifierExpression("Value", 10),
            new IecLiteralExpression(5, 18),
            10);

        var unit = new IecCompilationUnit([declaration], [statement], sourcePos: 1);

        Assert.AreEqual(1, unit.SourcePos);
        Assert.AreEqual(1, unit.Declarations.Count);
        Assert.AreEqual("Value", unit.Declarations[0].Identifier);
        Assert.AreEqual("INT", unit.Declarations[0].TypeName);
        Assert.AreEqual(1, unit.Statements.Count);
        Assert.AreSame(statement, unit.Statements[0]);
    }

    [TestMethod]
    public void FunctionCall_StoresArguments()
    {
        var expression = new IecFunctionCallExpression(
            "rw_CONCAT",
            [new IecIdentifierExpression("a"), new IecLiteralExpression("B")],
            12);

        Assert.AreEqual("rw_CONCAT", expression.FunctionName);
        Assert.AreEqual(2, expression.Arguments.Count);
        Assert.IsInstanceOfType<IecIdentifierExpression>(expression.Arguments[0]);
        Assert.IsInstanceOfType<IecLiteralExpression>(expression.Arguments[1]);
        Assert.AreEqual(12, expression.SourcePos);
    }

    [TestMethod]
    public void BinaryExpression_StoresOperatorAndOperands()
    {
        var expression = new IecBinaryExpression(
            new IecIdentifierExpression("Left", 2),
            IecBinaryOperator.Add,
            new IecUnaryExpression(IecUnaryOperator.Negate, new IecLiteralExpression(4, 8), 7),
            5);

        Assert.AreEqual(IecBinaryOperator.Add, expression.OperatorType);
        Assert.IsInstanceOfType<IecIdentifierExpression>(expression.Left);
        Assert.IsInstanceOfType<IecUnaryExpression>(expression.Right);
        Assert.AreEqual(5, expression.SourcePos);
    }

    [TestMethod]
    public void Declaration_ExposesSourcePosition()
    {
        var declaration = new IecVariableDeclaration("Value", "INT", sourcePos: 42);

        Assert.AreEqual(42, declaration.SourcePos);
    }

    [TestMethod]
    public void Declaration_InheritsCodeBlockShell()
    {
        var declaration = new IecVariableDeclaration("Value", "INT", sourcePos: 42)
        {
            Code = "Value : INT",
            Type = CodeBlockType.Variable,
        };

        Assert.AreEqual("Value : INT", declaration.Code);
        Assert.AreEqual(CodeBlockType.Variable, declaration.Type);
        Assert.AreEqual(0, declaration.SubBlocks.Count);
    }

    [TestMethod]
    public void Statement_InheritsParentChildBlockBehavior()
    {
        var parent = new IecCompilationUnit();
        var statement = new IecAssignmentStatement(
            new IecIdentifierExpression("Value"),
            new IecLiteralExpression(5),
            sourcePos: 10)
        {
            Code = "Value := 5",
            Type = CodeBlockType.Assignment,
        };

        statement.Parent = parent;

        Assert.AreSame(parent, statement.Parent);
        Assert.AreEqual(1, parent.SubBlocks.Count);
        Assert.AreSame(statement, parent.SubBlocks[0]);
        Assert.AreEqual(1, statement.Level);
    }

    [TestMethod]
    public void SymbolTable_ExposesIndexedSymbols()
    {
        var symbolTable = new IecSymbolTable(
        [
            new IecVariableDeclaration("Value", "INT"),
            new IecVariableDeclaration("Other", "BOOL"),
        ]);

        Assert.AreEqual(2, symbolTable.Symbols.Count);
        Assert.IsTrue(symbolTable.Symbols.ContainsKey("Value"));
        Assert.AreEqual("BOOL", symbolTable.Symbols["Other"].TypeName);
    }
}
