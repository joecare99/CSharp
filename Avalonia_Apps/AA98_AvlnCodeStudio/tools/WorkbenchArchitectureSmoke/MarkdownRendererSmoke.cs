using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AA98.MarkDown.Host.Models;
using AA98.MarkDown.Host.ViewModels;
using AA98.MarkDown.Host.Views;
using AA98.MarkDown.UI.Controls;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Avalonia.VisualTree;
using AvaloniaEdit;
using ColorTextBlock.Avalonia;
using Markdown.Avalonia;

internal static partial class Program
{
    private static int RunMarkdownRendererSmoke()
    {
        string fixture = Path.Combine(Environment.CurrentDirectory, "MarkdownRenderer-" + Guid.NewGuid().ToString("N") + ".md");
        string secondFixture = Path.Combine(Environment.CurrentDirectory, "MarkdownRenderer-" + Guid.NewGuid().ToString("N") + ".md");
        bool created = false;
        bool secondCreated = false;
        try
        {
            using (FileStream stream = new(fixture, FileMode.CreateNew, FileAccess.Write, FileShare.None))
            {
                created = true;
                using StreamWriter writer = new(stream);
                writer.Write("# Renderer heading\n\nParagraph **strong** text.\n\n```csharp\nint answer = 42;\n```\n");
            }

            using (FileStream stream = new(secondFixture, FileMode.CreateNew, FileAccess.Write, FileShare.None))
            {
                secondCreated = true;
                using StreamWriter writer = new(stream);
                writer.Write("# Second document\n\nSecond paragraph.\n");
            }

            return AA98.MarkDown.Host.Program.BuildAvaloniaApp()
                .AfterSetup(_ => Dispatcher.UIThread.Post(async () => await CheckMarkdownRendererAsync(fixture, secondFixture)))
                .StartWithClassicDesktopLifetime([]);
        }
        finally
        {
            if (created)
            {
                File.Delete(fixture);
            }
            if (secondCreated)
            {
                File.Delete(secondFixture);
            }
        }
    }

