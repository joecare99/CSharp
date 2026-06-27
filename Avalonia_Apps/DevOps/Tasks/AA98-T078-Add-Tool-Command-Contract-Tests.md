# AA98-T078 Add Tool Command Contract Tests

## Parent
- Backlog Item: `../BacklogItems/AA98-Bl049-Tool-Capable-Command-Contracts.md`

## Goal
Add tests for tool-capable command descriptor contracts.

## Scope
- Test descriptor normalization and validation.
- Test parameter/result descriptor behavior.
- Test safety and consent metadata defaults.

## Execution Notes
1. Use MSTest `[TestMethod]` for new tests.
2. Avoid obsolete `[DataTestMethod]`.
3. Keep tests focused on contract behavior.

## Acceptance Criteria
- Tool-capable command contracts have targeted tests.
- Existing command tests continue to pass.

## Validation
- Run targeted command contract tests.

## Status
- Proposed
