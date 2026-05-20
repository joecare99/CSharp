# Epic: WPF App for Advanced Text Analysis and Processing

## Status
In Progress (Partial Done, Draft for future scopes)

## Description
This epic focuses on creating a specialized WPF-based Tool and Client for advanced textual content processing. It abstracts textual operations such as plain text analysis, summarization, structural extraction, translation, and code-specific operations into specialized tool requests, while leaving media operations (such as image analysis) to a separate domain app.

## Goals
1. Establish a foundational desktop platform (`Ollama.Wpf.TextAnalysis`) for evaluating text tools.
2. Abstract content and text handling behind a unified, polymorphic service layer (`ContentAnalysisService` and `ContentAnalysisRouter`).
3. Expand routing capabilities for subsequent NLP actions such as Summarization, Extraction, Translation and general textual reformatting.
4. Separate concern between different content types: Keep this app heavily focused on Text/Code/PDFs and route media/image analysis tools into their own scope.

## Boundaries
- Requires `net8.0-windows` and `CommunityToolkit.Mvvm` as established conventions.
- Image processing (apart from potential embedded text / OCR) remains out of scope here.
- Must remain aligned with existing `Ollama.Client` and `Ollama.Tools` abstraction layer.

## Platform Link
[Epic: OpenAI/Ollama Platform and Shared Client Ecosystem](Epic-OpenAI-Ollama.md)

## Known Sub-Features
- Feature: Initial WPF Architecture & Router (Done)
- Feature: Basic Text and C# Code Analysis Implementations (Done)
- Feature: Advanced Text Services (Draft: Summarize, Extract, Reformat)
- Feature: PDF Analysis integration (Draft)
- Feature: Assistant Memory and Developer Tooling Planning (Draft)

## Open Questions
- Do we need an additional parser dependency for PDF (like iText7 or PdfPig) or is basic extraction sufficient?
- Should the ContentAnalysisRouter expand horizontally automatically or be dynamically loaded via reflection?