# T0131A - CS14 Null-Conditional Assignment Package Baseline

## Parent Backlog Item

- B0112 - Define C# 14 Feature Comparison Scenarios

## Summary

Define the first concrete example package for null-conditional assignment in `TestStatements.CS14` and the older explicit-null-check comparison patterns it should use.

## Package Intent

Show how null-conditional assignment simplifies common guarded assignment patterns while preserving the core behavior of explicit null checks.

## Modern Feature Focus

- `?.` on the left side of assignment
- null-conditional compound assignment where applicable
- short-circuiting of right-side evaluation when the receiver is null

## Non-C# 14 Comparison Direction

- explicit null check blocks
- explicit guarded assignment logic before mutation

## Comparison Type

- Exact comparison for simple assignment cases
- Exact comparison with caveat notes for compound assignment cases

## Planned Example Cuts

### Example A - Simple Guarded Assignment

Goal:

- Show assignment to a nested object or property only when the receiver is non-null.

Alternative:

- explicit `if (x is not null)` guard

Teaching focus:

- reduced control-flow noise
- same semantic intent in normal cases

### Example B - Deferred Right-Side Evaluation

Goal:

- Show that the right side is evaluated only when the receiver is not null.

Alternative:

- explicit null check with method call inside the guarded branch

Teaching focus:

- equivalence of guarded evaluation
- why the shorter syntax is still behavior-aware

### Example C - Compound Assignment Boundaries

Goal:

- Show allowed compound assignment forms and mention unsupported increment or decrement cases.

Alternative:

- explicit guarded read-modify-write logic

Teaching focus:

- concise mutation pattern
- limitation awareness

## Expected Documentation Style

- runtime-equivalent comparison with small observable behaviors
- emphasize equivalence first, then syntax benefit
- note limitations explicitly

## Output Guidance

- use deterministic examples where possible
- document shared behavior once
- include caveat text for unsupported forms rather than inventing misleading alternatives

## Migration Note Direction

- explain when the new syntax improves everyday guarded assignment code
- explain when teams may still prefer explicit control flow for clarity in complex mutations

## Done Criteria

- Package scope is defined.
- Comparison cuts are outlined.
- The package is ready for concrete example implementation planning.
