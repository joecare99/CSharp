# Backlog Item: Add Web Knowledge Retrieval and Local LLM Wiki

## Feature Link
[Feature: OpenAI/Ollama Coding Agent Platform](../Features/Feat-09-OpenAIOllamaCodingAgent.md)

## Status
Done

## Description
As an engineer, I want the coding agent to retrieve trusted external knowledge (for example Wikipedia, Rosetta Code, Microsoft Learn) and maintain a local LLM wiki so that coding decisions are grounded in references and reusable local knowledge.

## Acceptance Criteria
- The agent can execute bounded web-lookup tasks through explicit tools/connectors.
- Source allowlist and citation metadata are enforced for external knowledge usage.
- A local wiki store is available for curated summaries, patterns, and validated snippets.
- Retrieval ranking and write policies for the local wiki are defined and tested.

## Primary Project Targets
- `Ollama.CodingAgent` (knowledge orchestration and policies)
- `Ollama.Tools` (web/wiki tool contracts where reusable)
- `McpTools` (optional bridge for external fetch connectors)

## Tasks
- Define trusted source policy and allowlist for web retrieval.
- Implement one web lookup abstraction with citation envelope.
- Implement local wiki store schema and update workflow.
- Add retrieval + write-back policy tests.

## Test Tasks
- Add tests for allowlist enforcement and blocked-domain behavior.
- Add tests for citation format and local wiki entry consistency.
- Add tests for retrieval relevance ordering from local wiki entries.

## Dependencies
- Depends on: [PBI-16](./PBI-16-ToolExecutionAndMemoryIntegration.md)

## Open Questions
- Should wiki entries be plain markdown, structured JSON, or hybrid records?
- Which freshness policy is required for external knowledge sources?

## Delivery Slices

1. **Web knowledge and citation hardening** - in progress/completed for the first implementation slice: trusted source lookup, typed citation envelope, HTTPS host allowlist, and contract tests.
2. **Local wiki ranking and writeback quality** - next slice: relevance ordering, citation/write policy, consistency tests, and readiness documentation.

## Status Log

- 2026-08-13: Refined PBI-18 into two delivery slices to isolate external retrieval safety from local wiki quality behavior.
- 2026-08-13: Completed the web-knowledge slice with typed citation metadata, bounded previews, and strict allowlisted HTTPS hosts.
- 2026-08-13: Completed the local-wiki slice with deterministic relevance ranking across title, tags, and summary, plus citation/write consistency policy.
- 2026-08-13: Added local-wiki ranking, trusted-citation rejection, and citation-free curated-entry tests; `Ollama.CodingAgent.Tests` passes 51 tests.
- 2026-08-13: Validated the importer against the real `CodeWikiVault`: all 495 Markdown pages were imported without skips, with YAML frontmatter titles and tags normalized correctly.
