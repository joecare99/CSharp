# Project: Ollama.Tools

## Status
In Progress

## Purpose
`Ollama.Tools` provides reusable orchestration, tool abstraction, and content analysis functionality on top of the shared Ollama client platform.

## Current Responsibilities
- Register and resolve tools.
- Build prompt instructions for tool execution.
- Execute tool loops and orchestration flows.
- Route content analysis requests to the correct analysis tool.
- Provide text and code analysis tools.

## Notable Types
- `ContentAnalysisRouter`
- `ContentAnalysisToolAdapter`
- `TextAnalysisTool`
- `CSharpCodeAnalysisTool`
- `ImageAnalysisTool`
- `OllamaToolRegistry`
- `OllamaToolOrchestrator`
- `OllamaToolLoopRunner`

## Related Planning Items
- [Epic: OpenAI/Ollama Platform and Shared Client Ecosystem](../Epics/Epic-OpenAI-Ollama.md)
- [Feature: Shared Ollama Platform Foundation](../Features/Feat-00-PlatformFoundation.md)
- [Epic: WPF App for Advanced Text Analysis and Processing](../Epics/Epic-WpfTextAnalysis.md)

## Planning Notes
- Reusable analysis and orchestration components should remain separate from document IO adapters.
- PDF OCR and LLM-heavy interpretation should not be added here if a dedicated document interpretation project is available.

## Boundary Notes
- Document IO belongs in the document projects, not in Ollama.Tools.
- Ollama.Tools should only consume document-analysis outputs when needed.