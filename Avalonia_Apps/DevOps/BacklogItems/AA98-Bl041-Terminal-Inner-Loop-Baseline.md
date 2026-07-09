# AA98-Bl041 Terminal Inner Loop Baseline

## Parent
- Feature: `../Features/AA98-F42-Developer-Inner-Loop.md`
- Epic: `../Epics/AA98-E11-Linux-Self-Hosting.md`

## Value
A developer can use a terminal session as part of the AA98 self-hosting inner loop.

## Scope
- Reuse terminal core and Avalonia terminal assets where possible.
- Define a thin terminal host for isolated validation.
- Validate Linux shell process behavior.
- Keep terminal integration separate from shell-specific layout decisions.

## Acceptance Criteria
- A terminal component can be validated independently.
- Linux process/session assumptions are documented and tested where possible.
- The terminal component can later be composed into the workbench shell.

## Implementation Tasks
- `AA98-T052 Inspect Terminal Integration Sources`
- `AA98-T053 Implement Terminal Component Wrapper`
- `AA98-T054 Create Terminal Micro Host`
- `AA98-T055 Add Terminal Inner Loop Tests`

## Assumptions
- Terminal assets from sibling sources should be adapted rather than reimplemented.

## Open Questions
- Which Linux shell should be the default development terminal?

## Next Refinement Steps
1. Inspect available terminal contracts and controls.
2. Define the smallest terminal host scenario.

## Progress Notes
- Source inspection is complete: `Avln_TerminalHost` provides a useful reference split between Avalonia console hosting and process/session services, but the current shell locator is Windows-oriented.
- The first AA98 terminal baseline should therefore wrap a configurable stream-based shell session behind AA98 contracts and keep the Avalonia console surface as a reusable UI adapter.
- PTY-specific behavior is intentionally deferred until the wrapper, host, and Linux shell-selection seams are validated.
- Terminal inner-loop tests are now in place for fallback wrapper behavior and Linux-oriented host configuration boundaries, while interactive shell verification remains a documented manual smoke activity.

## Refined Task Targets
- `AA98-T053 Implement Terminal Component Wrapper` targets AA98-facing session contracts, shell resolution abstractions, and a reusable wrapper over configurable process launch behavior.
- `AA98-T054 Create Terminal Micro Host` targets a thin `AA98.Terminal.Host` that composes the wrapper for one configurable Linux-oriented shell scenario.
- `AA98-T055 Add Terminal Inner Loop Tests` targets repeatable wrapper and configuration tests plus documented manual smoke checks for interactive behavior.

## Status
- Completed
