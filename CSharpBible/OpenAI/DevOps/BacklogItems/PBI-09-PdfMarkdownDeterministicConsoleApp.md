# Backlog Item: Add a Deterministic PDF-to-Markdown Console App

## Feature Link
[Feature: PDF Analysis and Document Extraction](../Features/Feat-05-PDFAnalysis.md)

## Status
In Progress

## Description
As a developer, I want a console application that deterministically converts a PDF into a Markdown document so that I can test the extraction pipeline without relying on an LLM for the text portion.

The app should keep text conversion deterministic and reserve any later LLM usage for image analysis only.

## Acceptance Criteria
- A console project exists in the solution root.
- The app accepts a PDF file path and writes Markdown output.
- The text portion is converted without LLM involvement.
- Images or image references are handled as a separate, optional concern.
- The app can be exercised with sample PDFs stored under `DevOps/Samples/Pdf/`.

## Tasks
- Create the console application project in the solution root.
- Define the command-line contract and output path behavior.
- Reuse the PDF extraction pipeline for deterministic text conversion.
- Add Markdown rendering for page or section structure.
- Add a separate path for image extraction or image-reference reporting.
- Add manual sample inputs under `DevOps/Samples/Pdf/`.
- Add tests or verification steps for representative PDF samples.
