using AA98_AvlnCodeStudio.UI.Services;
using AA98_AvlnCodeStudio.Model.Workspaces;
using AA98_AvlnCodeStudio.Base.Testing.Models;
using AA98_AvlnCodeStudio.Base.Testing.Services;
using AA98_AvlnCodeStudio.Base.Debugging.Models;
using AA98_AvlnCodeStudio.Base.Debugging.Services;
using AppKomponentBaseLib.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
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

                public class BaseViewModel : ObservableObject
                {
                    public string Status { get; set; } = string.Empty;
                }

                public partial class MainWindowViewModel : BaseViewModel
                {
                    private int closeCommand;
                    public void OnStatusChanged(string value) { }

                    // Preserve this user-authored implementation.
                    [ObservableProperty]
                    private string _name = string.Empty;

                    [RelayCommand]
                    private void Save() { }

                    [RelayCommand]
                    private void FooCommand() { }

                    [RelayCommand]
                    private async System.Threading.Tasks.Task RefreshAsync() => await System.Threading.Tasks.Task.Yield();

                    [RelayCommand(IncludeCancelCommand = true)]
                    private async System.Threading.Tasks.Task DownloadAsync(System.Threading.CancellationToken cancellationToken)
                        => await System.Threading.Tasks.Task.Yield();
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
            AxamlMvvmSemanticMember generatedCancelCommand = resolution.ViewModelMembers
                .Single(member => member.Name == "DownloadCancelCommand");
            Assert.AreEqual("ICommand", generatedCancelCommand.TypeName);
            Assert.IsTrue(generatedCancelCommand.IsCommand);
            Assert.IsFalse(generatedCancelCommand.IsWritable);

            AxamlMvvmMemberPlanEntry[] collidingAddition = new AxamlMvvmMemberPlanner().Plan(
                [new AxamlMvvmMemberRequirement("CloseCommand", "ICommand", IsCommand: true)],
                resolution.ViewModelMembers,
                createMissingMembers: true).ToArray();
            await Assert.ThrowsExactlyAsync<InvalidDataException>(async () =>
                await service.PlanViewModelSourceAdditionAsync(
                    root,
                    projectPath,
                    "Sample.MainWindowViewModel",
                    collidingAddition));

            AxamlMvvmMemberPlanEntry[] inheritedCollision =
            [
                new(
                    new AxamlMvvmMemberRequirement("Status", "string", IsCommand: false),
                    AxamlMvvmMemberPlanAction.Create,
                    null),
            ];
            await Assert.ThrowsExactlyAsync<InvalidDataException>(async () =>
                await service.PlanViewModelSourceAdditionAsync(
                    root,
                    projectPath,
                    "Sample.MainWindowViewModel",
                    inheritedCollision));

            AxamlMvvmMemberPlanEntry[] changeHookCollision =
            [
                new(
                    new AxamlMvvmMemberRequirement("Status", "string", IsCommand: false),
                    AxamlMvvmMemberPlanAction.Create,
                    null),
            ];
            await Assert.ThrowsExactlyAsync<InvalidDataException>(async () =>
                await service.PlanViewModelSourceAdditionAsync(
                    root,
                    projectPath,
                    "Sample.MainWindowViewModel",
                    changeHookCollision));

            AxamlMvvmMemberPlanEntry[] generatedCommandFieldCollision =
            [
                new(
                    new AxamlMvvmMemberRequirement("fooCommandCommand", "ICommand", IsCommand: true),
                    AxamlMvvmMemberPlanAction.Create,
                    null),
            ];
            await Assert.ThrowsExactlyAsync<InvalidDataException>(async () =>
                await service.PlanViewModelSourceAdditionAsync(
                    root,
                    projectPath,
                    "Sample.MainWindowViewModel",
                    generatedCommandFieldCollision));

            AxamlMvvmMemberPlanEntry[] asyncCommandFieldCollision =
            [
                new(
                    new AxamlMvvmMemberRequirement("refreshCommand", "string", IsCommand: false),
                    AxamlMvvmMemberPlanAction.Create,
                    null),
            ];
            await Assert.ThrowsExactlyAsync<InvalidDataException>(async () =>
                await service.PlanViewModelSourceAdditionAsync(
                    root,
                    projectPath,
                    "Sample.MainWindowViewModel",
                    asyncCommandFieldCollision));

            AxamlMvvmMemberPlanEntry[] cancelCommandFieldCollision =
            [
                new(
                    new AxamlMvvmMemberRequirement("downloadCancelCommand", "string", IsCommand: false),
                    AxamlMvvmMemberPlanAction.Create,
                    null),
            ];
            await Assert.ThrowsExactlyAsync<InvalidDataException>(async () =>
                await service.PlanViewModelSourceAdditionAsync(
                    root,
                    projectPath,
                    "Sample.MainWindowViewModel",
                    cancelCommandFieldCollision));

            AxamlMvvmMemberPlanEntry[] invalidCommandAddition =
            [
                new(
                    new AxamlMvvmMemberRequirement("Command", "ICommand", IsCommand: true),
                    AxamlMvvmMemberPlanAction.Create,
                    null),
            ];
            await Assert.ThrowsExactlyAsync<InvalidDataException>(async () =>
                await service.PlanViewModelSourceAdditionAsync(
                    root,
                    projectPath,
                    "Sample.MainWindowViewModel",
                    invalidCommandAddition));

            AxamlMvvmMemberPlanEntry[] additions = new AxamlMvvmMemberPlanner().Plan(
                [
                    new AxamlMvvmMemberRequirement("Notice", "string", IsCommand: false),
                    new AxamlMvvmMemberRequirement("ExitCommand", "ICommand", IsCommand: true),
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
            StringAssert.Contains(sourceChange.ProposedContent,
                await service.SupportsObservablePartialPropertiesAsync(projectPath)
                    ? "public partial string Notice { get; set; } = string.Empty;"
                    : "private string _notice = string.Empty;");
            StringAssert.Contains(sourceChange.ProposedContent, "[global::CommunityToolkit.Mvvm.Input.RelayCommand]");
            StringAssert.Contains(sourceChange.ProposedContent, "private void Exit()");
            Assert.AreEqual(
                0,
                (await service.GetNewCompilerErrorsAsync(projectPath, sourceChange)).Count);
            WorkspaceFileChangeRequest invalidSourceChange = WorkspaceFileChangeRequest.ReplaceExisting(
                sourceChange.RelativePath,
                sourceChange.ExpectedContent!,
                sourceChange.ProposedContent + Environment.NewLine + "invalid C#");
            Assert.IsTrue((await service.GetNewCompilerErrorsAsync(projectPath, invalidSourceChange)).Count > 0);

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
    public async Task ObservableProperties_UseActualProjectGeneratorAndPreserveOlderProjects()
    {
        if (Environment.Version.Major < 10)
        {
            return;
        }

        string root = Path.Combine(Directory.GetCurrentDirectory(), $"mvvm-property-fixture-{Guid.NewGuid():N}");
        Directory.CreateDirectory(root);
        try
        {
            foreach ((string language, string toolkit, bool modern) in new[]
            {
                ("14", "8.4.2", true),
                ("13", "8.4.2", false),
                ("14", "8.4.0", false),
            })
            {
                string directory = Path.Combine(root, $"cs{language}-toolkit{toolkit}");
                Directory.CreateDirectory(directory);
                string projectPath = Path.Combine(directory, "Fixture.csproj");
                string viewModelPath = Path.Combine(directory, "ExistingViewModel.cs");
                string projectText = $"""
                    <Project Sdk="Microsoft.NET.Sdk">
                      <PropertyGroup>
                        <OutputType>Exe</OutputType>
                        <TargetFramework>net10.0</TargetFramework>
                        <LangVersion>{language}</LangVersion>
                        <Nullable>enable</Nullable>
                        <ManagePackageVersionsCentrally>false</ManagePackageVersionsCentrally>
                      </PropertyGroup>
                      <ItemGroup><PackageReference Include="CommunityToolkit.Mvvm" Version="{toolkit}" /></ItemGroup>
                    </Project>
                    """;
                await File.WriteAllTextAsync(projectPath, projectText);
                await File.WriteAllTextAsync(viewModelPath, """
                    namespace Sample;
                    public partial class ExistingViewModel : global::CommunityToolkit.Mvvm.ComponentModel.ObservableObject
                    {
                        private string _notice = "";
                        private string Clash = "";
                    }
                    """);
                await File.WriteAllTextAsync(Path.Combine(directory, "Program.cs"), """
                    using System;
                    var generated = new Sample.MainWindowViewModel();
                    if (generated.Name != "initial") throw new Exception("Initial value");
                    bool changed = false;
                    generated.PropertyChanged += (_, e) => changed = e.PropertyName == nameof(generated.Name);
                    generated.Name = "changed";
                    if (!changed || generated.Name != "changed") throw new Exception("Setter notification");
                    """);

                await RunFixtureAsync("restore", projectPath);
                using CSharpSyntaxDiagnosticService service = new();
                Assert.AreEqual(modern, await service.SupportsObservablePartialPropertiesAsync(projectPath),
                    $"Incorrect capability for C# {language}, Toolkit {toolkit}.");

                WorkspaceFileChangeRequest[] generated = new AxamlMvvmGenerator().Generate(
                    "MainWindow.axaml",
                    """
                    <Window xmlns="https://github.com/avaloniaui"
                            xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                            x:Class="Sample.MainWindow">
                      <TextBox x:Name="NameTextBox" Text="initial" />
                    </Window>
                    """,
                    await service.SupportsObservablePartialPropertiesAsync(projectPath)).ToArray();
                StringAssert.Contains(generated[1].ProposedContent, modern
                    ? "public partial string Name { get; set; } = \"initial\";"
                    : "private string _name = \"initial\";");
                Assert.AreEqual(0, (await service.GetNewSourceCompilerErrorsAsync(
                    directory, projectPath, generated[1])).Count);
                WorkspaceFileChangeRequest conflictingSource = WorkspaceFileChangeRequest.CreateNew(
                    generated[1].RelativePath,
                    generated[1].ProposedContent.Replace(
                        "partial class MainWindowViewModel",
                        "partial class Name",
                        StringComparison.Ordinal));
                Assert.IsTrue((await service.GetNewSourceCompilerErrorsAsync(
                    directory, projectPath, conflictingSource)).Count > 0);

                string memberName = modern ? "Notice" : "Title";
                AxamlMvvmMemberPlanEntry[] additions =
                [
                    new(new AxamlMvvmMemberRequirement(memberName, "string", IsCommand: false),
                        AxamlMvvmMemberPlanAction.Create, null),
                ];
                WorkspaceFileChangeRequest edit = await service.PlanViewModelSourceAdditionAsync(
                    root, projectPath, "Sample.ExistingViewModel", additions);
                StringAssert.Contains(edit.ProposedContent, modern
                    ? "public partial string Notice { get; set; } = string.Empty;"
                    : "private string _title = string.Empty;");
                Assert.AreEqual(0, (await service.GetNewCompilerErrorsAsync(
                    projectPath, edit, workspaceRoot: root)).Count);
                await Assert.ThrowsExactlyAsync<InvalidDataException>(() =>
                    service.PlanViewModelSourceAdditionAsync(directory, projectPath, "Sample.ExistingViewModel",
                    [
                        new(new AxamlMvvmMemberRequirement("Clash", "string", IsCommand: false),
                            AxamlMvvmMemberPlanAction.Create, null),
                    ]));
                Assert.AreEqual(projectText, await File.ReadAllTextAsync(projectPath));
                Assert.AreEqual(edit.ExpectedContent, await File.ReadAllTextAsync(viewModelPath));
                await File.WriteAllTextAsync(viewModelPath, edit.ProposedContent);
                await File.WriteAllTextAsync(Path.Combine(directory, generated[1].RelativePath), generated[1].ProposedContent);
                await RunFixtureAsync("run", projectPath);
            }

            string multiDirectory = Path.Combine(root, "multi-target");
            Directory.CreateDirectory(multiDirectory);
            string multiProject = Path.Combine(multiDirectory, "Fixture.csproj");
            await File.WriteAllTextAsync(multiProject, """
                <Project Sdk="Microsoft.NET.Sdk">
                  <PropertyGroup>
                    <TargetFrameworks>net9.0;net10.0</TargetFrameworks>
                    <ManagePackageVersionsCentrally>false</ManagePackageVersionsCentrally>
                  </PropertyGroup>
                  <ItemGroup><PackageReference Include="CommunityToolkit.Mvvm" Version="8.4.2" /></ItemGroup>
                </Project>
                """);
            using CSharpSyntaxDiagnosticService multiService = new();
            Assert.IsFalse(await multiService.SupportsObservablePartialPropertiesAsync(multiProject));
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    private static async Task RunFixtureAsync(string command, string projectPath)
    {
        ProcessStartInfo start = new("dotnet")
        {
            WorkingDirectory = Path.GetDirectoryName(projectPath)!,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
        };
        start.ArgumentList.Add(command);
        if (command == "run")
        {
            start.ArgumentList.Add("--project");
            start.ArgumentList.Add(projectPath);
            start.ArgumentList.Add("--no-restore");
        }
        else
        {
            start.ArgumentList.Add(projectPath);
        }
        start.ArgumentList.Add("--verbosity");
        start.ArgumentList.Add("quiet");
        ClearInheritedMsBuildEnvironment(start);
        using Process process = Process.Start(start) ?? throw new InvalidOperationException("Could not start fixture.");
        Task<string> output = process.StandardOutput.ReadToEndAsync();
        Task<string> error = process.StandardError.ReadToEndAsync();
        await process.WaitForExitAsync();
        Assert.AreEqual(0, process.ExitCode, $"{command} failed: {await output}{await error}");
    }

    [TestMethod]
    public async Task GetQuickInfoAsync_UsesEditedSourceAndReturnsNullForUnknownDocument()
    {
        string root = Path.Combine(Path.GetTempPath(), $"AA98_RoslynQuickInfo_{Guid.NewGuid():N}");
        Directory.CreateDirectory(root);
        try
        {
            string projectPath = Path.Combine(root, "Sample.csproj");
            string sourcePath = Path.Combine(root, "Sample.cs");
            string otherSourcePath = Path.Combine(root, "Other.cs");
            await File.WriteAllTextAsync(projectPath,
                $"<Project Sdk=\"Microsoft.NET.Sdk\"><PropertyGroup><TargetFramework>net{Environment.Version.Major}.0</TargetFramework></PropertyGroup></Project>");
            await File.WriteAllTextAsync(sourcePath, "public class Sample { }");
            await File.WriteAllTextAsync(otherSourcePath, "public class Other { public int Value => 1; }");
            await RunFixtureAsync("restore", projectPath);
            CSharpWorkspace workspace = new CSharpWorkspaceLoader().Load(projectPath);
            using CSharpSyntaxDiagnosticService service = new();
            const string edited = """
                public class Sample
                {
                    /// <summary>A useful count.</summary>
                    public int Count => 42;
                    public int Read() => Count;
                }
                """;
            int position = edited.LastIndexOf("Count", StringComparison.Ordinal);
            string? info = await service.GetQuickInfoAsync(edited, sourcePath, position, workspace);
            Assert.IsNotNull(info);
            StringAssert.Contains(info, "Sample.Count");
            StringAssert.Contains(info, "A useful count.");
            Assert.AreEqual("public class Sample { }", await File.ReadAllTextAsync(sourcePath));

            await service.AnalyzeAsync(edited, sourcePath, workspace);
            Assert.AreEqual("public class Sample { }", await File.ReadAllTextAsync(sourcePath));
            await service.GetCompletionsAsync(edited, sourcePath, position, workspace);
            Assert.AreEqual("public class Sample { }", await File.ReadAllTextAsync(sourcePath));
            Code.Navigation.CodeLocation? definition = await service.FindDefinitionAsync(edited, sourcePath, position, workspace);
            Assert.IsNotNull(definition);
            Assert.AreEqual("public class Sample { }", await File.ReadAllTextAsync(sourcePath));

            const string crossFileSource =
                "public class Sample { public int Read(Other other) => other.Value; }";
            const string otherDraft = """
                public class Other
                {
                    public string Value => "";
                    public int DraftMember => 1;
                }
                """;
            Dictionary<string, string> openDocuments = new()
            {
                [sourcePath] = crossFileSource,
                [otherSourcePath] = otherDraft,
            };
            int valuePosition = crossFileSource.LastIndexOf("Value", StringComparison.Ordinal);
            string? crossFileInfo = await service.GetQuickInfoAsync(
                crossFileSource, sourcePath, valuePosition, workspace, openDocumentTexts: openDocuments);
            Assert.IsNotNull(crossFileInfo);
            StringAssert.Contains(crossFileInfo, "string Other.Value");
            IReadOnlyList<Diagnostic> crossFileDiagnostics = await service.AnalyzeAsync(
                crossFileSource, sourcePath, workspace, openDocumentTexts: openDocuments);
            Assert.IsTrue(crossFileDiagnostics.Any(diagnostic => diagnostic.Code == "CS0029"));
            Code.Navigation.CodeLocation? crossFileDefinition = await service.FindDefinitionAsync(
                crossFileSource, sourcePath, valuePosition, workspace, openDocumentTexts: openDocuments);
            Assert.IsNotNull(crossFileDefinition);
            Assert.AreEqual(otherSourcePath, crossFileDefinition.Path);
            Assert.AreEqual(3, crossFileDefinition.Line);
            Assert.AreEqual("public class Other { public int Value => 1; }", await File.ReadAllTextAsync(otherSourcePath));

            const string signatureSource =
                "public class Sample { void Run() { System.Console.WriteLine(1); } }";
            int signaturePosition = signatureSource.IndexOf("WriteLine(", StringComparison.Ordinal)
                + "WriteLine(".Length;
            CSharpSignatureHelp? signatureHelp = await service.GetSignatureHelpAsync(
                signatureSource, sourcePath, signaturePosition, workspace);
            Assert.IsNotNull(signatureHelp);
            Assert.IsTrue(signatureHelp.Signatures.Any(signature => signature.Contains("WriteLine(", StringComparison.Ordinal)));
            Assert.AreEqual(0, signatureHelp.ActiveParameterIndex);

            const string overloadSource =
                "public class Sample { void Run() { Write(1, \"value\"); } "
                + "void Write(int number, string text) { } void Write(int number, object value) { } }";
            int activeParameterPosition = overloadSource.IndexOf("\"value\"", StringComparison.Ordinal) + 3;
            CSharpSignatureHelp? overloadHelp = await service.GetSignatureHelpAsync(
                overloadSource, sourcePath, activeParameterPosition, workspace);
            Assert.IsNotNull(overloadHelp);
            Assert.AreEqual(2, overloadHelp.Signatures.Count);
            Assert.AreEqual(1, overloadHelp.ActiveParameterIndex);
            Assert.AreEqual("text", overloadHelp.ActiveParameterName);
            StringAssert.Contains(overloadHelp.Signatures[overloadHelp.SelectedSignatureIndex], "string text");

            const string completionSource = "public class Sample { void Run(Other other) { other.Draft; } }";
            int completionPosition = completionSource.IndexOf("Draft", StringComparison.Ordinal) + "Draft".Length;
            IReadOnlyList<CSharpCompletionSuggestion> crossFileCompletions = await service.GetCompletionItemsAsync(
                completionSource, sourcePath, completionPosition, workspace, openDocumentTexts: openDocuments);
            Assert.IsTrue(crossFileCompletions.Any(item => item.DisplayText == "DraftMember"));

            Assert.IsNull(await service.GetQuickInfoAsync(edited, Path.Combine(root, "Unknown.cs"), position, workspace));
            Assert.IsNull(await service.GetQuickInfoAsync(edited,
                Path.Combine(Path.GetDirectoryName(root)!, "Outside.cs"), position, workspace));
            string otherProjectPath = Path.Combine(root, "Other.csproj");
            string solutionPath = Path.Combine(root, "Sample.slnx");
            await File.WriteAllTextAsync(otherProjectPath, "<Project />");
            await File.WriteAllTextAsync(solutionPath,
                "<Solution><Project Path=\"Sample.csproj\" /><Project Path=\"Other.csproj\" /></Solution>");
            Assert.IsNull(await service.GetQuickInfoAsync(edited, sourcePath, position,
                new CSharpWorkspaceLoader().Load(solutionPath)));
            Assert.AreEqual(0, (await service.GetCompletionItemsAsync(edited, sourcePath, position,
                new CSharpWorkspaceLoader().Load(solutionPath))).Count);
            await Assert.ThrowsExactlyAsync<ArgumentOutOfRangeException>(
                () => service.GetQuickInfoAsync(edited, sourcePath, edited.Length + 1, workspace));
            using CancellationTokenSource cancelled = new();
            cancelled.Cancel();
            await Assert.ThrowsAsync<OperationCanceledException>(
                () => service.GetQuickInfoAsync(edited, sourcePath, position, workspace, cancelled.Token));
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [TestMethod]
    public async Task FindTestMethodCandidatesAsync_UsesSemanticAttributesAndSelectedConfiguration()
    {
        string root = Path.Combine(Path.GetTempPath(), $"AA98_TestCandidates_{Guid.NewGuid():N}");
        Directory.CreateDirectory(root);
        try
        {
            string projectPath = Path.Combine(root, "Sample.csproj");
            string sourcePath = Path.Combine(root, "Tests.cs");
            await File.WriteAllTextAsync(projectPath,
                $"<Project Sdk=\"Microsoft.NET.Sdk\"><PropertyGroup><TargetFramework>net{Environment.Version.Major}.0</TargetFramework></PropertyGroup></Project>");
            const string source = """
                namespace Microsoft.VisualStudio.TestTools.UnitTesting
                {
                    public class TestMethodAttribute : System.Attribute { }
                    public class DataTestMethodAttribute : TestMethodAttribute { }
                }
                namespace Xunit
                {
                    public class FactAttribute : System.Attribute { }
                    public class TheoryAttribute : FactAttribute { }
                }
                namespace NUnit.Framework { public class TestCaseAttribute : System.Attribute { } }
                namespace Other { public class TestMethodAttribute : System.Attribute { } }
                namespace Sample
                {
                    public class Tests
                    {
                        [Microsoft.VisualStudio.TestTools.UnitTesting.DataTestMethod] public void Data() { }
                        [Xunit.Theory] public void Theory() { }
                        [NUnit.Framework.TestCase] public void Case() { }
                        [Other.TestMethod] public void NotATest() { }
                        [Xunit.Fact] private void NotPublic() { }
                #if DEBUG
                        [Xunit.Fact] public void DebugOnly() { }
                #endif
                    }
                }
                """;
            await File.WriteAllTextAsync(sourcePath, source);
            await RunFixtureAsync("restore", projectPath);
            using CSharpSyntaxDiagnosticService service = new();
            string framework = $"net{Environment.Version.Major}.0";
            IReadOnlyList<TestMethodCandidate> debug = await service.FindTestMethodCandidatesAsync(projectPath, "Debug", framework);
            CollectionAssert.AreEqual(new[]
            {
                "Sample.Tests.Case", "Sample.Tests.Data", "Sample.Tests.DebugOnly", "Sample.Tests.Theory",
            }, debug.Select(candidate => candidate.FullyQualifiedName).ToArray());
            Assert.IsTrue(debug.All(candidate => candidate.SourcePath == sourcePath && candidate.LineNumber > 0));
            IReadOnlyList<TestMethodCandidate> release = await service.FindTestMethodCandidatesAsync(projectPath, "Release", framework);
            CollectionAssert.AreEqual(new[]
            {
                "Sample.Tests.Case", "Sample.Tests.Data", "Sample.Tests.Theory",
            }, release.Select(candidate => candidate.FullyQualifiedName).ToArray());
            Assert.AreEqual(source, await File.ReadAllTextAsync(sourcePath));
            using CancellationTokenSource canceled = new();
            canceled.Cancel();
            await Assert.ThrowsAsync<OperationCanceledException>(
                () => service.FindTestMethodCandidatesAsync(projectPath, "Debug", framework, canceled.Token));
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [TestMethod]
    public async Task FindTestMethodCandidatesAsync_NUnitCandidateRunsParameterizedCases()
    {
        string root = Path.Combine(Path.GetTempPath(), $"AA98_NUnitCandidates_{Guid.NewGuid():N}");
        Directory.CreateDirectory(root);
        try
        {
            string projectPath = Path.Combine(root, "Sample.csproj");
            string framework = $"net{Environment.Version.Major}.0";
            await File.WriteAllTextAsync(projectPath,
                $"""
                <Project Sdk="Microsoft.NET.Sdk">
                  <PropertyGroup>
                    <TargetFramework>{framework}</TargetFramework>
                    <IsTestProject>true</IsTestProject>
                  </PropertyGroup>
                  <ItemGroup>
                    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="18.11.0" />
                    <PackageReference Include="NUnit" Version="5.0.0" />
                    <PackageReference Include="NUnit3TestAdapter" Version="6.3.1" />
                  </ItemGroup>
                </Project>
                """);
            await File.WriteAllTextAsync(Path.Combine(root, "Tests.cs"),
                """
                using NUnit.Framework;
                namespace Sample;
                public class NUnitTests
                {
                    [TestCase(1)]
                    [TestCase(2)]
                    public void Values(int value) => Assert.That(value, Is.GreaterThan(0));
                    [Test] public void Other() => Assert.Fail("Expected case failure.");
                    [Test, Ignore("Skipped fixture")] public void Skipped() { }
                }
                """);
            await RunFixtureAsync("restore", projectPath);
            using CSharpSyntaxDiagnosticService service = new();
            IReadOnlyList<TestMethodCandidate> candidates = await service.FindTestMethodCandidatesAsync(projectPath, "Debug", framework);
            CollectionAssert.AreEqual(new[] { "Sample.NUnitTests.Other", "Sample.NUnitTests.Skipped", "Sample.NUnitTests.Values" },
                candidates.Select(candidate => candidate.FullyQualifiedName).ToArray());
            TestRunSummary result = await new DotNetTestExecutionService().RunTestsAsync(new TestRunRequest
            {
                ProjectPath = projectPath,
                WorkspaceRootPath = root,
                Configuration = "Debug",
                TargetFramework = framework,
                MatchTargetsExactly = true,
                Targets = { "Sample.NUnitTests.Values" },
            });
            Assert.AreEqual(2, result.TotalCount);
            Assert.AreEqual(2, result.PassedCount);
            Assert.AreEqual(2, result.Cases.Count);
            Assert.AreEqual(2, result.Cases.Select(item => item.TestName).Distinct().Count());
            Assert.IsTrue(result.Cases.All(item => item.Outcome == "Passed"));
            Assert.IsTrue(result.Cases.All(item => item.TestClassName == "Sample.NUnitTests"));
            DiscoveredTestCase selected = await VerifyAdapterDiscoveryAsync(root, framework, "Sample.NUnitTests.Values");
            string? adapter = Environment.GetEnvironmentVariable("AVLN_NETCOREDBG_PATH");
            if (OperatingSystem.IsWindows() && !string.IsNullOrWhiteSpace(adapter) && File.Exists(adapter))
            {
                await VerifyAdapterDebugAsync(new DotNetTestDiscoveryService(), adapter,
                    Path.Combine(root, "bin", "Debug", framework, "Sample.dll"), root, selected);
            }
        }
        finally
        {
            await DeleteAdapterFixtureAsync(root);
        }
    }

    [TestMethod]
    public async Task FindTestMethodCandidatesAsync_XunitCandidateRunsParameterizedCases()
    {
        string root = Path.Combine(Path.GetTempPath(), $"AA98_XunitCandidates_{Guid.NewGuid():N}");
        Directory.CreateDirectory(root);
        try
        {
            string projectPath = Path.Combine(root, "Sample.csproj");
            string framework = $"net{Environment.Version.Major}.0";
            await File.WriteAllTextAsync(projectPath,
                $"""
                <Project Sdk="Microsoft.NET.Sdk">
                  <PropertyGroup>
                    <TargetFramework>{framework}</TargetFramework>
                    <OutputType>Exe</OutputType>
                    <IsTestProject>true</IsTestProject>
                  </PropertyGroup>
                  <ItemGroup>
                    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="18.11.0" />
                    <PackageReference Include="xunit.v3.mtp-off" Version="3.2.2" />
                    <PackageReference Include="xunit.runner.visualstudio" Version="4.0.1" />
                  </ItemGroup>
                </Project>
                """);
            await File.WriteAllTextAsync(Path.Combine(root, "Tests.cs"),
                """
                using Xunit;
                namespace Sample;
                public class XunitTests
                {
                    [Theory]
                    [InlineData(1)]
                    [InlineData(2)]
                    public void Values(int value)
                    {
                        string marker = System.Environment.GetEnvironmentVariable("AA98_XUNIT_DEBUG_CHILD_MARKER");
                        if (!string.IsNullOrEmpty(marker))
                        {
                            System.IO.File.WriteAllText(marker, System.Environment.ProcessId.ToString());
                            System.Threading.Thread.Sleep(120000);
                        }
                        Assert.True(value > 0);
                    }
                    [Fact] public void Other() => Assert.True(false, "Expected case failure.");
                    [Fact(Skip = "Skipped fixture")] public void Skipped() { }
                }
                """);
            await RunFixtureAsync("restore", projectPath);
            using CSharpSyntaxDiagnosticService service = new();
            IReadOnlyList<TestMethodCandidate> candidates = await service.FindTestMethodCandidatesAsync(projectPath, "Debug", framework);
            CollectionAssert.AreEqual(new[] { "Sample.XunitTests.Other", "Sample.XunitTests.Skipped", "Sample.XunitTests.Values" },
                candidates.Select(candidate => candidate.FullyQualifiedName).ToArray());
            TestRunSummary result = await new DotNetTestExecutionService().RunTestsAsync(new TestRunRequest
            {
                ProjectPath = projectPath,
                WorkspaceRootPath = root,
                Configuration = "Debug",
                TargetFramework = framework,
                MatchTargetsExactly = true,
                Targets = { "Sample.XunitTests.Values" },
            });
            Assert.AreEqual(2, result.TotalCount);
            Assert.AreEqual(2, result.PassedCount);
            Assert.AreEqual(2, result.Cases.Count);
            Assert.AreEqual(2, result.Cases.Select(item => item.TestName).Distinct().Count());
            Assert.IsTrue(result.Cases.All(item => item.Outcome == "Passed"));
            Assert.IsTrue(result.Cases.All(item => item.TestClassName == "Sample.XunitTests"));
            DiscoveredTestCase selected = await VerifyAdapterDiscoveryAsync(root, framework, "Sample.XunitTests.Values");
            string assembly = Path.Combine(root, "bin", "Debug", framework, "Sample.dll");
            await VerifyChildAttachFailureAsync(assembly, root, selected);
            string? adapter = Environment.GetEnvironmentVariable("AVLN_NETCOREDBG_PATH");
            if (OperatingSystem.IsWindows() && !string.IsNullOrWhiteSpace(adapter) && File.Exists(adapter))
            {
                await VerifyAdapterDebugAsync(new DotNetTestDiscoveryService(), adapter, assembly, root, selected);
                await VerifyDebugCancellationStopsAdapterChildAsync(adapter, assembly, root, selected);
            }
        }
        finally
        {
            await DeleteAdapterFixtureAsync(root);
        }
    }

    private static async Task DeleteAdapterFixtureAsync(string root)
    {
        for (int attempt = 0; ; attempt++)
        {
            try
            {
                if (Directory.Exists(root))
                {
                    Directory.Delete(root, recursive: true);
                }
                return;
            }
            catch (Exception exception) when (attempt < 20 && exception is IOException or UnauthorizedAccessException)
            {
                await Task.Delay(250);
            }
        }
    }

    private static async Task<DiscoveredTestCase> VerifyAdapterDiscoveryAsync(string root, string framework, string method)
    {
        string output = Path.Combine(root, "bin", "Debug", framework);
        DotNetTestDiscoveryService service = new();
        string assembly = Path.Combine(output, "Sample.dll");
        TestDiscoveryResult discovery = await service.DiscoverAsync(assembly, root);
        Assert.AreEqual(4, discovery.Cases.Count, string.Join(Environment.NewLine, discovery.Warnings));
        DiscoveredTestCase[] parameterized = discovery.Cases
            .Where(item => item.FullyQualifiedName == method
                || item.FullyQualifiedName.StartsWith(method + "(", StringComparison.Ordinal))
            .ToArray();
        Assert.AreEqual(2, parameterized.Length, string.Join(", ", discovery.Cases.Select(item => item.FullyQualifiedName)));
        Assert.AreEqual(2, parameterized.Select(item => item.Id).Distinct().Count());
        Assert.IsTrue(parameterized.All(item => !string.IsNullOrWhiteSpace(item.ExecutorUri)));
        DiscoveredTestCase selected = parameterized.OrderBy(item => item.DisplayName, StringComparer.Ordinal).First();
        TestRunSummary single = await service.RunCaseAsync(assembly, root, selected);
        Assert.AreEqual(1, single.TotalCount);
        Assert.AreEqual(1, single.PassedCount);
        Assert.AreEqual(selected.Id, single.Cases.Single().TestId);
        DiscoveredTestCase failing = discovery.Cases.Single(item => item.FullyQualifiedName.EndsWith(".Other", StringComparison.Ordinal));
        TestRunSummary failed = await service.RunCaseAsync(assembly, root, failing);
        Assert.AreEqual(1, failed.TotalCount);
        Assert.AreEqual(1, failed.FailedCount);
        StringAssert.Contains(failed.Failures.Single().Message, "Expected case failure.");
        DiscoveredTestCase skipped = discovery.Cases.Single(item => item.FullyQualifiedName.EndsWith(".Skipped", StringComparison.Ordinal));
        TestRunSummary skippedResult = await service.RunCaseAsync(assembly, root, skipped);
        Assert.AreEqual(1, skippedResult.TotalCount);
        Assert.AreEqual(1, skippedResult.SkippedCount);
        await Assert.ThrowsExactlyAsync<InvalidOperationException>(() =>
            service.RunCaseAsync(assembly, root, selected with { Id = Guid.NewGuid().ToString() }));

        return selected;
    }

    private static async Task VerifyChildAttachFailureAsync(string assembly, string root, DiscoveredTestCase selected)
    {
        string marker = Path.Combine(root, "failed-debug-child.txt");
        string? previous = Environment.GetEnvironmentVariable("AA98_XUNIT_DEBUG_CHILD_MARKER");
        Environment.SetEnvironmentVariable("AA98_XUNIT_DEBUG_CHILD_MARKER", marker);
        int attaches = 0;
        int releases = 0;
        try
        {
            InvalidOperationException error = await Assert.ThrowsExactlyAsync<InvalidOperationException>(() =>
                new DotNetTestDiscoveryService().DebugCaseAsync(assembly, root, selected, (_, _) =>
                {
                    if (Interlocked.Increment(ref attaches) == 1)
                    {
                        return Task.FromResult<IAsyncDisposable>(new CallbackLease(() => Interlocked.Increment(ref releases)));
                    }
                    return Task.FromException<IAsyncDisposable>(new InvalidOperationException("Child attach failed deliberately."));
                }));
            StringAssert.Contains(error.ToString(), "Child attach failed deliberately.");
            Assert.AreEqual(2, attaches);
            Assert.AreEqual(1, releases);
            Assert.IsFalse(File.Exists(marker), "The child ran the test after its debugger attach failed.");
        }
        finally
        {
            Environment.SetEnvironmentVariable("AA98_XUNIT_DEBUG_CHILD_MARKER", previous);
        }
    }

    private static async Task VerifyDebugCancellationStopsAdapterChildAsync(
        string adapter, string assembly, string root, DiscoveredTestCase selected)
    {
        string marker = Path.Combine(root, "debug-child.txt");
        string? previous = Environment.GetEnvironmentVariable("AA98_XUNIT_DEBUG_CHILD_MARKER");
        Environment.SetEnvironmentVariable("AA98_XUNIT_DEBUG_CHILD_MARKER", marker);
        NetCoreDbgDebugSessionService debugger = new(adapter);
        using CancellationTokenSource cancellation = new(TimeSpan.FromSeconds(60));
        Task<TestRunSummary> run = new DotNetTestDiscoveryService().DebugCaseAsync(assembly, root, selected, async (pid, token) =>
        {
            DebugSessionInfo session = await debugger.StartSessionAsync(new DebugLaunchRequest
            {
                AttachToExistingProcess = true,
                ProcessId = pid,
                WorkingDirectory = root,
            }, token);
            return new AdapterDebugLease(debugger, session.SessionId);
        }, cancellation.Token);
        Process? child = null;
        try
        {
            int pid;
            while (!File.Exists(marker) || !int.TryParse(await File.ReadAllTextAsync(marker, cancellation.Token), out pid))
            {
                if (run.IsCompleted)
                {
                    await run;
                    Assert.Fail("The adapter case ended before its child process reported execution.");
                }
                await Task.Delay(100, cancellation.Token);
            }
            child = Process.GetProcessById(pid);
            await cancellation.CancelAsync().WaitAsync(TimeSpan.FromSeconds(15));
            await Assert.ThrowsAsync<OperationCanceledException>(() => run.WaitAsync(TimeSpan.FromSeconds(15)));
            await child.WaitForExitAsync().WaitAsync(TimeSpan.FromSeconds(15));
        }
        finally
        {
            await cancellation.CancelAsync().WaitAsync(TimeSpan.FromSeconds(15));
            try
            {
                await run.WaitAsync(TimeSpan.FromSeconds(15));
            }
            catch (Exception) when (cancellation.IsCancellationRequested)
            {
            }
            if (child is not null)
            {
                if (!child.HasExited)
                {
                    child.Kill(entireProcessTree: true);
                    await child.WaitForExitAsync().WaitAsync(TimeSpan.FromSeconds(15));
                }
                child.Dispose();
            }
            Environment.SetEnvironmentVariable("AA98_XUNIT_DEBUG_CHILD_MARKER", previous);
        }
    }

    private static async Task VerifyAdapterDebugAsync(
        DotNetTestDiscoveryService tests, string adapter, string assembly, string root, DiscoveredTestCase selected)
    {
        string source = Path.Combine(root, "Tests.cs");
        int line = Array.FindIndex(File.ReadAllLines(source), text => text.Contains("void Values(", StringComparison.Ordinal)) + 1;
        Assert.IsTrue(line > 0);
        NetCoreDbgDebugSessionService debugger = new(adapter);
        TaskCompletionSource stopped = new(TaskCreationOptions.RunContinuationsAsynchronously);
        debugger.SessionEventReceived += (_, args) =>
        {
            if (args.EventName == "stopped" && args.Reason == "breakpoint")
            {
                stopped.TrySetResult();
            }
        };
        DebugSessionInfo? session = null;
        using CancellationTokenSource cancellation = new(TimeSpan.FromSeconds(60));
        Task<TestRunSummary> run = tests.DebugCaseAsync(assembly, root, selected, async (pid, token) =>
        {
            session = await debugger.StartSessionAsync(new DebugLaunchRequest
            {
                AttachToExistingProcess = true,
                ProcessId = pid,
                WorkingDirectory = root,
            }, token);
            try
            {
                await debugger.SetBreakpointsAsync(session.SessionId, source, [line], token);
                return new AdapterDebugLease(debugger, session.SessionId);
            }
            catch
            {
                await debugger.StopSessionAsync(session.SessionId);
                throw;
            }
        }, cancellation.Token);
        try
        {
            Task first = await Task.WhenAny(stopped.Task, run).WaitAsync(cancellation.Token);
            if (ReferenceEquals(first, run))
            {
                await run;
                Assert.Fail("The adapter case completed without reaching its breakpoint.");
            }
            await stopped.Task;
            Assert.IsNotNull(session);
            Assert.IsTrue((await debugger.GetStackTraceAsync(session.SessionId, cancellation.Token)).Count > 0);
            await debugger.ContinueSessionAsync(session.SessionId, cancellation.Token);
            TestRunSummary result = await run.WaitAsync(cancellation.Token);
            Assert.AreEqual(1, result.PassedCount);
            Assert.AreEqual(selected.Id, result.Cases.Single().TestId);
        }
        finally
        {
            await cancellation.CancelAsync().WaitAsync(TimeSpan.FromSeconds(15));
            try
            {
                await run.WaitAsync(TimeSpan.FromSeconds(15));
            }
            catch (Exception) when (cancellation.IsCancellationRequested)
            {
            }
        }
    }

    private sealed class AdapterDebugLease(IDebugSessionService debugger, string sessionId) : IAsyncDisposable
    {
        public async ValueTask DisposeAsync() => await debugger.StopSessionAsync(sessionId);
    }

    private sealed class CallbackLease(Action release) : IAsyncDisposable
    {
        public ValueTask DisposeAsync()
        {
            release();
            return ValueTask.CompletedTask;
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

            const string middleSource = "public class Sample { void Run() { System.Console.WriteLine(); } }";
            int middlePosition = middleSource.IndexOf("WriteLine", StringComparison.Ordinal) + "WriteL".Length;
            IReadOnlyList<CSharpCompletionSuggestion> middleItems = await service.GetCompletionItemsAsync(
                middleSource, sourcePath, middlePosition, workspace);
            CSharpCompletionEdit middleEdit = await service.GetCompletionEditAsync(
                middleItems.First(item => item.DisplayText == "WriteLine"));
            Assert.AreEqual(middleSource, middleSource.Remove(middleEdit.Start, middleEdit.Length)
                .Insert(middleEdit.Start, middleEdit.NewText));
            Assert.AreEqual(middleSource.IndexOf("WriteLine", StringComparison.Ordinal) + "WriteLine".Length, middleEdit.CaretOffset);

            const string escapedSource = "public class Sample { public int @class; void Run() { this.@cl; } }";
            int escapedPosition = escapedSource.IndexOf("this.@cl", StringComparison.Ordinal) + "this.@cl".Length;
            IReadOnlyList<CSharpCompletionSuggestion> escapedItems = await service.GetCompletionItemsAsync(
                escapedSource, sourcePath, escapedPosition, workspace);
            Assert.IsTrue(escapedItems.Any(item => item.DisplayText.Contains("class", StringComparison.Ordinal)),
                $"Escaped completion candidates: {string.Join(", ", escapedItems.Select(item => item.DisplayText))}");
            CSharpCompletionEdit escapedEdit = await service.GetCompletionEditAsync(
                escapedItems.First(item => item.DisplayText.Contains("class", StringComparison.Ordinal)));
            StringAssert.Contains(escapedSource.Remove(escapedEdit.Start, escapedEdit.Length)
                .Insert(escapedEdit.Start, escapedEdit.NewText), "this.@class;");
            Assert.AreEqual("public class Sample { }", await File.ReadAllTextAsync(sourcePath));
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
