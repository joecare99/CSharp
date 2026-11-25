using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AA98.MarkDown.Host.Models;
using AA98.MarkDown.Host.ViewModels;
using AA98.MarkDown.Host.Views;
using AA98.MarkDown.UI.Controls;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
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
            Console.WriteLine("Markdown host: rendering, live unsaved edits, undo/redo and two-tab switching verified.");
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
