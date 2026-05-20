# T0102A - Output-Sensitive Example Baseline

## Parent Backlog Item

- B0101 - Document Expected Output for Learning-Critical Examples

## Summary

Record the current example groups where output, sequencing, or runtime-observable behavior is essential for understanding the example.

## Output-Sensitive Areas

### High Priority for Expected Output Notes

- `TestStatements.Threading.Tasks`
  - Reason: async and task examples depend on observed execution order, sequencing, and completion behavior
  - Notes: parts of the observed order may be illustrative rather than strictly deterministic
- `TestStatements.Diagnostics`
  - Reason: debug and stopwatch examples are meaningful through timing or diagnostic output
  - Notes: timing values are non-deterministic and should be documented as patterns, not exact values
- `TestStatements.Reflection`
  - Reason: the value of the examples is in the visible metadata and reflection results
  - Notes: exact ordering can vary and should be treated carefully when documented
- `TestStatements.Runtime.Loader`
  - Reason: loader behavior is only understandable when runtime side effects and generated results are visible
- `TestStatements.Runtime.Dynamic`
  - Reason: dynamic assembly behavior is primarily demonstrated through generated runtime results or artifacts
- `TestStatements.SystemNS.Text.Json`
  - Reason: serialization examples are meaningful through concrete output structure, file content, or async write behavior
  - Notes: file paths and formatting may need framework-aware wording

### Medium Priority for Expected Output Notes

- `TestStatements.DataTypes`
  - Reason: formatting and string examples teach through visible text transformation
- `TestStatements.Collection.Generic`
  - Reason: collection examples often demonstrate ordering, lookup results, and membership behavior through printed output
- `TestStatements.Linq`
  - Reason: lookup and enumeration examples are easier to understand when expected results are visible
- `TestStatements.Anweisungen`
  - Reason: several statement examples are instructional through console-visible branching or flow output, though some are more concept-driven than output-driven

### Lower Priority or More Concept-Driven Areas

- `TestStatements.CS_Concepts`
- `TestStatements.ClassesAndObjects`
- `TestStatements.Constants`
- `TestStatements.Helper`

These areas still benefit from explanation, but they are less dependent on precise output transcripts than runtime, async, reflection, serialization, and diagnostics examples.

## Cross-Project Notes

- `CallAllExamples` is output-sensitive because it aggregates multiple example groups into one execution flow.
- Existing test projects already validate parts of reflection, collections, threading, async, formatting, and dynamic behavior, which can support later output documentation.

## Documentation Guidance

- Prefer describing patterns and observations instead of exact timing values.
- Mark non-deterministic behavior explicitly.
- Note cross-framework differences only where they affect learning value or expected behavior.

## Done Criteria

- Output-sensitive groups are listed.
- Reasons are recorded.
- Deterministic and illustrative behavior are distinguished.
