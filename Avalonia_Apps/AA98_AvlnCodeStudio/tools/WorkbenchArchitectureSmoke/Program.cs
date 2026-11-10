using AA98_AvlnCodeStudio.AxamlDesigner.UI.Controls;
using AA98_AvlnCodeStudio.AxamlDesigner.UI.ViewModels;
using AA98_AvlnCodeStudio.AxamlDesigner.UI.Views;
using AA98_AvlnCodeStudio.Host.Mcp;
using AA98_AvlnCodeStudio.Model.Documents;
using AA98_AvlnCodeStudio.Model.Workspaces;
using AA98_AvlnCodeStudio.UI;
using AA98_AvlnCodeStudio.UI.Controls;
using AA98_AvlnCodeStudio.UI.ViewModels;
using AA98_AxamlDesigner.Host.Commands;
using AA98_AxamlDesigner.Host.ViewModels;
using AA98_AxamlDesigner.Host.Views;
using Avalonia;
using Avalonia.Automation.Peers;
using Avalonia.Automation.Provider;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Themes.Fluent;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Code.Navigation;
using Microsoft.Extensions.DependencyInjection;
using Project.Explorer.Avalonia.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

internal static class Program
{
    private static string s_root = null!;

    [STAThread]
    private static int Main(string[] args)
    {
        s_root = Directory.CreateTempSubdirectory("AvlnWorkbenchArchitecture-").FullName;
        try
        {
            Environment.SetEnvironmentVariable("CONFIG_ROOT", Path.Combine(s_root, "config"));
            Environment.SetEnvironmentVariable("AVLN_CONFIG_GROUP_ROOT", null);
            Environment.SetEnvironmentVariable("AVLN_CONFIG_GROUP_ID", null);
            string fixture = Path.Combine(s_root, "fixture");
            Directory.CreateDirectory(fixture);
            File.WriteAllText(Path.Combine(fixture, "Sample.csproj"),
                "<Project Sdk=\"Microsoft.NET.Sdk\"><PropertyGroup><TargetFramework>net10.0</TargetFramework></PropertyGroup></Project>");
            File.WriteAllText(Path.Combine(fixture, "First.axaml"),
                "<StackPanel><TextBlock Text=\"First\" /></StackPanel>");
            File.WriteAllText(Path.Combine(fixture, "Second.axaml"),
                "<StackPanel xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\"><TextBox x:Name=\"Input\" Width=\"100\" Text=\"Before\" /><TextBlock x:Name=\"Label\" Text=\"Label\" /></StackPanel>");
            File.WriteAllText(Path.Combine(fixture, "Host.axaml"),
                "<StackPanel><TextBox Width=\"100\" Text=\"Host\" /><Button Content=\"Other\" /></StackPanel>");
            File.WriteAllText(Path.Combine(fixture, "Grid.axaml"),
                "<av:Grid xmlns:av=\"https://github.com/avaloniaui\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\" "
                + "RowDefinitions=\"*,*,*\" ColumnDefinitions=\"*,*,*,*\">"
                + "<av:Button x:Name=\"Span\" av:Grid.RowSpan=\"2\" av:Grid.ColumnSpan=\"2\" />"
                + "<av:TextBlock x:Name=\"Blocker\" av:Grid.Column=\"3\" Width=\"80\" /></av:Grid>");
            File.WriteAllText(Path.Combine(fixture, "Content.axaml"),
                "<Border xmlns=\"https://github.com/avaloniaui\"><Button /></Border>");
            File.WriteAllText(Path.Combine(fixture, "ControlPreviews.axaml"),
                "<StackPanel xmlns=\"https://github.com/avaloniaui\">"
                + "<ComboBox PlaceholderText=\"Choose an option\" HorizontalContentAlignment=\"Right\" VerticalContentAlignment=\"Bottom\" Background=\"White\" BorderBrush=\"Blue\" BorderThickness=\"2\" CornerRadius=\"4\" FontSize=\"14\" FontStyle=\"Italic\" FontWeight=\"Medium\" Foreground=\"Black\" Padding=\"3\" />"
                + "<NumericUpDown Value=\"12.5\" FormatString=\"N2\" Increment=\"0.25\" Minimum=\"0.5\" Maximum=\"40\" AllowSpin=\"False\" ShowButtonSpinner=\"False\" ClipValueToMinMax=\"True\" ButtonSpinnerLocation=\"Left\" TextAlignment=\"Center\" HorizontalContentAlignment=\"Center\" VerticalContentAlignment=\"Top\" ParsingNumberStyle=\"Integer\" Margin=\"3,4\" HorizontalAlignment=\"Right\" VerticalAlignment=\"Center\" Width=\"120\" Height=\"60\" MinWidth=\"40\" MaxWidth=\"160\" MinHeight=\"30\" MaxHeight=\"90\" Background=\"#FFF0F0F0\" BorderBrush=\"Green\" BorderThickness=\"2\" CornerRadius=\"3\" FontSize=\"15\" FontStyle=\"Italic\" FontWeight=\"Bold\" Foreground=\"DarkGreen\" Padding=\"2\" />"
                + "<NumericUpDown Value=\"0\" />"
                + "<NumericUpDown Value=\"1\" FormatString=\"Q\" ParsingNumberStyle=\"AllowHexSpecifier\" HorizontalAlignment=\"999\" VerticalAlignment=\"999\" HorizontalContentAlignment=\"999\" VerticalContentAlignment=\"999\" />"
                + "<DockPanel HorizontalSpacing=\"4\" VerticalSpacing=\"6\" LastChildFill=\"False\"><TextBlock Text=\"Docked preview\" /><Button Content=\"Docked button\" DockPanel.Dock=\"Bottom\" /></DockPanel>"
                + "<DockPanel HorizontalSpacing=\"-1\" VerticalSpacing=\"NaN\" LastChildFill=\"False\" />"
                + "<UniformGrid Rows=\"2\" Columns=\"3\" RowSpacing=\"3\" ColumnSpacing=\"5\"><Button Content=\"Uniform one\" /><Button Content=\"Uniform two\" /></UniformGrid>"
                + "<UniformGrid Columns=\"4\" RowSpacing=\"NaN\" ColumnSpacing=\"-1\" />"
                + "<Grid RowSpacing=\"2\" ColumnSpacing=\"4\" ShowGridLines=\"True\" Width=\"99\" />"
                + "<Grid RowSpacing=\"NaN\" ColumnSpacing=\"-1\" ShowGridLines=\"sometimes\" Width=\"98\" />"
                + "<ScrollViewer HorizontalScrollBarVisibility=\"Hidden\" VerticalScrollBarVisibility=\"Disabled\" Width=\"97\"><TextBlock Text=\"Scroll content\" /></ScrollViewer>"
                + "<ScrollViewer HorizontalScrollBarVisibility=\"Always\" VerticalScrollBarVisibility=\"None\" Width=\"96\" />"
                + "<Viewbox StretchDirection=\"UpOnly\" Width=\"93\"><TextBlock Text=\"Scale direction\" /></Viewbox>"
                + "<Viewbox StretchDirection=\"Sideways\" Width=\"92\"><TextBlock Text=\"Fallback direction\" /></Viewbox>"
                + "<WrapPanel ItemSpacing=\"5\" LineSpacing=\"7\" ItemWidth=\"40\" ItemHeight=\"20\" ItemsAlignment=\"Center\"><Button Content=\"First item\" /><Button Content=\"Second item\" /></WrapPanel>"
                + "<WrapPanel />"
                + "<WrapPanel Orientation=\"Vertical\" ItemSpacing=\"-1\" LineSpacing=\"NaN\" ItemWidth=\"-1\" ItemHeight=\"Infinity\" ItemsAlignment=\"999\" />"
                + "<ProgressBar Minimum=\"10\" Maximum=\"90\" Value=\"42.5\" IsIndeterminate=\"False\" ShowProgressText=\"True\" ProgressTextFormat=\"{}{1:0.0}%\" Background=\"White\" BorderBrush=\"Purple\" BorderThickness=\"1\" CornerRadius=\"2\" FontSize=\"13\" FontStyle=\"Normal\" FontWeight=\"Normal\" Foreground=\"Blue\" Padding=\"1\" />"
                + "<ProgressBar Value=\"35\" IsIndeterminate=\"True\" />"
                + "<ProgressBar Value=\"25\" ProgressTextFormat=\"{}{2:0}%\" />"
                + "<Slider Minimum=\"-10\" Maximum=\"60\" Value=\"35.5\" TickFrequency=\"5\" IsSnapToTickEnabled=\"True\" IsDirectionReversed=\"True\" TickPlacement=\"Outside\" Orientation=\"Vertical\" Background=\"#FFF0F0F0\" BorderBrush=\"Black\" BorderThickness=\"3\" CornerRadius=\"5\" FontSize=\"16\" FontStyle=\"Oblique\" FontWeight=\"SemiBold\" Foreground=\"Red\" Padding=\"4\" />"
                + "<CheckBox IsChecked=\"True\" IsThreeState=\"True\" HorizontalContentAlignment=\"Left\" VerticalContentAlignment=\"Center\" Background=\"#FFF0F0F0\" BorderBrush=\"Blue\" BorderThickness=\"2\" CornerRadius=\"1,2,3,4\" FontSize=\"16\" FontStyle=\"Italic\" FontWeight=\"Medium\" Foreground=\"White\" Padding=\"4\" />"
                + "<RadioButton IsChecked=\"False\" GroupName=\"Choices\" HorizontalContentAlignment=\"Center\" VerticalContentAlignment=\"Top\" Background=\"White\" BorderBrush=\"Purple\" BorderThickness=\"1\" CornerRadius=\"4\" FontSize=\"17\" FontStyle=\"Oblique\" FontWeight=\"Bold\" Foreground=\"Black\" Padding=\"3\" />"
                + "<Button Content=\"Keyboard action preview\" IsDefault=\"True\" IsCancel=\"True\" HorizontalContentAlignment=\"Left\" VerticalContentAlignment=\"Top\" ClipToBounds=\"True\" Opacity=\"0.8\" MinWidth=\"100\" MaxWidth=\"200\" MinHeight=\"30\" MaxHeight=\"60\" Background=\"#FFF0F0F0\" BorderBrush=\"Blue\" BorderThickness=\"2\" CornerRadius=\"6,4,2,8\" FontSize=\"18\" FontStyle=\"Italic\" FontWeight=\"Medium\" Foreground=\"White\" Padding=\"8,4\" />"
                + "<Button Content=\"Clip fallback\" ClipToBounds=\"maybe\" />"
                + "<Button Content=\"Hidden preview\" IsVisible=\"False\" Width=\"90\" />"
                + "<Button Content=\"Visibility fallback\" IsVisible=\"maybe\" Width=\"89\" />"
                + "<Border BorderBrush=\"Purple\" CornerRadius=\"12,4,8,2\" BorderThickness=\"2\" Background=\"#FFF0F0F0\" Padding=\"6\"><TextBlock Text=\"Border appearance\" /></Border>"
                + "<TextBlock Text=\"Alignment preview\" TextAlignment=\"Center\" FontStyle=\"Italic\" FontWeight=\"SemiBold\" LineHeight=\"24\" MaxLines=\"2\" Opacity=\"0.5\" TextTrimming=\"CharacterEllipsis\" TextWrapping=\"NoWrap\" Foreground=\"Red\" />"
                + "<TextBox Text=\"Input preview\" PlaceholderText=\"Enter a value\" PlaceholderForeground=\"DarkGray\" IsReadOnly=\"False\" MaxLength=\"120\" MinLines=\"2\" MaxLines=\"5\" LineHeight=\"22\" UseFloatingPlaceholder=\"True\" AcceptsReturn=\"True\" AcceptsTab=\"True\" HorizontalContentAlignment=\"Right\" VerticalContentAlignment=\"Bottom\" Background=\"#FFF0F0F0\" BorderBrush=\"Green\" BorderThickness=\"2\" CornerRadius=\"3,6,9,12\" FontSize=\"18\" FontStyle=\"Italic\" FontWeight=\"Medium\" HorizontalAlignment=\"Center\" Opacity=\"0.75\" Padding=\"8,4\" TextAlignment=\"Right\" TextWrapping=\"WrapWithOverflow\" VerticalAlignment=\"Bottom\" Foreground=\"Blue\" />"
                + "<TextBox Text=\"Default line count preview\" />"
                + "<TextBox Text=\"Invalid line count preview\" MinLines=\"-1\" MaxLines=\"many\" LineHeight=\"NaN\" UseFloatingPlaceholder=\"sometimes\" PlaceholderForeground=\"Gradient\" />"
                + "<ToggleSwitch Content=\"Notifications\" IsChecked=\"True\" IsThreeState=\"True\" OnContent=\"Enabled\" OffContent=\"Disabled\" HorizontalContentAlignment=\"Right\" VerticalContentAlignment=\"Center\" Background=\"PaleGreen\" BorderBrush=\"DarkGreen\" BorderThickness=\"3\" CornerRadius=\"5\" FontSize=\"15\" FontStyle=\"Italic\" FontWeight=\"SemiBold\" Foreground=\"DarkGreen\" Padding=\"2\" />"
                + "<Border FlowDirection=\"RightToLeft\"><TextBlock Text=\"Inherited RTL direction\" UseLayoutRounding=\"True\" /></Border>"
                + "<TextBlock Text=\"Invalid flow fallback\" FlowDirection=\"Sideways\" UseLayoutRounding=\"sometimes\" />"
                + "<TextBlock Text=\"Bound flow fallback\" FlowDirection=\"{Binding Direction}\" UseLayoutRounding=\"{Binding LayoutRounding}\" />"
                + "<ContentControl HorizontalContentAlignment=\"Stretch\" VerticalContentAlignment=\"Bottom\"><TextBlock Text=\"ContentControl alignment\" /></ContentControl>"
                + "<Expander Header=\"Expander alignment\" IsExpanded=\"True\" HorizontalContentAlignment=\"Right\" VerticalContentAlignment=\"Top\"><TextBlock Text=\"Expander content\" /></Expander>"
                + "<GroupBox Header=\"GroupBox alignment\" HorizontalContentAlignment=\"Left\" VerticalContentAlignment=\"Bottom\"><TextBlock Text=\"GroupBox content\" /></GroupBox>"
                + "<Label Content=\"Label alignment\" FontSize=\"18\" FontStyle=\"Italic\" FontWeight=\"SemiBold\" Foreground=\"Blue\" TextAlignment=\"Right\" TextTrimming=\"WordEllipsis\" TextWrapping=\"NoWrap\" HorizontalContentAlignment=\"Center\" VerticalContentAlignment=\"Top\" />"
                + "</StackPanel>");
            File.WriteAllText(Path.Combine(fixture, "Sample.cs"), "namespace Sample; public class Example { }");
            File.WriteAllBytes(Path.Combine(fixture, "Image.png"), [0x89, 0x50, 0x4e, 0x47, 0x00]);
            File.WriteAllBytes(Path.Combine(fixture, "Binary.cs"), [0x41, 0x00, 0x42]);
            File.WriteAllBytes(Path.Combine(fixture, "Fixture.bin"), [0x00, 0xff, 0x42]);

            return AppBuilder.Configure<SmokeApplication>()
                .UsePlatformDetect()
                .AfterSetup(_ => Dispatcher.UIThread.Post(async () => await RunAsync(fixture)))
                .StartWithClassicDesktopLifetime(args);
        }
        finally
        {
            SmokeApplication.Services?.Dispose();
            Directory.Delete(s_root, recursive: true);
        }
    }

