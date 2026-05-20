# T0159A - Implicit Span Conversions Package Documentation Baseline

## Parent Backlog Item

- B0113 - Define Documentation and Expected-Output Strategy for C# 14 Comparisons

## Summary

Define how the first implicit-span-conversions package should be documented so the comparison examples follow one coherent explanation and caveat strategy.

## Package Documentation Intent

The package should present implicit span conversions as a C# 14 ergonomics improvement for span-based APIs, making common array- and string-based inputs fit more naturally into `Span<T>` and `ReadOnlySpan<T>` usage patterns.

## Package-Level Documentation Structure

### 1. Package Overview

- explain what implicit span conversions add
- explain that the package focuses on API-ergonomics and behavior-aware comparisons
- emphasize natural API consumption over raw syntax novelty

### 2. Shared Comparison Conventions

- mark the examples as behavior-aware comparisons
- explain that the newer form often improves consumption shape more than visible output
- keep supported conversions and nearby limitations visible together

### 3. Example Sequence

- Example A first because array-to-span calls provide the clearest API-ergonomics story
- Example B second because string-to-`ReadOnlySpan<char>` shows a familiar text-processing scenario
- Example C third because boundaries and limitations are easiest to understand after the positive cases are established

### 4. Shared Output Strategy

- keep output minimal and deterministic
- prefer describing the shared processing outcome over long transcripts
- focus on API-shape differences and conversion support

### 5. Shared Migration Discussion

- explain when the newer conversions simplify adoption of span-oriented APIs
- explain when older explicit conversions or helper patterns remain necessary
- keep limitations such as method-group conversion exclusion visible as part of responsible adoption guidance

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
