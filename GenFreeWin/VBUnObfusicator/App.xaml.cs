using BaseLib.Helper;
using BaseLib.Models;
using BaseLib.Models.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;
using TranspilerLib.Interfaces.Code;
using TranspilerLib.Models.Scanner;
#if NET8_0_OR_GREATER
using TranspilerLib.CSharp.StatEqualCheck;
using TranspilerLib.CSharp.VBLegacyReplace;
#endif

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

           .AddTransient<ICSCode, CSCode>()
           .AddTransient<VBUnObfusicator.ViewModels.CodeUnObFusViewModel>();

#if NET8_0_OR_GREATER
        builder
            .AddSingleton<Func<ICodeBlock, ICodeBlock, System.Collections.Generic.IEnumerable<ICodeBlock>?, System.Collections.Generic.IEnumerable<ICodeBlock>?, StatEqualResult>>(_ => StatEqualCheck.Compare)
            .AddTransient<LegacyReplacementEngine>(_ => LegacyReplacementEngine.LoadDefaultRules());
#endif

        IoC.GetReqSrv = builder.BuildServiceProvider().GetRequiredService;
    }

}


