using BaseLib.Interfaces;

namespace BaseLib.Show;

/// <summary>
/// Orchestrates the interactive lifecycle of the BaseLib showcase application.
/// </summary>
internal sealed class ShowcaseApplication
{
    private readonly IConsole _console;
    private readonly IReadOnlyList<IDemoModule> _demoModules;
    private readonly ShowcaseConsole _showcaseConsole;
    private readonly ShowcaseText _text;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShowcaseApplication"/> class.
    /// </summary>
    /// <param name="console">The console abstraction.</param>
    /// <param name="demoModules">The registered demo modules.</param>
    /// <param name="showcaseConsole">The showcase renderer.</param>
    /// <param name="text">The localized text provider.</param>
    public ShowcaseApplication(
        IConsole console,
        IEnumerable<IDemoModule> demoModules,
        ShowcaseConsole showcaseConsole,
        ShowcaseText text)
    {
        _console = console;
        _demoModules = demoModules.OrderBy(module => module.SelectionKey).ToArray();
        _showcaseConsole = showcaseConsole;
        _text = text;
    }

    /// <summary>
    /// Runs the interactive showcase loop.
    /// </summary>
    public void Run()
    {
        _console.Title = _text.Get("App_Title");
        ShowWelcome();

        bool running = true;
        while (running)
        {
            ShowMainMenu();
            ConsoleKeyInfo? key = _console.ReadKey();
            _showcaseConsole.BlankLine();
            _showcaseConsole.BlankLine();
            running = HandleSelection(key?.KeyChar);
        }

        ShowGoodbye();
    }

    private bool HandleSelection(char? selection)
    {
        if (selection is 'q' or 'Q')
        {
            return false;
        }

        if (selection is '0')
        {
            foreach (IDemoModule demo in _demoModules)
            {
                demo.Run();
            }

            WaitForContinuation();
            return true;
        }

        IDemoModule? demoModule = _demoModules.FirstOrDefault(module => module.SelectionKey == selection);
        if (demoModule is null)
        {
            return true;
        }

        demoModule.Run();
        WaitForContinuation();
        return true;
    }

    private void ShowWelcome()
    {
        _console.ForegroundColor = ConsoleColor.Cyan;
        _console.WriteLine("╔════════════════════════════════════════════════════════════════╗");
        _console.WriteLine("║                                                                ║");
        _console.WriteLine($"║              {_text.Get("Welcome_Title").PadRight(50)}║");
        _console.WriteLine("║                                                                ║");
        _console.WriteLine($"║          {_text.Get("Welcome_Subtitle").PadRight(54)}║");
        _console.WriteLine("║                                                                ║");
        _console.WriteLine("╚════════════════════════════════════════════════════════════════╝");
        _console.ForegroundColor = ConsoleColor.Gray;
        _showcaseConsole.BlankLine();
    }

    private void ShowMainMenu()
    {
        _console.ForegroundColor = ConsoleColor.Yellow;
        _console.WriteLine("══════════════════════════════════════════════════════════════════");
        _console.WriteLine($"                         {_text.Get("Menu_Title")}");
        _console.WriteLine("══════════════════════════════════════════════════════════════════");
        _console.ForegroundColor = ConsoleColor.White;
        _showcaseConsole.BlankLine();

        foreach (IDemoModule demo in _demoModules)
        {
            _console.WriteLine($"  [{demo.SelectionKey}] {_text.Get(demo.TitleKey),-15} - {_text.Get(demo.MenuDescriptionKey)}");
        }

        _showcaseConsole.BlankLine();
        _console.WriteLine($"  [0] {_text.Get("Menu_RunAll")}");
        _console.WriteLine($"  [Q] {_text.Get("Menu_Exit")}");
        _showcaseConsole.BlankLine();
        _console.ForegroundColor = ConsoleColor.Gray;
        _console.Write(_text.Get("Menu_Prompt"));
    }

    private void ShowGoodbye()
    {
        _showcaseConsole.BlankLine();
        _console.ForegroundColor = ConsoleColor.Green;
        _console.WriteLine(_text.Get("Goodbye_Message"));
        _console.ForegroundColor = ConsoleColor.Gray;
    }

    private void WaitForContinuation()
    {
        _showcaseConsole.BlankLine();
        _console.ForegroundColor = ConsoleColor.DarkGray;
        _console.Write(_text.Get("Continue_Prompt"));
        _console.ForegroundColor = ConsoleColor.Gray;
        _console.ReadKey();
        _showcaseConsole.BlankLine();
        _showcaseConsole.Clear();
        ShowWelcome();
    }
}
