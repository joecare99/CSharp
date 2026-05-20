# T0105A - Launcher Flow Baseline

## Parent Backlog Item

- B0102 - Describe Project Entry Points and Learning Paths

## Summary

Document the current execution behavior of `CallAllExamples` so the repository has a baseline description of how aggregated example execution works today.

## Current Launcher Behavior

### Main Execution Pattern

The current `CallAllExamples` launcher uses reflection to inspect the `TestStatements` assembly and enumerate public static methods that return `void`.

It then invokes matching methods under multiple argument shapes:

- no parameters
- `args` with an empty string array
- `args` with one string value
- `args` with two string values
- `args` with numeric string values such as `"20"` and `"10"`

### Invocation Style

Before each invocation, the launcher prints a visible title block that identifies the target type, method, and argument pattern.

This creates a broad smoke-style execution flow rather than a carefully curated learning sequence.

### Input Handling

For invoked methods that rely on console input, the launcher temporarily replaces `Console.In` with a `StringReader` that supplies predefined test input.

Current predefined input:

- `one`
- `two`
- empty line

This allows examples with interactive reads to execute in an automated aggregate run.

### Error Handling

Invocation exceptions are caught and printed as exception messages in the launcher output instead of stopping the entire run immediately.

This behavior supports broad execution coverage even when single examples fail.

## Current Learning and Maintenance Value

- Fast way to execute many examples without manually selecting them one by one
- Useful as a smoke-style check after changes
- Helpful for discovering which public static sample methods are actually runnable in an aggregate scenario
- Good baseline for future grouped execution profiles

## Current Limitations

- The execution order is reflection-driven rather than topic-driven
- The launcher does not currently group examples by learning theme
- The launcher may invoke methods that are technically runnable but not equally useful as learning entry points
- Output can become noisy because invocation breadth is prioritized over curated flow
- Exception output is visible, but failure categories are not yet structured

## Grouped Execution Intent for Future Work

The current launcher behavior suggests a later evolution toward explicit run groups such as:

- language basics
- data types and formatting
- collections and LINQ
- diagnostics and reflection
- runtime and dynamic behavior
- async and tasks

A future grouped launcher should preserve the broad smoke-style value while allowing targeted execution by theme.

## Output Documentation Relevance

`CallAllExamples` is itself output-sensitive because it produces:

- invocation title blocks
- sample output from many executed examples
- exception summaries for failed invocations
- a final interactive exit prompt

Future documentation should therefore distinguish:

- launcher framing output
- output from invoked examples
- exception and failure output

## Notes

- This baseline is derived from the current `CallAllExamples/Program.cs` implementation and the related project README.
- The description is intentionally neutral and implementation-oriented so it can support later refactoring into grouped execution profiles.

## Done Criteria

- The current invocation model is documented.
- The launcher value as an aggregate runner is clear.
- Future grouped execution intent is recorded.
