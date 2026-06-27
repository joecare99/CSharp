# AA98-T052 Inspect Terminal Integration Sources

## Parent
- Backlog Item: `../BacklogItems/AA98-Bl041-Terminal-Inner-Loop-Baseline.md`

## Goal
Inspect existing terminal sources and decide the first AA98 terminal integration boundary.

## Scope
- Review terminal core and Avalonia terminal sources where available.
- Identify contracts, controls, and platform assumptions.
- Decide the minimum terminal scenario for self-hosting.

## Execution Notes
1. Preserve the split between terminal core and Avalonia UI assets.
2. Identify Linux shell/session assumptions.
3. Record recommended wrapper and host approach.

## Acceptance Criteria
- First terminal integration approach is documented.
- `AA98-T053` and `AA98-T054` have clear targets.

## Validation
- Inspection only; no code change required.

## Findings
- `Avln_TerminalHost` already separates terminal UI hosting, session coordination, and process launch behind `TerminalConsoleView`, `MainWindowViewModel`, `IProcessRunner`, and `IHostedProcess`.
- The existing host uses redirected standard input, output, and error streams with partial-output flushing, which is sufficient for a first inner-loop shell slice without introducing PTY-specific complexity.
- Shell discovery is currently Windows-oriented through `ComSpecLocator`, so Linux shell resolution must become an explicit AA98 configuration boundary instead of being inferred from the current sample.
- The reusable Avalonia console surface should stay separate from shell/process abstractions so that the first AA98 terminal wrapper can remain host-neutral and the micro host can stay thin.

## Recommended First Integration Boundary
- Introduce an AA98-facing terminal session wrapper that exposes session start, line input, stop, and output events through reusable contracts.
- Keep shell selection and process launch behavior behind explicit abstractions with Linux-oriented defaults supplied by configuration.
- Reuse the existing Avalonia console hosting pattern as a UI adapter layer instead of coupling process logic directly into the future workbench shell.
- Treat the first AA98 terminal slice as a stream-based shell host and defer PTY-specific behavior until the wrapper and host seams are stable.

## Follow-up Targets
- `AA98-T053` should implement the reusable AA98 terminal contracts and the first shell/session wrapper around configurable process-launch services.
- `AA98-T054` should create a thin `AA98.Terminal.Host` that composes the wrapper and exercises one configurable development-shell scenario.
- `AA98-T055` should add repeatable tests for shell resolution, session configuration, and non-interactive wrapper behavior, with manual smoke checks reserved for interactive shell validation.

## Status
- Completed
