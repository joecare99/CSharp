# Task: Coverage Wave for ContentAnalysisRouter 01

## Parent
- Related note: `DevOps/OllamaCoverageRound2.Info.md`

## Goal
Close the uncovered `ContentAnalysisRouter` surface in `Ollama.Tools` with focused tests that verify routing decisions and execution behavior.

## Scope
- add MSTest coverage for explicit routing modes
- add MSTest coverage for automatic C# detection
- add MSTest coverage for request normalization
- add MSTest coverage for validation failure and successful execution paths

## Assumptions
- direct tests with the real `TextAnalysisTool` and `CSharpCodeAnalysisTool` provide sufficient coverage without production changes
- this wave should stay local to `Ollama.Tools.Tests`

## Open Questions
- whether later waves should also isolate tool interaction through additional test doubles if router behavior expands

## Exit Criteria
- new router tests pass
- relevant projects build successfully
- the router area is covered by the updated `Ollama.Tools.Tests` run
