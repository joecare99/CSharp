# T0119A - Metadata Usage Rules Baseline

## Parent Backlog Item

- B0109 - Define Reusable Metadata for Examples

## Summary

Define the baseline rules for maintaining and reusing example metadata so future documentation, launcher behavior, UI presentation, and validation mapping can rely on the same information consistently.

## Ownership Assumptions

- The source code remains the authoritative source for what an example actually does.
- Metadata is the authoritative descriptive layer for how an example is cataloged, grouped, and presented.
- DevOps baseline documents define the planning and documentation meaning of metadata before any implementation format is introduced.
- Future implementation files should not silently contradict the documented DevOps baseline.

## Maintenance Rules

### Rule 1 - Update Metadata When Example Meaning Changes

If an example changes in learning purpose, output behavior, project ownership, or execution grouping, its metadata must be reviewed as part of the same change.

### Rule 2 - Keep Stable Identity Fields Stable

Fields such as `Id`, `Project`, `NamespaceGroup`, and `PrimarySource` should not change without a clear reason. If a change is unavoidable, related documentation and validation mappings must be updated as well.

### Rule 3 - Prefer Reusing Existing Vocabulary

Metadata values should reuse existing documented concepts such as namespace groups, execution profiles, output sensitivity, and framework scope instead of inventing parallel labels.

### Rule 4 - Use Optional Fields Gradually

Optional metadata fields should be introduced only when they support a real consumer such as documentation, launcher filtering, UI presentation, or validation mapping.

### Rule 5 - Distinguish Observation from Guarantee

Metadata should clearly distinguish between deterministic expectations and illustrative or environment-sensitive observations.

## Authority by Field Category

### Identity Fields

Examples:

- `Id`
- `Project`
- `NamespaceGroup`
- `PrimarySource`

Authority rule:

- These fields should be derived from the repository structure and maintained with strong stability expectations.

### Learning and Presentation Fields

Examples:

- `Title`
- `Summary`
- `LearningIntent`
- `ExpectedObservation`

Authority rule:

- These fields should align with the DevOps learning-intent and output baselines.
- They are descriptive and may be refined for clarity, but should not drift away from code reality.

### Execution and Validation Fields

Examples:

- `ExecutionProfile`
- `OutputSensitivity`
- `Determinism`
- `ValidationTargets`

Authority rule:

- These fields should align with execution-profile baselines, expected-output baselines, and later validation planning.

### Extension and Navigation Fields

Examples:

- `Tags`
- `RelatedExamples`
- `Prerequisites`
- `Notes`
- `OutputArtifacts`

Authority rule:

- These fields are helpful secondary aids and may evolve as consumers mature.
- They should not override the primary identity and learning fields.

## Reuse Rules by Consumer

### Documentation

- Use metadata summaries and learning intent for catalog and overview text.
- Do not duplicate large custom descriptions when metadata can provide the shared wording.

### Launcher

- Use metadata execution profiles, tags, and representative methods for grouping and filtering.
- Do not derive user-facing launcher labels from raw type names when metadata can provide clearer naming.

### UI

- Use metadata for display labels, grouping, summaries, and expected-observation text.
- Keep UI-specific formatting outside the metadata itself.

### Validation

- Use metadata to identify output-sensitive and environment-sensitive examples.
- Do not treat illustrative metadata as a strict assertion source unless validation rules explicitly require that.

## Change Management Rules

- Review metadata whenever a new example is added.
- Review metadata whenever an example is moved between conceptual groups.
- Review metadata whenever output behavior changes in a learner-visible way.
- Review metadata whenever new execution profiles or framework-scope categories are introduced.

## Minimal Governance Model

- Source changes trigger metadata review.
- Metadata changes should be traceable to an example, a documentation need, or a tooling consumer.
- Metadata should first be introduced for the highest-value areas before aiming for full repository coverage.

## Notes

- These rules are a baseline for future implementation, not yet an enforcement mechanism.
- The rules are designed to keep metadata helpful without making maintenance disproportionately expensive.

## Done Criteria

- Ownership assumptions are explicit.
- Maintenance and reuse rules are defined.
- The rules support future documentation, launcher, UI, and validation consumers.
