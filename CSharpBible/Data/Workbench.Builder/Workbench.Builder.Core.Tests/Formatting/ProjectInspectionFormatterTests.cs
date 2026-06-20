using Microsoft.VisualStudio.TestTools.UnitTesting;
using Workbench.Builder.Core.Models.Diagnostics;
using Workbench.Builder.Core.Models.Inspection;
using Workbench.Builder.Core.Models.Projects;
using Workbench.Builder.Core.Models.References;
using Workbench.Builder.Core.Services.Formatting;

namespace Workbench.Builder.Core.Tests.Formatting;

/// <summary>
/// Verifies plain-text and JSON formatting for project inspection results.
/// </summary>
[TestClass]
public class ProjectInspectionFormatterTests
{
    /// <summary>
    /// Verifies that the plain-text formatter emits the expected key sections.
    /// </summary>
    [TestMethod]
    public void Format_PlainText_ReturnsReadableReport()
    {
        ProjectInspectionFormatter formatter = new();
        ProjectInspectionResult result = CreateResult();

        string output = formatter.Format(result, ProjectInspectionOutputFormat.PlainText);

        StringAssert.Contains(output, "Project Inspection");
        StringAssert.Contains(output, "Assembly Name: Sample.Assembly");
        StringAssert.Contains(output, "Compile Items (1)");
        StringAssert.Contains(output, "Resolved References (1)");
        StringAssert.Contains(output, "Diagnostics (1)");
        StringAssert.Contains(output, @"C:\Temp\Sample\Sample.csproj(12,34): warning WB1999: Sample warning");
    }

    /// <summary>
    /// Verifies that the JSON formatter emits a structured JSON document with enum names.
    /// </summary>
    [TestMethod]
    public void Format_Json_ReturnsStructuredJson()
    {
        ProjectInspectionFormatter formatter = new();
        ProjectInspectionResult result = CreateResult();

        string output = formatter.Format(result, ProjectInspectionOutputFormat.Json);

        StringAssert.Contains(output, "\"Project\"");
        StringAssert.Contains(output, "\"AssemblyName\": \"Sample.Assembly\"");
        StringAssert.Contains(output, "\"Kind\": \"Framework\"");
        StringAssert.Contains(output, "\"Severity\": \"Warning\"");
    }

    private static ProjectInspectionResult CreateResult()
    {
        BuildProjectInfo project = new(
            projectFilePath: @"C:\Temp\Sample\Sample.csproj",
            projectDirectory: @"C:\Temp\Sample",
            assemblyName: "Sample.Assembly",
            rootNamespace: "Sample.Namespace",
            targetFramework: "net10.0",
            outputType: "Exe",
            langVersion: "preview",
            nullable: "enable",
            defineConstants: "TRACE;DEBUG",
            implicitUsings: "enable",
            configuration: "Debug",
            runtimeIdentifier: null,
            outputPath: @"bin\Debug\net10.0\",
            intermediateOutputPath: @"obj\Debug\net10.0\",
            isSdkStyle: true,
            isPackable: false);

        return new ProjectInspectionResult(
            project,
            new[]
            {
                new CompileItemInfo("Program.cs", @"C:\Temp\Sample\Program.cs", true),
            },
            new[]
            {
                new ProjectReferenceInfo("..\\Shared\\Shared.csproj", @"C:\Temp\Shared\Shared.csproj", true),
            },
            new[]
            {
                new PackageReferenceInfo("Sample.Package", "1.2.3", "all"),
            },
            new[]
            {
                new ResolvedReferenceInfo(ReferenceKind.Framework, "System.Runtime", "FrameworkReference", @"C:\Ref\System.Runtime.dll", true),
            },
            new[]
            {
                new BuildDiagnostic(BuildDiagnosticSeverity.Warning, "WB1999", "Sample warning", @"C:\Temp\Sample\Sample.csproj", 12, 34),
            },
            isTestProject: false);
    }
}
