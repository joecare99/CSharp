# Ollama Client Milestone 1

## Goal
Create the first public client layer on top of `Ollama.Protocol` with an API shape that starts to feel familiar to Azure.AI consumers while still staying honest to Ollama behavior.

## Scope
- add `Ollama.Client` as the first public API project
- add a root `OllamaClient`
- add feature clients for chat, generate, and embeddings
- expose buffered and streaming operations where the current protocol supports them
- add `Ollama.Client.Tests` with non-live behavioral tests
- update the basic sample to use the public client layer

## Acceptance Criteria
- consuming code no longer needs to reference `Ollama.Protocol` directly for the basic scenarios
- the public client exposes model-scoped feature clients
- chat and generate support streaming through the client layer
- embeddings can be requested through the client layer
- tests verify public client behavior without a live Ollama server
- the workspace builds successfully and the client tests pass

## Notes
- this milestone intentionally avoids tool use and skills
- the API should prefer async methods and explicit option objects where useful
- the surface should stay small until the protocol behavior is stabilized further
