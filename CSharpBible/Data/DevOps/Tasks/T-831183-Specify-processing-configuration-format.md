# Task T-831183 - Specify processing configuration format

## Status

Draft

## Parent

- Backlog Item `BI-830363` - `Create canonical trace processing model`

## Goal

Define the persisted configuration format used to describe trace-processing pipelines for both graphical authoring and console execution.

## Current Domain Context

- Canonical trace data is already exchanged through a shared `ITraceDataSet` model
- Input and output filters already support reusable non-UI import and export workflows
- A future WPF workbench should help users author processing definitions graphically
- The same saved definition should later be executable by a console-oriented runner
- The first increment focuses on deterministic, understandable configurations over maximum expressiveness
- Some useful operations are naturally multi-output, for example bit-splitting of an integer channel or `sin`/`cos` decomposition of one angle channel

## Scope

- Define the root document structure for a saved processing configuration
- Define how one or more ordered processing steps are represented
- Define references to input channels, derived channels, parameters, and output names
- Define how a single processing step can declare one or more derived output channels
- Define configuration metadata such as name, description, version, and provenance
- Define validation expectations for incomplete or inconsistent configurations
- Define how unit-related metadata is represented when available or unknown
- Keep the format suitable for UI editing and non-UI execution

## Out of Scope

- Full implementation of pipeline execution
- Rich graphical editing workflow details
- Advanced schema migration strategy beyond a first version marker

## Implementation Notes

- Prefer a deterministic, human-readable format such as JSON unless later refinement proves otherwise
- Keep the structure compatible with multi-targeted .NET services and simple serialization approaches
- Distinguish clearly between pipeline definition, runtime input selection, and exported output data
- Leave room for later support of multiple named pipelines if needed, but document the first-increment assumption explicitly
- Do not artificially restrict one step to exactly one output when the operation semantics are intrinsically multi-output

## Finalized First-Increment Decisions

- Use `JSON` as the persisted processing configuration format
- Use **one executable pipeline per configuration file** in the first increment
- Keep the configuration independent from a specific UI workspace file
- Keep runtime input and output file paths outside the main pipeline definition where practical
- Store enough metadata for user-facing inspection, diagnostics, provenance, and later migration
- Prefer explicit operation parameters over free-form formulas in the first increment
- Allow **one or more outputs per step**, depending on the selected operation
- Keep single-output operations simple, but model outputs as a collection from the start

## Root Document Baseline

The configuration document should contain one root object with the following top-level sections:

1. `format`
   - Stable document marker, for example `TraceAnalysis.ProcessingConfig/1.0`
2. `metadata`
   - Human-oriented information such as configuration name, description, authoring tool hint, and timestamps
3. `pipeline`
   - Ordered list of executable processing steps
4. `defaults`
   - Optional baseline behavior for units, missing values, and timestamp assumptions
5. `validation`
   - Optional persisted validation snapshot or ruleset version marker when useful

## Metadata Baseline

`metadata` should support at least:

- `name`
- `description`
- `createdUtc`
- `updatedUtc`
- `authoringTool`
- `configurationId`
- `sourceProvenance`

The metadata section should remain descriptive and should not be required to execute the pipeline except for the format/version marker.

## Pipeline Baseline

`pipeline` should contain an ordered array named `steps`.

Each step should include at least:

- `id`
  - Stable identifier within the configuration
- `operation`
  - Supported operation name
- `inputs`
  - Ordered references to source or derived channels
- `outputs`
  - Ordered collection of one or more derived channel definitions
- `parameters`
  - Operation-specific settings
- `enabled`
  - Optional boolean, default `true`
- `description`
  - Optional explanatory text for UI readability

`outputs` should always be a collection, even for single-output operations. This keeps the model uniform for single-output and multi-output steps.

## Channel Reference Baseline

Each entry in `inputs` should use an explicit object rather than a positional string. The first-increment reference shape should support:

- `kind`
  - `source` or `derived`
- `channel`
  - Stable channel name reference
- `stepId`
  - Optional link to the producing step when `kind` is `derived`

This keeps references clear in both graphical editing and console diagnostics.

## Derived Output Baseline

Each entry in `outputs` should include:

- `channel`
  - Final derived channel name
- `displayName`
  - Optional UI-facing display label
- `unit`
  - Optional explicit unit string
- `unitBehavior`
  - For example `inherit`, `explicit`, `computed`, `unknown`
- `description`
  - Optional explanation of the derived signal
- `outputRole`
  - Optional semantic label for multi-output steps, for example `bit0`, `bit1`, `sin`, `cos`, `magnitude`

## Output Cardinality Baseline

The configuration format should support these step categories:

- **single-output steps**
  - for example `differentiate`, `integrate`, `scale`, `offset`
- **fixed multi-output steps**
  - for example `sinCos`, `cartesianFromAngleMagnitude`, `bitSplit16`
- **parameter-driven multi-output steps**
  - for example a bit extractor that emits only selected bits or a splitter that emits a configured set of subchannels

The format should therefore validate:

- minimum output count
- maximum output count where applicable
- semantic completeness of output roles for known multi-output operations

## Parameter Baseline by Operation Family

### Calculus-style operations

For `differentiate` and `integrate`, parameters should leave room for:

- `timeReference`
  - Usually timestamp-based
