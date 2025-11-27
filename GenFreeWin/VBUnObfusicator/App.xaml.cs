using MVVM.View.Extension;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using TranspilerLib.Models;
using TranspilerLib.Interfaces.Code;
using BaseLib.Helper;
using VBUnObfusicator.Models;

namespace VBUnObfusicator
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
               .AddTransient<ICSCode, CSCode>();

            IoC.GetReqSrv = builder.BuildServiceProvider().GetRequiredService;
        }

    }
}
