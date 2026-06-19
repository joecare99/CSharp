# Feature F-WorkbenchBuilder-02 - Thin host and inspection output contracts

## Status

In Progress

## Parent

- Epic `E-WorkbenchBuilder-001` - `Roslyn-based build workbench`

## Goal

Provide a thin command-line host that triggers project inspection and emits predictable plain-text or JSON output for interactive use and automation.

## Summary

The current on-disk state already includes `Workbench.Builder.Host` with command-line parsing, service registration, host orchestration, console abstraction, exit codes, and startup entry point. The feature therefore starts from a partially implemented baseline rather than from zero. The planning focus is to stabilize the host contract, make output expectations explicit, and keep the host intentionally thin so `Workbench.Builder.Core` remains reusable.

The host is not meant to become a second business-logic layer. It should parse arguments, call inspection services, choose an output format, and present the result or failure clearly. The main design objective is a small observable host boundary that can later coexist with additional UI or automation adapters.

## In Scope

- Command-line parsing for project path, output format, and help behavior
- Host orchestration around inspection and formatting
- Plain-text output for direct human consumption
- JSON output for tool integration and scripted flows
- Exit-code and error-output conventions for invalid arguments and runtime failures
- Regression coverage for host argument and orchestration behavior

## Out of Scope

- Rich interactive terminal UX
- Full build execution or emit orchestration in the first host slice
- UI-specific presentation concerns for future desktop integration
- Localization redesign for this initial builder-host increment

## Acceptance Criteria

- The host accepts a project path and output-format selection
- Help and invalid-argument flows are distinguishable through exit code and output behavior
- A successful run emits the formatted inspection result exactly once
- Plain-text output remains understandable for manual diagnosis
- JSON output remains structurally stable enough for future consumers
- Host tests cover success, help, and invalid-argument paths

## Dependencies

- `Workbench.Builder.Core` inspection and formatting services
- Existing host-side command-line parser and console abstraction
- Current sample tests for host orchestration and formatter behavior

## Risks

- Output shape may drift if formatter and host are refined independently
- Future automation consumers may require stronger JSON compatibility guarantees than the current slice documents
- Additional host options for V1.2 may pressure the current minimal parser design

## Open Questions

- Should the host later expose explicit switches for configuration, target framework, or runtime identifier?
- Which parts of diagnostics should be printed to standard output versus standard error in non-failure cases?
- Should future machine-readable output separate data from diagnostics more explicitly?

## Next Refinement Steps

1. Confirm the minimum stable host contract for V1.1
2. Add tests for option combinations and output-format edge cases
3. Clarify how future V1.2 options extend the parser without overcomplicating V1.1
4. Keep the host thin while additional consumers are introduced later
