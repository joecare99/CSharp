# T0027-B0009 - Static C# execution-equivalence checking

## Status

Implemented; targeted validation passed. The broader UnObfusicator suite still has unrelated parser/block-test failures (see validation notes).

## Objective

Provide a standalone checker for two C# source inputs or two parsed `ICodeBlock` trees. The checker builds control-flow graphs and compares ordered decisions and actions, independently of the optimizer pass that produced either input.

## Agreed comparison contract

- Compare sources or already parsed code-block trees through one public library API.
- Use a conservative weak-bisimulation comparison over labeled control-flow graphs.
- Treat labels, structural braces, and direct goto routing as structural epsilon steps. Observable action order, condition evaluation order, branches, and exits remain significant.
- Permit different `if`/`while`/`switch`/`goto` shapes only when their observable decision/action paths match.
- Parse boolean expressions into short-circuit decision graphs for `!`, `&&`, `||`, `&`, `|`, comparisons, grouping, and supported atoms.
- Preserve unsupported expressions as ordered opaque atoms and produce findings when proof is incomplete. Never claim general program equivalence.
- Findings include severity/status and source ranges for navigation in the UI and structured JSON output in the CLI.

## Explicit terminal-block handling

The source API is `StatEqualCheck.Compare(string, string)`. The tree API accepts `originalTerminalBlocks` and `candidateTerminalBlocks` independently. Each listed block is treated as a synthetic `return` terminal: control flow stops at that block, and its descendants are not traversed. This lets callers exclude known generated dispatcher tails without heuristic pattern detection. The comparison library does not recognize Resume/Resume Next blocks automatically, and unlisted `try`, `catch`, `switch`, or user-authored code remains observable.

## Frontend behavior

- CLI provides a standalone two-file compare mode (`--compare-against`) as well as optional comparison of original input against this run's optimized intermediate output.
- The WPF UnObfusculator can compare original input with the generated intermediate output and presents a navigable findings list. It does not expose a heuristic Resume-block toggle; callers that have exact parsed block identities may pass them to the comparer API.
- CLI and WPF pass no terminal blocks because they cannot reliably map source text to exact `ICodeBlock` identities. Callers with exact parsed block identities can use the library API directly.
- If VB legacy replacements are also enabled, equivalence checking happens before replacements are applied.

## Validation

- Positive tests cover differently structured but equivalent short-circuit conditions, goto/label normalization, loops and switches, and statement ordering.
- Negative tests cover missing/extra actions, changed condition order, branch mismatches, and unmatched exits.
- Explicit terminal-block tests prove that only caller-listed blocks terminate traversal and that unlisted switch/try/catch bodies remain observable.
- Unsupported syntax and graph limits result in explicit inconclusive findings rather than success.
- Validation: StatEqualCheck tests passed **27/27** on net8.0, net9.0, and net10.0. Focused CLI/WPF frontend tests passed **26/26** on net8.0-windows; CLI option tests passed **5/5**. CLI and WPF Release builds passed, including WPF net8.0-windows and net481 targets.
- The full UnObfusicator net8.0-windows suite is not green: **140/201 passed, 61 failed** in existing parser/block scenarios (`MoveBlocksTest`, `MoveBlocksTest2`, `DeleteBlocksTest`, `ParseEnum*`, `RemoveLabelsTest`, `ReorderLabelsTest`, and `Tokenize2Test`). `MoveBlocksTest2` also fails when run alone with an out-of-range block index; these failures are outside the new comparer/frontend-focused tests.
