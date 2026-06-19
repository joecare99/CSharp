# Task T-WorkbenchBuilder-004 - Extend host request options for V1.1 parity where needed

## Status

Draft

## Parent

- Backlog Item `BI-WorkbenchBuilder-002` - `Stabilize thin host and output contracts`

## Goal

Decide whether the thin host must expose additional request options such as configuration, target framework, or runtime identifier to stay aligned with the V1.1 core service surface.

## Scope

- Compare current host options against `ProjectLoadRequest`
- Identify gaps that block realistic inspection scenarios
- Add only the minimum host-surface extensions needed for practical parity

## Acceptance Criteria

- Any required host-surface gaps are explicit
- New options are added only when they improve real V1.1 scenarios
- The host remains intentionally thin after the change

## Dependencies

- `ProjectLoadRequest`
- `HostCommandOptions`
- `HostCommandLineParser`
- `HostApplication`
