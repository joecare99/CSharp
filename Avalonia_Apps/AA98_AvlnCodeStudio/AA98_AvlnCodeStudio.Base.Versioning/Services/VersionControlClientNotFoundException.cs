using System;
using System.ComponentModel;

namespace AA98_AvlnCodeStudio.Base.Versioning.Services;

/// <summary>Indicates that a version-control executable could not be started.</summary>
public sealed class VersionControlClientNotFoundException(string executable, Win32Exception innerException)
    : Exception($"Version-control client '{executable}' could not be found. Install it or make it available on PATH. {innerException.Message}", innerException)
{
}
