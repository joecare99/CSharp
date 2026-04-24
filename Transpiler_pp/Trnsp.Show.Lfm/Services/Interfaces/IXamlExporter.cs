using System.Text;
using Trnsp.Show.Lfm.Models.Components;

namespace Trnsp.Show.Lfm.Services.Interfaces;

/// <summary>
/// Interface for exporting LFM components to XAML.
/// </summary>
public interface IXamlExporter
{
    /// <summary>
    /// Exports a component tree to XAML Window markup.
    /// </summary>
    string ExportToXaml(LfmComponentBase component);

    /// <summary>
    /// Exports a component tree to XAML and saves it to a file.
    /// </summary>
    void ExportToFile(LfmComponentBase component, string filePath);
}
