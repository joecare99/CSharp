# T0127A - CS14 Documentation Template Baseline

## Parent Backlog Item

- B0113 - Define Documentation and Expected-Output Strategy for C# 14 Comparisons

## Summary

Define a reusable documentation template for `TestStatements.CS14` examples so each comparison can be explained in a consistent, comparison-first format.

## Template Goals

- Make each comparison easy to scan
- Keep modern and alternative code paths clearly separated
- Preserve both educational clarity and future metadata reuse
- Support examples with and without meaningful runtime output

## Proposed Documentation Template

### 1. Title

- Use a descriptive comparison-oriented title
- Preferred pattern: `<Feature> versus <Alternative>`

Example:

- `Null-Conditional Assignment versus Explicit Null Check`

### 2. Feature Summary

- Briefly state what the C# 14 feature is
- Explain what friction or verbosity it reduces

### 3. Comparison Type

- Mark whether the alternative is `Exact` or `Approximate`
- If approximate, explain why the older version cannot mirror the new syntax directly

### 4. Modern C# 14 Version

- Show the C# 14 example
- Keep the example as small as possible while still demonstrating the feature

### 5. Non-C# 14 Alternative

- Show the fallback or older-language alternative
- Keep the alternative honest and readable, not artificially poor

### 6. Main Difference

- Explain the source-level difference in a few sentences
- Highlight readability, expressiveness, ceremony, or structural change

### 7. Expected Behavior or Output

- State whether both versions behave the same at runtime
- If output exists, describe the expected result or equivalence
- If the comparison is readability-only, say so explicitly

### 8. Migration Note

- Explain when a team might adopt the new syntax
- Explain when an older alternative may still be preferred for compatibility or readability reasons

### 9. Learning Takeaway

- End with a short statement of the key lesson
- This should be reusable as metadata summary or UI summary text later

## Template Variant Guidance

### Variant A - Runtime-Equivalent Comparison

Use when both versions should behave the same and the main contrast is source code style.

Best for:

- field-backed properties
- null-conditional assignment
- lambda parameter modifiers
- unbound generic `nameof`

### Variant B - Structurally Approximate Comparison

Use when the older-language alternative needs a different structure, not just more syntax.

Best for:

- extension members
- partial constructors and events
- user-defined compound assignment operators

### Variant C - API-Ergonomics Comparison

Use when the main difference is how naturally code can be written around a feature or API surface.

Best for:

- implicit span conversions
- feature cases where source ergonomics matter more than visible output

## Reuse with Metadata

The documentation template should support later population of metadata fields such as:

- `Title`
- `Summary`
- `LearningIntent`
- `ExpectedObservation`
- `Determinism`
- `Notes`

## Reuse with Future UI

The same sections can later map to a detail page or comparison panel in a future browser:

- title
- summary
- modern example
- alternative example
- difference note
- migration note
- takeaway

## Anti-Patterns to Avoid

- Do not overload a single comparison page with too many unrelated variations.
- Do not hide the alternative just because the modern syntax is shorter.
- Do not use a contrived alternative that no reasonable developer would write.
- Do not force runtime output into examples whose value is primarily source readability.

## Notes

- The template is intentionally comparison-first rather than API-reference-first.
- The section order is designed to work in DevOps, README content, and future UI presentations.

## Done Criteria

- Template sections are defined.
- The template supports comparison-oriented explanation.
- The template can be reused across multiple CS14 feature examples.
