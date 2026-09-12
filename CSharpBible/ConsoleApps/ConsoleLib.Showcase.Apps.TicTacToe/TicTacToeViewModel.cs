using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ConsoleLib.Showcase.Apps.TicTacToe;

public partial class TicTacToeViewModel : ObservableObject
{
    private readonly TicTacToeGame _game;

    [ObservableProperty]
    private string status;

    public TicTacToeViewModel(TicTacToeGame? game = null)
    {
        _game = game ?? new TicTacToeGame();
        status = TurnStatus();
    }

    public TicTacToeGame Game => _game;

    public string Cell0Text => CellText(0);
    public string Cell1Text => CellText(1);
    public string Cell2Text => CellText(2);
    public string Cell3Text => CellText(3);
    public string Cell4Text => CellText(4);
    public string Cell5Text => CellText(5);
    public string Cell6Text => CellText(6);
    public string Cell7Text => CellText(7);
    public string Cell8Text => CellText(8);

    public void PlayCell(int index)
    {
        var result = _game.Play(index);
        if (result == TicTacToeMoveResult.Invalid)
            return;

        NotifyBoardChanged();
        Status = result switch
        {
            TicTacToeMoveResult.Won => $"{_game.Winner} wins!",
            TicTacToeMoveResult.Draw => "Draw.",
            _ => TurnStatus()
        };
    }

    [RelayCommand]
    private void Play0() => PlayCell(0);

    [RelayCommand]
    private void Play1() => PlayCell(1);

    [RelayCommand]
    private void Play2() => PlayCell(2);

    [RelayCommand]
    private void Play3() => PlayCell(3);

    [RelayCommand]
    private void Play4() => PlayCell(4);

    [RelayCommand]
    private void Play5() => PlayCell(5);

    [RelayCommand]
    private void Play6() => PlayCell(6);

    [RelayCommand]
    private void Play7() => PlayCell(7);

    [RelayCommand]
    private void Play8() => PlayCell(8);

    [RelayCommand]
    private void Restart()
    {
        _game.Restart();
        NotifyBoardChanged();
        Status = TurnStatus();
    }

    private string CellText(int index) => _game.GetCell(index) switch
    {
        TicTacToePlayer.X => "X",
        TicTacToePlayer.O => "O",
        _ => " "
    };

    private string TurnStatus() => $"{_game.CurrentPlayer}'s turn";

    private void NotifyBoardChanged()
    {
        for (var index = 0; index < 9; index++)
            OnPropertyChanged($"Cell{index}Text");
    }
}
