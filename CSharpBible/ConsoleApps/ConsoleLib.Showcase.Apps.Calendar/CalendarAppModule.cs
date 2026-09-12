using System;
using System.Drawing;
using System.IO;
using System.Linq;
using ConsoleLib;
using ConsoleLib.Showcase.Apps;

namespace ConsoleLib.Showcase.Apps.Calendar;

/// <summary>Registers the standalone calendar feature with any ConsoleLib host.</summary>
public sealed class CalendarAppModule : IShowcaseAppModule
{
    public void Register(ShowcaseAppRegistrationContext context)
    {
        context.Register(new ShowcaseAppDescriptor(
            "Calendar",
            "Calendar",
            ShowcaseAppCategory.Productivity,
            "Month navigation implemented as a reusable CXAML Page.",
            new Point(3, 4),
            new Size(44, 18),
            _ => new CalendarViewModel(),
            (_, viewModel) => Load(viewModel)));
    }

    private static CxamlLoadResult Load(object viewModel)
    {
        var assembly = typeof(CalendarAppModule).Assembly;
        var resourceName = assembly.GetManifestResourceNames()
            .Single(name => name.EndsWith(".Calendar.cxaml", StringComparison.Ordinal));
        using var stream = assembly.GetManifestResourceStream(resourceName)
            ?? throw new InvalidOperationException("The calendar CXAML resource is missing.");
        using var reader = new StreamReader(stream);
        return new CxamlLoader().LoadPage(reader, new CxamlLoadContext(viewModel));
    }
}
