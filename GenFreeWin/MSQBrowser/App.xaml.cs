using BaseLib.Helper;
using Microsoft.Extensions.DependencyInjection;
using MSQBrowser.Models;
using MSQBrowser.Models.Interfaces;
using MSQBrowser.ViewModels;
using MSQBrowser.ViewModels.Interfaces;
using MSQBrowser.Views;
using MySqlConnector;
using System;
using System.Data.Common;
using System.Windows;

namespace MSQBrowser
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App() : base()
        {
#if NET6_0_OR_GREATER
            DbProviderFactories.RegisterFactory("MySqlConnector", MySqlConnectorFactory.Instance);
#endif
            // Build the DependencyInjection container
            var builder = new ServiceCollection()
                .AddSingleton<IDBModel, DBModel>()
               .AddSingleton<IDBViewViewModel>((s) => DBViewViewModel.This!)
               .AddTransient<IDialogWindow, DBConnect>()
               .AddTransient<ITableViewViewModel, TableViewViewModel>()
               .AddTransient<ISchemaViewViewModel, SchemaViewViewModel>();

            IoC.GetReqSrv = builder.BuildServiceProvider().GetRequiredService;
        }

    }
}
