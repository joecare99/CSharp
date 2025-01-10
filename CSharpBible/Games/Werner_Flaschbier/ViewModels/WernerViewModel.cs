using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Werner_Flaschbier_Base.Model;

namespace Werner_Flaschbier_Base.ViewModels;

public partial class WernerViewModel : ObservableObject, IWernerViewModel
{
    public class _TileProxy(IWernerGame game) : IWernerViewModel.ITileProxy
    {
        IWernerGame _game = game;
        public Enum this[Point p] 
            { get => _game[p]; }
    }
    #region
    private IWernerGame _game;
    private bool _halfStep;

    public IWernerViewModel.ITileProxy Tiles { get; }

    public Size size => _game.size;

    public int Level => _game.Level;

    public int Score => _game.Score;

    public int Lives => _game.Lives;

    public int MaxLives => _game.MaxLives;

    public float TimeLeft => _game.TimeLeft;

    public bool HalfStep => _halfStep;
    #endregion

    #region Methods
    public WernerViewModel(IWernerGame game) { 
        _game = game;
        Tiles = new _TileProxy(game);
        _game.visUpdate += (s, e) =>
        {
            _halfStep = e;
            OnPropertyChanged(nameof(Tiles));
        };
        _game.visFullRedraw += (s, e) => OnPropertyChanged(nameof(Level));        
    }

    /// <summary>
    /// Handles the user action.
    /// </summary>
    /// <param name="action">The action.</param>
    public void HandleUserAction(UserAction action)
    {
        switch (action)
        {
            case UserAction.Quit:
                _game.ReqQuit();
                break;
            case UserAction.Help:
                _game.ReqHelp();
                break;
            case UserAction.Restart:
                _game.ReqRestart();
                break;
            case UserAction.Nop:
                break;
            case UserAction.NextLvl:
                _game.NextLvl();
                break;
            case UserAction.PrevLvl:
                _game.PrevLvl();
                break;
            default:
                if ((int)action < typeof(Direction).GetEnumValues().Length)
                {
                    _game.MovePlayer((Direction)action);
                }
                break;
        }
        _game.HandleGameLogic();
    }

    public Point OldPos(Point p)
    {
        return _game.OldPos(p);
    }

    #endregion
}
