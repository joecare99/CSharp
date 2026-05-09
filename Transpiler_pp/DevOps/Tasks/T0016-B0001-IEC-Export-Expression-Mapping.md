# T0016-B0001-IEC-Export-Expression-Mapping

## Parent
- Backlog Item: B0001-Typed-IEC-AST

## Description
Improve expression mapping for realistic exported IEC source text so generated C# output retains more of the current supported arithmetic and function-call intent from the export fixture.

## Goal
Reduce the current expression-loss gaps in export-driven typed compilation units and strengthen the end-to-end IEC-to-CSharp regression path.

## Scope
- Inspect the remaining lossy expressions in the export fixture path
- Improve typed expression mapping in `IecAstMapper`
- Add focused export-based regression coverage
- Tighten end-to-end backend assertions where the improved mapping becomes visible

## Out of Scope
- Full IEC expression coverage
- Broad parser redesign
- New semantic node categories beyond the current AST subset

## Notes
- Keep the increment minimal and fixture-driven
- Prefer mapper improvements over broad parser normalization changes
