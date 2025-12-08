using CommunityToolkit.Mvvm.Input;
using System.ComponentModel;

namespace WPF_Hello_World.Models.Interfaces;

public interface IHelloWorldModel : INotifyPropertyChanged
{
    EGreeting Greeting { get; }

    IRelayCommand ClosingCommand { get; }
}
