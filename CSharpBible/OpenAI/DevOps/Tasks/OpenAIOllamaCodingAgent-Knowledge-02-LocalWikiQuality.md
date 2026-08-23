# Task: PBI-18 Local Wiki Quality and Retrieval

## Parent

- Backlog Item: [PBI-18 Web Knowledge and Local LLM Wiki](../BacklogItems/PBI-18-WebKnowledgeAndLocalLlmWiki.md)

## Status

Done

## Scope

- rank local wiki results by title, tags, and summary relevance
- enforce citation/write policy for curated entries
- validate entry consistency and bounded update behavior
- add retrieval and writeback regression tests

## Exit Criteria

- relevant entries are returned before weak matches
- entries with malformed or untrusted citations are rejected
- write and retrieval behavior is deterministic and session-safe

## Completion Log

- 2026-08-13: Added `LocalWikiWritePolicy` for required fields, summary bounds, and allowlisted HTTPS citations.
- 2026-08-13: Added deterministic ranking using exact phrase, title, tag, and summary relevance with stable ID tie-breaking.
- 2026-08-13: Added consistency and ranking regression tests; `Ollama.CodingAgent.Tests` passes 51 tests.
- 2026-08-13: Added `LocalWikiMarkdownImporter` for Obsidian-compatible Markdown vaults, including YAML `title`/`tags` extraction and frontmatter removal from indexed summaries.
- 2026-08-13: Ran the host check against `C:\Projekte\CSharp\CodeWikiVault`: 495 pages imported, 0 skipped; dependency-injection retrieval returned correctly titled pages.
