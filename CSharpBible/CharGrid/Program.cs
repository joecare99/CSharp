// ***********************************************************************
// Assembly : CharGrid
// Author : Mir
// Created :12-19-2021
//
// Last Modified By : GitHub Copilot
// Last Modified On :2025-11-05
// ***********************************************************************
using System;
using System.Windows.Forms;
using CSharpBible.CharGrid.Views;
using CSharpBible.CharGrid.Services;
using CSharpBible.CharGrid.ViewModels.Interfaces;
using CSharpBible.CharGrid.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace CSharpBible.CharGrid
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var services = new ServiceCollection()
            .AddSingleton<IRandomCharService, RandomCharService>()
            .AddSingleton<ICharGridProvider, InMemoryCharGridProvider>()
            .AddTransient<ICharGridViewModel, CharGridViewModel>()
            .AddTransient<FrmCharGridMain>()
            .BuildServiceProvider();

            Application.Run(services.GetRequiredService<FrmCharGridMain>());
        }
    }
}
