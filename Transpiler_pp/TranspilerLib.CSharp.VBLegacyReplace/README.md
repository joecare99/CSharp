# TranspilerLib.CSharp.VBLegacyReplace

This library applies externally authored JSON replacement rules to C# source without regular expressions or a compiler dependency. Rules use schema version 1, documented in `Rules/replacement-rules.schema.json`. The embedded defaults cover `Strings.Trim`, `LTrim`, and `RTrim` (trimming only U+0020 spaces and preserving VB's null-to-empty behavior), `Information.IsDBNull`, and `DateAndTime.Now`/`Today`.

```csharp
var engine = LegacyReplacementEngine.LoadFile("legacy-rules.json");
ReplacementResult result = engine.Apply(sourceText);
string convertedSource = result.Source;
IReadOnlyList<string> additionalUsings = result.RequiredUsings;
IReadOnlyList<RuleDiagnostic> diagnostics = result.Diagnostics;
```

Templates are compared as C# tokens, so source whitespace and comments do not affect matching. A placeholder has the form `<Param1>` or `<Param1:bool>`; expression captures preserve the original source text and balance parentheses, brackets, braces, and quoted literals. Nested helper calls inside a captured expression are replaced recursively before the containing call, up to a depth limit of 128; deeper nesting is left unchanged with a warning. A typed boolean capture is accepted only for boolean literals, unary `!`, comparison/pattern-test operators, and logical operators. Unknown expression types skip the candidate with a warning. Rules are considered by descending priority, then greater fixed-token specificity, then ordinal rule ID; matches are applied left-to-right without overlapping sibling spans. Required usings are returned as an ordinally sorted distinct collection for rules actually applied.

Malformed rule definitions are diagnosed and excluded while valid rules remain usable. Disabled and unmatched rules receive skipped diagnostics. The token matcher deliberately does not perform type binding or claim semantic equivalence beyond the rule's declared template.

The defaults intentionally do not rewrite `Conversion.Val`/`Str`, `Strings.Mid`/`Left`/`Right`/`InStr`/`Replace`/`Len`, or `Operators` helpers. Their VB conversion, bounds, comparison, null, late-binding, and evaluation semantics are not interchangeable with similarly named C# operations without stronger type and range analysis.
