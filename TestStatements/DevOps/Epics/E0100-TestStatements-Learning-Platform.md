# E0100 - Evolve TestStatements as a Discoverable Learning Platform

## Summary

The repository already contains a broad collection of runnable examples around C#, .NET runtime behavior, desktop UI, plugins, reflection, diagnostics, asynchronous programming, dynamic loading, and specialized experiments.

This epic frames the existing code base as a structured learning platform. The goal is to make current capabilities easier to discover, easier to understand, and easier to extend without losing the educational value of small runnable examples.

## Current Repository Strengths

- Broad namespace coverage in `TestStatements`
- Multi-target comparison across classic .NET Framework and modern .NET
- Aggregated launchers such as `CallAllExamples`
- Focused sandbox projects such as `Tutorials`, `TestNamespaces`, and `DynamicSample`
- Plugin host and plugin contract projects
- Existing test projects for regression checks

## Epic Goal

Turn the current sample collection into a more explicit framework for learning, exploration, and validation by improving discoverability, expected-output documentation, grouped execution, and extension points.

## Scope

- Document the current example catalog and namespace intent
- Improve how examples are grouped, launched, and validated
- Define sensible framework-level extension paths
- Strengthen documentation around output and learning expectations

## Out of Scope

- Full rewrite of existing examples
- Removal of legacy target frameworks that are still part of the educational comparison
- Converting every project into a single architecture style

## Candidate Features

- F0100 - Example Catalog and Learning Map
- F0101 - Grouped Execution and Validation
- F0102 - Modern Example Presentation
- F0103 - Advanced Runtime and Extensibility

## Assumptions

- The source code is the authoritative basis for planning.
- Existing README files are helpful but not yet sufficient as a structured backlog.
- The educational value depends on clear visibility of both intent and observed behavior.

## Open Questions

- Which projects should be treated as the main learning entry point?
- Which examples need stable expected-output documentation across frameworks?
- Which extension areas add the most value without diluting the repository focus?

## Next Refinement Steps

1. Create a feature for catalog and output documentation.
2. Create a feature for grouped execution and validation.
3. Create a feature for modern UI-based exploration.
4. Create a feature for advanced runtime, plugin, and dynamic scenarios.
