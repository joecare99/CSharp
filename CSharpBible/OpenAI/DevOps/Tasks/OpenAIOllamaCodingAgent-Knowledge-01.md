# Task: OpenAI/Ollama Coding Agent Knowledge Integration Wave 01

## Parent
- Backlog Item: [PBI-18 Web Knowledge and Local LLM Wiki](../BacklogItems/PBI-18-WebKnowledgeAndLocalLlmWiki.md)

## Goal
Enable the coding agent to use trusted internet knowledge sources and build a local wiki corpus for reusable coding guidance.

## Scope
- define trusted-source retrieval policy and source metadata contract
- add web-lookup tool path for selected sources (Wikipedia, Rosetta Code, Microsoft Learn)
- implement local wiki write/read workflow with quality gates
- add tests for source filtering, citation, and local wiki consistency

## Existing Project Integration
- Extend `Ollama.CodingAgent` delegated toolset with bounded knowledge tools.
- Keep reusable tool interfaces in `Ollama.Tools` when source-agnostic.
- Reuse `McpTools` where connector abstraction already exists.

## Recommended Implementation Order
1. Define source allowlist and citation contract.
2. Implement one source adapter end-to-end.
3. Add local wiki storage schema and write policy.
4. Add retrieval ranking across local wiki entries.
5. Add focused tests and one live host check scenario.

## Subtasks
1. Add knowledge-source policy model and domain validation.
2. Add `web_lookup` delegated tool with citation envelope output.
3. Add `local_wiki_write` and `local_wiki_search` delegated tools.
4. Add normalization strategy for wiki pages (title, tags, summary, links).
5. Add tests for source filtering, citation shape, and retrieval quality.

## Assumptions
- external retrieval remains bounded by explicit allowlist and timeout policies
- local wiki content is curated summaries, not full page mirroring
- agent responses should cite used external or local knowledge entries

## Exit Criteria
- at least one trusted external source is integrated end-to-end
- local wiki entries can be created and retrieved through delegated tools
- tests cover allowlist/citation/wiki consistency behavior
- one host-check scenario demonstrates real lookup + local wiki writeback flow

## Status
Done

## Status Log
- 2026-08-13: Added `Ollama.CodingAgent.HostCheck.InternetInfo` for real web-source checks (Wikipedia/RosettaCode/MS Learn paths) including malformed input/output handling.
- 2026-08-13: Added `Ollama.CodingAgent.HostCheck.KnowledgeBase` and initial `LocalKnowledgeBaseStore` for local wiki write/search flows with malformed data validation checks.
- 2026-08-13: Executed both new host checks; internet host fetched Wikipedia summary successfully and malformed source/query/output checks behaved as expected, knowledge-base host validated write/search and malformed-entry/database handling.
- 2026-08-13: Hardened knowledge-base host check to use a per-run temp database path, preventing malformed-database test residue from affecting subsequent runs.
- 2026-08-13: Completed web-knowledge slice with typed `WebKnowledgeCitation`/`WebKnowledgeLookupResult` envelopes and strict HTTPS host validation for Wikipedia, Rosetta Code, and Microsoft Learn.
- 2026-08-13: Added source filtering, citation shape, URL construction, and blocked-host tests; `Ollama.CodingAgent.Tests` passes 48 tests.
