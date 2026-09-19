namespace GenInterfaces.Interfaces.Genealogic.Drivers;

/// <summary>
/// Defines an asynchronous genealogy driver that supports both import and export.
/// </summary>
/// <typeparam name="TModel">The model type handled by the driver.</typeparam>
public interface IGenImportExportDriver<TModel> :
    IGenImportDriver<TModel>,
    IGenExportDriver<TModel>
{
}
