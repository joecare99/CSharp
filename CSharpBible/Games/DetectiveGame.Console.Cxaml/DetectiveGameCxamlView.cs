using BaseLib.Interfaces;
using ConsoleLib;
using ConsoleLib.CommonControls;
using ConsoleLib.Interfaces;

namespace DetectiveGame.Console.Cxaml;

/// <summary>Applies application-specific behavior around the declarative detective-game layout.</summary>
internal sealed class DetectiveGameCxamlView
{
    private static readonly string[] HelpLines =
    [
        "Anleitung:",
        "Start: Neues Spiel mit den Spielern.",
        "Verdacht: Prüft Widerlegung.",
        "Anklage: Finale Vermutung.",
        "Next: Nächster Spieler.",
        "Ziel: Kombination finden.",
        "? erneut für Hilfe.",
    ];

    private readonly IApplication? _application;
    private readonly IConsole? _console;
    private readonly IGameViewModel _viewModel;

    public DetectiveGameCxamlView(
        IGameViewModel viewModel,
        IConsole? console,
        IApplication? application)
    {
        _viewModel = viewModel;
        _console = console;
        _application = application;
    }

    public CxamlLoadResult Load()
    {
        using Stream stream = typeof(DetectiveGameCxamlView).Assembly.GetManifestResourceStream(
            "DetectiveGame.Console.Cxaml.Views.Main.cxaml")
            ?? throw new InvalidOperationException("The detective-game CXAML view resource is missing.");
        using StreamReader reader = new(stream);
        CxamlLoadResult result = new CxamlLoader().Load(reader, new CxamlLoadContext(_viewModel));

        Button close = GetControl<Button>(result, "Close");
        if (_application is not null)
        {
            close.OnClick += (_, _) => _application.Stop();
        }

        _viewModel.DisplayHelp = DisplayHelp;
        DisplayHelp();
        _viewModel.PropertyChanged += OnViewModelPropertyChanged;
        UpdateTitle();
        return result;
    }

    private void DisplayHelp()
    {
        foreach (string line in HelpLines)
        {
            _viewModel.History.Add(line);
        }
    }

    private void OnViewModelPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs eventArgs)
    {
        if (eventArgs.PropertyName == nameof(IGameViewModel.CurrentTitle))
        {
            UpdateTitle();
        }
    }

    private void UpdateTitle()
    {
        if (_console is not null)
        {
            _console.Title = _viewModel.CurrentTitle;
        }
    }

    private static TControl GetControl<TControl>(CxamlLoadResult result, string name)
        where TControl : class, IControl
        => result.NamedControls.TryGetValue(name, out IControl? control) && control is TControl typedControl
            ? typedControl
            : throw new CxamlParseException($"The detective-game CXAML view is missing '{name}'.");
}
