# Task: PDF to Document.Model Mapping Wave 01

## Parent
- Related note: `DevOps/BacklogItems/PBI-13-DocumentModelCanonicalStructure.md`

## Goal
Define the first deterministic mapping from PDF analysis output into `Document.Model` so that the importer can populate pages, blocks, resources, metadata, and analysis hints without depending on Markdown output.

## Status
Draft

## Mapping Scope
- map PDF source metadata into `DocumentModel`
- map PDF pages into `DocumentPageModel`
- map extracted text into `TextBlockModel`
- map image XObjects into `ImageBlockModel` and `DocumentResourceModel`
- map vector or drawing streams into `DrawingBlockModel`
- map OCR input and OCR results into document resources and image-block fields
- map heuristics and classifier output into `DocumentAnalysisHintModel`

## Recommended Mapping Order
1. Map document-level source information and metadata.
2. Map page ordering and page size information.
3. Map readable text into text blocks.
4. Map exported images and their references.
5. Map drawing blocks and OCR fallback artifacts.
6. Map analysis hints and confidence values.
7. Validate the mapping against representative PDF samples.

## Dependency Notes
- This mapping is expected to consume the import API defined in `PDFToDocumentModelImportApi-01`.
- The API should stay small and focused so the mapping remains deterministic and testable.

## Detailed Notes
- The mapping should be importer-driven and deterministic.
- The model should preserve source order so renderers can rebuild the document faithfully.
- Markdown should remain a renderer, not the primary representation.
- OCR artifacts should be attached as derived content, not as a replacement for the source structure.
- Image descriptions from LLM analysis should stay linked to the originating image artifact.

## Subtasks
1. Define the PDF source-to-model field mapping.
2. Define the page-to-block population rules.
3. Define image export and resource attachment rules.
4. Define OCR and analysis-hint attachment rules.
5. Validate the mapping against the current PDF analysis flow.

## Assumptions
- the PDF analyzer already exposes enough structure to seed the model
- the first wave can remain heuristic and deterministic
- the importer can evolve without changing the output model shape immediately

## Exit Criteria
- the mapping is documented
- the mapping is specific enough for implementation
- the mapping supports text, images, drawings, OCR, and analysis hints
