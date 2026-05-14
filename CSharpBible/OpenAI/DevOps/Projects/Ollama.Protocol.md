# Project: Ollama.Protocol

## Status
In Progress

## Purpose
`Ollama.Protocol` provides the low-level HTTP and JSON protocol access for Ollama endpoints.

## Current Responsibilities
- Call Ollama REST endpoints.
- Deserialize tags, chat, generate, and embedding responses.
- Stream NDJSON style response chunks.
- Represent protocol request and response models.

## Notable Types
- `OllamaProtocolClient`
- `OllamaProtocolClientOptions`
- `OllamaChatRequest`
- `OllamaChatMessage`
- `OllamaGenerateRequest`
- `OllamaEmbedRequest`

## Related Planning Items
- [Epic: OpenAI/Ollama Platform and Shared Client Ecosystem](../Epics/Epic-OpenAI-Ollama.md)
- [Feature: Shared Ollama Platform Foundation](../Features/Feat-00-PlatformFoundation.md)