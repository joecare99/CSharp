# AA98-T081 Add AI Workflow Consent and Boundary Tests

## Parent
- Backlog Item: `../BacklogItems/AA98-Bl050-First-Copilot-Assisted-Self-Hosting-Workflow.md`

## Goal
Add tests that verify consent and boundary behavior for the first AI-assisted workflow.

## Scope
- Test explicit consent requirement.
- Test minimal context sharing.
- Test provider-neutral invocation behavior with fakes.
- Test diagnostics for denied or missing consent.

## Execution Notes
1. Use fake AI providers or command invokers.
2. Avoid network calls and real credentials in tests.
3. Keep tests deterministic.

## Acceptance Criteria
- AI workflow does not run without required consent.
- Context sharing is bounded and test-visible.
- Provider-specific dependencies are not required for tests.

## Validation
- Run targeted AI workflow tests.

## Status
- Proposed
