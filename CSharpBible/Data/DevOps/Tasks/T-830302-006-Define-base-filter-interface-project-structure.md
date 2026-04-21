# Task T-830302-006 - Define base filter interface project structure

## Status

Draft

## Parent

- Backlog Item `BI-830332` - `Create pluggable input filters for initial source formats`

## Goal

Define one or more shared base interface projects for filter contracts and separate input/output filter implementation projects.

## Scope

- Define baseline project split for shared contracts vs. concrete filter implementations
- Define dependency direction to avoid cyclic references
- Define registration approach for Dependency Injection
- Define when multiple input filters may share one project and when they must be split

## Current Direction

- Shared contracts are placed in base interface project(s)
- Input filters reference base contracts and stay separate from output filters
- Output filters reference base contracts and stay separate from input filters
- CSV and `TraceCsv2realCsv` CSV may initially share an input-filter project, with a split when parsing divergence becomes relevant

## Deliverables

- Proposed project map with responsibilities per project
- Allowed dependency graph (from implementation projects to base contracts)
- Naming proposal for base contracts and implementation projects
- DI registration strategy for discovering input/output filters

## Done Criteria

- Base interface project structure is documented
- Input/output separation rules are documented
- Dependency direction rules are documented
- DI registration strategy is documented
- Split/merge criteria for multiple input filters in one project are documented
