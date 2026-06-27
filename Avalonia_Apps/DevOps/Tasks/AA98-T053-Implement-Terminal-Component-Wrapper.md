# AA98-T053 Implement Terminal Component Wrapper

## Parent
- Backlog Item: `../BacklogItems/AA98-Bl041-Terminal-Inner-Loop-Baseline.md`

## Goal
Implement an AA98-facing wrapper for terminal component integration.

## Scope
- Expose terminal session creation and interaction through AA98 component boundaries.
- Keep terminal core logic outside the workbench shell.
- Prepare Avalonia integration without coupling to the final docking layout.

## Execution Notes
1. Use existing terminal contracts where possible.
2. Abstract shell selection and process launch behavior.
3. Keep Linux defaults configurable.

## Implementation Notes
- Start from the `Avln_TerminalHost` split between UI hosting and process/session services, but move the AA98 wrapper behind reusable AA98 contracts instead of binding it to the sample host view model.
- The first implementation target is a stream-based shell session wrapper with explicit shell path or shell command resolution for Linux-oriented defaults.
- Keep PTY-specific features out of the first slice unless a validated requirement appears during host integration.

## Acceptance Criteria
- Terminal component can be composed through AA98 contracts.
- Linux shell assumptions are explicit and configurable.

## Validation
- Build changed projects.
- Run targeted terminal wrapper tests if available.

## Status
- Proposed
