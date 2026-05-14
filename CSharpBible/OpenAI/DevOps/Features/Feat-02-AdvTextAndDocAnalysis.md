# Feature: Advanced Text and Document Analysis Capabilities

## Epic Link
[Epic: WPF App for Advanced Text Analysis and Processing](../Epics/Epic-WpfTextAnalysis.md)

## Status
Draft

## Description
Evolve the `Ollama.Wpf.TextAnalysis` application and `Ollama.Tools` suite to introduce capabilities beyond basic verification or routing. Expand the local text toolbox to generate actual valuable derived textual data. 

## Goals
- Provide UI interactions that allow calling different explicit "Modes" (Summary, Extraction, Restructure).
- Introduce a 'SummaryTool' and 'RestructureTool' to `Ollama.Tools` that leverages local or LLM-backed heuristics.
- Integrate PDF textual extraction support to allow processing large document contexts.

## Known Backlog Items
- Backlog Item: Implement SummaryTool within Ollama.Tools (Draft)
- Backlog Item: Add PDF Extraction Capabilities (Draft)
- Backlog Item: Implement Language Translation / Reformatter Tool (Draft)
- Backlog Item: Add UI commands and interactions to execute specific new modes (Draft)