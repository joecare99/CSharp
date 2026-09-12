using System;
using System.Collections.Generic;
using ConsoleLib.CommonControls;

namespace ConsoleLib.Interfaces;

/// <summary>Application-scoped registry for open floating dialog sessions.</summary>
public interface IDialogManager
{
    IReadOnlyList<DialogSession> Sessions { get; }
    DialogSession? ActiveModal { get; }
    event EventHandler<DialogSession>? Opened;
    event EventHandler<DialogSession>? Activated;
    event EventHandler<DialogSession>? Closed;
    DialogSession Open(Dialog dialog, IControl? owner = null, bool modal = true);
    void Activate(DialogSession session);
    void Close(DialogSession? session);
    void CloseOwnedBy(IControl owner);
    void CloseAll();
}
