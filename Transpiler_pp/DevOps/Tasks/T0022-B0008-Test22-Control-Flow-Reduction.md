# T0022-B0008 - Test22 control-flow reduction

## Status

Implemented and validated on `net8.0`.

## Requirement

`Test22Dat.cs` contains a non-case `IL_023d` label inside a resume switch. Several switch-local branches target that label, and the final `end_IL_0000_2` target is reached by many conditional gotos. The required reduction is based on the standard optimizer pipeline; it must not invent a switch `break` for the `IL_023d` branches.

## Implementation

- Added `Test22Dat.cs` to the test resources.
- Added a focused optimizer regression test for Test22.
- The duplicate-goto cleanup removes the inner `goto IL_023d` when the next executable instruction is the same goto.
- The existing standard label-reduction pipeline then removes the now-redundant `IL_023d` label and concatenates the blocks.
- `ReplaceFinalSwitchGotoWithBreak` explicitly excludes `IL_023d`; the original goto target semantics are therefore preserved while the redundant inner jump is removed.
- Consecutive direct `if` branches with a common multi-source continuation are reconstructed as nested `else if` branches. Their shared fall-through work is moved into the terminal `else { ... }` branch, so removed gotos cannot expose work that the original branch skipped.
- When that terminal `else` contains multiple sibling instructions, they are enclosed by an explicit block node and rendered as `else { ... }`.
- The reconstruction follows enclosing `if`/`else` flow to the common continuation goto. It excludes labels directly owned by a `switch`, preventing resume-switch labels from being restructured.
- The serialized Test22 expectation records the verified standard-optimizer output. No Test22-specific branch-builder or switch-continuation hoisting is used.

## Validation

- `RemoveSingleSourceLabels1_ReducesTest22ControlFlow`: passed.
- `CodeOptimizerTests`: 12/12 passed.
- `RemoveLabelsTest`: 17/17 passed on `net8.0`.
- `Test09` and `Test22` serialized expectations were synchronized with their verified optimized output.
