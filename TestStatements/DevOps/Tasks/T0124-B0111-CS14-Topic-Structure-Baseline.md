# T0124A - CS14 Topic Structure Baseline

## Parent Backlog Item

- B0111 - Define the TestStatements.CS14 Project Structure

## Summary

Define the internal topic structure for `TestStatements.CS14` so new language features and their alternatives can be discovered, compared, and extended consistently.

## Structural Principle

Organize the project by comparison topics, not only by syntax keyword.

Each topic area should make it easy to answer three questions:

1. What is the new C# 14 feature?
2. What is the non-C# 14 alternative or fallback?
3. What is the main learning takeaway?

## Proposed Topic Groups

### `Overview`

Purpose:

- Introduce the project intent and explain how feature-versus-alternative comparisons should be read.

Content ideas:

- project introduction
- comparison conventions
- notes about exact versus approximate alternatives

### `PrimaryConstructs`

Purpose:

- Group features that change how core language constructs are written or expressed.

Content ideas:

- examples where C# 14 adds a more direct or compact construct
- side-by-side fallback versions using older syntax patterns

### `ObjectModel`

Purpose:

- Group features that affect class, object, member, or type-expression style.

Content ideas:

- examples where the new language version changes how object-oriented code is expressed
- comparisons emphasizing readability and maintainability

### `PatternAndMatching`

Purpose:

- Group features related to pattern-based expression, matching, or more expressive control structures.

Content ideas:

- comparisons between newer compact matching forms and more traditional conditional structures
- examples that show when the alternative becomes more verbose or less expressive

### `CollectionAndDataFlow`

Purpose:

- Group features that affect data shaping, collection handling, or compact data-oriented expression.

Content ideas:

- comparisons where newer language constructs reduce ceremony in data-centric code
- alternatives based on earlier initialization or transformation patterns

### `AsyncAndLambdas`

Purpose:

- Group features that affect local expression style, lambdas, or async-adjacent code patterns where language evolution changes readability.

Content ideas:

- comparisons focusing on reduced ceremony or clearer intent
- notes where runtime behavior stays the same but code expression improves

### `ReadabilityAndMigration`

Purpose:

- Group examples where the main difference is not runtime output but source readability, maintainability, or migration trade-offs.

Content ideas:

- examples whose primary teaching value is comparison commentary
- notes on when an older alternative is still preferable for compatibility reasons

## Comparison Unit Structure

Each individual comparison topic should eventually contain:

- a modern C# 14 example
- a non-C# 14 alternative
- a concise explanation of the main difference
- notes on readability, expressiveness, or migration relevance
- expected output or expected equivalence notes where runtime behavior matters

## Folder or File Strategy

Recommended approach:

- one comparison topic per file where practical
- optionally grouped under folders matching the topic groups above
- avoid large monolithic files that mix many unrelated language features

This keeps the project aligned with the repository preference for clear structure and later metadata reuse.

## Extension Guidance

- Add new topic groups only when several features justify them.
- Prefer stable conceptual groups over compiler-version marketing labels.
- Allow one feature to reference another when the comparison naturally overlaps, but keep one primary home for discoverability.

## Notes

- The structure is intentionally comparison-first, because the educational value of `TestStatements.CS14` lies in contrast rather than in listing features without context.
- Exact topic names can still be refined once the first concrete C# 14 candidates are selected in T0125.

## Done Criteria

- Topic grouping is proposed.
- The structure supports discoverability.
- The structure supports comparison-oriented learning and later extension.
