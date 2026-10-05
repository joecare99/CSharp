using System.IO;
using AhnWin52Backup.Core.Hej;

namespace AhnWin52Backup.Core.Abstractions;

public interface IHejWriter
{
    void Write(HejDocument document, Stream destination);
}
