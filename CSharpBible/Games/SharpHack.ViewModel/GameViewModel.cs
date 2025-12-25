using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SharpHack.Engine;
using SharpHack.Base.Model;
using System.Collections.ObjectModel;

namespace SharpHack.ViewModel;

public partial class GameViewModel : ObservableObject
{
    private readonly GameSession _session;

    [ObservableProperty]
    private string _playerName;

    [ObservableProperty]
    private int _hP;

    [ObservableProperty]
    private int _maxHP;

    [ObservableProperty]
    private int _level;

    public ObservableCollection<string> Messages { get; } = new();
    public ObservableCollection<Item> Inventory { get; } = new();

    public Map Map => _session.Map;
    public Creature Player => _session.Player;
    public List<Creature> Enemies => _session.Enemies;

    public GameViewModel(GameSession session)
    {
        _session = session;
        _session.OnMessage += OnGameMessage;
        
        // Initialize properties
        PlayerName = _session.Player.Name;
        UpdateStats();
        UpdateInventory();
    }

    private void OnGameMessage(string message)
    {
        Messages.Add(message);
        if (Messages.Count > 5)
        {
            Messages.RemoveAt(0);
        }
    }

    private void UpdateStats()
    {
        HP = _session.Player.HP;
        MaxHP = _session.Player.MaxHP;
        Level = _session.Level;
    }

    private void UpdateInventory()
    {
        Inventory.Clear();
        foreach (var item in _session.Player.Inventory)
        {
            Inventory.Add(item);
        }
    }

    [RelayCommand]
    public void Move(Direction direction)
    {
        _session.MovePlayer(direction);
        UpdateStats();
        UpdateInventory(); // In case we picked up something
        OnPropertyChanged(nameof(Map)); // Notify map update (though map object ref might be same, content changed)
    }

    [RelayCommand]
    public void Wait()
    {
        _session.Update();
        UpdateStats();
    }
}
