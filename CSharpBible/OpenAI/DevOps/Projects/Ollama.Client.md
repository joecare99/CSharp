# Project: Ollama.Client

## Status
In Progress

## Purpose
`Ollama.Client` provides the public, model-scoped client API for chat, generation, and embeddings.

## Current Responsibilities
- Create model-specific clients from a shared `OllamaClient`.
- Validate message and prompt input.
- Forward requests to the protocol layer.
- Aggregate streamed responses into buffered results.
- Support vision-style chat messages with attached images.

## Notable Types
- `OllamaClient`
- `OllamaChatClient`
- `OllamaGenerateClient`
- `OllamaEmbeddingClient`
- `ChatCompletionOptions`
- `GenerateOptions`
- `EmbeddingOptions`
- `OllamaClientChatMessage`

## Related Planning Items
- [Epic: OpenAI/Ollama Platform and Shared Client Ecosystem](../Epics/Epic-OpenAI-Ollama.md)
- [Feature: Shared Ollama Platform Foundation](../Features/Feat-00-PlatformFoundation.md)
- [Backlog Item: Integrate Vision capabilities into Ollama.Client](../BacklogItems/PBI-02-VisionClientSupport.md)