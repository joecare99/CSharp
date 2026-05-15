# Epic: OpenAI/Ollama Platform and Shared Client Ecosystem

## Status
In Progress

## Description
This epic covers the shared platform for all Ollama-based applications in this solution. It includes the protocol layer, public client layer, reusable tooling layer, and the application scenarios that build on top of them.

The purpose is to provide a consistent foundation for:
- local LLM chat, generation, and embedding access
- tool orchestration and content analysis
- WPF and console sample applications
- specialized domain scenarios such as text analysis and picture analysis

## Goals
1. Maintain a reusable `Ollama.Protocol` layer for low-level HTTP access to Ollama endpoints.
2. Maintain a reusable `Ollama.Client` layer for safe, model-scoped application usage.
3. Maintain a reusable `Ollama.Tools` layer for orchestration, content analysis, and tool execution.
4. Provide sample applications that demonstrate how the shared layers can be used in practice.
5. Keep media/image analysis and text analysis as separate application concerns while still sharing the same platform.

## Boundaries
- This epic is about the shared Ollama platform, not a single product scenario.
- Picture analysis, text analysis, and other future scenarios are separate application areas beneath this platform.
- Changes should remain compatible with the current multi-targeted .NET workspace.

## Known Sub-Epics and Scenario Epics
- Epic: WPF App for Advanced Text Analysis and Processing
- Epic: AI-Driven Picture Database (PictureDB)

## Known Platform Areas
- `Ollama.Protocol`
- `Ollama.Client`
- `Ollama.Tools`
- `Ollama.Extensions.DependencyInjection`
- Console and WPF sample applications

## Known Scenario Areas
- `Ollama.Wpf.TextAnalysis`
- `PictureDB.Base`
- `PictureDB.UI`
- `PictureDB.OllamaTest`
- `AssistantMemory` planning epic for PDF, long-term memory, MCP, and developer tools

## Open Questions
- Should the shared platform expose a formal application shell abstraction for all sample apps?
- Do we want a more explicit package/versioning strategy for reusable Ollama libraries?
- Should PictureDB remain a separate domain epic or be represented as a feature set under the platform epic?

## Related Project Notes
- [Project: Ollama.Client](../Projects/Ollama.Client.md)
- [Project: Ollama.Protocol](../Projects/Ollama.Protocol.md)
- [Project: Ollama.Tools](../Projects/Ollama.Tools.md)
- [Project: Ollama.Wpf.TextAnalysis](../Projects/Ollama.Wpf.TextAnalysis.md)