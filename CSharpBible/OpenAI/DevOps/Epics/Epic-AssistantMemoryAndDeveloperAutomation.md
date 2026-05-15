# Epic: Assistant Memory, PDF Intelligence, and Developer Automation

## Status
Draft

## Description
This epic covers the next planning phase for assistant-oriented capabilities in the Ollama platform. The focus is on document intelligence, long-term working memory, user preference modeling, MCP-based tool integration, and practical software development tools.

The intent is to evolve the workspace from analysis-centric samples into a more capable assistant foundation that can:
- read and extract knowledge from PDFs and related documents,
- remember durable user preferences and reusable context,
- expose capabilities through an MCP-compatible interface,
- support developer workflows such as code assistance, repository inspection, and task automation.

## Goals
1. Add PDF analysis and extraction capabilities as a first-class document scenario.
2. Design a long-term working memory model for durable user context and preferences.
3. Plan and implement an MCP interface for tool discovery and agent integration.
4. Provide developer-focused tools that support software engineering tasks in the workspace.
5. Keep the solution aligned with the existing shared Ollama platform and multi-targeted .NET structure.

## Boundaries
- This epic is about assistant capabilities and not about a single UI product.
- Persistent memory must be designed with explicit user consent and clear scope boundaries.
- PDF processing should be focused on text extraction and document understanding before any advanced layout reconstruction.
- MCP integration should reuse existing tool abstractions where practical.

## Known Sub-Features
- Feature: PDF Analysis and Document Extraction
- Feature: Long-Term Memory and User Preference Store
- Feature: MCP Interface and Tool Exposure
- Feature: Software Development Assistance Tools

## Open Questions
- Should memory be stored per user, per workspace, or per solution?
- Should PDF analysis live primarily in `Ollama.Tools` or in a dedicated document-processing layer?
- Which MCP transport(s) should be supported first?
- What minimal set of software-development tools should be considered essential for the first release?

## Related Platform Link
- [Epic: OpenAI/Ollama Platform and Shared Client Ecosystem](Epic-OpenAI-Ollama.md)
