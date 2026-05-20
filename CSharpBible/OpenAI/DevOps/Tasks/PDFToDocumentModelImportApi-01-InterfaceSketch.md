# Task: PDF to Document.Model Import API Interface Sketch Wave 01

## Parent
- Related note: `DevOps/Tasks/PDFToDocumentModelImportApi-01.md`

## Goal
Sketch the first import API that a PDF adapter can call to populate `Document.Model` without exposing internal model collections or rendering concerns.

## Draft API Shape

### Import entry point
- `IDocumentModelImporter`
  - `DocumentModel Import(DocumentImportSourceModel source)`

### Importer responsibilities
- read the source document
- extract source metadata and page order
- produce pages, blocks, resources, and analysis hints
- stay deterministic and renderer-neutral
- delegate all model mutation to the writer

### Naming guidance
- The shared contract should use a source-agnostic name.
- PDF-specific implementations can live in a separate project and still implement the shared contract.
- The `Document.Pdf` project should contain the PDF adapter and related PDF-only helpers.
- Advanced interpretation, OCR, and LLM-based analysis should live in a separate project and be wired in only when needed.

### Document.Pdf role
- primarily write PDF documents
- read simple PDFs directly when possible
- optionally delegate advanced import/interpretation to `IDocumentModelImporter`
- avoid hosting OCR or LLM-heavy logic in the simple IO adapter

### Source input
- `DocumentImportSourceModel`
  - source path / source name / media type
  - optional metadata payload

### Model population contract
- `IDocumentModelWriter`
  - `SetMetadata(...)`
  - `AddPage(...)`
  - `AddTextBlock(...)`
  - `AddImageBlock(...)`
  - `AddDrawingBlock(...)`
  - `AddResource(...)`
  - `AddAnalysisHint(...)`

### Model types
- `DocumentModel`
- `DocumentPageModel`
- `TextBlockModel`
- `ImageBlockModel`
- `DrawingBlockModel`
- `DocumentResourceModel`
- `DocumentAnalysisHintModel`

## Design Notes
- The importer should stay thin and deterministic.
- The importer should be the only layer that reads PDF structure.
- The writer should be the only layer allowed to mutate the model.
- The writer should preserve source order and source references.
- The writer should not know anything about Markdown, HTML, or OCR rendering.
- The API should remain compatible with future non-PDF importers.

## Suggested Minimum Responsibilities
- the importer extracts structure from the PDF
- the writer translates structure into the document model
- the model stays renderer-neutral
- the API supports text, image, drawing, resource, and hint population

## Questions to Resolve
- Should the writer return created nodes or only accept input payloads?
- Should blocks be created by type-specific methods or via a generic append method?
- Should the import source include page geometry and object references up front, or should those be added incrementally?
- Should the API expose a validation result before the document is finalized?

## Exit Criteria
- the API surface is documented
- the responsibilities are split between importer and model writer
- the design is specific enough to start implementation planning
