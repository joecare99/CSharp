# Project: PictureDB.UI

## Status
In Progress

## Purpose
`PictureDB.UI` is the WPF user interface for the picture database scenario.

## Current Responsibilities
- Start the WPF application with host-based dependency injection.
- Resolve picture processing services from `PictureDB.Base`.
- Let the user select a folder.
- Run image analysis and show progress/results in the UI.
- Persist analysis output to JSON.

## Current Relationship to the Platform
- Uses the shared Ollama client platform indirectly through `PictureDB.Base` services.
- Represents the user-facing shell for a future picture catalog and AI metadata experience.

## Architecture Notes
- Uses `CommunityToolkit.Mvvm` for the ViewModel layer.
- Uses `Microsoft.Extensions.Hosting` for composition and lifecycle management.
- Keeps the UI layer thin and delegates processing to backend services.

## Notable Types
- `App`
- `MainWindow`
- `MainViewModel`

## Related Planning Items
- [Epic: AI-Driven Picture Database (PictureDB)](../Epics/Epic-PictureDatabase.md)
- [Feature: Core Data Models and Persistence](../Features/Feat-04-PictureDBCoreModel.md)