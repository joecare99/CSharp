using BaseLib.Helper;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using TranspilerLib.Interfaces.Code;
using TranspilerLib.Models.Scanner;

namespace VBUnObfusicator;

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
