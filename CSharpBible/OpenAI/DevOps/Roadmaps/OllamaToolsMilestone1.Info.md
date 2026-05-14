# Ollama Tools Milestone 1

## Goal
Create the first reusable tool-use layer on top of the Ollama client so applications can register callable tools and orchestrate a simple request-response loop.

## Scope
- add `Ollama.Tools`
- define tool contracts and result envelopes
- add a tool registry abstraction
- add a first orchestration service for a host-driven tool loop
- add `Ollama.Tools.Tests` with non-live behavior tests
- add a console sample that demonstrates tool registration and execution flow

## Acceptance Criteria
- applications can define and register tools with a predictable contract
- the tool registry can resolve tools by name
- the orchestration service can invoke registered tools through a simple host-controlled loop
- tests verify registry and orchestration behavior without a live Ollama server
- the sample demonstrates the first end-to-end developer workflow for tools
- the relevant projects build successfully and tests pass

## Notes
- this milestone intentionally starts with a host-controlled tool loop
- model-native tool-calling conventions can be layered on top later
