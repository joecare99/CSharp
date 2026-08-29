using ConsoleLib;
using ConsoleLib.CommonControls;
using ConsoleLib.Interfaces;
using Leonardo.ViewModels.Interfaces;
using CxamlButton = ConsoleLib.CommonControls.Button;
using CxamlPanel = ConsoleLib.CommonControls.Panel;

namespace Leonardo.ST.Cxaml;

/// <summary>Connects Leonardo's platform dialogs and terminal output to named CXAML controls.</summary>
internal sealed class LeonardoCxamlView
{
    private readonly IApplication? _application;
    private readonly LeonardoInteractionAdapter _interactions = new();
    private readonly ILeonardoViewModel _viewModel;

    public LeonardoCxamlView(ILeonardoViewModel viewModel, IApplication? application)
    {
        _viewModel = viewModel;
        _application = application;
    }

    public CxamlLoadResult Load()
    {
        using Stream stream = typeof(LeonardoCxamlView).Assembly.GetManifestResourceStream(
            "Leonardo.ST.Cxaml.Views.Main.cxaml")
            ?? throw new InvalidOperationException("The Leonardo CXAML view resource is missing.");
        using StreamReader reader = new(stream);
        CxamlLoadResult result = new CxamlLoader().Load(reader, new CxamlLoadContext(_viewModel));

        _ = GetControl<CxamlPanel>(result, "Leonardo");
        CxamlButton close = GetControl<CxamlButton>(result, "Close");
        if (_application is not null)
        {
            close.OnClick += (_, _) => _application.Stop();
        }

        Terminal txtWindow = GetControl<Terminal>(result, "Output");
        CxamlButton btnTest1 = GetControl<CxamlButton>(result, "Test");
        if (_application is not null && txtWindow is not null)
        {
            btnTest1.OnClick += (s, e) => txtWindow.WriteLine($"This is a Test {btnTest1.Tag = ((int?)btnTest1.Tag ?? 0) % 100 + 1}...");
        }

        _viewModel.ShowFileDialog = static dialog => dialog.ShowDialog();
        _viewModel.InputShowDialog = _interactions.RequestInput;
        _viewModel.MessageBoxShow = _interactions.ShowMessage;
        _viewModel.SetConsole(txtWindow);
        return result;
    }

    private static TControl GetControl<TControl>(CxamlLoadResult result, string name)
        where TControl : class, IControl
        => result.NamedControls.TryGetValue(name, out IControl? control) && control is TControl typedControl
            ? typedControl
            : throw new CxamlParseException($"The Leonardo CXAML view is missing '{name}'.");
}
