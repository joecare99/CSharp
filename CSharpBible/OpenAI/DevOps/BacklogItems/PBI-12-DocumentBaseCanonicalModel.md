# Backlog Item: Clarify Document.Base as the Minimal Shared Base Layer for Document Pipelines

## Feature Link
[Feature: PDF Analysis and Document Extraction](../Features/Feat-05-PDFAnalysis.md)

## Status
Draft

## Description
As a developer, I want `Document.Base` to remain a minimal shared base layer so that the richer document model can live in a separate `Document.Model` project while common interfaces, enums, and rudimentary base classes stay reusable across document pipelines.

## Why This Matters
- keeps `Document.Base` focused and lightweight
- separates shared contracts from the richer document object graph
- avoids turning the base project into a format-specific domain model
- leaves room for `Document.Model` to hold the canonical document structure

## Architectural Notes
- `Document.Base` should contain only interfaces, enums, and rudimentary base classes.
- the richer semantic model belongs in `Document.Model`.
- PDF-specific interfaces and classes should stay outside the base layer.

## Project Boundary Notes
- `Document.Base` is a shared contract layer, not a feature-rich model project.
- `Document.Model` holds the document-agnostic semantic model.
- `Document.Pdf` should remain a PDF IO adapter and only invoke advanced import logic when required.
- advanced OCR/LLM interpretation should stay in a separate project.

## Related Tasks
- [Task: Document.Base Contracts Wave 01](../Tasks/DocumentBaseContracts-01.md)

## Scope
- keep `Document.Base` limited to interfaces, enums, and only the most rudimentary base classes
- define the boundary between `Document.Base` and the future `Document.Model` project
- keep PDF-specific parsing in the PDF adapter layer
- keep Markdown as one renderer, not the primary model
- prepare the contract layer so the richer model can attach image export and OCR artifacts later

## Acceptance Criteria
- `Document.Base` remains a lean contract package rather than the full canonical model
- the boundary to a future `Document.Model` project is clearly documented
- PDF analysis can target the richer model without depending on Markdown
- the design supports later renderers such as Markdown and HTML
- the base layer remains suitable for reuse by OCR and image-analysis workflows

## Tasks
- define the model boundaries between `Document.Base` and `Document.Model`
- enumerate which contracts belong in the base layer
- define how PDF adapters map into the richer model
- define how renderers consume the richer model
- add tests or verification steps for the base contract surface and mapping assumptions
