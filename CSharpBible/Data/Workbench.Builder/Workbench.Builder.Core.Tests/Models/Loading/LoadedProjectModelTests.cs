using Microsoft.VisualStudio.TestTools.UnitTesting;
using Workbench.Builder.Core.Models.Diagnostics;
using Workbench.Builder.Core.Models.Loading;
using Workbench.Builder.Core.Models.Projects;

namespace Workbench.Builder.Core.Tests.Models.Loading;

/// <summary>
/// Verifies the immutable property mapping behavior of <see cref="LoadedProjectModel"/>.
/// </summary>
[TestClass]
public class LoadedProjectModelTests
{
    /// <summary>
    /// Verifies that the constructor preserves all supplied values and references.
    /// </summary>
    [TestMethod]
    public void Constructor_WhenAllValuesAreProvided_AssignsAllProperties()
    {
        ProjectLoadRequest request = new(
            projectFilePath: @"C:\Projects\Sample\Sample.csproj",
            configuration: "Release",
            targetFramework: "net10.0",
            runtimeIdentifier: "win-x64");

        ProjectPropertySet properties = new(
            assemblyName: "Sample.Assembly",
            rootNamespace: "Sample.Namespace",
            targetFramework: "net10.0",
            outputType: "Exe",
            langVersion: "preview",
            nullable: "enable",
            defineConstants: "TRACE;DEBUG",
            implicitUsings: "enable",
            isPackable: "false",
            isTestProject: "true",
            configuration: "Release",
            runtimeIdentifier: "win-x64",
            outputPath: @"bin\Release\net10.0\",
            intermediateOutputPath: @"obj\Release\net10.0\",
            projectAssetsFile: @"obj\project.assets.json");

        CompileItemInfo[] compileItems =
        [
            new CompileItemInfo("Program.cs", @"C:\Projects\Sample\Program.cs", true),
        ];

        ProjectReferenceInfo[] projectReferences =
        [
            new ProjectReferenceInfo("..\\Shared\\Shared.csproj", @"C:\Projects\Shared\Shared.csproj", true),
        ];

        PackageReferenceInfo[] packageReferences =
        [
            new PackageReferenceInfo("Sample.Package", "1.2.3", "all"),
        ];

        BuildDiagnostic[] diagnostics =
        [
            new BuildDiagnostic(BuildDiagnosticSeverity.Warning, "WB1999", "Sample warning", request.ProjectFilePath, 12, 34),
        ];

        LoadedProjectModel result = new(
            request: request,
            projectFilePath: @"C:\Projects\Sample\Sample.csproj",
            projectDirectory: @"C:\Projects\Sample",
            properties: properties,
            compileItems: compileItems,
            projectReferences: projectReferences,
            packageReferences: packageReferences,
            isSdkStyle: true,
            diagnostics: diagnostics);

        Assert.AreSame(request, result.Request);
        Assert.AreEqual(@"C:\Projects\Sample\Sample.csproj", result.ProjectFilePath);
        Assert.AreEqual(@"C:\Projects\Sample", result.ProjectDirectory);
        Assert.AreSame(properties, result.Properties);
        Assert.AreSame(compileItems, result.CompileItems);
        Assert.AreSame(projectReferences, result.ProjectReferences);
        Assert.AreSame(packageReferences, result.PackageReferences);
        Assert.IsTrue(result.IsSdkStyle);
        Assert.AreSame(diagnostics, result.Diagnostics);
    }

    /// <summary>
    /// Verifies that empty collections and a non-SDK-style flag are preserved unchanged.
    /// </summary>
    [TestMethod]
    public void Constructor_WhenCollectionsAreEmpty_PreservesEmptyCollectionsAndFlags()
    {
        ProjectLoadRequest request = new(@"C:\Projects\Legacy\Legacy.csproj");
        ProjectPropertySet properties = new(
            assemblyName: "Legacy.Assembly",
            rootNamespace: "Legacy.Namespace",
            targetFramework: string.Empty,
            outputType: null,
            langVersion: null,
            nullable: null,
            defineConstants: null,
            implicitUsings: null,
            isPackable: null,
            isTestProject: null,
            configuration: null,
            runtimeIdentifier: null,
            outputPath: null,
            intermediateOutputPath: null,
            projectAssetsFile: null);

        CompileItemInfo[] compileItems = [];
        ProjectReferenceInfo[] projectReferences = [];
        PackageReferenceInfo[] packageReferences = [];
        BuildDiagnostic[] diagnostics = [];

        LoadedProjectModel result = new(
            request: request,
            projectFilePath: @"C:\Projects\Legacy\Legacy.csproj",
            projectDirectory: @"C:\Projects\Legacy",
            properties: properties,
            compileItems: compileItems,
            projectReferences: projectReferences,
            packageReferences: packageReferences,
            isSdkStyle: false,
            diagnostics: diagnostics);

        Assert.AreSame(request, result.Request);
        Assert.AreSame(properties, result.Properties);
        Assert.AreEqual(0, result.CompileItems.Count);
        Assert.AreEqual(0, result.ProjectReferences.Count);
        Assert.AreEqual(0, result.PackageReferences.Count);
        Assert.IsFalse(result.IsSdkStyle);
        Assert.AreEqual(0, result.Diagnostics.Count);
    }
}
