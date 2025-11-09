using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DetectiveGame.Engine.Game;
using DetectiveGame.Engine.Cards;
using DetectiveGame.Engine.Game.Interfaces;
using System.Collections;

namespace DetectiveGame.ConsoleApp;

/// <summary>
/// ViewModel für das Detektivspiel – kapselt Spiellogik und präsentiert bindbare Daten & Befehle.
/// </summary>
public partial class GameViewModel : ObservableObject, IGameViewModel
{
    private readonly IGameService _service;
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SuggestCommand))]
    [NotifyCanExecuteChangedFor(nameof(AccuseCommand))]
    [NotifyCanExecuteChangedFor(nameof(NextCommand))]
    private GameState? _state;

    public IList Players { get; } = new ObservableCollection<string>();
    public IList History { get; } = new ObservableCollection<string>();

    [ObservableProperty]
    private string _currentTitle = "Detektivspiel";

    public bool CanInteract => _state != null && !_state.Finished;

    public Action DisplayHelp { get ; set; }

    public GameViewModel(IGameService service)
    {
        _service = service;
    }

    [RelayCommand]
    private void Start()
    {
        if (State != null && !State.Finished) return; // bereits laufend
        Players.Clear();
        History.Clear();
        State = ((IGameSetup)_service).CreateNew(new[] { "Alice", "Bob", "Carol" });
        foreach (var p in _state.Players)
            Players.Add(p.Name);
        History.Add("Spiel gestartet");
        UpdateTitle();
    }

    [RelayCommand(CanExecute = nameof(CanInteract))]
    private void Suggest()
    {
        if (_state == null) return;
        var cur = State.Players[_state.CurrentPlayerIndex];
        var person = GameData.Persons.First();
        var weapon = GameData.Weapons.First();
        var room = GameData.Rooms.First();
        var sug = _service.MakeSuggestion(_state, cur.Id, person, weapon, room);
        History.Add($"V: {cur.Name}: {person.Name}/{weapon.Name}/{room.Name} -> {(sug.RefutingPlayerId is int id ? $"von {_state.Players.First(p=>p.Id==id).Name}" : "Keiner")}");
        UpdateTitle();
    }

    [RelayCommand(CanExecute = nameof(CanInteract))]
    private void Accuse()
    {
        if (_state == null) return;
        var cur = State.Players[_state.CurrentPlayerIndex];
        var ok = _service.MakeAccusation(_state, cur.Id, GameData.Persons.First(), GameData.Weapons.First(), GameData.Rooms.First());
        History.Add(ok ? $"{cur.Name} gewinnt" : $"{cur.Name} falsch (inaktiv)");
        UpdateTitle();
    }

    [RelayCommand(CanExecute = nameof(CanInteract))]
    private void Next()
    {
        if (State == null) return;
        _service.NextTurn(State);
        UpdateTitle();
    }

    [RelayCommand]
    private void Help()
    {
        
    }

    private void UpdateTitle()
    {
        if (State == null)
            CurrentTitle = "Detektivspiel";
        else if (State.Finished)
            CurrentTitle = $"Ende – Sieger {_state.WinnerPlayerId}";
        else
            CurrentTitle = $"Zug: {_state.Players[_state.CurrentPlayerIndex].Name}";
    }

}
