using System.Linq;
using TranspilerLib.IEC.Models.Ast;
using TranspilerLib.IEC.TestData;

namespace TranspilerLib.IEC.Models.Ast.Tests;

/// <summary>
/// Tests declaration extraction and symbol-table construction for realistic IEC export fixtures.
/// </summary>
[TestClass]
public class IecDeclarationExtractorTests
{
    [TestMethod]
    public void ExtractDeclarations_Reads_ExportDeclarationSections()
    {
        var fixture = IecExportFixtureData.LoadMfComputeAlignmentRot();

        var declarations = IecDeclarationExtractor.ExtractDeclarations(fixture.DeclarationText);

        Assert.IsTrue(declarations.Count >= 20, "The export fixture should yield a representative declaration set.");
        Assert.IsTrue(declarations.Any(declaration => declaration.Identifier == "lrAngleSetp" && declaration.Section == IecDeclarationSection.Input && declaration.TypeName == "LREAL"));
        Assert.IsTrue(declarations.Any(declaration => declaration.Identifier == "lrMemAngSetp" && declaration.Section == IecDeclarationSection.InOut && declaration.TypeName == "LREAL"));
        Assert.IsTrue(declarations.Any(declaration => declaration.Identifier == "_lrAngleAct" && declaration.Section == IecDeclarationSection.Instance && declaration.TypeName == "LREAL"));
        Assert.IsTrue(declarations.Any(declaration => declaration.Identifier == "lrDeadTime" && declaration.InitializerText == "0.1"));
        Assert.IsTrue(declarations.Any(declaration => declaration.Identifier == "_xNew" && declaration.TypeName == "BOOL"));
    }

    [TestMethod]
    public void ExtractDeclarations_StoresDeclarationMetadata()
    {
        var declarations = IecDeclarationExtractor.ExtractDeclarations("VAR_INPUT\nInput : LREAL := 1.5;\nEND_VAR");
        var declaration = declarations.Single();

        Assert.AreEqual(IecDeclarationSection.Input, declaration.DeclarationMetadata.Section);
        Assert.AreEqual("1.5", declaration.DeclarationMetadata.InitializerText);
    }

    [TestMethod]
    public void ExtractDeclarationHeader_ReadsMethodMetadata_FromExportStyleHeader()
    {
        var header = IecDeclarationExtractor.ExtractDeclarationHeader("METHOD PROTECTED MF_ComputeAlignmentRot : LREAL;\nVAR_INPUT\nInput : LREAL;\nEND_VAR");

        Assert.AreEqual("MF_ComputeAlignmentRot", header.ArtifactName);
        Assert.AreEqual("LREAL", header.ReturnTypeName);
        Assert.AreEqual(IecArtifactKind.Function, header.ArtifactMetadata.ArtifactKind);
        Assert.AreEqual(IecAccessibility.Protected, header.ArtifactMetadata.Accessibility);
    }

    [TestMethod]
    public void ExtractDeclarations_ReadsOutputSection()
    {
        var declarations = IecDeclarationExtractor.ExtractDeclarations("VAR_OUTPUT\nResult : BOOL;\nEND_VAR");
        var declaration = declarations.Single();

        Assert.AreEqual(IecDeclarationSection.Output, declaration.Section);
        Assert.AreEqual("Result", declaration.Identifier);
        Assert.AreEqual("BOOL", declaration.TypeName);
    }

    [TestMethod]
    public void ExtractDeclarations_SkipsDeclaration_WithMissingIdentifier()
    {
        var declarations = IecDeclarationExtractor.ExtractDeclarations("VAR\n: LREAL;\nValid : INT;\nEND_VAR");

        Assert.AreEqual(1, declarations.Count);
        Assert.AreEqual("Valid", declarations.Single().Identifier);
    }

    [TestMethod]
    public void DeclarationMetadata_InheritsSharedMetadataBase()
    {
        IecMetadata metadata = new IecDeclarationMetadata(IecDeclarationSection.Input, "1.5");

        Assert.IsInstanceOfType<IecDeclarationMetadata>(metadata);
    }

    [TestMethod]
    public void CreateCompilationUnit_Builds_SymbolTable_From_ExportDeclarations()
    {
        var fixture = IecExportFixtureData.LoadMfComputeAlignmentRot();

        var compilationUnit = IecDeclarationExtractor.CreateCompilationUnit(fixture.DeclarationText, fixture.ImplementationText);

        Assert.IsTrue(compilationUnit.Declarations.Count >= 20, "The compilation unit should contain the extracted declarations.");
        Assert.IsTrue(compilationUnit.Symbols.TryGet("lrAngleVelAlign", out var angleVelAlignDeclaration));
        Assert.IsNotNull(angleVelAlignDeclaration);
        Assert.AreEqual(IecDeclarationSection.Input, angleVelAlignDeclaration.Section);
        Assert.AreEqual("LREAL", angleVelAlignDeclaration.TypeName);

        Assert.IsTrue(compilationUnit.Symbols.TryGet("_lrAxisVelTarget", out var axisVelTargetDeclaration));
        Assert.IsNotNull(axisVelTargetDeclaration);
        Assert.AreEqual(IecDeclarationSection.Instance, axisVelTargetDeclaration.Section);
        Assert.AreEqual("LREAL", axisVelTargetDeclaration.TypeName);
    }

