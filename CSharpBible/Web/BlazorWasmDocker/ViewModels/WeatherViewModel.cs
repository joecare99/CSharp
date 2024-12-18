using BlazorWasmDocker.Models;
using BlazorWasmDocker.ViewModels.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using System.Net.Http;
using System.Net.Http.Json;

namespace BlazorWasmDocker.ViewModels
{
    public partial class WeatherViewModel : ObservableObject , IWeatherViewModel
	{
        [ObservableProperty]
        public IEnumerable<WeatherForecast>? forecasts;

        private HttpClient _http { get; set; }

        public WeatherViewModel(HttpClient http) { 
            _http = http;
        }

        async public void LoadData()
        {
            Forecasts = await _http.GetFromJsonAsync<WeatherForecast[]>("sample-data/weather.json");
        }
    }
}
