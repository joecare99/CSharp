# Task T-DB-FRAMEWORK-002: Define Target Neutral DB Project Structure

## Parent

- Backlog Item `BI-DB-FRAMEWORK-001` - Assess and Plan Domain-Neutral DB Framework Extraction

## Objective

Define the target project boundaries, naming, namespace strategy, and dependency direction for the future neutral DB framework.

## Proposed target structure

- `Db.Core.Abstractions`
- `Db.Core`
- `Db.Provider.OleDb`
- `Db.Provider.MySql`
- `Db.Provider.SqlServer`
- `Db.Provider.DBase`

## Expected outputs

- project responsibilities
- allowed dependency graph
- compatibility adapter strategy for current `GenFree*` consumers
- naming strategy without genealogy/domain references
