namespace OFBCreator.Publishing.Models;

/// <summary>Describes a host-neutral status or warning emitted during publication.</summary>
public sealed record OFBExportProgress(string Message, bool IsWarning = false);
