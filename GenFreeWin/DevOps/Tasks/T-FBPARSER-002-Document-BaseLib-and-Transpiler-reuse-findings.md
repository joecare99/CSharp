# Task T-FBPARSER-002 Document BaseLib and Transpiler reuse findings

## Summary
Document reuse opportunities and conversion findings discovered during the first parser-port and test pass.

## Findings
### BaseLib reuse opportunities
- `BaseLib.Helper.StringUtils.Left` and `BaseLib.Helper.StringUtils.Right` already provide reusable substring helpers with richer negative-count behavior.
- `BaseLib.Helper.StringUtils.Quote` is conceptually related to `PascalCompat.QuotedString`, but the semantics are different:
  - `Quote` escapes control characters.
  - `QuotedString` wraps a value in single quotes to mimic Delphi/FPC diagnostics.
- `BaseLib` currently does not provide the exact Pascal-oriented one-based APIs needed for direct parser porting such as:
  - one-based `Copy`
  - one-based `Pos`
  - Pascal-like `IndexOfAny` for string arrays
  - parser-specific German charset sets including explicit umlaut membership
- Conclusion: `PascalCompat` remains justified for the port, but selected implementations can later be aligned with `BaseLib` naming and helper style.

### Transpiler project findings
- `TranspilerLib.Data.CharSets` already centralizes generic tokenizer character sets.
- The parser port revealed additional Pascal migration patterns worth encoding in the transpiler pipeline:
  - one-based string indexing and slicing helpers are required for faithful direct ports
  - Pascal `set of char` usage maps naturally to cached `HashSet<char>` or `ISet<char>` helpers in C#
  - parser ports often need explicit compatibility helpers before semantic refactoring
  - direct state-machine ports benefit from preserving original mode numbers and emitting detailed callback traces
- Recommendation: add a Pascal compatibility layer concept to the transpiler backlog rather than emitting raw ad-hoc string arithmetic in each converted file.

## Suggested backlog direction
- Add reusable transpiler output support for one-based substring helpers.
- Add explicit conversion templates for Pascal char-set constants.
- Add a migration note for event-based parser ports where delegates and callback signatures should be preserved first, refactored second.
