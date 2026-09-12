using System;
using ConsoleLib.Interfaces;

namespace ConsoleLib.CommonControls;

/// <summary>Identity and lifecycle state for one open floating dialog.</summary>
public sealed class DialogSession
{
    internal DialogSession(Guid id, Dialog dialog, IControl? owner, bool isModal)
    {
        Id = id;
        Dialog = dialog;
        Owner = owner;
        IsModal = isModal;
    }

    public Guid Id { get; }
    public Dialog Dialog { get; }
    public IControl? Owner { get; }
    public bool IsModal { get; }
    public bool IsClosed { get; internal set; }
    public long ZIndex { get; internal set; }
}