    private static async Task CheckMarkdownRendererAsync(string fixture, string secondFixture)
    {
        IClassicDesktopStyleApplicationLifetime desktop =
            Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime
            ?? throw new InvalidOperationException("The Markdown host did not start a desktop lifetime.");
        int exitCode = 0;
        MainWindowViewModel? vm = null;
        try
        {
            if (desktop.MainWindow is not MainWindow window || window.DataContext is not MainWindowViewModel host)
            {
                throw new InvalidOperationException("The Markdown host did not create its main window and view model.");
            }
            vm = host;
            vm.OpenDocument(fixture);
            Check(vm.SelectedDocument?.FilePath == fixture, "Markdown document did not open.");
            MarkdownDocumentTab first = vm.SelectedDocument!;
            string original = first.Text;
            await WaitUntilAsync(() => window.GetVisualDescendants().OfType<MarkdownEditorPreview>().Any(),
                "Markdown editor");
            MarkdownEditorPreview editor = window.GetVisualDescendants().OfType<MarkdownEditorPreview>().Single();
            await WaitUntilAsync(() => editor.GetVisualDescendants().OfType<MarkdownScrollViewer>().Any(),
                "Markdown preview");
            MarkdownScrollViewer preview = editor.GetVisualDescendants().OfType<MarkdownScrollViewer>().Single();
            await WaitUntilAsync(() => preview.GetVisualDescendants().OfType<CTextBlock>()
                .Any(block => block.Text == "Renderer heading" && block.Bounds.Width > 0),
                "rendered Markdown heading");
            CTextBlock heading = preview.GetVisualDescendants().OfType<CTextBlock>()
                .Single(block => block.Text == "Renderer heading");
            CTextBlock paragraph = preview.GetVisualDescendants().OfType<CTextBlock>()
                .Single(block => block.Text == "Paragraph strong text.");
            TextEditor code = preview.GetVisualDescendants().OfType<TextEditor>()
                .Single(control => control.Text?.Contains("int answer = 42;", StringComparison.Ordinal) == true);
            Check(preview.IsVisible && paragraph.Bounds.Width > 0 && code.Bounds.Width > 0,
                "Markdown preview content was not laid out.");
            Check(heading.Classes.Contains("Heading2") && paragraph.Classes.Contains("Paragraph"),
                "Markdown heading or paragraph was not rendered with the expected role.");

            TextEditor source = editor.GetVisualDescendants().OfType<TextEditor>()
                .Single(control => control.Name == "PART_Editor");
            const string addition = "\nLive first paragraph.\n";
            source.Document.Insert(source.Document.TextLength, addition);
            await CheckRenderedAsync(window, "Live first paragraph.", "Second paragraph.");
            Check(first.Text == original + addition && first.IsDirty && File.ReadAllText(fixture) == original,
                "The first unsaved edit did not stay in the dirty buffer.");

            source.Undo();
            await CheckRenderedAsync(window, "Paragraph strong text.", "Live first paragraph.");
            Check(first.Text == original && !first.IsDirty, "Undo did not restore the clean first buffer.");
            source.Redo();
            await CheckRenderedAsync(window, "Live first paragraph.", "Second paragraph.");
            Check(first.Text == original + addition && first.IsDirty, "Redo did not restore the dirty first buffer.");

            vm.OpenDocument(secondFixture);
            Check(vm.SelectedDocument?.FilePath == secondFixture, "Second Markdown document did not open.");
            MarkdownDocumentTab second = vm.SelectedDocument!;
            string secondOriginal = second.Text;
            await CheckRenderedAsync(window, "Second paragraph.", "Live first paragraph.");
            editor = window.GetVisualDescendants().OfType<MarkdownEditorPreview>().Single();
            source = editor.GetVisualDescendants().OfType<TextEditor>().Single(control => control.Name == "PART_Editor");
            const string secondAddition = "\nLive second paragraph.\n";
            source.Document.Insert(source.Document.TextLength, secondAddition);
            await CheckRenderedAsync(window, "Live second paragraph.", "Live first paragraph.");
            Check(second.Text == secondOriginal + secondAddition && second.IsDirty
                && File.ReadAllText(secondFixture) == secondOriginal, "The second unsaved edit was not isolated.");

            vm.SelectedDocument = first;
            await CheckRenderedAsync(window, "Live first paragraph.", "Live second paragraph.");
            Check(first.Text == original + addition && first.IsDirty, "First buffer was lost after switching tabs.");
            vm.SelectedDocument = second;
            await CheckRenderedAsync(window, "Live second paragraph.", "Live first paragraph.");
            Check(second.Text == secondOriginal + secondAddition && second.IsDirty,
                "Second buffer was lost after switching tabs.");
            Check(File.ReadAllText(fixture) == original && File.ReadAllText(secondFixture) == secondOriginal,
                "An unsaved edit changed a fixture on disk.");

            const string external = "# External change\n";
            File.WriteAllText(fixture, external);
            vm.OpenDocument(fixture);
            Check(ReferenceEquals(vm.SelectedDocument, first) && first.Text == original + addition && first.IsDirty,
                "Reopening an already open file replaced its unsaved buffer.");

            Menu menu = window.GetVisualDescendants().OfType<Menu>().Single();
            MenuItem fileMenu = menu.Items.OfType<MenuItem>().Single(item => Equals(item.Header, "_File"));
            MenuItem closeTab = fileMenu.Items.OfType<MenuItem>().Single(item => Equals(item.Header, "Close Tab"));
            closeTab.RaiseEvent(new RoutedEventArgs(MenuItem.ClickEvent));
            await WaitUntilAsync(() => desktop.Windows.OfType<UnsavedChangesDialog>().Any(), "dirty tab close dialog");
            Click(desktop.Windows.OfType<UnsavedChangesDialog>().Single(), "Abbrechen");
            await WaitUntilAsync(() => !desktop.Windows.OfType<UnsavedChangesDialog>().Any(), "cancelled tab close dialog");
            Check(vm.Documents.Contains(first) && ReferenceEquals(vm.SelectedDocument, first)
                && first.IsDirty && first.Text == original + addition && File.ReadAllText(fixture) == external,
                "Cancelling tab close did not preserve the dirty buffer and external file.");

            window.Close();
            await WaitUntilAsync(() => desktop.Windows.OfType<UnsavedChangesDialog>().Any(), "dirty window close dialog");
            Click(desktop.Windows.OfType<UnsavedChangesDialog>().Single(), "Abbrechen");
            await WaitUntilAsync(() => !desktop.Windows.OfType<UnsavedChangesDialog>().Any(), "cancelled window close dialog");
            Check(window.IsVisible && vm.Documents.Contains(first) && vm.Documents.Contains(second)
                && first.IsDirty && first.Text == original + addition && second.IsDirty
                && second.Text == secondOriginal + secondAddition && File.ReadAllText(fixture) == external
                && File.ReadAllText(secondFixture) == secondOriginal,
                "Cancelling window close did not preserve both dirty buffers and their files.");

            vm.SelectedDocument = first;
            vm.SaveCommand.Execute(null);
            Console.WriteLine($"External-change observation: disk equals buffer={File.ReadAllText(fixture) == first.Text}, dirty={first.IsDirty}.");
            await CheckDelayedSavePickerAsync();
            await CheckCloseSavePickerAsync(desktop);
            Console.WriteLine("Markdown host: rendering, dirty buffers, undo/redo, tab switching and close cancellation verified.");
        }
        catch (Exception exception)
        {
            Console.Error.WriteLine(exception);
            exitCode = 1;
        }
        finally
        {
            if (vm is not null)
            {
                foreach (MarkdownDocumentTab document in vm.DirtyDocuments.ToArray())
                {
                    vm.RemoveDocument(document);
                }
            }
            desktop.Shutdown(exitCode);
        }
    }

