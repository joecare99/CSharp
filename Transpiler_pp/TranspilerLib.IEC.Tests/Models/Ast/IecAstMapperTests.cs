using TranspilerLib.Data;
using TranspilerLib.IEC.Models.Ast;
using TranspilerLib.IEC.Models.Scanner;

namespace TranspilerLib.IEC.Models.Ast.Tests;

[TestClass]
public class IecAstMapperTests
{
    [TestMethod]
    public void TryGetAssignmentStatement_ReturnsAttachedTypedNode()
    {
        var assignment = new IecAssignmentStatement(
            new IecIdentifierExpression("Target"),
            new IecLiteralExpression(1));
        var block = new IECCodeBlock
        {
            Code = "Target := 1",
            Type = CodeBlockType.Assignment,
            AstNode = assignment,
        };

        var result = IecAstMapper.TryGetAssignmentStatement(block, out var actual);

        Assert.IsTrue(result);
        Assert.AreSame(assignment, actual);
    }

    [TestMethod]
    public void TryGetAssignmentStatement_ReturnsFalseWithoutTypedNode()
    {
        var block = new IECCodeBlock
        {
            Code = "Target := 1",
            Type = CodeBlockType.Assignment,
        };

        var result = IecAstMapper.TryGetAssignmentStatement(block, out var actual);

        Assert.IsFalse(result);
        Assert.IsNull(actual);
    }

    [TestMethod]
    public void TryAttachAssignmentStatement_MapsBinaryExpressionWithPrecedence()
    {
        var assignment = new IECCodeBlock
        {
            Code = ":=",
            Type = CodeBlockType.Assignment,
        };
        _ = new IECCodeBlock { Code = "Target", Type = CodeBlockType.Variable, Parent = assignment };
        _ = new IECCodeBlock { Code = "Left", Type = CodeBlockType.Variable, Parent = assignment };
        _ = new IECCodeBlock { Code = "+", Type = CodeBlockType.Operation, Parent = assignment };
        _ = new IECCodeBlock { Code = "Middle", Type = CodeBlockType.Variable, Parent = assignment };
        _ = new IECCodeBlock { Code = "*", Type = CodeBlockType.Operation, Parent = assignment };
        _ = new IECCodeBlock { Code = "Right", Type = CodeBlockType.Variable, Parent = assignment };

        var result = IecAstMapper.TryAttachAssignmentStatement(assignment);

        Assert.IsTrue(result);
        Assert.IsInstanceOfType<IecAssignmentStatement>(assignment.AstNode);
        var statement = (IecAssignmentStatement)assignment.AstNode!;
        Assert.IsInstanceOfType<IecBinaryExpression>(statement.Value);
        var addExpression = (IecBinaryExpression)statement.Value;
        Assert.AreEqual(IecBinaryOperator.Add, addExpression.OperatorType);
        Assert.IsInstanceOfType<IecIdentifierExpression>(addExpression.Left);
        Assert.IsInstanceOfType<IecBinaryExpression>(addExpression.Right);
        var multiplyExpression = (IecBinaryExpression)addExpression.Right;
        Assert.AreEqual(IecBinaryOperator.Multiply, multiplyExpression.OperatorType);
    }

    [TestMethod]
    public void TryAttachAssignmentStatement_MapsFunctionCallArgumentBinaryExpression()
    {
        var assignment = new IECCodeBlock
        {
            Code = ":=",
            Type = CodeBlockType.Assignment,
        };
        _ = new IECCodeBlock { Code = "Target", Type = CodeBlockType.Variable, Parent = assignment };
        _ = new IECCodeBlock { Code = "LIMIT(", Type = CodeBlockType.Function, Parent = assignment };
        _ = new IECCodeBlock { Code = "Low", Type = CodeBlockType.Variable, Parent = assignment };
        _ = new IECCodeBlock { Code = ",", Type = CodeBlockType.Operation, Parent = assignment };
        _ = new IECCodeBlock { Code = "Value", Type = CodeBlockType.Variable, Parent = assignment };
        _ = new IECCodeBlock { Code = "/", Type = CodeBlockType.Operation, Parent = assignment };
        _ = new IECCodeBlock { Code = "Divisor", Type = CodeBlockType.Variable, Parent = assignment };
        _ = new IECCodeBlock { Code = ",", Type = CodeBlockType.Operation, Parent = assignment };
        _ = new IECCodeBlock { Code = "High", Type = CodeBlockType.Variable, Parent = assignment };

        var result = IecAstMapper.TryAttachAssignmentStatement(assignment);

        Assert.IsTrue(result);
        Assert.IsInstanceOfType<IecAssignmentStatement>(assignment.AstNode);
        var statement = (IecAssignmentStatement)assignment.AstNode!;
        Assert.IsInstanceOfType<IecFunctionCallExpression>(statement.Value);
        var functionCall = (IecFunctionCallExpression)statement.Value;
        Assert.AreEqual("LIMIT", functionCall.FunctionName);
        Assert.AreEqual(3, functionCall.Arguments.Count);
        Assert.IsInstanceOfType<IecBinaryExpression>(functionCall.Arguments[1]);
        Assert.AreEqual(IecBinaryOperator.Divide, ((IecBinaryExpression)functionCall.Arguments[1]).OperatorType);
    }

