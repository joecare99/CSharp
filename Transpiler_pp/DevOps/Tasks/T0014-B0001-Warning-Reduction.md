# T0014-B0001-Warning-Reduction

## Parent
- Backlog Item: B0001-Typed-IEC-AST

## Description
Continue reducing active build warnings in the current solution incrementally, starting from the currently visible warning set after recent cleanup work.

## Goal
Remove the next meaningful warning with a minimal, validated code change.

## Scope
- Inspect current warning output
- Pick one concrete warning in active projects
- Apply a minimal fix
- Validate with build and relevant tests when needed

## Out of Scope
- Broad refactoring without warning evidence
- Unrelated functional changes
- Large-scale nullable redesign across the whole solution

## Notes
- Handle warnings incrementally
- Prefer fixes that align interface contracts and nullable annotations
- Validate after each focused warning fix
