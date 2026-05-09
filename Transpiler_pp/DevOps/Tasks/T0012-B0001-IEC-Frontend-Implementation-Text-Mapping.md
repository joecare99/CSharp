# T0012-B0001-IEC-Frontend-Implementation-Text-Mapping

## Parent
- Backlog Item: B0001-Typed-IEC-AST

## Description
Close the current IEC frontend gap by mapping implementation text into typed IEC statements when a compilation unit is created from declaration and implementation source text.

## Goal
Provide a frontend-owned bridge from exported IEC text to a non-empty typed compilation unit so interpreter and C# backend flows can consume declaration and statement semantics together.

## Scope
- Add a frontend-owned compilation-unit factory in `TranspilerLib.IEC`
- Parse IEC implementation text through the existing scanner and code-block pipeline
- Map supported implementation blocks to typed statements through `IecAstMapper`
- Keep shared semantics free of frontend parser dependencies
- Add focused MSTest coverage for export-fixture and minimal source-text scenarios

## Out of Scope
- Broad parser refactoring
- Full IEC statement coverage
- Declaration-header metadata inference beyond current defaults

## Assumptions
- The existing `IECCode` and `IecAstMapper` pipeline is sufficient for the currently supported statement subset
- Frontend-specific source-text parsing must stay in `TranspilerLib.IEC` to avoid a circular dependency from shared semantics

## Implementation Notes
1. Do not move scanner dependencies into `TranspilerLib.Semantics`
2. Reuse `IecDeclarationExtractor` for declaration text extraction
3. Validate the new bridge with realistic export-fixture coverage

## Acceptance Criteria
- `TranspilerLib.IEC` exposes a source-text-based compilation-unit creation API
- The API returns typed statements for the currently supported implementation subset
- Focused tests verify both minimal source text and export-fixture behavior
- Existing semantics and backend tests remain buildable
