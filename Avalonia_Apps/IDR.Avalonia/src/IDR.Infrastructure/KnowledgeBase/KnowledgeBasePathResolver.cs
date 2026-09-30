using IDR.Core.Services;
using System;
using System.IO;

namespace IDR.Infrastructure.KnowledgeBase;

public sealed class KnowledgeBasePathResolver : IKnowledgeBasePathResolver
{
    private readonly string _directoryPath;

    public KnowledgeBasePathResolver(string directoryPath)
    {
        if (string.IsNullOrWhiteSpace(directoryPath))
        {
            throw new ArgumentException("A Knowledge Base directory is required.", nameof(directoryPath));
        }

        _directoryPath = Path.GetFullPath(directoryPath);
    }

    public string ResolvePath(DelphiVersion version)
    {
        if (version == DelphiVersion.Unknown || !Enum.IsDefined(version))
        {
            throw new ArgumentOutOfRangeException(nameof(version), version, "A supported Delphi version is required.");
        }

        return Path.Combine(_directoryPath, $"kb{(int)version}.bin");
    }
}
