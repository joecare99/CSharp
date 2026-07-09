using BaseLib.Models;
using System.Globalization;

namespace BaseLib.Show.Demos;

/// <summary>
/// Demonstrates the system time and random abstractions from BaseLib.
/// </summary>
internal sealed class SysTimeAndRandomDemo : ShowcaseDemoBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SysTimeAndRandomDemo"/> class.
    /// </summary>
    /// <param name="showcaseConsole">The showcase console renderer.</param>
    /// <param name="text">The localized text provider.</param>
    public SysTimeAndRandomDemo(ShowcaseConsole showcaseConsole, ShowcaseText text)
        : base(showcaseConsole, text)
    {
    }

    /// <inheritdoc/>
    public override char SelectionKey => '9';

    /// <inheritdoc/>
    public override string TitleKey => "Demo_TimeRandom_Title";

    /// <inheritdoc/>
    public override string MenuDescriptionKey => "Demo_TimeRandom_Description";

    /// <inheritdoc/>
    protected override IEnumerable<DemoExample> CreateExamples()
    {
        yield return Example(
            "Example_Time_SysTime",
            """
            var sysTime = new SysTime();
            DateTime now = sysTime.Now;
            DateTime today = sysTime.Today;

            Func<DateTime> originalGetNow = SysTime.GetNow;
            SysTime.GetNow = () => new DateTime(2020, 1, 1, 12, 0, 0);
            DateTime mockedNow = sysTime.Now;
            SysTime.GetNow = originalGetNow;
            """,
            () =>
            {
                SysTime sysTime = new();
                ShowcaseConsole.PrintResult(Text.Get("Common_Now"), sysTime.Now.ToString("G", CultureInfo.CurrentCulture));
                ShowcaseConsole.PrintResult(Text.Get("Common_Today"), sysTime.Today.ToString("d", CultureInfo.CurrentCulture));
                ShowcaseConsole.BlankLine();

                ShowcaseConsole.WriteLine($"    {Text.Get("Time_MockIntro")}");
                RawConsole.ForegroundColor = ConsoleColor.DarkGray;
                ShowcaseConsole.WriteLine("    SysTime.GetNow = () => new DateTime(2020, 1, 1, 12, 0, 0);");
                RawConsole.ForegroundColor = ConsoleColor.Gray;

                Func<DateTime> originalGetNow = SysTime.GetNow;
                try
                {
                    SysTime.GetNow = () => new DateTime(2020, 1, 1, 12, 0, 0);
                    ShowcaseConsole.PrintResult(Text.Get("Time_NowMocked"), sysTime.Now.ToString("G", CultureInfo.CurrentCulture));
                }
                finally
                {
                    SysTime.GetNow = originalGetNow;
                }

                ShowcaseConsole.PrintResult(Text.Get("Time_NowRestored"), sysTime.Now.ToString("G", CultureInfo.CurrentCulture));
            });

        yield return Example(
            "Example_Time_Random",
            """
            var random = new CRandom();
            for (int i = 0; i < 10; i++)
            {
                int number = random.Next(1, 100);
            }

            random.Seed(42);
            """,
            () =>
            {
                CRandom random = new();

                ShowcaseConsole.WriteLine($"    {Text.Get("Random_IntegerIntro")}");
                ShowcaseConsole.Write("    ");
                for (int i = 0; i < 10; i++)
                {
                    ShowcaseConsole.Write($"{random.Next(1, 100),4}");
                }

                ShowcaseConsole.BlankLine();
                ShowcaseConsole.BlankLine();
                ShowcaseConsole.WriteLine($"    {Text.Get("Random_DoubleIntro")}");
                ShowcaseConsole.Write("    ");
                for (int i = 0; i < 5; i++)
                {
                    ShowcaseConsole.Write($"{random.NextDouble():F3}  ");
                }

                ShowcaseConsole.BlankLine();
                ShowcaseConsole.BlankLine();
                ShowcaseConsole.WriteLine($"    {Text.Get("Random_ReproIntro")}");
                random.Seed(42);
                ShowcaseConsole.Write($"    {Text.Get("Common_Sequence1")}: ");
                for (int i = 0; i < 5; i++)
                {
                    ShowcaseConsole.Write($"{random.Next(1, 100),4}");
                }

                ShowcaseConsole.BlankLine();
                random.Seed(42);
                ShowcaseConsole.Write($"    {Text.Get("Common_Sequence2")}: ");
                for (int i = 0; i < 5; i++)
                {
                    ShowcaseConsole.Write($"{random.Next(1, 100),4}");
                }

                ShowcaseConsole.BlankLine();
                ShowcaseConsole.WriteLine($"    {Text.Get("Random_IdenticalNote")}");
            });
    }
}