- `samplingMode`
  - For example `source-interval`, `fixed-step`
- `fixedStep`
  - Optional numeric timestep for later extensions
- `missingValueBehavior`
  - For example `fail`, `skip`, `propagate-null`

### Arithmetic operations

For `add`, `subtract`, `multiply`, and `divide`, parameters should leave room for:

- `missingValueBehavior`
- `divideByZeroBehavior`
  - Relevant for division only
- `numericTypeHint`
  - Optional future hint for result typing

### Scalar operations

For `scale` and `offset`, parameters should include:

- `value`
  - Scalar numeric value
- `missingValueBehavior`

### Multi-output helper operations

For operations such as bit extraction or trigonometric decomposition, parameters should leave room for:

- `bitWidth`
  - For example `16`
- `selectedBits`
  - Optional subset of bits to expose
- `angleUnit`
  - For example `deg`, `rad`
- `outputMode`
  - Optional mode selector for known multi-output variants

## Defaults Baseline

`defaults` should support configuration-wide fallback settings such as:

- `timestampMode`
- `missingValueBehavior`
- `unitValidationMode`
  - For example `none`, `warn`, `strict`
- `namingConvention`
  - Optional naming pattern for derived channels

These defaults must be overridable by individual steps where needed.

## Validation Baseline

The configuration loader or editor should validate at least:

- `format` is present and supported
- `metadata.name` is present for user-created configurations
- `pipeline.steps` exists and is not empty
- each `step.id` is unique
- each derived output channel name is unique within the pipeline result set
- each `operation` is supported by the configured version
- required input references exist
- derived references do not point to later steps
- required parameters exist and are type-compatible
- explicit units are syntactically present when `unitBehavior` is `explicit`
- output count matches the selected operation contract

## Unit Representation Baseline

The first increment should treat units as lightweight metadata strings with an explicit behavior flag.

Recommended baseline:

- `unit`
  - String such as `m`, `m/s`, `A`, `V`, or `unknown`
- `unitBehavior`
  - `inherit`
  - `explicit`
  - `computed`
  - `unknown`
- `unitValidationMode`
  - Configuration or runtime validation strength

This avoids blocking the first increment on a full physical-unit algebra model while still keeping unit concerns visible and testable.

## Example Shape

```json
{
  "format": "TraceAnalysis.ProcessingConfig/1.0",
  "metadata": {
    "name": "Angle decomposition and bit extraction",
    "description": "Create sin/cos channels and split status bits.",
    "createdUtc": "2026-01-15T10:00:00Z",
    "updatedUtc": "2026-01-15T10:05:00Z",
    "authoringTool": "TraceAnalysis.Wpf/0.1",
    "configurationId": "cfg-angle-bits"
  },
  "defaults": {
    "timestampMode": "source",
    "missingValueBehavior": "warn",
    "unitValidationMode": "warn"
  },
  "pipeline": {
    "steps": [
      {
        "id": "step-angle-sincos",
        "operation": "sinCos",
        "inputs": [
          {
            "kind": "source",
            "channel": "Angle"
          }
        ],
        "outputs": [
          {
            "channel": "AngleSin",
            "outputRole": "sin",
            "unit": "1",
            "unitBehavior": "explicit"
          },
          {
            "channel": "AngleCos",
            "outputRole": "cos",
            "unit": "1",
            "unitBehavior": "explicit"
          }
        ],
        "parameters": {
          "angleUnit": "deg"
        },
        "enabled": true
      },
      {
        "id": "step-status-bits",
        "operation": "bitSplit",
        "inputs": [
          {
            "kind": "source",
            "channel": "StatusWord"
          }
        ],
        "outputs": [
          {
            "channel": "StatusBit0",
            "outputRole": "bit0",
            "unit": "1",
            "unitBehavior": "explicit"
          },
          {
            "channel": "StatusBit1",
            "outputRole": "bit1",
            "unit": "1",
            "unitBehavior": "explicit"
          }
        ],
        "parameters": {
          "bitWidth": 16,
          "selectedBits": [0, 1]
        },
        "enabled": true
      }
    ]
  }
}
```

## Invalid Configuration Examples to Cover

- Missing `format`
- Unsupported format version
- Empty `steps`
- Duplicate `step.id`
- Duplicate derived output channel name
- `derived` input reference without valid producing `stepId`
- Unsupported `operation`
- `scale` without numeric scalar value
- `sinCos` with only one output
- `bitSplit` with invalid or duplicate selected bit definitions

## Proposed Decision Areas

- Single pipeline per configuration file versus multiple named pipelines
- Stable identifiers for steps and derived outputs
- Parameter representation for arithmetic and calculus-style operations
- Modeling of multi-output operations and output-role semantics
- Error and warning handling during configuration load and validation
- Unit representation strategy for source and derived channels

## Test Strategy

- Add specification examples for valid and invalid configurations
- Define roundtrip expectations for save and reload scenarios
- Define validation cases for missing channel references, duplicate output names, and unsupported operations
- Define compatibility checks between UI-authored files and console-loaded files
- Define version-marker behavior for unsupported or future configuration revisions

## Done Criteria

- Configuration document structure is defined
- Versioning baseline is defined
- Validation baseline is defined
- Unit-related metadata representation is documented
- UI authoring and console execution compatibility is documented
- A first-increment example configuration is documented
