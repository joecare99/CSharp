using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AA98_AvlnCodeStudio.UI;
using AA98_AvlnCodeStudio.UI.Controls;
using AA98_AvlnCodeStudio.UI.ViewModels;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Themes.Fluent;
using Avalonia.Threading;
using AvaloniaEdit;
using Microsoft.Extensions.DependencyInjection;

internal static class Program
{
    private static string s_root = null!;

    [STAThread]
    private static int Main(string[] args)
    {
        string? adapterPath = Environment.GetEnvironmentVariable("AVLN_NETCOREDBG_PATH");
        if (string.IsNullOrWhiteSpace(adapterPath) || !File.Exists(adapterPath))
        {
            Console.Error.WriteLine("Set AVLN_NETCOREDBG_PATH to an installed NetCoreDbg executable.");
            return 2;
        }

        s_root = Directory.CreateTempSubdirectory("AvlnWorkbenchDebug-").FullName;
        try
        {
            Environment.SetEnvironmentVariable("CONFIG_ROOT", Path.Combine(s_root, "config"));
            Environment.SetEnvironmentVariable("AVLN_CONFIG_GROUP_ROOT", null);
            Environment.SetEnvironmentVariable("AVLN_CONFIG_GROUP_ID", null);

            string projectDirectory = Path.Combine(s_root, "Debuggee");
            Directory.CreateDirectory(projectDirectory);
            string projectPath = Path.Combine(projectDirectory, "Debuggee.csproj");
            string sourcePath = Path.Combine(projectDirectory, "Program.cs");
            File.WriteAllText(
                projectPath,
                """
                <Project Sdk="Microsoft.NET.Sdk">
                  <PropertyGroup>
                    <OutputType>Exe</OutputType>
                    <TargetFramework>net10.0</TargetFramework>
                    <ImplicitUsings>enable</ImplicitUsings>
                    <Nullable>enable</Nullable>
                    <DebugType>portable</DebugType>
                  </PropertyGroup>
                </Project>
                """);
            File.WriteAllText(
                sourcePath,
                """
                int answer = 40;
                answer += 2;
                Console.WriteLine(answer);
                int doubled = answer * 2;
                Thread.Sleep(TimeSpan.FromMinutes(2));
                """);

            return AppBuilder.Configure<SmokeApplication>()
                .UsePlatformDetect()
                .AfterSetup(_ => Dispatcher.UIThread.Post(RunAsync))
                .StartWithClassicDesktopLifetime(args);
        }
        finally
        {
            SmokeApplication.Services?.Dispose();
            Directory.Delete(s_root, recursive: true);
        }
    }

