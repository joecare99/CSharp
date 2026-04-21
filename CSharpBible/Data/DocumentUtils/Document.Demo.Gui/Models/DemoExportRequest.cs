namespace Document.Demo.Gui.Models;

public sealed class DemoExportRequest
{
    public DemoExportRequest(string outputPath, string formatKey, IReadOnlyCollection<DemoFeature> selectedFeatures)
    {
        OutputPath = outputPath;
        FormatKey = formatKey;
        SelectedFeatures = selectedFeatures;
    }

    public string OutputPath { get; }

    public string FormatKey { get; }

    public IReadOnlyCollection<DemoFeature> SelectedFeatures { get; }
}