    private static async Task RunAsync(string fixture)
    {
        IClassicDesktopStyleApplicationLifetime desktop =
            (IClassicDesktopStyleApplicationLifetime)Application.Current!.ApplicationLifetime!;
        int exitCode = 0;
        try
        {
            await VerifyWorkbenchAsync(desktop, fixture);
            await VerifyStandaloneAsync(desktop, fixture);
            EditorDocumentWorkspace workspace = SmokeApplication.Services!.GetRequiredService<EditorDocumentWorkspace>();
            desktop.MainWindow!.Close();
            Check(workspace.Tabs.Count == 0, "Closing the workbench did not dispose its document tabs.");
            Console.WriteLine("Architecture: navigation, catalog control previews, structure and Grid editing, designer contributions, binary editor, drafts, safe files, standalone review/undo, MCP dispatch and screenshot verified.");
        }
        catch (Exception exception)
        {
            Console.Error.WriteLine(exception);
            exitCode = 1;
        }
        finally
        {
            foreach (Window window in desktop.Windows.ToArray())
            {
                window.DataContext = null;
                window.Close();
            }
        }

        desktop.Shutdown(exitCode);
    }

    private static async Task VerifyWorkbenchAsync(IClassicDesktopStyleApplicationLifetime desktop, string fixture)
    {
        MainWindowViewModel workbench = SmokeApplication.Services!.GetRequiredService<MainWindowViewModel>();
        Window window = desktop.MainWindow!;
        await workbench.OpenWorkspaceFileAsync(Path.Combine(fixture, "Sample.csproj"));
        ProjectExplorerViewModel explorer = workbench.ProjectExplorer!;
        ProjectExplorerItemViewModel first = FindItem(explorer.RootItems, Path.Combine(fixture, "First.axaml"));
        ProjectExplorerItemViewModel second = FindItem(explorer.RootItems, Path.Combine(fixture, "Second.axaml"));
        ProjectExplorerItemViewModel source = FindItem(explorer.RootItems, Path.Combine(fixture, "Sample.cs"));
        ProjectExplorerItemViewModel image = FindItem(explorer.RootItems, Path.Combine(fixture, "Image.png"));
        ProjectExplorerItemViewModel binary = FindItem(explorer.RootItems, Path.Combine(fixture, "Binary.cs"));
        ProjectExplorerItemViewModel fixtureBinary = FindItem(explorer.RootItems, Path.Combine(fixture, "Fixture.bin"));

        explorer.SelectedItem = first;
        Check(workbench.PropertySelectionCaption.StartsWith("Navigation - First.axaml", StringComparison.Ordinal),
            "Navigation properties did not update immediately.");
        await WaitForActiveAsync(workbench, first.Item.Path);
        Check(workbench.ActiveEditorTab.IsPreview, "Explorer selection did not create a preview.");

        explorer.SelectedItem = second;
        await WaitForActiveAsync(workbench, second.Item.Path);
        EditorTabViewModel secondTab = workbench.ActiveEditorTab;
        Check(workbench.EditorWorkspace.FindByPath(first.Item.Path) is null && secondTab.IsPreview,
            "The clean preview was not replaced.");
        await WaitUntilAsync(() => secondTab.View.GetVisualDescendants().OfType<AxamlPreviewControl>().Any(),
            "the document's designer surface");
        Check(secondTab.TextView is EditorTextArea, "The AXAML editor does not contain its code editor.");
        ((EditorTextArea)secondTab.TextView).FocusAndMoveCaret(new CodeLocation(second.Item.Path, 1, 1));
        Check(secondTab.IsPreview, "Programmatic caret navigation pinned the preview.");

        TabItem paletteTab = window.GetVisualDescendants().OfType<TabItem>()
            .First(item => string.Equals(item.Header?.ToString(), "Palette", StringComparison.Ordinal));
        TabControl leftTabs = paletteTab.GetVisualAncestors().OfType<TabControl>().First();
        leftTabs.SelectedItem = paletteTab;
        await WaitUntilAsync(() => window.GetVisualDescendants().Contains(secondTab.PaletteView),
            "the externally hosted palette");
        Check(!secondTab.View.GetVisualDescendants().Contains(secondTab.PaletteView),
            "The palette is still inside the document editor.");

        AxamlDesignerViewModel designer = secondTab.Designer!;
        TreeView structure = secondTab.PaletteView!.GetVisualDescendants().OfType<TreeView>()
            .Single(tree => tree.Name == "StructureTree");
        structure.SelectedItem = designer.PreviewRoot!.Children[1];
        await WaitUntilAsync(() => designer.SelectedNode?.StableName == "Label", "programmatic structure selection");
        Check(secondTab.IsPreview
            && workbench.PropertySelectionCaption.StartsWith("Navigation - Second.axaml", StringComparison.Ordinal),
            "Programmatic structure selection incorrectly activated the editor.");
        await WaitUntilAsync(() => structure.GetVisualDescendants().OfType<TreeViewItem>()
            .Any(item => ReferenceEquals(item.DataContext, designer.SelectedNode)), "the selected structure item");
        TreeViewItem selectedStructureItem = structure.GetVisualDescendants().OfType<TreeViewItem>()
            .First(item => ReferenceEquals(item.DataContext, designer.SelectedNode));
        Check(selectedStructureItem.Focus(NavigationMethod.Tab), "The structure item could not receive keyboard focus.");
        await WaitUntilAsync(() => !secondTab.IsPreview
            && workbench.PropertySelectionCaption.Contains("Editor - ", StringComparison.Ordinal),
            "keyboard activation of the structure tree");
        Check(workbench.PropertySelectionCaption.Contains("TextBlock", StringComparison.Ordinal),
            "Structure activation did not expose the selected control's properties.");
        Invoke(secondTab.PaletteView!, "Up");
        await WaitUntilAsync(() => designer.PreviewRoot!.Children[0].StableName == "Label"
            && designer.SelectedNode?.StableName == "Label", "structure reorder");
        Check(secondTab.Editor.IsDirty && secondTab.Editor.Text.IndexOf("Label", StringComparison.Ordinal)
            < secondTab.Editor.Text.IndexOf("Input", StringComparison.Ordinal),
            "Structure reorder did not update the shared editor buffer.");
        designer.UndoCommand.Execute(null);
        await WaitUntilAsync(() => designer.PreviewRoot!.Children[1].StableName == "Label"
            && designer.SelectedNode?.StableName == "Label", "structure undo");
        Check(!secondTab.Editor.IsDirty && ReferenceEquals(structure.SelectedItem, designer.SelectedNode),
            $"Structure undo did not restore the clean buffer and tree selection (dirty={secondTab.Editor.IsDirty}, tree={((AxamlPreviewNode?)structure.SelectedItem)?.StableName ?? "<none>"}, designer={designer.SelectedNode?.StableName ?? "<none>"}).");

        designer.SelectedNode = designer.PreviewRoot!.Children[0];
        await WaitUntilAsync(() => workbench.SelectedEditorPropertyView is { } view
            && window.GetVisualDescendants().Contains(view), "the external property contribution");
        Check(!secondTab.IsPreview && workbench.PropertySelectionCaption.Contains("Editor - ", StringComparison.Ordinal),
            "Editor activation did not pin the tab and select its properties.");
        Check(!secondTab.View.GetVisualDescendants().Contains(workbench.SelectedEditorPropertyView),
            "The properties are still inside the document editor.");

        explorer.SelectedItem = source;
        secondTab.ActivateByUser(designerSelection: true);
        await Task.Delay(350);
        Check(ReferenceEquals(workbench.ActiveEditorTab, secondTab)
            && workbench.EditorWorkspace.FindByPath(source.Item.Path) is null,
            "A delayed explorer preview overrode a newer editor activation.");

        designer.PropertyEditor.Items.Single(item => item.PropertyItem.Name == "Width").ScalarValue = "-2";
        Check(designer.HasUnappliedPropertyDrafts, "Invalid property input was not retained.");
        Button closeTab = window.GetVisualDescendants().OfType<Button>()
            .Single(button => ReferenceEquals(button.DataContext, secondTab)
                && string.Equals(button.Content?.ToString(), "x", StringComparison.Ordinal));
        closeTab.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        await WaitUntilAsync(() => desktop.Windows.OfType<AA98_AvlnCodeStudio.UI.Views.DiscardWorkbenchChangesDialog>().Any(),
            "the workbench tab close prompt");
        Click(desktop.Windows.OfType<AA98_AvlnCodeStudio.UI.Views.DiscardWorkbenchChangesDialog>().Single(), "Cancel");
        await WaitUntilAsync(() => !desktop.Windows.OfType<AA98_AvlnCodeStudio.UI.Views.DiscardWorkbenchChangesDialog>().Any(),
            "the cancelled tab close prompt");
        Check(ReferenceEquals(workbench.EditorWorkspace.FindByPath(second.Item.Path), secondTab)
            && designer.HasUnappliedPropertyDrafts, "Cancelling tab close lost the property draft.");

        window.Close();
        await WaitUntilAsync(() => desktop.Windows.OfType<AA98_AvlnCodeStudio.UI.Views.DiscardWorkbenchChangesDialog>().Any(),
            "the workbench window close prompt");
        Click(desktop.Windows.OfType<AA98_AvlnCodeStudio.UI.Views.DiscardWorkbenchChangesDialog>().Single(), "Cancel");
        await WaitUntilAsync(() => !desktop.Windows.OfType<AA98_AvlnCodeStudio.UI.Views.DiscardWorkbenchChangesDialog>().Any(),
            "the cancelled workbench close prompt");
        Check(window.IsVisible && designer.HasUnappliedPropertyDrafts, "Cancelling workbench close lost the property draft.");

        designer.SelectedNode = designer.PreviewRoot.Children[1];
        Check(designer.UnappliedDrafts.Single().Value == "-2", "The draft was lost on selection change.");
        designer.SelectedNode = designer.PreviewRoot.Children[0];
        Check(designer.PropertyRows.Single(row => row.Name == "Width").Value == "-2",
            "The unchanged target did not recover its draft.");
        designer.SelectedNode = designer.PreviewRoot.Children[1];
        secondTab.Editor.Text = secondTab.Editor.Text.Replace("Width=\"100\"", "Width=\"120\"", StringComparison.Ordinal);
        designer.SelectedNode = designer.PreviewRoot!.Children[0];
        AxamlPropertyDraft retained = designer.UnappliedDrafts.Single();
        Check(retained.CanRestore && designer.RestoreDraft(retained), "Changed-source draft recovery was not explicit.");
        AxamlPropertyRowViewModel width = designer.PropertyRows.Single(row => row.Name == "Width");
        Check(width.Value == "-2", "Draft recovery changed the retained value.");
        designer.DiscardPropertyDraft(width);
        await secondTab.Editor.SaveCommand.ExecuteAsync(null);
        Check(!secondTab.Editor.IsDirty && File.ReadAllText(second.Item.Path).Contains("Width=\"120\"", StringComparison.Ordinal),
            "Saving did not preserve the shared code/designer buffer.");

        explorer.SelectedItem = first;
        explorer.SelectedItem = source;
        await WaitForActiveAsync(workbench, source.Item.Path);
        Check(workbench.PropertySelectionCaption.StartsWith("Navigation - Sample.cs", StringComparison.Ordinal),
            "Navigation did not retake property ownership.");
        Check(workbench.ActiveEditorTab.IsPreview && workbench.ActiveEditorTab.Designer is null,
            "A C# file did not use the text editor preview.");
        explorer.SelectedItem = first;
        await WaitForActiveAsync(workbench, first.Item.Path);
        Check(workbench.EditorWorkspace.FindByPath(source.Item.Path) is null
            && ReferenceEquals(secondTab, workbench.EditorWorkspace.FindByPath(second.Item.Path)),
            "Preview replacement discarded a pinned document or kept the previous preview.");
        await explorer.OpenAsync(first);
        Check(!workbench.ActiveEditorTab.IsPreview, "Explicit explorer activation did not pin the document.");
        Check(ReferenceEquals(leftTabs.SelectedItem, paletteTab), "Changing editors changed the user's palette selection.");

        byte[] imageBytes = File.ReadAllBytes(image.Item.Path);
        explorer.SelectedItem = image;
        await WaitForActiveAsync(workbench, image.Item.Path);
        Check(workbench.ActiveEditorTab.IsDiagnostic && !workbench.CanEditActiveDocument,
            "An unsupported image did not show a read-only diagnostic.");
        Check(File.ReadAllBytes(image.Item.Path).SequenceEqual(imageBytes), "The unsupported file was modified.");
        byte[] binaryBytes = File.ReadAllBytes(binary.Item.Path);
        explorer.SelectedItem = binary;
        await WaitForActiveAsync(workbench, binary.Item.Path);
        Check(workbench.ActiveEditorTab.IsDiagnostic && File.ReadAllBytes(binary.Item.Path).SequenceEqual(binaryBytes),
            "A binary file was opened or modified as text.");

        explorer.SelectedItem = fixtureBinary;
        await WaitForActiveAsync(workbench, fixtureBinary.Item.Path);
        EditorTabViewModel binaryTab = workbench.ActiveEditorTab;
        BinarySmokeSession session = ((BinarySmokeFactory)SmokeApplication.Services!
            .GetRequiredService<IDocumentEditorFactory<EditorTabViewModel>>()).LastSession!;
        Check(binaryTab.IsPreview && !binaryTab.HasTextEditor && session.Content.SequenceEqual(new byte[] { 0x00, 0xff, 0x42 }),
            "The registered binary editor did not open its own document state.");
        await WaitUntilAsync(() => window.GetVisualDescendants().Contains(binaryTab.View), "the registered binary editor view");
        session.ReplaceContent([0xfe, 0x00, 0x24]);
        Check(!binaryTab.IsPreview && binaryTab.IsDirty, "The binary editor did not pin before mutation.");
        Button saveButton = window.GetVisualDescendants().OfType<Button>()
            .Single(button => ReferenceEquals(button.Command, binaryTab.SaveCommand));
        Check(saveButton.IsEnabled && saveButton.Command!.CanExecute(null),
            "The binary editor Save button is disabled.");
        IInvokeProvider saveProvider = ControlAutomationPeer.CreatePeerForElement(saveButton)
            .GetProvider<IInvokeProvider>() ?? throw new InvalidOperationException("The Save button has no invoke provider.");
        saveProvider.Invoke();
        await WaitUntilAsync(
            () => !binaryTab.IsDirty && File.ReadAllBytes(fixtureBinary.Item.Path).SequenceEqual(new byte[] { 0xfe, 0x00, 0x24 }),
            "saving the registered binary editor");
        Button closeBinaryTab = window.GetVisualDescendants().OfType<Button>()
            .Single(button => ReferenceEquals(button.DataContext, binaryTab)
                && string.Equals(button.Content?.ToString(), "x", StringComparison.Ordinal));
        closeBinaryTab.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        await WaitUntilAsync(() => workbench.EditorWorkspace.FindByPath(fixtureBinary.Item.Path) is null,
            "closing the binary editor tab");
        Check(session.IsDisposed, "Closing the tab did not dispose the binary editor session.");

        ProjectExplorerItemViewModel gridItem = FindItem(explorer.RootItems, Path.Combine(fixture, "Grid.axaml"));
        explorer.SelectedItem = gridItem;
        await WaitForActiveAsync(workbench, gridItem.Item.Path);
        EditorTabViewModel gridTab = workbench.ActiveEditorTab;
        await WaitUntilAsync(() => window.GetVisualDescendants().Contains(gridTab.PaletteView)
            && gridTab.View.GetVisualDescendants().OfType<AxamlPreviewControl>().Any(), "the Grid designer");
        await VerifyGridEditingAsync(gridTab.Designer!, gridTab.PaletteView!, gridTab.View,
            () => gridTab.Editor.Text, () => gridTab.Editor.IsDirty);
        Check(!gridTab.IsPreview, "Grid editing did not protect the workbench preview tab.");

        ProjectExplorerItemViewModel contentItem = FindItem(explorer.RootItems, Path.Combine(fixture, "Content.axaml"));
        explorer.SelectedItem = contentItem;
        await WaitForActiveAsync(workbench, contentItem.Item.Path);
        EditorTabViewModel contentTab = workbench.ActiveEditorTab;
        await WaitUntilAsync(() => window.GetVisualDescendants().Contains(contentTab.PaletteView),
            "the single-content designer");
        await WaitUntilAsync(() => contentTab.Designer?.PreviewRoot is
            { TypeName: "Border", Children.Count: 1 },
            "the single-content preview");
        await VerifySingleContentRemovalAsync(contentTab.Designer!, contentTab.PaletteView!,
            () => contentTab.Editor.Text, () => contentTab.Editor.IsDirty);

        ProjectExplorerItemViewModel controlPreviewsItem = FindItem(
            explorer.RootItems,
            Path.Combine(fixture, "ControlPreviews.axaml"));
        explorer.SelectedItem = controlPreviewsItem;
        await WaitForActiveAsync(workbench, controlPreviewsItem.Item.Path);
        await VerifyCatalogControlPreviewsAsync(workbench.ActiveEditorTab.View);
    }

