using System;
using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using MVVM.ViewModel;
using System.Timers;
using System.Windows;
using CommunityToolkit.Mvvm.Input;

namespace VectorGfx.ViewModels;

public partial class VfxDisplayViewModel : BaseViewModelCT
{
    [ObservableProperty]
    private List<IVisualObject> _visObjects;

    [ObservableProperty]
    private string _dataText;

    private Timer timer;

    private double dTime;
    public VfxDisplayViewModel()
    {
        VisObjects = new()
        {
            new VisualObject() {Idx=0, IType=0, P= new Point (50, 50), MouseHover=MouseHoverCommand },
            new VisualObject() {Idx=3, IType=1, P=new Point (90, 50), MouseHover=MouseHoverCommand },
            new VisualObject() {Idx=2, IType=2, P=new Point (130, 50), MouseHover=MouseHoverCommand },
            new VisualObject() {Idx = 1,  IType=3, P=new Point (170, 50), MouseHover=MouseHoverCommand },
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
            var _zz = vo.Z;
            vo.Z = (int)(10 + Math.Sin((dTime * 3 + vo.Idx * 2) * 40 / 180 * Math.PI) * 9);
            vo.X = (int)(vo.P.X - vo.Z / 2);
            vo.Y = (int)(vo.P.Y - vo.Z / 2);
        }

        _l.Sort((x, y) => x.Z == y.Z ? 0 : x.Z < y.Z ? -1 : 1);
        
        //VisObjects = _l;
        OnPropertyChanged(nameof(VisObjects));

    }

    [RelayCommand]
    private void MouseHover(IVisualObject vo)
    {
        if (vo is IVisualObject obj)
            DataText = $"{GetNameOfType(obj.IType)} at {obj.P}";
    }

    private static string GetNameOfType(int iType)
        => iType switch { 0 => "Rectangle", 1 => "Circle", 2 => "Hexagon", 3 => "Torus", _ => "Shape" };

}