    private static async Task CheckDelayedSavePickerAsync()
    {
        string target = Path.Combine(Environment.CurrentDirectory, "MarkdownPicker-" + Guid.NewGuid().ToString("N") + ".md");
        string otherTarget = Path.Combine(Environment.CurrentDirectory, "MarkdownPicker-" + Guid.NewGuid().ToString("N") + ".md");
        MainWindowViewModel vm = new();
        MainWindow window = new() { DataContext = vm };
        try
        {
            window.Show();
            MarkdownDocumentTab first = vm.SelectedDocument!;
            first.Text = "first buffer";
            TaskCompletionSource<string?> picker = new();
            window.SavePathPicker = _ => picker.Task;
            Task save = window.SaveAsWithPickerAsync();
            Check(!save.IsCompleted, "The fake save picker was not pending.");
            vm.NewDocumentCommand.Execute(null);
            MarkdownDocumentTab second = vm.SelectedDocument!;
            second.Text = "second buffer";
            string secondInput = vm.DocumentPathInput;
            picker.SetResult(target);
            await save;
            Check(File.Exists(target) && File.ReadAllText(target) == "first buffer",
                "The pending Save As did not save its original tab.");
            Check(first.FilePath == target && !first.IsDirty && second.FilePath is null && second.IsDirty
                && vm.DocumentPathInput == secondInput
                && ReferenceEquals(vm.SelectedDocument, second), "The pending Save As changed the wrong tab.");

            picker = new();
            Task unchanged = window.SaveAsWithPickerAsync();
            picker.SetResult(otherTarget);
            await unchanged;
            Check(File.ReadAllText(otherTarget) == "second buffer" && second.FilePath == otherTarget && !second.IsDirty,
                "Save As with unchanged context failed.");

            first.Text = "changed first";
            vm.SelectedDocument = first;
            string priorInput = vm.DocumentPathInput;
            string priorStatus = vm.StatusText;
            picker = new();
            Task cancelled = window.SaveAsWithPickerAsync();
            picker.SetResult(null);
            await cancelled;
            Check(first.FilePath == target && first.Text == "changed first" && first.IsDirty
                && vm.DocumentPathInput == priorInput && vm.StatusText == priorStatus
                && File.ReadAllText(target) == "first buffer", "Cancelling Save As changed state.");

            async Task CheckInvalidatedAsync(Action mutate)
            {
                picker = new();
                Task pending = window.SaveAsWithPickerAsync();
                Check(!pending.IsCompleted, "The fake save picker was not pending.");
                mutate();
                picker.SetResult(otherTarget);
                await pending;
                Check(File.ReadAllText(otherTarget) == "second buffer" && first.FilePath == target
                    && first.Text == "changed first" && first.IsDirty,
                    "A stale picker wrote or changed the original document.");
            }

            await CheckInvalidatedAsync(() =>
            {
                first.Text = "temporary edit";
                first.Text = "changed first";
            });
            await CheckInvalidatedAsync(() =>
            {
                first.FilePath = otherTarget;
                first.FilePath = target;
            });
            await CheckInvalidatedAsync(() =>
            {
                int index = vm.Documents.IndexOf(first);
                vm.Documents.RemoveAt(index);
                vm.Documents.Insert(index, first);
                vm.SelectedDocument = first;
            });
            await CheckInvalidatedAsync(() =>
            {
                int index = vm.Documents.IndexOf(first);
                vm.Documents[index] = new MarkdownDocumentTab("replacement.md", "replacement", null);
                vm.Documents[index] = first;
                vm.SelectedDocument = first;
            });
            await CheckInvalidatedAsync(() =>
            {
                window.DataContext = new MainWindowViewModel();
                window.DataContext = vm;
                vm.SelectedDocument = first;
            });

            first.MarkSaved();
            picker = new();
            Task afterClose = window.SaveAsWithPickerAsync();
            window.Close();
            picker.SetResult(otherTarget);
            await afterClose;
            Check(!window.IsVisible && first.FilePath == target && File.ReadAllText(otherTarget) == "second buffer",
                "A picker wrote after its window closed.");
            Console.WriteLine("Markdown Save As: delayed tab switch, unchanged context, cancel, mutations, replacement and window closure verified with fake picker.");
        }
        finally
        {
            window.SavePathPicker = null;
            foreach (MarkdownDocumentTab document in vm.DirtyDocuments.ToArray())
            {
                vm.RemoveDocument(document);
            }
            window.Close();
            if (File.Exists(target))
            {
                File.Delete(target);
            }
            if (File.Exists(otherTarget))
            {
                File.Delete(otherTarget);
            }
        }
    }

