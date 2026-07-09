# Backlog Item BI-WorkbenchBuilder-002 - Stabilize thin host and output contracts

## Status

Completed

## Parent

- Feature `F-WorkbenchBuilder-02` - `Thin host and inspection output contracts`

## Goal

Keep the builder host intentionally small while defining a predictable contract for arguments, output formats, and failure handling.

## Description

`Workbench.Builder.Host` now exists as a real executable slice rather than only a planned idea. It already parses arguments, calls inspection services, and writes formatted output. This backlog item tracks the work needed to keep that host small, explicit, and stable while future builder capabilities are added.

The host should remain an adapter around the reusable core, not a second place where build logic accumulates. Argument parsing, output format selection, usage/help behavior, and exit-code mapping must therefore be treated as an explicit compatibility surface rather than incidental glue code.

## Scope

- Define the stable V1.1 host argument and output baseline
- Clarify invalid-argument, help, and execution-failure behavior
- Keep plain-text and JSON output responsibilities aligned with the formatter boundary
- Add or refine host tests where behavior is still implicit
- Prepare documented extension points for later V1.2 options

## Acceptance Criteria

- The host contract is documented clearly enough for future extension
- Existing tests cover the main success and failure paths
- Output responsibilities between host and formatter are explicit
- Planned future options do not require rethinking the host architecture fundamentally

## Assumptions

- The host remains console-based for the current slice
- The core continues to own the inspection logic while the host only orchestrates

## Risks

- Future compile/emit options may pressure the current parser into overgrowth
- Consumers may begin depending on ad-hoc output details unless the contract is documented now

## Open Questions

- Which additional request parameters should the host expose next: configuration, target framework, runtime identifier, or emit mode?
- Should JSON output later carry richer command-status metadata around the inspection payload?

## Next Refinement Steps

1. Preserve the current thin-host baseline while later waves add capabilities
2. Record extension points for future host options without moving logic out of `Workbench.Builder.Core`
3. Expand tests only where later host-visible behavior changes

## Planned Implementation Tasks

- `T-WorkbenchBuilder-003` - `Document and validate thin host command contract`
- `T-WorkbenchBuilder-004` - `Extend host request options for V1.1 parity where needed`
