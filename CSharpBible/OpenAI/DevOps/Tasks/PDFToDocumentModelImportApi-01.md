# Task: PDF to Document.Model Import API Wave 01

## Parent
- Related note: `DevOps/BacklogItems/PBI-13-DocumentModelCanonicalStructure.md`

## Goal
Define the first small import API that PDF adapters can call to populate `Document.Model` without knowing the full internal implementation of the document graph.

## Status
Draft

## Why an API First
- keeps PDF adapters thin
- separates extraction logic from model mutation details
- makes the mapping deterministic and testable
- allows future importers to share the same entry points

## Proposed API Scope
- create a document import source contract for source metadata
- create a document model writer contract for page-level and block-level append operations
- create a source-agnostic document importer contract that consumes the source model and populates the writer
- create text-block append operations
- create image/resource append operations
- create drawing-block append operations
- create analysis-hint append operations
- keep the API focused on model population, not rendering

## Recommended Order
1. Define the import contract surface.
2. Define page and block append operations.
3. Define resource and analysis-hint operations.
4. Define validation and error handling behavior.
5. Review the API against the planned PDF-to-model mapping.

## Related API Drafts
- [Task: PDF to Document.Model Import API Interface Sketch Wave 01](../Tasks/PDFToDocumentModelImportApi-01-InterfaceSketch.md)

## Naming Note
- Because the shared importer contract is intended to work across document types, the short-term PDF-specific name should be avoided in the shared API.
- A source-agnostic name such as `IDocumentModelImporter` is preferred for the shared contract.
- PDF-specific importer implementations can live in `Document.Pdf` and remain registered through DI.
- `Document.Pdf` should remain primarily a PDF writer and simple reader adapter.
- OCR and LLM-heavy interpretation should live in a separate project and only be wired in when the richer importer is used.

## Project Split Notes
- `Document.Base` should stay minimal and contain only shared contracts and low-level base types.
- `Document.Model` should stay source-agnostic and contain the semantic document graph.
- `Document.Pdf` should stay focused on PDF IO, with advanced interpretation only enabled through the importer contract.
- OCR and LLM orchestration should be planned as a separate project so the simple PDF adapter remains small.

## Detailed Notes
- The API should be small enough to call from a PDF importer directly.
- The API should avoid exposing mutable internals of the model.
- The API should preserve source order and source references.
- The API should not know about Markdown, HTML, or OCR internals beyond attachment points.
- The shared contract should be agnostic to document source type.
- PDF-only behavior belongs in the `Document.Pdf` project or in a separate advanced interpretation project.

## Subtasks
1. Draft the import contract.
2. Draft the document and page append operations.
3. Draft the resource and analysis-hint append operations.
4. Draft the error handling and validation assumptions.
5. Review the API against the mapping task.

## Assumptions
- the importer will populate the model in one pass where possible
- the model remains renderer-neutral
- the API should be compatible with the planned PDF adapter flow

## Exit Criteria
- the API is documented
- the API is small and deterministic
- the API supports the PDF-to-model mapping task