    private static async Task CheckCloseSavePickerAsync(IClassicDesktopStyleApplicationLifetime desktop)
    {
        string target = Path.Combine(Environment.CurrentDirectory, "MarkdownPickerClose-" + Guid.NewGuid().ToString("N") + ".md");
        MainWindowViewModel vm = new();
        MainWindow window = new() { DataContext = vm };
        try
        {
            window.Show();
            MarkdownDocumentTab first = vm.SelectedDocument!;
            first.Text = "close first";
            vm.NewDocumentCommand.Execute(null);
            MarkdownDocumentTab second = vm.SelectedDocument!;
            second.Text = "close second";
            vm.SelectedDocument = first;
            TaskCompletionSource<string?> picker = new();
            bool pickerStarted = false;
            window.SavePathPicker = _ =>
            {
                pickerStarted = true;
                return picker.Task;
            };
            MenuItem fileMenu = window.GetVisualDescendants().OfType<Menu>().Single().Items.OfType<MenuItem>()
                .Single(item => Equals(item.Header, "_File"));
            MenuItem closeTab = fileMenu.Items.OfType<MenuItem>().Single(item => Equals(item.Header, "Close Tab"));
            closeTab.RaiseEvent(new RoutedEventArgs(MenuItem.ClickEvent));
            await WaitUntilAsync(() => desktop.Windows.OfType<UnsavedChangesDialog>().Any(), "tab cancel prompt");
            Click(desktop.Windows.OfType<UnsavedChangesDialog>().Single(), "Speichern");
            await WaitUntilAsync(() => pickerStarted, "delayed close-tab cancel picker");
            picker.SetResult(null);
            await WaitUntilAsync(() => vm.StatusText == "Speichern wurde abgebrochen.", "cancelled tab picker");
            Check(vm.Documents.Contains(first) && first.IsDirty && first.FilePath is null && !File.Exists(target),
                "Picker cancellation did not prevent tab closure.");

            pickerStarted = false;
            picker = new();
            closeTab.RaiseEvent(new RoutedEventArgs(MenuItem.ClickEvent));
            await WaitUntilAsync(() => desktop.Windows.OfType<UnsavedChangesDialog>().Any(), "tab save prompt");
            Click(desktop.Windows.OfType<UnsavedChangesDialog>().Single(), "Speichern");
            await WaitUntilAsync(() => pickerStarted, "delayed close-tab picker");
            vm.SelectedDocument = second;
            picker.SetResult(target);
            await WaitUntilAsync(() => !vm.Documents.Contains(first), "saved tab closure");
            Check(File.ReadAllText(target) == "close first" && second.IsDirty && second.FilePath is null,
                "Close-tab Save As saved or closed the wrong document.");

            pickerStarted = false;
            picker = new();
            window.Close();
            await WaitUntilAsync(() => desktop.Windows.OfType<UnsavedChangesDialog>().Any(), "window save prompt");
            Click(desktop.Windows.OfType<UnsavedChangesDialog>().Single(), "Speichern");
            await WaitUntilAsync(() => pickerStarted, "delayed window-close picker");
            window.Close();
            Check(window.IsVisible, "A second close bypassed the active save prompt.");
            picker.SetResult(null);
            await WaitUntilAsync(() => vm.StatusText == "Speichern wurde abgebrochen.", "cancelled close picker");
            Check(window.IsVisible && vm.Documents.Contains(second) && second.IsDirty && second.FilePath is null
                && File.ReadAllText(target) == "close first", "Picker cancellation did not prevent window closure.");
            Console.WriteLine("Markdown close flows: delayed tab save, repeated close and picker cancellation verified with fake picker.");
        }
        finally
        {
            window.SavePathPicker = null;
            foreach (MarkdownDocumentTab document in vm.DirtyDocuments.ToArray())
            {
                vm.RemoveDocument(document);
            }
            window.Close();
            if (File.Exists(target))
            {
                File.Delete(target);
            }
        }
    }

    private static async Task CheckRenderedAsync(MainWindow window, string expected, string absent)
    {
        await WaitUntilAsync(() =>
        {
            MarkdownScrollViewer? preview = window.GetVisualDescendants().OfType<MarkdownScrollViewer>().SingleOrDefault();
            return preview is not null && preview.IsVisible
                && preview.GetVisualDescendants().OfType<CTextBlock>()
                    .Any(block => block.Text == expected && block.Bounds.Width > 0)
                && !preview.GetVisualDescendants().OfType<CTextBlock>().Any(block => block.Text == absent);
        }, $"rendered paragraph: {expected}");
    }
}
