# Backlog Item: Integrate Ollama.Client into PictureDB.OllamaTest

## Feature Link
[Feature: Ollama Vision Prototyping and Sandbox](../Features/Feat-03-VisionPrototyping.md)

## Platform Link
[Epic: OpenAI/Ollama Platform and Shared Client Ecosystem](../Epics/Epic-OpenAI-Ollama.md)

## Status
Done

## Description
Rewrite `PictureDB.OllamaTest\Program.cs` to ditch the CLI `Process.Start` approach and use the newly upgraded `Ollama.Client` implementation. It should accept an image path, load it as Base64, construct an `OllamaClientChatMessage` with the image, and stream the response to the console.

## Tasks
- Replace `Program.cs` logic with `OllamaClient` chat streaming.
- Read image bytes and encode as Base64.
- Make CLI accept Model, ImagePath, and Prompt.


