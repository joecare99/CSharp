// ***********************************************************************
// Assembly         : MVVM_41_Sudoku
// Author           : Mir
// Created          : 08-11-2022
//
// Last Modified By : Mir
// Last Modified On : 08-24-2022
// ***********************************************************************
// <copyright file="App.xaml.cs" company="JC-Soft">
//     Copyright © JC-Soft 2022
// </copyright>
// <summary></summary>
// ***********************************************************************
using BaseLib.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using BaseLib.Helper;
using Sudoku_Base.Models;
using Sudoku_Base.Models.Interfaces;
using System.Windows;
using BaseLib.Models.Interfaces;
using BaseLib.Models;


namespace MVVM_41_Sudoku
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var sc = new ServiceCollection()
                .AddTransient<ISysTime, SysTime>()
                .AddTransient<ILog, SimpleLog>()
                .AddSingleton<ISudokuModel, SudokuModel>();

            var bc = sc.BuildServiceProvider();

            IoC.Configure(bc);
        }
    }
}
namespace MVVM_41_Sudoku.Models { }
namespace MVVM_41_Sudoku.ValueConverter { }
namespace MVVM_41_Sudoku.Services { }
