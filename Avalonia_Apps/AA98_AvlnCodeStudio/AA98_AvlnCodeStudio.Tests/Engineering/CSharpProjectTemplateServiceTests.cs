using AA98_AvlnCodeStudio.Model.Workspaces;
using AA98_AvlnCodeStudio.Base.Testing.Models;
using AA98_AvlnCodeStudio.Base.Testing.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.Tests.Engineering;

[TestClass]
public sealed class CSharpProjectTemplateServiceTests
{
    [TestMethod]
    public void CreateConsoleProject_WritesRunnableProjectAndSource()
    {
        WithTemporaryDirectory(root =>
        {
            string projectPath = Path.Combine(root, "HelloWorld.csproj");

            string createdPath = new CSharpProjectTemplateService().CreateConsoleProject(projectPath);

            Assert.AreEqual(Path.GetFullPath(projectPath), createdPath);
            StringAssert.Contains(File.ReadAllText(projectPath), "<OutputType>Exe</OutputType>");
            StringAssert.Contains(
                File.ReadAllText(projectPath),
                $"<TargetFramework>net{Environment.Version.Major}.0</TargetFramework>");
            StringAssert.Contains(File.ReadAllText(Path.Combine(root, "Program.cs")), "Console.WriteLine(\"Hello, World!\");");
            CSharpWorkspace workspace = new CSharpWorkspaceLoader().Load(createdPath);
            Assert.AreEqual(1, workspace.Projects.Count);
            Assert.AreEqual("HelloWorld", workspace.Projects[0].Name);
        });
    }

    [TestMethod]
    public void ReloadTemplates_LoadsSavedJsonTemplateAndCreatesItsFiles()
    {
        WithTemporaryDirectory(root =>
        {
            string templatesDirectory = Path.Combine(root, "templates");
            string outputDirectory = Path.Combine(root, "output");
            Directory.CreateDirectory(templatesDirectory);
            Directory.CreateDirectory(outputDirectory);
            CSharpProjectTemplateService service = new([templatesDirectory]);
            CSharpProjectTemplateDefinition definition = new()
            {
                Id = "custom-app",
                Name = "Custom Application",
                Description = "A custom JSON template.",
                DefaultProjectFileName = "{{ProjectName}}.csproj",
                ProjectFileContent = "<Project><TargetFramework>{{TargetFramework}}</TargetFramework></Project>",
                Files =
                [
                    new CSharpProjectTemplateFile
                    {
                        Path = "src/{{ProjectNameIdentifier}}.cs",
                        Content = "namespace {{ProjectNameIdentifier}};",
                    },
                ],
            };
            string templatePath = Path.Combine(templatesDirectory, "custom-app.json");
            service.SaveTemplate(definition, templatePath);

            service.ReloadTemplates();
            string projectPath = service.CreateProject("custom-app", Path.Combine(outputDirectory, "My-App.csproj"), "net9.0");
            service.SaveTemplate(new CSharpProjectTemplateDefinition
            {
                Id = "custom-app",
                Name = "Updated Custom Application",
                DefaultProjectFileName = "{{ProjectName}}.csproj",
                ProjectFileContent = "<Project />",
            }, templatePath);
            var templates = service.ReloadTemplates();

            Assert.IsTrue(templates.Any(template => template.Id == "custom-app"));
            Assert.AreEqual("Updated Custom Application", templates.Single(template => template.Id == "custom-app").Name);
            Assert.AreEqual("<Project><TargetFramework>net9.0</TargetFramework></Project>", File.ReadAllText(projectPath));
            Assert.AreEqual(
                "namespace My_App;",
                File.ReadAllText(Path.Combine(outputDirectory, "src", "My_App.cs")));
        });
    }

    [TestMethod]
    public void ReloadTemplates_RejectsUnsafeJsonAndKeepsCurrentCatalog()
    {
        WithTemporaryDirectory(root =>
        {
            CSharpProjectTemplateService service = new([root]);
            int currentTemplateCount = service.Templates.Count;
            File.WriteAllText(
                Path.Combine(root, "unsafe.json"),
                JsonSerializer.Serialize(new CSharpProjectTemplateDefinition
                {
                    Id = "unsafe",
                    Name = "Unsafe",
                    ProjectFileContent = "<Project />",
                    Files =
                    [
                        new CSharpProjectTemplateFile
                        {
                            Path = "..\\outside.txt",
                            Content = "not allowed",
                        },
                    ],
                }));

            Assert.ThrowsExactly<InvalidDataException>(() => service.ReloadTemplates());
            Assert.AreEqual(currentTemplateCount, service.Templates.Count);
            Assert.IsFalse(service.Templates.Any(template => template.Id == "unsafe"));
        });
    }

    [TestMethod]
    public void CreateConsoleProject_RejectsExistingGeneratedFilesWithoutOverwriting()
    {
        WithTemporaryDirectory(root =>
        {
            string projectPath = Path.Combine(root, "Existing.csproj");
            string sourcePath = Path.Combine(root, "Program.cs");
            const string existingSource = "existing source";
            File.WriteAllText(sourcePath, existingSource);

            Assert.ThrowsExactly<IOException>(() =>
                new CSharpProjectTemplateService().CreateConsoleProject(projectPath));

            Assert.IsFalse(File.Exists(projectPath));
            Assert.AreEqual(existingSource, File.ReadAllText(sourcePath));
        });
    }

