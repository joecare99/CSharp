# T0013-B0001-IEC-Source-Text-Control-Flow-Mapping

## Parent
- Backlog Item: B0001-Typed-IEC-AST

## Description
Extend the IEC frontend source-text bridge so statement separators and control-flow markers preserve a stable statement sequence for typed `RETURN` and `IF` extraction.

## Goal
Allow source-text parsing to produce typed control-flow statements from the existing scanner and mapper pipeline without moving parser concerns into shared semantics.

## Scope
- Adjust IEC frontend parsing so statement separators do not hide subsequent statements under separator wrapper blocks
- Preserve ordered source-text statement extraction for `RETURN` and `IF`
- Add focused MSTest coverage for minimal source-text control-flow cases
- Extend export-fixture validation when the improved extraction becomes available

## Out of Scope
- Full parser redesign
- New semantic node types
- Generalized support for all IEC control-flow constructs

## Assumptions
- The current typed `IecAstMapper` already supports `RETURN` and `IF` when block ordering is stable
- A minimal parser-side change is preferable to a broad normalization layer for this increment

## Implementation Notes
1. Keep the change inside `TranspilerLib.IEC`
2. Reuse the existing source-text factory and mapper tests
3. Validate both minimal source-text cases and realistic fixture behavior

## Acceptance Criteria
- Minimal source-text parsing yields a typed `RETURN` after a preceding statement separator
- Minimal source-text parsing yields a typed `IF` statement with typed branch assignments
- Focused tests pass across supported modern targets
- Existing build remains successful
