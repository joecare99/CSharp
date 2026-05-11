# Ollama Tools Milestone 3

## Goal
Extend the tool layer so host applications can submit files and content items for AI-assisted evaluation across plain text, source code, and images.

## Scope
- add a reusable content analysis contract on top of the existing tool abstractions
- define request and result models for text, source code, and image analysis
- support file-backed and in-memory content inputs with explicit content type metadata
- standardize structured evaluation output with score, findings, confidence, and rationale
- add a first text analysis tool and a first C# source analysis tool
- prepare the architecture for later image analysis and multi-modal expansion
- add tests for contract mapping, validation, and result shaping

## Acceptance Criteria
- applications can describe content to analyze without coupling file access to model execution
- analysis requests support at least plain text, source code, and images as declared content kinds
- analysis results return a predictable structured envelope for scores, findings, and reasoning
- the first text and C# source analysis tools validate inputs and produce structured outputs
- the relevant projects build successfully and tests pass

## Proposed Architecture
- separate file acquisition from extracted content analysis
- introduce a content analysis request model with source, media type, declared language, and evaluation criteria
- introduce a content analysis result model with summary, score, confidence, findings, and suggested actions
- keep binary and image handling distinct from plain text extraction so later OCR or vision models can be added cleanly
- keep UI- or presentation-specific phrasing outside the reusable analysis core

## Planned Work Packages
1. define shared content analysis models and validation rules
2. add reusable tool contracts for content-oriented analysis tools
3. implement a first text evaluation tool
4. implement a first C# source evaluation tool
5. add tests for request validation and result formatting
6. add a sample that demonstrates file evaluation flow

## Notes
- this milestone should stay host-controlled and compatible with the current tool orchestration approach
- image analysis may initially use metadata-driven stubs or model-specific adapters until a stable vision path is added
- future milestones can add OCR, PDF extraction, repository-wide analysis, and richer code-language support

## Implementation Status
- completed: added shared content analysis request models for content kind, source kind, criteria, and request payloads
- completed: added shared content analysis result models for severity, findings, suggestions, and structured result envelopes
- completed: added structured request validation with stable issue codes and field-level validation details
- completed: added MSTest coverage for valid and invalid content analysis request scenarios
- completed: added a reusable `IContentAnalysisTool` contract for structured analysis tools
- completed: added `ContentAnalysisToolAdapter` to bridge structured analysis requests into the existing string-based tool workflow
- completed: added adapter and orchestrator integration tests for structured content analysis tools
- completed: added a first local `TextAnalysisTool` for heuristic plain-text evaluation
- completed: added dedicated tests for text analysis validation and result behavior
- completed: added a console sample for inline and file-based text analysis
- completed: added a WPF sample with MVVM and DI for interactive text analysis
- completed: added a first local `CSharpCodeAnalysisTool` for heuristic C# source analysis
- completed: added dedicated tests for C# source validation and result behavior
- completed: extended the console sample to switch between text and C# source analysis via `--csharp`
- completed: extended the WPF MVVM/DI sample to switch between text and C# source analysis

## Validation
- build: solution build completed successfully in Visual Studio
- tests: `Ollama.Tools.Tests` passed with 37/37 successful, 0 failed, 0 skipped
- samples: `Ollama.Samples.TextAnalysis` build succeeded and `Ollama.Wpf.TextAnalysis` build succeeded for both text and C# analysis integrations
