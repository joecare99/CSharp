using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Avln_ImageEditor.Host.ViewModels;

internal static class PngExportFile
{
    internal static async Task WriteNewAsync(string path, Func<Stream, CancellationToken, Task> write, CancellationToken cancellationToken = default)
    {
        string destination = Path.GetFullPath(path);
        string temporary = Path.Combine(Path.GetDirectoryName(destination)!, $".image-editor-{Guid.NewGuid():N}.tmp");
        bool created = false;
        try
        {
            await using (var stream = new FileStream(temporary, FileMode.CreateNew, FileAccess.Write, FileShare.None, 81920, useAsync: true))
            {
                created = true;
                await write(stream, cancellationToken);
                await stream.FlushAsync(cancellationToken);
            }
            cancellationToken.ThrowIfCancellationRequested();
            File.Move(temporary, destination, overwrite: false);
            created = false;
        }
        finally
        {
            if (created) File.Delete(temporary);
        }
    }
}
