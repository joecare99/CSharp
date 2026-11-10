using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AA98_AvlnCodeStudio.AxamlDesigner.UI.Views;
using AA98_AvlnCodeStudio.UI;
using AA98_AvlnCodeStudio.UI.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Themes.Fluent;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Microsoft.Extensions.DependencyInjection;

internal static class Program
{
    private static string s_root = null!;

    [STAThread]
    private static int Main(string[] args)
    {
        s_root = Directory.CreateTempSubdirectory("AvlnWorkbenchMvvm-").FullName;
        try
        {
            Environment.SetEnvironmentVariable("CONFIG_ROOT", Path.Combine(s_root, "config"));
            Environment.SetEnvironmentVariable("AVLN_CONFIG_GROUP_ROOT", null);
            Environment.SetEnvironmentVariable("AVLN_CONFIG_GROUP_ID", null);

            string fixtureDirectory = Path.Combine(s_root, "Fixture");
            Directory.CreateDirectory(fixtureDirectory);
            string projectPath = Path.Combine(fixtureDirectory, "Sample.csproj");
            string axamlPath = Path.Combine(fixtureDirectory, "LoginView.axaml");
            File.WriteAllText(
                projectPath,
                """
                <Project Sdk="Microsoft.NET.Sdk">
                  <PropertyGroup><TargetFramework>net10.0</TargetFramework></PropertyGroup>
                  <ItemGroup>
                    <PackageReference Include="Avalonia" Version="12.1.5" />
                    <PackageReference Include="Avalonia.Controls.ColorPicker" Version="12.1.5" />
                    <PackageReference Include="CommunityToolkit.Mvvm" Version="8.4.2" />
                  </ItemGroup>
                </Project>
                """);
            File.WriteAllText(
                Path.Combine(fixtureDirectory, "LoginView.axaml.cs"),
                "using Avalonia.Controls; namespace Sample; public partial class LoginView : Window { }");
            File.WriteAllText(
                axamlPath,
                """
                <Window xmlns="https://github.com/avaloniaui"
                        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                        x:Class="Sample.LoginView">
                  <StackPanel>
                    <AutoCompleteBox x:Name="CityAutoCompleteBox" Text="Paris" />
                    <TextBox x:Name="UserNameTextBox" Text="Ada" />
                    <ColorPicker x:Name="ThemeColorPicker" Color="#FF123456" />
                    <TabControl x:Name="SettingsTabControl" SelectedIndex="1">
                      <TabItem Header="General" />
                      <TabItem Header="Advanced" />
                    </TabControl>
                    <Menu>
                      <MenuItem x:Name="SaveMenuItem" Header="Save" />
                    </Menu>
                    <Button x:Name="SubmitButton" Content="Submit" />
                  </StackPanel>
                </Window>
                """);

            return AppBuilder.Configure<SmokeApplication>()
                .UsePlatformDetect()
                .AfterSetup(_ => Dispatcher.UIThread.Post(async () => await RunAsync(projectPath, axamlPath)))
                .StartWithClassicDesktopLifetime(args);
        }
        finally
        {
            SmokeApplication.Services?.Dispose();
            Directory.Delete(s_root, recursive: true);
        }
    }

