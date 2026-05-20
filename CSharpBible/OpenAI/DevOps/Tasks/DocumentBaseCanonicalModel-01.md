# Task: Document.Base Boundary Planning Wave 01

## Parent
- Related note: `DevOps/BacklogItems/PBI-12-DocumentBaseCanonicalModel.md`

## Goal
Prepare the first detailed boundary plan for keeping `Document.Base` as a minimal shared contract layer while a future `Document.Model` project carries the richer canonical document representation for PDF analysis, image export, and OCR fallback.

## Status
Draft

## Plan
1. Define the boundary between `Document.Base` and `Document.Model`.
2. Decide which contracts belong in `Document.Base`.
3. Decide which document entities belong in `Document.Model`.
4. Define how PDF-specific adapters map into the richer model.
5. Define how Markdown and future renderers consume the richer model.
6. Validate the plan against the current PDF analysis workflow.

## Detailed Planning Notes
- `Document.Base` should remain intentionally small and stable.
- `Document.Model` should hold the logical document structure.
- PDF parsing should produce the richer model, not replace it.
- Markdown should be a renderer of the model.
- OCR output should be attached as derived content or annotations, not as a replacement for the source structure.
- Image descriptions from LLM analysis should stay linked to the originating image artifact.

## Subtasks
1. Document the base-layer contract boundary.
2. Define the candidate `Document.Model` entity set.
3. Define mapping rules from PDF analysis into the richer model.
4. Define renderer-facing output contracts.
5. Review the plan against the existing PDF analysis flow.

## Assumptions
- the current `Document.Base` project will remain the shared contract project
- the first iteration can evolve the existing interfaces rather than replacing them immediately
- format-specific behavior stays outside the base layer when possible

## Exit Criteria
- the base-layer boundary is documented
- the richer model boundary is ready for implementation planning
- the design can support Markdown, HTML, and OCR-related outputs
