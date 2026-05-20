# Task: Document.Model Class Layout Wave 01

## Parent
- Related note: `DevOps/Tasks/DocumentModelCanonicalStructure-01.md`

## Goal
Draft the first concrete class layout for `Document.Model` so the shared document graph can represent documents, pages, blocks, resources, and analysis metadata.

## Status
Draft

## Proposed Class Layout

### Core document model
- `Document`
  - source information
  - metadata
  - pages
  - attachments / derived artifacts

- `DocumentPage`
  - page number
  - size / bounds
  - blocks
  - page-level resources

### Block model
- `DocumentBlock` as the base abstraction
  - block kind
  - bounds
  - source reference
  - optional analysis notes

- `TextBlock`
  - text content
  - layout / line-break info
  - language or script hints

- `ImageBlock`
  - exported image reference
  - original object reference
  - OCR result reference
  - image analysis result reference

- `DrawingBlock`
  - vector or drawing instructions
  - source stream reference
  - render hints

### Supporting model
- `DocumentMetadata`
  - title
  - author
  - creation and modification data
  - source file metadata

- `DocumentResource`
  - exported image files
  - rendered block images
  - OCR artifacts
  - analysis outputs

- `DocumentAnalysisHint`
  - text meaningful yes/no
  - image type
  - face yes/no
  - confidence
  - origin

## Layout Notes
- keep the model renderer-neutral
- keep PDF parser details out of the core types where possible
- prefer explicit composition over large inheritance trees
- preserve the existing document concepts that already live in `Document.Base` if they are still useful

## Validation Questions
- should `DrawingBlock` be part of the first release or introduced after image export?
- should OCR results be attached to blocks directly or via a separate artifact collection?
- should the base model expose mutable collections or controlled append methods?

## Exit Criteria
- the proposed class layout is documented
- the layout is specific enough for implementation work
- the layout matches the planned PDF-to-document pipeline
