# T0151A - Lambda Modifiers Package Documentation Baseline

## Parent Backlog Item

- B0113 - Define Documentation and Expected-Output Strategy for C# 14 Comparisons

## Summary

Define how the first lambda-modifiers package should be documented so the comparison examples follow one coherent explanation and caveat strategy.

## Package Documentation Intent

The package should present lambda parameter modifiers in C# 14 as a focused readability improvement for delegate-compatible lambdas, where the modifier remains visible but unnecessary explicit parameter types can often be omitted.

## Package-Level Documentation Structure

### 1. Package Overview

- explain what lambda modifiers add in C# 14
- explain that the package focuses on exact comparisons with source-level benefits
- emphasize unchanged delegate semantics with less ceremony

### 2. Shared Comparison Conventions

- mark Examples A and B as runtime-equivalent comparisons
- mark Example C as an exact comparison with caveat notes
- explain that the main learning artifact is signature clarity rather than different runtime behavior

### 3. Example Sequence

- Example A first because `out` is a familiar and easy-to-read modifier case
- Example B second because `ref`, `in`, or `ref readonly` show the broader advanced-signature value
- Example C third because boundaries such as `params` are most useful after the basic value proposition is established

### 4. Shared Output Strategy

- keep output minimal and deterministic
- prefer shared behavior statements over repeated transcripts
- focus attention on signature shape and equivalence notes

### 5. Shared Migration Discussion

- explain when omitting explicit types improves clarity immediately
- explain when teams may still prefer explicit types for consistency or readability in advanced lambdas
- keep the `params` requirement visible as part of responsible adoption guidance

## Metadata and Reuse Direction

The package should later support metadata fields such as:

- `Summary`
- `LearningIntent`
- `ExpectedObservation`
- `Determinism`
- `Notes`

## Done Criteria

- Package-level documentation intent is defined.
- Example order and documentation style are defined.
- Output, caveat, and migration expectations are aligned across the package.
