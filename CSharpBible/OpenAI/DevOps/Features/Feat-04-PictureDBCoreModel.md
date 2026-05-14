# Feature: Core Data Models and Persistence

## Epic Link
[Epic: AI-Driven Picture Database (PictureDB)](../Epics/Epic-PictureDatabase.md)

## Platform Link
[Epic: OpenAI/Ollama Platform and Shared Client Ecosystem](../Epics/Epic-OpenAI-Ollama.md)

## Status
In Progress

## Description
Develop the core domain models representing an image and its AI-generated metadata within `PictureDB.Base`. Establish a mechanism to save and load this data.

## Goals
- Define domain models: `ImageEntry`, `ImageTag`, `AnalysisResult`.
- Use `SixLabors.ImageSharp` to extract basic EXIF/metadata (dimensions, date taken).
- Define the persistence abstraction (`IPictureRepository`).

## Known Backlog Items
- Backlog Item: Create domain model classes (Draft)
- Backlog Item: Implement basic file-based or SQLite repository (Draft)
- Backlog Item: Implement metadata extraction service using ImageSharp (Draft)
