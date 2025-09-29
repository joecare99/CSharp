using BaseLib.Interfaces;
using ConsoleLib; // Application, ConsoleProxy, ExtendedConsole
using ConsoleLib.CommonControls;
using ConsoleLib.Interfaces;
using DetectiveGame.ConsoleApp;
using DetectiveGame.Engine.Game;
using DetectiveGame.Engine.Cards;

namespace DetectiveGame.Console.Views;

public class GameView
{
    private readonly IGameViewModel _vm;
    private readonly IConsole _console;
    private readonly IApplication _app;

    public GameView(IGameViewModel vm, IConsole console, IApplication app)
    {
        _vm = vm;
        _console = console;
        _app = app;
        _app.Dimension = new System.Drawing.Rectangle(0, 0, _console.WindowWidth, 50);
        _app.BackColor = ConsoleColor.Black;
        _app.ForeColor = ConsoleColor.Gray;

        Initialize();
    }

    private void Initialize()
    {
        string[] instructions =
        {
            "Anleitung:",
            "Start: Neues Spiel mit den Spielern.",
            "Verdacht: Prüft Widerlegung.",
            "Anklage: Finale Vermutung.",
            "Next: Nächster Spieler.",
            "Ziel: Kombination finden.",
            "? erneut für Hilfe."};

        _vm.DisplayHelp += () =>
        {
            foreach (var l in instructions)
                _vm.History.Add(l);
        };

        var playerList = new ListBox
        {
            Position = new System.Drawing.Point(0, 1),
            size = new System.Drawing.Size(20, 10),
            ItemsSource = _vm.Players,
            Text = "Spieler"
        };
        var historyList = new ListBox
        {
            Position = new System.Drawing.Point(22, 1),
            size = new System.Drawing.Size(58, 20),
            ItemsSource = _vm.History,
            Text = "History"
        };

        var btnStart = new Button();
        btnStart.Set(0, 0, "Start", ConsoleColor.DarkBlue);
        btnStart.Command = _vm.StartCommand;

        var btnSuggest = new Button();
        btnSuggest.Set(8, 0, "Verdacht", ConsoleColor.DarkGreen);
        btnSuggest.OnClick += (_, _) => ShowSuggestionDialog();

        var btnAccuse = new Button();
        btnAccuse.Set(20, 0, "Anklage", ConsoleColor.DarkRed);
        btnAccuse.Command = _vm.AccuseCommand;

        var btnNext = new Button();
        btnNext.Set(30, 0, "Next", ConsoleColor.DarkCyan);
        btnNext.Command = _vm.NextCommand;

        var btnHelp = new Button();
        btnHelp.Set(36, 0, "?", ConsoleColor.DarkYellow);
        btnHelp.Command = _vm.HelpCommand;
        
        _vm.DisplayHelp();

        _app
            .Add(btnStart)
            .Add(btnSuggest)
            .Add(btnAccuse)
            .Add(btnNext)
            .Add(btnHelp)
            .Add(playerList)
            .Add(historyList);

        _vm.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(GameViewModel.CurrentTitle))
                _console.Title = _vm.CurrentTitle;
        };
        _console.Title = _vm.CurrentTitle;
    }

    private void ShowSuggestionDialog()
    {
        if (!_vm.SuggestCommand.CanExecute(null)) return;
        var dlg = new SuggestionDialog();
        // zentrieren oberes Drittel
        var x = (_app.size.Width - dlg.size.Width) / 2;
        dlg.Position = new System.Drawing.Point(x, 1);
        dlg.Closed += () =>
        {
            if (dlg.Accepted && dlg.SelectedPerson!=null && dlg.SelectedWeapon!=null && dlg.SelectedRoom!=null)
            {
                // Führe SuggestCommand mit Parametern aus => Erweiterung nötig? Temporär direkt Service im VM erweitern wäre besser
                // Interim: direkten History-Eintrag erzeugen über VM StartCommand Logik simulieren
                // Besser: IGameViewModel erweitern – hier nur Trigger
                _vm.History.Add($"Verdacht gewählt: {dlg.SelectedPerson.Name}/{dlg.SelectedWeapon.Name}/{dlg.SelectedRoom.Name}");
                // Falls VM später Parameter akzeptiert: _vm.Suggest(dlg.SelectedPerson,...)
            }
        };
        _app.Add(dlg);
        (dlg as IPopup).Show();
        dlg.Invalidate();
    }

    public void Run()
    {
        _app.Initialize();
        _app.Run();
    }
}
