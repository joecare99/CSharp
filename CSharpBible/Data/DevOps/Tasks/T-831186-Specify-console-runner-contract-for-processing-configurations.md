# Task T-831186 - Specify console runner contract for processing configurations

## Status

Draft

## Parent

- Backlog Item `BI-830365` - `Create console runner for processing configurations`

## Goal

Define the command-line contract and execution behavior for running saved processing configurations against trace inputs without a graphical UI.

## Current Domain Context

- Processing configurations are planned as shared JSON artifacts that can be authored in WPF and executed unchanged in a console workflow
- The processing model supports both single-output and multi-output operations
- Input and output filters already exist for reusable import and export paths
- The console runner should be suitable for repeatable automation and later CI or batch execution
- The first increment should prefer deterministic behavior and explicit failure reporting over interactive convenience

## Scope

- Define the command-line arguments for configuration file, input source, and output target
- Define baseline behavior for one input file per invocation and document future batch-extension points
- Define how supported trace input filters are reused during execution
- Define how output format selection works for processed results
- Define how multi-output processing steps are reflected in exported processed datasets without special console-side handling
- Define diagnostics and exit-code behavior for invalid configurations, unsupported inputs, processing failures, and successful completion
- Define reporting expectations for unattended execution scenarios
- Define how configuration metadata is surfaced in logs or summaries when useful

## Out of Scope

- Full implementation of the console runner
- Directory-wide orchestration beyond documenting future extension points
- Advanced scripting DSLs or interactive shell behavior

## Implementation Notes

- Prefer deterministic command-line behavior with minimal implicit assumptions
- Reuse shared filter and processing services instead of console-specific transformation code
- Keep the contract compatible with automation, CI, and future batch use
- Make failure modes observable and machine-friendly
- Keep console behavior identical for single-output and multi-output processing; the runner should execute the pipeline, not reinterpret operation internals

## Finalized First-Increment Decisions

- Support **one input file per invocation** in the first increment
- Require an explicit processing configuration file path
- Require an explicit input file path
- Require an explicit output target path
- Determine output format by output-file extension through shared output-filter selection
- Fail by default when the output target already exists unless an explicit overwrite flag is later introduced
- Treat configuration validation failures and runtime processing failures as non-zero exit conditions
- Do not add special CLI flags for single-output versus multi-output operations; both use the same execution path

## Proposed Command-Line Baseline

Recommended baseline invocation shape:

`TraceProcessingRunner <configuration-file> <input-file> <output-file>`

Example:

`TraceProcessingRunner motion-config.json trace.csv processed.json`

This simple positional contract is suitable for first-increment automation. Optional named arguments can be considered later if the command grows.

## Proposed Argument Baseline

Required arguments:

1. `configuration-file`
   - Path to the saved processing configuration
2. `input-file`
   - Path to the source trace input
3. `output-file`
   - Path to the processed output target

Optional future arguments to document but not implement yet:

- overwrite switch
- verbosity level
- machine-readable diagnostics output path
- batch directory input

## Execution Flow Baseline

1. Read command-line arguments
2. Validate argument count and file existence expectations
3. Load and validate the processing configuration
4. Open the input trace through the shared filter-selection pipeline
5. Execute the configured processing pipeline
6. Export the processed dataset through the output filter selected by target extension
7. Emit deterministic summary diagnostics
8. Return an exit code

## Multi-Output Baseline

- Multi-output steps such as `bitSplit` or `sinCos` are executed entirely by the shared processing engine
- The console runner should not expose special flags for individual suboutputs in the first increment
- All derived outputs produced by the pipeline become part of the processed dataset and are exported through the chosen output filter
- Console summaries may mention the number of produced derived channels, but should not require special per-output command-line syntax

This keeps the CLI stable even when the processing model grows.

## Output Selection Baseline

- Output format should be selected by the output-file extension
- The first baseline should be compatible with shared output filters such as:
  - `.csv`
  - `.json`
  - `.xlsx` when available through the shared export layer
- Unsupported output extensions should fail before export begins

## Diagnostics Baseline

The console runner should emit short, automation-friendly diagnostics.

Recommended summary areas:

- configuration name or identifier when available
- input source path
- output target path
- selected input filter
- selected output filter
- step count executed
- derived channel count produced
- success or failure summary

For failures, diagnostics should include when possible:

- failure category
- failing step id
- operation name
- input or output context when known
- human-readable error message

## Exit-Code Baseline

Recommended first-increment exit codes:

- `0`
  - successful execution
- `1`
  - invalid command-line usage
- `2`
  - configuration load or validation failure
- `3`
  - input load or input-filter failure
- `4`
  - processing execution failure
- `5`
  - output export failure

This gives enough granularity for scripts without overcomplicating the first increment.

## Failure Baseline

### Usage failures

- wrong argument count
- missing required file path

### Configuration failures

- configuration file missing
- unsupported configuration format version
- invalid step references
- invalid output-role definitions for multi-output steps

### Input failures

- input file missing
- no input filter can handle the source
- source parse failure beyond recoverable limits

### Processing failures

- runtime failure in a configured step
- divide-by-zero under fatal behavior
- invalid timestamp semantics for calculus-style operations

### Output failures

- unsupported output extension
- output file already exists
- export-layer write failure

## Reporting for Unattended Execution

- Console output should remain concise by default
- Errors should go to standard error where practical
- Success summaries should be stable enough for log collection
- A later increment may add machine-readable diagnostics, but the first increment should already avoid ambiguous wording

## Example Successful Run

Input:

`TraceProcessingRunner angle-bits.json trace.csv processed.csv`

Expected behavior:

- configuration loads successfully
- input file is read through the shared filter pipeline
- pipeline executes, including any multi-output steps
- processed dataset is exported to CSV
- exit code `0`

## Example Failure Run

Input:

`TraceProcessingRunner angle-bits.json trace.csv processed.csv`

Failure case:

- pipeline contains a division step with a fatal divide-by-zero event
- console reports step id and operation context
- output file is not finalized as a successful result
- exit code `4`

## Decision Areas

- Required versus optional arguments
- Output overwrite behavior and safety defaults
- Single-file versus future multi-file execution model
- Whether later increments need selective output-channel export switches
- Summary verbosity and machine-readable diagnostics options

## Test Strategy

- Define successful run examples and failure examples
- Define exit-code expectations for each failure class
- Define compatibility checks with saved configurations authored by the WPF editor
- Define compatibility checks for configurations containing multi-output steps
- Define output-format tests across CSV, JSON, and future Excel export paths where applicable

## Done Criteria

- Command-line contract is defined
- Output-selection behavior is defined
- Error-reporting and exit-code behavior is defined
- Automation-oriented usage expectations are documented
- Compatibility with shared configuration artifacts is documented
- Multi-output processing compatibility is documented
