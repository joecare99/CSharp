# T-018 Define Designer Consumption

## Parent
B-Designer-Consumption | Previous: T-017 | Next: T-019

## Status
Planned

## Request
Document and implement only the shared-contract adapters needed by ConsoleLib.Cxaml.Designer. The designer consumes Config.UI, Property Editor, Project Explorer, and CodeLocation; the shared pool never references designer/ConsoleLib types.

## Planned tests
- Project-reference architecture test for one-way dependencies.
- Adapter fixture operates with navigator and property contracts through test doubles.
- AXaml error location maps to CodeLocation.

## Gate and completion
Pass G-Architecture. Before Done: documented boundary, tests/build, English SVN commit/revision, T-019 In Progress, status updates.