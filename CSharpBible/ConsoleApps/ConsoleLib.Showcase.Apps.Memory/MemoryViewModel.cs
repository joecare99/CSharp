using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ConsoleLib.Showcase.Apps.Memory;

public partial class MemoryViewModel : ObservableObject
{
    private readonly MemoryGame _game;

    [ObservableProperty]
    private string status;

    public MemoryViewModel(MemoryGame? game = null)
    {
        _game = game ?? new MemoryGame();
        status = StatusText();
    }

    public MemoryGame Game => _game;

    public string Card0Text => CardText(0);
    public string Card1Text => CardText(1);
    public string Card2Text => CardText(2);
    public string Card3Text => CardText(3);
    public string Card4Text => CardText(4);
    public string Card5Text => CardText(5);
    public string Card6Text => CardText(6);
    public string Card7Text => CardText(7);

    public void SelectCard(int index)
    {
        var result = _game.SelectCard(index);
        if (result == MemorySelectionResult.Rejected)
            return;

        NotifyCardsChanged();
        Status = result switch
        {
            MemorySelectionResult.Completed => $"Completed in {_game.Moves} moves!",
            MemorySelectionResult.Match => $"Pair found. Moves: {_game.Moves}",
            MemorySelectionResult.Mismatch => $"No pair. Moves: {_game.Moves}",
            _ => "Choose another card."
        };
    }

    [RelayCommand] private void Select0() => SelectCard(0);
    [RelayCommand] private void Select1() => SelectCard(1);
    [RelayCommand] private void Select2() => SelectCard(2);
    [RelayCommand] private void Select3() => SelectCard(3);
    [RelayCommand] private void Select4() => SelectCard(4);
    [RelayCommand] private void Select5() => SelectCard(5);
    [RelayCommand] private void Select6() => SelectCard(6);
    [RelayCommand] private void Select7() => SelectCard(7);

    [RelayCommand]
    private void Restart()
    {
        _game.Restart();
        NotifyCardsChanged();
        Status = StatusText();
    }

    private string CardText(int index)
    {
        var card = _game.Cards[index];
        return card.IsFaceUp || card.IsMatched ? card.Value : "?";
    }

    private string StatusText() => $"Find {_game.Cards.Count / 2} pairs. Moves: {_game.Moves}";

    private void NotifyCardsChanged()
    {
        for (var index = 0; index < _game.Cards.Count; index++)
            OnPropertyChanged($"Card{index}Text");
    }
}
