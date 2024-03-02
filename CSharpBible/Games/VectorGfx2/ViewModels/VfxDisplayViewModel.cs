using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using MVVM.ViewModel;
using System.Timers;
using System.Windows;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace VectorGfx2.ViewModels;

public partial class VfxDisplayViewModel : BaseViewModelCT
{
    [ObservableProperty]
    private List<IVisualObject2> _visObjects;

    [ObservableProperty]
    private string _dataText;

    private Timer timer;

    private double dTime;
    public VfxDisplayViewModel()
    {
        VisObjects = new()
        {
            new VisualObject2() {Idx=0, IType=0, P= new Point (50, 50), MouseHover=MouseHoverCommand,
                Pnts=[new(-10,-10), new(10, -10), new(10, 10), new(-10, 10)] },
            new VisualObject2() {Idx=1, IType=1, P=new Point (90, 50), MouseHover=MouseHoverCommand,
               Pnts=[new(-10,-10), new(10, -10), new(0, 10)]},
            new VisualObject2() {Idx=2, IType=2, P=new Point (130, 50), MouseHover=MouseHoverCommand,
               Pnts=[new(-5,-10), new(5, -10), new(10, 0), new(5, 10), new(-5, 10), new(-10, 0)] },
            new VisualObject2() {Idx = 1,  IType=3, P=new Point (170, 50), MouseHover=MouseHoverCommand,
             Pnts=[new(-5,-10), new(5, -10), new(10, 0), new(0, 10), new(-10, 0)]},
        };
        dTime = 0;
        timer = new Timer(40);
        timer.Elapsed += Update;
        timer.Start();
    }

    private void Update(object? sender, ElapsedEventArgs e)
    {
        dTime += 0.04;
        var _l = VisObjects.ToList();
        foreach (var vo in _l)
        {
            vo.ZRot = (int)((dTime + Math.Sin((dTime + vo.Idx) * 1.5)) * 50);
        }
        
        //VisObjects = _l;
        OnPropertyChanged(nameof(VisObjects));

    }

    [RelayCommand]
    private void MouseHover(IVisualObject2 vo)
    {
        if (vo is IVisualObject2 obj)
            DataText = $"{GetNameOfType(obj.IType)} at {obj.P}";
    }

    private static string GetNameOfType(int iType)
        => iType switch { 0 => "Rectangle", 1 => "Circle", 2 => "Hexagon", 3 => "Torus", _ => "Shape" };

}
