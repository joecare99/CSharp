# Game_BaseTests TestItem Common Base

## Type
Task

## Status
Completed

## Goal
Extract the duplicated parent/place/name behavior from `TestItem` and `TestItem2D` into a shared base class without changing existing test behavior.

## Backlog Notes
- Keep concrete types because tests and generic usages depend on them.
- Keep static logging events in the concrete classes.
- Keep `ToString()` outputs type-specific.

## Planned Validation
- Build affected projects
- Run relevant MSTest classes for `FieldTests` and `Playfield2DTests`

## Result
- Added `TestPlacedItemBase<TSelf>` to centralize shared name, parent, place, old-place, equality, and place-change behavior.
- Updated `TestItem` and `TestItem2D` to inherit from the shared base while keeping their own logging events and type-specific `ToString()` output.
- Preserved the existing `TestItem2D` stub methods unchanged.

## Validation Outcome
- Isolated project build: success
- Test status: success
- Tests passed: 264
- Tests skipped: 0
- Tests failed: 0
- Remaining warnings belong to pre-existing files such as `TestParent.cs` and `FieldTests.cs` and were not expanded by this refactor.
