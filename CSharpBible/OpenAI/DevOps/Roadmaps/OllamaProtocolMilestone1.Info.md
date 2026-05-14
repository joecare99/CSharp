# Ollama Protocol Milestone 1

## Goal
Create the first reusable Ollama protocol layer as a standalone project with a minimal sample and dedicated tests.

## Scope
- `Ollama.Protocol` class library
- `Ollama.Protocol.Tests` MSTest project
- `Ollama.Samples.BasicChat` console sample
- support for model listing and generate streaming as the initial feature set

## Acceptance Criteria
- The protocol layer hides raw HTTP details from consuming code.
- The sample project can stream thinking and response output through the protocol layer.
- The protocol test project covers request and response serialization plus streaming parsing basics.
- The workspace builds successfully after the new projects are added.

## Notes
- This milestone intentionally stops before Azure.AI-style feature clients and tool use.
- The current experimental console code is treated as seed behavior to extract into the protocol layer.
