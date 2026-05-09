using System.Linq;
using TranspilerLib.IEC.Models.Ast;
using TranspilerLib.IEC.TestData;

namespace TranspilerLib.IEC.Models.Ast.Tests;

/// <summary>
/// Tests lightweight IEC type binding and inference for the current typed AST subset.
/// </summary>
[TestClass]
public class IecTypeBinderTests
{
    private sealed class UnsupportedExpression : IecExpression
    {
        public UnsupportedExpression()
            : base(-1)
        {
        }
    }

    private sealed class UnsupportedUnaryExpression : IecExpression
    {
        public UnsupportedUnaryExpression()
            : base(-1)
        {
        }
    }

    [TestMethod]
    public void Bind_InfersTypes_For_BoundAssignmentExpressions()
    {
        var statement = new IecAssignmentStatement(
            new IecIdentifierExpression("Target"),
            new IecBinaryExpression(
                new IecIdentifierExpression("Left"),
                IecBinaryOperator.Add,
                new IecLiteralExpression(1.5)));
        var compilationUnit = new IecCompilationUnit(
        [
            new IecVariableDeclaration("Target", "LREAL", IecDeclarationSection.Local),
            new IecVariableDeclaration("Left", "INT", IecDeclarationSection.Input),
        ],
        [statement]);
        var bindingResult = IecIdentifierBinder.Bind(compilationUnit);

        var result = IecTypeBinder.Bind(compilationUnit, bindingResult);

        Assert.IsTrue(result.StatementTypes.Any(entry => ReferenceEquals(entry.Key, statement) && entry.Value == "LREAL"));
        Assert.IsTrue(result.ExpressionTypes.Any(entry => ReferenceEquals(entry.Key, statement.Target) && entry.Value == "LREAL"));
        Assert.IsTrue(result.ExpressionTypes.Any(entry => ReferenceEquals(entry.Key, statement.Value) && entry.Value == "LREAL"));
        Assert.AreEqual(0, result.UnresolvedExpressions.Count);
    }

    [TestMethod]
    public void Bind_InfersBoolType_For_ComparisonExpression()
    {
        var comparison = new IecBinaryExpression(
            new IecIdentifierExpression("Actual"),
            IecBinaryOperator.LessThan,
            new IecIdentifierExpression("Limit"));
        var statement = new IecAssignmentStatement(new IecIdentifierExpression("Target"), comparison);
        var compilationUnit = new IecCompilationUnit(
        [
            new IecVariableDeclaration("Target", "BOOL", IecDeclarationSection.Local),
            new IecVariableDeclaration("Actual", "LREAL", IecDeclarationSection.Input),
            new IecVariableDeclaration("Limit", "LREAL", IecDeclarationSection.Input),
        ],
        [statement]);
        var bindingResult = IecIdentifierBinder.Bind(compilationUnit);

        var result = IecTypeBinder.Bind(compilationUnit, bindingResult);

        Assert.IsTrue(result.ExpressionTypes.Any(entry => ReferenceEquals(entry.Key, comparison) && entry.Value == "BOOL"));
        Assert.IsTrue(result.StatementTypes.Any(entry => ReferenceEquals(entry.Key, statement) && entry.Value == "BOOL"));
    }

