# AA98-T077 Implement Tool Command Descriptor Contracts

## Parent
- Backlog Item: `../BacklogItems/AA98-Bl049-Tool-Capable-Command-Contracts.md`

## Goal
Implement tool-capable command descriptor contracts according to the metadata design.

## Scope
- Add parameter and result descriptor contracts.
- Add safety and consent metadata.
- Integrate with existing command descriptors without breaking UI command usage.

## Execution Notes
1. Keep reusable metadata in shared layers where justified.
2. Keep AA98 routing semantics in AA98-specific contracts.
3. Use explicit `using` directives; do not add global or implicit usings.

## Acceptance Criteria
- Commands can describe tool-facing parameters, results, context, and safety needs.
- Existing UI command contracts remain compatible.

## Validation
- Build changed projects.
- Run command contract tests and add tests in `AA98-T078`.

## Status
- Proposed
