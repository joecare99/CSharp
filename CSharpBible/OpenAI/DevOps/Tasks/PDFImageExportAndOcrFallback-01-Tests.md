# Task: PDF Image Export and OCR Fallback Verification Wave 01

## Parent
- Related note: `DevOps/BacklogItems/PBI-11-PdfImageExportAndOcrFallback.md`

## Goal
Add verification coverage for the image export and OCR fallback workflow so that the main-flow image export, Markdown links, and fallback decisions remain stable.

## Status
Draft

## Scope
- verify that image XObjects are enumerated and exported
- verify that Markdown links are emitted for exported images
- verify the internal image-description metadata shape
- verify the text-sufficiency heuristic behavior for meaningful and non-meaningful text
- verify the single full-page image shortcut
- verify that block-based OCR fallback is selected when text is insufficient

## Recommended Implementation Order
1. Add tests for image enumeration and export metadata.
2. Add tests for Markdown link rendering.
3. Add tests for the internal image-description metadata shape.
4. Add tests for the text-sufficiency heuristic.
5. Add tests for full-page image OCR routing.
6. Add tests for block-based OCR fallback routing.
7. Validate the updated test project and build.

## Assumptions
- deterministic test doubles are sufficient for the first wave
- the OCR engine itself can be mocked or isolated behind a contract
- no live model calls are required for verification

## Exit Criteria
- the new verification tests pass
- the relevant projects build successfully
- the fallback routing is covered for both success and fallback cases
