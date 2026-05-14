# Ollama Dependency Injection Milestone 1

## Goal
Add a first dependency injection integration layer so the public Ollama client can be registered and consumed consistently in console, web, and desktop applications.

## Scope
- add `Ollama.Extensions.DependencyInjection`
- register `OllamaClient` through `IServiceCollection`
- support endpoint-based configuration
- support model-scoped registrations for chat, generate, and embedding clients
- add non-live tests for registration and resolution behavior
- document a minimal usage pattern

## Acceptance Criteria
- applications can register the root client with one service registration call
- applications can register named or default model-scoped feature clients through simple extension methods
- DI resolution works without manual protocol wiring
- tests verify registrations without requiring a live Ollama server
- the relevant projects build successfully and tests pass

## Notes
- this milestone intentionally stays focused on registration and resolution
- advanced named-client patterns can be added later if needed
