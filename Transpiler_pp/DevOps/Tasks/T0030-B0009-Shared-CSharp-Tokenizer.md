# T0030-B0009 - Shared fine-grained C# tokenizer

## Status

Implemented and validated. Tokenizer, parser, CodeBuilder, optimizer, and VB legacy replacement tests pass on all declared target frameworks.

## Objective

Use `TranspilerLib.CSharp`'s tokenizer as the shared lexical foundation for parsing and VB legacy replacement. Emit fine-grained tokens with source spans while retaining existing `ICodeBlock` structures and generated code behavior.

## Scope delivered

- Added `CSharpLexer` tokenization for identifiers, numeric and quoted literals, operators, punctuation, comments, and atomic interpolated strings.
- Added lexical kind, token length, leading trivia, and exclusive end position to `TokenData`.
- Routed `CSCode.Tokenize()` through the shared lexer for `CSTokenHandler`, retaining the existing handler path when the lexer reports malformed input or another handler is supplied.
- Updated `CSCodeBuilder` to group fine-grained tokens into instructions, labels, gotos, comments, and nested blocks. It retains the legacy equality/negative-literal spacing and the Test22 output golden now reflects the source-faithful space before the comparison call.
- Kept catch-filter clauses together when supplied as separate legacy operation tokens and made directly adjacent unbraced `if` chains render on one line without changing their block tree.
- Replaced VBLegacyReplace's private tokenizer with `CSharpLexer` and shared `TokenData`; comments are excluded from rule matching without being removed from captured source.
- Restored the default Trim/LTrim/RTrim replacements to preserve VB semantics: null becomes empty, and only U+0020 is trimmed.
- Expanded tokenizer, parser, and legacy rule tests to cover source spans, multi-character operators, strings, nested expressions, comments, and replacements.

## Constraints and assumptions

- Interpolated strings are represented as one atomic token; expressions inside them are not separately matched by legacy rules.
- Malformed lexical input is reported by `CSharpLexer`; `CSCode` continues to use its legacy handler fallback so existing parser error behavior is retained.

## Validation

- Focused `Tokenize`/`Parse` selection: **109/109 passed** on each of net8.0, net9.0, and net10.0.
- Full `TranspilerLib.CSharp.Tests`: **377/377 passed** on each of net8.0, net9.0, and net10.0.
- StatEqualCheck tests: **27/27 passed** on each of net8.0, net9.0, and net10.0.
- VB legacy replacement tests: **18/18 passed** on net8.0, net9.0, net10.0, and net481.
