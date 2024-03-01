using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MVVM.ViewModel;
using System.Windows;

namespace VectorGfx2.ViewModels;

public partial class VisualObject : NotificationObjectCT, IVisualObject
{
    public VisualObject()
    {
    }

    public int Idx { get ; set ; }

    [ObservableProperty]
    private int _iType;

    [ObservableProperty]
    private Point _p;
    [ObservableProperty]
    private int _z;
    [ObservableProperty]
    private int _zRot;
    [ObservableProperty]
    private int _x;
    [ObservableProperty]
    private int _y;

    public IRelayCommand MouseHover { get; set; }
}
