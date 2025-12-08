using System.ComponentModel;
using System.Drawing;

namespace Werner_Flaschbier_Base.ViewModels;

public interface IWernerViewModel : INotifyPropertyChanged, INotifyPropertyChanging
{
    public interface ITileProxy
    {
        System.Enum this[Point p] { get; }
    }
    ITileProxy Tiles { get; }
    Size size { get; }
    int Level { get; }
    int Score { get; }
    int Lives { get; }
    int MaxLives { get; }
    float TimeLeft { get; }
    bool HalfStep { get; }

    void HandleUserAction(UserAction action);
    Point OldPos(Point p);
}