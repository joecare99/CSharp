# Task: Coverage Wave for ContentAnalysisToolAdapter 01

## Parent
- Related note: `DevOps/OllamaCoverageRound2.Info.md`

## Goal
Close the remaining uncovered branches in `ContentAnalysisToolAdapter` with focused tests around deserialization, validation failure, delegation, and execution results.

## Scope
- add MSTest coverage for adapter metadata delegation
- add MSTest coverage for empty input and JSON null handling
- add MSTest coverage for execution failure when validation fails
- keep production code unchanged unless a testability defect is found

## Assumptions
- existing `TestContentAnalysisTool` is sufficient for the remaining adapter branches
- this wave should stay local to `Ollama.Tools.Tests`

## Exit Criteria
- new adapter tests pass
- affected projects build successfully
- adapter coverage improves in the `Ollama.Tools` scoped coverage report
