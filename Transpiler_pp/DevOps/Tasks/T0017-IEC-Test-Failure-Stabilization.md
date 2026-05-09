# T0017 IEC Test Failure Stabilization

## Parent
- Backlog Item: Unknown yet

## Scope
- Investigate failing tests in `TranspilerLib.IEC.Tests`
- Identify shared root causes across scanner, interpreter, AST mapping, and output expectations
- Implement minimal production fixes first
- Adjust tests only if they assert outdated behavior and the implementation is confirmed correct

## Assumptions
- The failures are caused by recent IEC parser/interpreter/output changes rather than environment-specific issues
- Multi-target test runs should pass consistently for `net8.0`, `net9.0`, and `net10.0` when the implementation is stabilized

## Open Questions
- Whether one shared tokenization regression is cascading into parser and AST failures
- Whether interpreter failures are caused by expression tree changes or arithmetic semantics changes
- Whether output failures reflect formatting drift or model-shape regressions

## Validation
- Run focused failing tests first
- Run the full `TranspilerLib.IEC.Tests` project after fixes
- Run a workspace build to ensure no compilation regressions
