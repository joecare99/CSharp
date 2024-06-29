using CommunityToolkit.Mvvm.Input;
using System.ComponentModel;

namespace BlazorWasmDocker.Models.Interfaces
{
    public interface ICalculatorModel : INotifyPropertyChanged
    {
        double Display { get; }
        bool XNeg { get; }         
        ECalcCommand OpMode { get; }
        IRelayCommand<int> AddNumberCommand { get; }
        IRelayCommand<string> ClearCommand { get; }
        IRelayCommand CalculateCommand { get; }
        IRelayCommand<ECalcCommand> OperationCommand { get; }
    }
}