# Feature: Shared Ollama Platform Foundation

## Epic Link
[Epic: OpenAI/Ollama Platform and Shared Client Ecosystem](../Epics/Epic-OpenAI-Ollama.md)

## Status
In Progress

## Description
Provide the shared technical foundation for the solution, including protocol handling, public client APIs, tool orchestration, and dependency injection support. This feature groups the reusable libraries that multiple applications depend on.

## Goals
- Keep `Ollama.Protocol` as the low-level HTTP and serialization layer.
- Keep `Ollama.Client` as the public, model-scoped client layer.
- Keep `Ollama.Tools` as the reusable tool and content-analysis layer.
- Keep `Ollama.Extensions.DependencyInjection` as the composition layer for host applications.

## Known Backlog Items
- Backlog Item: Extend protocol and client support for additional Ollama request shapes (Done / In Progress depending on area)
- Backlog Item: Keep tool orchestration aligned with reusable platform abstractions (In Progress)
- Backlog Item: Add or update integration tests when the platform contract changes (Draft)
- Backlog Item: Maintain scenario links for text and picture applications (In Progress)