    private static async Task RunAsync(string projectPath, string axamlPath)
    {
        IClassicDesktopStyleApplicationLifetime desktop =
            (IClassicDesktopStyleApplicationLifetime)Application.Current!.ApplicationLifetime!;
        int exitCode = 0;
        try
        {
            MainWindowViewModel workbench = SmokeApplication.Services!.GetRequiredService<MainWindowViewModel>();
            await workbench.OpenWorkspaceFileAsync(projectPath);
            Check(workbench.Workspace?.Projects.Count == 1, "The fixture project was not opened in the Workbench.");
            await workbench.EditorWorkspace.OpenOrActivateAsync(axamlPath);
            await WaitUntilAsync(
                () => workbench.GenerateAxamlMvvmCommand.CanExecute(null),
                "Workbench MVVM generation becoming available");

            Task generating = workbench.GenerateAxamlMvvmCommand.ExecuteAsync(null);
            WorkspaceFileChangeReviewDialog review = await WaitForReviewAsync(desktop, generating, workbench);
            Click(review, "Apply 2 change(s)");
            await generating.WaitAsync(TimeSpan.FromMinutes(2));

            string generatedViewModelPath = Path.Combine(Path.GetDirectoryName(axamlPath)!, "LoginViewViewModel.cs");
            string originalMarkup = """
                <Window xmlns="https://github.com/avaloniaui"
                        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                        x:Class="Sample.LoginView">
                  <StackPanel>
                    <AutoCompleteBox x:Name="CityAutoCompleteBox" Text="Paris" />
                    <TextBox x:Name="UserNameTextBox" Text="Ada" />
                    <ColorPicker x:Name="ThemeColorPicker" Color="#FF123456" />
                    <TabControl x:Name="SettingsTabControl" SelectedIndex="1">
                      <TabItem Header="General" />
                      <TabItem Header="Advanced" />
                    </TabControl>
                    <Menu>
                      <MenuItem x:Name="SaveMenuItem" Header="Save" />
                    </Menu>
                    <Button x:Name="SubmitButton" Content="Submit" />
                  </StackPanel>
                </Window>
                """;
            string generatedMarkup = await File.ReadAllTextAsync(axamlPath);
            string generatedViewModel = await File.ReadAllTextAsync(generatedViewModelPath);
            Check(generatedMarkup.Contains("Text=\"{Binding UserName, Mode=TwoWay}\"", StringComparison.Ordinal),
                "The approved proposal did not bind the TextBox.");
            Check(generatedMarkup.Contains("Text=\"{Binding City, Mode=TwoWay}\"", StringComparison.Ordinal),
                "The approved proposal did not bind the AutoCompleteBox.");
            Check(generatedMarkup.Contains("Command=\"{Binding SubmitCommand}\"", StringComparison.Ordinal),
                "The approved proposal did not bind the Button command.");
            Check(generatedMarkup.Contains("Command=\"{Binding SaveCommand}\"", StringComparison.Ordinal),
                "The approved proposal did not bind the MenuItem command.");
            Check(generatedMarkup.Contains("Color=\"{Binding Theme, Mode=TwoWay}\"", StringComparison.Ordinal),
                "The approved proposal did not bind the ColorPicker.");
            Check(generatedMarkup.Contains("SelectedIndex=\"{Binding Settings, Mode=TwoWay}\"", StringComparison.Ordinal),
                "The approved proposal did not bind the TabControl selection.");
            Check(generatedViewModel.Contains("private string _userName = \"Ada\";", StringComparison.Ordinal),
                "The generated ViewModel did not preserve the TextBox's initial value.");
            Check(generatedViewModel.Contains("private string? _city = \"Paris\";", StringComparison.Ordinal),
                "The generated ViewModel did not preserve the AutoCompleteBox's initial value.");
            Check(generatedViewModel.Contains(
                    "private global::Avalonia.Media.Color _theme = global::Avalonia.Media.Color.Parse(\"#FF123456\");",
                    StringComparison.Ordinal),
                "The generated ViewModel did not preserve the ColorPicker's initial color.");
            Check(generatedViewModel.Contains("private int _settings = 1;", StringComparison.Ordinal),
                    "The generated ViewModel did not preserve the selected tab index.");
            Check(generatedViewModel.Contains("private void Save()", StringComparison.Ordinal),
                "The generated ViewModel did not include the MenuItem command.");
            Check(!workbench.WorkspaceFileChangeService.CanRedo, "The applied proposal unexpectedly has redo history.");

            Check(workbench.WorkspaceFileChangeService.Undo(), "The generated files could not be undone.");
            Check(await File.ReadAllTextAsync(axamlPath) == originalMarkup, "Undo did not restore the original AXAML.");
            Check(!File.Exists(generatedViewModelPath), "Undo did not remove the newly generated ViewModel.");
            Check(workbench.Editor.Text == originalMarkup, "Undo did not synchronize the open AXAML document.");
            Check(workbench.WorkspaceFileChangeService.Redo(), "The generated files could not be redone.");
            Check(await File.ReadAllTextAsync(axamlPath) == generatedMarkup, "Redo did not restore the approved AXAML.");
            Check(await File.ReadAllTextAsync(generatedViewModelPath) == generatedViewModel,
                "Redo did not restore the generated ViewModel.");

            await BuildFixtureAsync(projectPath);
            Console.WriteLine("Workbench MVVM: review, two-file apply, editor synchronization, undo/redo and generated-project build verified.");
        }
        catch (Exception exception)
        {
            Console.Error.WriteLine(exception);
            exitCode = 1;
        }

        desktop.Shutdown(exitCode);
    }

