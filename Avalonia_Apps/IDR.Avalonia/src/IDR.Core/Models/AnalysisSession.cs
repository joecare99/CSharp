using System;
using System.Collections.Generic;
using System.Linq;
using IDR.Core.Services;

namespace IDR.Core.Models;

public sealed class AnalysisSession
{
    private readonly Dictionary<uint, AnalysisItem> _items = [];
    private readonly Dictionary<uint, List<CrossReference>> _incomingCrossReferences = [];
    private IReadOnlyList<KnowledgeBaseModule> _knowledgeBaseModules = [];
    private IReadOnlyList<KnowledgeBaseProcedure> _knowledgeBaseProcedures = [];
    private IReadOnlyList<PeImportModule> _imports = [];
    private IReadOnlyList<PeExport> _exports = [];
    private readonly List<DisassemblyLine> _disassemblyLines = [];

    public AnalysisSession(
        string sourcePath,
        ReadOnlyMemory<byte> image,
        IEnumerable<PeSection>? sections = null,
        ulong imageBase = 0,
        DelphiVersion selectedDelphiVersion = DelphiVersion.Unknown)
    {
        if (string.IsNullOrWhiteSpace(sourcePath))
        {
            throw new ArgumentException("A source path is required.", nameof(sourcePath));
        }

        SourcePath = sourcePath;
        Image = image;
        ImageBase = imageBase;
        SelectedDelphiVersion = selectedDelphiVersion;
        Sections = sections?.ToArray() ?? [];
        AddressMap = Sections.Count == 0 ? null : new AddressMap(Sections);
    }

    public string SourcePath { get; }

    public ReadOnlyMemory<byte> Image { get; }

    public ulong ImageBase { get; }

    public DelphiVersion SelectedDelphiVersion { get; set; }

    public IReadOnlyList<PeSection> Sections { get; }

    public AddressMap? AddressMap { get; }

    public uint EntryPointRva { get; set; }

    public IReadOnlyDictionary<uint, AnalysisItem> Items => _items;

    public IReadOnlyList<KnowledgeBaseModule> KnowledgeBaseModules => _knowledgeBaseModules;

    public IReadOnlyList<KnowledgeBaseProcedure> KnowledgeBaseProcedures => _knowledgeBaseProcedures;

    public bool HasKnowledgeBase { get; private set; }

    public IReadOnlyList<PeImportModule> Imports => _imports;

    public IReadOnlyList<PeExport> Exports => _exports;

    public IReadOnlyList<DisassemblyLine> DisassemblyLines => _disassemblyLines;

    public IReadOnlyList<CrossReference> GetIncomingCrossReferences(uint targetAddress)
    {
        return _incomingCrossReferences.TryGetValue(targetAddress, out List<CrossReference>? references)
            ? references
            : Array.Empty<CrossReference>();
    }

    public ushort? SystemModuleId { get; private set; }

    public void LoadKnowledgeBase(IKnowledgeBase knowledgeBase)
    {
        ArgumentNullException.ThrowIfNull(knowledgeBase);
        _knowledgeBaseModules = knowledgeBase.Modules.ToArray();
        _knowledgeBaseProcedures = knowledgeBase.Procedures.ToArray();
        HasKnowledgeBase = true;
        SystemModuleId = null;
    }

    public void LoadPeDirectories(
        IEnumerable<PeImportModule> imports,
        IEnumerable<PeExport> exports)
    {
        ArgumentNullException.ThrowIfNull(imports);
        ArgumentNullException.ThrowIfNull(exports);
        _imports = imports.ToArray();
        _exports = exports.ToArray();
    }

    public void ClearAnalysisItems()
    {
        _items.Clear();
        _incomingCrossReferences.Clear();
        _disassemblyLines.Clear();
        SystemModuleId = null;
    }

    public CrossReference AddCrossReference(
        uint sourceAddress,
        uint targetAddress,
        CrossReferenceKind kind)
    {
        string? targetName = _items.TryGetValue(targetAddress, out AnalysisItem? target)
            ? target.Name
            : null;
        CrossReference reference = new(sourceAddress, targetAddress, kind, targetName);
        GetOrAddItem(sourceAddress).AddCrossReference(reference);
        if (!_incomingCrossReferences.TryGetValue(targetAddress, out List<CrossReference>? references))
        {
            references = [];
            _incomingCrossReferences.Add(targetAddress, references);
        }

        references.Add(reference);
        return reference;
    }

    public void AddDisassemblyLine(DisassemblyLine line)
    {
        ArgumentNullException.ThrowIfNull(line);
        _disassemblyLines.Add(line);
    }

    public void SetSystemModuleId(ushort moduleId)
    {
        if (!HasKnowledgeBase || !_knowledgeBaseModules.Any(module => module.Id == moduleId))
        {
            throw new ArgumentOutOfRangeException(nameof(moduleId), moduleId, "The module is not part of the loaded Knowledge Base.");
        }

        SystemModuleId = moduleId;
    }

    public AnalysisItem GetOrAddItem(uint address)
    {
        if (!_items.TryGetValue(address, out AnalysisItem? item))
        {
            item = new AnalysisItem(address);
            _items.Add(address, item);
        }

        return item;
    }
}
