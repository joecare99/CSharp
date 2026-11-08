using System;

namespace AA98_AvlnCodeStudio.Editor.Services;

/// <summary>Allows a host to reserve a document identity for the duration of a save.</summary>
public interface ICoordinatedEditorSave
{
    Func<string, IDisposable>? AcquireSaveTarget { get; set; }
}
