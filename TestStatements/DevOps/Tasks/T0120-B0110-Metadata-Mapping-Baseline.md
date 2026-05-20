# T0120A - Metadata Mapping Baseline

## Parent Backlog Item

- B0110 - Map Metadata to Current Example Groups and Execution Profiles

## Summary

Define how the metadata model connects to the DevOps baselines that already describe namespace groups, learning intent, expected output, project positioning, and execution profiles.

## Mapping to Existing Baselines

### Namespace Inventory Baseline

Source baseline:

- `T0100A - Namespace Inventory Baseline for TestStatements`

Mapped metadata fields:

- `Project`
- `NamespaceGroup`
- `PrimarySource`
- `RepresentativeMethods`

Purpose:

- Connect metadata entries to the current code structure and representative example files.

### Namespace Learning-Intent Baseline

Source baseline:

- `T0101A - Namespace Learning Intent Baseline`

Mapped metadata fields:

- `Summary`
- `LearningIntent`
- `Prerequisites`
- `RelatedExamples`

Purpose:

- Reuse the documented learning meaning of each namespace group instead of redefining it per consumer.

### Output-Sensitive Example Baseline

Source baseline:

- `T0102A - Output-Sensitive Example Baseline`

Mapped metadata fields:

- `OutputSensitivity`
- `ExpectedObservation`
- `Determinism`

Purpose:

- Prioritize output-sensitive examples and describe how strongly their observed behavior matters.

### Advanced Expected-Output Notes Baseline

Source baseline:

- `T0103A - Advanced Expected-Output Notes Baseline`

Mapped metadata fields:

- `ExpectedObservation`
- `Determinism`
- `OutputArtifacts`
- `Notes`

Purpose:

- Reuse advanced observation rules for async, diagnostics, reflection, runtime, and serialization scenarios.

### Project Positioning Baseline

Source baseline:

- `T0104A - Project Positioning Baseline`

Mapped metadata fields:

- `Project`
- `Summary`
- `Prerequisites`
- `Notes`

Purpose:

- Ensure metadata reflects whether an example belongs to the main catalog, a guided project, a focused sandbox, or a presentation-oriented surface.

### Launcher Flow Baseline

Source baseline:

- `T0105A - Launcher Flow Baseline`

Mapped metadata fields:

- `RepresentativeMethods`
- `ExecutionProfile`
- `ExpectedObservation`
- `ValidationTargets`

Purpose:

- Support future launcher filtering, user-facing labels, and smoke-style execution planning.

### Run-Group Mapping Baseline

Source baseline:

- `T0106A - Run-Group Mapping Baseline`

Mapped metadata fields:

- `ExecutionProfile`
- `Tags`
- `RelatedExamples`

Purpose:

- Place examples into coherent themed runs and support grouped navigation.

### Execution-Profile Naming Baseline

Source baseline:

- `T0107A - Execution Profile Naming Baseline`

Mapped metadata fields:

- `ExecutionProfile`
- `Summary`
- `Tags`

Purpose:

- Reuse stable profile identifiers and labels across documentation, launcher filters, and future UI groupings.

## Mapping Priorities

### First Priority

- `Project`
- `NamespaceGroup`
- `PrimarySource`
- `ExecutionProfile`
- `Summary`

Reason:

- These fields create the minimum link between code structure, catalog use, and grouped execution.

### Second Priority

- `LearningIntent`
- `ExpectedObservation`
- `OutputSensitivity`
- `Determinism`
- `RepresentativeMethods`

Reason:

- These fields connect the metadata model to the existing documentation and output baselines.

### Third Priority

- `Tags`
- `Prerequisites`
- `RelatedExamples`
- `ValidationTargets`
- `Notes`
- `OutputArtifacts`

Reason:

- These fields add navigation, validation, and advanced context once the core mapping is stable.

## Mapping Guidance

- Do not duplicate full baseline text in metadata entries; reference or summarize it.
- Use metadata fields to capture reusable distilled meaning, not entire narrative documents.
- Prefer stable conceptual mappings over temporary implementation details.

## Notes

- This mapping baseline is intended to minimize duplication across the DevOps planning artifacts while making future metadata entries meaningful.
- The mapping also prepares a future transition from planning documents to concrete metadata instances in code or content files.

## Done Criteria

- Existing baselines are connected to metadata fields.
- Priority order for field mapping is defined.
- The mapping supports future implementation and phased adoption.
