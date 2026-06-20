using Microsoft.VisualStudio.TestTools.UnitTesting;
using Workbench.Builder.Core.Models.Projects;

namespace Workbench.Builder.Core.Tests.Models.Projects;

/// <summary>
/// Verifies the immutable property mapping behavior of <see cref="ProjectPropertySet"/>.
/// </summary>
[TestClass]
public class ProjectPropertySetTests
{
    /// <summary>
    /// Verifies that the constructor preserves all supplied evaluated project properties.
    /// </summary>
    [TestMethod]
    public void Constructor_WhenAllValuesAreProvided_AssignsAllProperties()
    {
        ProjectPropertySet result = new(
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

        Assert.AreEqual("Sample.Assembly", result.AssemblyName);
        Assert.AreEqual("Sample.Namespace", result.RootNamespace);
        Assert.AreEqual("net10.0", result.TargetFramework);
        Assert.AreEqual("Exe", result.OutputType);
        Assert.AreEqual("preview", result.LangVersion);
        Assert.AreEqual("enable", result.Nullable);
        Assert.AreEqual("TRACE;DEBUG", result.DefineConstants);
        Assert.AreEqual("enable", result.ImplicitUsings);
        Assert.AreEqual("false", result.IsPackable);
        Assert.AreEqual("true", result.IsTestProject);
        Assert.AreEqual("Release", result.Configuration);
        Assert.AreEqual("win-x64", result.RuntimeIdentifier);
        Assert.AreEqual(@"bin\Release\net10.0\", result.OutputPath);
        Assert.AreEqual(@"obj\Release\net10.0\", result.IntermediateOutputPath);
        Assert.AreEqual(@"obj\project.assets.json", result.ProjectAssetsFile);
    }

    /// <summary>
    /// Verifies that optional constructor arguments can remain null without being transformed.
    /// </summary>
    [TestMethod]
    public void Constructor_WhenOptionalValuesAreNull_PreservesNullOptionals()
    {
        ProjectPropertySet result = new(
            assemblyName: "Sample.Assembly",
            rootNamespace: "Sample.Namespace",
            targetFramework: "net8.0",
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

        Assert.AreEqual("Sample.Assembly", result.AssemblyName);
        Assert.AreEqual("Sample.Namespace", result.RootNamespace);
        Assert.AreEqual("net8.0", result.TargetFramework);
        Assert.IsNull(result.OutputType);
        Assert.IsNull(result.LangVersion);
        Assert.IsNull(result.Nullable);
        Assert.IsNull(result.DefineConstants);
        Assert.IsNull(result.ImplicitUsings);
        Assert.IsNull(result.IsPackable);
        Assert.IsNull(result.IsTestProject);
        Assert.IsNull(result.Configuration);
        Assert.IsNull(result.RuntimeIdentifier);
        Assert.IsNull(result.OutputPath);
        Assert.IsNull(result.IntermediateOutputPath);
        Assert.IsNull(result.ProjectAssetsFile);
    }
}
