using System;
using System.Collections.Generic;
using System.IO;
using OFBCreator.Abstractions.Interfaces;

namespace OFBCreator.Core.Services;

/// <summary>
/// Default implementation of IFamilyDataSourceRegistry using a simple registration list.
/// Supports runtime registration and auto-detection via CanRead probing.
/// </summary>
public class DefaultFamilyDataSourceRegistry : IFamilyDataSourceRegistry
{
    private readonly List<IFamilyDataSource> _providers = new();

    /// <inheritdoc />
    public void RegisterProvider(IFamilyDataSource provider)
    {
        ArgumentNullException.ThrowIfNull(provider);

        // Remove existing provider with same ID (idempotent registration)
        var index = _providers.FindIndex(p => p.SourceId == provider.SourceId);
        if (index >= 0)
            _providers.RemoveAt(index);

        _providers.Add(provider);
    }

    /// <inheritdoc />
    public IFamilyDataSource? GetProvider(string sourceId)
    {
        if (string.IsNullOrWhiteSpace(sourceId))
            return null;

        return _providers.Find(p => p.SourceId == sourceId);
    }

    /// <inheritdoc />
    public IReadOnlyList<IFamilyDataSource> ListProviders()
    {
        return _providers.AsReadOnly();
    }

    /// <inheritdoc />
    public IFamilyDataSource? DetectProvider(Stream stream)
    {
        if (stream == null || !stream.CanSeek)
            throw new ArgumentException("Stream must support seeking for detection.", nameof(stream));

        var originalPosition = stream.Position;

        try
        {
            // Peek at first few bytes/characters to detect format
            var peekLength = Math.Min(4096, stream.Length - stream.Position);
            if (peekLength <= 0)
                return null;

            var buffer = new byte[peekLength];
            stream.Read(buffer, 0, buffer.Length);
            stream.Position = originalPosition; // Reset position for actual read

            foreach (var provider in _providers)
            {
                try
                {
                    if (provider.CanRead(stream))
                        return provider;
                }
                catch
                {
                    // Skip providers that throw during detection
                    continue;
                }
            }

            return null;
        }
        finally
        {
            stream.Position = originalPosition;
        }
    }
}