    [TestMethod]
    public void TryAttachAssignmentStatement_MapsComparisonWithNestedFunctionCallArgument()
    {
        var assignment = new IECCodeBlock
        {
            Code = ":=",
            Type = CodeBlockType.Assignment,
        };
        _ = new IECCodeBlock { Code = "Target", Type = CodeBlockType.Variable, Parent = assignment };
        _ = new IECCodeBlock { Code = "SEL(", Type = CodeBlockType.Function, Parent = assignment };
        _ = new IECCodeBlock { Code = "ABS(", Type = CodeBlockType.Function, Parent = assignment };
        _ = new IECCodeBlock { Code = "Diff", Type = CodeBlockType.Variable, Parent = assignment };
        _ = new IECCodeBlock { Code = "<", Type = CodeBlockType.Operation, Parent = assignment };
        _ = new IECCodeBlock { Code = "Max", Type = CodeBlockType.Variable, Parent = assignment };
        _ = new IECCodeBlock { Code = "*", Type = CodeBlockType.Operation, Parent = assignment };
        _ = new IECCodeBlock { Code = "Cycle", Type = CodeBlockType.Variable, Parent = assignment };
        _ = new IECCodeBlock { Code = ",", Type = CodeBlockType.Operation, Parent = assignment };
        _ = new IECCodeBlock { Code = "0.0", Type = CodeBlockType.Number, Parent = assignment };
        _ = new IECCodeBlock { Code = ",", Type = CodeBlockType.Operation, Parent = assignment };
        _ = new IECCodeBlock { Code = "Diff", Type = CodeBlockType.Variable, Parent = assignment };

        var result = IecAstMapper.TryAttachAssignmentStatement(assignment);

        Assert.IsTrue(result);
        Assert.IsInstanceOfType<IecAssignmentStatement>(assignment.AstNode);
        var statement = (IecAssignmentStatement)assignment.AstNode!;
        Assert.IsInstanceOfType<IecFunctionCallExpression>(statement.Value);
        var functionCall = (IecFunctionCallExpression)statement.Value;
        Assert.AreEqual("SEL", functionCall.FunctionName);
        Assert.AreEqual(3, functionCall.Arguments.Count);
        Assert.IsInstanceOfType<IecBinaryExpression>(functionCall.Arguments[0]);

        var comparison = (IecBinaryExpression)functionCall.Arguments[0];
        Assert.AreEqual(IecBinaryOperator.LessThan, comparison.OperatorType);
        Assert.IsInstanceOfType<IecFunctionCallExpression>(comparison.Left);
        Assert.IsInstanceOfType<IecBinaryExpression>(comparison.Right);
        Assert.AreEqual(IecBinaryOperator.Multiply, ((IecBinaryExpression)comparison.Right).OperatorType);
    }

    [TestMethod]
    public void TryGetStatement_ReturnsAttachedTypedNode()
    {
        var returnStatement = new IecReturnStatement(new IecLiteralExpression(1));
        var block = new IECCodeBlock
        {
            Code = "RETURN",
            Type = CodeBlockType.Function,
            AstNode = returnStatement,
        };

        var result = IecAstMapper.TryGetStatement(block, out var actual);

        Assert.IsTrue(result);
        Assert.AreSame(returnStatement, actual);
    }

