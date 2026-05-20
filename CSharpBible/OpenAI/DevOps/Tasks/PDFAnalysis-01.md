# Task: PDF Analysis and Extraction Wave 01

## Parent
- Related note: `DevOps/BacklogItems/PBI-05-PDFAnalysis.md`

## Goal
Deliver the first reusable PDF extraction slice that can read text from a PDF and make it available to the analysis pipeline.

## Status
In Progress

## Scope
- define a small PDF extraction abstraction and the minimal result model
- choose PdfPig as the first PDF backend and add its dependency
- implement a first extraction adapter for the current solution
- capture minimal metadata needed for analysis and diagnostics
- wire the extracted text into the downstream analysis pipeline
- add focused unit tests for a successful sample document and an error or empty-result path

## Recommended Implementation Order
1. Define the extraction contract and result model.
2. Choose the first PDF backend and add its dependency.
3. Implement the adapter against that concrete PDF backend.
3. Add metadata capture so extraction results are traceable.
4. Connect the extracted text to the analysis flow.
5. Add tests for success, empty output, and failure paths.

## Subtasks
1. [x] Specify the PDF extraction interface and result DTOs.
2. [x] Decide the first PDF backend and identify required dependencies.
3. [x] Implement extraction for a minimal sample PDF.
4. [x] Capture source path, page count, and extraction diagnostics.
5. [x] Wire the extracted text into the existing analysis pipeline.
6. [x] Add tests for the contract and result model.
7. [x] Add tests for successful extraction.
8. [x] Add tests for empty, invalid, or unsupported PDF inputs.
9. [x] Add tests for file metadata capture and metadata merging behavior.
10. [x] Add tests for routing extracted PDF text through the content analysis pipeline.

## Follow-up Backlog
- Add a small console application in the repository root for deterministic PDF extraction and Markdown generation tests after the current sprint closes.

## Assumptions
- the first step should prioritize deterministic extraction over layout reconstruction
- PDF processing should remain reusable for later summarization and memory scenarios
- production code should stay minimal until the extraction contract is stable

## Exit Criteria
- extraction is implemented behind an abstraction
- tests pass
- the affected project builds successfully
- the extracted text can be consumed by the content-analysis layer
