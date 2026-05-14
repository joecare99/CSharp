# Backlog Item: Create Domain Model classes

## Feature Link
[Feature: Core Data Models and Persistence](../Features/Feat-04-PictureDBCoreModel.md)

## Platform Link
[Epic: OpenAI/Ollama Platform and Shared Client Ecosystem](../Epics/Epic-OpenAI-Ollama.md)

## Status
In Progress

## Description
Create the base models that define an image entry in the picture database. This includes its location, actual tags, and potentially its last analysis output from Ollama Vision models. 

## Tasks
- Define `ImageEntry` (Id, FilePath, DisplayName, Tags, Description, CapturedAt, ExtractedMetadata).
- Ensure models match C# base nullability requirements and are independent of DB/UI layers.
