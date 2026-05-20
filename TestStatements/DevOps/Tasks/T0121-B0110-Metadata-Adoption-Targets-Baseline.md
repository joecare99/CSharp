# T0121A - Metadata Adoption Targets Baseline

## Parent Backlog Item

- B0110 - Map Metadata to Current Example Groups and Execution Profiles

## Summary

Define the first repository areas that should adopt metadata-driven workflows so the metadata foundation can be introduced incrementally with visible value.

## Adoption Strategy

Introduce metadata in the order of highest reuse value and lowest ambiguity.

This means starting with areas where:

- the example grouping is already well documented
- the expected output is already partially described
- multiple future consumers can benefit immediately

## Phase 1 - Documentation Reuse

Primary targets:

- namespace catalog summaries
- learning-intent summaries
- expected-observation snippets for high-priority areas

Reason:

- Documentation is the fastest place to prove metadata value.
- Existing DevOps baselines already provide the source meaning.
- This phase creates visible consistency with relatively low implementation risk.

Expected benefit:

- Reusable summary text
- Reduced duplication across planning and future README updates
- Clear first set of metadata instances

## Phase 2 - Execution Profile and Launcher Use

Primary targets:

- execution-profile grouping
- launcher labeling and filtering design
- representative method mapping for aggregate runs

Reason:

- The run-group and profile baselines are already in place.
- Launcher improvements benefit from stable metadata names and group assignments.
- This phase turns metadata into an operational organizing tool, not only a documentation layer.

Expected benefit:

- Better grouped execution planning
- Future filterable launcher behavior
- Clearer mapping from example to execution profile

## Phase 3 - UI Preparation

Primary targets:

- example list summaries for `TestStatementsNew`
- grouped navigation metadata
- learner-facing detail information such as summary and expected observation

Reason:

- UI value depends on having stable metadata first.
- The modern presentation layer benefits strongly from reusable descriptive fields.
- Earlier documentation and launcher work reduce ambiguity before UI adoption.

Expected benefit:

- Strong basis for MVVM-driven grouping and browsing
- Reduced duplication between planning text and UI content
- Better discoverability in a future example browser

## Phase 4 - Validation Mapping

Primary targets:

- output-sensitive example classification
- mapping examples to smoke runs or targeted tests
- tracking illustrative versus deterministic behavior

Reason:

- Validation becomes more valuable once grouping and expected observations are already clear.
- This phase should build on the earlier documentation and execution-profile alignment.

Expected benefit:

- Better traceability from example to validation intent
- Improved prioritization for smoke checks and future test additions

## Suggested First Concrete Adoption Areas

### First Wave

- `TestStatements.Reflection`
- `TestStatements.Threading.Tasks`
- `TestStatements.Diagnostics`
- `TestStatements.DataTypes`

Why these first:

- They have high learning value.
- They have strong output or observation relevance.
- They benefit immediately from summary, expected observation, and determinism fields.

### Second Wave

- `TestStatements.Collection.Generic`
- `TestStatements.Linq`
- `TestStatements.SystemNS`
- `TestStatements.Runtime.Loader`
- `TestStatements.Runtime.Dynamic`

Why these next:

- They fit well into execution profiles and grouped learning.
- They gain value from metadata but may need slightly more interpretation or artifact-aware notes.

### Third Wave

- `TestStatements.ClassesAndObjects`
- `TestStatements.Helper`
- `TestStatements.Constants`
- `TestStatements.CS_Concepts`
- `TestStatements.DependencyInjection`

Why later:

- These areas remain important, but some are less output-driven and some are more architecture-weighted, so early metadata pressure is lower.

## Rollout Rules

- Start with a small set of high-value examples or groups.
- Confirm that the metadata shape is useful before scaling broadly.
- Prefer consistency in a few areas over incomplete coverage everywhere.
- Revisit the shape if a real consumer needs additional fields.

## Notes

- This rollout baseline is intentionally phased so the repository can gain value before full metadata coverage exists.
- The order reflects current DevOps baselines and expected consumer value, not an absolute restriction.

## Done Criteria

- Adoption phases are defined.
- First concrete targets are listed.
- The rollout order is justified by reuse value and implementation clarity.
