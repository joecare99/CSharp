# Task: PDF Image Export and OCR Fallback Wave 01

## Parent
- Related note: `DevOps/BacklogItems/PBI-11-PdfImageExportAndOcrFallback.md`

## Goal
Implement the first end-to-end image export and OCR fallback path for PDF analysis so that images are always exported, Markdown links are generated, and text-poor documents can fall back to block rendering and OCR.

## Status
Draft

## Scope
- enumerate image XObjects and their source pages
- export image XObjects as separate files
- render Markdown links to the exported images
- generate internal image-analysis descriptors for each exported image
- implement the text-sufficiency check that triggers OCR fallback
- render only relevant drawing blocks at up to 600 dpi for OCR input
- support the single full-page image shortcut for direct OCR

## Recommended Implementation Order
1. Define the image export model and page association.
2. Add deterministic export of image XObjects.
3. Add Markdown rendering for the exported images.
4. Define the image-description result payload for internal LLM use.
5. Implement the text-sufficiency decision and OCR fallback routing.
6. Add the block renderer with a 600 dpi ceiling.
7. Add the single full-page image shortcut for direct OCR.
8. Validate the flow against representative sample PDFs.

## Assumptions
- the text path remains deterministic and stays the first output channel
- image export is always part of the main flow
- OCR should only run when text is not meaningful or not present, or for the full-page image shortcut
- the first iteration can keep the renderer limited to drawing blocks rather than full PDF fidelity

## Exit Criteria
- exported images appear as separate files
- Markdown links point to the exported images
- OCR fallback is triggered by the configured text-sufficiency rules
- the project builds successfully after the implementation
