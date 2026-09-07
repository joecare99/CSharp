using System;
using System.Collections.Generic;
using Terminal.Core;

namespace ConsoleLib.Showcase.Terminal.Core;

/// <summary>Converts provider-neutral terminal snapshots into displayable text rows.</summary>
public sealed class TerminalSnapshotRenderer
{
    public IReadOnlyList<string> Render(TerminalSnapshot snapshot)
    {
        if (snapshot is null)
            throw new ArgumentNullException(nameof(snapshot));

        var rows = new string[snapshot.Size.Rows];
        for (var row = 0; row < rows.Length; row++)
        {
            var cells = row < snapshot.Lines.Count ? snapshot.Lines[row] : Array.Empty<TerminalCell>();
            var chars = new char[snapshot.Size.Columns];
            for (var column = 0; column < chars.Length; column++)
                chars[column] = column < cells.Count ? cells[column].Character : ' ';
            rows[row] = new string(chars);
        }

        return rows;
    }
}
