# T0019-B0008-CodeBlock-Semantic-Base

## Parent
- Backlog Item: B0008-Shared-Transpiler-Semantics

## Description
Refactor the first shared semantic node layer so typed semantic elements inherit from `TranspilerLib.Models.Scanner.CodeBlock` instead of a standalone `IecAstNode` shell.

## Goal
Adopt `CodeBlock` as the semantic base for the current typed IEC model so full-loop testability stays intact while semantic child types can encapsulate their own interpretation of `SubBlocks`.

## Scope
- Make the current semantic root reuse `CodeBlock` infrastructure
- Preserve existing IEC-prefixed semantic type names in this slice
- Keep mapper, binder, emitter, and interpreter contracts behaviorally stable
- Treat inherited `SubBlocks` as an internal black box for current child semantic types
- Add focused tests that guard the inherited block shell and source position behavior

## Out of Scope
- Broad renaming away from IEC-prefixed semantic type names
- Replacing the current mapper attachment contract in `IECCodeBlock.AstNode`
- Introducing new semantic child types such as generic branch or loop abstractions in this slice
- Reworking parser-owned `SubBlocks` interpretation rules across the whole frontend

## Assumptions
- `CodeBlock` already carries enough structural and positional semantics to serve as the shared semantic base
- The lowest-risk migration path is to move inheritance first and postpone broader semantic generalization
- Child semantic classes can inherit `SubBlocks` safely as long as they do not expose parser-shape coupling as public contract

## Open Questions
- Which semantic child types should be the first to expose typed accessors backed by inherited `SubBlocks`
- Whether `IECCodeBlock.AstNode` should later narrow from a side-car attachment to a stronger semantic bridge
- When the IEC-prefixed semantic type names should be generalized after the inheritance transition stabilizes

## Implementation Notes
1. Change only the semantic base inheritance in the first step
2. Preserve constructor call sites wherever possible to keep test churn low
3. Validate focused semantics and IEC frontend tests after the refactor

## Acceptance Criteria
- `IecAstNode` inherits from `CodeBlock`
- Existing semantic node constructors still preserve source position behavior
- Focused semantic and IEC mapper/interpreter tests still pass
- The first refactor slice does not widen public `SubBlocks` assumptions in semantic consumers
