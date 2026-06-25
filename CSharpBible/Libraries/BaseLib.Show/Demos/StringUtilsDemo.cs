using BaseLib.Helper;
using System.Globalization;

namespace BaseLib.Show.Demos;

/// <summary>
/// Demonstrates the string helper extensions from BaseLib.
/// </summary>
internal sealed class StringUtilsDemo : ShowcaseDemoBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StringUtilsDemo"/> class.
    /// </summary>
    /// <param name="showcaseConsole">The showcase console renderer.</param>
    /// <param name="text">The localized text provider.</param>
    public StringUtilsDemo(ShowcaseConsole showcaseConsole, ShowcaseText text)
        : base(showcaseConsole, text)
    {
    }

    /// <inheritdoc/>
    public override char SelectionKey => '1';

    /// <inheritdoc/>
    public override string TitleKey => "Demo_StringUtils_Title";

    /// <inheritdoc/>
    public override string MenuDescriptionKey => "Demo_StringUtils_Description";

    /// <inheritdoc/>
    protected override IEnumerable<DemoExample> CreateExamples()
    {
        yield return Example(
            "Example_String_Quote",
            """
            string original = "Line1\nLine2\tTab";
            string quoted = original.Quote();
            string unquoted = quoted.UnQuote();
            """,
            () =>
            {
                string original = "Line1\nLine2\tTab";
                string quoted = original.Quote();
                string unquoted = quoted.UnQuote();

                ShowcaseConsole.PrintResult(Text.Get("Common_Original"), $"\"{Visualize(original)}\"");
                ShowcaseConsole.PrintResult(Text.Get("Common_Quoted"), $"\"{quoted}\"");
                ShowcaseConsole.PrintResult(Text.Get("Common_Unquoted"), $"\"{Visualize(unquoted)}\"");
            });

        yield return Example(
            "Example_String_Format",
            """
            string template = "Hello {0}, today is {1}!";
            string formatted = template.Format("World", DateTime.Now.ToString("dddd", CultureInfo.CurrentCulture));
            """,
            () =>
            {
                string template = "Hello {0}, today is {1}!";
                string formatted = template.Format("World", DateTime.Now.ToString("dddd", CultureInfo.CurrentCulture));

                ShowcaseConsole.PrintResult(Text.Get("Common_Template"), template);
                ShowcaseConsole.PrintResult(Text.Get("Common_Formatted"), formatted);
            });

        yield return Example(
            "Example_String_FirstRest",
            """
            string sentence = "This is a sample sentence";
            string first = sentence.SFirst();
            string rest = sentence.SRest();
            """,
            () =>
            {
                string sentence = "This is a sample sentence";
                ShowcaseConsole.PrintResult(Text.Get("Common_Original"), sentence);
                ShowcaseConsole.PrintResult(Text.Get("Common_FirstWord"), sentence.SFirst());
                ShowcaseConsole.PrintResult(Text.Get("Common_Rest"), sentence.SRest());
            });

        yield return Example(
            "Example_String_ToNormal",
            """
            string[] names = ["pETER", "MÜLLER", "schmidt", "McGregor"];
            foreach (string name in names)
            {
                string normalized = name.ToNormal();
            }
            """,
            () =>
            {
                string[] names = ["pETER", "MÜLLER", "schmidt", "McGregor"];
                foreach (string name in names)
                {
                    ShowcaseConsole.PrintResult(name, name.ToNormal());
                }
            });

        yield return Example(
            "Example_String_LeftRight",
            """
            string text = "BaseLib Showcase";
            string left = text.Left(7);
            string right = text.Right(8);
            string leftWithoutLast = text.Left(-8);
            string rightWithoutFirst = text.Right(-7);
            """,
            () =>
            {
                string text = "BaseLib Showcase";
                ShowcaseConsole.PrintResult(Text.Get("Common_Original"), text);
                ShowcaseConsole.PrintResult("Left(7)", text.Left(7));
                ShowcaseConsole.PrintResult("Right(8)", text.Right(8));
                ShowcaseConsole.PrintResult("Left(-8)", text.Left(-8));
                ShowcaseConsole.PrintResult("Right(-7)", text.Right(-7));
            });

        yield return Example(
            "Example_String_QuotedSplit",
            """
            string csv = "Name,\"Description, with comma\",Value";
            IReadOnlyList<string> parts = csv.QuotedSplit();
            """,
            () =>
            {
                string csv = "Name,\"Description, with comma\",Value";
                IReadOnlyList<string> parts = csv.QuotedSplit();
                ShowcaseConsole.PrintResult(Text.Get("Common_CsvLine"), csv);
                for (int i = 0; i < parts.Count; i++)
                {
                    ShowcaseConsole.PrintResult(Text.Format("Common_ItemNumber", i + 1), parts[i]);
                }
            });

        yield return Example(
            "Example_String_Identifier",
            """
            string[] identifiers = ["MyVariable", "123Invalid", "_underscore", "Valid_Name"];
            foreach (string identifier in identifiers)
            {
                bool isValid = identifier.IsValidIdentifyer();
            }
            """,
            () =>
            {
                string[] identifiers = ["MyVariable", "123Invalid", "_underscore", "Valid_Name"];
                foreach (string identifier in identifiers)
                {
                    ShowcaseConsole.PrintResult(identifier, identifier.IsValidIdentifyer() ? Text.Get("Common_Valid") : Text.Get("Common_Invalid"));
                }
            });

        yield return Example(
            "Example_String_PadTab",
            """
            string tabText = "A\tB\tC";
            string aligned = tabText.PadTab();
            """,
            () =>
            {
                string tabText = "A\tB\tC";
                ShowcaseConsole.PrintResult(Text.Get("Common_Original"), Visualize(tabText));
                ShowcaseConsole.PrintResult("PadTab", tabText.PadTab());
            });

        yield return Example(
            "Example_String_Any",
            """
            string testString = "HelloWorld.txt";
            bool contains = testString.ContainsAny("llo", "xyz");
            bool starts = testString.StartswithAny("Hi", "Hello");
            bool ends = testString.EndswithAny(".cs", ".txt");
            """,
            () =>
            {
                string testString = "HelloWorld.txt";
                ShowcaseConsole.PrintResult(Text.Get("Common_String"), testString);
                ShowcaseConsole.PrintResult("ContainsAny('llo', 'xyz')", testString.ContainsAny("llo", "xyz"));
                ShowcaseConsole.PrintResult("StartswithAny('Hi', 'Hello')", testString.StartswithAny("Hi", "Hello"));
                ShowcaseConsole.PrintResult("EndswithAny('.cs', '.txt')", testString.EndswithAny(".cs", ".txt"));
            });
    }

    private static string Visualize(string value) => value.Replace("\n", "↵").Replace("\t", "→");
}
