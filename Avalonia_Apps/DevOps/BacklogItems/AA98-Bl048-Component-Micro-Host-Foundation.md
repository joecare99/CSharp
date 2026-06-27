# AA98-Bl048 Component Micro Host Foundation

## Parent
- Feature: `../Features/AA98-F39-Component-Micro-Hosts.md`
- Epic: `../Epics/AA98-E11-Linux-Self-Hosting.md`

## Value
AA98 components can be developed and validated through a consistent micro-host pattern.

## Scope
- Define the common micro-host project pattern.
- Identify required shared startup/composition helpers.
- Apply the pattern to the first mandatory self-hosting hosts.
- Keep component core logic outside executable host projects.

## Acceptance Criteria
- A documented micro-host pattern exists.
- First mandatory hosts can follow the same structure.
- Host-specific shortcuts are identified as architecture risks.

## Implementation Tasks
- `AA98-T073 Define Micro Host Project Pattern`
- `AA98-T074 Create Shell and Editor Micro Hosts`
- `AA98-T075 Add Micro Host Smoke Tests`

## Assumptions
- Not every future host must be created before minimal self-hosting.

## Open Questions
- Which host set is mandatory for the first milestone: shell/editor only or shell/editor/builder/terminal/planning?

## Next Refinement Steps
1. Define the pattern before creating multiple hosts.
2. Add separate host tasks only when a component slice is ready.

## Status
- Proposed
