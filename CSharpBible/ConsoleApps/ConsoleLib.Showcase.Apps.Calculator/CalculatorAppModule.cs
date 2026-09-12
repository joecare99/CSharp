using System;
using System.Drawing;
using System.IO;
using ConsoleLib;
using ConsoleLib.Showcase.Apps;

namespace ConsoleLib.Showcase.Apps.Calculator;

public sealed class CalculatorAppModule : IShowcaseAppModule
{
    public const string AppId = "Calculator";

    public void Register(ShowcaseAppRegistrationContext context)
    {
        if (context is null) throw new ArgumentNullException(nameof(context));
        context.Register(new ShowcaseAppDescriptor(AppId, "Calculator", ShowcaseAppCategory.Productivity,
            "A four-function calculator.", new Point(10, 6), new Size(36, 18),
            _ => new CalculatorViewModel(), (_, model) => Load(model)));
    }

    private static CxamlLoadResult Load(object model) =>
        LoadResource("Calculator.cxaml", model);

    private static CxamlLoadResult LoadResource(string name, object model)
    {
        var resourceName = typeof(CalculatorAppModule).Assembly.GetManifestResourceNames()
            .Single(resource => resource.EndsWith("Pages." + name, StringComparison.Ordinal));
        using var stream = typeof(CalculatorAppModule).Assembly.GetManifestResourceStream(resourceName)
            ?? throw new InvalidOperationException("The calculator CXAML resource is missing.");
        using var reader = new StreamReader(stream);
        return new CxamlLoader().Load(reader, new CxamlLoadContext(model));
    }
}
