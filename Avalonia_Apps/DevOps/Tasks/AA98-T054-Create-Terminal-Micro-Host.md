# AA98-T054 Create Terminal Micro Host

## Parent
- Backlog Item: `../BacklogItems/AA98-Bl041-Terminal-Inner-Loop-Baseline.md`
- Feature: `../Features/AA98-F39-Component-Micro-Hosts.md`

## Goal
Create a thin terminal host for isolated validation of terminal sessions.

## Scope
- Host the terminal component with minimal composition.
- Validate Linux shell/session startup.
- Keep workbench docking and layout out of scope.

## Execution Notes
1. Follow the shared micro-host pattern when available.
2. Use configurable shell defaults.
3. Keep the host focused on one terminal scenario.

## Implementation Notes
- The host should be a thin Avalonia executable, preferably named `AA98.Terminal.Host`, that composes the reusable terminal wrapper and one console-facing view.
- Reuse the existing Avalonia console hosting approach as a UI adapter, but keep shell/session logic in reusable services outside the host.
- The first smoke scenario should validate startup, output flow, line input, and clean shutdown for one configurable development shell.

## Acceptance Criteria
- Terminal host can start a configured development shell.
- Host remains a thin wrapper around terminal component services.

## Validation
- Run a terminal host smoke scenario.
- Build host and related projects.

## Status
- Proposed
