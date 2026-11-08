using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AA98_AvlnCodeStudio.AxamlDesigner.UI.Views;
using AA98_AvlnCodeStudio.Model.Workspaces;
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
using Config.UI.Avalonia.ViewModels;
using Config.UI.Avalonia.Views;
using Microsoft.Extensions.DependencyInjection;

internal static class Program
{
    private static string s_root = null!;

    [STAThread]
    private static int Main(string[] args)
    {
        s_root = Directory.CreateTempSubdirectory("AvlnProjectSettingsReview-").FullName;
        try
        {
            Environment.SetEnvironmentVariable("CONFIG_ROOT", Path.Combine(s_root, "config"));
            Environment.SetEnvironmentVariable("AVLN_CONFIG_GROUP_ROOT", null);
            Environment.SetEnvironmentVariable("AVLN_CONFIG_GROUP_ID", null);

            string projectPath = Path.Combine(s_root, "ReviewSample.csproj");
            string otherDirectory = Path.Combine(s_root, "Other");
            Directory.CreateDirectory(otherDirectory);
            File.WriteAllText(projectPath,
                "<Project Sdk=\"Microsoft.NET.Sdk\"><PropertyGroup><TargetFramework>net10.0</TargetFramework><AssemblyName>OriginalName</AssemblyName></PropertyGroup></Project>");
            File.WriteAllText(Path.Combine(otherDirectory, "Other.csproj"),
                "<Project Sdk=\"Microsoft.NET.Sdk\"><PropertyGroup><TargetFramework>net10.0</TargetFramework><AssemblyName>Other</AssemblyName></PropertyGroup></Project>");

            return AppBuilder.Configure<SmokeApplication>()
                .UsePlatformDetect()
                .AfterSetup(_ => Dispatcher.UIThread.Post(async () => await RunAsync(projectPath)))
                .StartWithClassicDesktopLifetime(args);
        }
        finally
        {
            SmokeApplication.Services?.Dispose();
            Directory.Delete(s_root, recursive: true);
        }
    }

