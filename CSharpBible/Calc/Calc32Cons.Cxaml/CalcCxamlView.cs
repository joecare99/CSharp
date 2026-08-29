using Calc32.ViewModels.Interfaces;
using ConsoleLib;
using ConsoleLib.CommonControls;
using ConsoleLib.Data;
using ConsoleLib.Interfaces;

namespace Calc32Cons.Cxaml;

/// <summary>Connects calculator-specific control metadata to the declarative calculator view.</summary>
internal sealed class CalcCxamlView
{
    private readonly IApplication? _application;
    private readonly ICalculatorViewModel _viewModel;

    public CalcCxamlView(ICalculatorViewModel viewModel, IApplication? application)
    {
        _viewModel = viewModel;
        _application = application;
    }

    public CxamlLoadResult Load()
    {
        using Stream stream = typeof(CalcCxamlView).Assembly.GetManifestResourceStream(
            "Calc32Cons.Cxaml.Views.Main.cxaml")
            ?? throw new InvalidOperationException("The calculator CXAML view resource is missing.");
        using StreamReader reader = new(stream);
        CxamlLoadResult result = new CxamlLoader().Load(reader, new CxamlLoadContext(_viewModel));

        Button close = GetControl<Button>(result, "Close");
        if (_application is not null)
        {
            close.OnClick += (_, _) => _application.Stop();
        }

        return result;
    }

    private static TControl GetControl<TControl>(CxamlLoadResult result, string name)
        where TControl : class, IControl
        => result.NamedControls.TryGetValue(name, out IControl? control) && control is TControl typedControl
            ? typedControl
            : throw new CxamlParseException($"The calculator CXAML view is missing '{name}'.");
}
