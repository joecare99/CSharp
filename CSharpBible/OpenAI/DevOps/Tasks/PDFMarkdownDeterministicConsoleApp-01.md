# Task: PDF-to-Markdown Deterministic Console App Wave 01

## Parent
- Related note: `DevOps/BacklogItems/PBI-09-PdfMarkdownDeterministicConsoleApp.md`

## Goal
Create the first console-based PDF-to-Markdown workflow that converts PDF text deterministically and leaves image analysis as a separate concern.

## Status
In Progress

## Scope
- create a console project in the repository root
- define the command-line contract and output behavior
- reuse the PDF extraction pipeline for the text portion
- produce Markdown that stays as close to the original text structure as practical
- add a separate hook or placeholder for image extraction / image analysis
- validate with sample PDFs from `DevOps/Samples/Pdf/`

## Recommended Implementation Order
1. Create the console project shell in the repository root.
2. Define the command-line contract for input and output files.
3. Reuse the existing PDF extraction pipeline for text.
4. Render deterministic Markdown from the extracted text.
5. Add a separate image-processing hook for later LLM analysis.
6. Validate the app against representative sample PDFs.

## Subtasks
1. [x] Create the console application project.
2. [x] Add CLI parsing for PDF input and Markdown output.
3. [x] Reuse the PDF extraction service for deterministic text extraction.
4. [x] Render the extracted text into Markdown.
5. [x] Add a placeholder or hook for image extraction/reporting.
6. [ ] Add sample PDFs under `DevOps/Samples/Pdf/`.
7. [x] Add verification steps for representative sample documents.
8. [x] Add a deterministic PDF inspection pass for metadata and text-operator hints.
9. [x] Add heuristics for XObject references and inline-image markers to better explain visible image content.
10. [x] Add font, XObject, and vector-drawing hints to the Markdown diagnostics.

## Follow-up Notes
- Next implementation phase should focus on a dedicated PDF structure inspector for XObjects, fonts, and encoding hints.

## Assumptions
- the console app stays in the repository root, not under `DevOps/Samples/`
- deterministic text conversion is the primary goal
- image analysis can be deferred to a later LLM-backed step

## Exit Criteria
- the project builds successfully
- the app can convert a sample PDF to Markdown
- the text path remains deterministic
- image handling is separated from text conversion
