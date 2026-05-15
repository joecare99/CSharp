# Backlog Item: Export PDF Images and Provide OCR Fallback for Insufficient Text

## Feature Link
[Feature: PDF Analysis and Document Extraction](../Features/Feat-05-PDFAnalysis.md)

## Status
Draft

## Description
As a user, I want the PDF analysis flow to always export images and, when the extracted text is not meaningful or not present, provide an OCR fallback for image-based or drawing-based content so that scanned documents and outline-heavy PDFs can still be analyzed.

The main flow should always produce formatted Markdown text, export images as separate files, and attach an internal image-analysis description for each exported image.

## Acceptance Criteria
- Text extraction always runs and is rendered as formatted Markdown.
- Missing spaces and line breaks are normalized as far as practical.
- All image XObjects are exported as separate files.
- The Markdown output links to the exported images.
- Each exported image receives an internal LLM-based description that includes drawing/photo, content, and face yes/no.
- If a PDF contains a single full-page image, the fallback OCR runs directly on that image.
- If text is missing or not meaningful, the workflow renders only the relevant drawing blocks up to 600 dpi maximum, depending on feature size, and applies OCR to those blocks.
- The OCR/renderer linkage remains internal to the workflow.

## Tasks
1. Define the image export contract and image-to-page association model.
2. Add deterministic export of image XObjects as separate files.
3. Render Markdown links for exported images.
4. Define the internal image-analysis result shape for drawing/photo/content/face classification.
5. Add a text-sufficiency heuristic that decides when OCR fallback is required.
6. Implement a block-based renderer fallback with a maximum of 600 dpi.
7. Add the special case for a single full-page image and direct OCR.
8. Add representative verification steps for image export and OCR fallback behavior.

## Refinement Notes
- Image export belongs to the main flow and should be available before any OCR fallback work starts.
- OCR fallback should be introduced only after the export and Markdown-linking flow is in place.
