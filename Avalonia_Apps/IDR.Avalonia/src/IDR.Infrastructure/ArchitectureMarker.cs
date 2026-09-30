using Iced.Intel;
using System;

namespace IDR.Infrastructure;

/// <summary>
/// Verifies that the infrastructure project references the selected decoder.
/// </summary>
public sealed class ArchitectureMarker
{
    public ArchitectureMarker()
    {
        Decoder = Decoder.Create(32, new ByteArrayCodeReader(Array.Empty<byte>()));
    }

    public Decoder Decoder { get; }
}
