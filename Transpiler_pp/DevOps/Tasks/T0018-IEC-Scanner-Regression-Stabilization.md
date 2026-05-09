# T0018 IEC Scanner Regression Stabilization

## Parent
- Backlog Item: Unknown yet

## Scope
- Investigate remaining failing scanner-oriented tests in `TranspilerLib.IEC.Tests`
- Compare current tokenizer, parser, and code-builder output with legacy expectations
- Restore legacy-compatible scanner and formatting behavior with minimal production changes
- Keep the already fixed interpreter, AST, and ExtOutput regressions stable

## Assumptions
- The remaining failures are concentrated in scanner/tokenization, parse rendering, and code-block formatting paths
- Most failures are shared across `net8.0`, `net9.0`, and `net10.0`, indicating logic regressions rather than framework-specific behavior

## Open Questions
- Whether token classification drift begins in `IECTokenHandler`, `IECCodeBuilder`, `IECCodeBlock.ToCode`, or a combination of them
- Whether some failures are caused by changed formatting only, while underlying token trees remain structurally correct
- Whether test resource baselines are still authoritative for the current parser model

## Validation
- Run focused failing scanner tests first
- Run the full `TranspilerLib.IEC.Tests` project after fixes
- Run a workspace build to ensure no compilation regressions

## Outcome
- Restored focused scanner test stability by reconciling the remaining parser and builder expectations with the current IEC scanner behavior.
- Kept the token list regression fix in place through legacy `CodeBlockType` remapping for persisted JSON baselines.
- Corrected the `IECCodeBuilderTests` inline-expression data rows so builder-only cases exercise the intended token sequences, including empty-root cases.
- Updated embedded and file-based parse/code baselines for the remaining IEC samples to reflect the current serialized block trees and reconstructed code output.
