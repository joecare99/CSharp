using IDR.Core.Models;
using IDR.Core.Services;
using System;
using System.Linq;
using System.Text;

namespace IDR.Cli;

public sealed class DisassemblyEmitter
{
    public string Emit(AnalysisSession session)
    {
        ArgumentNullException.ThrowIfNull(session);
        StringBuilder output = new();
        output.AppendLine($"; IDR disassembly: {session.SourcePath}");
        output.AppendLine($"; Delphi: {session.SelectedDelphiVersion}; ImageBase: 0x{session.ImageBase:X8}");
        output.AppendLine();
        foreach (DisassemblyLine line in session.DisassemblyLines.OrderBy(line => line.Address))
        {
            string branch = line.BranchTarget is uint target
                ? $" ; -> RVA 0x{target:X8} / VA 0x{session.ImageBase + target:X8}"
                : string.Empty;
            output.AppendLine(
                $"0x{line.Address:X8}  {line.Bytes,-24} {line.FormattedText}{branch} ; {line.FlowControl}");
        }

        return output.ToString();
    }
}
