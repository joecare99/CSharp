using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using BlazorWasmDocker;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using System;
using BlazorWasmDocker.Models;
using BlazorWasmDocker.ViewModels.Interfaces;
using BlazorWasmDocker.ViewModels;
using BlazorWasmDocker.Models.Interfaces;
using LocalStorage.Services;
using BlazorWasmDocker.Services;
using ConsoleDisplay.View;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) })
	.AddScoped<ICalculatorModel,CalculatorModel>()
	.AddTransient<ICalculatorViewModel,CalculatorViewModel>()
	.AddTransient<IWeatherViewModel,WeatherViewModel>()
	.AddTransient<IRConsoleViewModel,RConsoleViewModel>()
	.AddScoped<ILocalStorageService,LocalStorageService>()
	.AddScoped<IConsoleHandler,ConsoleHandler>()
	.AddScoped<IConsole,WebConsole>();
IoC.ServiceProvider = builder.Services.BuildServiceProvider();
await builder.Build().RunAsync();
