# Backlog Item: Introduce Document.Model as the Canonical Document Structure

## Feature Link
[Feature: PDF Analysis and Document Extraction](../Features/Feat-05-PDFAnalysis.md)

## Status
Draft

## Description
As a developer, I want a separate `Document.Model` project to hold the canonical document structure so that PDF analysis, image export, OCR, and future renderers can all operate on the same richer document graph without overloading `Document.Base`.

## Why This Matters
- keeps the base layer minimal and reusable
- provides one shared document graph for PDF, OCR, and future document sources
- allows Markdown, HTML, JSON, and assistant workflows to render the same structure
- creates a clear home for pages, blocks, resources, and analysis hints

## Scope
- define the canonical document entities in a new `Document.Model` project
- represent pages, blocks, resources, metadata, and analysis hints
- keep PDF-specific parsing outside the model project
- keep Markdown and other outputs as renderers of the model
- attach image export and OCR artifacts to the model

## Acceptance Criteria
- a separate `Document.Model` project exists in the planning scope
- pages, text blocks, image blocks, drawing blocks, metadata, and resources can be represented
- PDF analysis can map into the shared model without depending on Markdown
- the design supports later renderers such as Markdown and HTML
- the model can carry OCR and image-analysis artifacts

## Tasks
- define the canonical entity set for `Document.Model`
- define relationships between pages, blocks, and resources
- define the import API surface that PDF adapters will call
- define how PDF adapters map into the model
- define how image export and OCR artifacts attach to the model
- add verification steps for the model shape and mapping assumptions

## Architectural Notes
- `Document.Model` must stay agnostic to document type.
- shared semantic interfaces belong here; PDF-specific interfaces and classes belong elsewhere.
- `Document.Pdf` can be used as the simple reader/writer adapter and can invoke a richer importer through DI when available.
- OCR and LLM processing should remain outside the simple PDF adapter.

## Project Boundary Notes
- `Document.Base` holds the minimal shared contracts and rudimentary base classes.
- `Document.Model` holds the semantic document graph and shared model contracts.
- `Document.Pdf` is the primary PDF IO project and should only perform advanced interpretation when an importer is injected.
- OCR and LLM-heavy analysis should be modeled as a separate project in the planning scope.

## Related Tasks
- [Task: Document.Model Core Wave 01](../Tasks/DocumentModelCore-01.md)

## Related Tasks
- [Task: PDF to Document.Model Import API Interface Sketch Wave 01](../Tasks/PDFToDocumentModelImportApi-01-InterfaceSketch.md)
- [Task: PDF to Document.Model Import API Wave 01](../Tasks/PDFToDocumentModelImportApi-01.md)
- [Task: PDF to Document.Model Mapping Wave 01](../Tasks/PDFToDocumentModelMapping-01.md)
