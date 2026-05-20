# T0118A - Metadata Shape Baseline

## Parent Backlog Item

- B0109 - Define Reusable Metadata for Examples

## Summary

Define the first reusable metadata shape for repository examples so that the same descriptive information can later drive documentation, grouped execution, UI presentation, and validation mapping.

## Metadata Design Goals

- Describe examples consistently across projects
- Separate stable identity fields from optional presentation fields
- Support both human-readable documentation and future tooling
- Reuse existing DevOps baselines for namespace, output, project, and run-group context

## Proposed Baseline Metadata Shape

### Required Fields

- `Id`
  - Stable example identifier
  - Used for references in documentation, UI, and validation mapping
- `Title`
  - Short human-readable example name
  - Used in README files, UI lists, and summaries
- `Project`
  - Owning project such as `TestStatements`, `CallAllExamples`, or `TestStatementsNew`
  - Used for project-level grouping and navigation
- `NamespaceGroup`
  - Main conceptual area such as `Anweisungen`, `DataTypes`, or `Reflection`
  - Used for catalog grouping and learning paths
- `PrimarySource`
  - Main source file or entry file for the example
  - Used for traceability to code
- `ExecutionProfile`
  - Profile such as `LanguageBasics`, `CollectionsAndQueries`, or `AsyncAndTasks`
  - Used for grouped execution and filtering
- `Summary`
  - Short explanation of what the example teaches
  - Used across documentation and future UI surfaces

### Recommended Fields

- `LearningIntent`
  - More explicit educational purpose than the short summary
  - Reuses the namespace learning-intent work already documented
- `ExpectedObservation`
  - Main visible outcome the learner should observe
  - Connects to expected-output documentation
- `OutputSensitivity`
  - Classification such as `Low`, `Medium`, or `High`
  - Helps prioritize output documentation and validation work
- `Determinism`
  - Classification such as `Deterministic`, `Illustrative`, or `EnvironmentSensitive`
  - Helps interpret observed behavior correctly
- `TargetFrameworkScope`
  - Notes whether the example is cross-target, modern-only, legacy-only, or framework-weighted
  - Helps compare behavior across runtimes
- `RepresentativeMethods`
  - Key methods or entry points for execution
  - Useful for launchers and browsing
- `Tags`
  - Short topic labels such as `async`, `reflection`, `json`, `linq`, or `di`
  - Useful for search and filtering

### Optional Extension Fields

- `Prerequisites`
  - What the learner should already understand before using the example
- `RelatedExamples`
  - Links to nearby examples for learning progression
- `ValidationTargets`
  - Related tests, smoke runs, or validation notes
- `Notes`
  - Additional context, caveats, or extension hints
- `OutputArtifacts`
  - Files, generated assemblies, or other artifacts that the example may create

## Example Metadata Sketch

Illustrative metadata shape for a reflection-oriented example:

- `Id`: `TS-REF-ASM-001`
- `Title`: `Assembly Metadata Inspection`
- `Project`: `TestStatements`
- `NamespaceGroup`: `Reflection`
- `PrimarySource`: `TestStatements/Reflection/AssemblyExample.cs`
- `ExecutionProfile`: `DiagnosticsAndReflection`
- `Summary`: `Shows how assembly metadata can be inspected and printed at runtime.`
- `LearningIntent`: `Teach runtime introspection and assembly metadata discovery.`
- `ExpectedObservation`: `Visible assembly and type-related metadata output.`
- `OutputSensitivity`: `High`
- `Determinism`: `Illustrative`
- `TargetFrameworkScope`: `CrossTarget`
- `RepresentativeMethods`: `ExampleMain`
- `Tags`: `reflection`, `assembly`, `metadata`

## Usage by Consumer

### Documentation

Use metadata to generate or maintain:

- namespace catalog entries
- project overviews
- expected-output summaries
- learning-path references

### Launcher and Execution Profiles

Use metadata to:

- map examples into profile-based runs
- filter examples by execution profile or tag
- show more meaningful labels than raw reflection output

### UI Presentation

Use metadata to:

- populate list and detail views
- show summary, learning intent, and expected observation
- support grouping, filtering, and future search

### Validation

Use metadata to:

- map examples to tests or smoke runs
- highlight output-sensitive examples
- track environment-sensitive or illustrative behaviors

## Baseline Rules

- Keep the metadata shape small enough for manual maintenance.
- Prefer stable identifiers and clear summaries over premature detail.
- Allow optional fields to remain empty until their consumer exists.
- Reuse previously documented terms such as namespace groups and execution profiles instead of inventing parallel labels.

## Follow-Up Candidates

- Define ownership and maintenance rules for metadata entries.
- Define how metadata maps to the existing DevOps baselines.
- Identify the first examples or groups that should receive concrete metadata instances.

## Done Criteria

- Baseline fields are defined.
- Required and optional fields are distinguished.
- The metadata shape is connected to documentation, launcher, UI, and validation use cases.
