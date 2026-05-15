# Backlog Item: Inspect PDF Internal Structure for XObjects and Fonts

## Feature Link
[Feature: PDF Analysis and Document Extraction](../Features/Feat-05-PDFAnalysis.md)

## Status
Draft

## Description
As a developer, I want the PDF analysis workflow to inspect internal PDF structure such as XObjects, fonts, and encoding hints so that visible content that is not directly extracted as text can still be explained deterministically.

## Acceptance Criteria
- The workflow can report XObject references and their names.
- The workflow can report font resources and basic font metadata.
- The workflow can surface encoding or ToUnicode-style hints when present.
- The Markdown output includes a diagnostic section for internal structure.
- The inspection remains deterministic and does not depend on an LLM for the structural analysis.

## Tasks
- Add a PDF inspector abstraction for structural diagnostics.
- Enumerate XObject references and related resource names.
- Enumerate font resources and extract available metadata.
- Detect basic encoding hints and ToUnicode-style mappings when present.
- Render the structural data into the Markdown output.
- Add verification with representative sample PDFs.
