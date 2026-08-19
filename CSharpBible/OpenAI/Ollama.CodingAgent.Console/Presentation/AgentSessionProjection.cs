using Ollama.CodingAgent.Console.Configuration;
using Ollama.CodingAgent.Console.Commands;
using Ollama.CodingAgent.Console.Interfaces;
using Ollama.CodingAgent.Console.Infrastructure;
using Ollama.CodingAgent.Console.Presentation;
using Ollama.CodingAgent.Console.Services;
using Ollama.CodingAgent.Models;
using Ollama.CodingAgent.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using Ollama.CodingAgent.Application.Diagnostics;
using Ollama.CodingAgent.Application.Extensions;
using Ollama.CodingAgent.Application.Interfaces;
using Ollama.CodingAgent.Application.Models;
using Ollama.CodingAgent.Application.ViewModels;

namespace Ollama.CodingAgent.Console.Presentation;

/// <summary>
/// Projects UI-neutral agent session state into terminal text.
/// </summary>
public static class AgentSessionProjection
{
    /// <summary>
    /// Formats the visible transcript without exposing model thinking.
    /// </summary>
    public static string FormatTranscript(IEnumerable<AgentConversationTurn> conversation)
    {
        ArgumentNullException.ThrowIfNull(conversation);

        StringBuilder builder = new();
        foreach (AgentConversationTurn turn in conversation)
        {
            builder.Append('[')
                .Append(turn.CreatedAt.LocalDateTime.ToString("u"))
                .Append("] ")
                .Append(GetRoleLabel(turn.Role))
                .AppendLine(":");
            builder.AppendLine(turn.Content);
        }

        return builder.ToString().TrimEnd();
    }

    /// <summary>
    /// Formats a concise session status line.
    /// </summary>
    public static string FormatStatus(AgentSessionViewModel session)
    {
        ArgumentNullException.ThrowIfNull(session);
        return $"Session: {session.SessionId}{Environment.NewLine}"
            + $"Workspace: {session.WorkspacePath}{Environment.NewLine}"
            + $"Status: {session.Status}{Environment.NewLine}"
            + $"Turns: {session.Conversation.Count}{Environment.NewLine}"
            + $"Pending approvals: {session.PendingApprovals.Count}";
    }

    /// <summary>
    /// Formats pending approvals as complete reviewable records.
    /// </summary>
    public static string FormatApprovals(IReadOnlyList<AgentApprovalRequest> approvals)
    {
        ArgumentNullException.ThrowIfNull(approvals);
        if (approvals.Count == 0)
        {
            return "No pending approvals.";
        }

        StringBuilder builder = new();
        foreach (AgentApprovalRequest approval in approvals)
        {
            builder.Append("Id: ").AppendLine(approval.Id);
            builder.Append("Operation: ").AppendLine(approval.Operation);
            builder.Append("Created: ").AppendLine(approval.CreatedAt.LocalDateTime.ToString("u"));
            builder.AppendLine("Preview:");
            builder.AppendLine(approval.Preview);
            builder.AppendLine();
        }

        return builder.ToString().TrimEnd();
    }

    private static string GetRoleLabel(AgentConversationRole role)
        => role switch
        {
            AgentConversationRole.User => "user",
            AgentConversationRole.Assistant => "assistant",
            AgentConversationRole.System => "system",
            _ => "unknown",
        };
}
