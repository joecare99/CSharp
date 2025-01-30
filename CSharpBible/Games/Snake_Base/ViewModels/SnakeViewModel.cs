using System;
using System.Drawing;
using CommunityToolkit.Mvvm.ComponentModel;
using Snake_Base.Models;
using Snake_Base.Models.Data;
using Snake_Base.Models.Interfaces;

namespace Snake_Base.ViewModels;

public partial class SnakeViewModel : ObservableObject, ISnakeViewModel
{
    ISnakeGame _game;

    [ObservableProperty]
    private UserAction _userAction;
    private bool _halfStep;

    public SnakeViewModel(ISnakeGame game)
    {
        _game = game;
        Tiles = new _TileProxy(game);
        _game.visUpdate += (s, e) =>
        {
            _halfStep = e;
            OnPropertyChanged(nameof(Tiles));
        };
        _game.visFullRedraw += (s, e) => OnPropertyChanged(nameof(Level));
    }

    public class _TileProxy(ISnakeGame game) : ISnakeViewModel.ITileProxy<SnakeTiles>
    {
        ISnakeGame _game = game;
        public SnakeTiles this[Point p]
        { get => _game[p]; }
    }

    public ISnakeViewModel.ITileProxy<SnakeTiles> Tiles { get; }

    public Size size => _game.size;
    public int Level => _game.Level;
    public int Score => _game.Score;
    public int Lives => _game.Lives;
    public int MaxLives => _game.MaxLives;

    public Func<Point, Point> GetOldPos { get => _game.GetOldPos; }

    public bool HalfStep => _halfStep;

    partial void OnUserActionChanged(UserAction value)
    {
        if ((int)value < typeof(Direction).GetEnumValues().Length)
            _game.SetSnakeDir((Direction)value);
        else if (value <= UserAction.Quit)
            _game.UserQuit = true;
    }

}
