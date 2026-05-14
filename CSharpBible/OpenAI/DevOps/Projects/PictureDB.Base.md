# Project: PictureDB.Base

## Status
In Progress

## Purpose
`PictureDB.Base` contains the reusable backend services and domain objects for the picture database scenario.

## Current Responsibilities
- Load images from a folder.
- Store picture-related domain data and analysis results.
- Define reusable model types for the picture database scenario.
- Provide backend services that can evolve toward persistence and analysis orchestration.

## Notable Types
- `ImageEntry`
- `ImageResult`
- `ImageLoader`
- `ImageProcessor`
- `LLMClient`
- `Categorizer`
- `Evaluator`
- `Sorter`
- `JsonResultStore`

## Notes
- This project currently acts as a backend processing pipeline, not only as a passive model library.
- The long-term picture database direction should probably separate domain models from orchestration services.
- The current implementation is still scenario-specific and should be refactored carefully once the PictureDB domain stabilizes.

## Related Planning Items
- [Epic: AI-Driven Picture Database (PictureDB)](../Epics/Epic-PictureDatabase.md)
- [Feature: Core Data Models and Persistence](../Features/Feat-04-PictureDBCoreModel.md)
- [Backlog Item: Create Domain Model classes](../BacklogItems/PBI-04-PictureDBModels.md)