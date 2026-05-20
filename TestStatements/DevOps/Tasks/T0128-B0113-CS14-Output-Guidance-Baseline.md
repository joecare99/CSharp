# T0128A - CS14 Output Guidance Baseline

## Parent Backlog Item

- B0113 - Define Documentation and Expected-Output Strategy for C# 14 Comparisons

## Summary

Define how output, behavioral equivalence, or readability-only differences should be documented for `TestStatements.CS14` comparison examples.

## Core Output Principle

Not every CS14 comparison should be treated as an output-driven example.

For many C# 14 features, the primary learning value is source readability, reduced ceremony, or clearer expression. Output guidance should therefore distinguish between runtime equivalence and source-level comparison.

## Comparison Output Categories

### Category 1 - Runtime-Equivalent Comparisons

Definition:

- Both versions should produce the same runtime result.
- The main difference is the way the code is expressed.

How to document:

- State that runtime behavior is expected to be equivalent.
- Describe the shared result once, not twice.
- Shift most of the emphasis to readability, brevity, or maintainability.

Typical CS14 examples:

- field-backed properties
- null-conditional assignment
- unbound generic `nameof`
- modifiers on simple lambda parameters

### Category 2 - Readability-Only Comparisons

Definition:

- The main educational difference is source-level clarity, structure, or ergonomics.
- Runtime output is absent, trivial, or not the point.

How to document:

- Explicitly say that no meaningful output comparison is required.
- Describe the learning value as a source comparison.
- Avoid adding artificial console output only to create a visible result.

Typical CS14 examples:

- some extension-member comparisons
- some partial-member comparisons
- source-organization or declaration-style comparisons

### Category 3 - Behavior-Aware Comparisons

Definition:

- The new feature changes what can be expressed directly, how APIs can be consumed, or what usage patterns are natural.
- The older alternative may be exact only in outcome, or only approximate in structure.

How to document:

- State whether the behavior is equivalent, partially equivalent, or structurally approximate.
- Explain any extra steps needed in the older-language version.
- If the difference affects API shape, say so explicitly.

Typical CS14 examples:

- extension members
- user-defined compound assignment operators
- implicit span conversions
- partial constructors and events

## Expected-Output Writing Rules

### Rule 1 - Describe Outcome Before Transcript

Prefer outcome-oriented wording such as:

- both versions validate the same property behavior
- both versions assign the same value when the receiver is non-null
- both versions expose similar observable results, but the older version requires a different structure

Use exact transcript-style output only when the example is intentionally console-oriented.

### Rule 2 - Mark Equivalence Explicitly

Use one of these labels where practical:

- `Equivalent output`
- `Equivalent behavior`
- `Approximate behavior`
- `Readability-focused comparison`

These labels help learners understand what is actually being compared.

### Rule 3 - Do Not Overstate Equivalence

If the older alternative is only structurally similar, do not claim full equivalence.

Examples:

- extension members may require multiple older constructs, not one direct equivalent
- span-conversion examples may depend on surrounding API context

### Rule 4 - Keep Readability Comparisons Honest

If the main point is readability, say that directly. Do not pretend a console line is the main learning artifact.

### Rule 5 - Note Environment Sensitivity Only When Relevant

Most CS14 comparison topics are syntax-focused, so environment sensitivity is usually secondary.

If a comparison does involve environment-sensitive behavior, document it clearly but keep the language-feature comparison central.

## Suggested Per-Example Output Section

For each CS14 comparison, the output or equivalence section should use this structure:

- Comparison category
- Expected behavior or expected output
- Equivalence note
- Caveats or limitations

Example phrasing:

- `Comparison category: Runtime-equivalent comparison`
- `Expected behavior: Both versions throw the same exception when a null value is assigned.`
- `Equivalence note: The modern version reduces property boilerplate without changing behavior.`
- `Caveat: The older version is structurally longer but semantically direct.`

## Relationship to Existing Output Planning

This guidance should align with the broader repository output rules already used for advanced examples:

- prefer observable patterns over unnecessary transcript detail
- distinguish deterministic, illustrative, and environment-sensitive behavior
- document what learners should focus on, not only raw output text

## Metadata Alignment

The output guidance should support later population of metadata fields such as:

- `ExpectedObservation`
- `OutputSensitivity`
- `Determinism`
- `Notes`

Recommended defaults for many CS14 examples:

- `OutputSensitivity`: often `Low` or `Medium`
- `Determinism`: often `Deterministic` for runtime-equivalent comparisons, or `Illustrative` for readability-focused comparisons

## Anti-Patterns to Avoid

- Do not force every CS14 example into a console-driven demonstration.
- Do not duplicate output descriptions for both versions when one shared description is enough.
- Do not hide that some comparisons are mainly about source expression rather than runtime behavior.
- Do not claim exact equivalence when the older alternative is only approximate.

## Notes

- This output guidance keeps the CS14 project aligned with the repository's documentation-first and learning-oriented approach.
- The goal is to make learners compare meaningfully, not merely to confirm that two code samples both compile.

## Done Criteria

- Output-sensitive and non-output-sensitive cases are distinguished.
- Guidance supports runtime and readability-oriented comparisons.
- The guidance aligns with existing expected-output planning work.
