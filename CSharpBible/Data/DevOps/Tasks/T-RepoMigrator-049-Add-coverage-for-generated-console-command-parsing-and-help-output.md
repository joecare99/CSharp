# Task T-RepoMigrator-049 - Add coverage for generated command-line parsing and help output

## Status

Draft

## Parent

- Backlog Item `BI-RepoMigrator-013` - `Create reusable CommandlineHelper infrastructure`
- Task `T-RepoMigrator-047` - `Implement reusable command-line runtime and source generator`
- Task `T-RepoMigrator-048` - `Adopt generated command parsing in initial RepoMigrator console tools`

## Goal

Add test coverage that verifies generated parsing, inferred versus declared metadata behavior, and localized help output across the reusable helper infrastructure and its initial adopters.

## Scope

- Add focused tests for requiredness inference and attribute override precedence
- Add focused tests for default-value inference and attribute override precedence
- Add tests for generated help and usage output including resource-based text resolution
- Update integration-style program tests for the migrated console tools
- Validate the expected build behavior for generator-driven helper code in the workspace

## Acceptance Criteria

- Tests cover requiredness inference and explicit override behavior
- Tests cover default-value inference and explicit override behavior
- Tests cover generated usage/help output and resource-based command text resolution
- Program-level tests continue to validate the migrated console tool entry points
- Relevant test execution succeeds after the shared infrastructure is adopted

## Dependencies

- `T-RepoMigrator-047` - `Implement reusable command-line runtime and source generator`
- `T-RepoMigrator-048` - `Adopt generated command parsing in initial RepoMigrator console tools`

## Test Design

### 1. Test goal

This task defines the test and validation coverage required for the first implementation slice of `CommandlineHelper` and for the first three adopter tools.

The goal is to verify four things together:

1. generated parsing behaves correctly for supported declarations
2. inferred metadata and explicit overrides interact as specified
3. help and usage output respect resource-based user text rules
4. migrated consumer tools preserve their existing entry-point success and failure behavior

### 2. Test layers

Coverage should be split into three layers:

1. runtime-level tests
   - verify small public helper contracts where direct unit testing is meaningful
2. generator-focused tests
   - verify emitted behavior and diagnostics from attributed options declarations
3. adopter integration tests
   - verify that migrated tools still return the expected exit codes and command-line outcomes

This split keeps failures easier to localize and avoids relying only on broad integration coverage.

### 3. Runtime-level test coverage

Runtime-level tests should stay small because most behavior lives in generated code.

Recommended coverage:

- `CommandParseResult<TOptions>` state combinations
- any shared helper used by generated code for resource lookup or repeated formatting logic
- attribute construction guards if the runtime library enforces argument constraints directly

These tests should not attempt to simulate end-to-end parsing when that behavior is generated elsewhere.

### 4. Generator-focused test coverage

Generator-focused tests are the core of this task.

They should validate generated behavior for representative attributed options classes.

Recommended fixture categories:

#### 4.1 Requiredness inference

Add coverage for:

- `required string` without initializer => required
- non-nullable reference type without initializer => required when supported by the approved inference model
- nullable reference type => optional
- nullable value type => optional
- property with supported initializer => optional
- boolean flag => optional by default
- first positional argument at position `0` uses the same requiredness inference rules
- explicit `Required = true` overrides otherwise optional property shape
- explicit `Required = false` overrides otherwise required property shape

#### 4.2 Default-value inference

Add coverage for:

- string literal initializer
- numeric literal initializer
- boolean literal initializer
- enum literal initializer
- nullable default behavior where supported
- explicit attribute `DefaultValue` overriding inferred initializer value
- first positional argument at position `0` using the same default inference and override rules

#### 4.3 Type conversion and binding

Add coverage for:

- `string`
- `int`
- `long`
- enum
- nullable variants where supported
- boolean flags with presence-based semantics
- first positional argument binding at position `0`
- help-token handling with `--help` and `-h`

#### 4.4 Collection-typed option behavior

If collection-typed options are included in the first implemented slice, add explicit coverage for:

