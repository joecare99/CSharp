# AA98-Bl037 Versioning, Testing, and Debugging Foundation

## Parent
- Feature: `DevOps/Features/AA98-F37-Engineering-Tooling-Foundation.md`
- Epic: `DevOps/Epics/AA98-E09-Quality-Tests-and-Engineering-Baseline.md`
- Vision: `DevOps/Vision.md`

## Scope
Define the first shared engineering capability baseline for `AA98_AvlnCodeStudio` so versioning, testing, and debugging can be consumed through common base abstractions by all future program components.

## Goals
- Define provider-neutral contracts for versioning, testing, and debugging.
- Keep the contracts available through dedicated scope-specific base libraries for all components.
- Clarify the separation between shared engineering capabilities and later concrete providers.
- Keep the baseline architectural and independent from concrete tool implementations.

## Assumptions
- A common base abstraction is preferable to separate optional feature islands for these capabilities.
- The first iteration should focus on requests, status reporting, and lifecycle entry points.
- Concrete providers can be introduced later without reshaping the shared contracts fundamentally.

## Open Questions
- Is a shared aggregated engineering service needed later, or should capabilities stay separate long-term?
- Which capability should be connected to UI workflows first after the baseline contracts exist?

## Status
- Proposed