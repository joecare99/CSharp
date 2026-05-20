# Ollama Protocol Milestone 2

## Goal
Extend the initial protocol layer with chat streaming and embeddings so higher-level clients can be built on stable endpoint coverage.

## Scope
- add `/api/chat` request and response contracts
- add `/api/embed` request and response contracts
- extend `OllamaProtocolClient` with chat streaming and embedding calls
- expand protocol tests for both new endpoints
- update the basic sample to use the chat API instead of only the generate API
- add dedicated console test programs for tags, generate, chat, and embed endpoint checks against a live Ollama instance

## Acceptance Criteria
- chat requests can be streamed through the protocol layer
- embedding requests return parsed vector data
- tests cover the new request and response handling
- the sample demonstrates the chat endpoint through the protocol client
- dedicated console programs make it easy to validate each protocol step against a real Ollama server
- the workspace builds successfully after the extension

## Notes
- this milestone still stays below the future Azure.AI-style client abstraction
- tool use and skills remain out of scope for this step
