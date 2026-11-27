using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using AA98_AxamlDesigner.Host.ViewModels;
using AA98_AvlnCodeStudio.AxamlDesigner.UI.Views;
using AA98_AvlnCodeStudio.Model.Workspaces;
using AA98_AvlnCodeStudio.UI;
using AA98_AvlnCodeStudio.UI.Services;
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
    private static string s_repositoryRoot = null!;

    [STAThread]
    private static int Main(string[] args)
    {
        s_repositoryRoot = Directory.GetCurrentDirectory();
        s_root = Path.Combine(s_repositoryRoot, $"AvlnWorkbenchMvvm-{Guid.NewGuid():N}");
        Directory.CreateDirectory(s_root);
        try
        {
            Environment.SetEnvironmentVariable("AVALONIA_TELEMETRY_OPTOUT", "1");
            Environment.SetEnvironmentVariable("CONFIG_ROOT", Path.Combine(s_root, "config"));
            Environment.SetEnvironmentVariable("AVLN_CONFIG_GROUP_ROOT", null);
            Environment.SetEnvironmentVariable("AVLN_CONFIG_GROUP_ID", null);

            string fixtureDirectory = Path.Combine(s_root, "Fixture");
            Directory.CreateDirectory(fixtureDirectory);
            File.WriteAllText(
                Path.Combine(fixtureDirectory, "Directory.Build.props"),
                """
                <Project>
                  <PropertyGroup>
                    <BaseOutputPath>$(MSBuildThisFileDirectory)..\fixture-artifacts\bin\$(MSBuildProjectName)\</BaseOutputPath>
                    <BaseIntermediateOutputPath>$(MSBuildThisFileDirectory)..\fixture-artifacts\obj\$(MSBuildProjectName)\</BaseIntermediateOutputPath>
                  </PropertyGroup>
                </Project>
                """);
            string projectPath = Path.Combine(fixtureDirectory, "Sample.csproj");
            string axamlPath = Path.Combine(fixtureDirectory, "LoginView.axaml");
            File.WriteAllText(
                projectPath,
                """
                <Project Sdk="Microsoft.NET.Sdk">
                  <PropertyGroup>
                    <TargetFramework>net10.0</TargetFramework>
                    <ManagePackageVersionsCentrally>false</ManagePackageVersionsCentrally>
                  </PropertyGroup>
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
                Path.Combine(fixtureDirectory, "ExistingView.axaml.cs"),
                "using Avalonia.Controls; namespace Sample; public partial class ExistingView : Window { public ExistingView() { DataContext = new ExistingViewModel(); } }");
            File.WriteAllText(
                Path.Combine(fixtureDirectory, "ExistingViewModel.cs"),
                """
                using CommunityToolkit.Mvvm.ComponentModel;
                namespace Sample;
                public partial class ExistingViewModel : ObservableObject
                {
                    // preserve
                }
                """);
            File.WriteAllText(
                Path.Combine(fixtureDirectory, "ExistingView.axaml"),
                """
                <Window xmlns="https://github.com/avaloniaui"
                        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                        xmlns:vm="using:Sample"
                        x:Class="Sample.ExistingView" x:DataType="vm:ExistingViewModel">
                  <StackPanel />
                </Window>
                """);
            File.WriteAllText(
                axamlPath,
                """
                <Window xmlns="https://github.com/avaloniaui"
                        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                        x:Class="Sample.LoginView">
                  <StackPanel>
                    <AutoCompleteBox x:Name="CityAutoCompleteBox" Text="Paris" />
                    <TextBox x:Name="UserNameTextBox" Text="Ada" />
                    <CheckBox x:Name="RememberMeCheckBox" IsChecked="True" CommandParameter="remember-me" />
                    <ToggleButton x:Name="PinnedToggleButton" Content="Pin" IsChecked="True" CommandParameter="pin" />
                    <ColorPicker x:Name="ThemeColorPicker" Color="#FF123456" />
                    <TabControl x:Name="SettingsTabControl" SelectedIndex="1">
                      <TabItem Header="General" />
                      <TabItem Header="Advanced" />
                    </TabControl>
                    <Menu>
                      <MenuItem x:Name="SaveMenuItem" Header="Save" CommandParameter="menu-save" />
                    </Menu>
                    <Button x:Name="SubmitButton" Content="Submit" CommandParameter="document-submit" />
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
            await VerifyStandaloneBindingAsync();
            await BuildFixtureAsync(projectPath, restoreOnly: true);
            MainWindowViewModel workbench = SmokeApplication.Services!.GetRequiredService<MainWindowViewModel>();
            await workbench.OpenWorkspaceFileAsync(projectPath);
            Check(workbench.Workspace?.Projects.Count == 1, "The fixture project was not opened in the Workbench.");
            await workbench.EditorWorkspace.OpenOrActivateAsync(axamlPath);
            await WaitUntilAsync(
                () => workbench.GenerateAxamlMvvmCommand.CanExecute(null),
                "Workbench MVVM generation becoming available");

            string originalMarkup = """
                <Window xmlns="https://github.com/avaloniaui"
                        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                        x:Class="Sample.LoginView">
                  <StackPanel>
                    <AutoCompleteBox x:Name="CityAutoCompleteBox" Text="Paris" />
                    <TextBox x:Name="UserNameTextBox" Text="Ada" />
                    <CheckBox x:Name="RememberMeCheckBox" IsChecked="True" CommandParameter="remember-me" />
                    <ToggleButton x:Name="PinnedToggleButton" Content="Pin" IsChecked="True" CommandParameter="pin" />
                    <ColorPicker x:Name="ThemeColorPicker" Color="#FF123456" />
                    <TabControl x:Name="SettingsTabControl" SelectedIndex="1">
                      <TabItem Header="General" />
                      <TabItem Header="Advanced" />
                    </TabControl>
                    <Menu>
                      <MenuItem x:Name="SaveMenuItem" Header="Save" CommandParameter="menu-save" />
                    </Menu>
                    <Button x:Name="SubmitButton" Content="Submit" CommandParameter="document-submit" />
                  </StackPanel>
                </Window>
                """;
            string generatedViewModelPath = Path.Combine(Path.GetDirectoryName(axamlPath)!, "LoginViewViewModel.cs");
            Task generating = workbench.GenerateAxamlMvvmCommand.ExecuteAsync(null);
            WorkspaceFileChangeReviewDialog review = await WaitForReviewAsync(desktop, generating, workbench);
            Click(review, "Cancel");
            await generating.WaitAsync(TimeSpan.FromMinutes(2));
            Check(await File.ReadAllTextAsync(axamlPath) == originalMarkup && !File.Exists(generatedViewModelPath),
                "Cancelling the MVVM review changed the project files.");
            Check(!workbench.WorkspaceFileChangeService.CanUndo,
                "Cancelling the MVVM review unexpectedly recorded undo history.");
            Check(workbench.Editor.Text == originalMarkup, "Cancelling the MVVM review changed the open AXAML document.");

            generating = workbench.GenerateAxamlMvvmCommand.ExecuteAsync(null);
            review = await WaitForReviewAsync(desktop, generating, workbench);
            Click(review, "Apply 2 change(s)");
            await generating.WaitAsync(TimeSpan.FromMinutes(2));

            string generatedMarkup = await File.ReadAllTextAsync(axamlPath);
            string generatedViewModel = await File.ReadAllTextAsync(generatedViewModelPath);
            Check(generatedMarkup.Contains("Text=\"{Binding UserName, Mode=TwoWay}\"", StringComparison.Ordinal),
                "The approved proposal did not bind the TextBox.");
            Check(generatedMarkup.Contains("Text=\"{Binding City, Mode=TwoWay}\"", StringComparison.Ordinal),
                "The approved proposal did not bind the AutoCompleteBox.");
            Check(generatedMarkup.Contains("Command=\"{Binding SubmitCommand}\"", StringComparison.Ordinal),
                "The approved proposal did not bind the Button command.");
            Check(generatedMarkup.Contains("CommandParameter=\"document-submit\"", StringComparison.Ordinal),
                "The approved proposal did not preserve the Button command parameter.");
            Check(generatedMarkup.Contains("Command=\"{Binding RememberMeCheckBoxCommand}\"", StringComparison.Ordinal),
                "The approved proposal did not bind the CheckBox command.");
            Check(generatedMarkup.Contains("CommandParameter=\"remember-me\"", StringComparison.Ordinal),
                "The approved proposal did not preserve the CheckBox command parameter.");
            Check(generatedMarkup.Contains("IsChecked=\"{Binding Pinned, Mode=TwoWay}\"", StringComparison.Ordinal),
                "The approved proposal did not bind the ToggleButton state.");
            Check(generatedMarkup.Contains("Command=\"{Binding PinnedToggleButtonCommand}\"", StringComparison.Ordinal),
                "The approved proposal did not bind the ToggleButton command.");
            Check(generatedMarkup.Contains("CommandParameter=\"pin\"", StringComparison.Ordinal),
                "The approved proposal did not preserve the ToggleButton command parameter.");
            Check(generatedMarkup.Contains("Command=\"{Binding SaveCommand}\"", StringComparison.Ordinal),
                "The approved proposal did not bind the MenuItem command.");
            Check(generatedMarkup.Contains("CommandParameter=\"menu-save\"", StringComparison.Ordinal),
                "The approved proposal did not preserve the MenuItem command parameter.");
            Check(generatedMarkup.Contains("Color=\"{Binding Theme, Mode=TwoWay}\"", StringComparison.Ordinal),
                "The approved proposal did not bind the ColorPicker.");
            Check(generatedMarkup.Contains("SelectedIndex=\"{Binding Settings, Mode=TwoWay}\"", StringComparison.Ordinal),
                "The approved proposal did not bind the TabControl selection.");
            Check(generatedViewModel.Contains("public partial string UserName { get; set; } = \"Ada\";", StringComparison.Ordinal),
                "The generated ViewModel did not preserve the TextBox's initial value.");
            Check(generatedViewModel.Contains("public partial string? City { get; set; } = \"Paris\";", StringComparison.Ordinal),
                "The generated ViewModel did not preserve the AutoCompleteBox's initial value.");
            Check(generatedViewModel.Contains("public partial bool? Pinned { get; set; } = true;", StringComparison.Ordinal),
                "The generated ViewModel did not preserve the ToggleButton's initial state.");
            Check(generatedViewModel.Contains(
                    "public partial global::Avalonia.Media.Color Theme { get; set; } = global::Avalonia.Media.Color.Parse(\"#FF123456\");",
                    StringComparison.Ordinal),
                "The generated ViewModel did not preserve the ColorPicker's initial color.");
            Check(generatedViewModel.Contains("public partial int Settings { get; set; } = 1;", StringComparison.Ordinal),
                    "The generated ViewModel did not preserve the selected tab index.");
            Check(generatedViewModel.Contains("private void Save(string? commandParameter)", StringComparison.Ordinal),
                "The generated ViewModel did not include the parameterized MenuItem command.");
            Check(generatedViewModel.Contains("private void Submit(string? commandParameter)", StringComparison.Ordinal),
                "The generated ViewModel did not include the parameterized Button command.");
            Check(generatedViewModel.Contains("private void RememberMeCheckBox(string? commandParameter)", StringComparison.Ordinal),
                "The generated ViewModel did not include the parameterized CheckBox command.");
            Check(generatedViewModel.Contains("private void PinnedToggleButton(string? commandParameter)", StringComparison.Ordinal),
                "The generated ViewModel did not include the parameterized ToggleButton command.");
            Check(workbench.Editor.Text == generatedMarkup, "Apply did not synchronize the open AXAML document.");
            Check(!workbench.WorkspaceFileChangeService.CanRedo, "The applied proposal unexpectedly has redo history.");

            Check(workbench.WorkspaceFileChangeService.Undo(), "The generated files could not be undone.");
            Check(await File.ReadAllTextAsync(axamlPath) == originalMarkup, "Undo did not restore the original AXAML.");
            Check(!File.Exists(generatedViewModelPath), "Undo did not remove the newly generated ViewModel.");
            Check(workbench.Editor.Text == originalMarkup, "Undo did not synchronize the open AXAML document.");
            Check(workbench.WorkspaceFileChangeService.Redo(), "The generated files could not be redone.");
            Check(await File.ReadAllTextAsync(axamlPath) == generatedMarkup, "Redo did not restore the approved AXAML.");
            Check(await File.ReadAllTextAsync(generatedViewModelPath) == generatedViewModel,
                "Redo did not restore the generated ViewModel.");
            Check(workbench.Editor.Text == generatedMarkup, "Redo did not synchronize the open AXAML document.");

            await BuildFixtureAsync(projectPath);
            CSharpSyntaxDiagnosticService semanticService =
                (SmokeApplication.Services ?? throw new InvalidOperationException("Smoke services are unavailable."))
                .GetRequiredService<CSharpSyntaxDiagnosticService>();
            var safeContext = await semanticService.AnalyzeAxamlMvvmAsync(
                generatedMarkup, axamlPath, workbench.Workspace!, allowExistingRuntimeDataContext: true);
            Check(safeContext.IsSuccessful && safeContext.HasRuntimeDataContext,
                "The generated root DataContext was not resolved for visual review.");
            var freshGeneration = await semanticService.AnalyzeAxamlMvvmAsync(
                generatedMarkup, axamlPath, workbench.Workspace!);
            Check(!freshGeneration.IsSuccessful, "Fresh generation accepted an existing root DataContext.");
            XDocument mismatchedContext = XDocument.Parse(generatedMarkup);
            XElement root = mismatchedContext.Root!;
            XElement value = root.Element(root.Name.Namespace + "Window.DataContext")!.Elements().Single();
            value.Name = value.Name.Namespace + "OtherViewModel";
            var unsafeContext = await semanticService.AnalyzeAxamlMvvmAsync(
                mismatchedContext.ToString(), axamlPath, workbench.Workspace!, allowExistingRuntimeDataContext: true);
            Check(!unsafeContext.IsSuccessful, "Semantic analysis accepted a mismatched root DataContext.");

            await VerifyUnsavedViewModelContextAsync(workbench, semanticService, axamlPath, generatedViewModelPath);

            string visualMarkup = generatedMarkup
                .Replace("{Binding UserName,", "{CompiledBinding UserName,", StringComparison.Ordinal)
                .Replace("{Binding SubmitCommand}", "{ReflectionBinding SubmitCommand}", StringComparison.Ordinal);
            Check(visualMarkup != generatedMarkup, "Could not prepare visual binding kinds.");
            workbench.Editor.Text = visualMarkup;
            await workbench.Editor.SaveCommand.ExecuteAsync(null);
            await WaitUntilAsync(
                () => workbench.AxamlDesigner.PreviewRoot?.Children.Any(node =>
                    node.StableName == "UserNameTextBox"
                    && node.Properties.TryGetValue("Text", out string? text)
                    && text.Contains("{CompiledBinding", StringComparison.Ordinal)) == true,
                "generated view visual bindings");
            await VerifyVisualBindingAsync(workbench, desktop, axamlPath, generatedViewModelPath,
                "UserNameTextBox", "Text", "CompiledBinding", "VisualName");
            await VerifyVisualBindingAsync(workbench, desktop, axamlPath, generatedViewModelPath,
                "SubmitButton", "Command", "ReflectionBinding", "VisualCommand");
            generatedMarkup = await File.ReadAllTextAsync(axamlPath);
            generatedViewModel = await File.ReadAllTextAsync(generatedViewModelPath);
            await BuildFixtureAsync(projectPath);

            string incrementalAxamlPath = Path.Combine(Path.GetDirectoryName(axamlPath)!, "ExistingView.axaml");
            string incrementalViewModelPath = Path.Combine(Path.GetDirectoryName(axamlPath)!, "ExistingViewModel.cs");
            string originalIncrementalViewModel = await File.ReadAllTextAsync(incrementalViewModelPath);
            await workbench.EditorWorkspace.OpenOrActivateAsync(incrementalAxamlPath);
            string incrementalMarkup = workbench.Editor.Text.Replace(
                "<StackPanel />",
                "<StackPanel><TextBox Text=\"{CompiledBinding ExtraName}\" />"
                + "<Button Command=\"{ReflectionBinding ExtraCommand}\" /></StackPanel>",
                StringComparison.Ordinal);
            Check(incrementalMarkup != workbench.Editor.Text, "Could not extend the controlled AXAML fixture.");
            workbench.Editor.Text = incrementalMarkup;
            await workbench.Editor.SaveCommand.ExecuteAsync(null);
            string sentinelPath = Path.Combine(Path.GetDirectoryName(axamlPath)!, "Unrelated.txt");
            await File.WriteAllTextAsync(sentinelPath, "unchanged");
            string originalCodeBehind = await File.ReadAllTextAsync(Path.ChangeExtension(incrementalAxamlPath, ".axaml.cs"));
            await WaitUntilAsync(() => workbench.GenerateAxamlMvvmCommand.CanExecute(null),
                "incremental MVVM generation becoming available");
            generating = workbench.GenerateAxamlMvvmCommand.ExecuteAsync(null);
            review = await WaitForReviewAsync(desktop, generating, workbench);
            Click(review, "Cancel");
            await generating.WaitAsync(TimeSpan.FromMinutes(2));
            Check(await File.ReadAllTextAsync(incrementalViewModelPath) == originalIncrementalViewModel,
                "Cancelling incremental generation changed the ViewModel.");

            generating = workbench.GenerateAxamlMvvmCommand.ExecuteAsync(null);
            review = await WaitForReviewAsync(desktop, generating, workbench);
            Click(review, "Apply 1 change(s)");
            await generating.WaitAsync(TimeSpan.FromMinutes(2));
            string incrementalViewModel = await File.ReadAllTextAsync(incrementalViewModelPath);
            Check(incrementalViewModel.Contains("ExtraName", StringComparison.Ordinal)
                && incrementalViewModel.Contains("private void Extra()", StringComparison.Ordinal)
                && incrementalViewModel.Contains("// preserve", StringComparison.Ordinal),
                "Incremental generation did not add the property and command while preserving existing source.");
            Check(await File.ReadAllTextAsync(incrementalAxamlPath) == incrementalMarkup
                && workbench.Editor.Text == incrementalMarkup
                && await File.ReadAllTextAsync(sentinelPath) == "unchanged"
                && await File.ReadAllTextAsync(Path.ChangeExtension(incrementalAxamlPath, ".axaml.cs")) == originalCodeBehind
                && await File.ReadAllTextAsync(axamlPath) == generatedMarkup
                && await File.ReadAllTextAsync(generatedViewModelPath) == generatedViewModel,
                "Incremental generation changed AXAML or unrelated files.");
            Check(workbench.WorkspaceFileChangeService.Undo()
                && await File.ReadAllTextAsync(incrementalViewModelPath) == originalIncrementalViewModel,
                "Incremental generation could not be undone.");
            Check(workbench.WorkspaceFileChangeService.Redo()
                && await File.ReadAllTextAsync(incrementalViewModelPath) == incrementalViewModel,
                "Incremental generation could not be redone.");
            await BuildFixtureAsync(projectPath);
            Console.WriteLine("Workbench MVVM: generation and visual binding review, source preservation, undo/redo and generated-project build verified.");
        }
        catch (Exception exception)
        {
            Console.Error.WriteLine(exception);
            exitCode = 1;
        }

        desktop.Shutdown(exitCode);
    }

    private static async Task VerifyUnsavedViewModelContextAsync(
        MainWindowViewModel workbench,
        CSharpSyntaxDiagnosticService semanticService,
        string axamlPath,
        string viewModelPath)
    {
        byte[] markupBytes = await File.ReadAllBytesAsync(axamlPath);
        byte[] viewModelBytes = await File.ReadAllBytesAsync(viewModelPath);
        string savedMarkup = await File.ReadAllTextAsync(axamlPath);
        string savedViewModel = await File.ReadAllTextAsync(viewModelPath);
        await workbench.EditorWorkspace.OpenOrActivateAsync(viewModelPath);
        EditorTabViewModel viewModelTab = workbench.EditorWorkspace.ActiveTab;
        int closingBrace = savedViewModel.LastIndexOf('}');
        Check(closingBrace >= 0, "The fixture ViewModel has no class body.");
        viewModelTab.Editor.Text = savedViewModel.Insert(
            closingBrace, "\n    public string OverlayCaption { get; set; } = \"\";\n");
        Check(viewModelTab.Editor.Text != savedViewModel, "The ViewModel overlay was not edited.");

        TaskCompletionSource started = new(TaskCreationOptions.RunContinuationsAsynchronously);
        TaskCompletionSource release = new(TaskCreationOptions.RunContinuationsAsynchronously);
        PropertyInfo gate = typeof(CSharpSyntaxDiagnosticService).GetProperty(
            "BeforeAxamlCompilationAsync", BindingFlags.Instance | BindingFlags.NonPublic)
            ?? throw new InvalidOperationException("The existing AXAML compilation test hook is unavailable.");
        gate.SetValue(semanticService, (Func<CancellationToken, Task>)(async token =>
        {
            started.TrySetResult();
            await release.Task.WaitAsync(token);
        }));
        try
        {
            await workbench.EditorWorkspace.OpenOrActivateAsync(axamlPath);
            string draftMarkup = savedMarkup.Replace(
                "{Binding UserName,", "{Binding OverlayCaption,", StringComparison.Ordinal);
            Check(draftMarkup != savedMarkup, "The fixture binding was not found.");
            workbench.Editor.Text = draftMarkup;
            var designer = workbench.AxamlDesigner;
            designer.SelectedNode = designer.PreviewRoot!.Children.Single(node => node.StableName == "UserNameTextBox");
            var row = designer.BindingRows.Single(binding => binding.PropertyName == "Text");
            await started.Task.WaitAsync(TimeSpan.FromMinutes(2));
            row.BindingMode = AxamlBindingMode.OneWay;
            row.UpdateSourceTrigger = AxamlBindingUpdateSourceTrigger.LostFocus;
            TaskCompletionSource refreshed = new(TaskCreationOptions.RunContinuationsAsynchronously);
            NotifyCollectionChangedEventHandler onRefresh = (_, _) => refreshed.TrySetResult();
            designer.BindingRows.CollectionChanged += onRefresh;
            try
            {
                release.TrySetResult();
                await refreshed.Task.WaitAsync(TimeSpan.FromMinutes(2));
            }
            finally
            {
                designer.BindingRows.CollectionChanged -= onRefresh;
            }
            Check(row.IsSelectingExistingMember && row.SelectedMember == "OverlayCaption"
                && row.BindingMode == AxamlBindingMode.OneWay
                && row.UpdateSourceTrigger == AxamlBindingUpdateSourceTrigger.LostFocus,
                $"Unsaved ViewModel overlay was not recognized: selected={row.SelectedMember}, new={row.NewMemberName}, mode={row.BindingMode}, trigger={row.UpdateSourceTrigger}.");

            string originalOverlay = viewModelTab.Editor.Text;
            TaskCompletionSource staleStarted = new(TaskCreationOptions.RunContinuationsAsynchronously);
            TaskCompletionSource releaseStale = new(TaskCreationOptions.RunContinuationsAsynchronously);
            int calls = 0;
            gate.SetValue(semanticService, (Func<CancellationToken, Task>)(async _ =>
            {
                if (Interlocked.Increment(ref calls) == 1)
                {
                    staleStarted.TrySetResult();
                    await releaseStale.Task;
                }
            }));
            try
            {
                viewModelTab.Editor.Text = originalOverlay + "\n";
                await staleStarted.Task.WaitAsync(TimeSpan.FromMinutes(2));
                viewModelTab.Editor.Text = originalOverlay.Replace("OverlayCaption", "ReplacementCaption", StringComparison.Ordinal);
                releaseStale.TrySetResult();
                await WaitUntilAsync(() => row.AvailableMembers.Contains("ReplacementCaption"), "newer ViewModel overlay");
                Check(!row.AvailableMembers.Contains("OverlayCaption")
                    && row.BindingMode == AxamlBindingMode.OneWay
                    && row.UpdateSourceTrigger == AxamlBindingUpdateSourceTrigger.LostFocus,
                    "The stale ViewModel analysis replaced the newer overlay or binding draft.");
            }
            finally
            {
                releaseStale.TrySetResult();
                gate.SetValue(semanticService, null);
            }

            viewModelTab.Editor.Text = originalOverlay;
            await WaitUntilAsync(() => row.IsSelectingExistingMember && row.SelectedMember == "OverlayCaption",
                "restored ViewModel overlay");
            designer.ApplyBindingRow(row);
            Check(workbench.Editor.Text.Contains("UpdateSourceTrigger=LostFocus", StringComparison.Ordinal),
                "The pending binding options were not applied.");
            designer.UndoCommand.Execute(null);
            Check(workbench.Editor.Text == draftMarkup, "Designer undo did not restore the unsaved AXAML.");
            designer.RedoCommand.Execute(null);
            Check(workbench.Editor.Text.Contains("UpdateSourceTrigger=LostFocus", StringComparison.Ordinal),
                "Designer redo did not restore the binding edit.");
            byte[] currentMarkupBytes = await File.ReadAllBytesAsync(axamlPath);
            byte[] currentViewModelBytes = await File.ReadAllBytesAsync(viewModelPath);
            Check(viewModelTab.Editor.Text == originalOverlay
                && markupBytes.SequenceEqual(currentMarkupBytes)
                && viewModelBytes.SequenceEqual(currentViewModelBytes),
                "The overlay binding test changed the ViewModel buffer or a source file on disk.");
        }
        finally
        {
            release.TrySetResult();
            gate.SetValue(semanticService, null);
            viewModelTab.Editor.Text = savedViewModel;
            if (workbench.Editor.CurrentFilePath == axamlPath)
            {
                workbench.Editor.Text = savedMarkup;
            }
            workbench.EditorWorkspace.TryClose(viewModelTab);
        }
    }

    private static async Task VerifyStandaloneBindingAsync()
    {
        AxamlDesignerHostViewModel host = new();
        AA98_AxamlDesigner.Host.Views.MainWindow window = new() { DataContext = host };
        try
        {
            window.Show();
            host.Markup = "<TextBox xmlns='https://github.com/avaloniaui' Text='{Binding Caption}' />";
            host.Designer.SelectedNode = host.Designer.PreviewRoot;
            await Dispatcher.UIThread.InvokeAsync(() => { }, DispatcherPriority.Background);
            Check(window.GetVisualDescendants().OfType<AxamlDesignerView>()
                .Any(view => ReferenceEquals(view.DataContext, host.Designer)),
                "The standalone window did not attach the shared designer.");
            var row = host.Designer.BindingRows.Single();
            row.BindingMode = AxamlBindingMode.TwoWay;
            host.Designer.SetBindingContext(
            [
                new AxamlMvvmSemanticMember("Caption", "string", IsCommand: false, IsWritable: true),
            ]);
            Check(row.IsSelectingExistingMember && row.SelectedMember == "Caption"
                && row.BindingMode == AxamlBindingMode.TwoWay,
                "The standalone binding row lost its pending mode on context refresh.");
            host.Designer.ApplyBindingRow(row);
            Check(host.Markup.Contains("Mode=TwoWay", StringComparison.Ordinal), "Standalone binding apply failed.");
            host.Designer.UndoCommand.Execute(null);
            Check(!host.Markup.Contains("Mode=TwoWay", StringComparison.Ordinal), "Standalone binding undo failed.");
            host.Designer.RedoCommand.Execute(null);
            Check(host.Markup.Contains("Mode=TwoWay", StringComparison.Ordinal), "Standalone binding redo failed.");
        }
        finally
        {
            window.DataContext = null;
            window.Close();
        }
    }

    private static async Task VerifyVisualBindingAsync(
        MainWindowViewModel workbench,
        IClassicDesktopStyleApplicationLifetime desktop,
        string axamlPath,
        string viewModelPath,
        string controlName,
        string propertyName,
        string kind,
        string replacement)
    {
        string beforeMarkup = await File.ReadAllTextAsync(axamlPath);
        string beforeViewModel = await File.ReadAllTextAsync(viewModelPath);
        var designer = workbench.AxamlDesigner;
        designer.SelectedNode = designer.PreviewRoot!.Children.Single(node => node.StableName == controlName);
        designer.SetBindingContext([]);
        var row = designer.BindingRows.Single(binding => binding.PropertyName == propertyName);
        Check(row.CanEditBinding, $"The generated root DataContext blocked {kind}: {row.BindingExpressionDiagnostic}");
        if (propertyName == "Text")
        {
            row.BindingMode = AxamlBindingMode.OneWay;
            designer.SetBindingContext(
            [
                new AxamlMvvmSemanticMember("UserName", "string", IsCommand: false, IsWritable: true),
            ]);
            Check(row.IsSelectingExistingMember && row.SelectedMember == "UserName"
                && row.BindingMode == AxamlBindingMode.OneWay,
                $"The visual binding row did not retain its edit when the ViewModel context arrived: source={row.MemberSource}, selected={row.SelectedMember}, new={row.NewMemberName}, mode={row.BindingMode}, bound={row.MemberName}.");
            row.BindingMode = AxamlBindingMode.TwoWay;
            designer.SetBindingContext([]);
        }
        row.NewMemberName = replacement;
        Task applying = designer.ApplyBindingRowAsync(row);
        WorkspaceFileChangeReviewDialog review = await WaitForReviewAsync(desktop, applying, workbench);
        Click(review, "Apply 2 change(s)");
        await applying.WaitAsync(TimeSpan.FromMinutes(2));

        string afterMarkup = await File.ReadAllTextAsync(axamlPath);
        string afterViewModel = await File.ReadAllTextAsync(viewModelPath);
        Check(afterMarkup.Contains("{" + kind + " " + replacement, StringComparison.Ordinal)
            && afterViewModel.Contains(replacement == "VisualCommand" ? "private void Visual()" : replacement, StringComparison.Ordinal)
            && workbench.Editor.Text == afterMarkup,
            $"The {kind} visual review did not apply both source edits: {row.Diagnostic}");
        Check(workbench.WorkspaceFileChangeService.Undo()
            && await File.ReadAllTextAsync(axamlPath) == beforeMarkup
            && await File.ReadAllTextAsync(viewModelPath) == beforeViewModel
            && workbench.Editor.Text == beforeMarkup,
            $"The {kind} visual review could not be undone.");
        Check(workbench.WorkspaceFileChangeService.Redo()
            && await File.ReadAllTextAsync(axamlPath) == afterMarkup
            && await File.ReadAllTextAsync(viewModelPath) == afterViewModel
            && workbench.Editor.Text == afterMarkup,
            $"The {kind} visual review could not be redone.");
    }

    private static async Task BuildFixtureAsync(string projectPath, bool restoreOnly = false)
    {
        string dotnetHost = ResolveDotnetHost();
        bool isWindows = OperatingSystem.IsWindows();
        ProcessStartInfo startInfo = new(isWindows ? "powershell.exe" : "bash")
        {
            WorkingDirectory = s_repositoryRoot,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
        };
        if (isWindows)
        {
            startInfo.ArgumentList.Add("-NoProfile");
            startInfo.ArgumentList.Add("-Command");
        }

        string[] arguments =
        [
            Path.Combine(s_repositoryRoot, "tools", "build", isWindows ? "dotnet-isolated.ps1" : "dotnet-isolated.sh"),
            restoreOnly ? "restore" : "build",
            projectPath,
            $"-p:HostArtifactRoot={Path.Combine(s_root, "fixture-artifacts")}",
            "--verbosity",
            "quiet",
        ];
        if (isWindows)
        {
            string quotedArguments = string.Join(" ", arguments.Select(
                argument => "'" + argument.Replace("'", "''", StringComparison.Ordinal) + "'"));
            startInfo.ArgumentList.Add($"& {quotedArguments}; exit $LASTEXITCODE");
        }
        else
        {
            foreach (string argument in arguments)
            {
                startInfo.ArgumentList.Add(argument);
            }
        }
        if (Path.IsPathFullyQualified(dotnetHost))
        {
            startInfo.Environment["PATH"] = Path.GetDirectoryName(dotnetHost) + Path.PathSeparator
                + startInfo.Environment["PATH"];
        }
        startInfo.Environment.Remove("MSBuildSDKsPath");
        startInfo.Environment.Remove("MSBuildExtensionsPath");
        startInfo.Environment.Remove("MSBuildExtensionsPath32");
        startInfo.Environment.Remove("MSBuildToolsPath");
        startInfo.Environment.Remove("DOTNET_MSBUILD_SDK_RESOLVER_SDKS_DIR");
        startInfo.Environment.Remove("DOTNET_MSBUILD_SDK_RESOLVER_CLI_DIR");
        startInfo.Environment["MSBUILDDISABLENODEREUSE"] = "1";
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
