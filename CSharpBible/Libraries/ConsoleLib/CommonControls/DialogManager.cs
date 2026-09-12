using System;
using System.Collections.Generic;
using System.Linq;
using ConsoleLib.Interfaces;

namespace ConsoleLib.CommonControls;

/// <summary>Maintains all floating dialog sessions for one application.</summary>
public sealed class DialogManager : IDialogManager
{
    private readonly List<DialogSession> _sessions = new();
    private long _nextZIndex;

    public IReadOnlyList<DialogSession> Sessions => _sessions;
    public DialogSession? ActiveModal => _sessions.LastOrDefault(session => session.IsModal && !session.IsClosed);

    public event EventHandler<DialogSession>? Opened;
    public event EventHandler<DialogSession>? Activated;
    public event EventHandler<DialogSession>? Closed;

    public DialogSession Open(Dialog dialog, IControl? owner = null, bool modal = true)
    {
        if (dialog is null)
            throw new ArgumentNullException(nameof(dialog));
        if (modal && ActiveModal is not null)
            throw new InvalidOperationException("Only one modal dialog can be open at a time.");

        if (dialog.Parent is null && owner is IGroupControl ownerGroup)
            ownerGroup.Add(dialog);

        var session = new DialogSession(Guid.NewGuid(), dialog, owner, modal);
        _sessions.Add(session);
        dialog.Show();
        Activate(session);
        Opened?.Invoke(this, session);
        return session;
    }

    public void Activate(DialogSession session)
    {
        if (session is null)
            throw new ArgumentNullException(nameof(session));
        if (session.IsClosed || !_sessions.Contains(session))
            throw new InvalidOperationException("The dialog session is not open.");

        session.ZIndex = ++_nextZIndex;
        if (session.Dialog.Parent is IGroupControl group)
            group.BringToFront(session.Dialog);
        session.Dialog.Active = true;
        Activated?.Invoke(this, session);
    }

    public void Close(DialogSession? session)
    {
        if (session is null || session.IsClosed)
            return;
        if (!_sessions.Remove(session))
            return;

        session.Dialog.Hide();
        session.Dialog.Parent?.Remove(session.Dialog);
        session.IsClosed = true;
        Closed?.Invoke(this, session);
    }

    public void CloseOwnedBy(IControl owner)
    {
        if (owner is null)
            throw new ArgumentNullException(nameof(owner));

        foreach (var session in _sessions.Where(session => ReferenceEquals(session.Owner, owner)).ToArray())
            Close(session);
    }

    public void CloseAll()
    {
        foreach (var session in _sessions.ToArray())
            Close(session);
    }
}