    [TestMethod]
    public void Bind_InfersTypes_For_ExportBasedStatement()
    {
        var fixture = IecExportFixtureData.LoadMfComputeAlignmentRot();
        var declarations = IecDeclarationExtractor.ExtractDeclarations(fixture.DeclarationText);
        var statement = new IecAssignmentStatement(
            new IecIdentifierExpression("_lrAngleSetpDiff"),
            new IecFunctionCallExpression(
                "SEL",
                [
                    new IecBinaryExpression(
                        new IecFunctionCallExpression("ABS", [new IecIdentifierExpression("_lrAngleSetpDiff")]),
                        IecBinaryOperator.LessThan,
                        new IecBinaryExpression(
                            new IecIdentifierExpression("_lrMaxAngleVel"),
                            IecBinaryOperator.Multiply,
                            new IecIdentifierExpression("lrCycleTime"))),
                    new IecLiteralExpression(0.0),
                    new IecIdentifierExpression("_lrAngleSetpDiff"),
                ]));
        var compilationUnit = new IecCompilationUnit(declarations, [statement]);
        var bindingResult = IecIdentifierBinder.Bind(compilationUnit);

        var result = IecTypeBinder.Bind(compilationUnit, bindingResult);

        Assert.IsTrue(result.StatementTypes.Any(entry => ReferenceEquals(entry.Key, statement) && entry.Value == "LREAL"));
        Assert.IsTrue(result.ExpressionTypes.Any(entry => ReferenceEquals(entry.Key, statement.Value) && entry.Value == "LREAL"));
        Assert.IsTrue(result.ExpressionTypes.Any(entry => entry.Key is IecBinaryExpression binary && binary.OperatorType == IecBinaryOperator.LessThan && entry.Value == "BOOL"));
        Assert.IsTrue(result.UnresolvedExpressions.OfType<IecIdentifierExpression>().Any(identifier => identifier.Identifier == "lrCycleTime"));
    }

    [TestMethod]
    public void Bind_InfersBoolType_For_LogicalExpressionWithUnaryCondition()
    {
        var unary = new IecUnaryExpression(IecUnaryOperator.Not, new IecIdentifierExpression("Flag"));
        var comparison = new IecBinaryExpression(
            new IecIdentifierExpression("Left"),
            IecBinaryOperator.LessThan,
            new IecIdentifierExpression("Right"));
        var logicalExpression = new IecBinaryExpression(unary, IecBinaryOperator.Or, comparison);
        var statement = new IecAssignmentStatement(new IecIdentifierExpression("Target"), logicalExpression);
        var compilationUnit = new IecCompilationUnit(
        [
            new IecVariableDeclaration("Target", "BOOL", IecDeclarationSection.Local),
            new IecVariableDeclaration("Flag", "BOOL", IecDeclarationSection.Input),
            new IecVariableDeclaration("Left", "INT", IecDeclarationSection.Input),
            new IecVariableDeclaration("Right", "INT", IecDeclarationSection.Input),
        ],
        [statement]);
        var bindingResult = IecIdentifierBinder.Bind(compilationUnit);

        var result = IecTypeBinder.Bind(compilationUnit, bindingResult);

        Assert.IsTrue(result.ExpressionTypes.Any(entry => ReferenceEquals(entry.Key, unary) && entry.Value == "BOOL"));
        Assert.IsTrue(result.ExpressionTypes.Any(entry => ReferenceEquals(entry.Key, comparison) && entry.Value == "BOOL"));
        Assert.IsTrue(result.ExpressionTypes.Any(entry => ReferenceEquals(entry.Key, logicalExpression) && entry.Value == "BOOL"));
        Assert.IsTrue(result.StatementTypes.Any(entry => ReferenceEquals(entry.Key, statement) && entry.Value == "BOOL"));
    }

    [TestMethod]
    public void Bind_InfersOperandType_ForNegatedNumericExpression()
    {
        var unary = new IecUnaryExpression(IecUnaryOperator.Negate, new IecLiteralExpression(3));
        var statement = new IecReturnStatement(unary);
        var compilationUnit = new IecCompilationUnit(statements: [statement]);
        var bindingResult = IecIdentifierBinder.Bind(compilationUnit);

        var result = IecTypeBinder.Bind(compilationUnit, bindingResult);

        Assert.IsTrue(result.ExpressionTypes.Any(entry => ReferenceEquals(entry.Key, unary) && entry.Value == "INT"));
        Assert.IsTrue(result.StatementTypes.Any(entry => ReferenceEquals(entry.Key, statement) && entry.Value == "INT"));
    }

