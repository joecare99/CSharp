using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
        bool created = false;
        try
        {
            using (FileStream stream = new(fixture, FileMode.CreateNew, FileAccess.Write, FileShare.None))
            {
                created = true;
                using StreamWriter writer = new(stream);
                writer.Write("# Renderer heading\n\nParagraph **strong** text.\n\n```csharp\nint answer = 42;\n```\n");
            }

            return AA98.MarkDown.Host.Program.BuildAvaloniaApp()
                .AfterSetup(_ => Dispatcher.UIThread.Post(async () => await CheckMarkdownRendererAsync(fixture)))
                .StartWithClassicDesktopLifetime([]);
        }
        finally
        {
            if (created)
            {
                File.Delete(fixture);
            }
        }
    }

    private static async Task CheckMarkdownRendererAsync(string fixture)
    {
        IClassicDesktopStyleApplicationLifetime desktop =
            Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime
            ?? throw new InvalidOperationException("The Markdown host did not start a desktop lifetime.");
        int exitCode = 0;
        try
        {
            if (desktop.MainWindow is not MainWindow window || window.DataContext is not MainWindowViewModel vm)
            {
                throw new InvalidOperationException("The Markdown host did not create its main window and view model.");
            }
            vm.OpenDocument(fixture);
            Check(vm.SelectedDocument?.FilePath == fixture, "Markdown document did not open.");
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
            Console.WriteLine("Markdown host: rendered heading, paragraph and C# code block verified.");
        }
        catch (Exception exception)
        {
            Console.Error.WriteLine(exception);
            exitCode = 1;
        }
        finally
        {
            desktop.Shutdown(exitCode);
        }
    }
}