    [TestMethod]
    public void CreateConsoleProject_RejectsUnsupportedPathsAndFrameworks()
    {
        WithTemporaryDirectory(root =>
        {
            CSharpProjectTemplateService service = new();

            Assert.ThrowsExactly<ArgumentException>(() =>
                service.CreateConsoleProject(Path.Combine(root, "Project.txt")));
            Assert.ThrowsExactly<ArgumentException>(() =>
                service.CreateConsoleProject(Path.Combine(root, "Project.csproj"), "net10"));
            Assert.IsFalse(File.Exists(Path.Combine(root, "Project.csproj")));
        });
    }

    [TestMethod]
    public void CreateClassLibraryProject_WritesLibraryProjectAndSource()
    {
        WithTemporaryDirectory(root =>
        {
            string projectPath = Path.Combine(root, "Shared.csproj");

            new CSharpProjectTemplateService().CreateClassLibraryProject(projectPath, "net8.0");

            StringAssert.Contains(File.ReadAllText(projectPath), "<TargetFramework>net8.0</TargetFramework>");
            Assert.IsFalse(File.ReadAllText(projectPath).Contains("<OutputType>", StringComparison.Ordinal));
            StringAssert.Contains(
                File.ReadAllText(Path.Combine(root, "Class1.cs")),
                "public sealed class Class1");
            Assert.AreEqual(1, new CSharpWorkspaceLoader().Load(projectPath).Projects.Count);
        });
    }

    [TestMethod]
    public void CreateTestProject_WritesRunnableMSTestProject()
    {
        WithTemporaryDirectory(root =>
        {
            string projectPath = Path.Combine(root, "Sample.Tests.csproj");

            new CSharpProjectTemplateService().CreateTestProject(projectPath, "net9.0");

            string projectContent = File.ReadAllText(projectPath);
            StringAssert.Contains(projectContent, "<TargetFramework>net9.0</TargetFramework>");
            StringAssert.Contains(projectContent, "<IsTestProject>true</IsTestProject>");
            StringAssert.Contains(projectContent, "Microsoft.NET.Test.Sdk");
            StringAssert.Contains(projectContent, "MSTest");
            StringAssert.Contains(
                File.ReadAllText(Path.Combine(root, "SampleTests.cs")),
                "[TestMethod]");
            Assert.AreEqual(1, new CSharpWorkspaceLoader().Load(projectPath).Projects.Count);
        });
    }

    [TestMethod]
    public async Task CreateTestProject_GeneratedProjectExecutesItsExampleTest()
    {
        await WithTemporaryDirectoryAsync(async root =>
        {
            string projectPath = Path.Combine(root, "Sample.Tests.csproj");
            new CSharpProjectTemplateService().CreateTestProject(projectPath);

            TestRunSummary result = await new DotNetTestExecutionService().RunTestsAsync(new TestRunRequest
            {
                WorkspaceRootPath = root,
                ProjectPath = projectPath,
            });

            Assert.AreEqual(TestRunOutcome.Passed, result.Outcome);
            Assert.AreEqual(1, result.TotalCount);
            Assert.AreEqual(1, result.PassedCount);
        });
    }

    [TestMethod]
    public async Task CreateConsoleAndClassLibraryProjects_BuildSuccessfully()
    {
        await WithTemporaryDirectoryAsync(async root =>
        {
            string consoleDirectory = Path.Combine(root, "console");
            string libraryDirectory = Path.Combine(root, "library");
            Directory.CreateDirectory(consoleDirectory);
            Directory.CreateDirectory(libraryDirectory);
            CSharpProjectTemplateService service = new();
            string consoleProject = service.CreateConsoleProject(Path.Combine(consoleDirectory, "ConsoleApp.csproj"));
            string libraryProject = service.CreateClassLibraryProject(Path.Combine(libraryDirectory, "Shared.csproj"));

            await Task.WhenAll(BuildProjectAsync(consoleProject), BuildProjectAsync(libraryProject));
        });
    }

    private static void WithTemporaryDirectory(Action<string> action)
    {
        string root = Path.Combine(Path.GetTempPath(), $"AA98_CSharpProjectTemplate_{Guid.NewGuid():N}");
        Directory.CreateDirectory(root);
        try
        {
            action(root);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    private static async Task WithTemporaryDirectoryAsync(Func<string, Task> action)
    {
        string root = Path.Combine(Path.GetTempPath(), $"AA98_CSharpProjectTemplate_{Guid.NewGuid():N}");
        Directory.CreateDirectory(root);
        try
        {
            await action(root);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    private static async Task BuildProjectAsync(string projectPath)
    {
        ProcessStartInfo startInfo = new("dotnet")
        {
            WorkingDirectory = Path.GetDirectoryName(projectPath)!,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };
        startInfo.ArgumentList.Add("build");
        startInfo.ArgumentList.Add(projectPath);
        startInfo.ArgumentList.Add("--nologo");
        startInfo.ArgumentList.Add("--verbosity");
        startInfo.ArgumentList.Add("quiet");
        startInfo.Environment.Remove("MSBuildSDKsPath");
        startInfo.Environment.Remove("MSBuildExtensionsPath");
        startInfo.Environment.Remove("MSBuildExtensionsPath32");
        startInfo.Environment.Remove("MSBuildToolsPath");
        startInfo.Environment.Remove("DOTNET_MSBUILD_SDK_RESOLVER_SDKS_DIR");
        startInfo.Environment.Remove("DOTNET_MSBUILD_SDK_RESOLVER_CLI_DIR");

        using Process process = Process.Start(startInfo)
            ?? throw new InvalidOperationException("The generated project build process could not be started.");
        Task<string> outputTask = process.StandardOutput.ReadToEndAsync();
        Task<string> errorTask = process.StandardError.ReadToEndAsync();
        await process.WaitForExitAsync();
        Assert.AreEqual(0, process.ExitCode, $"{await outputTask}{Environment.NewLine}{await errorTask}");
    }
}
