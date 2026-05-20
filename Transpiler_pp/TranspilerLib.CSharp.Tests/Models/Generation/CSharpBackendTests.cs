using TranspilerLib.CSharp.Models.Generation;
using TranspilerLib.IEC.Models.Ast;

using System;

namespace TranspilerLib.CSharp.Models.Generation.Tests;

/// <summary>
/// Validates the C# backend bridge that consumes the shared semantic model.
/// </summary>
[TestClass]
public class CSharpBackendTests
{
    private static IecCompilationUnit CreateCompilationUnit(
        IecArtifactKind artifactKind = IecArtifactKind.Function,
        IecAccessibility accessibility = IecAccessibility.Public,
        bool? isStatic = null,
        bool isPartial = false)
    {
        return new IecCompilationUnit(
        [
            new IecVariableDeclaration("Input", "LREAL", IecDeclarationSection.Input),
            new IecVariableDeclaration("Memory", "LREAL", IecDeclarationSection.Instance, "0.5"),
            new IecVariableDeclaration("localValue", "INT", IecDeclarationSection.Local),
        ],
        [
            new IecAssignmentStatement(new IecIdentifierExpression("localValue"), new IecLiteralExpression(1)),
            new IecReturnStatement(new IecIdentifierExpression("Memory")),
        ],
        new IecArtifactMetadata(artifactKind, accessibility, isStatic, isPartial));
    }

    [TestMethod]
    public void EmitMethod_UsesSharedSemanticsCompilationUnit()
    {
        var compilationUnit = CreateCompilationUnit();

        var result = CSharpBackend.EmitMethod(compilationUnit, "Compute");

        StringAssert.Contains(result, "private static double Memory = 0.5;");
        StringAssert.Contains(result, "public static double Compute(double Input)");
        StringAssert.Contains(result, "int localValue;");
        StringAssert.Contains(result, "localValue = 1;");
        StringAssert.Contains(result, "return Memory;");
    }

    [TestMethod]
    public void EmitMethod_UsesConfiguredNamespaceAndTypeName()
    {
        var result = CSharpBackend.EmitMethod(
            CreateCompilationUnit(),
            "Compute",
            new CSharpBackendOptions()
            {
                NamespaceName = "TranspilerLib.Generated",
                TypeName = "ComputeAlignmentProgram",
            });

        StringAssert.Contains(result, "namespace TranspilerLib.Generated");
        StringAssert.Contains(result, "public static class ComputeAlignmentProgram");
        Assert.IsFalse(result.Contains("public static class GeneratedIecProgram", StringComparison.Ordinal));
    }

    [TestMethod]
    public void EmitMethod_UsesFunctionBlockArtifactKind()
    {
        var result = CSharpBackend.EmitMethod(CreateCompilationUnit(IecArtifactKind.FunctionBlock), "Compute");

        StringAssert.Contains(result, "public sealed class GeneratedIecProgram");
        StringAssert.Contains(result, "public double Compute(double Input)");
        Assert.IsFalse(result.Contains("public static double Compute(double Input)", StringComparison.Ordinal));
    }

    [TestMethod]
    public void EmitMethod_UsesProgramArtifactKind_ForVoidEntryPoint()
    {
        var compilationUnit = new IecCompilationUnit(
            [new IecVariableDeclaration("Input", "LREAL", IecDeclarationSection.Input)],
            [new IecAssignmentStatement(new IecIdentifierExpression("Input"), new IecLiteralExpression(1.0))],
            new IecArtifactMetadata(IecArtifactKind.Program));

        var result = CSharpBackend.EmitMethod(compilationUnit, "RunCycle");

        StringAssert.Contains(result, "public sealed class GeneratedIecProgram");
        StringAssert.Contains(result, "public void RunCycle(double Input)");
        Assert.IsFalse(result.Contains("public static void RunCycle(double Input)", StringComparison.Ordinal));
    }

    [TestMethod]
    public void EmitMethod_MapsInternalAccessibility_FromSharedSemantics()
    {
        var result = CSharpBackend.EmitMethod(CreateCompilationUnit(accessibility: IecAccessibility.Internal), "Compute");

        StringAssert.Contains(result, "internal static class GeneratedIecProgram");
        StringAssert.Contains(result, "internal static double Compute(double Input)");
        Assert.IsFalse(result.Contains("public static class GeneratedIecProgram", StringComparison.Ordinal));
    }

    [TestMethod]
    public void EmitMethod_MapsNonStaticTrait_FromSharedSemantics()
    {
        var result = CSharpBackend.EmitMethod(CreateCompilationUnit(isStatic: false), "Compute");

        StringAssert.Contains(result, "public sealed class GeneratedIecProgram");
        StringAssert.Contains(result, "public double Compute(double Input)");
        Assert.IsFalse(result.Contains("public static double Compute(double Input)", StringComparison.Ordinal));
    }

    [TestMethod]
    public void EmitMethod_MapsPartialTrait_FromSharedSemantics()
    {
        var result = CSharpBackend.EmitMethod(CreateCompilationUnit(isPartial: true), "Compute");

        StringAssert.Contains(result, "public static partial class GeneratedIecProgram");
        StringAssert.Contains(result, "public static double Compute(double Input)");
    }
}
