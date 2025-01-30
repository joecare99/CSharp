using BaseLib.Models.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MVVM.ViewModel;
using System.Threading;
using System.Timers;
using WPF_Hello_World.Models.Interfaces;

namespace WPF_Hello_World.Models;

public partial class HelloWorldModel : BaseViewModelCT, IHelloWorldModel
{
    private readonly ICyclTimer timer;

    [ObservableProperty]
    private EGreeting _greeting = EGreeting.None;

    public HelloWorldModel(ICyclTimer timer)
    {
        this.timer = timer;
        timer.Start();
        timer.Interval = 3000;
        timer.Elapsed += _Timer_Elapsed;
        timer.Start();
    }

    private void _Timer_Elapsed(object sender, ElapsedEventArgs e)
    {
        (Greeting, timer.Interval) = Greeting switch
        {
            EGreeting.None => (EGreeting.Greeting, 10000),
            EGreeting.Greeting => (EGreeting.NiceDay, 5000),
            _ => (EGreeting.NiceDay, 5000)
        };
        if (Greeting == EGreeting.NiceDay)
            timer.Stop();
    }

    [RelayCommand]
    private void Closing()
    {
        timer.Stop();
        Greeting = EGreeting.Goodbye;
        for (int i = 0; i < 10; i++)
        {
            Thread.Sleep(100);
        }
    }

}
