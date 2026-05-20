# B0004-Time-and-IO-Abstractions

## Parent
- Feature: F0001-IEC-to-CSharp-Testability

## Description
Abstract time, timers, counters, and external IO so IEC behavior can be executed deterministically without live runtime dependencies.

## Value
Testability depends on replacing hardware and wall-clock coupling with explicit, controllable abstractions.

## Scope
- Define abstractions for time access and elapsed time progression
- Define support strategy for timer and counter semantics in the supported subset
- Define external IO boundaries needed by interpretation and generated C#

## Out of Scope
- Vendor-specific runtime integration
- Full device simulation

## Acceptance Criteria
- Timer- and time-dependent behavior can be executed under test with explicit control
- External dependencies are injectable or otherwise replaceable in tests
- Design supports later IEC standard function block extensions

## Assumptions
- The execution model from B0003 is available or defined in parallel

## Open Questions
- Which timer blocks should be included in the first supported subset
- Whether IO should be represented as interfaces, data objects, or both
