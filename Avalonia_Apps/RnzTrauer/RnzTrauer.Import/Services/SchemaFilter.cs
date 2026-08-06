using System;
using System.Collections.Generic;

namespace RnzTrauer.Import.Services;

/// <summary>
/// Small, deterministic port of the Pascal <c>TBaseFilter</c> schema state machine.
/// Schema jump destinations intentionally use the legacy zero-based line numbering.
/// </summary>
public sealed class SchemaFilter : ISchemaFilter
{
    private readonly List<string> _schema = [];
    private bool _filterMode;
    private int _testLine;

    /// <inheritdoc />
    public bool FilterMode => _filterMode;

    /// <inheritdoc />
    public int TestLine => _testLine;

    /// <inheritdoc />
    public void SetSchema(IReadOnlyList<string> schema)
    {
        ArgumentNullException.ThrowIfNull(schema);
        _schema.Clear();
        _schema.AddRange(schema);
        Reset();
    }

    /// <inheritdoc />
    public void Reset()
    {
        _testLine = 0;
        _filterMode = false;
    }

    /// <inheritdoc />
    public IReadOnlyList<SchemaFilterEmission> Test(string token)
    {
        token ??= string.Empty;
        var emissions = new List<SchemaFilterEmission>();
        if (_testLine >= _schema.Count)
            return emissions;

        var line = _schema[_testLine];
        if (line.Length > 0 && line[0] == '+')
        {
            emissions.Add(new SchemaFilterEmission(0, line[1..]));
            _testLine++;
            return emissions;
        }

        if (line.Length > 0 && (line[0] == 'j' || line[0] == 'J'))
        {
            ProcessJump(token, emissions);
            return emissions;
        }

        var match = line.Length > 1 && StartsWithToken(token, line[1..]);
        if (match)
        {
            _filterMode = line[0] == '[';
            _testLine++;
            EmitFollowingOutput(emissions);
        }

        return emissions;
    }

    private void ProcessJump(string token, List<SchemaFilterEmission> emissions)
    {
        var line = _schema[_testLine];
        if (line.Length < 4 || !int.TryParse(line.Substring(1, 2), out var destination))
        {
            _testLine++;
            return;
        }

        var prefix = line[3..];
        if (StartsWithToken(token, prefix))
        {
            _testLine = Math.Clamp(destination, 0, _schema.Count);
            _filterMode = line[0] == 'J';
            EmitFollowingOutput(emissions);
            return;
        }

        while (_testLine < _schema.Count &&
               _schema[_testLine].Length > 0 &&
               (_schema[_testLine][0] == 'j' || _schema[_testLine][0] == 'J'))
            _testLine++;
    }

    private void EmitFollowingOutput(List<SchemaFilterEmission> emissions)
    {
        if (_testLine < _schema.Count && _schema[_testLine].Length > 0 && _schema[_testLine][0] == '+')
        {
            var line = _schema[_testLine];
            emissions.Add(new SchemaFilterEmission(0, line[1..]));
            _testLine++;
        }
    }

    private static bool StartsWithToken(string token, string prefix) =>
        (token + " ").StartsWith(prefix, StringComparison.OrdinalIgnoreCase);
}
