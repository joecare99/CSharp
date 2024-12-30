using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.ComponentModel;
using System.Drawing;

namespace BlazorWasmDocker.ViewModels.Interfaces
{
    public interface IRConsoleViewModel : INotifyPropertyChanged, INotifyPropertyChanging
     {
        Char[] Buffer { get; }
        Color[] fColors { get; }
        Color[] bColors { get; }
        }
}
