# Task T-GEN-DRIVERS-006: Create GEDCOM Driver Project and Tests

## Parent

- Backlog Item `BI-GEN-DRIVERS-003` - Create GEDCOM Driver Project

## Objective

Create the new GEDCOM driver project and a matching MSTest project with initial baseline fixtures.

## Scope

- Production project candidate `..\WinAhnenNew\GedComDriver\GedComDriver.csproj`.
- Test project candidate `..\WinAhnenNew\GedComDriverTests\GedComDriverTests.csproj`.
- Project references to `BaseGenClasses`, `GenInterfaces`, and only necessary low-level abstractions.
- Initial test resources copied or referenced from existing GEDCOM samples.

## Output

- New project files added to the solution.
- Initial placeholder or minimal driver API wired to contracts.
- Test project with at least one import fixture test marked against a known resource.

## Acceptance Criteria

1. New projects build without UI dependencies.
2. Test project uses MSTest and explicit `using` directives.
3. Project target frameworks are documented and aligned with the dependency graph.
4. Initial tests fail only for intentionally unimplemented parser behavior, not for project setup errors.