    [TestMethod]
    public void CreateCompilationUnit_UsesEmptyStatements_ForCurrentImplementation()
    {
        var compilationUnit = IecDeclarationExtractor.CreateCompilationUnit("VAR\nValue : INT;\nEND_VAR", "Value := 1;");

        Assert.AreEqual(1, compilationUnit.Declarations.Count);
        Assert.AreEqual(0, compilationUnit.Statements.Count);
    }

    [TestMethod]
    public void CreateFromSourceText_MapsTypedStatements_FromMinimalImplementation()
    {
        var compilationUnit = IecFrontendCompilationUnitFactory.CreateFromSourceText(
            "VAR\nValue : INT;\nEND_VAR",
            "Value := 1;");

        Assert.AreEqual(1, compilationUnit.Declarations.Count);
        Assert.AreEqual(1, compilationUnit.Statements.Count);
        Assert.IsInstanceOfType<IecAssignmentStatement>(compilationUnit.Statements[0]);
    }

    [TestMethod]
    public void CreateFromSourceText_MapsReturnStatement_AfterSeparator()
    {
        var compilationUnit = IecFrontendCompilationUnitFactory.CreateFromSourceText(
            "VAR\nValue : INT;\nEND_VAR",
            "Value := 1;\nRETURN Value;");

        Assert.AreEqual(2, compilationUnit.Statements.Count);
        Assert.IsInstanceOfType<IecAssignmentStatement>(compilationUnit.Statements[0]);
        Assert.IsInstanceOfType<IecReturnStatement>(compilationUnit.Statements[1]);
    }

    [TestMethod]
    public void CreateFromSourceText_MapsIfStatement_AfterSeparator()
    {
        var compilationUnit = IecFrontendCompilationUnitFactory.CreateFromSourceText(
            "VAR\nFlag : BOOL;\nValue : INT;\nEND_VAR",
            "Value := 1;\nIF Flag THEN\nValue := 2;\nELSE\nValue := 3;\nEND_IF;");

        Assert.AreEqual(2, compilationUnit.Statements.Count);
        Assert.IsInstanceOfType<IecAssignmentStatement>(compilationUnit.Statements[0]);
        Assert.IsInstanceOfType<IecIfStatement>(compilationUnit.Statements[1]);

        var ifStatement = (IecIfStatement)compilationUnit.Statements[1];
        Assert.AreEqual(1, ifStatement.ThenStatements.Count);
        Assert.AreEqual(1, ifStatement.ElseStatements.Count);
        Assert.IsInstanceOfType<IecAssignmentStatement>(ifStatement.ThenStatements[0]);
        Assert.IsInstanceOfType<IecAssignmentStatement>(ifStatement.ElseStatements[0]);
    }

    [TestMethod]
    public void CreateFromSourceText_MapsElsIfStatementChain()
    {
        var compilationUnit = IecFrontendCompilationUnitFactory.CreateFromSourceText(
            "VAR\nFlagA : BOOL;\nFlagB : BOOL;\nValue : INT;\nEND_VAR",
            "IF FlagA THEN\nValue := 1;\nELSIF FlagB THEN\nValue := 2;\nELSE\nValue := 3;\nEND_IF;");

        Assert.AreEqual(1, compilationUnit.Statements.Count);
        Assert.IsInstanceOfType<IecIfStatement>(compilationUnit.Statements[0]);

        var ifStatement = (IecIfStatement)compilationUnit.Statements[0];
        Assert.AreEqual(1, ifStatement.ThenStatements.Count);
        Assert.AreEqual(1, ifStatement.ElseStatements.Count);
        Assert.IsInstanceOfType<IecIfStatement>(ifStatement.ElseStatements[0]);

        var elseIfStatement = (IecIfStatement)ifStatement.ElseStatements[0];
        Assert.AreEqual(1, elseIfStatement.ThenStatements.Count);
        Assert.AreEqual(1, elseIfStatement.ElseStatements.Count);
        Assert.IsInstanceOfType<IecAssignmentStatement>(elseIfStatement.ThenStatements[0]);
        Assert.IsInstanceOfType<IecAssignmentStatement>(elseIfStatement.ElseStatements[0]);
    }

    [TestMethod]
    public void CreateFromSourceText_MapsFunctionResultAssignment_ToReturnStatement()
    {
        var compilationUnit = IecFrontendCompilationUnitFactory.CreateFromSourceText(
            "METHOD PRIVATE Compute : LREAL;\nVAR\nValue : LREAL;\nEND_VAR",
            "Value := 1;\nCompute := Value;");

        Assert.AreEqual(IecAccessibility.Private, compilationUnit.Accessibility);
        Assert.AreEqual(2, compilationUnit.Statements.Count);
        Assert.IsInstanceOfType<IecAssignmentStatement>(compilationUnit.Statements[0]);
        Assert.IsInstanceOfType<IecReturnStatement>(compilationUnit.Statements[1]);
    }
}
