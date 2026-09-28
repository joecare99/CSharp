# T0028-B0009 - Token-based VB legacy replacements

## Status

Implemented; built-in mappings extended, focused tests passed, and the FamAlist candidate compiled.

## Objective

Create a reusable `TranspilerLib.CSharp.VBLegacyReplace` library that rewrites explicitly defined Visual Basic runtime idioms into modern C# without regex matching.

## Agreed rule model

- Rules are stored as versioned JSON and may be loaded from a caller-supplied file or conservative built-in defaults.
- Each rule records its identifier, source template, replacement template, optional placeholder type constraints, required `using` directives, deterministic priority/enabled state, and human-readable documentation.
- Templates are token-aware. A placeholder such as `<Param1>` captures a complete expression, balancing nested parentheses, calls, indexers, generic/argument lists, strings, and character literals.
- Replacements reuse the exact captured expression; regex-based matching/replacement is explicitly prohibited.
- Typed placeholders such as `<Param1:bool>` are applied only when bool-ness is established from supported syntax or reliable type information. Otherwise the rule is skipped and a diagnostic is emitted.
- Rule application is deterministic, non-overlapping, and reports applied/skipped/malformed rule findings with source spans.

## Example

`Strings.Trim(<Param1>)` → `((<Param1>) ?? string.Empty).Trim(' ')`

The defaults also cover `Strings.LTrim`/`RTrim`, `Information.IsDBNull`, and the direct `DateAndTime.Now`/`Today` properties. Trimming specifies U+0020 explicitly because the VB helpers do not trim all Unicode whitespace, and coalesces null to an empty string because that is the VB helper behavior.

For example, the placeholder must be able to capture the entire expression in:

`Strings.Trim(COND.QuTable.Fields["11"].Value.AsString())`

## Semantic safeguards

- Initial rules focus on well-defined string/date helpers. Culture-sensitive comparisons/formatting, null behavior, conversion overflow, and Visual Basic late-binding semantics require explicit rule-specific contracts and tests.
- The default set deliberately leaves `Conversion.Val`/`Str`, `Strings.Mid`/`Left`/`Right`/`InStr`/`Replace`/`Len`, `Operators` comparisons, and `ProjectData` error handling unchanged. FamAlist uses these heavily, but straightforward C# lookalikes can alter parsing, bounds, comparison, evaluation order, or VB error-state behavior.
- A rule is not considered safe merely because the destination method name looks similar.
- When replacements are enabled together with equivalence checking, the comparison is performed before replacements because those source-form transformations may be outside the initial checker proof model.

## Frontend behavior

- Both `VBUnObfusicator.Cli` and the WPF application expose independent, default-off replacement options.
- The CLI accepts an optional external JSON rule file.
- The WPF findings/log view reports each replacement and supports navigation to the affected source.

## Validation

- Tests cover the documented nested-call/indexer example, literals, balanced delimiters, overlapping rules, malformed templates, deterministic ordering, required usings, and bool type constraints.
- Nested helper calls are rewritten inside-out, with diagnostics retaining offsets into the original containing source.
- Unknown/unproven types are skipped with findings, never guessed.
- Default behavior is unchanged when the replacement option is disabled.
- Prior validation: the library test suite passed **14/14** on net8.0; the CLI and WPF Release builds passed. Frontend tests covering optional replacement behavior passed as part of the **26/26** focused net8.0-windows run. The broader UnObfusculator suite has unrelated parser/block-test failures documented in T0027-B0009.
- Current validation: extended library tests passed **18/18** on net8.0, net9.0, and net10.0. Applying the default rules to `FamAlist.cs` rewrote all **1,060** occurrences of `Strings.Trim/LTrim/RTrim` and `Information.IsDBNull`, including nested trim calls. The remaining **2,108** VB runtime-helper references are deliberately out of the default rule set. The generated candidate compiled in the isolated OSB project with **0 errors** and **1,387 warnings**; no OSB source file was replaced. The generated C# grows by about 19 KB due to explicit null/space semantics and CLI formatting.
- Nested rewriting is capped at 128 levels; deeper inputs remain partially unchanged and produce a warning rather than risking unbounded recursion.
- Running the library test project across every declared target also surfaces existing net481 project issues (`System.Text.Json` and `IsExternalInit` references); the supported modern targets above pass. No net481 support changes were included in this rule-only task.
