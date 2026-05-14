# Feature: Ollama Vision Prototyping and Sandbox

## Epic Link
[Epic: AI-Driven Picture Database (PictureDB)](../Epics/Epic-PictureDatabase.md)

## Platform Link
[Epic: OpenAI/Ollama Platform and Shared Client Ecosystem](../Epics/Epic-OpenAI-Ollama.md)

## Status
In Progress

## Description
Set up and use the `PictureDB.OllamaTest` console application to evaluate how well local Ollama vision models (like LLaVA) can describe, tag, and analyze the specific types of images expected in the database.

## Goals
- Connect `PictureDB.OllamaTest` to the existing `Ollama.Client`.
- Send sample images to the model and retrieve descriptions.
- Extract structured data (e.g., JSON arrays of tags) from the model's text response.
- Establish the prompt engineering baseline needed for reliable cataloging.

## Known Backlog Items
- Backlog Item: Integrate `Ollama.Client` into `PictureDB.OllamaTest` (In Progress)
- Backlog Item: Implement basic vision prompt loop for single image testing (Draft)


