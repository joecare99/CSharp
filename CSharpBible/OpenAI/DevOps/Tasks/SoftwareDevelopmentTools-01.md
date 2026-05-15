# Task: Software Development Assistance Wave 01

## Parent
- Related note: `DevOps/BacklogItems/PBI-08-SoftwareDevelopmentTools.md`

## Goal
Build the first repository-assistance slice that can inspect code or workspace state and support developer workflows such as build and test summaries.

## Scope
- define a minimal safe developer tool set and boundaries
- implement one or more inspection helpers
- add build/test helper behavior where practical
- return structured output suitable for assistant clients
- add tests for the main tool flow

## Recommended Implementation Order
1. Define the allowed read-only scope and any write restrictions.
2. Implement repository or code inspection helpers first.
3. Add build and test summary helpers.
4. Normalize output into a structured shape for assistant clients.
5. Add tests for the helper flow and orchestration behavior.

## Subtasks
1. Define the safe scope for developer assistance tools.
2. Choose the first repository or code inspection helper.
3. Implement inspection output in a structured format.
4. Add build and test summary helpers.
5. Expose the helpers to UI and agent-style callers.
6. Add tests for helper behavior and orchestration.
7. Document any operations that remain out of scope.

## Assumptions
- the first wave should be read-oriented unless write operations are clearly justified
- developer tool outputs should be predictable and automation-friendly
- the toolset should remain reusable across projects in the workspace

## Exit Criteria
- the first tool set is implemented
- tests pass
- the related project builds successfully
