# Epic E-GEN-DRIVERS-001: Genealogy Import/Export Driver Separation

## Objective

Separate genealogy file-format import/export concerns from the reusable genealogy domain model by turning `WinAhnenCls` into a dedicated `.hej` driver and by introducing a separate GEDCOM driver project.

## Business Value

- Makes `.hej` and GEDCOM support independently testable and maintainable.
- Keeps reusable genealogy entities, facts, places, sources, repositories, media, and persistence contracts in `BaseGenClasses` and `GenInterfaces`.
- Reduces coupling between file formats and the application-facing genealogy model.
- Enables future UI, command-line, and AI-invokable workflows to consume import/export drivers through stable contracts.

## Current State

- `..\WinAhnenNew\WinAhnenCls\WinAhnenCls.csproj` currently contains HEJ-related model/read logic such as `CHejGenealogy`, `CHejBase`, individual, marriage, place, and source classes.
- `WinAhnenCls` also contains a first GEDCOM reader shape under `Model\GenBase`.
- `..\WinAhnenNew\BaseGenClasses\BaseGenClasses.csproj` already contains central genealogy model types, helper builders, and persistence abstractions.
- Existing tests and resources in `..\WinAhnenNew\WinAhnenClsTests` provide regression samples for `.hej` and `.ged` files.

## Target State

1. `BaseGenClasses` owns reusable genealogy model and mapping primitives.
2. `WinAhnenCls` becomes a pure `.hej` import/export driver that maps `.hej` streams to and from the central model.
3. A new GEDCOM driver project owns GEDCOM parsing, writing, mapping, validation, and GEDCOM-specific compatibility behavior.
4. Import/export contracts are expressed through `GenInterfaces`/`BaseGenClasses` abstractions and can be used without UI dependencies.
5. Each driver has focused MSTest regression tests and resource-based round-trip fixtures.

## Scope

- Architectural planning for moving reusable types out of `WinAhnenCls` into `BaseGenClasses`.
- Planning the `.hej` driver responsibility boundaries.
- Planning a separate GEDCOM import/export driver project.
- Planning tests, compatibility, and migration slices.

## Non-Goals

- No immediate source-code movement in this planning item.
- No UI workflow redesign.
- No persistence-store migration beyond file-format driver responsibilities.
- No assumption that all legacy GEDCOM behavior is correct without regression analysis.

## Child Features

- Feature `F-GEN-DRIVERS-001` - Refactor WinAhnenCls into HEJ Import/Export Driver
- Feature `F-GEN-DRIVERS-002` - Create Separate GEDCOM Import/Export Driver

## Acceptance Criteria

1. The target ownership boundaries between `BaseGenClasses`, the `.hej` driver, and the GEDCOM driver are documented.
2. Migration backlog items exist for moving reusable model code into `BaseGenClasses`.
3. Driver API and test strategy are documented before implementation starts.
4. Open questions and format-specific risks are visible in planning artifacts.

## Assumptions

- Multi-targeting must remain compatible with the current project set, including `.NET Framework 4.8.1` and modern `.NET` targets.
- New or changed tests should use MSTest and explicit `using` directives.
- File access should be expressed through stream/file abstractions where practical, avoiding hard-coded UI or filesystem dependencies.

## Confirmed Decisions

- Shared import/export contracts belong in `GenInterfaces` as the format-neutral contract location.
- The GEDCOM driver should use the long-term neutral name `Genealogy.GedCom` and live in a neutral solution area.
- Driver execution should be asynchronous.
- `WinAhnenCls` should not retain temporary compatibility adapters after shared code moves.
- GEDCOM import should understand versions `5.5`, `5.5.1`, and `7`, with a slight implementation preference for `5.5.1`.
- GEDCOM export should choose the target version through user choice or configuration.
- Handling of special or foreign GEDCOM tags should be configurable: preserve them or report them diagnostically depending on mode.
- `GedLes` is at most a negative/reference input for comparison; implementation decisions should primarily follow the GEDCOM `5.5.1` standard.
- The shared result model should expose `Success`, `Diagnostics`, and an optional payload.
- Diagnostics should support at least `Trace`, `Info`, `Warning`, and `Error` severities and include file context plus line number.
- Driver tests should use both semantic JSON snapshots and explicit assertions.
- Round-trip validation should be tolerant, especially regarding ordering differences.
- HEJ work should prioritize import; export should default to semantic equivalence and may later offer a legacy-format mode.

## Open Questions

- Which concrete `WinAhnenCls` types are format-neutral enough to move into `BaseGenClasses` in the first migration slice?
