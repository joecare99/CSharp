# Feature F-WorkbenchBuilder-02 - Thin host and inspection output contracts

## Status

Completed

## Parent

- Epic `E-WorkbenchBuilder-001` - `Roslyn-based build workbench`

## Goal

Provide a thin command-line host boundary that stays small, delegates build logic to `Workbench.Builder.Core`, and emits predictable output and diagnostics for interactive use and automation.

## Summary

The current on-disk state includes `Workbench.Builder.Host` with command-line parsing, service registration, host orchestration, console abstraction, exit codes, startup entry point, detailed progress reporting, and IDE-friendly diagnostic output. The feature therefore moved beyond a partial baseline and now serves as the thin host boundary around reusable core services.

The host is not meant to become a second business-logic layer. It should parse arguments, call core services, and present success, warnings, and failures clearly. The main design objective remains a small observable host boundary that can later coexist with additional UI or automation adapters.

## In Scope

- Command-line parsing for project path, output directory, verbosity, and help behavior
- Host orchestration around inspection, compilation, and formatting
- Plain-text output for direct human consumption
- Exit-code and error-output conventions for invalid arguments and runtime failures
- Regression coverage for host argument and orchestration behavior

## Out of Scope

- Rich interactive terminal UX
- Full build execution or emit orchestration in the first host slice
- UI-specific presentation concerns for future desktop integration
- Localization redesign for this initial builder-host increment

## Acceptance Criteria

- The host accepts a project path and the current supported command options
- Help and invalid-argument flows are distinguishable through exit code and output behavior
- A successful run emits the formatted build result exactly once while preserving non-error diagnostics
- Plain-text output remains understandable for manual diagnosis
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

1. Preserve the thin-host boundary while future waves add options or alternate consumers
2. Extend tests only where later compilation work changes host-visible behavior
3. Revisit machine-readable output separately if a later consumer requires it explicitly
