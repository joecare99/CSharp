# AA98-Bl042 Local Repository Workflow Baseline

## Parent
- Feature: `../Features/AA98-F43-Repository-and-Planning-Workflows.md`
- Epic: `../Epics/AA98-E11-Linux-Self-Hosting.md`

## Value
A developer can inspect local repository state and use repository context during self-hosting work.

## Scope
- Start with local Git repository inspection.
- Keep provider-specific behavior outside the core workflow.
- Prepare repository context for planning and AI/tool commands.
- Avoid direct dependency on Azure DevOps or GitHub.

## Acceptance Criteria
- Local repository root, branch, status, and changed files can be represented through neutral contracts.
- The first repository host or service can validate local state.
- Provider-specific remote workflows remain excluded.

## Implementation Tasks
- `AA98-T056 Define Local Repository Contracts`
- `AA98-T057 Implement Local Repository Inspection`
- `AA98-T058 Add Repository Workflow Tests`

## Assumptions
- Commit and push workflows can be deferred unless needed for the first self-hosting milestone.

## Open Questions
- Is repository inspection sufficient for minimal self-hosting?

## Next Refinement Steps
1. Define neutral local repository contracts.
2. Add commit workflow planning only after inspection is useful.

## Progress Notes
- The provider-neutral local repository contract baseline is now defined in the shared AA98 versioning layer, including repository context, active local reference kind, capability metadata, and staged or ignored change details.
- Deterministic model tests now cover the new local repository request and status defaults before live Git inspection is introduced.
- Local Git inspection is now implemented through a dedicated adapter project with repository root discovery, branch or detached-head mapping, capability reporting, and porcelain-based change enumeration behind an isolated command runner seam.
- Repository workflow validation now covers Git request-flag behavior and conservative fallback context handling, and the remaining real-repository smoke-check flow is documented for manual confirmation when needed.

## Status
- Completed