    private static async Task BuildFixtureAsync(string projectPath)
    {
        string dotnetHost = ResolveDotnetHost();
        ProcessStartInfo startInfo = new(dotnetHost)
        {
            WorkingDirectory = Path.GetDirectoryName(projectPath)!,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
        };
        startInfo.ArgumentList.Add("build");
        startInfo.ArgumentList.Add(projectPath);
        startInfo.ArgumentList.Add("--verbosity");
        startInfo.ArgumentList.Add("quiet");
        startInfo.Environment.Remove("MSBuildSDKsPath");
        startInfo.Environment.Remove("MSBuildExtensionsPath");
        startInfo.Environment.Remove("MSBuildExtensionsPath32");
        startInfo.Environment.Remove("MSBuildToolsPath");
        startInfo.Environment.Remove("DOTNET_MSBUILD_SDK_RESOLVER_SDKS_DIR");
        startInfo.Environment.Remove("DOTNET_MSBUILD_SDK_RESOLVER_CLI_DIR");
        using Process process = Process.Start(startInfo)
            ?? throw new InvalidOperationException("Could not start the generated MVVM fixture build.");
        Task<string> outputTask = process.StandardOutput.ReadToEndAsync();
        Task<string> errorTask = process.StandardError.ReadToEndAsync();
        try
        {
            await process.WaitForExitAsync().WaitAsync(TimeSpan.FromMinutes(2));
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

        string output = (await outputTask) + (await errorTask);
        Check(process.ExitCode == 0, $"The generated ViewModel fixture did not build:{Environment.NewLine}{output}");
    }

    private static string ResolveDotnetHost()
    {
        string? configuredHost = Environment.GetEnvironmentVariable("DOTNET_HOST_PATH");
        if (!string.IsNullOrWhiteSpace(configuredHost) && File.Exists(configuredHost))
        {
            return configuredHost;
        }

        string? dotnetRoot = Environment.GetEnvironmentVariable("DOTNET_ROOT");
        if (!string.IsNullOrWhiteSpace(dotnetRoot))
        {
            string rootedHost = Path.Combine(dotnetRoot, OperatingSystem.IsWindows() ? "dotnet.exe" : "dotnet");
            if (File.Exists(rootedHost))
            {
                return rootedHost;
            }
        }

        if (OperatingSystem.IsLinux())
        {
            string userHost = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                ".dotnet",
                "dotnet");
            if (File.Exists(userHost))
            {
                return userHost;
            }
        }

        string? processPath = Environment.ProcessPath;
        if (!string.IsNullOrWhiteSpace(processPath)
            && string.Equals(
                Path.GetFileNameWithoutExtension(processPath),
                "dotnet",
                StringComparison.OrdinalIgnoreCase))
        {
            return processPath;
        }

        return "dotnet";
    }

    private static async Task<WorkspaceFileChangeReviewDialog> WaitForReviewAsync(
        IClassicDesktopStyleApplicationLifetime desktop,
        Task generating,
        MainWindowViewModel workbench)
    {
        await WaitUntilAsync(
            () => generating.IsCompleted || desktop.Windows.OfType<WorkspaceFileChangeReviewDialog>().Any(),
            "workspace review dialog",
            attempts: 1200);
        WorkspaceFileChangeReviewDialog? review = desktop.Windows.OfType<WorkspaceFileChangeReviewDialog>().SingleOrDefault();
        if (review is null)
        {
            await generating;
            throw new InvalidOperationException(
                $"MVVM generation ended before review: {workbench.Editor.StatusText}{Environment.NewLine}{workbench.Editor.NotificationText}");
        }

        return review;
    }

    private static void Click(Window window, string label)
    {
        Button button = window.GetVisualDescendants().OfType<Button>()
            .Single(candidate => string.Equals(candidate.Content?.ToString(), label, StringComparison.Ordinal));
        button.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
    }

    private static async Task WaitUntilAsync(Func<bool> condition, string description, int attempts = 300)
    {
        for (int attempt = 0; attempt < attempts; attempt++)
        {
            if (condition()) return;
            await Task.Delay(100);
        }

        throw new TimeoutException($"Timed out waiting for {description}.");
    }

    private static void Check(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }

    private sealed class SmokeApplication : Application
    {
        public static ServiceProvider? Services { get; private set; }

        public override void Initialize()
        {
            Styles.Add(new FluentTheme());
            Styles.Add(new StyleInclude(new Uri("avares://AvaloniaEdit/"))
            {
                Source = new Uri("avares://AvaloniaEdit/Themes/Fluent/AvaloniaEdit.xaml"),
            });
        }

        public override void OnFrameworkInitializationCompleted()
        {
            IClassicDesktopStyleApplicationLifetime desktop =
                (IClassicDesktopStyleApplicationLifetime)ApplicationLifetime!;
            Services = App.CreateServiceProvider(
                () => desktop.MainWindow,
                userConfigRoot: Path.Combine(s_root, "config"),
                systemConfigRoot: Path.Combine(s_root, "system"));
            desktop.MainWindow = App.CreateMainWindow(Services);
            base.OnFrameworkInitializationCompleted();
        }
    }
}
