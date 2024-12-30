using MdbBrowser.Models;
using MdbBrowser.Models.Interfaces;
using MdbBrowser.ViewModels;
using MdbBrowser.ViewModels.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using MVVM.View.Extension;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace MdbBrowser
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App() : base()
        {
            // Build the DependencyInjection container
            var builder = new ServiceCollection()
                .AddSingleton<IDBModel,DBModel>()
               .AddSingleton<IDBViewViewModel>((s)=>DBViewViewModel.This!)
               .AddTransient<ITableViewViewModel,TableViewViewModel>()
               .AddTransient<ISchemaViewViewModel, SchemaViewViewModel>();

            IoC.GetReqSrv = builder.BuildServiceProvider().GetRequiredService;
        }

    }
}