    private static async Task RunAsync(string projectPath)
    {
        IClassicDesktopStyleApplicationLifetime desktop =
            (IClassicDesktopStyleApplicationLifetime)Application.Current!.ApplicationLifetime!;
        int exitCode = 0;
        try
        {
            MainWindowViewModel workbench = SmokeApplication.Services!.GetRequiredService<MainWindowViewModel>();
            string original = await File.ReadAllTextAsync(projectPath);
            await workbench.OpenWorkspaceFileAsync(projectPath);
            Check(workbench.Workspace?.Projects.Count == 1, "The fixture project was not opened.");

            Task showing = workbench.OpenProjectConfigurationDialogAsync!();
            await WaitUntilAsync(() => desktop.Windows.OfType<ConfigUiDialog>().Any(), "project settings dialog");
            ConfigUiDialog dialog = desktop.Windows.OfType<ConfigUiDialog>().Single();
            ConfigUiViewModel config = (ConfigUiViewModel)dialog.DataContext!;
            await WaitUntilAsync(() => config.IsSelectedSectionReady, "initial project section");
            SetAssemblyName(config, "ChangedName");
            Check(config.HasUnsavedChanges, "The change was not tracked.");

            Task<bool> applying = config.ApplyChangesAsync();
            WorkspaceFileChangeReviewDialog review = await WaitForReviewAsync(desktop);
            Check(await File.ReadAllTextAsync(projectPath) == original, "The file changed before confirmation.");
            Click(review, "Approve 1 change(s)");
            Check(await applying.WaitAsync(TimeSpan.FromSeconds(30)),
                $"The confirmed change was not reported as applied: {config.OperationMessage}");
            Check(!config.HasUnsavedChanges, "The applied draft is still marked dirty.");
            string expected = original.Replace("OriginalName", "ChangedName", StringComparison.Ordinal);
            Check(await File.ReadAllTextAsync(projectPath) == expected, "The reviewed change was not applied.");
            Check(workbench.WorkspaceFileChangeService.Undo(), "The change could not be undone.");
            Check(await File.ReadAllTextAsync(projectPath) == original, "Undo did not restore the original project.");
            Check(!workbench.WorkspaceFileChangeService.CanUndo, "The change was recorded more than once.");
            Check(workbench.WorkspaceFileChangeService.Redo(), "The change could not be redone.");
            Check(await File.ReadAllTextAsync(projectPath) == expected, "Redo did not restore the reviewed change.");

            SetAssemblyName(config, "Declined");
            Task<bool> declining = config.ApplyChangesAsync();
            Click(await WaitForReviewAsync(desktop), "Cancel");
            Check(!await declining.WaitAsync(TimeSpan.FromSeconds(30)), "Declining review reported success.");
            Check(await File.ReadAllTextAsync(projectPath) == expected, "Declining review changed the file.");
            Check(config.HasUnsavedChanges, "Declining review lost the draft.");
            Check(await config.DiscardChangesAsync(), "Could not discard the declined draft.");

            SetAssemblyName(config, "Cancelled");
            using (CancellationTokenSource cancellation = new())
            {
                Task saving = config.SelectedSection!.SaveAsync(cancellation.Token);
                await WaitForReviewAsync(desktop);
                cancellation.Cancel();
                try
                {
                    await saving.WaitAsync(TimeSpan.FromSeconds(30));
                    throw new InvalidOperationException("Cancellation during review was not propagated.");
                }
                catch (OperationCanceledException)
                {
                    Check(await File.ReadAllTextAsync(projectPath) == expected, "Cancellation changed the project.");
                    Check(!desktop.Windows.OfType<WorkspaceFileChangeReviewDialog>().Any(),
                        "Cancellation left the review dialog open.");
                }
            }
            Check(await config.DiscardChangesAsync(), "Could not discard the cancelled draft.");

            SetAssemblyName(config, "Conflicted");
            Task<bool> conflicting = config.ApplyChangesAsync();
            WorkspaceFileChangeReviewDialog conflictReview = await WaitForReviewAsync(desktop);
            string external = original.Replace("OriginalName", "ExternalName", StringComparison.Ordinal);
            await File.WriteAllTextAsync(projectPath, external);
            Click(conflictReview, "Approve 1 change(s)");
            Check(!await conflicting.WaitAsync(TimeSpan.FromSeconds(30)), "A stale proposal reported success.");
            Check(config.OperationMessage.Contains("changed after", StringComparison.Ordinal),
                "The stale proposal did not report its conflict.");
            Check(await File.ReadAllTextAsync(projectPath) == external, "A stale proposal overwrote the external edit.");
            Check(await config.DiscardChangesAsync(), "Could not reload after the conflict.");

            SetAssemblyName(config, "Detached");
            Task<bool> detached = config.ApplyChangesAsync();
            WorkspaceFileChangeReviewDialog detachedReview = await WaitForReviewAsync(desktop);
            Window mainWindow = desktop.MainWindow!;
            try
            {
                mainWindow.DataContext = null;
                Click(detachedReview, "Approve 1 change(s)");
                Check(!await detached.WaitAsync(TimeSpan.FromSeconds(30)), "A detached context reported success.");
                Check(config.OperationMessage.Contains("workspace changed during review", StringComparison.OrdinalIgnoreCase),
                    "The detached context was not reported.");
                Check(await File.ReadAllTextAsync(projectPath) == external, "The detached context changed the project.");
            }
            finally
            {
                mainWindow.DataContext = workbench;
            }
            Check(await config.DiscardChangesAsync(), "Could not discard the detached-context draft.");

            SetAssemblyName(config, "StaleWorkspace");
            Task<bool> stale = config.ApplyChangesAsync();
            WorkspaceFileChangeReviewDialog staleReview = await WaitForReviewAsync(desktop);
            await workbench.OpenWorkspaceFileAsync(Path.Combine(s_root, "Other", "Other.csproj"));
            Click(staleReview, "Approve 1 change(s)");
            Check(!await stale.WaitAsync(TimeSpan.FromSeconds(30)), "A stale workspace proposal reported success.");
            Check(config.OperationMessage.Contains("workspace changed during review", StringComparison.OrdinalIgnoreCase),
                "The workspace change was not reported.");
            Check(await File.ReadAllTextAsync(projectPath) == external, "The old workspace was changed after switching.");
            Check(await config.DiscardChangesAsync(), "Could not discard the stale workspace draft.");

            SetAssemblyName(config, "StaleBeforeReview");
            Check(!await config.ApplyChangesAsync().WaitAsync(TimeSpan.FromSeconds(30)),
                "A proposal from a previous workspace reported success.");
            Check(config.OperationMessage.Contains("active workspace changed", StringComparison.OrdinalIgnoreCase),
                "The old project context was not reported.");
            Check(!desktop.Windows.OfType<WorkspaceFileChangeReviewDialog>().Any(),
                "A proposal from a previous workspace opened a review.");
            Check(await File.ReadAllTextAsync(projectPath) == external, "The old workspace was changed.");
            Check(await config.DiscardChangesAsync(), "Could not discard the old project context draft.");

            Click(dialog, "Cancel");
            await showing.WaitAsync(TimeSpan.FromSeconds(30));
            await VerifyReviewModesAsync(desktop.MainWindow!);
            Console.WriteLine("Project settings: single apply, undo/redo, decline, cancellation, conflict and context changes verified.");
            Console.WriteLine("Review dialog: approval-only and original apply behavior verified.");
        }
        catch (Exception exception)
        {
            Console.Error.WriteLine(exception);
            exitCode = 1;
        }

        desktop.Shutdown(exitCode);
    }

