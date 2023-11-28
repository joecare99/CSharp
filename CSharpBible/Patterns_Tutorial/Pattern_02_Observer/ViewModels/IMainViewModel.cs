using BaseLib.Interfaces;
using Pattern_02_Observer.Models;
using System;

namespace Pattern_02_Observer.ViewModels
{
    public interface IMainViewModel
    {
        string Greeting { get; }
        TimeSpan RunTime { get; }

        event PropertyChangedAdvEventHandler? PropertyChangedAdv;

        void SetGreeting(EGreetings eGreetings);
    }
}