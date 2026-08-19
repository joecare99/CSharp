using Ollama.CodingAgent.Models;
using Ollama.CodingAgent.Interfaces;
using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Ollama.CodingAgent.Application.Models;

/// <summary>
/// Represents one durable conversation turn visible to a client.
/// </summary>
public sealed class AgentConversationTurn : ObservableObject
{
    /// <summary>
    /// Gets or sets the turn author.
    /// </summary>
    public required AgentConversationRole Role { get; init; }

    /// <summary>
    /// Gets the presentation kind of this transcript entry.
    /// </summary>
    public AgentConversationEntryKind Kind { get; init; } = AgentConversationEntryKind.Message;

    /// <summary>
    /// Gets or sets the visible turn content.
    /// </summary>
    public string Content
    {
        get => _content;
        init => _content = value;
    }

    /// <summary>
    /// Marks this entry as currently receiving streamed data.
    /// </summary>
    public void StartLive()
    {
        IsLive = true;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the entry is expanded in the UI.
    /// </summary>
    public bool IsExpanded
    {
        get => _isExpanded;
        set => SetProperty(ref _isExpanded, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the entry is still receiving data.
    /// </summary>
    public bool IsLive
    {
        get => _isLive;
        private set => SetProperty(ref _isLive, value);
    }

    /// <summary>
    /// Gets or sets when the turn was created.
    /// </summary>
    public required DateTimeOffset CreatedAt { get; init; }

    private bool _isExpanded;
    private bool _isLive;
    private string _content = string.Empty;

    /// <summary>
    /// Updates the visible content while a streamed entry is active.
    /// </summary>
    /// <param name="content">The latest content.</param>
    public void UpdateContent(string content)
    {
        ArgumentNullException.ThrowIfNull(content);
        if (SetProperty(ref _content, content))
        {
            IsLive = true;
        }
    }

    /// <summary>
    /// Marks this entry as no longer receiving streamed data.
    /// </summary>
    public void Complete()
    {
        IsLive = false;
    }
}
