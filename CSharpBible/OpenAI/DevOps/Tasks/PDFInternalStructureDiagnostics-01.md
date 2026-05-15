# Task: PDF Internal Structure Diagnostics Wave 01

## Parent
- Related note: `DevOps/BacklogItems/PBI-10-PdfInternalStructureDiagnostics.md`

## Goal
Add deterministic inspection of the PDF's internal structure so that XObjects, fonts, and encoding hints can explain visible content that is not returned as extracted text.

## Status
In Progress

## Scope
- create a small structural-inspection abstraction
- identify XObject references and resource names
- identify font resources and basic font metadata
- detect encoding or ToUnicode-style hints when possible
- surface the diagnostic data in the Markdown output
- validate against representative sample PDFs

## Recommended Implementation Order
1. Define the structural inspection contract.
2. Enumerate XObjects and related resource names.
3. Enumerate font resources and capture available metadata.
4. Detect encoding and ToUnicode hints.
5. Render the diagnostic information into the Markdown output.
6. Validate the output against sample PDFs.

## Subtasks
1. [x] Define the PDF structure inspection model.
2. [x] Implement XObject enumeration.
3. [x] Implement font enumeration and metadata capture.
4. [x] Implement encoding hint detection.
5. [x] Add Markdown rendering for the structural diagnostics.
6. [x] Add verification steps with representative PDFs.
7. [x] Add a synthetic test case for the inspector heuristics.
8. [x] Add page-level and object-level diagnostic sections to the Markdown output.

## Assumptions
- the first version can use deterministic heuristics before deeper parser integration
- the structural diagnostics should stay separate from any future OCR or LLM-based semantic analysis
- the implementation should fit the current console-app workflow

## Exit Criteria
- the app reports internal PDF structure deterministically
- tests or verification steps cover at least one representative sample
- the console app still builds successfully
