using BaseLib.Helper;
using System.Globalization;

namespace BaseLib.Show.Demos;

/// <summary>
/// Demonstrates the object conversion helpers from BaseLib.
/// </summary>
internal sealed class ObjectHelperDemo : ShowcaseDemoBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ObjectHelperDemo"/> class.
    /// </summary>
    /// <param name="showcaseConsole">The showcase console renderer.</param>
    /// <param name="text">The localized text provider.</param>
    public ObjectHelperDemo(ShowcaseConsole showcaseConsole, ShowcaseText text)
        : base(showcaseConsole, text)
    {
    }

    /// <inheritdoc/>
    public override char SelectionKey => '4';

    /// <inheritdoc/>
    public override string TitleKey => "Demo_Object_Title";

    /// <inheritdoc/>
    public override string MenuDescriptionKey => "Demo_Object_Description";

    /// <inheritdoc/>
    protected override IEnumerable<DemoExample> CreateExamples()
    {
        yield return Example(
            "Example_Object_AsInt",
            """
            object?[] values = ["42", 3.14, "abc", null, 100L];
            foreach (object? value in values)
            {
                int number = value.AsInt(-1);
            }
            """,
            () =>
            {
                object?[] values = ["42", 3.14, "abc", null, 100L];
                foreach (object? value in values)
                {
                    ShowcaseConsole.PrintResult(DescribeValue(value), value.AsInt(-1));
                }
            });

        yield return Example(
            "Example_Object_AsDouble",
            """
            object[] values = ["3.14", 42, "1.5e10", "invalid"];
            foreach (object value in values)
            {
                double number = value.AsDouble();
            }
            """,
            () =>
            {
                object[] values = ["3.14", 42, "1.5e10", "invalid"];
                foreach (object value in values)
                {
                    ShowcaseConsole.PrintResult(DescribeValue(value), value.AsDouble());
                }
            });

        yield return Example(
            "Example_Object_AsBool",
            """
            object[] values = ["true", "false", 1, 0, "1", 'T'];
            foreach (object value in values)
            {
                bool flag = value.AsBool();
            }
            """,
            () =>
            {
                object[] values = ["true", "false", 1, 0, "1", 'T'];
                foreach (object value in values)
                {
                    ShowcaseConsole.PrintResult(DescribeValue(value), value.AsBool());
                }
            });

        yield return Example(
            "Example_Object_AsDate",
            """
            object[] values = ["2024-01-15", 45290.0, "not a date"];
            foreach (object value in values)
            {
                DateTime result = value.AsDate();
            }
            """,
            () =>
            {
                object[] values = ["2024-01-15", 45290.0, "not a date"];
                foreach (object value in values)
                {
                    DateTime result = value.AsDate();
                    ShowcaseConsole.PrintResult(DescribeValue(value), result == default ? Text.Get("Common_Invalid") : result.ToString("d", CultureInfo.CurrentCulture));
                }
            });

        yield return Example(
            "Example_Object_AsEnum",
            """
            ConsoleColor namedColor = "Red".AsEnum<ConsoleColor>();
            ConsoleColor numericColor = 1.AsEnum<ConsoleColor>();
            ConsoleColor darkBlue = "DarkBlue".AsEnum<ConsoleColor>();
            """,
            () =>
            {
                ShowcaseConsole.PrintResult("\"Red\"", "Red".AsEnum<ConsoleColor>());
                ShowcaseConsole.PrintResult("1", 1.AsEnum<ConsoleColor>());
                ShowcaseConsole.PrintResult("\"DarkBlue\"", "DarkBlue".AsEnum<ConsoleColor>());
            });

        yield return Example(
            "Example_Object_AsString",
            """
            object?[] values = [123, 3.14, DateTime.Now, null];
            foreach (object? value in values)
            {
                string text = value?.AsString() ?? "(null)";
            }
            """,
            () =>
            {
                object?[] values = [123, 3.14, DateTime.Now, null];
                foreach (object? value in values)
                {
                    ShowcaseConsole.PrintResult(value?.GetType().Name ?? Text.Get("Common_Null"), value?.AsString() ?? Text.Get("Common_NullDisplay"));
                }
            });
    }

    private string DescribeValue(object? value)
    {
        string display = value?.ToString() ?? Text.Get("Common_Null");
        string typeName = value?.GetType().Name ?? Text.Get("Common_Null");
        return $"'{display}' ({typeName})";
    }
}
