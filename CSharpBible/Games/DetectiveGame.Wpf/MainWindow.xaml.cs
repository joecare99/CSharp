using System.Linq;
using System.Windows;
using DetectiveGame.Engine.Game;
using DetectiveGame.Engine.Cards;

namespace DetectiveGame.Wpf;

public partial class MainWindow : Window
{
    private readonly GameService _svc = new();
    private GameState? _state;

    public MainWindow()
    {
        InitializeComponent();
    }

    private void Start_Click(object sender, RoutedEventArgs e)
    {
        var names = new[] { Player1.Text, Player2.Text, Player3.Text }.Where(s=>!string.IsNullOrWhiteSpace(s)).ToList();
        _state = _svc.CreateNew(names);
        Log.Items.Add("Spiel gestartet");
        ShowTurn();
    }

    private void Suggestion_Click(object sender, RoutedEventArgs e)
    {
        if (_state is null) return;
        var cur = _state.Players[_state.CurrentPlayerIndex];
        var person = GameData.Persons.First();
        var weapon = GameData.Weapons.First();
        var room = GameData.Rooms.First();
        var sug = _svc.MakeSuggestion(_state, cur.Id, person, weapon, room);
        Log.Items.Add($"Verdacht von {cur.Name}: {person.Name}/{weapon.Name}/{room.Name} -> {(sug.RefutingPlayerId is int id ? $"Widerlegt von {id}" : "Keiner")} ");
    }

    private void Accusation_Click(object sender, RoutedEventArgs e)
    {
        if (_state is null) return;
        var cur = _state.Players[_state.CurrentPlayerIndex];
        var ok = _svc.MakeAccusation(_state, cur.Id, GameData.Persons.First(), GameData.Weapons.First(), GameData.Rooms.First());
        Log.Items.Add(ok ? $"{cur.Name} gewinnt" : $"{cur.Name} falsch – inaktiv");
    }

    private void Next_Click(object sender, RoutedEventArgs e)
    {
        if (_state is null) return;
        _svc.NextTurn(_state);
        ShowTurn();
    }

    private void ShowTurn()
    {
        if (_state is null) return;
        var cur = _state.Players[_state.CurrentPlayerIndex];
        Title = _state.Finished ? $"Ende. Sieger {_state.WinnerPlayerId}" : $"Zug: {cur.Name}";
    }
}