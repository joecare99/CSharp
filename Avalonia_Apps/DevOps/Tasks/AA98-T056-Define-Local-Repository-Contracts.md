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

## Validation
- Build changed projects.
- Add contract tests if descriptors contain normalization behavior.

## Status
- Proposed
