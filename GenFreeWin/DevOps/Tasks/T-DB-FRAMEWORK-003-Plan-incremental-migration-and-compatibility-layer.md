# Task T-DB-FRAMEWORK-003: Plan Incremental Migration and Compatibility Layer

## Parent

- Backlog Item `BI-DB-FRAMEWORK-001` - Assess and Plan Domain-Neutral DB Framework Extraction

## Objective

Define the migration sequence that allows introducing the neutral DB framework without forcing a full immediate rename/refactor of all consumers.

## Planned migration stages

1. Extract neutral contracts and enums
2. Add neutral namespaces/projects
3. Keep temporary compatibility adapters in `GenFree*`
4. Move provider implementations behind provider-specific facade projects
5. Update consumers incrementally
6. Remove compatibility adapters only after consumer migration is complete

## Risks

- namespace churn across many projects
- hidden coupling to `GenFree.Data`
- legacy DAO-style semantics not mapping cleanly to modern ADO.NET abstractions
