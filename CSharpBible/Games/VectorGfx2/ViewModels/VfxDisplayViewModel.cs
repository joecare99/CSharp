using System;
using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using MVVM.ViewModel;
using System.Timers;
using System.Windows;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Media;

namespace VectorGfx2.ViewModels;

public partial class VfxDisplayViewModel : BaseViewModelCT
{
    [ObservableProperty]
    private List<IVisualObject2> _visObjects;

    [ObservableProperty]
    private string _dataText;

    private Timer timer;

    private double dTime;

    [ObservableProperty]
    PointCollection _pnts2 = new();
    public VfxDisplayViewModel()
    {
        VisObjects = new()
        {
            new VisualObject2() {Idx=0, IType=0, P= new Point (50, 50), MouseHover=MouseHoverCommand,
                Pnts=[new(-10,-10), new(10, -10), new(10, 10), new(-10, 10)] },

            new VisualObject2() {Idx=1, IType=1, P=new Point (60, 50), MouseHover=MouseHoverCommand,
               Pnts=[new(-10,-10), new(10, -10), new(0, 10)]},
            new VisualObject2() {Idx=2, IType=2, P=new Point (80, 50), MouseHover=MouseHoverCommand,
               Pnts=[new(-5,-10), new(5, -10), new(10, 0), new(5, 10), new(-5, 10), new(-10, 0)] },
            new VisualObject2() {Idx = 3,  IType=3, P=new Point (150, 50), MouseHover=MouseHoverCommand,
             Pnts=[new(-5,-10), new(5, -10), new(10, 0), new(0, 10), new(-10, 0)]},
            new VisualObject2() {Idx = 4,  IType=4, P=new Point (250, 50), MouseHover=MouseHoverCommand,
             Pnts=[new(-5,-10), new(5, -10), new(10, 0), new(0, 10), new(-10, 0)]},
            new VisualObject2() {Idx = 5,  IType=5, P=new Point (300, 50), MouseHover=MouseHoverCommand,
             Pnts=[new(-5,-10), new(5, -10), new(10, 0), new(0, 10), new(-10, 0)]},
            new VisualObject2() {Idx = 6,  IType=6, P=new Point (340, 50), MouseHover=MouseHoverCommand,
             Pnts=[new(-5,-10), new(5, -10), new(10, 0), new(0, 10), new(-10, 0)]},
            new VisualObject2() {Idx = 7,  IType=7, P=new Point (380, 50), MouseHover=MouseHoverCommand,
             Pnts=[new(-5,-10), new(5, -10), new(10, 0), new(0, 10), new(-10, 0)]},
        };
        dTime = 0;
        timer = new Timer(40);
        timer.Elapsed += Update;
        timer.Start();
        var rnd = new Random();
        for (int i = 0; i < 12; i++)
        {
            var r = rnd.NextDouble(); 
           System.Diagnostics.Debug.WriteLine($"<Point X={'"'}{(60+Math.Sin(i/6*Math.PI)*(50+12*r)):000.00}{'"'} Y={'"'}{60+Math.Cos(i / 6 * Math.PI) * (50 + 12 * r):000.00}{'"'}/>");
        }
    }

    private void Update(object? sender, ElapsedEventArgs e)
    {
        dTime += 0.04;
        var _l = VisObjects.ToList();
        foreach (var vo in _l)
        {
            vo.ZRot = (int)((dTime + Math.Sin((dTime + vo.Idx) * 1.5)) * 50);
            vo.X = (int)vo.P.X;
            vo.Y = (int)vo.P.Y;
        }

        Pnts2.Clear();
            
        Pnts2.Add(new Point(10 + Math.Sin(dTime) * 10, 10 + Math.Cos(dTime) * 10));
        Pnts2.Add(new Point(10 + Math.Sin(dTime+1) * 10, 10 + Math.Cos(dTime+1) * 10));
        Pnts2.Add(new Point(10 + Math.Sin(dTime+2) * 10, 10 + Math.Cos(dTime+2) * 10));


        //VisObjects = _l;
        OnPropertyChanged(nameof(VisObjects));
        OnPropertyChanged(nameof(Pnts2));

    }

    [RelayCommand]
    private void MouseHover(IVisualObject2 vo)
    {
        if (vo is IVisualObject2 obj)
            DataText = $"{GetNameOfType(obj.IType)} at {obj.P}";
    }

    private static string GetNameOfType(int iType)
        => iType switch { 
            0 => "Dot", 1 => "Shot", 2 => "Ship", 3 => "Big Asteroid",
            4 => "Medium Asteroid", 5 => "Small Asteroid", 6 => "Tiny Asteroid",
            _ => "Character" };

}