    [TestMethod]
    public void ExtractStatements_MapsReturnStatement()
    {
        var returnBlock = new IECCodeBlock
        {
            Code = "RETURN",
            Type = CodeBlockType.Function,
        };
        _ = new IECCodeBlock { Code = "Target", Type = CodeBlockType.Variable, Parent = returnBlock };

        var statements = IecAstMapper.ExtractStatements([returnBlock]);

        Assert.AreEqual(1, statements.Count);
        Assert.IsInstanceOfType<IecReturnStatement>(statements[0]);
        Assert.IsInstanceOfType<IecIdentifierExpression>(((IecReturnStatement)statements[0]).Expression);
    }

    [TestMethod]
    public void ExtractStatements_MapsIfStatement_WithElseAssignments()
    {
        var ifBlock = new IECCodeBlock { Code = "IF", Type = CodeBlockType.Block };
        _ = new IECCodeBlock { Code = "Flag", Type = CodeBlockType.Variable, Parent = ifBlock };
        var thenBlock = new IECCodeBlock { Code = "THEN", Type = CodeBlockType.Block };
        var thenAssignment = new IECCodeBlock { Code = ":=", Type = CodeBlockType.Assignment };
        _ = new IECCodeBlock { Code = "Result", Type = CodeBlockType.Variable, Parent = thenAssignment };
        _ = new IECCodeBlock { Code = "1", Type = CodeBlockType.Number, Parent = thenAssignment };
        var elseBlock = new IECCodeBlock { Code = "ELSE", Type = CodeBlockType.Block };
        var elseAssignment = new IECCodeBlock { Code = ":=", Type = CodeBlockType.Assignment };
        _ = new IECCodeBlock { Code = "Result", Type = CodeBlockType.Variable, Parent = elseAssignment };
        _ = new IECCodeBlock { Code = "2", Type = CodeBlockType.Number, Parent = elseAssignment };
        var endIfBlock = new IECCodeBlock { Code = "END_IF", Type = CodeBlockType.Block };

        var root = new IECCodeBlock { Code = "ROOT", Type = CodeBlockType.Block };
        ifBlock.Parent = root;
        thenBlock.Parent = root;
        thenAssignment.Parent = root;
        elseBlock.Parent = root;
        elseAssignment.Parent = root;
        endIfBlock.Parent = root;

        var statements = IecAstMapper.ExtractStatements(root.SubBlocks);

        Assert.AreEqual(1, statements.Count);
        Assert.IsInstanceOfType<IecIfStatement>(statements[0]);
        var ifStatement = (IecIfStatement)statements[0];
        Assert.IsInstanceOfType<IecIdentifierExpression>(ifStatement.Condition);
        Assert.AreEqual(1, ifStatement.ThenStatements.Count);
        Assert.AreEqual(1, ifStatement.ElseStatements.Count);
        Assert.IsInstanceOfType<IecAssignmentStatement>(ifStatement.ThenStatements[0]);
        Assert.IsInstanceOfType<IecAssignmentStatement>(ifStatement.ElseStatements[0]);
    }

    [TestMethod]
    public void CreateCompilationUnit_UsesMappedImplementationStatements()
    {
        var declarations = new[]
        {
            new IecVariableDeclaration("Flag", "BOOL", IecDeclarationSection.Input),
            new IecVariableDeclaration("Result", "INT", IecDeclarationSection.Local),
        };
        var assignment = new IECCodeBlock { Code = ":=", Type = CodeBlockType.Assignment };
        _ = new IECCodeBlock { Code = "Result", Type = CodeBlockType.Variable, Parent = assignment };
        _ = new IECCodeBlock { Code = "1", Type = CodeBlockType.Number, Parent = assignment };
        var returnBlock = new IECCodeBlock { Code = "RETURN", Type = CodeBlockType.Function };
        _ = new IECCodeBlock { Code = "Result", Type = CodeBlockType.Variable, Parent = returnBlock };

        var compilationUnit = IecAstMapper.CreateCompilationUnit(declarations, [assignment, returnBlock]);

        Assert.AreEqual(2, compilationUnit.Declarations.Count);
        Assert.AreEqual(2, compilationUnit.Statements.Count);
        Assert.IsInstanceOfType<IecAssignmentStatement>(compilationUnit.Statements[0]);
        Assert.IsInstanceOfType<IecReturnStatement>(compilationUnit.Statements[1]);
    }
}
