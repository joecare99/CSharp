# Backlog Item: Integrate Vision capabilities into Ollama.Client

## Feature Link
[Feature: Ollama Vision Prototyping and Sandbox](../Features/Feat-03-VisionPrototyping.md)

## Status
Done

## Description
Ollama supports sending images to vision models (like llava) by attaching base64-encoded strings to the chat prompt. The underlying models `OllamaChatMessage` and `OllamaClientChatMessage` need an `Images` array to support this capability.

## Tasks
- Extend `Ollama.Protocol.Models.OllamaChatMessage` with an `IList<string>? Images` property.
- Extend `Ollama.Client.Models.OllamaClientChatMessage` with an `IReadOnlyList<string>? Images` property.
- Ensure `OllamaChatClient` maps `Images` from the client model to the protocol model during request creation.
- Add an `Images` validation step in `OllamaChatClient` (e.g. at least one image if the list is present, no empty strings).
