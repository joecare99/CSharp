# AA98-F37 Engineering Tooling Foundation

## Parent
- Epic: `DevOps/Epics/AA98-E09-Quality-Tests-and-Engineering-Baseline.md`
- Vision: `DevOps/Vision.md`

## Goal
Define the first provider-neutral engineering tooling foundation for `AA98_AvlnCodeStudio` so versioning, testing, and debugging capabilities are available as shared application contracts from the beginning.

## Scope
- Define provider-neutral contracts for versioning, testing, and debugging.
- Keep the contracts available to all application components through scope-specific `.Base.XXX` libraries built on the shared base layer.
- Clarify the architectural boundaries between shared contracts and later concrete providers.
- Prepare the path for future workflow, UI, and provider integrations without coupling the foundation to a specific toolchain.

## Included
- Versioning capability baseline
- Testing capability baseline
- Debugging capability baseline
- Shared request and status models for later orchestration
- Boundaries between capability contracts and provider implementations

## Excluded for Now
- Concrete Git integrations
- Concrete test runner adapters
- Concrete debugger adapters
- Rich UI workflow orchestration
- Cross-process session management details

## Success Indicators
- Future components can depend on shared versioning, testing, and debugging abstractions without referencing concrete tools.
- The base layer remains provider-agnostic and reusable.
- Later service and UI implementations can extend the contracts without breaking the initial architectural baseline.

## Candidate Backlog Items
- Define provider-neutral versioning abstractions
- Define provider-neutral testing abstractions
- Define provider-neutral debugging abstractions
- Keep the baseline available to all components through the shared base layer

## Assumptions
- These capabilities should be part of the common application foundation through dedicated scope-specific base libraries instead of isolated optional modules.
- The first contracts should remain small, stable, and implementation agnostic.
- Later providers may differ significantly, so the initial models should focus on shared workflow concepts only.

## Open Questions
- Should a higher-level engineering workspace facade be introduced later above the individual capabilities?
- Which first status details are required by the shell and tool windows once UI integration begins?

## Status
- Proposed