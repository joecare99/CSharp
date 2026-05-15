# Task: Document.Model Canonical Structure Planning Wave 01

## Parent
- Related note: `DevOps/BacklogItems/PBI-13-DocumentModelCanonicalStructure.md`

## Goal
Prepare the first detailed model plan for a separate `Document.Model` project that will hold the canonical document structure for PDF analysis, image export, OCR fallback, and future renderers.

## Status
Draft

## Plan
1. Define the canonical document boundary.
2. Decide which concepts belong in `Document.Base` versus `Document.Model`.
3. Model pages, blocks, resources, metadata, and analysis hints.
4. Define how image exports and OCR outputs attach to the shared model.
5. Define how Markdown and future renderers consume the model.
6. Validate the plan against the current PDF analysis workflow.

## Detailed Planning Notes
- `Document.Base` should remain a contract layer.
- `Document.Model` should carry the document graph.
- PDF parsing should produce the model, not replace it.
- Markdown should be a renderer of the model.
- OCR output should be attached as derived content or annotations, not as a replacement for the source structure.
- Image descriptions from LLM analysis should stay linked to the originating image artifact.

## Subtasks
1. Document the canonical entity boundaries.
2. Define the page and block relationships.
3. Define resource and artifact attachment rules.
4. Define renderer-facing output contracts.
5. Review the plan against the existing PDF analysis flow.

## Assumptions
- `Document.Model` will be a new project
- the first iteration can evolve the existing interfaces rather than replacing them immediately
- format-specific behavior stays outside `Document.Base` when possible

## Exit Criteria
- the model boundary is documented
- the layout is ready for implementation planning
- the design can support Markdown, HTML, and OCR-related outputs
