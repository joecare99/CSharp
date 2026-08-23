# Backlog Item: Approval-Gated Git Workspace Provider

## Feature Link

[Feature: OpenAI/Ollama Coding Agent Platform](../Features/Feat-09-OpenAIOllamaCodingAgent.md)

## Status

Done

## Description

As an operator, I want the coding agent to inspect local Git workspaces and propose every Git mutation through the shared approval service, so repository changes remain explicit, reviewable, and credential-free.

## Acceptance Criteria

- A dedicated Git provider discovers supported local repositories and exposes status, bounded diff previews, local branches, and sanitized remotes.
- Typed operations support stage, unstage, branch creation/switching, commit, fetch, pull, and push.
- Every mutation, including network mutations, creates one exact structured preview and waits for `IAgentApprovalService`.
- Invalid workspaces, unsafe refs, invalid paths, and unresolved merge/conflict states fail before an operation can mutate a repository.
- No credentials are accepted, stored, logged, or included in remote output.

## Completion Log

- 2026-08-14: Added dedicated `Ollama.CodingAgent.Git` and `Ollama.CodingAgent.Git.Tests` projects using centrally managed LibGit2Sharp.
- 2026-08-14: Added read-only workspace discovery, status, bounded diff, branch, and credential-sanitized remote contracts.
- 2026-08-14: Added typed approval-gated local Git mutations with validation for paths, refs, conflicts, and cancellation.
- 2026-08-14: Validated local temporary-repository tests for inspection, preview bounds, denied/approved staging, invalid branches, and cancellation.
