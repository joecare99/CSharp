# AA98-T056 Define Local Repository Contracts

## Parent
- Backlog Item: `../BacklogItems/AA98-Bl042-Local-Repository-Workflow-Baseline.md`

## Goal
Define neutral contracts for local repository context and status.

## Scope
- Represent repository root, branch, status, changed files, and capabilities.
- Avoid Azure DevOps and GitHub-specific assumptions.
- Prepare repository context for AI/tool-capable commands.

## Execution Notes
1. Place contracts in the correct shared or AA98-specific layer.
2. Keep provider remote concepts out of the local baseline unless capability-based.
3. Prefer capability descriptions over provider names.

## Acceptance Criteria
- Local repository status can be represented provider-neutrally.
- Contracts are suitable for local Git implementation and later adapters.

## Delivered
- Extended the shared versioning request contract with repository context and optional capability inclusion for local inspection scenarios.
- Extended the shared repository status contract with provider-neutral repository identity, active reference kind, detached-state information, repository-root discovery, and capability metadata.
- Extended the local change summary contract with staged and ignored-state flags for later local Git inspection without introducing remote-provider assumptions.
- Added deterministic engineering model tests for the new repository request, status, and change-summary defaults.

## Validation
- Build changed projects.
- Add contract tests if descriptors contain normalization behavior.

## Status
- Completed
