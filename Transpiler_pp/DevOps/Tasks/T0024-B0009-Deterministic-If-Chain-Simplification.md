# T0024-B0009 - Deterministic if-chain simplification

## Status

Implemented and validated on `net8.0`, `net9.0`, and `net10.0`.

## Requirement

Simplify only proven-equivalent two-condition chains while retaining C# short-circuit evaluation order:

- `if (b1) { if (b2) { Body; } }` becomes `if ((b1) && (b2)) { Body; }`.
- `if (b1) { Body; } else if (b2) { Body; }` becomes `if ((b1) || (b2)) { Body; }` when both bodies are structurally identical.

Each condition is evaluated at most once, and the second condition remains conditional on the first condition's result.

## Safety boundaries

- Only structurally adjacent conditions are merged per rewrite; longer chains are folded pairwise while retaining the original short-circuit grouping with parentheses.
- AND candidates with an outer `else`, extra executable statements, or unsafe control flow are retained; comments are kept in the resulting branch.
- OR candidates require a comment-only or empty `else` separator and equivalent, safe straight-line operation statements in both bodies. Identical assignment statements are eligible and remain only once in the merged body.
- Conditions must be recognizable boolean parameters/literals or contain a top-level `==`/`!=`; compound `&&`/`||` conditions are validated recursively and retained with explicit parentheses when merged.
- Body comparison ignores comment nodes, while the merge keeps comments from both alternatives in their corresponding block positions.
- Different executable bodies, labels, gotos, and nested control-flow statements prevent a merge.
- The first pair of a longer `else if` chain may be merged; subsequent branches remain attached as the original fallback chain.
- Block delimiters and the body statement order are retained.

## Implementation and tests

- Added `ICodeOptimizer.OptimizeIfChains` and invoked it before label reduction in `CSCode`.
- Added `TestIfElseOptimize(Code, Contains, NotContains, ContainsOnce)`, a shared resource-driven output test. `Code` is a `Resources.resx` key; the helper checks required/forbidden fragments and exact-once fragments. It writes the generated source to `Debug` output and includes it in assertion failures.
- Added numbered input resources under `TranspilerLibTests\Resources\Test24` through `Test27`, with `a`/`b`/`c` variants registered in `Resources.resx`:
  - **Test24:** nested `if` to `&&`, preserving a comment. **Test24a-b:** outer `else` and extra outer statement are merge barriers. **Test24c:** the conditions merge to `&&`, while the original `goto`, destination label, and source-link count remain unchanged.
  - **Test25:** equivalent `else if` bodies to `||`, retaining both comments. **Test25a:** different bodies and **Test25c:** an executable `else` block remain rejection cases. **Test25b:** identical assignments merge and execute once.
  - **Test26:** first equivalent pair to `||`, later distinct fallback retained. **Test26a:** differing first pair remains rejected. **Test26b:** an already-compound OR condition merges with explicit grouping. **Test26c:** three equivalent alternatives merge pairwise without changing short-circuit order.
  - **Test27:** different bodies remain separate. **Test27a-b:** different statement counts and nested control flow. **Test27c:** comment-only differences merge and every comment remains exactly once.
- The tests resolve resource strings dynamically by resource key; no generated `Resources.Designer.cs` accessors are needed.
- Retained direct `CodeBlock` tests where they verify parent links, synthetic malformed trees, recursive shape handling, and idempotency independently of source parsing.
- Added a direct regression proving an inner label remains a barrier to nested-AND merging, while Test24c confirms a body goto is preserved through a safe merge.
- Compound boolean operands are recursively validated and grouped as written during AND/OR merges; exact-equivalent straight-line assignments are eligible OR bodies alongside existing invocation bodies.
- Updated Test09's expected parse tree for the deterministic nested-AND merge of Kont[11] through Kont[14]; the existing `goto` and continuation remain unchanged.
- `MergeEquivalentBlocks` transfers comments through the parent-aware list without adding a duplicate node. Comment matching covers `Comment`, `LComment`, and `FLComment`.
- Grouped supported-condition variants and unproven/malformed-condition variants into MSTest `DataRow` cases with descriptive `DisplayName` values; added display names to the existing malformed-input and control-flow rows.
- Coverage for all newly introduced if-chain pass/helper methods is 100% line and branch coverage, including `TryMergeNestedIfWithAnd`, `ContainsUnsafeControlFlow`, `ContainsTopLevelEquality`, and comment merging.
- Focused `TestIfElseOptimize`: 16/16 numbered cases passed on `net8.0`.
- Focused `CodeOptimizerTests`: 92/92 passed on `net8.0`; Coverlet confirmed 100% line and branch coverage for the AND/OR merge paths, body equivalence, and compound-condition validation.
- Full `TranspilerLib.CSharp.Tests`: 325/325 passed on each of `net8.0`, `net9.0`, and `net10.0`.

## Completion

The pass is conservative and deterministic. No general condition reordering, De Morgan rewrite, or arbitrary body-equivalence inference is performed.

## Follow-up: left-associative nested conjunctions

- Three or more nested `if` statements were merged from the inside out, producing a right-associated condition such as `if ((b1) && ((b2) && (b3)))`.
- When the inner condition is a generated chain of parenthesized `&&` operands, the merge now folds those operands from left to right and emits `if (((b1) && (b2)) && (b3))`.
- Original compound conditions without the optimizer's generated operand grouping retain their existing grouping.
- Added a parser-based regression for three nested, braced conditions, including assertions that each condition and the body occur once. Updated the direct-tree regression to assert left association.
- Updated Test09's expected parse structure to the verified left-associated four-condition chain.
- Focused `CodeOptimizerTests`: 101/101 passed on `net8.0`. Coverlet reports 100% line and branch coverage for all three new condition-combination helpers.
- Full `TranspilerLib.CSharp.Tests`: 334/334 passed on each of `net8.0`, `net9.0`, and `net10.0`.
