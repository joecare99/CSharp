using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Asteroids.Model;
using Asteroids.Model.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using MVVM.ViewModel;

namespace Asteroids.ViewModels;

public partial class AsteroidsViewModel : BaseViewModelCT
{
    [ObservableProperty]
    private List<IScreenObj> screenObjs = new();
    private System.Timers.Timer? timer;

    bool IsInDesignMode => true;//App.Current==null;

    public List<IScreenObj> Bgr { get; } = new();

    public AsteroidsViewModel()
    {
        if (IsInDesignMode)
        {
            System.Diagnostics.Debug.WriteLine("Design mode");
            ScreenObjs = new();
            var rnd = new Random();
            for (int i = 0; i < 200; i++)
                Bgr.Add(new BgrStar(new Point(rnd.NextDouble() * 1600, rnd.NextDouble() * 900)));
            ScreenObjs.AddRange(Bgr);
            for (int i = 2; i < 8; i++)
            {
                for (int j = 0; j < 16; j++)
                {
                    ScreenObjs.Add(new ScreenChar((char)(i * 16 + j), 70, new(10 + j * 50, 1000 - i * 100)));
                }
            }
            timer = new System.Timers.Timer(80);
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
            // Code runs in Blend --> create design time data.
        }
        else
        {
        }
    }

    private void Timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
    {
        for (int i = 0; i < 200; i++)
            Bgr[i].Points[0].X = (Bgr[i].Points[0].X+1599) % 1600;
        ScreenObjs.Clear();
        ScreenObjs.AddRange(Bgr);
        for (int i = 2; i < 8; i++)
        {
            for (int j = 0; j < 16; j++)
            {
                ScreenObjs.Add(new ScreenChar((char)(i * 16 + j), 70, new(10 + j * 50, 1000 - i * 100)));
            }
        }
        OnPropertyChanged(nameof(ScreenObjs));
    }
}
