# Feature: Long-Term Memory and User Preference Store

## Epic Link
[Epic: Assistant Memory, PDF Intelligence, and Developer Automation](../Epics/Epic-AssistantMemoryAndDeveloperAutomation.md)

## Status
Draft

## Description
Introduce a durable memory model for user preferences, workspace context, and reusable assistant knowledge.

The goal is to support a limited but persistent "working memory" that can remember stable preferences and useful context across sessions without mixing it with transient conversation state.

## Goals
- Model user preferences explicitly and separately from temporary chat context.
- Store durable workspace knowledge that can be reused across sessions.
- Keep memory scope and retention rules transparent and controllable.
- Make memory accessible to assistant features such as routing, document analysis, and developer tools.

## Known Backlog Items
- [Backlog Item: Implement Long-Term Memory and User Preference Storage](../BacklogItems/PBI-06-LongTermMemoryAndPreferences.md)
- [Task: Long-Term Memory and Preference Wave 01](../Tasks/LongTermMemory-01.md)

## Open Questions
- Should preferences be stored in files, a database, or both?
- What should be considered durable versus transient memory?
- Should the assistant expose a user-visible review/edit workflow for stored preferences?
