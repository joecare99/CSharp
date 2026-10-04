using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Genealogy.Drivers;
using OFBCreator.Abstractions.Interfaces;
using OFBCreator.Core.Services;
using GenInterfaces.Interfaces.Genealogic;

namespace OFBCreator.Console.Services;

/// <summary>
/// Bridges the provider-neutral GEDCOM model into the existing OFB data-source contract.
/// </summary>
public sealed class CanonicalGedcomFamilyDataSource : IFamilyDataSource
{
    private readonly IGenealogyInputDriver _inputDriver;
    private readonly CanonicalGenealogyAdapter _adapter;

    public CanonicalGedcomFamilyDataSource(
        IGenealogyInputDriver inputDriver,
        CanonicalGenealogyAdapter adapter)
    {
        _inputDriver = inputDriver ?? throw new ArgumentNullException(nameof(inputDriver));
        _adapter = adapter ?? throw new ArgumentNullException(nameof(adapter));
    }

    public string SourceId => "gedcom";

    public string DisplayName => "GEDCOM Import (Canonical Model)";

    public bool CanRead(Stream stream) => _inputDriver.CanRead(stream);

    public async Task<IGenealogy> ImportAsync(
        Stream stream,
        CancellationToken cancellationToken = default)
    {
        var document = await _inputDriver.ReadAsync(stream, cancellationToken).ConfigureAwait(false);
        return _adapter.Adapt(document);
    }
}
