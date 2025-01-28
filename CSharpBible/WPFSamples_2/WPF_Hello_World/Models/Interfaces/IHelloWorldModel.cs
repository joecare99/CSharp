using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WPF_Hello_World.Models.Interfaces;

public interface IHelloWorldModel : INotifyPropertyChanged
{
    EGreeting Greeting { get; }

    IRelayCommand ClosingCommand { get; }
}
