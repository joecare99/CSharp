# T0126A - CS14 Comparison Pattern Rules Baseline

## Parent Backlog Item

- B0112 - Define C# 14 Feature Comparison Scenarios

## Summary

Define the baseline rules for comparing a modern C# 14 feature with a non-C# 14 alternative so `TestStatements.CS14` remains consistent, educational, and reusable.

## Core Comparison Rule

Every CS14 comparison should answer the same sequence of questions:

1. What is the new feature?
2. What problem or friction does it reduce?
3. What is the non-C# 14 alternative?
4. Is the alternative exact or only approximate?
5. What is the main learning takeaway?

## Comparison Unit Requirements

Each comparison topic should include:

- one C# 14 example
- one non-C# 14 alternative
- a short explanation of the difference
- notes on readability, expressiveness, or maintainability
- expected output or behavioral equivalence notes when runtime behavior matters

## Exact versus Approximate Alternatives

### Exact Alternative

Use this label when the older-language version can express the same behavior directly, only with more ceremony or a less convenient syntax.

Examples:

- field-backed property versus explicit backing field
- null-conditional assignment versus explicit null check
- lambda modifiers with omitted types versus explicitly typed lambda parameters

### Approximate Alternative

Use this label when the older-language version can only approximate the new feature through a different structure or a more manual design.

Examples:

- extension members versus separate extension methods plus helper members
- partial constructors or partial events versus manually split initialization or event wiring
- user-defined compound assignment versus manual operator plus reassignment patterns

## Comparison Priorities

### Priority 1 - Clarity

- The learner must be able to see the difference quickly.
- Avoid examples where the comparison depends on too much surrounding infrastructure.

### Priority 2 - Small Scope

- Prefer small self-contained examples.
- If a feature needs a larger scenario, provide a minimal example first and a richer example later.

### Priority 3 - Honest Trade-Offs

- Do not present the C# 14 version as always superior.
- Note compatibility, tooling, or readability trade-offs where relevant.

## Output Rules

### Runtime-Equivalent Comparisons

If both versions should produce the same runtime result:

- document the shared expected behavior
- emphasize source-level differences separately from runtime equivalence

### Readability-Only Comparisons

If the main difference is expression style and not output:

- state that the teaching value is source readability or maintainability
- avoid inventing artificial output just to make the example executable

### Behavior-Affecting Comparisons

If the new feature changes more than readability:

- document the behavior difference explicitly
- explain whether the older alternative is exact, approximate, or partially equivalent

## Documentation Rules per Comparison

Each comparison should eventually use a stable explanation template with these parts:

- Feature name
- C# 14 version
- Non-C# 14 alternative
- Exact or approximate comparison label
- Main takeaway
- Output or equivalence note
- Migration note

## Naming Rules

- Use the modern feature name as the primary example title.
- Use a subtitle or note for the alternative pattern.
- Avoid naming examples only by compiler-version marketing language.
- Prefer descriptive names such as `Null-Conditional Assignment versus Explicit Null Check`.

## Migration Discussion Rules

- Explain when a team might still prefer the older approach.
- Note when the modern syntax mainly reduces ceremony.
- Note when adopting the newer syntax could affect readability for mixed-experience teams.

## Reuse Rules

- The same comparison pattern should be reusable in DevOps documents, README content, and later UI presentation.
- Metadata fields such as `Summary`, `LearningIntent`, `ExpectedObservation`, and `Determinism` should be fillable from the comparison text without reinvention.

## Anti-Patterns to Avoid

- Do not create comparisons where the alternative is unrelated just to force contrast.
- Do not hide meaningful limitations of the C# 14 feature.
- Do not overcomplicate the alternative example until the learning point is lost.
- Do not use performance claims unless the example or documentation explicitly supports them.

## Notes

- These rules are meant to keep `TestStatements.CS14` educational rather than promotional.
- The comparison framework should support both documentation-first planning and later concrete implementation.

## Done Criteria

- Comparison rules are documented.
- Exact and approximate alternatives are distinguished.
- The pattern supports readability and migration discussion.
