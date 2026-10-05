using System.IO;
using AhnWin52Backup.Core.Workflows;

namespace AhnWin52Backup.Core.Abstractions;

public interface IHejInspectionService
{
    HejInspection Inspect(Stream source);
}
