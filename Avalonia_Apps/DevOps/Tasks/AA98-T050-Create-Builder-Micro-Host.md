# AA98-T050 Create Builder Micro Host

## Parent
- Backlog Item: `../BacklogItems/AA98-Bl040-Builder-Inner-Loop-Baseline.md`
- Feature: `../Features/AA98-F39-Component-Micro-Hosts.md`

## Goal
Create a thin builder host for isolated development and validation of builder workflows.

## Scope
- Add a host that exercises builder inspection/build scenarios.
- Keep the host thin and focused on builder composition.
- Avoid duplicating builder logic in the host.

## Execution Notes
1. Follow the micro-host pattern if already defined.
2. Keep output structured enough for troubleshooting.
3. Record any Linux execution assumptions.

## Acceptance Criteria
- Builder host can run the first selected builder scenario.
- Host remains a thin wrapper over reusable builder services.

## Validation
- Run the builder host smoke scenario.
- Build host project and related tests.

## Implementation Notes
- Added `AA98_AvlnCodeStudio.Building.Workbench` as the reusable net10.0 adapter from AA98 builder contracts to `Workbench.Builder.Core`.
- Added `AA98.Builder.Host` as a thin console-oriented micro host for `inspect` and `build` scenarios.
- Kept `AA98_AvlnCodeStudio.Base.Building` as the multi-target contract layer so existing net8.0/net9.0 consumers remain valid.

## Status
- Completed
