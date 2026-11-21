using AA98_AvlnCodeStudio.AxamlDesigner.UI.Views;
using AA98_AvlnCodeStudio.Model.Workspaces;
using AA98_AxamlDesigner.Host.ViewModels;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

internal static partial class Program
{
    private static async Task VerifyStandaloneBindingsAsync(IClassicDesktopStyleApplicationLifetime desktop)
    {
        string directory = Path.Combine(s_root, "standalone-binding");
        Directory.CreateDirectory(directory);
        string project = Path.Combine(directory, "Sample.csproj");
        string markupPath = Path.Combine(directory, "LoginView.axaml");
        string codePath = Path.Combine(directory, "LoginView.axaml.cs");
        string modelPath = Path.Combine(directory, "LoginViewViewModel.cs");
        string sentinelPath = Path.Combine(directory, "Unrelated.txt");
        File.WriteAllText(Path.Combine(directory, "Directory.Build.props"), """
            <Project>
              <PropertyGroup>
                <BaseOutputPath>$(MSBuildThisFileDirectory)..\standalone-fixture-artifacts\bin\$(MSBuildProjectName)\</BaseOutputPath>
                <BaseIntermediateOutputPath>$(MSBuildThisFileDirectory)..\standalone-fixture-artifacts\obj\$(MSBuildProjectName)\</BaseIntermediateOutputPath>
              </PropertyGroup>
            </Project>
            """);
        File.WriteAllText(project, """
            <Project Sdk="Microsoft.NET.Sdk">
              <PropertyGroup><TargetFramework>net10.0</TargetFramework><ManagePackageVersionsCentrally>false</ManagePackageVersionsCentrally></PropertyGroup>
              <ItemGroup>
                <PackageReference Include="Avalonia" Version="12.1.5" />
                <PackageReference Include="CommunityToolkit.Mvvm" Version="8.4.2" />
              </ItemGroup>
            </Project>
            """);
        File.WriteAllText(codePath, "using Avalonia.Controls; namespace Sample; public partial class LoginView : Window { }");
        const string inputMarkup = """
            <Window xmlns="https://github.com/avaloniaui"
                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                    x:Class="Sample.LoginView">
              <StackPanel>
                <TextBox x:Name="NameTextBox" Text="Ada" />
                <Button x:Name="SubmitButton" Content="Submit" />
              </StackPanel>
            </Window>
            """;
        WorkspaceFileChangeRequest[] generated = new AxamlMvvmGenerator().Generate("LoginView.axaml", inputMarkup).ToArray();
        Check(generated.Length == 2, "Fixture generation failed.");
        string generatedMarkup = generated[0].ProposedContent
            .Replace("{Binding Name,", "{CompiledBinding Name,", StringComparison.Ordinal)
            .Replace("{Binding SubmitCommand}", "{ReflectionBinding SubmitCommand}", StringComparison.Ordinal);
        Check(generatedMarkup.Contains("{CompiledBinding Name,", StringComparison.Ordinal)
            && generatedMarkup.Contains("{ReflectionBinding SubmitCommand}", StringComparison.Ordinal),
            "Could not prepare generated binding fixture.");
        File.WriteAllText(markupPath, generatedMarkup);
        File.WriteAllText(modelPath, "// preserve existing members\n" + generated[1].ProposedContent);
        File.WriteAllText(sentinelPath, "preserve");
        string originalCode = File.ReadAllText(codePath);
        await RunStandaloneFixtureBuildAsync(project, "restore");
        await RunStandaloneFixtureBuildAsync(project, "build");

        AxamlDesignerHostViewModel host = new();
        AA98_AxamlDesigner.Host.Views.MainWindow window = new() { DataContext = host };
        window.Show();
        Check(host.OpenDocument(markupPath) && host.SetWorkspaceEntry(project), "Standalone fixture could not be opened.");
        await WaitUntilAsync(() => host.Designer.PreviewRoot is not null, "standalone binding preview");
        await VerifyStandaloneBindingEditAsync(desktop, host, markupPath, modelPath, "NameTextBox", "Text", "CompiledBinding", "VisualName", "Name");
        await VerifyStandaloneBindingEditAsync(desktop, host, markupPath, modelPath, "SubmitButton", "Command", "ReflectionBinding", "VisualCommand", "SubmitCommand");
        Check(File.ReadAllText(codePath) == originalCode && File.ReadAllText(sentinelPath) == "preserve",
            "Standalone binding edits changed unrelated sources.");
        await RunStandaloneFixtureBuildAsync(project, "build");

        string safeMarkup = host.Markup;
        string safeModel = File.ReadAllText(modelPath);
        foreach (string unsafeMarkup in new[]
        {
            safeMarkup.Replace("<TextBox x:Name=\"NameTextBox\"", "<TextBox DataContext=\"{Binding Name}\" x:Name=\"NameTextBox\"", StringComparison.Ordinal),
            safeMarkup.Replace("<TextBox x:Name=\"NameTextBox\"", "<TextBox x:DataType=\"vm:OtherViewModel\" x:Name=\"NameTextBox\"", StringComparison.Ordinal),
            safeMarkup.Replace("{CompiledBinding VisualName, Mode=TwoWay}", "{CompiledBinding VisualName, Source={x:Reference SubmitButton}}", StringComparison.Ordinal),
        })
        {
            Check(unsafeMarkup != safeMarkup, "Could not prepare unsafe binding scope.");
            host.Markup = unsafeMarkup;
            host.Designer.SelectedNode = host.Designer.PreviewRoot!.Children.Single(node => node.StableName == "NameTextBox");
            var row = host.Designer.BindingRows.Single(binding => binding.PropertyName == "Text");
            Check(!row.CanEditBinding, "Unsafe binding scope was editable.");
            row.NewMemberName = "Forbidden";
            await host.Designer.ApplyBindingRowAsync(row);
            Check(host.Markup == unsafeMarkup && File.ReadAllText(markupPath) == safeMarkup
                && File.ReadAllText(modelPath) == safeModel, "Unsafe binding changed source.");
        }
        XDocument foreign = XDocument.Parse(safeMarkup);
        XElement root = foreign.Root!;
        XElement context = root.Element(root.Name.Namespace + "Window.DataContext")!.Elements().Single();
        context.Name = context.Name.Namespace + "OtherViewModel";
        host.Markup = foreign.ToString();
        string foreignMarkup = host.Markup;
        host.Designer.SelectedNode = host.Designer.PreviewRoot!.Children.Single(node => node.StableName == "NameTextBox");
        var foreignRow = host.Designer.BindingRows.Single(row => row.PropertyName == "Text");
        foreignRow.NewMemberName = "Forbidden";
        await host.Designer.ApplyBindingRowAsync(foreignRow);
        Check(!desktop.Windows.OfType<WorkspaceFileChangeReviewDialog>().Any()
            && host.Markup == foreignMarkup && File.ReadAllText(markupPath) == safeMarkup && File.ReadAllText(modelPath) == safeModel
            && !string.IsNullOrWhiteSpace(foreignRow.Diagnostic),
            "A foreign root context was accepted for source editing.");
        host.Markup = safeMarkup;
        Check(!host.IsDirty, "Restoring the fixture did not clear the dirty state.");
        window.DataContext = null;
        window.Close();
        Console.WriteLine("Standalone binding: 2 reviewed binding kinds, source preservation, undo/redo and 4 unsafe contexts verified.");
    }

