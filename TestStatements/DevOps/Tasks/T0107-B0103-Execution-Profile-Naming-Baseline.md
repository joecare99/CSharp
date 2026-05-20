# T0107A - Execution Profile Naming Baseline

## Parent Backlog Item

- B0103 - Define Grouped Execution Profiles for Examples

## Summary

Define consistent names and short intent statements for execution profiles so they can be reused across documentation, launcher options, UI grouping, and validation planning.

## Naming Principles

- Use short PascalCase profile names for implementation-facing identifiers.
- Use descriptive English display labels for documentation and UI surfaces.
- Prefer names that reflect learner intent instead of internal folder structure alone.
- Keep profile names stable enough for reuse across launcher filters and future metadata.

## Proposed Execution Profiles

### `LanguageBasics`

Display label:

- Language Basics

Intent:

- Introduce fundamental syntax, statements, and control-flow behavior.

Typical use:

- Beginner-friendly starting profile
- Quick validation of foundational language examples

### `TypesAndFormatting`

Display label:

- Types and Formatting

Intent:

- Demonstrate type concepts, string behavior, enums, and visible formatting results.

Typical use:

- Beginner to intermediate learning path
- Output-oriented demonstrations with stable or semi-stable visible results

### `CollectionsAndQueries`

Display label:

- Collections and Queries

Intent:

- Demonstrate generic collections, lookup behavior, ordering, and LINQ thinking.

Typical use:

- Intermediate learning profile
- Good candidate for grouped smoke runs after collection-related changes

### `ObjectModelAndReuse`

Display label:

- Object Model and Reuse

Intent:

- Demonstrate interfaces, members, extension methods, and reusable abstractions.

Typical use:

- Concept and design-oriented exploration
- Helpful profile for documentation and browsing even when output is not the main focus

### `DiagnosticsAndReflection`

Display label:

- Diagnostics and Reflection

Intent:

- Demonstrate runtime observation, timing, metadata inspection, and reflective discovery.

Typical use:

- Advanced learning profile
- Strong candidate for expected-output notes and validation mapping

### `RuntimeAndDynamic`

Display label:

- Runtime and Dynamic Behavior

Intent:

- Demonstrate loader behavior, runtime-generated artifacts, and dynamic execution scenarios.

Typical use:

- Advanced framework-focused profile
- Natural bridge to comparison samples and plugin-related extensions

### `SystemIntegration`

Display label:

- System Integration

Intent:

- Demonstrate practical framework API usage such as serialization, XML, and printing.

Typical use:

- Intermediate to advanced platform-oriented learning profile
- Good candidate for file-output and environment-aware documentation

### `AsyncAndTasks`

Display label:

- Async and Tasks

Intent:

- Demonstrate task orchestration, async flow, sequencing, and related runtime observations.

Typical use:

- Advanced output-sensitive profile
- Strong candidate for grouped runs that show behavioral differences between orchestration styles

### `ModernArchitecture`

Display label:

- Modern Architecture

Intent:

- Demonstrate dependency injection, service composition, and architecture-oriented patterns.

Typical use:

- Modern .NET learning profile
- Candidate for later alignment with plugin and MVVM-oriented scenarios

## Suggested Usage Modes

- Documentation mode: use display labels and short intent text
- Launcher mode: use PascalCase identifiers as filter names
- UI mode: use display labels, summary text, and metadata-backed grouping
- Validation mode: use profile identifiers to map tests and smoke runs to example clusters

## Stability Guidance

- Keep the identifier stable even if the display text is refined.
- Add new profiles only when a clear thematic gap exists.
- Avoid profile names that depend on temporary implementation details.

## Done Criteria

- Profile names are consistent.
- Each profile has a clear intent statement.
- Usage across documentation, launchers, UI, and validation is defined.
