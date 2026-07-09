using BaseLib.Helper;
using BaseLib.Interfaces;
using BaseLib.Models;
using Microsoft.Extensions.DependencyInjection;
using RandomInterface = BaseLib.Models.Interfaces.IRandom;
using SysTimeInterface = BaseLib.Models.Interfaces.ISysTime;

namespace BaseLib.Show.Demos;

/// <summary>
/// Demonstrates the static IoC helper integration.
/// </summary>
internal sealed class IoCDemo : ShowcaseDemoBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IoCDemo"/> class.
    /// </summary>
    /// <param name="showcaseConsole">The showcase console renderer.</param>
    /// <param name="text">The localized text provider.</param>
    public IoCDemo(ShowcaseConsole showcaseConsole, ShowcaseText text)
        : base(showcaseConsole, text)
    {
    }

    /// <inheritdoc/>
    public override char SelectionKey => '8';

    /// <inheritdoc/>
    public override string TitleKey => "Demo_IoC_Title";

    /// <inheritdoc/>
    public override string MenuDescriptionKey => "Demo_IoC_Description";

    /// <inheritdoc/>
    protected override IEnumerable<DemoExample> CreateExamples()
    {
        yield return Example(
            "Example_IoC_Configure",
            """
            var builder = new ServiceCollection();
            builder.AddSingleton<IConsole, ConsoleProxy>();
            builder.AddTransient<IRandom, CRandom>();
            builder.AddSingleton<ISysTime, SysTime>();
            IoC.Configure(builder.BuildServiceProvider());
            """,
            () =>
            {
                ShowcaseConsole.WriteLine($"    {Text.Get("IoC_CodeExampleLabel")}");
                RawConsole.ForegroundColor = ConsoleColor.DarkGray;
                ShowcaseConsole.WriteLine("    var builder = new ServiceCollection();");
                ShowcaseConsole.WriteLine("    builder.AddSingleton<IConsole, ConsoleProxy>();");
                ShowcaseConsole.WriteLine("    builder.AddTransient<IRandom, CRandom>();");
                ShowcaseConsole.WriteLine("    builder.AddSingleton<ISysTime, SysTime>();");
                ShowcaseConsole.WriteLine("    IoC.Configure(builder.BuildServiceProvider());");
                RawConsole.ForegroundColor = ConsoleColor.Gray;

                ServiceCollection builder = new();
                builder.AddSingleton<IConsole, ConsoleProxy>();
                builder.AddTransient<RandomInterface, CRandom>();
                builder.AddSingleton<SysTimeInterface, SysTime>();
                IoC.Configure(builder.BuildServiceProvider());
            });

        yield return Example(
            "Example_IoC_Resolve",
            """
            IConsole console = IoC.GetRequiredService<IConsole>();
            IRandom random = IoC.GetRequiredService<IRandom>();
            ISysTime? sysTime = IoC.GetService<ISysTime>();
            StringBuilder? missing = IoC.GetService<StringBuilder>();
            """,
            () =>
            {
                IConsole console = IoC.GetRequiredService<IConsole>();
                RandomInterface random = IoC.GetRequiredService<RandomInterface>();
                SysTimeInterface? sysTime = IoC.GetService<SysTimeInterface>();
                System.Text.StringBuilder? missing = IoC.GetService<System.Text.StringBuilder>();

                ShowcaseConsole.PrintResult("GetRequiredService<IConsole>()", console.GetType().Name);
                ShowcaseConsole.PrintResult("GetRequiredService<IRandom>()", random.GetType().Name);
                ShowcaseConsole.PrintResult("GetService<ISysTime>()", sysTime?.GetType().Name ?? Text.Get("Common_Null"));
                ShowcaseConsole.PrintResult("GetService<StringBuilder>()", missing?.GetType().Name ?? Text.Get("Common_Null"));
            });

        yield return Example(
            "Example_IoC_Scopes",
            """
            IServiceScope scope = IoC.GetNewScope();
            IoC.SetCurrentScope(scope);
            """,
            () =>
            {
                ShowcaseConsole.WriteLine($"    {Text.Get("IoC_ScopeIntro")}");
                RawConsole.ForegroundColor = ConsoleColor.DarkGray;
                ShowcaseConsole.WriteLine("    var scope = IoC.GetNewScope();");
                ShowcaseConsole.WriteLine("    IoC.SetCurrentScope(scope);");
                RawConsole.ForegroundColor = ConsoleColor.Gray;

                IServiceScope scope = IoC.GetNewScope();
                IoC.SetCurrentScope(scope);
            });
    }
}
