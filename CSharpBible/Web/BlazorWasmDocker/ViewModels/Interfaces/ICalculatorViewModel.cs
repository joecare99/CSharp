using CommunityToolkit.Mvvm.Input;
using System.ComponentModel;

namespace BlazorWasmDocker.ViewModels.Interfaces
{
    public interface ICalculatorViewModel : INotifyPropertyChanged
    {
        string Display { get; }
        string OpMode { get; }
        
        bool xEnterMode { get; set; }
        IRelayCommand<string> AddNumberCommand { get; }
		IRelayCommand<string> ClearCommand { get; }
		IRelayCommand CalculateCommand { get; }
		IRelayCommand<string> OperationCommand { get; }
	}
}