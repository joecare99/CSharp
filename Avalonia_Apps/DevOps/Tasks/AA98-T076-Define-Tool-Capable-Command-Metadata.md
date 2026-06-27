# AA98-T076 Define Tool-Capable Command Metadata

## Parent
- Backlog Item: `../BacklogItems/AA98-Bl049-Tool-Capable-Command-Contracts.md`

## Goal
Define metadata required for AA98 commands to be safely usable as AI tools.

## Scope
- Identify command ID, description, parameters, results, context requirements, safety level, and consent needs.
- Decide shared versus AA98-specific contract placement.
- Keep provider-specific AI assumptions out of the metadata.

## Execution Notes
1. Review existing command and context contracts first.
2. Prefer shared contracts when metadata is not AA98-specific.
3. Keep explicit using directives when implementing later code.

## Acceptance Criteria
- Metadata model and layer placement are documented.
- Implementation task `AA98-T077` has concrete contract targets.

## Validation
- Planning/design review only unless contract files are created in this task.

## Status
- Proposed
