using BaseLib.Helper;

namespace BaseLib.Show.Demos;

/// <summary>
/// Demonstrates list helper extensions from BaseLib.
/// </summary>
internal sealed class ListHelperDemo : ShowcaseDemoBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ListHelperDemo"/> class.
    /// </summary>
    /// <param name="showcaseConsole">The showcase console renderer.</param>
    /// <param name="text">The localized text provider.</param>
    public ListHelperDemo(ShowcaseConsole showcaseConsole, ShowcaseText text)
        : base(showcaseConsole, text)
    {
    }

    /// <inheritdoc/>
    public override char SelectionKey => '5';

    /// <inheritdoc/>
    public override string TitleKey => "Demo_List_Title";

    /// <inheritdoc/>
    public override string MenuDescriptionKey => "Demo_List_Description";

    /// <inheritdoc/>
    protected override IEnumerable<DemoExample> CreateExamples()
    {
        yield return Example(
            "Example_List_Swap",
            """
            List<string> items = ["A", "B", "C", "D", "E"];
            items.Swap(1, 3);
            """,
            () =>
            {
                List<string> items = ["A", "B", "C", "D", "E"];
                ShowcaseConsole.PrintResult(Text.Get("Common_Original"), string.Join(", ", items));
                items.Swap(1, 3);
                ShowcaseConsole.PrintResult("Swap(1, 3)", string.Join(", ", items));
            });

        yield return Example(
            "Example_List_MoveItem",
            """
            List<string> items = ["A", "B", "C", "D", "E"];
            items.MoveItem(0, 3);
            items.MoveItem(4, 1);
            """,
            () =>
            {
                List<string> items = ["A", "B", "C", "D", "E"];
                ShowcaseConsole.PrintResult(Text.Get("Common_Original"), string.Join(", ", items));
                items.MoveItem(0, 3);
                ShowcaseConsole.PrintResult("MoveItem(0, 3)", string.Join(", ", items));
                items.MoveItem(4, 1);
                ShowcaseConsole.PrintResult("MoveItem(4, 1)", string.Join(", ", items));
            });

        yield return Example(
            "Example_List_Range",
            """
            IEnumerable<int> range1 = 1.To(10);
            IEnumerable<int> range2 = 5.To(8);
            """,
            () =>
            {
                IEnumerable<int> range1 = 1.To(10);
                IEnumerable<int> range2 = 5.To(8);
                ShowcaseConsole.PrintResult("1.To(10)", string.Join(", ", range1));
                ShowcaseConsole.PrintResult("5.To(8)", string.Join(", ", range2));
            });
    }
}
