# T0020 - CSharp Else-if Regression Coverage

## Parent
- Feature: B0006 - Testable CSharp Generation

## Description
Add explicit regression coverage for the C# parser and optimizer around `else if` and chained conditional goto patterns that previously produced malformed blocks or missed structural simplifications.

## Scope
- Reproduce the `else` + nested `if` parser regression in `Test20Dat.cs`
- Reconstruct the `Test21Dat.cs` chained goto fixture as nested conditional branches while retaining the final common `goto End;`
- Ensure the C# parser keeps `else` and the following `if` as separate structural blocks
- Render adjacent structural `else` and nested `if` blocks as `else if`
- Add focused tests that guard parser output, optimizer reconstruction, and generated code
- Validate only the targeted C# regression slice instead of broad project-wide test churn

## Acceptance Criteria
- `else if` is parsed as two distinct block-level operations instead of a single malformed `elseif` node
- Test21's branch-local gotos are converted into an `if` / `else if` chain; its final common `goto End;` remains
- `ToCode` emits adjacent `else` and nested `if` as `else if` on one line
- The relevant C# scanner/parser regression tests pass for all supported target frameworks

## Validation
- `dotnet test TranspilerLib.CSharp.Tests\TranspilerLib.CSharp.Tests.csproj --filter "Parse_SeparatesElseAndFollowingIfBranches|RemoveSingleSourceLabels1_PreservesChainedConditionalGotoStructure"`
- CLI validation with `VBUnObfusicator.Cli` and the unchanged `Test21Dat.cs` fixture using `--remove-single-source-labels`; output contains `else if (b2)`, `else if (b3)`, and the retained final `goto End;`
