using BaseLib.Helper;

namespace BaseLib.Show.Demos;

/// <summary>
/// Demonstrates mathematical helpers and filters from BaseLib.
/// </summary>
internal sealed class MathUtilitiesDemo : ShowcaseDemoBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MathUtilitiesDemo"/> class.
    /// </summary>
    /// <param name="showcaseConsole">The showcase console renderer.</param>
    /// <param name="text">The localized text provider.</param>
    public MathUtilitiesDemo(ShowcaseConsole showcaseConsole, ShowcaseText text)
        : base(showcaseConsole, text)
    {
    }

    /// <inheritdoc/>
    public override char SelectionKey => '2';

    /// <inheritdoc/>
    public override string TitleKey => "Demo_Math_Title";

    /// <inheritdoc/>
    public override string MenuDescriptionKey => "Demo_Math_Description";

    /// <inheritdoc/>
    protected override IEnumerable<DemoExample> CreateExamples()
    {
        yield return Example(
            "Example_Math_Pt1",
            """
            double actualValue = 0;
            for (int i = 0; i < 10; i++)
            {
                double newValue = 100.0;
                newValue.PT1(ref actualValue, 0.3);
            }
            """,
            () =>
            {
                double actualValue = 0;
                ShowcaseConsole.WriteLine($"    {Text.Get("Math_Pt1_Intro")}");
                for (int i = 0; i < 10; i++)
                {
                    double newValue = 100.0;
                    newValue.PT1(ref actualValue, 0.3);
                    ShowcaseConsole.PrintResult(Text.Format("Common_StepNumber", i + 1), actualValue.ToString("F2"));
                }
            });

        yield return Example(
            "Example_Math_Mean",
            """
            double[] buffer = new double[5];
            int index = 0;
            double[] values = [10, 20, 30, 40, 50, 60, 70];
            foreach (double value in values)
            {
                double mean = value.Mean(buffer, ref index);
            }
            """,
            () =>
            {
                double[] buffer = new double[5];
                int index = 0;
                double[] values = [10, 20, 30, 40, 50, 60, 70];
                ShowcaseConsole.WriteLine($"    {Text.Format("Common_BufferSize", buffer.Length)}");
                foreach (double value in values)
                {
                    double mean = value.Mean(buffer, ref index);
                    ShowcaseConsole.PrintResult(Text.Format("Common_ValueNumber", value.ToString("F0")), $"Mean = {mean:F2}");
                }
            });

        yield return Example(
            "Example_Math_Median",
            """
            double[] buffer = new double[5];
            int index = 0;
            double[] noisyValues = [10, 100, 20, 15, 25, 200, 30];
            foreach (double value in noisyValues)
            {
                double median = value.Median(buffer, ref index);
            }
            """,
            () =>
            {
                double[] buffer = new double[5];
                int index = 0;
                double[] noisyValues = [10, 100, 20, 15, 25, 200, 30];
                ShowcaseConsole.WriteLine($"    {Text.Format("Math_Median_Buffer", buffer.Length)}");
                foreach (double value in noisyValues)
                {
                    double median = value.Median(buffer, ref index);
                    ShowcaseConsole.PrintResult(Text.Format("Common_ValueNumber", value.ToString("F0")), $"Median = {median:F2}");
                }
            });
    }
}
