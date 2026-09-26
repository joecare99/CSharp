# T0025-B0009 - Unnecessary conditional block removal

## Status

Implemented and validated on `net8.0`, `net9.0`, and `net10.0`.

## Requirement

Remove redundant braces around a conditional branch when the branch contains only one nested `if` statement. Its associated `else` remains part of that statement, allowing the output renderer to produce a conventional `else if` chain.

The rewrite also exposes nested conditions and singleton operation bodies to the existing deterministic AND-chain optimization. For example, `if (b1) {{ if (b2) { E(); } }}` can become `if ((b1) && (b2)) E();`.

## Safety contract

- Only well-formed conditional branch wrappers with a single nested `if` (and its associated `else`, when present) are flattened.
- Repeated singleton wrappers are handled until no further safe flattening applies.
- A leading line comment may move before the enclosing conditional node; its comment type and relative order are preserved.
- Block comments and other comments that cannot move without changing their ordering prevent flattening.
- Additional executable statements, labels/gotos at the branch boundary, loops/switches, malformed delimiters, and other non-matching shapes remain unchanged.
- For a grouped multi-statement body, only a redundant outer wrapper is removed; the inner block that groups those statements remains.
- After an AND merge, a block around a single safe operation may be removed. Multi-statement bodies retain their grouping.

## Implementation and regressions

- Added a pre-order retrying flatten step to `CodeOptimizer.OptimizeIfChains`, before the existing AND/OR merges.
- Added block-shape helpers that support both parser representations of brace delimiters and verify matching open/close boundaries before mutation.
- Added Test28 resource cases and data-driven assertions:
  - **Test28:** a leading `//` comment stays in order while an `else` wrapper becomes an `else if`.
  - **Test28a:** nested wrappers flatten, conditions merge, and the singleton operation's braces are removed.
  - **Test28b:** an additional executable statement prevents flattening.
  - **Test28c:** a leading block comment prevents flattening.
  - **Test28d:** an outer wrapper is removed while the inner multi-statement grouping remains.
- Direct tree tests verify line-comment parent/order/type and reject malformed block boundaries.
- Updated Test09, Test15, and Test22 expected parse trees only where the verified optimizer output changed. Test15 records the line-comment index after the inner conditional is flattened.
- New flattening helpers have 100% line and branch coverage. Focused new tests passed 6/6; `CodeOptimizerTests` passed 100/100 on `net8.0`.
- Full `TranspilerLib.CSharp.Tests`: 333/333 passed on each of `net8.0`, `net9.0`, and `net10.0`.

## Completion

The optimization preserves comment types, statement order, and required multi-statement grouping. It performs only shape-proven rewrites and leaves unsafe or malformed branches intact.
