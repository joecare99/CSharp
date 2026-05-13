# SH-T010103 - Expose Run State In ViewModels

## Work Item Type
Task

## Parent
`SH-BI0101 - Define Run Lifecycle States`

## Goal
Expose run lifecycle state to UI bindings through ViewModels.

## Scope
- Add observable state to `GameViewModel` and `LayeredGameViewModel`.
- Raise property change notifications after state transitions.
- Keep UI strings out of engine state.

## Done
- Console and WPF-facing ViewModels can read the current run state.
- State updates when the run ends.
