# Task: Coverage Wave for ContentAnalysis Image and Text 01

## Parent
- Related note: `DevOps/Roadmaps/OllamaToolsMilestone3.Info.md`

## Goal
Close the remaining high-value coverage gaps in `Ollama.Tools` for the `TextAnalysisTool` and `ImageAnalysisTool` validation and analysis paths.

## Scope
- add MSTest coverage for `TextAnalysisTool` null, inline, and file-path validation branches
- add MSTest coverage for `ImageAnalysisTool` validation branches
- add MSTest coverage for at least one structured image-analysis result path
- keep production code unchanged unless a testability defect is discovered

## Assumptions
- the existing heuristic tools are sufficient for unit-level coverage without live Ollama calls
- coverage improvements should remain local to `Ollama.Tools.Tests`

## Exit Criteria
- new tests pass
- affected projects build successfully
- the targeted content-analysis branches are covered by the updated `Ollama.Tools.Tests` run