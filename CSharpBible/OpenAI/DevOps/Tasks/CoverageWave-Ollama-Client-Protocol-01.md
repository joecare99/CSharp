# Task: Coverage Wave for Ollama Client and Protocol 01

## Parent
- Related note: `DevOps/OllamaCoverageRound2.Info.md`

## Goal
Increase automated test coverage for the current Ollama client and protocol layers with a focused first wave that targets high-value uncovered paths without broad production changes.

## Scope
- add targeted MSTest coverage for `Ollama.Client`
- add targeted MSTest coverage for `Ollama.Protocol` stream readers
- keep production code unchanged unless a testability defect is discovered
- validate with build, tests, and the reusable coverage workflow

## Assumptions
- the reusable coverage script at `C:\Projekte\CSharp\Tools\Skills\TestCoverage\Invoke-TestProjectCoverage.ps1` remains the preferred measurement path
- a green result requires explicit passed/skipped/failed visibility, where only fully green tests are considered good
- the best first-wave value comes from low-risk tests around overload forwarding, validation guards, and stream reader edge behavior

## Open Questions
- whether later waves should optimize for overall scoped coverage or per-assembly thresholds
- whether protocol client transport tests should be expanded after the lighter reader and client gaps are closed

## Planned Test Tasks
1. extend `Ollama.Client.Tests` for uncovered overload and guard paths
2. extend `Ollama.Protocol.Tests` for NDJSON reader edge paths
3. rerun focused tests and coverage

## Exit Criteria
- new tests pass
- affected projects build successfully
- coverage is remeasured with the reusable script
- resulting test status clearly states passed, skipped, and failed counts