    private static async Task VerifyStandaloneAsync(IClassicDesktopStyleApplicationLifetime desktop, string fixture)
    {
        string path = Path.Combine(fixture, "Host.axaml");
        AxamlDesignerHostViewModel host = new();
        AA98_AxamlDesigner.Host.Views.MainWindow window = new() { DataContext = host };
        window.Show();
        Check(host.OpenDocument(path) && host.SetWorkspaceEntry(Path.Combine(fixture, "Sample.csproj")),
            "The standalone designer could not open the fixture.");
        await WaitUntilAsync(() => window.GetVisualDescendants().OfType<AxamlDesignerSurfaceView>()
            .Any(surface => surface.Bounds.Width > 0 && surface.Bounds.Height > 0), "the standalone designer surface");
        Check(window.GetVisualDescendants().OfType<AxamlDesignerPaletteView>().Any()
            && window.GetVisualDescendants().OfType<AxamlDesignerPropertiesView>().Any()
            && window.GetVisualDescendants().OfType<EditorTextArea>().Any(),
            "The standalone host did not compose the shared designer components and code editor.");
        AxamlDesignerPaletteView palette = window.GetVisualDescendants().OfType<AxamlDesignerPaletteView>().Single();
        TreeView structure = palette.GetVisualDescendants().OfType<TreeView>()
            .Single(tree => tree.Name == "StructureTree");
        string originalMarkup = host.Markup;
        structure.SelectedItem = host.Designer.PreviewRoot!.Children[1];
        await WaitUntilAsync(() => host.Designer.SelectedNode?.TypeName == "Button", "standalone structure selection");
        Invoke(palette, "Up");
        await WaitUntilAsync(() => host.Designer.PreviewRoot!.Children[0].TypeName == "Button"
            && host.Designer.SelectedNode?.TypeName == "Button", "standalone structure reorder");
        Check(host.IsDirty && host.Markup.IndexOf("<Button", StringComparison.Ordinal)
            < host.Markup.IndexOf("<TextBox", StringComparison.Ordinal),
            "Standalone structure editing did not update the code buffer.");
        host.Designer.UndoCommand.Execute(null);
        Check(host.Markup == originalMarkup && !host.IsDirty, "Standalone structure undo did not restore the buffer.");
        host.Designer.RedoCommand.Execute(null);
        Check(host.IsDirty && host.Designer.SelectedNode?.TypeName == "Button", "Standalone structure redo failed.");
        host.Designer.UndoCommand.Execute(null);
        Check(host.Markup == originalMarkup && !host.IsDirty, "Standalone structure history did not restore the document.");
        byte[] png = await new AxamlDesignerScreenshotSource(host).CapturePngAsync(CancellationToken.None);
        Check(png.Length > 8 && png.AsSpan(0, 8).SequenceEqual(new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 }),
            "The standalone MCP screenshot did not produce PNG data.");

