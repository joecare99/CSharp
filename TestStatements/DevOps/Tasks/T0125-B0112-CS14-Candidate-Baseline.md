# T0125A - CS14 Comparison Candidate Baseline

## Parent Backlog Item

- B0112 - Define C# 14 Feature Comparison Scenarios

## Summary

Identify the first concrete C# 14 feature candidates for `TestStatements.CS14` and describe how each can be compared with a non-C# 14 alternative.

## Candidate Selection Principles

- Prefer features that are officially documented in the C# 14 language update.
- Prefer features with clear educational contrast against older syntax.
- Prefer a mix of runtime-visible and readability-oriented comparisons.
- Start with examples that can be explained in small, self-contained source files.

## First Candidate Set

### Candidate 1 - Extension Members

Official feature:

- extension members

Why include it:

- It is one of the clearest new syntax additions in C# 14.
- It expands the familiar extension-method concept in a way that is easy to compare.
- It supports both instance-style and static-style extension scenarios.

Comparison direction:

- C# 14: `extension` blocks with extension properties and static extensions
- Non-C# 14 alternative: classic static extension methods in helper classes, plus ordinary static helper members where needed

Teaching focus:

- Readability improvement
- Grouping of related extension members
- Conceptual difference between extending instances and extending a type surface

Priority:

- High

### Candidate 2 - Field-Backed Properties

Official feature:

- `field` backed properties

Why include it:

- The contrast to older explicit backing-field patterns is direct and easy to understand.
- It creates a strong side-by-side comparison between more ceremony and less ceremony.
- It is suitable for small self-contained examples.

Comparison direction:

- C# 14: accessors using the `field` contextual keyword
- Non-C# 14 alternative: explicit private backing field plus full accessor logic

Teaching focus:

- Reduced boilerplate
- Clearer property intent
- Possible ambiguity or naming caveats when `field` already exists as an identifier

Priority:

- High

### Candidate 3 - Null-Conditional Assignment

Official feature:

- null-conditional assignment

Why include it:

- The comparison to earlier null-check patterns is short and visually obvious.
- It shows a concrete readability and conciseness gain.
- It is suitable for both simple assignment and compound assignment discussions.

Comparison direction:

- C# 14: `customer?.Order = value;`
- Non-C# 14 alternative: explicit null check before assignment

Teaching focus:

- Simplified control flow
- Deferred evaluation of the right-hand side when the receiver is null
- Limitation notes such as lack of `++` or `--` support

Priority:

- High

### Candidate 4 - Unbound Generic Types in `nameof`

Official feature:

- `nameof` support for unbound generic types

Why include it:

- The comparison is small but precise.
- It demonstrates a focused language improvement without requiring a large runtime example.
- It is useful for documentation-oriented and API-oriented code samples.

Comparison direction:

- C# 14: `nameof(List<>)`
- Non-C# 14 alternative: `nameof(List<int>)` or hard-coded type-name string depending on intent

Teaching focus:

- Cleaner type-name expression for generic APIs
- Reduced need for arbitrary type arguments in `nameof`
- Small but meaningful syntax improvement

Priority:

- Medium

### Candidate 5 - Modifiers on Simple Lambda Parameters

Official feature:

- modifiers on simple lambda parameters

Why include it:

- It has a direct side-by-side comparison with older explicitly typed lambda syntax.
- It is useful for showing a focused readability improvement in advanced code.
- It stays close to realistic delegate usage.

Comparison direction:

- C# 14: lambda parameters with modifiers and omitted types where allowed
- Non-C# 14 alternative: explicitly typed lambda parameter list

Teaching focus:

- Reduced syntax noise
- Cases where modifiers are useful in real delegates
- Limitation that `params` still requires explicit typing

Priority:

- Medium

### Candidate 6 - More Partial Members

Official feature:

- partial constructors and partial events

Why include it:

- It extends an already familiar partial-member concept.
- It creates good comparison material for generated-code or split-type discussions.
- It supports architectural conversation, not only syntax novelty.

Comparison direction:

- C# 14: partial constructors and partial events
- Non-C# 14 alternative: non-partial constructor/event patterns, explicit split responsibilities, or manual wiring across partial types

Teaching focus:

- Coordination of defining and implementing declarations
- Where partial members help generated or layered code
- Exact-versus-approximate fallback distinction

Priority:

- Medium

### Candidate 7 - User-Defined Compound Assignment Operators

Official feature:

- user-defined compound assignment operators

Why include it:

- It is a useful advanced comparison topic.
- It connects language evolution to domain-specific type design.
- It may be less beginner-friendly, so it fits later in the first implementation set.

Comparison direction:

- C# 14: explicit user-defined compound assignment operators
- Non-C# 14 alternative: separate binary operator plus manual reassignment patterns

Teaching focus:

- Expressiveness for custom types
- Difference between direct language support and manual composition
- Advanced readability versus complexity trade-offs

Priority:

- Medium

### Candidate 8 - Implicit `Span<T>` and `ReadOnlySpan<T>` Conversions

Official feature:

- more implicit conversions for `Span<T>` and `ReadOnlySpan<T>`

Why include it:

- It is important for modern performance-oriented C#.
- It may require more explanation than the other syntax-oriented features.
- It is valuable, but likely better after the simpler comparison candidates are in place.

Comparison direction:

- C# 14: first-class implicit conversion support around span types
- Non-C# 14 alternative: more explicit conversions, overload choices, or helper code depending on the scenario

Teaching focus:

- Improved ergonomics for span-based APIs
- Reduced ceremony in performance-oriented code
- Need for careful examples because the alternative may depend on context

Priority:

- Lower initial priority

## Suggested Initial Implementation Order

1. Extension members
2. Field-backed properties
3. Null-conditional assignment
4. Unbound generic `nameof`
5. Modifiers on simple lambda parameters
6. More partial members
7. User-defined compound assignment operators
8. Implicit span conversions

## Notes

- The initial order favors features with the clearest educational contrast and the smallest example footprint.
- Some features have exact older-language alternatives, while others require approximate fallback patterns. That distinction should be made explicit in the comparison rules task.
- Official Microsoft Learn documentation identifies these features as part of the C# 14 release and provides the basis for the candidate list.

## Done Criteria

- Candidate features are listed.
- Each candidate has a comparison rationale.
- The list is suitable for phased implementation.
