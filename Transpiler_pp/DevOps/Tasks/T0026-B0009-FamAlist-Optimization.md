# T0026-B0009 - FamAlist optimizer application and audit

## Status

Completed and validated.

## Objective

Apply the deterministic conditional and label optimizations to the real `Osb\Osb\FamAlist.cs`, audit every direct nested-if candidate, and replace the source only after validating the generated result.

## Applied changes

- Extended the boolean-condition whitelist with comparison, member/indexer, and explicit Visual Basic boolean-helper forms. Unknown method calls and malformed expressions remain rejected.
- Added malformed quoted-literal validation and positive/negative `DataRow` regressions for the accepted condition syntax.
- Fixed C# token spacing before underscore-prefixed identifiers; added a parser regression for `return _field;`.
- Updated the Test09 and Test22 expected parse trees to reflect verified, order-preserving nested-if merges.
- Replaced `Gen_FreeWin\Osb\Osb\FamAlist.cs` with the output from `VBUnObfusicator.Cli --remove-single-source-labels --reorder-labels`.

## Candidate audit

The parser-based audit found 64 direct nested-if candidates:

| Classification | Count |
|---|---:|
| Merged | 31 |
| Remaining because of inner control-flow/body barriers | 32 |
| Remaining because the outer if has an else | 1 |
| Rejected by the approved condition whitelist | 0 |
| Unexplained remaining candidates | 0 |

The CLI output and the independent AST-audit output matched exactly. The audit also confirms that all 33 remaining candidates have an explicit safety reason.

## Before and after

| Metric | Original | Optimized |
|---|---:|---:|
| File size | 2,163,828 bytes | 1,813,375 bytes |
| Lines | 45,222 | 32,834 |
| `goto` statements | 10,017 | 5,020 |
| `if` statements | 2,491 | 2,428 |
| `else if` chains | 2 | 35 |
| `&&` operators | 106 | 157 |

The file is 350,453 bytes (16.2%) and 12,388 lines (27.4%) smaller; the number of `goto` statements is reduced by 4,997 (49.9%).

## Equivalence and validation

- Instrumented predicate/action tests compare the original nested form with the merged form for all four boolean result combinations, including evaluation order, call counts, and short-circuit behavior.
- The optimizer tests cover the supported expression categories and their rejection boundaries. The conditional optimization and condition-scanner helpers have 100% line and branch coverage in the focused Coverlet run.
- `TranspilerLib.CSharp.Tests`: 374/374 passed on each of `net8.0`, `net9.0`, and `net10.0`.
- The isolated OSB candidate build and the actual `osb.csproj` build both succeeded with 0 compiler errors and 1,362 warnings.
- No OSB business-data fixtures are available. The static audit and synthetic tests support the rewrite rules, but no business-level runtime equivalence claim is made.

## Completion

The audited CLI output now replaces the FamAlist source. All measured candidate categories are accounted for, the replacement compiles in the OSB project, and the optimizer suite passes across all supported target frameworks.
