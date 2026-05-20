using System.Linq;
using TranspilerLib.IEC.Models.Ast;
using TranspilerLib.IEC.TestData;

namespace TranspilerLib.IEC.Models.Ast.Tests;

/// <summary>
/// Tests identifier binding between typed IEC AST expressions and extracted declarations.
/// </summary>
[TestClass]
public class IecIdentifierBinderTests
{
    [TestMethod]
    public void Bind_AssignmentIdentifiers_ResolveAgainstCompilationUnitSymbols()
    {
        var compilationUnit = new IecCompilationUnit(
        [
            new IecVariableDeclaration("Target", "INT", IecDeclarationSection.Local),
            new IecVariableDeclaration("Input", "INT", IecDeclarationSection.Input),
        ],
        [
            new IecAssignmentStatement(
                new IecIdentifierExpression("Target"),
                new IecBinaryExpression(
                    new IecIdentifierExpression("Input"),
                    IecBinaryOperator.Add,
                    new IecLiteralExpression(1)))
        ]);

        var result = IecIdentifierBinder.Bind(compilationUnit);

        Assert.AreEqual(2, result.Bindings.Count);
        Assert.AreEqual(0, result.UnresolvedIdentifiers.Count);
        Assert.IsTrue(result.Bindings.Any(binding => binding.Key.Identifier == "Target" && binding.Value.Section == IecDeclarationSection.Local));
        Assert.IsTrue(result.Bindings.Any(binding => binding.Key.Identifier == "Input" && binding.Value.Section == IecDeclarationSection.Input));
    }

    [TestMethod]
    public void Bind_ExportFixtureStatementIdentifiers_ResolveAgainstExtractedSymbols()
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

        var result = IecIdentifierBinder.Bind(compilationUnit);

        Assert.IsTrue(result.Bindings.Count >= 4, "The export-based statement should bind its declared identifiers.");
        Assert.IsTrue(result.Bindings.Any(binding => binding.Key.Identifier == "_lrAngleSetpDiff" && binding.Value.Section == IecDeclarationSection.Instance));
        Assert.IsTrue(result.Bindings.Any(binding => binding.Key.Identifier == "_lrMaxAngleVel" && binding.Value.Section == IecDeclarationSection.Instance));
        Assert.AreEqual(1, result.UnresolvedIdentifiers.Count, "The export-based binding should currently leave external runtime identifiers unresolved.");
        Assert.AreEqual("lrCycleTime", result.UnresolvedIdentifiers.Single().Identifier);
    }

    [TestMethod]
    public void Bind_TraversesIfBranchesReturnStatementsAndFunctionArguments()
    {
        var compilationUnit = new IecCompilationUnit(
        [
            new IecVariableDeclaration("Flag", "BOOL", IecDeclarationSection.Input),
            new IecVariableDeclaration("Target", "INT", IecDeclarationSection.Local),
            new IecVariableDeclaration("Input", "INT", IecDeclarationSection.Input),
            new IecVariableDeclaration("Memory", "INT", IecDeclarationSection.InOut),
        ],
        [
            new IecIfStatement(
                new IecIdentifierExpression("Flag"),
                [
                    new IecAssignmentStatement(
                        new IecIdentifierExpression("Target"),
                        new IecFunctionCallExpression(
                            "Custom",
                            [
                                new IecUnaryExpression(IecUnaryOperator.Negate, new IecIdentifierExpression("Input")),
                                new IecBinaryExpression(
                                    new IecIdentifierExpression("Memory"),
                                    IecBinaryOperator.Add,
                                    new IecIdentifierExpression("Missing")),
                            ]))
                ],
                [new IecReturnStatement(new IecIdentifierExpression("Memory"))])
        ]);

        var result = IecIdentifierBinder.Bind(compilationUnit);

        Assert.IsTrue(result.Bindings.Any(binding => binding.Key.Identifier == "Flag"));
        Assert.IsTrue(result.Bindings.Any(binding => binding.Key.Identifier == "Target"));
        Assert.IsTrue(result.Bindings.Any(binding => binding.Key.Identifier == "Input"));
        Assert.AreEqual(2, result.Bindings.Count(binding => binding.Key.Identifier == "Memory"));
        Assert.AreEqual(1, result.UnresolvedIdentifiers.Count);
        Assert.AreEqual("Missing", result.UnresolvedIdentifiers.Single().Identifier);
    }
}
