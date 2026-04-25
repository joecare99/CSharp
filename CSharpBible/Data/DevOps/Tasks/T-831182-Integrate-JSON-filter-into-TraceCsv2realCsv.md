# Task T-831182 - Integrate JSON filter into TraceCsv2realCsv

## Status

Draft

## Parent

- Feature `830329` - `Filter-based trace intake and export foundation`

## Goal

Extend `TraceCsv2realCsv` so the console program can use the canonical JSON trace cache filter for input and output scenarios.

## Scope

- Add the JSON filter project reference to the console application and its tests
- Register the JSON analyzable input filter in the existing dependency injection setup
- Enable output-filter selection for `.csv` and `.json` targets without breaking current CSV defaults
- Update help text so supported input and output filters remain visible and deterministic
- Add automated tests for JSON availability and JSON output behavior in the console conversion path

## Out of Scope

- Command-line switches for manual filter selection
- Multiple simultaneous output files per invocation
- UI workflow changes outside the console application

## Implementation Notes

- Preserve the current default output behavior for omitted output paths
- Keep JSON as a technical exchange and cache format, not a UI report format
- Reuse the existing `TraceAnalysis` filter contracts instead of adding app-specific serialization logic
- Keep multi-target compatibility with older .NET Framework targets

## Test Strategy

- Add unit tests for JSON output selection by target extension
- Add unit tests for JSON input support through the conversion service
- Update help text tests to include the JSON filter entries

## Done Criteria

- `TraceCsv2realCsv` resolves and registers the JSON filter project
- The conversion service can write JSON when the output extension is `.json`
- The conversion service can read JSON through the shared input-filter selection pipeline
- Help text reflects the additional filter support
- Relevant tests and build validation succeed