    private static async Task VerifyStandaloneBindingEditAsync(
        IClassicDesktopStyleApplicationLifetime desktop, AxamlDesignerHostViewModel host,
        string markupPath, string modelPath, string control, string property, string kind, string replacement, string existing)
    {
        string beforeMarkup = File.ReadAllText(markupPath);
        string beforeModel = File.ReadAllText(modelPath);
        host.Designer.SelectedNode = host.Designer.PreviewRoot!.Children.Single(node => node.StableName == control);
        try
        {
            await WaitUntilAsync(() => host.Designer.BindingRows.Single(row => row.PropertyName == property)
                .AvailableMembers.Contains(existing), $"standalone semantic context for {control}");
        }
        catch (TimeoutException exception)
        {
            var pending = host.Designer.BindingRows.Single(row => row.PropertyName == property);
            throw new InvalidOperationException(
                $"Standalone semantic context: {pending.Diagnostic}; members: {string.Join(", ", pending.AvailableMembers)}; host: {host.StatusText}", exception);
        }
        var row = host.Designer.BindingRows.Single(binding => binding.PropertyName == property);
        Check(row.CanEditBinding, $"Generated root context blocked {kind}: {row.BindingExpressionDiagnostic}");
        row.NewMemberName = replacement;
        Task applying = host.Designer.ApplyBindingRowAsync(row);
        await WaitUntilAsync(() => applying.IsCompleted || desktop.Windows.OfType<WorkspaceFileChangeReviewDialog>().Any(),
            $"standalone {kind} review");
        WorkspaceFileChangeReviewDialog? review = desktop.Windows.OfType<WorkspaceFileChangeReviewDialog>().SingleOrDefault();
        Check(review is not null, $"No standalone {kind} review: {row.Diagnostic}; {host.StatusText}");
        Click(review!, "Cancel");
        await applying.WaitAsync(TimeSpan.FromMinutes(2));
        Check(File.ReadAllText(markupPath) == beforeMarkup && File.ReadAllText(modelPath) == beforeModel
            && host.Markup == beforeMarkup && !host.IsDirty, $"Standalone {kind} cancellation changed source.");
        applying = host.Designer.ApplyBindingRowAsync(row);
        await WaitUntilAsync(() => applying.IsCompleted || desktop.Windows.OfType<WorkspaceFileChangeReviewDialog>().Any(),
            $"standalone {kind} apply review");
        review = desktop.Windows.OfType<WorkspaceFileChangeReviewDialog>().SingleOrDefault();
        Check(review is not null, $"No repeated standalone {kind} review: {row.Diagnostic}; {host.StatusText}");
        Click(review!, "Apply 2 change(s)");
        await applying.WaitAsync(TimeSpan.FromMinutes(2));
        string afterMarkup = File.ReadAllText(markupPath);
        string afterModel = File.ReadAllText(modelPath);
        Check(afterMarkup == beforeMarkup.Replace("{" + kind + " " + existing,
                "{" + kind + " " + replacement, StringComparison.Ordinal)
            && afterModel.Contains(replacement == "VisualCommand" ? "private void Visual()" : replacement, StringComparison.Ordinal)
            && afterModel.Contains("// preserve existing members", StringComparison.Ordinal)
            && host.Markup == afterMarkup && !host.IsDirty,
            $"Standalone {kind} apply failed: {row.Diagnostic}; {host.StatusText}");
        Check(host.WorkspaceFileChangeService.Undo() && File.ReadAllText(markupPath) == beforeMarkup
            && File.ReadAllText(modelPath) == beforeModel && host.Markup == beforeMarkup && !host.IsDirty,
            $"Standalone {kind} undo failed.");
        Check(host.WorkspaceFileChangeService.Redo() && File.ReadAllText(markupPath) == afterMarkup
            && File.ReadAllText(modelPath) == afterModel && host.Markup == afterMarkup && !host.IsDirty,
            $"Standalone {kind} redo failed.");
    }

