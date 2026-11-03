using AA98_AvlnCodeStudio.UI.Services;
using AA98_AvlnCodeStudio.Model.Workspaces;
using AppKomponentBaseLib.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.Tests.Engineering;

[TestClass]
public sealed class CSharpSyntaxDiagnosticServiceTests
{
    [TestMethod]
    public void Analyze_InvalidCSharp_ReportsRoslynDiagnosticWithSourceLocation()
    {
        const string filePath = @"C:\workspace\Sample.cs";
        const string source = "class Sample\n{\n    void Run() { int value = 1 }\n}";

        Diagnostic diagnostic = new CSharpSyntaxDiagnosticService()
            .Analyze(source, filePath)
            .Single(item => item.Code == "CS1002");

        Assert.AreEqual(DiagnosticSeverity.Error, diagnostic.Severity);
        Assert.AreEqual(filePath, diagnostic.SourcePath);
        Assert.AreEqual(3, diagnostic.LineNumber);
        Assert.IsNotNull(diagnostic.ColumnNumber);
    }

    [TestMethod]
    public void Analyze_ValidCSharp_ReturnsNoSyntaxDiagnostics()
    {
        var diagnostics = new CSharpSyntaxDiagnosticService()
            .Analyze("class Sample { void Run() { int value = 1; } }", null);

        Assert.AreEqual(0, diagnostics.Count);
    }

