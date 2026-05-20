# T0104A - Project Positioning Baseline

## Parent Backlog Item

- B0102 - Describe Project Entry Points and Learning Paths

## Summary

Document the role of the main repository projects so the `TestStatements` solution can be approached as a structured learning platform with clear entry points.

## Project Roles

### `TestStatements`

Primary role:

- Main exploratory example catalog

Positioning:

- This is the central project for broad topic coverage.
- It contains the widest range of namespace-based examples.
- It is the best starting point for users who want a comprehensive example inventory instead of a guided sequence.

Recommended use:

- Use as the core reference project.
- Use when browsing by topic such as statements, collections, reflection, runtime, or async.
- Use as the source basis for future metadata, grouped execution, and output documentation.

### `Tutorials`

Primary role:

- Curated and linear learning entry point

Positioning:

- This project is suited to readers who want a more guided progression.
- It should be treated as the recommended starting point for learners who prefer staged understanding over breadth.
- It complements `TestStatements` by reducing cognitive load.

Recommended use:

- Start here for a guided introduction.
- Move to `TestStatements` after completing or understanding the curated path.

### `CallAllExamples`

Primary role:

- Aggregate runner and smoke-style execution entry point

Positioning:

- This project is not the best first learning entry point for understanding concepts from scratch.
- Its value is in broad execution coverage and quick visibility into runnable sample behavior.
- It is useful for maintainers and for fast exploratory runs across many public static examples.

Recommended use:

- Use to execute many examples quickly.
- Use as a smoke-style runner after changes.
- Use as a basis for future grouped execution profiles.

### `TestNamespaces`

Primary role:

- Focused sandbox for namespace-specific experiments

Positioning:

- This project isolates namespace topics from the wider sample catalog.
- It is best treated as a specialist side project rather than a general starting point.
- It supports targeted learning around namespace organization, scoping, and collision resolution.

Recommended use:

- Use when the learning question is specifically about namespace behavior.
- Use after basic language familiarity exists.

### `TestStatementsNew`

Primary role:

- Modern presentation-oriented evolution of the example framework

Positioning:

- This project is the natural host for a future browsable example UI.
- It should be treated as the presentation layer candidate rather than the current authoritative source of all examples.
- Its value increases as metadata, grouping, and output descriptions become more explicit.

Recommended use:

- Use as the future landing point for MVVM-based browsing and output presentation.
- Use as a modern companion to the console-based examples, not as a replacement.

## Recommended Learning Paths

### Path A - Guided First Contact

1. Start with `Tutorials`
2. Continue with selected `TestStatements` namespace groups
3. Use `CallAllExamples` for broad runnable verification
4. Explore `TestStatementsNew` once a richer presentation layer evolves

### Path B - Exploratory Topic Browsing

1. Start with `TestStatements`
2. Navigate by namespace group such as `Anweisungen`, `DataTypes`, `Collection.Generic`, `Reflection`, or `Threading.Tasks`
3. Use `CallAllExamples` to observe aggregated execution behavior
4. Use `TestNamespaces` for targeted namespace questions

### Path C - Advanced Runtime and Framework Topics

1. Start with `TestStatements.Reflection`, `Runtime.Loader`, `Runtime.Dynamic`, and `Threading.Tasks`
2. Compare related projects such as `DynamicSample`
3. Review tests and aggregate runs for validation context
4. Extend toward plugin and UI-based scenarios

## Positioning Rules for Future Documentation

- Present `Tutorials` as the easiest guided entry point.
- Present `TestStatements` as the main catalog and authoritative example inventory.
- Present `CallAllExamples` as an execution and validation helper.
- Present `TestNamespaces` as a focused sandbox.
- Present `TestStatementsNew` as the planned modern presentation surface.

## Notes

- This baseline is intended for reuse in README improvements, future UI navigation, and learning-path documentation.
- The positioning reflects the current code base and the active DevOps roadmap, not a future final architecture.

## Done Criteria

- Key project roles are described.
- Likely entry points are identified.
- Multiple learning paths are documented.
