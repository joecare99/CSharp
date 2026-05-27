# AA98-Bl010 Component Registration Baseline

## Parent
- Feature: `DevOps/Features/AA98-F12-Component-Registration-and-Composition-Pipeline.md`
- Epic: `DevOps/Epics/AA98-E03-Component-Extension-Model.md`
- Vision: `DevOps/Vision.md`

## Scope
Define the first explicit registration baseline for internal components in `AA98_AvlnCodeStudio` so contributed commands, UI modules, and configuration can be composed through a clear and testable application pipeline.

## Goals
- Define the first explicit component registration approach.
- Clarify how internal components are connected to application composition.
- Keep the registration model understandable, DI-friendly, and increment-friendly.
- Prepare the path for later composition of commands, UI modules, and configuration contributions.

## Assumptions
- Explicit registration is the safest first step before considering assembly scanning or dynamic discovery.
- Initial components are solution-internal and trusted.
- The registration model should remain simple enough to validate through tests and later extension.

## Open Questions
- Which responsibilities belong in application startup versus a dedicated composition layer?
- How early should discovery be abstracted beyond explicit registration?

## Status
- Proposed
