using BaseLib.Helper;
using BaseLib.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using VTileEdit.ViewModels;
using VTileEdit.Views;
using VTileEdit.Models;

namespace VTileEdit
{
    /// <summary>
    /// Class Program.
    /// </summary>
    public class Program
    {
        private ServiceProvider _sp;

        private void OnStartUp()
        {
            var sc = new ServiceCollection()
                .AddSingleton<IVTEModel, VTEModel>()
                .AddSingleton<IVTEViewModel, VTEViewModel>()
                .AddSingleton<IVisual, VTEVisual>();
            _sp = sc.BuildServiceProvider();
        }

        /// <summary>Defines the entry point of the application.</summary>
        /// <param name="args">The arguments.</param>
        [STAThread]
        static void Main(string[] args)
        {
            var _program = new Program();
            _program.Init(args);
            _program.Run();
        }

        private void Run()
        {
            var visual = _sp.GetRequiredService<IVisual>();
            visual.HandleUserInput();
        }

        private void Init(string[] args) => OnStartUp();
    }
}