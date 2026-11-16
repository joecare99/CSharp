using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using AA98_AvlnCodeStudio.Decompilation;
using AA98_AvlnCodeStudio.UI;
using AA98_AvlnCodeStudio.UI.Controls;
using AA98_AvlnCodeStudio.UI.Services;
using AA98_AvlnCodeStudio.UI.ViewModels;
using AA98_AvlnCodeStudio.UI.Views;
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
    private static ControlledService s_service = null!;
    private static ControlledPicker s_picker = null!;
    private static ServiceProvider s_services = null!;
    private static readonly string s_typeName = typeof(DecompilationSmokeFixture).FullName!;

    [STAThread]
    private static int Main()
    {
        s_root = Path.Combine(Environment.CurrentDirectory, "artifacts", "workbench-decompile-smoke-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(s_root);
        Environment.SetEnvironmentVariable("AVLN_CONFIG_GROUP_ROOT", null);
        Environment.SetEnvironmentVariable("AVLN_CONFIG_GROUP_ID", null);
        try
        {
            return AppBuilder.Configure<SmokeApplication>()
                .UsePlatformDetect()
                .AfterSetup(_ => Dispatcher.UIThread.Post(RunAsync))
                .StartWithClassicDesktopLifetime([]);
        }
        finally
        {
            s_services?.Dispose();
            if (Directory.Exists(s_root)) Directory.Delete(s_root, true);
        }
    }

    private static async void RunAsync()
    {
        var desktop = (IClassicDesktopStyleApplicationLifetime)Application.Current!.ApplicationLifetime!;
        int exitCode = 0;
        try
        {
            Window window = desktop.MainWindow!;
            var workbench = s_services.GetRequiredService<MainWindowViewModel>();
            string originalAssembly = typeof(DecompilationSmokeFixture).Assembly.Location;
            byte[] originalHash = SHA256.HashData(File.ReadAllBytes(originalAssembly));
            string manualPath = Path.Combine(s_root, "manual.cs");
            File.WriteAllText(manualPath, "public class ManualDocument { }\n");
            await workbench.EditorWorkspace.OpenOrActivateAsync(manualPath);
            EditorTabViewModel manualTab = workbench.ActiveEditorTab;

            DecompilationDialog dialog = await OpenDialogAsync(desktop, window);
            await ChooseTypeAsync(dialog);
            Click(Button(dialog, "Open source"));
            try
            {
                await WaitAsync(() => s_service.LastArtifact is { } artifact
                    && workbench.ActiveEditorTab.Editor.CurrentFilePath == artifact.NavigationLocation.Path, "editor navigation");
            }
            catch (TimeoutException)
            {
                throw new InvalidOperationException(
                    $"Navigation failed: artifact={s_service.LastArtifact?.ArtifactPath}, active={workbench.ActiveEditorTab.Editor.CurrentFilePath}, "
                    + $"editor status={workbench.Editor.StatusText}, dialog={dialog.IsVisible}, "
                    + $"dialog text={string.Join(" | ", dialog.GetVisualDescendants().OfType<TextBlock>().Select(t => t.Text))}");
            }
            DecompiledTypeArtifact artifact = s_service.LastArtifact!;
            Check(File.Exists(artifact.ArtifactPath), "Generated artifact is missing.");
            Check(File.ReadAllText(artifact.ArtifactPath).Contains("class DecompilationSmokeFixture", StringComparison.Ordinal),
                "The selected type is missing from the generated source.");
            await WaitAsync(() => workbench.ActiveEditorTab.TextView is EditorTextArea editor
                && editor.Text.Take(editor.CaretOffset).Count(c => c == '\n') + 1 == artifact.NavigationLocation.Line
                && editor.CaretOffset - editor.Text.LastIndexOf('\n', Math.Max(0, editor.CaretOffset - 1))
                    == artifact.NavigationLocation.Column,
                "caret at CodeLocation");
            Check(File.ReadAllText(manualPath) == "public class ManualDocument { }\n", "Manual source was modified.");
            Check(SHA256.HashData(File.ReadAllBytes(originalAssembly)).SequenceEqual(originalHash), "Assembly was modified.");

            dialog = await OpenDialogAsync(desktop, window);
            s_picker.CancelNext = true;
            int inspections = s_service.Inspections;
            Click(Button(dialog, "Choose assembly..."));
            await WaitAsync(() => !s_picker.CancelNext, "canceled picker");
            Check(s_service.Inspections == inspections && !Button(dialog, "Open source").IsEnabled,
                "Canceling file selection must not inspect or enable decompilation.");
            Click(Button(dialog, "Close"));
            await WaitAsync(() => !dialog.IsVisible, "closing canceled picker dialog");

            await CheckInterruptedAsync(desktop, window, workbench, manualTab, changeContext: false);
            await CheckInterruptedAsync(desktop, window, workbench, manualTab, changeContext: true);
            Console.WriteLine("PASS: real menu/dialog, controlled local picker, real assembly/type/service, editor and caret; picker cancel, close and stale-context cancellation.");
            window.Close();
        }
        catch (Exception exception)
        {
            Console.Error.WriteLine(exception);
            exitCode = 1;
        }
        finally
        {
            foreach (Window window in desktop.Windows.ToArray()) window.Close();
            desktop.Shutdown(exitCode);
        }
    }

    private static async Task CheckInterruptedAsync(
        IClassicDesktopStyleApplicationLifetime desktop, Window window, MainWindowViewModel workbench,
        EditorTabViewModel manualTab, bool changeContext)
    {
        s_service.HoldNext();
        DecompilationDialog dialog = await OpenDialogAsync(desktop, window);
        await ChooseTypeAsync(dialog);
        EditorTabViewModel before = workbench.ActiveEditorTab;
        Click(Button(dialog, "Open source"));
        await WaitAsync(() => s_service.IsHeld, "held decompilation");
        if (changeContext)
        {
            workbench.EditorWorkspace.Activate(manualTab);
        }
        else
        {
            Click(Button(dialog, "Close"));
        }

        await WaitAsync(() => !dialog.IsVisible, "dialog cancellation");
        Check(s_service.HeldToken.IsCancellationRequested, "Pending operation was not canceled.");
        s_service.Release();
        await WaitAsync(() => s_service.HeldCompleted, "late response completion");
        await Task.Delay(150);
        Check(ReferenceEquals(workbench.ActiveEditorTab, changeContext ? manualTab : before),
            "A late response navigated after cancellation or context change.");
    }

    private static async Task<DecompilationDialog> OpenDialogAsync(IClassicDesktopStyleApplicationLifetime desktop, Window window)
    {
        MenuItem menu = window.FindControl<MenuItem>("DecompileAssemblyMenuItem")!;
        menu.RaiseEvent(new RoutedEventArgs(MenuItem.ClickEvent));
        await WaitAsync(() => desktop.Windows.OfType<DecompilationDialog>().Any(dialog => dialog.IsVisible), "decompilation dialog");
        return desktop.Windows.OfType<DecompilationDialog>().Single(dialog => dialog.IsVisible);
    }

    private static async Task ChooseTypeAsync(DecompilationDialog dialog)
    {
        Click(Button(dialog, "Choose assembly..."));
        ListBox list = dialog.GetVisualDescendants().OfType<ListBox>().Single();
        await WaitAsync(() => list.ItemsSource is System.Collections.Generic.IEnumerable<AssemblyTypeInfo> types
            && types.Any(type => type.FullName == s_typeName), "assembly metadata");
        list.SelectedItem = ((System.Collections.Generic.IEnumerable<AssemblyTypeInfo>)list.ItemsSource!)
            .Single(type => type.FullName == s_typeName);
        await WaitAsync(() => Button(dialog, "Open source").IsEnabled, "selected type");
    }

    private static Button Button(Window window, string text) =>
        window.GetVisualDescendants().OfType<Button>().Single(button => Equals(button.Content, text));

    private static void Click(Button button) => button.RaiseEvent(new RoutedEventArgs(Avalonia.Controls.Button.ClickEvent));

    private static async Task WaitAsync(Func<bool> condition, string description)
    {
        for (int attempt = 0; attempt < 400; attempt++)
        {
            if (condition()) return;
            await Task.Delay(25);
        }
        throw new TimeoutException("Timed out waiting for " + description);
    }

    private static void Check(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }

    private sealed class SmokeApplication : Application
    {
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
            var desktop = (IClassicDesktopStyleApplicationLifetime)ApplicationLifetime!;
            ServiceCollection services = new();
            App.ConfigureServices(services, () => desktop.MainWindow,
                userConfigRoot: Path.Combine(s_root, "config"),
                systemConfigRoot: Path.Combine(s_root, "system"));
            s_service = new ControlledService(new AssemblyDecompilationService(Path.Combine(s_root, "generated")));
            services.AddSingleton<IAssemblyDecompilationService>(s_service);
            s_picker = new ControlledPicker();
            services.AddSingleton<IDecompilationAssemblyPicker>(s_picker);
            s_services = services.BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });
            desktop.MainWindow = App.CreateMainWindow(s_services);
            base.OnFrameworkInitializationCompleted();
        }
    }

    private sealed class ControlledPicker : IDecompilationAssemblyPicker
    {
        public bool CancelNext { get; set; }

        public Task<string?> PickAsync(Window owner)
        {
            if (CancelNext)
            {
                CancelNext = false;
                return Task.FromResult<string?>(null);
            }
            return Task.FromResult<string?>(typeof(DecompilationSmokeFixture).Assembly.Location);
        }
    }

    internal sealed class DecompilationSmokeFixture
    {
        public int Value() => 42;
    }

    private sealed class ControlledService(IAssemblyDecompilationService inner) : IAssemblyDecompilationService
    {
        private TaskCompletionSource<bool>? _gate;
        public bool IsHeld { get; private set; }
        public bool HeldCompleted { get; private set; }
        public int Inspections { get; private set; }
        public CancellationToken HeldToken { get; private set; }
        public DecompiledTypeArtifact? LastArtifact { get; private set; }

        public void HoldNext()
        {
            _gate = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            IsHeld = false;
            HeldCompleted = false;
        }

        public void Release() => _gate!.SetResult(true);

        public Task<AssemblyInspection> InspectAsync(string path, CancellationToken token = default)
        {
            Inspections++;
            return inner.InspectAsync(path, token);
        }

        public async Task<DecompiledTypeArtifact> DecompileTypeAsync(string path, string type, CancellationToken token = default)
        {
            DecompiledTypeArtifact result = await inner.DecompileTypeAsync(path, type, token);
            LastArtifact = result;
            if (_gate is { } gate)
            {
                HeldToken = token;
                IsHeld = true;
                await gate.Task;
                _gate = null;
                HeldCompleted = true;
            }
            return result;
        }
    }
}
