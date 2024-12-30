using BlazorWasmDocker.Models;
using System.Collections.Generic;
using System.ComponentModel;

namespace BlazorWasmDocker.ViewModels.Interfaces
{
    public interface IWeatherViewModel: INotifyPropertyChanged, INotifyPropertyChanging
    {
        public IEnumerable<WeatherForecast>? Forecasts { get; }

        void LoadData();
    }
}