    [TestMethod]
    public async Task AnalyzeAsync_ProjectWorkspaceReportsSemanticDiagnosticsForEditedFile()
    {
        string root = Path.Combine(Path.GetTempPath(), $"AA98_RoslynWorkspace_{Guid.NewGuid():N}");
        Directory.CreateDirectory(root);
        try
        {
            string projectPath = Path.Combine(root, "Sample.csproj");
            string sourcePath = Path.Combine(root, "Program.cs");
            await File.WriteAllTextAsync(projectPath,
                $"<Project Sdk=\"Microsoft.NET.Sdk\"><PropertyGroup><TargetFramework>net{Environment.Version.Major}.0</TargetFramework><ImplicitUsings>disable</ImplicitUsings></PropertyGroup></Project>");
            await File.WriteAllTextAsync(sourcePath,
                "public static class ReferencedType { public static int Value => 1; }");

            ProcessStartInfo restoreStartInfo = new("dotnet")
            {
                WorkingDirectory = AppContext.BaseDirectory,
                ArgumentList = { "restore", projectPath, "--ignore-failed-sources", "--verbosity", "quiet" },
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
            };
            ClearInheritedMsBuildEnvironment(restoreStartInfo);
            using Process restore = Process.Start(restoreStartInfo)
                ?? throw new InvalidOperationException("Could not start dotnet restore for the semantic diagnostics test.");
            Task<string> standardOutput = restore.StandardOutput.ReadToEndAsync();
            Task<string> standardError = restore.StandardError.ReadToEndAsync();
            await restore.WaitForExitAsync();
            if (restore.ExitCode != 0)
            {
                Assert.Fail($"Temporary C# project restore failed: {await standardOutput}{await standardError}");
            }

            AA98_AvlnCodeStudio.Model.Workspaces.CSharpWorkspace workspace =
                new AA98_AvlnCodeStudio.Model.Workspaces.CSharpWorkspaceLoader().Load(projectPath);
            const string editedSource = """
                public static class ReferencedType { public static int Value => 1; }
                public class Consumer
                {
                    public string Read() => ReferencedType.Value;
                }
                """;

            var diagnostics = await new CSharpSyntaxDiagnosticService()
                .AnalyzeAsync(editedSource, sourcePath, workspace);

            Assert.IsTrue(diagnostics.Any(diagnostic =>
                diagnostic.Code == "CS0029"
                && diagnostic.SourcePath == sourcePath
                && diagnostic.Severity == DiagnosticSeverity.Error));
            Assert.IsFalse(diagnostics.Any(diagnostic => diagnostic.Code == "CS0103"));
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [TestMethod]
    public async Task AnalyzeAxamlMvvmAsync_ResolvesViewAndRejectsExistingOrAmbiguousDataContext()
    {
        string root = Path.Combine(Path.GetTempPath(), $"AA98_AxamlSemantic_{Guid.NewGuid():N}");
        Directory.CreateDirectory(root);
        try
        {
            string projectPath = Path.Combine(root, "Sample.csproj");
            string viewPath = Path.Combine(root, "LoginView.axaml");
            await File.WriteAllTextAsync(projectPath,
                $"<Project Sdk=\"Microsoft.NET.Sdk\"><PropertyGroup><TargetFramework>net{Environment.Version.Major}.0</TargetFramework><ImplicitUsings>enable</ImplicitUsings><Nullable>enable</Nullable></PropertyGroup></Project>");
            await File.WriteAllTextAsync(
                Path.Combine(root, "LoginView.axaml.cs"),
                """
                namespace Sample;
                public partial class LoginView
                {
                }
                public partial class ExistingContextView
                {
                    public object? DataContext { get; set; }
                    public void InitializeViewModel() => DataContext = new LoginViewModel();
                }
                public partial class ConflictingContextView
                {
                    public object? DataContext { get; set; }
                    public void InitializeViewModel() => DataContext = new LoginViewModel();
                    public void ReplaceViewModel() => DataContext = new AlternateLoginViewModel();
                }
                public partial class MappedContextView
                {
                    public object? DataContext { get; set; }
                    public void InitializeViewModel() => DataContext = new LoginViewModel();
                }
                public class LoginViewModelBase
                {
                    public int Revision { get; set; }
                }
                public sealed class LoginViewModel : LoginViewModelBase, System.ComponentModel.INotifyPropertyChanged
                {
                    public event System.ComponentModel.PropertyChangedEventHandler? PropertyChanged;
                    private string _userName = string.Empty;
                    public string ReadOnlyName => _userName;
                    public string InitOnlyName { get; init; } = string.Empty;
                    public string? OptionalName { get; set; }
                    public string UserName
                    {
                        get => _userName;
                        set
                        {
                            if (_userName == value) return;
                            _userName = value;
                            PropertyChanged?.Invoke(this, new(nameof(UserName)));
                        }
                    }
                }
                public sealed class AlternateLoginViewModel { }
                public class NonPartialView { }
                """);
            await File.WriteAllTextAsync(
                Path.Combine(root, "XmlnsDefinitionMetadata.cs"),
                """
                [assembly: Avalonia.Metadata.XmlnsDefinitionAttribute("https://example.test/viewmodels", "Sample")]

                namespace Avalonia.Metadata
                {
                    [System.AttributeUsage(System.AttributeTargets.Assembly, AllowMultiple = true)]
                    public sealed class XmlnsDefinitionAttribute(string xmlNamespace, string clrNamespace)
                        : System.Attribute
                    {
                        public string XmlNamespace { get; } = xmlNamespace;
                        public string ClrNamespace { get; } = clrNamespace;
                    }
                }
                """);

            ProcessStartInfo restoreStartInfo = new("dotnet")
            {
                WorkingDirectory = AppContext.BaseDirectory,
                ArgumentList = { "restore", projectPath, "--ignore-failed-sources", "--verbosity", "quiet" },
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
            };
            ClearInheritedMsBuildEnvironment(restoreStartInfo);
            using Process restore = Process.Start(restoreStartInfo)
                ?? throw new InvalidOperationException("Could not start dotnet restore for AXAML semantic analysis.");
            Task<string> standardOutput = restore.StandardOutput.ReadToEndAsync();
            Task<string> standardError = restore.StandardError.ReadToEndAsync();
            await restore.WaitForExitAsync();
            if (restore.ExitCode != 0)
            {
                Assert.Fail($"Temporary C# project restore failed: {await standardOutput}{await standardError}");
            }

            AA98_AvlnCodeStudio.Model.Workspaces.CSharpWorkspace workspace =
                new AA98_AvlnCodeStudio.Model.Workspaces.CSharpWorkspaceLoader().Load(projectPath);
            using CSharpSyntaxDiagnosticService service = new();
            const string markup = """
                <Window xmlns="https://github.com/avaloniaui"
                        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                        xmlns:vm="using:Sample"
                        x:Class="Sample.LoginView" />
                """;
            AxamlMvvmSemanticResolution resolved = await service.AnalyzeAxamlMvvmAsync(
                markup,
                viewPath,
                workspace);
            Assert.IsTrue(resolved.IsSuccessful, string.Join(Environment.NewLine, resolved.Diagnostics));
            Assert.AreEqual("Sample.LoginView", resolved.ViewTypeName);
            Assert.IsFalse(resolved.HasRuntimeDataContext);

            const string typedMarkup = """
                <Window xmlns="https://github.com/avaloniaui"
                        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                        xmlns:vm="using:Sample"
                        x:Class="Sample.ExistingContextView"
                        x:DataType="vm:LoginViewModel" />
                """;
            AxamlMvvmSemanticResolution existingRuntimeContext = await service.AnalyzeAxamlMvvmAsync(
                typedMarkup,
                viewPath,
                workspace);
            Assert.IsFalse(existingRuntimeContext.IsSuccessful);
            Assert.AreEqual("Sample.LoginViewModel", existingRuntimeContext.ViewModelTypeName);
            Assert.IsTrue(existingRuntimeContext.HasRuntimeDataContext);
            Assert.IsTrue(existingRuntimeContext.Diagnostics.Any(message =>
                message.Contains("already receives runtime DataContext", StringComparison.Ordinal)));
            AxamlMvvmSemanticMember existingProperty = existingRuntimeContext.ViewModelMembers
                .Single(member => member.Name == "UserName");
            Assert.AreEqual("string", existingProperty.TypeName);
            Assert.IsTrue(existingProperty.IsWritable);
            Assert.IsTrue(existingRuntimeContext.ViewModelMembers
                .Single(member => member.Name == "Revision").IsWritable);
            Assert.IsFalse(existingRuntimeContext.ViewModelMembers
                .Single(member => member.Name == "ReadOnlyName").IsWritable);
            Assert.IsFalse(existingRuntimeContext.ViewModelMembers
                .Single(member => member.Name == "InitOnlyName").IsWritable);
            AxamlMvvmSemanticMember nullableProperty = existingRuntimeContext.ViewModelMembers
                .Single(member => member.Name == "OptionalName");
            Assert.AreEqual("string?", nullableProperty.TypeName);
            Assert.IsTrue(nullableProperty.IsWritable);

            AxamlMvvmSemanticResolution mismatchedDataType = await service.AnalyzeAxamlMvvmAsync(
                typedMarkup.Replace(
                    "vm:LoginViewModel",
                    "vm:AlternateLoginViewModel",
                    StringComparison.Ordinal),
                viewPath,
                workspace);
            Assert.IsFalse(mismatchedDataType.IsSuccessful);
            Assert.IsTrue(mismatchedDataType.Diagnostics.Any(message =>
                message.Contains("does not match the runtime DataContext type", StringComparison.Ordinal)));

            AxamlMvvmSemanticResolution unresolvedDataType = await service.AnalyzeAxamlMvvmAsync(
                typedMarkup.Replace(
                    "Sample.ExistingContextView",
                    "Sample.LoginView",
                    StringComparison.Ordinal)
                    .Replace(
                        "vm:LoginViewModel",
                        "vm:MissingViewModel",
                        StringComparison.Ordinal),
                viewPath,
                workspace);
            Assert.IsFalse(unresolvedDataType.IsSuccessful);
            Assert.IsTrue(unresolvedDataType.Diagnostics.Any(message =>
                message.Contains("could not be resolved unambiguously", StringComparison.Ordinal)
                && message.Contains("(line 5, column ", StringComparison.Ordinal)));

            AxamlMvvmSemanticResolution malformedMarkup = await service.AnalyzeAxamlMvvmAsync(
                "<Window>\n  <Broken>\n</Window>",
                viewPath,
                workspace);
            Assert.IsFalse(malformedMarkup.IsSuccessful);
            Assert.IsTrue(malformedMarkup.Diagnostics.Any(message =>
                message.Contains("could not be parsed at line 3, column ", StringComparison.Ordinal)));

            AxamlMvvmSemanticResolution conflictingRuntimeContext = await service.AnalyzeAxamlMvvmAsync(
                typedMarkup.Replace(
                    "Sample.ExistingContextView",
                    "Sample.ConflictingContextView",
                    StringComparison.Ordinal),
                viewPath,
                workspace);
            Assert.IsFalse(conflictingRuntimeContext.IsSuccessful);
            Assert.IsTrue(conflictingRuntimeContext.Diagnostics.Any(message =>
                message.Contains("multiple DataContext types", StringComparison.Ordinal)));

            const string mappedNamespaceMarkup = """
                <Window xmlns="https://github.com/avaloniaui"
                        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                        xmlns:vm="https://example.test/viewmodels"
                        x:Class="Sample.MappedContextView"
                        x:DataType="{x:Type vm:LoginViewModel}" />
                """;
            AxamlMvvmSemanticResolution mappedNamespace = await service.AnalyzeAxamlMvvmAsync(
                mappedNamespaceMarkup,
                viewPath,
                workspace);
            Assert.IsTrue(mappedNamespace.Diagnostics.Any(message =>
                message.Contains("already receives runtime DataContext", StringComparison.Ordinal)));
            Assert.IsFalse(mappedNamespace.Diagnostics.Any(message =>
                message.Contains("x:DataType", StringComparison.Ordinal)));
            Assert.AreEqual("Sample.LoginViewModel", mappedNamespace.ViewModelTypeName);

            AxamlMvvmSemanticResolution assemblyQualifiedType = await service.AnalyzeAxamlMvvmAsync(
                mappedNamespaceMarkup
                    .Replace(
                        "https://example.test/viewmodels",
                        "clr-namespace:Sample;assembly=Sample",
                        StringComparison.Ordinal)
                    .Replace(
                        "{x:Type vm:LoginViewModel}",
                        "vm:LoginViewModel",
                        StringComparison.Ordinal),
                viewPath,
                workspace);
            Assert.AreEqual("Sample.LoginViewModel", assemblyQualifiedType.ViewModelTypeName);
            Assert.IsFalse(assemblyQualifiedType.Diagnostics.Any(message =>
                message.Contains("x:DataType", StringComparison.Ordinal)));

            AxamlMvvmSemanticResolution wrongAssemblyType = await service.AnalyzeAxamlMvvmAsync(
                mappedNamespaceMarkup.Replace(
                    "https://example.test/viewmodels",
                    "clr-namespace:Sample;assembly=MissingAssembly",
                    StringComparison.Ordinal),
                viewPath,
                workspace);
            Assert.AreEqual("Sample.LoginViewModel", wrongAssemblyType.ViewModelTypeName);
            Assert.IsTrue(wrongAssemblyType.Diagnostics.Any(message =>
                message.Contains("x:DataType", StringComparison.Ordinal)));

            AxamlMvvmSemanticResolution nonPartial = await service.AnalyzeAxamlMvvmAsync(
                markup.Replace("Sample.LoginView", "Sample.NonPartialView", StringComparison.Ordinal),
                viewPath,
                workspace);
            Assert.IsFalse(nonPartial.IsSuccessful);
            Assert.IsTrue(nonPartial.Diagnostics.Any(message =>
                message.Contains("is not partial", StringComparison.Ordinal)));

            string secondProjectPath = Path.Combine(root, "Additional.csproj");
            string solutionPath = Path.Combine(root, "Sample.slnx");
            await File.WriteAllTextAsync(secondProjectPath,
                $"<Project Sdk=\"Microsoft.NET.Sdk\"><PropertyGroup><TargetFramework>net{Environment.Version.Major}.0</TargetFramework></PropertyGroup></Project>");
            await File.WriteAllTextAsync(solutionPath,
                "<Solution><Project Path=\"Sample.csproj\" /><Project Path=\"Additional.csproj\" /></Solution>");
            AA98_AvlnCodeStudio.Model.Workspaces.CSharpWorkspace ambiguousWorkspace =
                new AA98_AvlnCodeStudio.Model.Workspaces.CSharpWorkspaceLoader().Load(solutionPath);
            AxamlMvvmSemanticResolution ambiguousProject = await service.AnalyzeAxamlMvvmAsync(
                markup,
                viewPath,
                ambiguousWorkspace);
            Assert.IsFalse(ambiguousProject.IsSuccessful);
            Assert.IsNull(ambiguousProject.ProjectPath);
            Assert.IsTrue(ambiguousProject.Diagnostics.Any(message =>
                message.Contains("More than one C# project could own", StringComparison.Ordinal)));

            string duplicateTypeDirectory = Path.Combine(root, "DuplicateTypes");
            Directory.CreateDirectory(duplicateTypeDirectory);
            string duplicateTypeProjectPath = Path.Combine(duplicateTypeDirectory, "DuplicateTypes.csproj");
            string mainProjectWithReference = """
                <Project Sdk="Microsoft.NET.Sdk">
                  <PropertyGroup>
                    <TargetFramework>netTARGET.0</TargetFramework>
                    <ImplicitUsings>enable</ImplicitUsings>
                    <Nullable>enable</Nullable>
                  </PropertyGroup>
                  <ItemGroup>
                    <ProjectReference Include="DuplicateTypes\DuplicateTypes.csproj" />
                  </ItemGroup>
                </Project>
                """.Replace("TARGET", Environment.Version.Major.ToString(), StringComparison.Ordinal);
            await File.WriteAllTextAsync(projectPath, mainProjectWithReference);
            await File.WriteAllTextAsync(duplicateTypeProjectPath,
                $"<Project Sdk=\"Microsoft.NET.Sdk\"><PropertyGroup><TargetFramework>net{Environment.Version.Major}.0</TargetFramework></PropertyGroup></Project>");
            await File.WriteAllTextAsync(
                Path.Combine(duplicateTypeDirectory, "DuplicateViewModel.cs"),
                """
                [assembly: Avalonia.Metadata.XmlnsDefinitionAttribute("https://example.test/viewmodels", "Sample")]
                namespace Avalonia.Metadata
                {
                    [System.AttributeUsage(System.AttributeTargets.Assembly, AllowMultiple = true)]
                    public sealed class XmlnsDefinitionAttribute(string xmlNamespace, string clrNamespace)
                        : System.Attribute { }
                }
                namespace Sample
                {
                    public sealed class LoginViewModel { }
                }
                """);
            ProcessStartInfo ambiguityRestoreInfo = new("dotnet")
            {
                WorkingDirectory = AppContext.BaseDirectory,
                ArgumentList = { "restore", projectPath, "--ignore-failed-sources", "--verbosity", "quiet" },
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
            };
            ClearInheritedMsBuildEnvironment(ambiguityRestoreInfo);
            using Process ambiguityRestore = Process.Start(ambiguityRestoreInfo)
                ?? throw new InvalidOperationException("Could not start restore for the ambiguous type fixture.");
            Task<string> ambiguityOutput = ambiguityRestore.StandardOutput.ReadToEndAsync();
            Task<string> ambiguityError = ambiguityRestore.StandardError.ReadToEndAsync();
            await ambiguityRestore.WaitForExitAsync();
            if (ambiguityRestore.ExitCode != 0)
            {
                Assert.Fail($"Ambiguous type fixture restore failed: {await ambiguityOutput}{await ambiguityError}");
            }

            using CSharpSyntaxDiagnosticService ambiguityService = new();
            AxamlMvvmSemanticResolution ambiguousViewModel = await ambiguityService.AnalyzeAxamlMvvmAsync(
                mappedNamespaceMarkup,
                viewPath,
                new AA98_AvlnCodeStudio.Model.Workspaces.CSharpWorkspaceLoader().Load(projectPath));
            Assert.IsFalse(ambiguousViewModel.IsSuccessful);
            Assert.IsTrue(ambiguousViewModel.Diagnostics.Any(message =>
                message.Contains("x:DataType", StringComparison.Ordinal)
                && message.Contains("could not be resolved unambiguously", StringComparison.Ordinal)));
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [TestMethod]
    public async Task AnalyzeAxamlMvvmAsync_ResolvesViewModelFromBuiltCommunityToolkitProject()
    {
        string root = Path.Combine(Path.GetTempPath(), $"AA98_AxamlToolkitSemantic_{Guid.NewGuid():N}");
        Directory.CreateDirectory(root);
        try
        {
            string projectPath = Path.Combine(root, "Sample.csproj");
            string viewPath = Path.Combine(root, "MainWindow.axaml");
            await File.WriteAllTextAsync(
                projectPath,
                $"""
                <Project Sdk="Microsoft.NET.Sdk">
                  <PropertyGroup>
                    <TargetFramework>net{Environment.Version.Major}.0</TargetFramework>
                    <ImplicitUsings>enable</ImplicitUsings>
                    <Nullable>enable</Nullable>
                  </PropertyGroup>
                  <ItemGroup>
                    <PackageReference Include="CommunityToolkit.Mvvm" Version="8.4.2" />
                  </ItemGroup>
                </Project>
                """);
            await File.WriteAllTextAsync(
                Path.Combine(root, "MainWindow.axaml.cs"),
                """
                namespace Sample;
                public partial class MainWindow
                {
                    public object? DataContext { get; set; }
                    public MainWindow() => DataContext = new MainWindowViewModel();
                }
                """);
            await File.WriteAllTextAsync(
                Path.Combine(root, "MainWindowViewModel.cs"),
                """
                using CommunityToolkit.Mvvm.ComponentModel;
                using CommunityToolkit.Mvvm.Input;

                namespace Sample;

                public partial class MainWindowViewModel : ObservableObject
                {
                    // Preserve this user-authored implementation.
                    [ObservableProperty]
                    private string _name = string.Empty;

                    [RelayCommand]
                    private void Save() { }

                    [RelayCommand]
                    private async System.Threading.Tasks.Task RefreshAsync() => await System.Threading.Tasks.Task.Yield();
                }
                """);

            ProcessStartInfo buildStartInfo = new("dotnet")
            {
                WorkingDirectory = AppContext.BaseDirectory,
                ArgumentList = { "build", projectPath, "--verbosity", "quiet" },
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
            };
            ClearInheritedMsBuildEnvironment(buildStartInfo);
            using Process build = Process.Start(buildStartInfo)
                ?? throw new InvalidOperationException("Could not start CommunityToolkit fixture build.");
            Task<string> standardOutput = build.StandardOutput.ReadToEndAsync();
            Task<string> standardError = build.StandardError.ReadToEndAsync();
            await build.WaitForExitAsync();
            if (build.ExitCode != 0)
            {
                Assert.Fail($"CommunityToolkit fixture build failed: {await standardOutput}{await standardError}");
            }

            AA98_AvlnCodeStudio.Model.Workspaces.CSharpWorkspace workspace =
                new AA98_AvlnCodeStudio.Model.Workspaces.CSharpWorkspaceLoader().Load(projectPath);
            using CSharpSyntaxDiagnosticService service = new();
            AxamlMvvmSemanticResolution resolution = await service.AnalyzeAxamlMvvmAsync(
                """
                <Window xmlns="https://github.com/avaloniaui"
                        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                        xmlns:vm="using:Sample"
                        x:Class="Sample.MainWindow"
                        x:DataType="vm:MainWindowViewModel" />
                """,
                viewPath,
                workspace);

            Assert.IsFalse(resolution.IsSuccessful);
            Assert.AreEqual("Sample.MainWindowViewModel", resolution.ViewModelTypeName);
            Assert.IsTrue(resolution.HasRuntimeDataContext);
            Assert.IsTrue(resolution.Diagnostics.All(message =>
                message.Contains("already receives runtime DataContext", StringComparison.Ordinal)));
            AxamlMvvmSemanticMember generatedProperty = resolution.ViewModelMembers
                .Single(member => member.Name == "Name");
            Assert.AreEqual("string", generatedProperty.TypeName);
            Assert.IsTrue(generatedProperty.IsWritable);
            AxamlMvvmSemanticMember generatedCommand = resolution.ViewModelMembers
                .Single(member => member.Name == "SaveCommand");
            Assert.IsTrue(generatedCommand.IsCommand);
            Assert.IsFalse(generatedCommand.IsWritable);
            AxamlMvvmSemanticMember generatedAsyncCommand = resolution.ViewModelMembers
                .Single(member => member.Name == "RefreshCommand");
            Assert.AreEqual("IAsyncRelayCommand", generatedAsyncCommand.TypeName);
            Assert.IsTrue(generatedAsyncCommand.IsCommand);
            Assert.IsFalse(generatedAsyncCommand.IsWritable);

            AxamlMvvmMemberPlanEntry[] additions = new AxamlMvvmMemberPlanner().Plan(
                [
                    new AxamlMvvmMemberRequirement("Status", "string", IsCommand: false),
                    new AxamlMvvmMemberRequirement("CloseCommand", "ICommand", IsCommand: true),
                ],
                resolution.ViewModelMembers,
                createMissingMembers: true).ToArray();
            WorkspaceFileChangeRequest sourceChange = await service.PlanViewModelSourceAdditionAsync(
                root,
                projectPath,
                "Sample.MainWindowViewModel",
                additions);

            Assert.AreEqual(Path.GetFileName("MainWindowViewModel.cs"), sourceChange.RelativePath);
            Assert.AreEqual(WorkspaceFileChangeTarget.MustExist, sourceChange.Target);
            Assert.AreEqual(
                sourceChange.ExpectedContent,
                await File.ReadAllTextAsync(Path.Combine(root, "MainWindowViewModel.cs")));
            StringAssert.Contains(sourceChange.ProposedContent, "// Preserve this user-authored implementation.");
            StringAssert.Contains(sourceChange.ProposedContent, "[global::CommunityToolkit.Mvvm.ComponentModel.ObservableProperty]");
            StringAssert.Contains(sourceChange.ProposedContent, "private string _status = string.Empty;");
            StringAssert.Contains(sourceChange.ProposedContent, "[global::CommunityToolkit.Mvvm.Input.RelayCommand]");
            StringAssert.Contains(sourceChange.ProposedContent, "private void Close()");

            await File.WriteAllTextAsync(Path.Combine(root, "MainWindowViewModel.cs"), sourceChange.ProposedContent);
            ProcessStartInfo additionBuildInfo = new("dotnet")
            {
                WorkingDirectory = AppContext.BaseDirectory,
                ArgumentList = { "build", projectPath, "--no-restore", "--verbosity", "quiet" },
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
            };
            ClearInheritedMsBuildEnvironment(additionBuildInfo);
            using Process additionBuild = Process.Start(additionBuildInfo)
                ?? throw new InvalidOperationException("Could not start the incremental Toolkit fixture build.");
            Task<string> additionOutput = additionBuild.StandardOutput.ReadToEndAsync();
            Task<string> additionError = additionBuild.StandardError.ReadToEndAsync();
            await additionBuild.WaitForExitAsync();
            if (additionBuild.ExitCode != 0)
            {
                Assert.Fail($"Incremental CommunityToolkit fixture build failed: {await additionOutput}{await additionError}");
            }
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [TestMethod]
    public async Task GetCompletionsAsync_ProjectWorkspaceProvidesMemberCompletions()
    {
        string root = Path.Combine(Path.GetTempPath(), $"AA98_RoslynCompletion_{Guid.NewGuid():N}");
        Directory.CreateDirectory(root);
        try
        {
            string projectPath = Path.Combine(root, "Sample.csproj");
            string sourcePath = Path.Combine(root, "Program.cs");
            await File.WriteAllTextAsync(projectPath,
                $"<Project Sdk=\"Microsoft.NET.Sdk\"><PropertyGroup><TargetFramework>net{Environment.Version.Major}.0</TargetFramework><ImplicitUsings>disable</ImplicitUsings></PropertyGroup></Project>");
            await File.WriteAllTextAsync(sourcePath, "public class Sample { }");

            ProcessStartInfo restoreStartInfo = new("dotnet")
            {
                WorkingDirectory = AppContext.BaseDirectory,
                ArgumentList = { "restore", projectPath, "--ignore-failed-sources", "--verbosity", "quiet" },
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
            };
            ClearInheritedMsBuildEnvironment(restoreStartInfo);
            using Process restore = Process.Start(restoreStartInfo)
                ?? throw new InvalidOperationException("Could not start dotnet restore for the completion test.");
            Task<string> standardOutput = restore.StandardOutput.ReadToEndAsync();
            Task<string> standardError = restore.StandardError.ReadToEndAsync();
            await restore.WaitForExitAsync();
            if (restore.ExitCode != 0)
            {
                Assert.Fail($"Temporary C# project restore failed: {await standardOutput}{await standardError}");
            }

            AA98_AvlnCodeStudio.Model.Workspaces.CSharpWorkspace workspace =
                new AA98_AvlnCodeStudio.Model.Workspaces.CSharpWorkspaceLoader().Load(projectPath);
            const string source = "public class Sample { void Run() { System.Console.W } }";
            int position = source.IndexOf("System.Console.W", StringComparison.Ordinal) + "System.Console.W".Length;
            using CSharpSyntaxDiagnosticService service = new();

            IReadOnlyList<string> completions = await service.GetCompletionsAsync(
                source,
                sourcePath,
                position,
                workspace);

            CollectionAssert.Contains(completions.ToArray(), "WriteLine");
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [TestMethod]
    public async Task FindDefinitionAsync_ReturnsSourceLocationForReferencedSymbol()
    {
        string root = Path.Combine(Path.GetTempPath(), $"AA98_RoslynDefinition_{Guid.NewGuid():N}");
        Directory.CreateDirectory(root);
        try
        {
            string projectPath = Path.Combine(root, "Sample.csproj");
            string sourcePath = Path.Combine(root, "Program.cs");
            string definitionPath = Path.Combine(root, "MarkerType.cs");
            await File.WriteAllTextAsync(projectPath,
                $"<Project Sdk=\"Microsoft.NET.Sdk\"><PropertyGroup><TargetFramework>net{Environment.Version.Major}.0</TargetFramework><ImplicitUsings>disable</ImplicitUsings></PropertyGroup></Project>");
            await File.WriteAllTextAsync(sourcePath,
                "using Demo;\npublic class Consumer { void Run() { var value = new MarkerType(); } }");
            await File.WriteAllTextAsync(definitionPath,
                "namespace Demo;\npublic class MarkerType { }");

            ProcessStartInfo restoreStartInfo = new("dotnet")
            {
                WorkingDirectory = AppContext.BaseDirectory,
                ArgumentList = { "restore", projectPath, "--ignore-failed-sources", "--verbosity", "quiet" },
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
            };
            ClearInheritedMsBuildEnvironment(restoreStartInfo);
            using Process restore = Process.Start(restoreStartInfo)
                ?? throw new InvalidOperationException("Could not start dotnet restore for the definition test.");
            Task<string> standardOutput = restore.StandardOutput.ReadToEndAsync();
            Task<string> standardError = restore.StandardError.ReadToEndAsync();
            await restore.WaitForExitAsync();
            if (restore.ExitCode != 0)
            {
                Assert.Fail($"Temporary C# project restore failed: {await standardOutput}{await standardError}");
            }

            AA98_AvlnCodeStudio.Model.Workspaces.CSharpWorkspace workspace =
                new AA98_AvlnCodeStudio.Model.Workspaces.CSharpWorkspaceLoader().Load(projectPath);
            const string source = "using Demo;\npublic class Consumer { void Run() { var value = new MarkerType(); } }";
            int position = source.IndexOf("MarkerType", source.IndexOf("new MarkerType", StringComparison.Ordinal), StringComparison.Ordinal);
            using CSharpSyntaxDiagnosticService service = new();

            Code.Navigation.CodeLocation? location = await service.FindDefinitionAsync(
                source,
                sourcePath,
                position,
                workspace);

            Assert.IsNotNull(location);
            Assert.AreEqual(Path.GetFullPath(definitionPath), location.Path);
            Assert.AreEqual(2, location.Line);
            Assert.AreEqual(14, location.Column);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    private static void ClearInheritedMsBuildEnvironment(ProcessStartInfo startInfo)
    {
        startInfo.Environment.Remove("MSBuildSDKsPath");
        startInfo.Environment.Remove("MSBuildExtensionsPath");
        startInfo.Environment.Remove("MSBuildExtensionsPath32");
        startInfo.Environment.Remove("MSBuildToolsPath");
        startInfo.Environment.Remove("DOTNET_MSBUILD_SDK_RESOLVER_SDKS_DIR");
        startInfo.Environment.Remove("DOTNET_MSBUILD_SDK_RESOLVER_CLI_DIR");
    }
}