    private static async void RunAsync()
    {
        IClassicDesktopStyleApplicationLifetime desktop =
            (IClassicDesktopStyleApplicationLifetime)Application.Current!.ApplicationLifetime!;
        int exitCode = 0;
        MainWindowViewModel? workbench = null;
        try
        {
            workbench = SmokeApplication.Services!.GetRequiredService<MainWindowViewModel>();
            string projectPath = Path.Combine(s_root, "Debuggee", "Debuggee.csproj");
            string sourcePath = Path.Combine(s_root, "Debuggee", "Program.cs");
            await workbench.OpenWorkspaceFileAsync(projectPath);
            Check(workbench.Workspace?.Projects.Count == 1, "The debuggee project was not opened.");

            workbench.SelectedWorkflowProject = workbench.Workspace!.Projects.Single();
            await workbench.InspectSelectedProjectCommand.ExecuteAsync(null);
            Check(
                workbench.ProjectInspection?.TargetFramework == "net10.0",
                $"Project inspection did not resolve net10.0: {workbench.ProjectWorkflowStatus}");

            await workbench.BuildSelectedProjectCommand.ExecuteAsync(null);
            Check(
                workbench.ProjectWorkflowStatus.Contains("succeeded", StringComparison.OrdinalIgnoreCase),
                $"Selected-project build failed: {workbench.ProjectWorkflowStatus}");

            await workbench.EditorWorkspace.OpenOrActivateAsync(sourcePath);
            Check(
                workbench.EditorWorkspace.ActiveTab?.TextView is EditorTextArea,
                "The debuggee source was not opened in the Workbench editor.");
            EditorTextArea editor = (EditorTextArea)workbench.EditorWorkspace.ActiveTab!.TextView!;
            int breakpointOffset = (editor.Text ?? string.Empty).IndexOf("answer += 2;", StringComparison.Ordinal);
            Check(breakpointOffset >= 0, "The debuggee source text was not loaded into the editor.");

            await workbench.DebugSelectedProjectCommand.ExecuteAsync(null);
            Check(
                workbench.HasDebugSession
                    && workbench.ActiveDebugSessionState == AA98_AvlnCodeStudio.Base.Debugging.Models.DebugSessionState.Paused,
                $"Selected-project debugging did not stop at entry: {workbench.ProjectWorkflowStatus}");
            await WaitUntilAsync(
                () => workbench.DebugStackFrames.Count > 0,
                "entry stack frame");

            editor.SetCaretOffset(breakpointOffset);
            await workbench.ToggleBreakpointCommand.ExecuteAsync(null);
            Check(
                workbench.Editor.StatusText.Contains("Program.cs:2", StringComparison.Ordinal),
                $"The line-2 breakpoint was not set: {workbench.Editor.StatusText}");
            await workbench.ContinueDebugSessionCommand.ExecuteAsync(null);
            try
            {
                await WaitUntilAsync(
                    () => workbench.ActiveDebugSessionState
                            == AA98_AvlnCodeStudio.Base.Debugging.Models.DebugSessionState.Paused
                        && workbench.SelectedDebugStackFrame?.Line == 2,
                    "the line-2 breakpoint");
            }
            catch (TimeoutException exception)
            {
                string frames = string.Join(
                    ", ",
                    workbench.DebugStackFrames.Select(frame => $"{frame.Name}@{frame.SourcePath}:{frame.Line}"));
                throw new TimeoutException(
                    $"{exception.Message} State={workbench.ActiveDebugSessionState}; status={workbench.Editor.StatusText}; " +
                    $"notification={workbench.Editor.NotificationText}; frames=[{frames}]",
                    exception);
            }

            await workbench.StepOverCommand.ExecuteAsync(null);
            await WaitUntilAsync(
                () => workbench.ActiveDebugSessionState
                        == AA98_AvlnCodeStudio.Base.Debugging.Models.DebugSessionState.Paused
                    && workbench.SelectedDebugStackFrame?.Line == 3,
                "step-over to the variable initialization");
            await workbench.LoadDebugVariablesCommand.ExecuteAsync(null);
            string variables = string.Join(
                ", ",
                workbench.DebugVariables.Select(variable => $"{variable.Name}={variable.Value} ({variable.Type})"));
            Check(
                workbench.DebugVariables.Any(variable =>
                    string.Equals(variable.Name, "answer", StringComparison.Ordinal)
                    && string.Equals(variable.Value, "42", StringComparison.Ordinal)),
                $"The debugger did not expose answer=42 at the breakpoint. Locals: {variables}");

            await workbench.StepOverCommand.ExecuteAsync(null);
            await WaitUntilAsync(
                () => workbench.ActiveDebugSessionState
                        == AA98_AvlnCodeStudio.Base.Debugging.Models.DebugSessionState.Paused
                    && workbench.SelectedDebugStackFrame?.Line == 4,
                "step-over to the next source line");

            await workbench.ContinueDebugSessionCommand.ExecuteAsync(null);
            Check(
                workbench.ActiveDebugSessionState
                    == AA98_AvlnCodeStudio.Base.Debugging.Models.DebugSessionState.Running,
                "Continue did not resume the debuggee.");
            await workbench.StopDebugSessionCommand.ExecuteAsync(null);
            Check(!workbench.HasDebugSession, "Stop left a debug session active.");

            Console.WriteLine(
                "Workbench project debug: inspect, build, entry stop, breakpoint, stack/variables, step-over, continue and stop verified.");
        }
        catch (Exception exception)
        {
            Console.Error.WriteLine(exception);
            exitCode = 1;
        }
        finally
        {
            if (workbench?.HasDebugSession == true)
            {
                try
                {
                    await workbench.StopDebugSessionCommand.ExecuteAsync(null);
                }
                catch (Exception exception)
                {
                    Console.Error.WriteLine($"Could not stop the debug session during smoke cleanup: {exception}");
                    exitCode = 1;
                }
            }

            desktop.Shutdown(exitCode);
        }
    }

    private static async Task WaitUntilAsync(Func<bool> condition, string description)
    {
        for (int attempt = 0; attempt < 300; attempt++)
        {
            if (condition())
            {
                return;
            }

            await Task.Delay(100);
        }

        throw new TimeoutException($"Timed out waiting for {description}.");
    }

    private static void Check(bool condition, string message)
    {
        if (!condition)
        {
            throw new InvalidOperationException(message);
        }
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
