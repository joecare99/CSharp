# T0147A - Unbound Generic Nameof Package Documentation Baseline

## Parent Backlog Item

- B0113 - Define Documentation and Expected-Output Strategy for C# 14 Comparisons

## Summary

Define how the first unbound-generic-`nameof` package should be documented so the comparison examples follow one coherent explanation and boundary-note strategy.

## Package Documentation Intent

The package should present unbound generic `nameof` as a small but meaningful C# 14 improvement that removes arbitrary generic-type arguments from compile-time naming scenarios while preserving the same resulting names.

## Package-Level Documentation Structure

### 1. Package Overview

- explain what unbound generic `nameof` adds
- explain that the package focuses on exact comparisons with mostly source-level benefit
- emphasize compile-time naming clarity over runtime novelty

### 2. Shared Comparison Conventions

- mark Examples A and B as runtime-equivalent or compile-time-equivalent comparisons
- mark Example C as an exact comparison with caveat notes
- explain that the key learning artifact is clearer naming intent, not changed runtime behavior

### 3. Example Sequence

- Example A first because generic type naming is the simplest entry point
- Example B second because member access on unbound generic types expands the feature naturally
- Example C third because motivation and unsupported boundaries make more sense after the basic syntax is established

### 4. Shared Output Strategy

- keep output optional and minimal
- prefer expected-name statements over repetitive console transcripts
- document shared compile-time result once per comparison

### 5. Shared Migration Discussion

- explain when replacing bound generic `nameof` operands improves clarity immediately
- explain when teams may keep older bound forms for compatibility with older language versions
- keep unsupported partially unbound or nested-unbound forms visible as boundary notes

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