- repeated named option occurrences populating `T[]`, `IList<T>`, `IReadOnlyList<T>`, or supported collection targets
- repeated string values including paths with spaces as already-tokenized arguments
- `Required = true` on a collection meaning at least one value is required
- unsupported multi-value-per-single-occurrence forms remaining rejected or unsupported by design

If collection-typed options are not yet implemented in `T-RepoMigrator-047`, this section should become a documented follow-up rather than silent missing coverage.

#### 4.5 Diagnostics

Add coverage for generator diagnostics such as:

- duplicate long option names
- duplicate short aliases
- duplicate positional argument positions
- unsupported property type
- `CommandFlagAttribute` on non-boolean property
- invalid position other than `0` for the first slice
- unresolved resource property
- invalid explicit default value type

Each diagnostic test should assert the expected diagnostic ID and target declaration location where practical.

### 5. Help and usage output coverage

Generated help output should be validated with focused golden-style expectations that are stable enough for maintenance.

Recommended coverage:

- usage line includes command name
- short description is resolved from resources
- options list includes declared long names and short aliases where present
- requiredness markers or wording appear consistently
- default value display appears where applicable
- extended help text is resolved from resources
- first positional argument is represented clearly in usage/help output when declared

The tests should verify semantic content first and exact whitespace alignment only where the formatting contract is intentionally strict.

### 6. Resource-based text coverage

Because localization is a core requirement, add dedicated tests for resource resolution behavior.

Recommended coverage:

- command description resolved from command `ResourceType`
- command help text resolved from command `ResourceType`
- option description resolved from option `ResourceType`
- option description falling back to command `ResourceType` when allowed by the spec
- unresolved resource names producing build diagnostics rather than silent omissions

The first slice does not need exhaustive culture-switching tests if the helper relies on standard .NET resource mechanics, but it should verify that the generated code uses the configured resource members correctly.

### 7. Consumer integration coverage

The existing program-level tests for the three adopter tools should be retained and updated to reflect the generated parser path.

Per adopter, preserve tests for:

- invalid arguments return exit code `1`
- help request returns exit code `0`
- runtime failure returns exit code `2`

Additional adopter-specific tests should cover:

- expected successful parse path for representative valid arguments
- generated help path if a stable observation point exists
- resource-driven user text references indirectly through parser/help behavior

For `ArchiveSmokeTest`, coverage should explicitly track the chosen handling of repeated `--extension` values.

### 8. Build validation

This task must also validate that the generator behaves correctly in normal builds.

Recommended validation:

- workspace build succeeds after helper adoption
- generator outputs compile cleanly in adopter projects
- no unexpected analyzer or generator diagnostics appear in valid consumer projects
- intentionally invalid test fixtures surface the expected diagnostics

### 9. Recommended test organization

Recommended structure:

- helper runtime and generator tests in a dedicated test project or dedicated test areas within the existing test project
- adopter integration tests remain near existing RepoMigrator tool tests
- fixture source files for generator tests should stay small and purpose-specific

If generator testing infrastructure requires an additional test project, that should be introduced in the most isolated way practical.

### 10. Definition of done for coverage

This task should be considered complete when:

- inference, override, conversion, and diagnostic behaviors are covered for the approved first-slice declaration model
- help and usage output have focused coverage including resource-based text resolution
- adopter integration tests pass with the generated parser path
- build validation confirms that generator output compiles and behaves as expected
- unsupported or deferred features are either covered explicitly as unsupported or tracked as follow-up work

## Deliverables

- Focused tests for `CommandlineHelper` generated parsing behavior
- Focused tests for diagnostics emitted by `CommandlineHelper.Generators`
- Help and usage output tests including resource-based text checks
- Updated adopter integration tests for `GitBranchSplitter`, `PipelinedMigration`, and `ArchiveSmokeTest`
- Build validation evidence for generator-driven consumer compilation

## Validation Evidence

- The coverage task now distinguishes runtime, generator, and adopter integration test layers
- Requiredness, default inference, diagnostics, localization, and help output all have explicit coverage goals
- Collection-typed option behavior and other first-slice edge cases are called out so they cannot disappear untested by accident
