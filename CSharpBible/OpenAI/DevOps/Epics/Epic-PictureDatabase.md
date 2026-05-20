# Epic: AI-Driven Picture Database (PictureDB)

## Platform Link
[Epic: OpenAI/Ollama Platform and Shared Client Ecosystem](Epic-OpenAI-Ollama.md)

## Status
In Progress

## Description
This epic focuses on creating a picture database (`PictureDB`) that leverages local AI vision models (via Ollama) to automatically analyze, caption, and tag images. This aligns with the strategy to handle media/image analysis in a dedicated domain, strictly separated from pure text processing.

## Goals
1. Establish a foundational database and model layer in `PictureDB.Base` using built-in or otherwise solution-approved image handling for image normalization and metadata extraction.
2. Provide a WPF frontend (`PictureDB.UI`) for browsing, searching, and viewing images alongside their AI-generated metadata.
3. Use `PictureDB.OllamaTest` as a sandbox to evaluate and tune the Ollama vision capabilities (e.g., LLaVA or similar models) before integrating them into the main service layer.
4. Establish an `ImageAnalysisTool` or `VisionService` that uses `Ollama.Client` to perform the actual inferences.

## Boundaries
- Target framework is `net8.0` / `net8.0-windows`.
- MVVM pattern via `CommunityToolkit.Mvvm` in the UI.
- Use `Microsoft.Extensions.Hosting` for dependency and lifecycle management in the DB/UI layers.
- Prefer system-library-based image processing for the console sandbox if it remains sufficient and keeps the dependency footprint small.
- Text analysis remains out of scope here; focus strictly on image data, OCR (if applicable through vision), and metadata generation.

## Known Sub-Features
- Feature: Ollama Vision Prototyping (Draft)
- Feature: Core Data Models and Persistence (Draft)
- Feature: WPF Image Gallery and Metadata View (Draft)

## Open Questions
- Which persistence layer should be used for the PictureDB? (e.g., SQLite, LiteDB, or a simple JSON file initially?)
- How do we handle batch processing of images? Vision requests can be computationally heavy.