    [TestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public void Bind_MergesNumericType_WhenOneOperandIsUnresolved(bool unresolvedOnLeft)
    {
        var unresolvedIdentifier = new IecIdentifierExpression("Missing");
        var knownLiteral = new IecLiteralExpression(1);
        IecExpression left = unresolvedOnLeft ? unresolvedIdentifier : knownLiteral;
        IecExpression right = unresolvedOnLeft ? knownLiteral : unresolvedIdentifier;
        var binary = new IecBinaryExpression(left, IecBinaryOperator.Add, right);
        var statement = new IecAssignmentStatement(new IecIdentifierExpression("Target"), binary);
        var compilationUnit = new IecCompilationUnit(
        [new IecVariableDeclaration("Target", "INT", IecDeclarationSection.Local)],
        [statement]);
        var bindingResult = IecIdentifierBinder.Bind(compilationUnit);

        var result = IecTypeBinder.Bind(compilationUnit, bindingResult);

        Assert.IsTrue(result.ExpressionTypes.Any(entry => ReferenceEquals(entry.Key, binary) && entry.Value == "INT"));
        Assert.IsTrue(result.UnresolvedExpressions.OfType<IecIdentifierExpression>().Any(identifier => ReferenceEquals(identifier, unresolvedIdentifier)));
    }

    [TestMethod]
    [DataRow("ABS", "LREAL")]
    [DataRow("SIGN", "LREAL")]
    [DataRow("rw_ABS", "INT")]
    public void Bind_UsesKnownFunctionReturnTypes(string functionName, string expectedType)
    {
        var functionCall = new IecFunctionCallExpression(functionName, [new IecLiteralExpression(1)]);
        var statement = new IecReturnStatement(functionCall);
        var compilationUnit = new IecCompilationUnit(statements: [statement]);
        var bindingResult = IecIdentifierBinder.Bind(compilationUnit);

        var result = IecTypeBinder.Bind(compilationUnit, bindingResult);

        Assert.IsTrue(result.ExpressionTypes.Any(entry => ReferenceEquals(entry.Key, functionCall) && entry.Value == expectedType));
        Assert.IsTrue(result.StatementTypes.Any(entry => ReferenceEquals(entry.Key, statement) && entry.Value == expectedType));
    }

    [TestMethod]
    public void Bind_TracksUnknownFunctionCall_AsUnresolvedExpression()
    {
        var functionCall = new IecFunctionCallExpression("Custom", [new IecLiteralExpression(1)]);
        var statement = new IecReturnStatement(functionCall);
        var compilationUnit = new IecCompilationUnit(statements: [statement]);
        var bindingResult = IecIdentifierBinder.Bind(compilationUnit);

        var result = IecTypeBinder.Bind(compilationUnit, bindingResult);

        Assert.AreEqual(0, result.StatementTypes.Count);
        Assert.IsTrue(result.ExpressionTypes.Any(entry => entry.Key is IecLiteralExpression && entry.Value == "INT"));
        Assert.AreEqual(1, result.UnresolvedExpressions.Count(expression => ReferenceEquals(expression, functionCall)));
    }

    [TestMethod]
    public void Bind_InfersConditionType_AndBranchStatements_ForIfStatement()
    {
        var condition = new IecIdentifierExpression("Flag");
        var thenAssignment = new IecAssignmentStatement(new IecIdentifierExpression("Target"), new IecLiteralExpression("Then"));
        var elseReturn = new IecReturnStatement(new IecLiteralExpression(false));
        var ifStatement = new IecIfStatement(condition, [thenAssignment], [elseReturn]);
        var compilationUnit = new IecCompilationUnit(
        [
            new IecVariableDeclaration("Flag", "BOOL", IecDeclarationSection.Input),
            new IecVariableDeclaration("Target", "STRING", IecDeclarationSection.Local),
        ],
        [ifStatement]);
        var bindingResult = IecIdentifierBinder.Bind(compilationUnit);

        var result = IecTypeBinder.Bind(compilationUnit, bindingResult);

        Assert.IsTrue(result.StatementTypes.Any(entry => ReferenceEquals(entry.Key, ifStatement) && entry.Value == "BOOL"));
        Assert.IsTrue(result.StatementTypes.Any(entry => ReferenceEquals(entry.Key, thenAssignment) && entry.Value == "STRING"));
        Assert.IsTrue(result.StatementTypes.Any(entry => ReferenceEquals(entry.Key, elseReturn) && entry.Value == "BOOL"));
        Assert.IsTrue(result.ExpressionTypes.Any(entry => ReferenceEquals(entry.Key, condition) && entry.Value == "BOOL"));
        Assert.IsTrue(result.ExpressionTypes.Any(entry => entry.Key is IecLiteralExpression literal && Equals(literal.Value, "Then") && entry.Value == "STRING"));
        Assert.IsTrue(result.ExpressionTypes.Any(entry => entry.Key is IecLiteralExpression literal && Equals(literal.Value, false) && entry.Value == "BOOL"));
    }

    [TestMethod]
    public void Bind_CachesUnsupportedExpression_AsSingleUnresolvedEntry()
    {
        var unsupportedExpression = new UnsupportedExpression();
        var statement = new IecReturnStatement(unsupportedExpression);
        var compilationUnit = new IecCompilationUnit(statements: [statement]);
        var bindingResult = IecIdentifierBinder.Bind(compilationUnit);

        var result = IecTypeBinder.Bind(compilationUnit, bindingResult);

        Assert.AreEqual(0, result.ExpressionTypes.Count(entry => ReferenceEquals(entry.Key, unsupportedExpression)));
        Assert.AreEqual(1, result.UnresolvedExpressions.Count(expression => ReferenceEquals(expression, unsupportedExpression)));
        Assert.AreEqual(0, result.StatementTypes.Count(entry => ReferenceEquals(entry.Key, statement)));
    }

    [TestMethod]
    public void Bind_TracksUnsupportedLiteral_AsUnresolvedExpression()
    {
        var unsupportedLiteral = new IecLiteralExpression(new object());
        var statement = new IecReturnStatement(unsupportedLiteral);
        var compilationUnit = new IecCompilationUnit(statements: [statement]);
        var bindingResult = IecIdentifierBinder.Bind(compilationUnit);

        var result = IecTypeBinder.Bind(compilationUnit, bindingResult);

        Assert.AreEqual(1, result.UnresolvedExpressions.Count(expression => ReferenceEquals(expression, unsupportedLiteral)));
        Assert.AreEqual(0, result.StatementTypes.Count(entry => ReferenceEquals(entry.Key, statement)));
    }

    [TestMethod]
    public void Bind_MergesNumericType_FromLeftOperand_WhenRightOperandIsUnknown()
    {
        var missing = new IecIdentifierExpression("Missing");
        var binary = new IecBinaryExpression(new IecLiteralExpression(1.5), IecBinaryOperator.Add, missing);
        var statement = new IecReturnStatement(binary);
        var compilationUnit = new IecCompilationUnit(statements: [statement]);
        var bindingResult = IecIdentifierBinder.Bind(compilationUnit);

        var result = IecTypeBinder.Bind(compilationUnit, bindingResult);

        Assert.IsTrue(result.ExpressionTypes.Any(entry => ReferenceEquals(entry.Key, binary) && entry.Value == "LREAL"));
        Assert.IsTrue(result.StatementTypes.Any(entry => ReferenceEquals(entry.Key, statement) && entry.Value == "LREAL"));
        Assert.IsTrue(result.UnresolvedExpressions.Any(expression => ReferenceEquals(expression, missing)));
    }

    [TestMethod]
    [DataRow(IecBinaryOperator.Subtract)]
    [DataRow(IecBinaryOperator.Multiply)]
    [DataRow(IecBinaryOperator.Divide)]
    public void Bind_UsesLeftNumericType_ForSupportedIntArithmetic(IecBinaryOperator operatorType)
    {
        var binary = new IecBinaryExpression(new IecLiteralExpression(4), operatorType, new IecLiteralExpression(2));
        var statement = new IecReturnStatement(binary);
        var compilationUnit = new IecCompilationUnit(statements: [statement]);
        var bindingResult = IecIdentifierBinder.Bind(compilationUnit);

        var result = IecTypeBinder.Bind(compilationUnit, bindingResult);

        Assert.IsTrue(result.ExpressionTypes.Any(entry => ReferenceEquals(entry.Key, binary) && entry.Value == "INT"));
        Assert.IsTrue(result.StatementTypes.Any(entry => ReferenceEquals(entry.Key, statement) && entry.Value == "INT"));
    }

    [TestMethod]
    public void Bind_ReusesCachedExpressionType_ForSharedExpressionInstance()
    {
        var sharedExpression = new IecBinaryExpression(new IecLiteralExpression(1), IecBinaryOperator.Add, new IecLiteralExpression(2));
        var firstStatement = new IecAssignmentStatement(new IecIdentifierExpression("Left"), sharedExpression);
        var secondStatement = new IecAssignmentStatement(new IecIdentifierExpression("Right"), sharedExpression);
        var compilationUnit = new IecCompilationUnit(
        [
            new IecVariableDeclaration("Left", "INT", IecDeclarationSection.Local),
            new IecVariableDeclaration("Right", "INT", IecDeclarationSection.Local),
        ],
        [firstStatement, secondStatement]);
        var bindingResult = IecIdentifierBinder.Bind(compilationUnit);

        var result = IecTypeBinder.Bind(compilationUnit, bindingResult);

        Assert.AreEqual(1, result.ExpressionTypes.Count(entry => ReferenceEquals(entry.Key, sharedExpression)));
        Assert.IsTrue(result.StatementTypes.Any(entry => ReferenceEquals(entry.Key, firstStatement) && entry.Value == "INT"));
        Assert.IsTrue(result.StatementTypes.Any(entry => ReferenceEquals(entry.Key, secondStatement) && entry.Value == "INT"));
    }

    [TestMethod]
    public void Bind_ReturnsOperandType_ForInvalidUnaryOperatorValue()
    {
        var unary = new IecUnaryExpression((IecUnaryOperator)999, new IecLiteralExpression(3));
        var statement = new IecReturnStatement(unary);
        var compilationUnit = new IecCompilationUnit(statements: [statement]);
        var bindingResult = IecIdentifierBinder.Bind(compilationUnit);

        var result = IecTypeBinder.Bind(compilationUnit, bindingResult);

        Assert.AreEqual(1, result.UnresolvedExpressions.Count(expression => ReferenceEquals(expression, unary)));
        Assert.AreEqual(0, result.StatementTypes.Count(entry => ReferenceEquals(entry.Key, statement)));
    }

    [TestMethod]
    public void Bind_ReturnsUnresolved_ForInvalidBinaryOperatorValue()
    {
        var binary = new IecBinaryExpression(new IecLiteralExpression(1), (IecBinaryOperator)999, new IecLiteralExpression(2));
        var statement = new IecReturnStatement(binary);
        var compilationUnit = new IecCompilationUnit(statements: [statement]);
        var bindingResult = IecIdentifierBinder.Bind(compilationUnit);

        var result = IecTypeBinder.Bind(compilationUnit, bindingResult);

        Assert.AreEqual(1, result.UnresolvedExpressions.Count(expression => ReferenceEquals(expression, binary)));
        Assert.AreEqual(0, result.StatementTypes.Count(entry => ReferenceEquals(entry.Key, statement)));
    }
}
