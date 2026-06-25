namespace BaseLib.Show.Demos;

/// <summary>
/// Demonstrates the console abstraction used by BaseLib.
/// </summary>
internal sealed class ConsoleProxyDemo : ShowcaseDemoBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ConsoleProxyDemo"/> class.
    /// </summary>
    /// <param name="showcaseConsole">The showcase console renderer.</param>
    /// <param name="text">The localized text provider.</param>
    public ConsoleProxyDemo(ShowcaseConsole showcaseConsole, ShowcaseText text)
        : base(showcaseConsole, text)
    {
    }

    /// <inheritdoc/>
    public override char SelectionKey => '7';

    /// <inheritdoc/>
    public override string TitleKey => "Demo_Console_Title";

    /// <inheritdoc/>
    public override string MenuDescriptionKey => "Demo_Console_Description";

    /// <inheritdoc/>
    protected override IEnumerable<DemoExample> CreateExamples()
    {
        yield return Example(
            "Example_Console_Properties",
            """
            int width = console.WindowWidth;
            int height = console.WindowHeight;
            string title = console.Title;
            bool redirected = console.IsOutputRedirected;
            int largestHeight = console.LargestWindowHeight;
            """,
            () =>
            {
                ShowcaseConsole.PrintResult(Text.Get("Common_WindowWidth"), RawConsole.WindowWidth);
                ShowcaseConsole.PrintResult(Text.Get("Common_WindowHeight"), RawConsole.WindowHeight);
                ShowcaseConsole.PrintResult(Text.Get("Common_Title"), RawConsole.Title);
                ShowcaseConsole.PrintResult(Text.Get("Common_IsOutputRedirected"), RawConsole.IsOutputRedirected);
                ShowcaseConsole.PrintResult(Text.Get("Common_LargestWindowHeight"), RawConsole.LargestWindowHeight);
            });

        yield return Example(
            "Example_Console_Cursor",
            """
            (int Left, int Top) position = console.GetCursorPosition();
            """,
            () =>
            {
                (int Left, int Top) position = RawConsole.GetCursorPosition();
                ShowcaseConsole.PrintResult(Text.Get("Common_CurrentPosition"), $"Left={position.Left}, Top={position.Top}");
            });

        yield return Example(
            "Example_Console_Colors",
            """
            ConsoleColor[] colors = [ConsoleColor.Red, ConsoleColor.Green, ConsoleColor.Blue, ConsoleColor.Yellow, ConsoleColor.Magenta, ConsoleColor.Cyan];
            foreach (ConsoleColor color in colors)
            {
                console.ForegroundColor = color;
                console.Write("■ ");
            }
            console.ForegroundColor = ConsoleColor.Gray;
            """,
            () =>
            {
                ConsoleColor[] colors = [ConsoleColor.Red, ConsoleColor.Green, ConsoleColor.Blue, ConsoleColor.Yellow, ConsoleColor.Magenta, ConsoleColor.Cyan];
                ShowcaseConsole.Write($"    {Text.Get("Console_PaletteLabel")}: ");
                foreach (ConsoleColor color in colors)
                {
                    RawConsole.ForegroundColor = color;
                    ShowcaseConsole.Write("■ ");
                }

                RawConsole.ForegroundColor = ConsoleColor.Gray;
                ShowcaseConsole.BlankLine();
            });

        yield return Example(
            "Example_Console_Purpose",
            """
            // ConsoleProxy makes console interaction testable and injectable.
            """,
            () =>
            {
                ShowcaseConsole.WriteLine($"    {Text.Get("Console_PurposeIntro")}");
                ShowcaseConsole.WriteLine($"    • {Text.Get("Console_PurposeTesting")}");
                ShowcaseConsole.WriteLine($"    • {Text.Get("Console_PurposeDi")}");
                ShowcaseConsole.WriteLine($"    • {Text.Get("Console_PurposeMockable")}");
            });
    }
}
