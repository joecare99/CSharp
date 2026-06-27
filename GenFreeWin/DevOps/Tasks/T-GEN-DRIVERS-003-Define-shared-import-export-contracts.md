# Task T-GEN-DRIVERS-003: Define Shared Import/Export Contracts

## Parent

- Backlog Item `BI-GEN-DRIVERS-002` - Extract HEJ Import/Export Driver Surface

## Objective

Define shared import/export driver contracts that HEJ and GEDCOM implementations can use without UI dependencies.

## Scope

- Contract location in `GenInterfaces`.
- Import result shape, export result shape, diagnostics, and cancellation support.
- Stream-first asynchronous API design with optional file abstraction adapters.

## Output

- Contract proposal with method names, input/output types, and ownership location.
- Diagnostics model proposal for warnings, unsupported fields, and invalid data.
- Result-object proposal with `Success`, optional payload, and diagnostics.
- Diagnostic severity and location proposal covering at least `Trace`, `Info`, `Warning`, `Error`, file context, and line number.

## Acceptance Criteria

1. Contracts can represent both HEJ and GEDCOM import/export.
2. Contracts do not include UI text strings or UI types.
3. Contracts support diagnostics without forcing exceptions for every recoverable issue.
4. The chosen project location and asynchronous execution model are documented with rationale.
