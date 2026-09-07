using System;
using System.IO;
using System.Linq;
using ConsoleLib;

namespace ConsoleLib.Showcase.Desktop;

/// <summary>Loads embedded CXAML pages independently of the selected host widget set.</summary>
public sealed class ShowcasePageLoader
{
    private readonly CxamlLoader _loader;

    public ShowcasePageLoader(CxamlLoader? loader = null) =>
        _loader = loader ?? new CxamlLoader();

    /// <summary>Loads and binds a named page to the supplied ViewModel.</summary>
    public CxamlLoadResult Load(ShowcasePage page, object viewModel)
    {
        if (viewModel is null)
            throw new ArgumentNullException(nameof(viewModel));

        var pageName = page + ".cxaml";
        var resourceName = typeof(ShowcasePageLoader).Assembly.GetManifestResourceNames()
            .SingleOrDefault(name => name.EndsWith("Pages." + pageName, StringComparison.OrdinalIgnoreCase));
        if (resourceName is null)
        {
            throw new InvalidOperationException($"The CXAML page resource '{pageName}' is missing.");
        }

        using var stream = typeof(ShowcasePageLoader).Assembly.GetManifestResourceStream(resourceName)
            ?? throw new InvalidOperationException($"The CXAML page resource '{pageName}' could not be opened.");
        using var reader = new StreamReader(stream);
        return _loader.Load(reader, new CxamlLoadContext(viewModel));
    }
}
