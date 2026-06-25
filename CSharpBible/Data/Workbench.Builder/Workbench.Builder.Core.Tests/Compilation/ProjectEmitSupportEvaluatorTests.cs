using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Workbench.Builder.Core.Models.Compilation;
using Workbench.Builder.Core.Models.Diagnostics;
using Workbench.Builder.Core.Models.Inspection;
using Workbench.Builder.Core.Models.Projects;
using Workbench.Builder.Core.Models.References;
using Workbench.Builder.Core.Services.Compilation;

namespace Workbench.Builder.Core.Tests.Compilation;

/// <summary>
/// Verifies first-slice V1.2 emit classification for inspected projects.
/// </summary>
[TestClass]
public class ProjectEmitSupportEvaluatorTests
{
    /// <summary>
    /// Verifies that a default library-shaped project is emit-enabled as a library.
    /// </summary>
    [TestMethod]
    public void Evaluate_WhenProjectIsLibrary_ReturnsLibraryEmitSupport()
    {
        var evaluator = new ProjectEmitSupportEvaluator();
        ProjectInspectionResult inspectionResult = CreateInspectionResult(outputType: null, isSdkStyle: true, isTestProject: false);

        ProjectEmitSupport emitSupport = evaluator.Evaluate(inspectionResult);

        Assert.IsTrue(emitSupport.CanEmit);
        Assert.AreEqual(ProjectEmitKind.Library, emitSupport.EmitKind);
    }

    /// <summary>
    /// Verifies that an executable console-shaped project is emit-enabled as an executable.
    /// </summary>
    [TestMethod]
    public void Evaluate_WhenProjectIsConsoleExecutable_ReturnsExecutableEmitSupport()
    {
        var evaluator = new ProjectEmitSupportEvaluator();
        ProjectInspectionResult inspectionResult = CreateInspectionResult(outputType: "Exe", isSdkStyle: true, isTestProject: false);

        ProjectEmitSupport emitSupport = evaluator.Evaluate(inspectionResult);

        Assert.IsTrue(emitSupport.CanEmit);
        Assert.AreEqual(ProjectEmitKind.Executable, emitSupport.EmitKind);
    }

    /// <summary>
    /// Verifies that test projects remain non-emit in the first V1.2 slice.
    /// </summary>
    [TestMethod]
    public void Evaluate_WhenProjectIsTestProject_ReturnsNonEmitSupport()
    {
        var evaluator = new ProjectEmitSupportEvaluator();
        ProjectInspectionResult inspectionResult = CreateInspectionResult(outputType: "Exe", isSdkStyle: true, isTestProject: true);

        ProjectEmitSupport emitSupport = evaluator.Evaluate(inspectionResult);

        Assert.IsFalse(emitSupport.CanEmit);
        Assert.AreEqual(ProjectEmitKind.None, emitSupport.EmitKind);
        StringAssert.Contains(emitSupport.Reason, "not emitted");
    }

    /// <summary>
    /// Verifies that non-SDK-style projects remain non-emit in the first V1.2 slice.
    /// </summary>
    [TestMethod]
    public void Evaluate_WhenProjectIsNotSdkStyle_ReturnsNonEmitSupport()
    {
        var evaluator = new ProjectEmitSupportEvaluator();
        ProjectInspectionResult inspectionResult = CreateInspectionResult(outputType: "Exe", isSdkStyle: false, isTestProject: false);

        ProjectEmitSupport emitSupport = evaluator.Evaluate(inspectionResult);

        Assert.IsFalse(emitSupport.CanEmit);
        Assert.AreEqual(ProjectEmitKind.None, emitSupport.EmitKind);
        StringAssert.Contains(emitSupport.Reason, "Only SDK-style projects are supported");
    }

    /// <summary>
    /// Verifies that unsupported output types remain visible as non-emit decisions.
    /// </summary>
    [TestMethod]
    public void Evaluate_WhenOutputTypeIsUnsupported_ReturnsNonEmitSupport()
    {
        var evaluator = new ProjectEmitSupportEvaluator();
        ProjectInspectionResult inspectionResult = CreateInspectionResult(outputType: "WinExe", isSdkStyle: true, isTestProject: false);

        ProjectEmitSupport emitSupport = evaluator.Evaluate(inspectionResult);

        Assert.IsFalse(emitSupport.CanEmit);
        Assert.AreEqual(ProjectEmitKind.None, emitSupport.EmitKind);
        StringAssert.Contains(emitSupport.Reason, "WinExe");
    }

    private static ProjectInspectionResult CreateInspectionResult(string? outputType, bool isSdkStyle, bool isTestProject)
    {
        BuildProjectInfo project = new(
            projectFilePath: @"C:\Temp\Sample\Sample.csproj",
            projectDirectory: @"C:\Temp\Sample",
            assemblyName: "Sample",
            rootNamespace: "Sample",
            targetFramework: "net10.0",
            outputType: outputType,
            langVersion: "preview",
            nullable: "enable",
            defineConstants: "TRACE;DEBUG",
            implicitUsings: "enable",
            configuration: "Debug",
            runtimeIdentifier: null,
            outputPath: @"bin\Debug\net10.0\",
            intermediateOutputPath: @"obj\Debug\net10.0\",
            isSdkStyle: isSdkStyle,
            isPackable: false);

        return new ProjectInspectionResult(
            project,
            Array.Empty<CompileItemInfo>(),
            Array.Empty<ProjectReferenceInfo>(),
            Array.Empty<PackageReferenceInfo>(),
            Array.Empty<ResolvedReferenceInfo>(),
            new[] { new BuildDiagnostic(BuildDiagnosticSeverity.Information, "WB2000", "Sample diagnostic.") },
            isTestProject);
    }
}