        host.Designer.SelectedNode = host.Designer.PreviewRoot!.Children[0];
        AxamlPropertyRowViewModel width = host.Designer.PropertyRows.Single(row => row.Name == "Width");
        Check(!width.TrySetValue("-2") && host.HasUnsavedChanges, "The standalone draft was not protected.");
        Check(!host.OpenDocument(Path.Combine(fixture, "First.axaml")), "Opening silently discarded the standalone draft.");
        window.Close();
        await WaitUntilAsync(() => desktop.Windows.OfType<AxamlUnsavedChangesDialog>().Any(), "the standalone close prompt");
        AxamlUnsavedChangesDialog prompt = desktop.Windows.OfType<AxamlUnsavedChangesDialog>().Single();
        Check(!FindButton(prompt, "Save").IsEnabled, "An unapplied property draft was incorrectly offered as saved.");
        Click(prompt, "Cancel");
        await WaitUntilAsync(() => !desktop.Windows.OfType<AxamlUnsavedChangesDialog>().Any(), "the cancelled close prompt");
        Check(window.IsVisible && width.Value == "-2", "Cancelling close lost the standalone draft.");
        host.Designer.DiscardPropertyDraft(width);

        host.Markup = host.Markup.Replace("Text=\"Host\"", "Text=\"Saved\"", StringComparison.Ordinal);
        Check(host.SaveDocument() && !host.HasUnsavedChanges, "The standalone code editor did not save its buffer.");
        string saved = host.Markup;
        string proposed = saved.Replace("Text=\"Saved\"", "Text=\"Reviewed\"", StringComparison.Ordinal);
        WorkspaceFileChangeProposal proposal = host.PrepareWorkspaceChanges(
        [
            WorkspaceFileChangeRequest.ReplaceExisting("Host.axaml", saved, proposed, expectedOpenDocumentContent: saved),
        ]);
        Task<bool> reviewing = host.ReviewWorkspaceFileChangesAsync!(proposal, CancellationToken.None);
        await WaitUntilAsync(() => desktop.Windows.OfType<WorkspaceFileChangeReviewDialog>().Any(), "standalone review");
        Click(desktop.Windows.OfType<WorkspaceFileChangeReviewDialog>().Single(), "Apply 1 change(s)");
        Check(await reviewing && host.Markup == proposed && File.ReadAllText(path) == proposed,
            "The standalone review did not synchronize the open document.");
        Check(host.WorkspaceFileChangeService.Undo() && host.Markup == saved, "Standalone workspace undo failed.");
        Check(host.WorkspaceFileChangeService.Redo() && host.Markup == proposed, "Standalone workspace redo failed.");

        string mcpPath = Path.Combine(fixture, "Mcp.axaml");
        File.WriteAllText(mcpPath, "<StackPanel><TextBlock Text=\"Before\" /></StackPanel>");
        ServiceCollection services = new();
        services.AddSingleton(host);
        services.AddSingleton<ComponentMcpCommandCatalog>();
        services.AddAxamlDesignerMcpExports();
        using ServiceProvider provider = services.BuildServiceProvider();
        ComponentMcpCommandCatalog catalog = provider.GetRequiredService<ComponentMcpCommandCatalog>();
        using JsonDocument openArguments = JsonDocument.Parse(JsonSerializer.Serialize(new { path = mcpPath }));
        using JsonDocument setArguments = JsonDocument.Parse("""{"nodePath":"0","propertyName":"Text","value":"After"}""");
        using JsonDocument canceledArguments = JsonDocument.Parse("""{"nodePath":"0","propertyName":"Text","value":"Canceled"}""");
        using JsonDocument emptyArguments = JsonDocument.Parse("{}");
        int markupChanges = 0;
        bool allChangesOnUiThread = true;
        PropertyChangedEventHandler observeMarkup = (_, eventArgs) =>
        {
            if (eventArgs.PropertyName == nameof(AxamlDesignerHostViewModel.Markup))
            {
                markupChanges++;
                allChangesOnUiThread &= Dispatcher.UIThread.CheckAccess();
            }
        };
        host.PropertyChanged += observeMarkup;
        try
        {
            await Task.Run(() => catalog.ExecuteAsync("designer.open", openArguments.RootElement, CancellationToken.None))
                .WaitAsync(TimeSpan.FromSeconds(10));
            await Task.Run(() => catalog.ExecuteAsync("designer.set_property", setArguments.RootElement, CancellationToken.None))
                .WaitAsync(TimeSpan.FromSeconds(10));
            await Task.Run(() => catalog.ExecuteAsync("designer.save", emptyArguments.RootElement, CancellationToken.None))
                .WaitAsync(TimeSpan.FromSeconds(10));
            Check(markupChanges >= 2 && allChangesOnUiThread
                && File.ReadAllText(mcpPath).Contains("Text=\"After\"", StringComparison.Ordinal),
                "The standalone MCP commands did not execute on the UI thread and save the document.");

            using CancellationTokenSource cancellation = new();
            Task<string> canceledRequest = Task.Factory.StartNew(() =>
            {
                Task<string> request = catalog.ExecuteAsync(
                    "designer.set_property", canceledArguments.RootElement, cancellation.Token);
                Check(!request.IsCompleted, "The MCP request was not queued on the UI dispatcher.");
                cancellation.Cancel();
                return request;
            }, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default)
                .WaitAsync(TimeSpan.FromSeconds(5)).GetAwaiter().GetResult();
            try
            {
                await canceledRequest.WaitAsync(TimeSpan.FromSeconds(5));
                throw new InvalidOperationException("A canceled queued MCP request completed.");
            }
            catch (OperationCanceledException)
            {
            }
            TaskCompletionSource<bool> dispatchBarrier = new(TaskCreationOptions.RunContinuationsAsynchronously);
            Dispatcher.UIThread.Post(() => dispatchBarrier.TrySetResult(true), DispatcherPriority.Background);
            await dispatchBarrier.Task.WaitAsync(TimeSpan.FromSeconds(5));
            Check(host.Markup.Contains("Text=\"After\"", StringComparison.Ordinal),
                "A canceled queued MCP request changed the document.");
        }
        finally
        {
            host.PropertyChanged -= observeMarkup;
        }

