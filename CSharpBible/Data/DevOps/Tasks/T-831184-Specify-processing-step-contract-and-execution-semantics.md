# Task T-831184 - Specify processing-step contract and execution semantics

## Status

Draft

## Parent

- Backlog Item `BI-830363` - `Create canonical trace processing model`

## Goal

Define the processing-step contract and the execution semantics for applying ordered transformations to canonical trace datasets.

## Current Domain Context

- The canonical exchange model already represents trace data as ordered records with a mandatory timestamp and optional channel values
- Processing configurations are planned as persisted JSON documents that can be authored in WPF and executed in a console workflow
- The first processing increment should favor deterministic, explainable behavior over highly optimized or mathematically exhaustive behavior
- Derived channels must remain exportable through existing CSV, JSON, and Excel filter paths
- Units matter, but the first increment should avoid blocking on a full formal unit algebra engine
- Some operations are naturally multi-output, for example extracting individual bits from a status word or generating `sin` and `cos` from one angle input

## Scope

- Define the contract for a processing step
- Define how a single step may produce multiple derived outputs
- Define pipeline execution order and dependency behavior
- Define how raw and derived channels coexist during execution
- Define semantics for the first operations:
  - differentiation
  - integration
  - addition
  - subtraction
  - multiplication
  - division
  - scaling
  - offset
  - bit splitting / bit extraction
  - `sin` / `cos` decomposition from one angle input
- Define behavior for missing values, irregular timestamps, and invalid channel references
- Define provenance expectations for derived-channel metadata
- Define unit propagation or fallback behavior baseline

## Out of Scope

- Full production implementation of all operations
- UI-specific editing behavior
- Complex symbolic expressions beyond first-step operations

## Implementation Notes

- Keep processing logic reusable across WPF and console execution paths
- Prefer immutable or clearly versioned transformation results over destructive mutation of source channels
- Make timestamp assumptions explicit for differentiation and integration
- Document error behavior for invalid operations, divide-by-zero scenarios, and unsupported unit combinations
- Model outputs as a collection at the execution-contract level so single-output and multi-output operations share the same execution framework

## Finalized First-Increment Decisions

- A processing pipeline executes **strictly in configured step order**
- Each step produces **one or more derived output channels** depending on the operation contract
- Raw source channels remain unchanged and available throughout execution
- Derived channels from earlier steps may be referenced by later steps
- A step failure is **fatal by default** for the pipeline execution result in the first increment
- Step execution should still report structured diagnostics that explain the failing step and reason
- Processing semantics operate on the canonical ordered record sequence and use each record timestamp as the default time reference
- Multi-output operations are supported explicitly in the first design baseline

## Processing-Step Contract Baseline

Each processing step should expose the following conceptual contract:

- `StepId`
  - Stable identifier within the pipeline
- `Operation`
  - Supported first-increment operation name
- `Inputs`
  - Ordered collection of source or derived channel references
- `Outputs`
  - Ordered collection of one or more derived channel definitions
- `Parameters`
  - Operation-specific settings
- `Enabled`
  - Whether the step participates in execution
- `Description`
  - Optional human-readable explanation

The executable processing service should interpret this contract independently of any UI-specific representation.

## Output Cardinality Baseline

The step contract should distinguish:

- **single-output operations**
  - `differentiate`
  - `integrate`
  - `add`
  - `subtract`
  - `multiply`
  - `divide`
  - `scale`
  - `offset`
- **multi-output operations**
  - `bitSplit`
  - `sinCos`

The operation contract should define:

- minimum output count
- maximum output count where applicable
- semantic output roles when relevant

## Execution Model Baseline

### Data flow

1. Load the canonical source dataset
2. Build an execution context containing:
   - source channels
   - already derived channels
   - ordered records
   - pipeline-level defaults
3. Execute each enabled step in order
4. Append every produced derived output channel to the execution context
5. Produce a processed dataset containing:
   - original channels
   - derived channels
   - propagated metadata
   - processing diagnostics when applicable

### Ordering and dependencies

- A step may reference:
  - a source channel
  - a derived channel from an earlier step
- A step must not reference:
  - a later step
  - itself
  - an undefined channel
- Dependency validation should happen before execution starts where possible

### Enabled and disabled steps

- Disabled steps are skipped
- Outputs from disabled steps are not available to later steps
- A later step referencing a disabled-step output should fail validation unless an alternate input is configured

## Record and Channel Semantics

The first increment should use a **hybrid execution model**:

- Channel-oriented semantics for defining inputs and outputs
- Record-sequence-oriented semantics for computing values across timestamps

This means:

- Arithmetic operations are evaluated record by record for aligned timestamps
- Differentiation and integration operate across adjacent records in timestamp order
- Bit-splitting and trigonometric decomposition operate record by record
- Missing values are handled according to explicit behavior settings or pipeline defaults

## Timestamp Baseline

- The canonical record order is authoritative for execution order
- Timestamps should normally be monotonic for calculus-style operations
- Differentiation and integration use the timestamp delta between consecutive records by default
- If timestamps are non-numeric or not suitable for delta computation, the step should fail unless a later approved fallback mode exists
- The first increment should not silently reorder records

## Decision Areas

- Whether steps operate record-wise, channel-wise, or hybrid depending on operation type
- How intermediate derived channels are named and referenced
- How multi-output operations express semantic output roles and validation rules
- Whether failed steps stop the pipeline or produce partial diagnostics
- How metadata and parse errors flow through processed datasets

## Test Strategy

- Define expected behavior examples for each first-increment operation
- Define edge cases for empty datasets, sparse values, and non-monotonic timestamps
- Define provenance and unit test cases for derived channels
- Define validation tests for step-order dependency failures and disabled-step references
- Define runtime-failure tests for divide-by-zero, invalid timestamps, and non-numeric values
- Define multi-output tests for `bitSplit` and `sinCos`, including role validation and exported channel naming

## Done Criteria

- Processing-step contract is defined
- Execution-order rules are defined
- First-operation semantics are documented
- Error and missing-value behavior is documented
- Provenance and unit baseline rules are documented
- Example execution cases are documented
- Multi-output operation semantics are documented
