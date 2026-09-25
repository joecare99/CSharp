using BaseLib.Helper;
using BaseLib.Models;
using BaseLib.Models.Interfaces;
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
            .AddTransient<ICodeOptimizer, CodeOptimizer>()
            .AddTransient<ITokenHandler>(sp => new CSTokenHandler()
            {
                stringEndChars = CSCode.stringEndChars,
                reservedWords = CSCode.ReservedWords
            })
            .AddTransient<ICodeBuilder, CSCodeBuilder>()

           .AddTransient<ICSCode, CSCode>();

        IoC.GetReqSrv = builder.BuildServiceProvider().GetRequiredService;
    }

}