        Check(host.OpenDocument(Path.Combine(fixture, "Grid.axaml")), "The standalone Grid fixture did not open.");
        await VerifyGridEditingAsync(host.Designer, palette, window, () => host.Markup, () => host.IsDirty);
        Check(host.OpenDocument(Path.Combine(fixture, "Content.axaml")),
            "The standalone single-content fixture did not open.");
        await WaitUntilAsync(() => host.Designer.PreviewRoot?.TypeName == "Border",
            "the standalone single-content fixture");
        await VerifySingleContentRemovalAsync(host.Designer, palette, () => host.Markup, () => host.IsDirty);
        Check(host.OpenDocument(Path.Combine(fixture, "ControlPreviews.axaml")),
            "The standalone catalog-control preview fixture did not open.");
        await WaitUntilAsync(() => host.Designer.PreviewRoot?.TypeName == "StackPanel",
            "the standalone catalog-control preview fixture");
        await VerifyCatalogControlPreviewsAsync(window);
        window.Close();
    }

    private static async Task VerifyCatalogControlPreviewsAsync(Control root)
    {
        await WaitUntilAsync(() => root.GetVisualDescendants().OfType<ComboBox>().Any()
            && root.GetVisualDescendants().OfType<NumericUpDown>().Any()
            && root.GetVisualDescendants().OfType<ProgressBar>().Any()
            && root.GetVisualDescendants().OfType<Slider>().Any()
            && root.GetVisualDescendants().OfType<WrapPanel>().Count() == 3
            && root.GetVisualDescendants().OfType<CheckBox>().Any()
            && root.GetVisualDescendants().OfType<RadioButton>().Any()
            && root.GetVisualDescendants().OfType<ToggleSwitch>().Any(),
            "catalogued numeric, progress, and toggle control previews");
        ComboBox comboBox = root.GetVisualDescendants().OfType<ComboBox>()
            .Single(candidate => candidate.CornerRadius == new CornerRadius(4));
        NumericUpDown numericUpDown = root.GetVisualDescendants().OfType<NumericUpDown>()
            .Single(candidate => candidate.Value == 12.5m);
        NumericUpDown defaultNumericUpDown = root.GetVisualDescendants().OfType<NumericUpDown>()
            .Single(candidate => candidate.Value == 0m);
        NumericUpDown invalidFormatNumericUpDown = root.GetVisualDescendants().OfType<NumericUpDown>()
            .Single(candidate => candidate.Value == 1m);
        ProgressBar progressBar = root.GetVisualDescendants().OfType<ProgressBar>()
            .Single(candidate => candidate.Maximum == 90);
        ProgressBar indeterminateProgressBar = root.GetVisualDescendants().OfType<ProgressBar>()
            .Single(candidate => candidate.IsIndeterminate);
        ProgressBar invalidFormatProgressBar = root.GetVisualDescendants().OfType<ProgressBar>()
            .Single(candidate => candidate.Value == 25);
        Slider slider = root.GetVisualDescendants().OfType<Slider>().Single();
        DockPanel dockPanel = root.GetVisualDescendants().OfType<DockPanel>()
            .Single(candidate => candidate.HorizontalSpacing == 4);
        DockPanel invalidDockPanel = root.GetVisualDescendants().OfType<DockPanel>()
            .Single(candidate => candidate.LastChildFill == false && candidate.HorizontalSpacing == 0);
        Avalonia.Controls.Primitives.UniformGrid uniformGrid = root.GetVisualDescendants()
            .OfType<Avalonia.Controls.Primitives.UniformGrid>()
            .Single(candidate => candidate.ColumnSpacing == 5);
        Avalonia.Controls.Primitives.UniformGrid invalidUniformGrid = root.GetVisualDescendants()
            .OfType<Avalonia.Controls.Primitives.UniformGrid>()
            .Single(candidate => candidate.Columns == 4);
        Grid grid = root.GetVisualDescendants().OfType<Grid>()
            .Single(candidate => candidate.Width == 99);
        Grid invalidGridSpacing = root.GetVisualDescendants().OfType<Grid>()
            .Single(candidate => candidate.Width == 98);
        ScrollViewer scrollViewer = root.GetVisualDescendants().OfType<ScrollViewer>()
            .Single(candidate => candidate.Width == 97);
        ScrollViewer invalidScrollViewer = root.GetVisualDescendants().OfType<ScrollViewer>()
            .Single(candidate => candidate.Width == 96);
        Viewbox viewbox = root.GetVisualDescendants().OfType<Viewbox>()
            .Single(candidate => candidate.Width == 93);
        Viewbox invalidViewbox = root.GetVisualDescendants().OfType<Viewbox>()
            .Single(candidate => candidate.Width == 92);
        WrapPanel wrapPanel = root.GetVisualDescendants().OfType<WrapPanel>()
            .Single(candidate => candidate.ItemSpacing == 5);
        WrapPanel defaultWrapPanel = root.GetVisualDescendants().OfType<WrapPanel>()
            .Single(candidate => candidate.Orientation == Orientation.Horizontal && double.IsNaN(candidate.ItemWidth));
        WrapPanel invalidWrapPanel = root.GetVisualDescendants().OfType<WrapPanel>()
            .Single(candidate => candidate.Orientation == Orientation.Vertical);
        CheckBox checkBox = root.GetVisualDescendants().OfType<CheckBox>().Single();
        RadioButton radioButton = root.GetVisualDescendants().OfType<RadioButton>().Single();
        Button keyboardActionButton = root.GetVisualDescendants().OfType<Button>()
            .Single(button => button.Content?.ToString() == "Keyboard action preview");
        Button invalidClipButton = root.GetVisualDescendants().OfType<Button>()
            .Single(button => button.Content?.ToString() == "Clip fallback");
        Button hiddenPreviewButton = root.GetVisualDescendants().OfType<Button>()
            .Single(button => button.Width == 90);
        Button invalidVisibilityButton = root.GetVisualDescendants().OfType<Button>()
            .Single(button => button.Width == 89);
        Border styledBorder = root.GetVisualDescendants().OfType<Border>()
            .Single(border => border.CornerRadius == new CornerRadius(12, 4, 8, 2));
        TextBox inputTextBox = root.GetVisualDescendants().OfType<TextBox>().Single(textBox => textBox.MaxLength == 120);
        TextBox defaultLineCountTextBox = root.GetVisualDescendants().OfType<TextBox>()
            .Single(textBox => textBox.Text == "Default line count preview");
        TextBox invalidLineCountTextBox = root.GetVisualDescendants().OfType<TextBox>()
            .Single(textBox => textBox.Text == "Invalid line count preview");
        TextBlock alignedTextBlock = root.GetVisualDescendants().OfType<TextBlock>().Single(textBlock => textBlock.Text == "Alignment preview");
        TextBlock inheritedFlowTextBlock = root.GetVisualDescendants().OfType<TextBlock>().Single(textBlock => textBlock.Text == "Inherited RTL direction");
        TextBlock invalidFlowTextBlock = root.GetVisualDescendants().OfType<TextBlock>().Single(textBlock => textBlock.Text == "Invalid flow fallback");
        TextBlock boundFlowTextBlock = root.GetVisualDescendants().OfType<TextBlock>().Single(textBlock => textBlock.Text == "Bound flow fallback");
        ContentControl alignedContentControl = root.GetVisualDescendants().OfType<ContentControl>()
            .Single(control => control.GetType() == typeof(ContentControl)
                && control.Content is Control content
                && content.GetVisualDescendants().OfType<TextBlock>()
                    .Any(textBlock => textBlock.Text == "ContentControl alignment"));
        Expander alignedExpander = root.GetVisualDescendants().OfType<Expander>()
            .Single(expander => expander.Header?.ToString() == "Expander alignment");
        GroupBox alignedGroupBox = root.GetVisualDescendants().OfType<GroupBox>()
            .Single(groupBox => groupBox.Header?.ToString() == "GroupBox alignment");
        Label alignedLabel = root.GetVisualDescendants().OfType<Label>()
            .Single(label => label.Content is TextBlock labelText && labelText.Text == "Label alignment");
        ToggleSwitch toggleSwitch = root.GetVisualDescendants().OfType<ToggleSwitch>().Single();
        Check(comboBox.PlaceholderText == "Choose an option" && !comboBox.IsEnabled
            && comboBox.HorizontalContentAlignment == HorizontalAlignment.Right
            && comboBox.VerticalContentAlignment == VerticalAlignment.Bottom
            && comboBox.Background is ISolidColorBrush comboBackground
            && comboBackground.Color == Colors.White
            && comboBox.BorderBrush is ISolidColorBrush comboBorderBrush
            && comboBorderBrush.Color == Colors.Blue
            && comboBox.BorderThickness == new Thickness(2)
            && comboBox.CornerRadius == new CornerRadius(4)
            && comboBox.FontSize == 14
            && comboBox.FontStyle == FontStyle.Italic
            && comboBox.FontWeight == FontWeight.Medium
            && comboBox.Foreground is ISolidColorBrush comboForeground
            && comboForeground.Color == Colors.Black
            && comboBox.Padding == new Thickness(3),
            $"The ComboBox preview did not preserve its appearance or remained interactive: placeholder={comboBox.PlaceholderText}, enabled={comboBox.IsEnabled}, background={comboBox.Background}, border={comboBox.BorderBrush}, borderThickness={comboBox.BorderThickness}, cornerRadius={comboBox.CornerRadius}, fontSize={comboBox.FontSize}, fontStyle={comboBox.FontStyle}, fontWeight={comboBox.FontWeight}, foreground={comboBox.Foreground}, padding={comboBox.Padding}.");
        Check(numericUpDown.Value == 12.5m && numericUpDown.FormatString == "N2"
            && !numericUpDown.AllowSpin && !numericUpDown.ShowButtonSpinner && numericUpDown.ClipValueToMinMax
            && numericUpDown.ButtonSpinnerLocation == Location.Left
            && numericUpDown.TextAlignment == TextAlignment.Center
            && numericUpDown.HorizontalContentAlignment == HorizontalAlignment.Center
            && numericUpDown.VerticalContentAlignment == VerticalAlignment.Top
            && numericUpDown.ParsingNumberStyle == System.Globalization.NumberStyles.Integer
            && numericUpDown.Margin == new Thickness(3, 4)
            && numericUpDown.HorizontalAlignment == Avalonia.Layout.HorizontalAlignment.Right
            && numericUpDown.VerticalAlignment == Avalonia.Layout.VerticalAlignment.Center
            && numericUpDown.Width == 120 && numericUpDown.Height == 60
            && numericUpDown.MinWidth == 40 && numericUpDown.MaxWidth == 160
            && numericUpDown.MinHeight == 30 && numericUpDown.MaxHeight == 90
            && numericUpDown.Increment == 0.25m && numericUpDown.Minimum == 0.5m && numericUpDown.Maximum == 40m && !numericUpDown.IsEnabled
            && numericUpDown.Background is ISolidColorBrush numericBackground
            && numericBackground.Color == Color.FromRgb(240, 240, 240)
            && numericUpDown.BorderBrush is ISolidColorBrush numericBorderBrush
            && numericBorderBrush.Color == Colors.Green
            && numericUpDown.BorderThickness == new Thickness(2)
            && numericUpDown.CornerRadius == new CornerRadius(3)
            && numericUpDown.FontSize == 15
            && numericUpDown.FontStyle == FontStyle.Italic
            && numericUpDown.FontWeight == FontWeight.Bold
            && numericUpDown.Foreground is ISolidColorBrush numericForeground
            && numericForeground.Color == Colors.DarkGreen
            && numericUpDown.Padding == new Thickness(2),
            "The NumericUpDown preview did not preserve its value or remained interactive.");
        Check(defaultNumericUpDown.AllowSpin && defaultNumericUpDown.ShowButtonSpinner
            && !defaultNumericUpDown.ClipValueToMinMax
            && defaultNumericUpDown.ButtonSpinnerLocation == Location.Right
            && defaultNumericUpDown.TextAlignment == TextAlignment.Start
            && defaultNumericUpDown.ParsingNumberStyle == System.Globalization.NumberStyles.Any
            && !defaultNumericUpDown.IsEnabled,
            "The default NumericUpDown preview did not preserve Avalonia spin defaults or remained interactive.");
        Check(invalidFormatNumericUpDown.FormatString == defaultNumericUpDown.FormatString
            && invalidFormatNumericUpDown.ParsingNumberStyle == defaultNumericUpDown.ParsingNumberStyle
            && invalidFormatNumericUpDown.HorizontalAlignment == Avalonia.Layout.HorizontalAlignment.Stretch
            && invalidFormatNumericUpDown.VerticalAlignment == Avalonia.Layout.VerticalAlignment.Stretch
            && invalidFormatNumericUpDown.HorizontalContentAlignment == defaultNumericUpDown.HorizontalContentAlignment
            && invalidFormatNumericUpDown.VerticalContentAlignment == defaultNumericUpDown.VerticalContentAlignment,
            "The NumericUpDown preview applied an invalid format, parsing style or layout alignment.");
        Check(wrapPanel.ItemSpacing == 5 && wrapPanel.LineSpacing == 7
            && wrapPanel.ItemWidth == 40 && wrapPanel.ItemHeight == 20
            && wrapPanel.ItemsAlignment == WrapPanelItemsAlignment.Center
            && wrapPanel.Children.Count == 2,
            "The WrapPanel preview did not preserve its spacing, item size, alignment or children.");
        Check(defaultWrapPanel.ItemSpacing == 0 && defaultWrapPanel.LineSpacing == 0
            && double.IsNaN(defaultWrapPanel.ItemWidth) && double.IsNaN(defaultWrapPanel.ItemHeight)
            && defaultWrapPanel.ItemsAlignment == WrapPanelItemsAlignment.Start,
            "The WrapPanel preview did not preserve its framework defaults.");
        Check(invalidWrapPanel.ItemSpacing == 0 && invalidWrapPanel.LineSpacing == 0
            && double.IsNaN(invalidWrapPanel.ItemWidth) && double.IsNaN(invalidWrapPanel.ItemHeight)
            && invalidWrapPanel.ItemsAlignment == WrapPanelItemsAlignment.Start,
            "The WrapPanel preview applied an invalid layout value.");
        Check(dockPanel.HorizontalSpacing == 4 && dockPanel.VerticalSpacing == 6
            && !dockPanel.LastChildFill && dockPanel.Children.Count == 2,
            "The DockPanel preview did not preserve spacing, last-child-fill or children.");
        Check(invalidDockPanel.HorizontalSpacing == 0 && invalidDockPanel.VerticalSpacing == 0
            && !invalidDockPanel.LastChildFill,
            "The DockPanel preview applied invalid spacing values.");
        Check(uniformGrid.Rows == 2 && uniformGrid.Columns == 3
            && uniformGrid.RowSpacing == 3 && uniformGrid.ColumnSpacing == 5
            && uniformGrid.Children.Count == 2,
            "The UniformGrid preview did not preserve its row/column spacing or layout.");
        Check(invalidUniformGrid.RowSpacing == 0 && invalidUniformGrid.ColumnSpacing == 0,
            "The UniformGrid preview applied invalid spacing values.");
        Check(grid.RowSpacing == 2 && grid.ColumnSpacing == 4 && grid.ShowGridLines,
            "The Grid preview did not preserve row/column spacing or show-grid-lines.");
        Check(invalidGridSpacing.RowSpacing == 0 && invalidGridSpacing.ColumnSpacing == 0
            && !invalidGridSpacing.ShowGridLines,
            "The Grid preview applied invalid spacing or show-grid-lines values.");
        Check(scrollViewer.HorizontalScrollBarVisibility == ScrollBarVisibility.Hidden
            && scrollViewer.VerticalScrollBarVisibility == ScrollBarVisibility.Disabled,
            "The ScrollViewer preview did not preserve its scrollbar visibility settings.");
        Check(invalidScrollViewer.HorizontalScrollBarVisibility == ScrollBarVisibility.Auto
            && invalidScrollViewer.VerticalScrollBarVisibility == ScrollBarVisibility.Auto,
            "The ScrollViewer preview applied invalid scrollbar visibility values.");
        Check(viewbox.StretchDirection == StretchDirection.UpOnly
            && invalidViewbox.StretchDirection == StretchDirection.Both,
            "The Viewbox preview did not preserve or safely default its stretch direction.");
        Check(progressBar.Minimum == 10 && progressBar.Maximum == 90 && progressBar.Value == 42.5
            && !progressBar.IsIndeterminate && progressBar.ShowProgressText
            && progressBar.ProgressTextFormat == "{1:0.0}%"
            && progressBar.Orientation == Orientation.Horizontal && !progressBar.IsEnabled
            && progressBar.Background is ISolidColorBrush progressBackground
            && progressBackground.Color == Colors.White
            && progressBar.BorderBrush is ISolidColorBrush progressBorderBrush
            && progressBorderBrush.Color == Colors.Purple
            && progressBar.BorderThickness == new Thickness(1)
            && progressBar.CornerRadius == new CornerRadius(2)
            && progressBar.FontSize == 13
            && progressBar.FontWeight == FontWeight.Normal
            && progressBar.Foreground is ISolidColorBrush progressForeground
            && progressForeground.Color == Colors.Blue
            && progressBar.Padding == new Thickness(1),
            "The ProgressBar preview did not preserve its value/state or remained interactive.");
        Check(indeterminateProgressBar.IsIndeterminate && !indeterminateProgressBar.ShowProgressText
            && indeterminateProgressBar.ProgressTextFormat == "{1:0}%"
            && invalidFormatProgressBar.ProgressTextFormat == "{1:0}%",
            "The ProgressBar preview did not preserve the indeterminate state/default text setting.");
        Check(slider.Minimum == -10 && slider.Maximum == 60 && slider.Value == 35.5
            && slider.TickFrequency == 5 && slider.IsSnapToTickEnabled
            && slider.IsDirectionReversed && slider.TickPlacement == TickPlacement.Outside
            && slider.Orientation == Orientation.Vertical && !slider.IsEnabled
            && slider.Background is ISolidColorBrush sliderBackground
            && sliderBackground.Color == Color.FromRgb(240, 240, 240)
            && slider.BorderBrush is ISolidColorBrush sliderBorderBrush
            && sliderBorderBrush.Color == Colors.Black
            && slider.BorderThickness == new Thickness(3)
            && slider.CornerRadius == new CornerRadius(5)
            && slider.FontSize == 16
            && slider.FontStyle == FontStyle.Oblique
            && slider.FontWeight == FontWeight.SemiBold
            && slider.Foreground is ISolidColorBrush sliderForeground
            && sliderForeground.Color == Colors.Red
            && slider.Padding == new Thickness(4),
            "The Slider preview did not preserve its value/orientation or remained interactive.");
        Check(checkBox.IsChecked == true && checkBox.IsThreeState && !checkBox.IsEnabled
            && checkBox.HorizontalContentAlignment == HorizontalAlignment.Left
            && checkBox.VerticalContentAlignment == VerticalAlignment.Center
            && checkBox.Background is ISolidColorBrush checkBoxBackground
            && checkBoxBackground.Color == Color.FromRgb(240, 240, 240)
            && checkBox.BorderBrush is ISolidColorBrush checkBoxBorderBrush
            && checkBoxBorderBrush.Color == Colors.Blue
            && checkBox.BorderThickness == new Thickness(2)
            && checkBox.CornerRadius == new CornerRadius(1, 2, 3, 4)
            && checkBox.FontSize == 16
            && checkBox.FontStyle == FontStyle.Italic
            && checkBox.FontWeight == FontWeight.Medium
            && checkBox.Foreground is ISolidColorBrush checkBoxForeground
            && checkBoxForeground.Color == Colors.White
            && checkBox.Padding == new Thickness(4),
            "The CheckBox preview did not preserve its state or remained interactive.");
        Check(radioButton.IsChecked == false && radioButton.GroupName == "Choices" && !radioButton.IsEnabled
            && radioButton.HorizontalContentAlignment == HorizontalAlignment.Center
            && radioButton.VerticalContentAlignment == VerticalAlignment.Top
            && radioButton.Background is ISolidColorBrush radioBackground
            && radioBackground.Color == Colors.White
            && radioButton.BorderBrush is ISolidColorBrush radioBorderBrush
            && radioBorderBrush.Color == Colors.Purple
            && radioButton.BorderThickness == new Thickness(1)
            && radioButton.CornerRadius == new CornerRadius(4)
            && radioButton.FontSize == 17
            && radioButton.FontStyle == FontStyle.Oblique
            && radioButton.FontWeight == FontWeight.Bold
            && radioButton.Foreground is ISolidColorBrush radioForeground
            && radioForeground.Color == Colors.Black
            && radioButton.Padding == new Thickness(3),
            "The RadioButton preview did not preserve its state or remained interactive.");
        Check(!keyboardActionButton.IsEnabled && !keyboardActionButton.IsDefault && !keyboardActionButton.IsCancel
            && keyboardActionButton.HorizontalContentAlignment == HorizontalAlignment.Left
            && keyboardActionButton.VerticalContentAlignment == VerticalAlignment.Top
            && keyboardActionButton.ClipToBounds && !invalidClipButton.ClipToBounds
            && keyboardActionButton.Opacity == 0.8
            && keyboardActionButton.MinWidth == 100 && keyboardActionButton.MaxWidth == 200
            && keyboardActionButton.MinHeight == 30 && keyboardActionButton.MaxHeight == 60
            && keyboardActionButton.Background is ISolidColorBrush buttonBackground
            && buttonBackground.Color == Color.FromRgb(240, 240, 240)
            && keyboardActionButton.BorderBrush is ISolidColorBrush buttonBorderBrush
            && buttonBorderBrush.Color == Colors.Blue
            && keyboardActionButton.BorderThickness == new Thickness(2)
            && keyboardActionButton.CornerRadius == new CornerRadius(6, 4, 2, 8)
            && keyboardActionButton.FontSize == 18
            && keyboardActionButton.FontStyle == FontStyle.Italic
            && keyboardActionButton.FontWeight == FontWeight.Medium
            && keyboardActionButton.Foreground is ISolidColorBrush buttonForeground
            && buttonForeground.Color == Colors.White
            && keyboardActionButton.Padding == new Thickness(8, 4),
            "The Button preview did not suppress Enter/Escape actions from the source markup.");
        Check(!hiddenPreviewButton.IsVisible && invalidVisibilityButton.IsVisible,
            "The Button preview did not preserve visibility or use its safe default.");
        Check(styledBorder.BorderBrush is ISolidColorBrush styledBorderBrush
            && styledBorderBrush.Color == Colors.Purple
            && styledBorder.CornerRadius == new CornerRadius(12, 4, 8, 2)
            && styledBorder.BorderThickness == new Thickness(2)
            && styledBorder.Padding == new Thickness(6),
            "The Border preview did not preserve its validated appearance properties.");
        Check(inputTextBox.Text == "Input preview"
            && inputTextBox.MaxLength == 120
            && inputTextBox.MinLines == 2 && inputTextBox.MaxLines == 5
            && inputTextBox.LineHeight == 22 && inputTextBox.UseFloatingPlaceholder
            && inputTextBox.AcceptsReturn
            && !inputTextBox.AcceptsTab
            && inputTextBox.Opacity == 0.75
            && inputTextBox.FontSize == 18
            && inputTextBox.TextAlignment == TextAlignment.Right
            && inputTextBox.TextWrapping == TextWrapping.WrapWithOverflow
            && inputTextBox.PlaceholderForeground is ISolidColorBrush placeholderForeground
            && placeholderForeground.Color == Colors.DarkGray
            && inputTextBox.FontStyle == FontStyle.Italic
            && inputTextBox.FontWeight == FontWeight.Medium
            && inputTextBox.HorizontalAlignment == HorizontalAlignment.Center
            && inputTextBox.VerticalAlignment == VerticalAlignment.Bottom
            && inputTextBox.IsReadOnly
            && inputTextBox.BorderThickness == new Thickness(2)
            && inputTextBox.CornerRadius == new CornerRadius(3, 6, 9, 12)
            && inputTextBox.Padding == new Thickness(8, 4)
            && inputTextBox.Background is ISolidColorBrush inputBackground
            && inputBackground.Color == Color.FromRgb(240, 240, 240)
            && inputTextBox.BorderBrush is ISolidColorBrush inputBorderBrush
            && inputBorderBrush.Color == Colors.Green
            && inputTextBox.Foreground is ISolidColorBrush inputForeground
            && inputForeground.Color == Colors.Blue,
            "The TextBox preview did not preserve its layout/input constraints or remained read-only.");
        Check(defaultLineCountTextBox.MinLines == 0 && defaultLineCountTextBox.MaxLines == 0
            && invalidLineCountTextBox.MinLines == defaultLineCountTextBox.MinLines
            && invalidLineCountTextBox.MaxLines == defaultLineCountTextBox.MaxLines,
            "The TextBox preview did not preserve the unset line-count defaults or reject invalid values.");
        Check(double.IsNaN(defaultLineCountTextBox.LineHeight)
            && double.IsNaN(invalidLineCountTextBox.LineHeight)
            && !defaultLineCountTextBox.UseFloatingPlaceholder
            && invalidLineCountTextBox.UseFloatingPlaceholder == defaultLineCountTextBox.UseFloatingPlaceholder
            && Equals(invalidLineCountTextBox.PlaceholderForeground, defaultLineCountTextBox.PlaceholderForeground),
            $"The TextBox preview did not preserve line-height/placeholder defaults: line heights {defaultLineCountTextBox.LineHeight}/{invalidLineCountTextBox.LineHeight}, floating placeholders {defaultLineCountTextBox.UseFloatingPlaceholder}/{invalidLineCountTextBox.UseFloatingPlaceholder}.");
        Check(alignedTextBlock.TextAlignment == TextAlignment.Center
            && alignedTextBlock.FontStyle == FontStyle.Italic
            && alignedTextBlock.MaxLines == 2
            && alignedTextBlock.Opacity == 0.5
            && alignedTextBlock.LineHeight == 24
            && alignedTextBlock.TextTrimming == TextTrimming.CharacterEllipsis,
            "The TextBlock preview did not preserve its alignment, font style, line limit, or trimming.");
        Check(alignedTextBlock.TextWrapping == TextWrapping.NoWrap,
            "The TextBlock preview did not preserve its wrapping mode.");
        Check(alignedTextBlock.FontWeight == FontWeight.SemiBold,
            "The TextBlock preview did not preserve its selected font weight.");
        Check(alignedTextBlock.Foreground is ISolidColorBrush foreground && foreground.Color == Colors.Red,
            "The TextBlock preview did not preserve its foreground brush.");
        Check(inheritedFlowTextBlock.FlowDirection == FlowDirection.RightToLeft
            && invalidFlowTextBlock.FlowDirection == FlowDirection.LeftToRight
            && boundFlowTextBlock.FlowDirection == FlowDirection.LeftToRight
            && inheritedFlowTextBlock.UseLayoutRounding
            && invalidFlowTextBlock.UseLayoutRounding == boundFlowTextBlock.UseLayoutRounding,
            "The preview did not inherit explicit flow direction or ignore invalid/bound values.");
        Check(alignedContentControl.HorizontalContentAlignment == HorizontalAlignment.Stretch
            && alignedContentControl.VerticalContentAlignment == VerticalAlignment.Bottom
            && alignedExpander.HorizontalContentAlignment == HorizontalAlignment.Right
            && alignedExpander.VerticalContentAlignment == VerticalAlignment.Top
            && alignedGroupBox.HorizontalContentAlignment == HorizontalAlignment.Left
            && alignedGroupBox.VerticalContentAlignment == VerticalAlignment.Bottom
            && alignedLabel.HorizontalContentAlignment == HorizontalAlignment.Center
            && alignedLabel.VerticalContentAlignment == VerticalAlignment.Top
            && alignedLabel.FontSize == 18
            && alignedLabel.FontStyle == FontStyle.Italic
            && alignedLabel.FontWeight == FontWeight.SemiBold
            && alignedLabel.Foreground is ISolidColorBrush labelForeground
            && labelForeground.Color == Colors.Blue
            && alignedLabel.Content is TextBlock labelText
            && labelText.TextAlignment == TextAlignment.Right
            && labelText.TextTrimming == TextTrimming.WordEllipsis
            && labelText.TextWrapping == TextWrapping.NoWrap,
            $"The Label content-alignment or typography preview did not preserve its values: alignment={alignedLabel.HorizontalContentAlignment}/{alignedLabel.VerticalContentAlignment}, font={alignedLabel.FontSize}/{alignedLabel.FontStyle}/{alignedLabel.FontWeight}, foreground={alignedLabel.Foreground}.");
        Check(toggleSwitch.Content?.ToString() == "Notifications"
            && toggleSwitch.IsChecked == true
            && toggleSwitch.IsThreeState
            && toggleSwitch.HorizontalContentAlignment == HorizontalAlignment.Right
            && toggleSwitch.VerticalContentAlignment == VerticalAlignment.Center
            && toggleSwitch.OnContent?.ToString() == "Enabled"
            && toggleSwitch.OffContent?.ToString() == "Disabled"
            && toggleSwitch.Background is ISolidColorBrush toggleBackground
            && toggleBackground.Color == Colors.PaleGreen
            && toggleSwitch.BorderBrush is ISolidColorBrush toggleBorderBrush
            && toggleBorderBrush.Color == Colors.DarkGreen
            && toggleSwitch.BorderThickness == new Thickness(3)
            && toggleSwitch.CornerRadius == new CornerRadius(5)
            && toggleSwitch.FontSize == 15
            && toggleSwitch.FontStyle == FontStyle.Italic
            && toggleSwitch.FontWeight == FontWeight.SemiBold
            && toggleSwitch.Foreground is ISolidColorBrush toggleForeground
            && toggleForeground.Color == Colors.DarkGreen
            && toggleSwitch.Padding == new Thickness(2)
            && !toggleSwitch.IsEnabled,
            "The ToggleSwitch preview did not preserve its content/state or remained interactive.");
    }

    private static async Task VerifyGridEditingAsync(
        AxamlDesignerViewModel designer,
        Control palette,
        Control editorView,
        Func<string> getMarkup,
        Func<bool> isDirty)
    {
        string original = getMarkup();
        TreeView structure = palette.GetVisualDescendants().OfType<TreeView>()
            .Single(tree => tree.Name == "StructureTree");
        structure.SelectedItem = designer.PreviewRoot!.Children[0];
        await WaitUntilAsync(() => designer.SelectedNode?.StableName == "Span"
            && designer.IsSelectedGridChild, "Grid child selection");

        Border? FindSpan() => editorView.GetVisualDescendants().OfType<AxamlPreviewControl>()
            .SelectMany(static preview => preview.GetVisualDescendants().OfType<Border>())
            .FirstOrDefault(border => border.Tag is AxamlPreviewNode { StableName: "Span" });
        await WaitUntilAsync(() => FindSpan() is { Bounds.Width: > 0, Bounds.Height: > 0 }, "the spanning Grid preview");
        Border span = FindSpan()!;
        Check(Grid.GetRowSpan(span) == 2 && Grid.GetColumnSpan(span) == 2 && Grid.GetColumn(span) == 0,
            "The Grid preview did not preserve the child's row and column spans.");
        Grid renderedGrid = (Grid)span.Parent!;
        Check(Math.Abs(span.Bounds.Width - renderedGrid.Bounds.Width / 2) < 2
            && Math.Abs(span.Bounds.Height - renderedGrid.Bounds.Height * 2 / 3) < 2,
            "The spanning child was not rendered across its Grid cells.");

        Invoke(palette, "Col +");
        await WaitUntilAsync(() => designer.SelectedNode?.Properties.GetValueOrDefault("Grid.Column") == "1"
            && FindSpan() is { } current && Grid.GetColumn(current) == 1, "Grid column move");
        Check(isDirty() && getMarkup().Contains("av:Grid.Column=\"1\"", StringComparison.Ordinal),
            "Moving a Grid child did not update the shared document.");
        Check(!designer.MoveGridColumnLaterCommand.CanExecute(null),
            "The Grid allowed a span to overlap another child.");
        Invoke(palette, "Row +");
        await WaitUntilAsync(() => designer.SelectedNode?.Properties.GetValueOrDefault("Grid.Row") == "1"
            && FindSpan() is { } current && Grid.GetRow(current) == 1, "Grid row move");
        span = FindSpan()!;
        Check(Grid.GetRowSpan(span) == 2 && Grid.GetColumnSpan(span) == 2,
            "Moving a Grid child changed its rendered span.");
        string moved = getMarkup();

        designer.SelectedNode = designer.PreviewRoot!.Children[1];
        AxamlPropertyRowViewModel width = designer.PropertyRows.Single(row => row.Name == "Width");
        Check(!width.TrySetValue("-2"), "The Grid draft fixture did not retain an invalid value.");
        designer.SelectedNode = designer.PreviewRoot.Children[0];
        Check(designer.UnappliedDrafts.Count == 1 && !designer.MoveGridColumnEarlierCommand.CanExecute(null)
            && !designer.RemoveStructureCommand.CanExecute(null) && !designer.UndoCommand.CanExecute(null),
            "A draft for another Grid child did not block structural edits and history.");
        designer.MoveGridColumnEarlierCommand.Execute(null);
        Check(getMarkup() == moved, "A Grid edit changed the document while a draft was pending.");
        designer.DiscardStoredDraft(designer.UnappliedDrafts.Single());

        designer.UndoCommand.Execute(null);
        designer.UndoCommand.Execute(null);
        Check(getMarkup() == original && !isDirty(), "Grid undo did not restore the clean original.");
        designer.RedoCommand.Execute(null);
        designer.RedoCommand.Execute(null);
        Check(getMarkup() == moved && isDirty(), "Grid redo did not restore both moves.");
        designer.SelectedNode = designer.PreviewRoot!.Children[1];
        Invoke(palette, "Remove");
        Check(designer.PreviewRoot!.Children.Count == 1, "Grid child removal did not update the preview.");
        designer.UndoCommand.Execute(null);
        designer.UndoCommand.Execute(null);
        designer.UndoCommand.Execute(null);
        Check(getMarkup() == original && !isDirty(), "Grid removal and move undo did not restore the original.");

        AxamlPreviewNode invalidGrid = new AxamlPreviewService().Parse(
            "<Grid RowDefinitions='-1,NaN' ColumnDefinitions='*'><Button Grid.Row='-1' Grid.RowSpan='0' /></Grid>");
        Check(!invalidGrid.HasSafeGridLayout, "An invalid Grid was offered for editing.");
        _ = new AxamlPreviewControl { PreviewNode = invalidGrid };
    }

    private static async Task VerifySingleContentRemovalAsync(
        AxamlDesignerViewModel designer,
        Control palette,
        Func<string> getMarkup,
        Func<bool> isDirty)
    {
        string original = getMarkup();
        TreeView structure = palette.GetVisualDescendants().OfType<TreeView>()
            .Single(tree => tree.Name == "StructureTree");
        structure.SelectedItem = designer.PreviewRoot!.Children[0];
        await WaitUntilAsync(() => designer.SelectedNode?.TypeName == "Button",
            "single-content child selection");
        Check(designer.RemoveStructureCommand.CanExecute(null),
            "The supported single-content child was not available for removal.");
        Invoke(palette, "Remove");
        await WaitUntilAsync(() => designer.PreviewRoot!.Children.Count == 0,
            "single-content child removal");
        Check(isDirty(), "Removing single content did not update the document buffer.");
        designer.UndoCommand.Execute(null);
        await WaitUntilAsync(() => getMarkup() == original && designer.PreviewRoot!.Children.Count == 1,
            "single-content removal undo");
        Check(!isDirty(), "Undoing single-content removal did not restore the clean document.");
    }

    private static ProjectExplorerItemViewModel FindItem(IEnumerable<ProjectExplorerItemViewModel> items, string path)
    {
        foreach (ProjectExplorerItemViewModel item in items)
        {
            if (WorkspacePathIdentity.AreEqual(item.Item.Path, path))
            {
                return item;
            }
            ProjectExplorerItemViewModel? nested = FindItemOrNull(item.Children, path);
            if (nested is not null)
            {
                return nested;
            }
        }
        throw new InvalidOperationException($"Explorer did not contain '{path}'.");
    }

    private static ProjectExplorerItemViewModel? FindItemOrNull(IEnumerable<ProjectExplorerItemViewModel> items, string path)
    {
        foreach (ProjectExplorerItemViewModel item in items)
        {
            if (WorkspacePathIdentity.AreEqual(item.Item.Path, path))
            {
                return item;
            }
            ProjectExplorerItemViewModel? nested = FindItemOrNull(item.Children, path);
            if (nested is not null)
            {
                return nested;
            }
        }
        return null;
    }

    private static Task WaitForActiveAsync(MainWindowViewModel workbench, string path) =>
        WaitUntilAsync(() => workbench.ActiveEditorTab.FilePath is { } active
            && WorkspacePathIdentity.AreEqual(active, path), $"editor for {Path.GetFileName(path)}");

    private static Button FindButton(Window window, string label) =>
        window.GetVisualDescendants().OfType<Button>()
            .Single(button => string.Equals(button.Content?.ToString(), label, StringComparison.Ordinal));

    private static void Click(Window window, string label) =>
        FindButton(window, label).RaiseEvent(new RoutedEventArgs(Button.ClickEvent));

    private static void Invoke(Control parent, string label)
    {
        Button button = parent.GetVisualDescendants().OfType<Button>()
            .Single(item => string.Equals(item.Content?.ToString(), label, StringComparison.Ordinal));
        Check(button.IsEnabled, $"The '{label}' button is disabled.");
        IInvokeProvider provider = ControlAutomationPeer.CreatePeerForElement(button)
            .GetProvider<IInvokeProvider>() ?? throw new InvalidOperationException($"The '{label}' button has no invoke provider.");
        provider.Invoke();
    }

    private static async Task WaitUntilAsync(Func<bool> condition, string description)
    {
        for (int attempt = 0; attempt < 300; attempt++)
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

    private sealed class BinarySmokeFactory : IDocumentEditorFactory<EditorTabViewModel>
    {
        public string Id => "binary-smoke";
        public int Priority => 1;
        public BinarySmokeSession? LastSession { get; private set; }

        public bool CanOpen(string filePath) => Path.GetExtension(filePath) == ".bin";

        public EditorTabViewModel Create(string filePath)
        {
            LastSession = new BinarySmokeSession();
            return new EditorTabViewModel(
                LastSession,
                new Border { Child = new TextBlock { Text = "Synthetic binary editor" } },
                _ => Task.FromResult<string?>(null),
                Id);
        }
    }

    private sealed class BinarySmokeSession : IDocumentEditorSession
    {
        private byte[] _savedContent = [];
        public string? FilePath { get; private set; }
        public string DisplayName => Path.GetFileName(FilePath) ?? "Untitled.bin";
        public byte[] Content { get; private set; } = [];
        public bool IsDirty => !Content.SequenceEqual(_savedContent);
        public bool IsDisposed { get; private set; }
        public event EventHandler? ContentChanging;
        public event EventHandler? StateChanged;

        public async Task OpenAsync(string filePath, CancellationToken cancellationToken = default)
        {
            Content = await File.ReadAllBytesAsync(filePath, cancellationToken);
            _savedContent = Content.ToArray();
            FilePath = filePath;
            StateChanged?.Invoke(this, EventArgs.Empty);
        }

        public async Task SaveAsync(string filePath, CancellationToken cancellationToken = default)
        {
            byte[] snapshot = Content.ToArray();
            await File.WriteAllBytesAsync(filePath, snapshot, cancellationToken);
            _savedContent = snapshot;
            FilePath = filePath;
            StateChanged?.Invoke(this, EventArgs.Empty);
        }

        public void ReplaceContent(byte[] content)
        {
            ContentChanging?.Invoke(this, EventArgs.Empty);
            Content = content.ToArray();
            StateChanged?.Invoke(this, EventArgs.Empty);
        }

        public void Dispose() => IsDisposed = true;
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
            ServiceCollection services = new();
            App.ConfigureServices(
                services,
                () => desktop.MainWindow,
                userConfigRoot: Path.Combine(s_root, "config"),
                systemConfigRoot: Path.Combine(s_root, "system"));
            services.AddSingleton<IDocumentEditorFactory<EditorTabViewModel>>(new BinarySmokeFactory());
            Services = services.BuildServiceProvider(new ServiceProviderOptions
            {
                ValidateOnBuild = true,
                ValidateScopes = true,
            });
            desktop.MainWindow = App.CreateMainWindow(Services);
            base.OnFrameworkInitializationCompleted();
        }
    }
}
