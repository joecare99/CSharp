using BaseLib.Helper;

namespace BaseLib.Show.Demos;

/// <summary>
/// Demonstrates type helper utilities from BaseLib.
/// </summary>
internal sealed class TypeUtilsDemo : ShowcaseDemoBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TypeUtilsDemo"/> class.
    /// </summary>
    /// <param name="showcaseConsole">The showcase console renderer.</param>
    /// <param name="text">The localized text provider.</param>
    public TypeUtilsDemo(ShowcaseConsole showcaseConsole, ShowcaseText text)
        : base(showcaseConsole, text)
    {
    }

    /// <inheritdoc/>
    public override char SelectionKey => '6';

    /// <inheritdoc/>
    public override string TitleKey => "Demo_Type_Title";

    /// <inheritdoc/>
    public override string MenuDescriptionKey => "Demo_Type_Description";

    /// <inheritdoc/>
    protected override IEnumerable<DemoExample> CreateExamples()
    {
        yield return Example(
            "Example_Type_TypeCode",
            """
            Type[] types = [typeof(int), typeof(string), typeof(double), typeof(bool), typeof(DateTime)];
            foreach (Type type in types)
            {
                TypeCode typeCode = type.TC();
            }
            """,
            () =>
            {
                Type[] types = [typeof(int), typeof(string), typeof(double), typeof(bool), typeof(DateTime)];
                foreach (Type type in types)
                {
                    ShowcaseConsole.PrintResult(type.Name, type.TC());
                }
            });

        yield return Example(
            "Example_Type_ToType",
            """
            string[] typeNames = ["Int32", "String", "Double", "Boolean"];
            foreach (string typeName in typeNames)
            {
                Type resolvedType = typeName.ToType();
            }
            """,
            () =>
            {
                string[] typeNames = ["Int32", "String", "Double", "Boolean"];
                foreach (string typeName in typeNames)
                {
                    Type resolvedType = typeName.ToType();
                    ShowcaseConsole.PrintResult(typeName, resolvedType.FullName ?? Text.Get("Common_NotAvailable"));
                }
            });

        yield return Example(
            "Example_Type_Between",
            """
            int value = 5;
            bool inclusive = value.IsBetweenIncl(1, 5);
            bool exclusive = value.IsBetweenExcl(1, 5);
            """,
            () =>
            {
                const int value = 5;
                ShowcaseConsole.PrintResult("5.IsBetweenIncl(1, 5)", value.IsBetweenIncl(1, 5));
                ShowcaseConsole.PrintResult("5.IsBetweenExcl(1, 5)", value.IsBetweenExcl(1, 5));
                ShowcaseConsole.PrintResult("5.IsBetweenIncl(5, 10)", value.IsBetweenIncl(5, 10));
                ShowcaseConsole.PrintResult("5.IsBetweenExcl(5, 10)", value.IsBetweenExcl(5, 10));
            });

        yield return Example(
            "Example_Type_CheckLimit",
            """
            int inside = 5.CheckLimit(1, 10);
            int above = 15.CheckLimit(1, 10);
            int below = 0.CheckLimit(1, 10);
            """,
            () =>
            {
                ShowcaseConsole.PrintResult("5.CheckLimit(1, 10)", 5.CheckLimit(1, 10));
                ShowcaseConsole.PrintResult("15.CheckLimit(1, 10)", 15.CheckLimit(1, 10));
                ShowcaseConsole.PrintResult("0.CheckLimit(1, 10)", 0.CheckLimit(1, 10));
            });

        yield return Example(
            "Example_Type_Get",
            """
            object intValue = typeof(int).Get("42");
            object doubleValue = typeof(double).Get("3.14");
            object boolValue = typeof(bool).Get("true");
            """,
            () =>
            {
                ShowcaseConsole.PrintResult("typeof(int).Get(\"42\")", typeof(int).Get("42"));
                ShowcaseConsole.PrintResult("typeof(double).Get(\"3.14\")", typeof(double).Get("3.14"));
                ShowcaseConsole.PrintResult("typeof(bool).Get(\"true\")", typeof(bool).Get("true"));
            });
    }
}
