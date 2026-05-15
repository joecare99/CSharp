# Task: Document.Pdf Adapter Wave 01

## Parent
- Related note: `DevOps/Features/Feat-05-PDFAnalysis.md`

## Goal
Define the primary role of `Document.Pdf` as a PDF writer and simple reader adapter, with optional importer-based extension for richer interpretation.

## Status
Draft

## Scope
- document the PDF writer responsibilities
- document the simple PDF reader responsibilities
- define how the adapter uses the shared document importer when injected
- keep OCR and LLM-heavy interpretation out of the simple adapter
- verify the project boundary against the shared model and base contracts

## Recommended Order
1. Review the existing PDF project responsibilities.
2. Document the simple writer/reader boundaries.
3. Define the importer injection point.
4. Keep advanced interpretation out of the adapter.
5. Validate the project boundary against the model and base layers.

## Assumptions
- the adapter should stay lightweight
- advanced interpretation belongs elsewhere
- the adapter may delegate to an importer when configured

## Exit Criteria
- the adapter role is clearly documented
- the project boundary is preserved
- advanced OCR/LLM logic is excluded from the simple adapter
