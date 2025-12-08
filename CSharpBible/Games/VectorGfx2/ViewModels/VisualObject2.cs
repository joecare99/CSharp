using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MVVM.ViewModel;
using System.Collections.Generic;
using System.Windows;

namespace VectorGfx2.ViewModels;

public partial class VisualObject2 : NotificationObjectCT, IVisualObject2
{
    public VisualObject2()
    {
    }

    public int Idx { get; set; }

    [ObservableProperty]
    private int _iType;

    [ObservableProperty]
    private Point _p;
    [ObservableProperty]
    private int _zRot;
    [ObservableProperty]
    private int _x;
    [ObservableProperty]
    private int _y;
    [ObservableProperty]
    private List<Point> _pnts;

    public IRelayCommand MouseHover { get; set; }
}