    private static async Task RunStandaloneFixtureBuildAsync(string project, string command)
    {
        string repository = Environment.CurrentDirectory;
        bool windows = OperatingSystem.IsWindows();
        string wrapper = Path.Combine(repository, "tools", "build", windows ? "dotnet-isolated.ps1" : "dotnet-isolated.sh");
        ProcessStartInfo start = new(windows ? "powershell.exe" : "bash")
        {
            WorkingDirectory = repository,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
        };
        string[] arguments =
        [
            wrapper, command, project,
            "-p:HostArtifactRoot=" + Path.Combine(s_root, "standalone-fixture-artifacts"),
            "--verbosity", "quiet",
        ];
        if (windows)
        {
            start.ArgumentList.Add("-NoProfile");
            start.ArgumentList.Add("-Command");
            string quoted = string.Join(" ", arguments.Select(argument =>
                "'" + argument.Replace("'", "''", StringComparison.Ordinal) + "'"));
            start.ArgumentList.Add($"& {quoted}; exit $LASTEXITCODE");
        }
        else
        {
            foreach (string argument in arguments)
                start.ArgumentList.Add(argument);
        }
        foreach (string name in new[] { "MSBuildSDKsPath", "MSBuildExtensionsPath", "MSBuildExtensionsPath32", "MSBuildToolsPath", "DOTNET_MSBUILD_SDK_RESOLVER_SDKS_DIR", "DOTNET_MSBUILD_SDK_RESOLVER_CLI_DIR" })
            start.Environment.Remove(name);
        start.Environment["AVALONIA_TELEMETRY_OPTOUT"] = "1";
        using Process process = Process.Start(start) ?? throw new InvalidOperationException("Could not start fixture build.");
        Task<string> stdout = process.StandardOutput.ReadToEndAsync();
        Task<string> stderr = process.StandardError.ReadToEndAsync();
        try
        {
            await process.WaitForExitAsync().WaitAsync(TimeSpan.FromMinutes(3));
        }
        catch (TimeoutException)
        {
            if (!process.HasExited)
            {
                process.Kill(entireProcessTree: true);
                await process.WaitForExitAsync();
            }
            throw;
        }
        string output = await stdout + await stderr;
        Check(process.ExitCode == 0, $"Standalone fixture {command} failed:{Environment.NewLine}{output}");
    }
}
