namespace RepoMigrator.Core.Diagnostics;

/// <summary>
/// Provides binary-exact file copy operations with immediate read-back verification.
/// </summary>
public static class VerifiedFileCopy
{
    /// <summary>
    /// Copies a file and verifies that the destination bytes match the source bytes.
    /// </summary>
    /// <param name="sSourcePath">The source file path.</param>
    /// <param name="sDestinationPath">The destination file path.</param>
    public static void CopyAndVerify(string sSourcePath, string sDestinationPath)
    {
        File.Copy(sSourcePath, sDestinationPath, overwrite: true);

        var sourceInfo = new FileInfo(sSourcePath);
        var destinationInfo = new FileInfo(sDestinationPath);
        if (sourceInfo.Length != destinationInfo.Length)
            throw new IOException($"Verified copy failed for '{sDestinationPath}': source length {sourceInfo.Length} differs from destination length {destinationInfo.Length}.");

        if (!AreFilesContentEqual(sSourcePath, sDestinationPath, sourceInfo.Length))
            throw new IOException($"Verified copy failed for '{sDestinationPath}': destination content differs from source content.");
    }

    private static bool AreFilesContentEqual(string sSourcePath, string sDestinationPath, long iLength)
    {
        if (iLength == 0)
            return true;

        const int iBufferSize = 128 * 1024;
        var arrSourceBuffer = new byte[iBufferSize];
        var arrDestinationBuffer = new byte[iBufferSize];

        using var sourceStream = new FileStream(sSourcePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        using var destinationStream = new FileStream(sDestinationPath, FileMode.Open, FileAccess.Read, FileShare.Read);

        while (true)
        {
            var iReadSource = sourceStream.Read(arrSourceBuffer, 0, arrSourceBuffer.Length);
            var iReadDestination = destinationStream.Read(arrDestinationBuffer, 0, arrDestinationBuffer.Length);

            if (iReadSource != iReadDestination)
                return false;

            if (iReadSource == 0)
                return true;

            if (!arrSourceBuffer.AsSpan(0, iReadSource).SequenceEqual(arrDestinationBuffer.AsSpan(0, iReadDestination)))
                return false;
        }
    }
}
