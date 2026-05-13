# SH-BI0701 - Build Smoke Test Suite

## Work Item Type
Backlog Item

## Parent
`SH-F07 - Test Quality and Release Readiness`

## Description
Create a deterministic validation suite proving the selected playable slice works end-to-end.

## Scope
- Identify authoritative SharpHack validation project set.
- Add smoke tests for run creation, movement, combat, level transition, save/load, and UI ViewModel commands.
- Ignore only repository-approved unsupported older-framework warnings when newer supported targets pass.

## Acceptance Criteria
- Relevant SharpHack tests pass in one documented validation path.
- Smoke tests are deterministic.
- Validation status is recorded in release notes.

## Child Tasks
- `SH-T070101 - Define SharpHack Validation Project Set`
- `SH-T070102 - Add Core Smoke Tests`
- `SH-T070103 - Add Persistence Smoke Tests`
- `SH-T070104 - Add ViewModel Smoke Tests`
- `SH-T070105 - Document Validation Path`