    private static async Task VerifyReviewModesAsync(Window owner)
    {
        string target = Path.Combine(s_root, "ReviewGenerated.txt");
        WorkspaceFileChangeService service = new(new NoOpenDocuments());
        WorkspaceFileChangeProposal proposal = service.Prepare(s_root,
            [WorkspaceFileChangeRequest.CreateNew("ReviewGenerated.txt", "synthetic")]);

        WorkspaceFileChangeReviewDialog approval = WorkspaceFileChangeReviewDialog.CreateApprovalOnly(proposal);
        Task<bool> approving = approval.ShowDialog(owner);
        Click(approval, "Approve 1 change(s)");
        Check(await approving.WaitAsync(TimeSpan.FromSeconds(30)), "Approval-only review failed.");
        Check(!File.Exists(target) && !service.CanUndo, "Approval-only review applied the proposal.");

        WorkspaceFileChangeReviewDialog application = new(proposal, service);
        Task<bool> applying = application.ShowDialog(owner);
        Click(application, "Apply 1 change(s)");
        Check(await applying.WaitAsync(TimeSpan.FromSeconds(30)), "The existing apply review failed.");
        Check(File.Exists(target) && await File.ReadAllTextAsync(target) == "synthetic",
            "The existing review did not apply the proposal.");
        Check(service.Undo() && !File.Exists(target), "The existing review's change could not be undone.");
    }

    private static void SetAssemblyName(ConfigUiViewModel config, string value) =>
        config.PropertyEditor.Items.Single(item => item.DisplayName == "Assembly name").ScalarValue = value;

    private static async Task<WorkspaceFileChangeReviewDialog> WaitForReviewAsync(
        IClassicDesktopStyleApplicationLifetime desktop)
    {
        await WaitUntilAsync(() => desktop.Windows.OfType<WorkspaceFileChangeReviewDialog>().Any(), "review dialog");
        return desktop.Windows.OfType<WorkspaceFileChangeReviewDialog>().Single();
    }

    private static void Click(Window window, string label)
    {
        Button button = window.GetVisualDescendants().OfType<Button>()
            .Single(candidate => string.Equals(candidate.Content?.ToString(), label, StringComparison.Ordinal));
        button.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
    }

    private static async Task WaitUntilAsync(Func<bool> condition, string description)
    {
        for (int attempt = 0; attempt < 150; attempt++)
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

    private sealed class NoOpenDocuments : IWorkspaceOpenDocumentProvider
    {
        public bool IsDocumentOpen(string fullPath) => false;
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
